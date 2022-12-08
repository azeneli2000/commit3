using System;
using System.Threading.Tasks;
using Configurator.Std.BL.Mobile;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;

namespace Configurator.Std.BL
{
   public class MobileManager : IMobileManager
   {
      private readonly IMessageCenterService mobjMessageCenter;
      private readonly ILoggerService Log;

      public MobileManager(
         IMessageCenterService msgcenter,
         ILoggerService logSvc
      )
      {
         mobjMessageCenter = msgcenter;
         Log = logSvc;
      }

      public bool Disconnect(string deviceID)
      {
         try
         {
            mobjMessageCenter.SendMessage(MobileServiceHelper.NewMobileDisconnect(deviceID));
            return true;
         }
         catch (Exception e)
         {
            Log.ErrorException(e, "Error disconnecting device {0}", deviceID);
            return false;
         }
      }

      public bool RequestLogs(string deviceID, DateTime? date)
      {
         try
         {
            mobjMessageCenter.SendMessage(MobileServiceHelper.NewGetLogFile(deviceID, date));
            return true;
         }
         catch (Exception e)
         {
            Log.ErrorException(e, "Error requesting logs for device {0}", deviceID);
            return false;
         }
      }

      async public Task<MobileConfig> GetConfiguration(string deviceID)
      {
         using (var mgr = new AsyncConfigurationRetriever(mobjMessageCenter, Log))
         {
            return await mgr.GetConfiguration(deviceID);
         }
      }

      async public Task<bool> SetConfiguration(MobileConfig config)
      {
         using (var mgr = new AsyncConfigurationDispatcher(mobjMessageCenter, Log))
         {
            return await mgr.SendConfiguration(config.ServerAddress, config.ServerPort, config.DigistatLauncher, config.DeviceID);
         }
      }

      async public Task<SystemStatus> RequestSystemStatus(string deviceID)
      {
         using (var mgr = new AsyncSystemStatusDispatcher(mobjMessageCenter, Log))
         {
            return await mgr.RequestSystemStatus(deviceID);
         }
      }

        async public Task<SourceRef> RequestSourceRef(string deviceID)
        {
            using (var mgr = new AsyncSourceRefDispatcher(mobjMessageCenter, Log))
            {
                return await mgr.RequestSourceRef(deviceID);
            }
        }

        async public Task<bool> SendCommunication(Communication communication)
      {
         using (var mgr = new AsyncCommunicationDispatcher(mobjMessageCenter, Log))
         {
            return await mgr.SendCommunication(communication);
         }
      }
   }
}
