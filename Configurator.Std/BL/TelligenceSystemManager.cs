using System;
using System.Collections.Generic;
using System.Linq;

using Configurator.Std.BL.Hubs;
using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model.DAS3Plus;

namespace Configurator.Std.BL
{
   public class TelligenceSystemManager : DalManagerBase<TelligenceSystem>, ITelligenceSystemManager
   {
      
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDictionaryService mobjDicSvc;
      
      #region Costructors

      public TelligenceSystemManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService,IDictionaryService dicSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjDicSvc = dicSvc;
      }

      #endregion

 
      public TelligenceSystem Get(int id)
      {
         TelligenceSystem result = null;

         try
         {
            IQueryable<TelligenceSystem> repository = mobjDbContext.Set<TelligenceSystem>().Include<TelligenceSystem>("ty_ts_");
            result = repository.Where(x => x.ty_ID == id).SingleOrDefault();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, string.Format("Error reading TelligenceSystem with id {0} from DB", id));
            throw new Exception(string.Format("Error reading TelligenceSystem with id {0} from DB", id), e);
         }


         return result;

      }


      public TelligenceSystem GetByHostID(int hostid)
      {
         TelligenceSystem result = null;

         try
         {
            IQueryable<TelligenceSystem> repository = mobjDbContext.Set<TelligenceSystem>();
            result = repository.Where(x => x.ty_hostID == hostid).FirstOrDefault();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, string.Format("Error reading TelligenceSystem with HOSTid {0} from DB", hostid));
            throw new Exception(string.Format("Error reading TelligenceSystem with HOSTid {0} from DB", hostid), e);
         }


         return result;
      }



      public IEnumerable<TelligenceSystem> GetAllSystems()
      {

         //TODO Trace

         List<TelligenceSystem> result;

         try
         {
            IQueryable<TelligenceSystem> repository = mobjDbContext.Set<TelligenceSystem>().Include<TelligenceSystem>("ty_ts_");
            result = repository.ToList();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to GetAllSystems");
            string message = string.Format("Unable to GetAllSystems");
            throw new Exception(message, e);
         }


         return result;

      }


      public new TelligenceSystem Update(TelligenceSystem tlSys)
      {
         TelligenceSystem objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();

            //Update all portservers related to this telligence System
            var objPSRepo = mobjDbContext.Set<PortServer>();
            var objTLRepo = mobjDbContext.Set<TelligenceDevice>();

            var obJQ = from a in objTLRepo
                        join b in objPSRepo on a.tl_psv_ID equals b.ID
                        where a.tl_ty_ID == tlSys.ty_ID
                        select b.ID;
            List<int> objPSIds = obJQ.ToList();


            var objPSList = objPSRepo.Where(p => objPSIds.Contains(p.ID)).ToList();


            foreach(PortServer objPs in objPSList)
            {
               objPs.EncryptionKey = tlSys.ty_MDIEncKey;
               objPs.FirstPort = tlSys.ty_MDIPort.HasValue ? Convert.ToInt32(tlSys.ty_MDIPort.Value) : 0;
            }

            //Update Telligence Systems
            mobjDbContext.Set<TelligenceSystem>().Update(tlSys);
            mobjDbContext.SaveChanges();
            
            mobjDbContext.CommitTransaction();
            objRet = tlSys;
         }
         catch(Exception e)
         {
            mobjDbContext.RollbackTransaction();
            string errMsg = "Error on creating TelligenceSystem";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }

      public string Delete(int id)
      {
         string strRet = string.Empty;
         try
         {
            var tlDeviceRepo = mobjDbContext.Set<TelligenceDevice>();
            int intNDevice = tlDeviceRepo.Where(p => p.tl_ty_.ty_ID == id).Count();
            if (intNDevice > 0)
            {
               strRet = mobjDicSvc.XLate("Cannot delete a Telligence System bound to Telligence Device. Remove Telligence Device first.");
            }
            else
            {
               var tlSystemRepo = mobjDbContext.Set<TelligenceSystem>();
               TelligenceSystem tlSystemItem = tlSystemRepo.Where(p => p.ty_ID  == id).FirstOrDefault();
               tlSystemRepo.Remove(tlSystemItem);
               mobjDbContext.SaveChanges();
            }

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, string.Format("Error deleting Telligence System {0}", id));
            string message = string.Format("Error deleting Telligence System {0}", id);
            throw new Exception(message, e);
         }
         return strRet;
      }
      //public TelligenceSystem CreateSystem(TelligenceSystem tlSys)
      //{
      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var tlserverRepo = mobjDbContext.Set<TelligenceSystem>();

      //      //Prevent duplications
      //      TelligenceSystem loadedTlSrv = tlserverRepo.SingleOrDefault(x => x!=null && x.ty_ID==tlSys.ty_ID);
      //      if (loadedTlSrv != null)
      //      {
      //         throw new Exception(string.Format("Unable to create Telligence System {0}; URL already exists.", tlSys.ty_telGUID));
      //      }



      //      //Create new record
      //      tlserverRepo.Add(tlSys);

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      return tlSys;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, string.Format("Error creating Telligence System {0}", tlSys.ty_telGUID));
      //      string message = string.Format("Error creating Telligence System {0}", tlSys.ty_telGUID);
      //      throw new Exception(message, e);
      //   }
      //}

      //public IEnumerable<TelligenceSystem> GetSystemAll(int pageNumber = 0, int pageSize = 0)
      //{
      //   List<TelligenceSystem> result;


      //   try
      //   {
      //      IQueryable<TelligenceSystem> repository = mobjDbContext.Set<TelligenceSystem>();

      //      if (pageNumber > 0)
      //      {
      //         repository = repository.Skip((pageNumber - 1) * pageSize);
      //      }

      //      if (pageSize > 0)
      //      {
      //         repository = repository.Take(pageSize);
      //      }

      //      result = repository.ToList();


      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to read all TelligenceSystem from DB");
      //      string message = string.Format("Unable to read all TelligenceSystem from DB");
      //      throw new Exception(message, e);
      //   }

      //   return result;
      //}

      //public TelligenceSystem UpdateSystem(TelligenceSystem tserver)
      //{
      //   throw new NotImplementedException();
      //}

   }
}
