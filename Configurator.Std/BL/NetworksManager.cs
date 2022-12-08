using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Configurator.Std.Exceptions;
using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model.Integration.Telligence;

namespace Configurator.Std.BL
{
   public class NetworksManager : DalManagerBase<Network>, INetworksManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;

      public NetworksManager(DigistatDBContext context, ILoggerService loggerService,IMessageCenterManager msgCtrMgr)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrMgr;
      }

      #endregion

      #region Data reading functions


      public Network GetWithBeds(int id)
      {
         Network result = null;
         try
         {
            IQueryable<Network> repository = mobjDbContext.Set<Network>().Include(x => x.NetworkBedLinks);
            result = repository.Where(x => x.Id == id).SingleOrDefault();
            return result;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Network GetWithBeds with id {0} from DB", id);
            throw new Exception(string.Format("Error reading Network  GetWithBedswith id {0} from DB", id), e);
         }
      }

      public Network UpdateNetwork(Network objNetwork)
      {
         //Network objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();


            var netBedRepo = mobjDbContext.Set<NetworkBedLink>();
            var oldNetBedLink = netBedRepo.Where(p=>p.IdNetwork==objNetwork.Id).ToList();
            netBedRepo.RemoveRange(oldNetBedLink);

            //make sure all networkbedlinks have correct networkID 


            netBedRepo.AddRange(objNetwork.NetworkBedLinks.ToList());
            mobjDbContext.SaveChanges();


            mobjDbContext.Update(objNetwork);
            
            mobjDbContext.SaveChanges();
            mobjMsgCtrMgr.SendNetworkEdited(objNetwork);
            mobjDbContext.CommitTransaction();
            return objNetwork;
            //return objNetworkDB;
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
          
            string errMsg = "Error on CreateNetwork";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         //return objRet;
      }

      public Network CreateNetwork(Network objNetwork)
      {
         Network objRet = null;
         try
         {
            //Check hostname already exists
            int intCount = mobjDbContext.Set<Network>().Where(p => p.HostName.Trim().ToUpper() == objNetwork.HostName.Trim().ToUpper()).Count();
            if (intCount == 0)
            {
               mobjDbContext.BeginTransaction();
               //Add entry in network
               mobjDbContext.Set<Network>().Add(objNetwork);
               mobjDbContext.SaveChanges();

               //set networkid 
               if(objNetwork.NetworkBedLinks!=null)
               {
                  foreach(NetworkBedLink objnet in objNetwork.NetworkBedLinks)
                  {
                     objnet.IdNetwork = objNetwork.Id;
                     objnet.Network = null;
                  }
                  //Add entry in networkbedlinks
                  var netBedRepo = mobjDbContext.Set<NetworkBedLink>();
                  netBedRepo.AddRange(objNetwork.NetworkBedLinks.ToList());
                  mobjDbContext.SaveChanges();
               }



               mobjMsgCtrMgr.SendNetworkEdited(objNetwork);

               mobjDbContext.CommitTransaction();
               objRet = objNetwork;
            }
            else
            {
               throw new NetworkCreationException(string.Format("Unable to create network {0}: Hostname already exists.", objNetwork.HostName));
            }


         }
         catch (Exception e)
         {
            if(e is NetworkCreationException)
            {
               throw;
            }
            mobjDbContext.RollbackTransaction();
            string errMsg = "Error on CreateNetwork";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }

      //public Permission Get(string guid, IEnumerable<System.Linq.Expressions.Expression<Func<Network, object>>> includePredicates = null) {
      public Network Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for Network with id {0}", id);

         Network result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Network> repository = mobjDbContext.Set<Network>().Include(p=>p.DefaultLocation).Include(p => p.NetworkBedLinks)
               .ThenInclude(c => c.Bed).ThenInclude(d=>d.Location);

            //if (includePredicates != null && includePredicates.Count() > 0) {
            //   includePredicates.ToList().ForEach(x => repository = repository.Include(x));
            //}

            //TODO Trace
            mobjLoggerService.Info("Reading Network with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("Network with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Network with id {0} from DB", id);
            throw new Exception(string.Format("Error reading Network with id {0} from DB", id), e);
         }

         finally
         {
            //disposeContext();
         }


         return result;

      }

      public IEnumerable<Network> GetList()
      {

         //TODO Trace
         mobjLoggerService.Info("Executing GetList for Network");

         IQueryable<Network> result = null;

         try
         {
            //Set detached

            result = mobjDbContext.Set<Network>().Include(p => p.DefaultLocation).Include(p => p.NetworkBedLinks)
               .ThenInclude(c => c.Bed).ThenInclude(d => d.Location);

            //TODO Trace
            mobjLoggerService.Info("Reading Network List from DB");

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Network List from DB");
            throw new Exception("Error reading Network List from DB");
         }

         finally
         {
            //disposeContext();
         }


         return result;

      }

      #endregion

      #region Data Writing functions

      //public Network Create(Network entity)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Creating new Network");

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var userRepository = mobjDbContext.Set<Network>();


      //      //Create new record
      //      userRepository.Add(entity);

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      //TODO Trace
      //      mobjLoggerService.Info("Network succesfully created with id {1}", entity.Id);

      //      return entity;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, "Error creating new network");
      //      string message = string.Format("Error creating new network");
      //      throw new Exception(message, e);
      //   }

      //}

      //public Network Update(Network entity)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Updating Network with id {0}", entity.Id);

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var repository = mobjDbContext.Set<Network>();

      //      //Network loadedEntity = repository.SingleOrDefault(x => x.Id == entity.Id);
      //      //if (loadedEntity == null)
      //      //{
      //      //   throw new Exception(string.Format("Unable to update network with id {0}; network not found.", entity.Id));
      //      //}

      //      repository.Attach(entity);

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      //TODO Trace
      //      mobjLoggerService.Info("Network with id {0} updated succesfully", entity.Id);

      //      return entity;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, "Error updating network with id {0}", entity.Id);
      //      string message = string.Format("Error updating network with id {0}", entity.Id);
      //      throw new Exception(message, e);
      //   }

      //}

      /// <summary>
      /// Replace all NetworkbedLink for a specific locations
      /// </summary>
      /// <param name="locationID"></param>
      /// <param name="objLink"></param>
      /// <returns></returns>
      public List<NetworkBedLink> UpdateNetworkBedLinkForLocation(IEnumerable<int?> locationIDS , int networkID,  IEnumerable<NetworkBedLink> objLink)
      {
         List<NetworkBedLink> objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();
            foreach(int? locID in locationIDS)
            {
               if(locID.HasValue)
               {
                  var repository = mobjDbContext.Set<NetworkBedLink>()
               .FromSqlRaw(@"select n.* from Network_Bed_Link n
               inner join BED b on n.ln_IDBed = b.IDBed
               where b.LocationRef = @location AND n.ln_IDNetwork = @networkID", locID.Value, networkID);
               }
            }
            mobjDbContext.Set<NetworkBedLink>().AddRange(objLink);

            mobjDbContext.CommitTransaction();

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error on UpdateNetworkBedLinkForLocation ");
            string message = string.Format("Error on UpdateNetworkBedLinkForLocation");
            throw new Exception(message, e);
         }
         return objRet;
      }

      public void Delete(int entityId)
      {

         //TODO Trace
         mobjLoggerService.Info("Deleting Network with id {0}", entityId);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            var repository = mobjDbContext.Set<Network>();

            Network loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update network with id {0}; network not found.", entityId));
            }

            if (loadedEntity.Type == (short)NetworkTypeEnum.TelligenceSS)
            {
               var objTlgDeviceRepo = mobjDbContext.Set<TelligenceDevice>();
               var objTlgDeviceItem = objTlgDeviceRepo.Where(p => p.tl_NetworkID == entityId).FirstOrDefault();
               if (objTlgDeviceItem!=null)
               {
                  objTlgDeviceItem.tl_NetworkID = 0;
                  objTlgDeviceRepo.Update(objTlgDeviceItem);
               }
            }
            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            if(executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Network with id {0} removed succesfully", loadedEntity.Id);

         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing network with id {0}", entityId);
            string message = string.Format("Error removing network with id {0}", entityId);
            throw new Exception(message, e);
         }
         
      }

      #endregion
   }
}
