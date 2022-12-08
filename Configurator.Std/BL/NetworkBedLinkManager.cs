using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public class NetworkBedLinkManager : DalManagerBase<Bed>, INetworkBedLinkManager
   {

      #region Costructors

      private readonly IBedsManager mobjBedMgr;
      private readonly IMessageCenterService mobjMsgCtrSvc;

      public NetworkBedLinkManager(DigistatDBContext context, ILoggerService loggerService,IBedsManager bedMgr,IMessageCenterService msgCtrSvc)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjBedMgr = bedMgr;
         mobjMsgCtrSvc = msgCtrSvc;
      }


      /// <summary>
      /// Update networkbedlinks. Removes all existing links for a specific location then writes the new entries.
      /// NOTE : this function acts only on beds for locations contained in objList collection. NetworkBedLinks bound to 
      /// other locations will not be modified.
      /// </summary>
      /// <param name="objList"></param>
      /// <param name="idNetwork"></param>
      /// <returns></returns>
      public bool UpdateNetworkBedLinkForLocation(List<Bed> objList,int idNetwork)
      {
         bool bolRet = false;
         try
         {
            var repository = mobjDbContext.Set<NetworkBedLink>();
            List<NetworkBedLink> objNBLList = new List<NetworkBedLink>();
            foreach(Bed b in objList)
            {
               NetworkBedLink objNBL = new NetworkBedLink();
               objNBL.IdBed = b.Id;
               objNBL.IdNetwork = idNetwork;
               objNBL.Index = Convert.ToInt16(b.Index);
               objNBLList.Add(objNBL);
            }

            IEnumerable<int?> objLocationIds = objList.Select(p => p.IdLocation).Distinct();
            if(objLocationIds!=null && objLocationIds.Count() > 0)
            {
               mobjDbContext.BeginTransaction();
               foreach(int? idLocation in objLocationIds)
               {
                  if(idLocation.HasValue)
                  {
                     List<Bed> objBedList = mobjBedMgr.Find(p => p.IdLocation == idLocation.Value).ToList();
                    
                     if(objBedList!=null && objBedList.Count()>0)
                     {
                        var bedIds = objBedList.Select(g => g.Id);
                        var objToRemove = repository.Where(p => p.IdNetwork==idNetwork && bedIds.Contains(p.IdBed)).ToList();
                        repository.RemoveRange(objToRemove);
                     }
                  }
               }
               repository.AddRange(objNBLList);
               mobjDbContext.SaveChanges();
               
               mobjDbContext.CommitTransaction();
               bolRet = true;
            }
         }
         catch(Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error UpdateNetworkBedLinkForLocation");
            string message = string.Format("Error UpdateNetworkBedLinkForLocation");
            throw new Exception(message, e);
         }
         return bolRet;
      }

      #endregion

   }
}
