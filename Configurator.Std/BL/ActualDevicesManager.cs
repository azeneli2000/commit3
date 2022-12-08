using System;
using System.Collections.Generic;
using System.Text;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using Configurator.Std.BL.Hubs;

namespace Configurator.Std.BL
{
   public class ActualDevicesManager : DalManagerBase<ActualDevice>, IActualDevicesManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;

      public ActualDevicesManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrMgr)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrMgr;
      }

      #endregion


      public void Delete(int entityId)
      {
         //TODO Trace
         mobjLoggerService.Info("Deleting ActualDevice with id {0}", entityId);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            var repository = mobjDbContext.Set<ActualDevice>();

            ActualDevice loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to delete ActualDevice with id {0}; element not found.", entityId));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            if (executeClose) { mobjDbContext.CommitTransaction(); }

            //TODO Trace
            mobjLoggerService.Info("ActualDevice with id {0} removed succesfully", loadedEntity.Id);

         }
         catch (Exception e)
         {
            if (executeClose) { mobjDbContext.RollbackTransaction(); }
            mobjLoggerService.ErrorException(e, "Error removing ActualDevice with id {0}", entityId);
            string message = string.Format("Error removing ActualDevice with id {0}", entityId);
            throw new Exception(message, e);
         }
      }

      /// <summary>
      /// Update LABEL (only!) for an ActualDevice
      /// </summary>
      /// <param name="ad"></param>
      /// <returns></returns>
      public new ActualDevice Update(ActualDevice ad)
      {

         ActualDevice result = null;

         try
         {
            //Check if actualdevice exists
            if (ad != null)
            {
               IQueryable<ActualDevice> repository = mobjDbContext.Set<ActualDevice>();
               ActualDevice objOldDevice = repository.Where(p => p.Id == ad.Id).FirstOrDefault();
               if (objOldDevice!=null)
               {
                  objOldDevice.Label = ad.Label;
                  mobjDbContext.SaveChanges();
                  //Send message to Digistat Network
                  mobjMsgCtrMgr.SendActualDeviceUpdated(Digistat.FrameworkStd.MessageCenter.DestinationHostCodes.All,
                     Digistat.FrameworkStd.MessageCenter.ApplicationCodes.All,ad);
                  result = objOldDevice;
               }
               else
               {
                  throw new Exception(string.Format("Unable to delete ActualDevice with id {0}; element not found.", ad.Id));
               }
            }
            else
            {
               throw new Exception(string.Format("Unable to delete ActualDevice: element is null", ad.Id));
            }

            
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error updating ActualDevice with id {0} ", ad.Id);
            throw new Exception(string.Format("Error updating ActualDevice with id {0} ", ad.Id), e);
         }

         return result;
      }

      public ActualDevice Get(int id)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for Actual Device with id {0}", id);

         ActualDevice result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<ActualDevice> repository = mobjDbContext.Set<ActualDevice>();

            //TODO Trace
            mobjLoggerService.Info("Reading ActualDevice with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("ActualDevice with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ActualDevice with id {0} from DB", id);
            throw new Exception(string.Format("Error reading ActualDevice with id {0} from DB", id), e);
         }

         return result;
      }

      public IEnumerable<ActualDevice> GetByDeviceType(int deviceType) {

         List<ActualDevice> result;

         try
         {
            IQueryable<ActualDevice> repository = mobjDbContext.Set<ActualDevice>();

            result = repository.Where(x => x.DeviceType == deviceType)
                               .GroupBy(g => new { g.DeviceType, g.Name })
                               //.Distinct()
                               //.Cast<ActualDevice>()
                               .Select(s => new ActualDevice()
                               {
                                  DeviceType = s.Key.DeviceType,
                                  Name = s.Key.Name
                               })
                               .OrderBy(o => o.Name)
                               .ToList();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read Actual Devices with device type {0} from DB", deviceType);
            string message = string.Format("Unable to read Actual Devices with device type {0} from DB", deviceType);
            throw new Exception(message, e);
         }


         return result;

      }

      public IEnumerable<ActualDevice> GetByDeviceTypeDeviceName(int deviceType, string deviceName)
      {
         List<ActualDevice> result;

         try
         {
            IQueryable<ActualDevice> repository = mobjDbContext.Set<ActualDevice>();

            result = repository.Where(x => x.DeviceType == deviceType && x.Name == deviceName)
                     .Distinct()
                     .OrderBy(o => o.SerialNumber)
                     .ToList();

            //result = repository.ToList();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read Actual Devices with device type {0} and name {1} from DB", deviceType, deviceName);
            string message = string.Format("Unable to read Actual Devices with device type {0} and name {1} from DB", deviceType, deviceName);
            throw new Exception(message, e);
         }

         return result;
      }

   }
}
