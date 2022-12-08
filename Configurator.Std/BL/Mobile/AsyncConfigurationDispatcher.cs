using System;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;

namespace Configurator.Std.BL.Mobile
{
   public class AsyncConfigurationDispatcher : AsyncDispatcher<bool>
   {
      public AsyncConfigurationDispatcher(IMessageCenterService msgCtrSvc, ILoggerService logSvc, int timeoutms = 60000) : base(msgCtrSvc, logSvc, timeoutms)
      {
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         if (msg.Message == Constants.MOBILE_CONFIGURED)
         {
            var data = msg.Options.Find((opt) => opt.Key.Equals("SUCCESS"));
            var result = !string.IsNullOrWhiteSpace(data.Value.ToString()) && data.Value.ToString() == "true";
            Notify(result);
         }
      }

      public Task<bool> SendConfiguration(string url, int port, bool launcher, string deviceId = null)
      {
         var message = MobileServiceHelper.NewSendConfiguration(url, port, launcher, deviceId);
         Send(message);
         return WaitForResults();
      }
      
   }

   public class AsyncConfigurationRetriever : AsyncDispatcher<MobileConfig>
   {
      public AsyncConfigurationRetriever(IMessageCenterService msgCtrSvc,ILoggerService logSvc, int timeoutms = 5000) : base(msgCtrSvc,logSvc, timeoutms)
      {
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         if (msg.Message == Constants.MOBILE_SET_CONFIGURATION)
         {
            try
            {
               var server = msg.Options.Find((opt) => opt.Key.Equals(Constants.SERVER_ADDRESS));
               var port = msg.Options.Find((opt) => opt.Key.Equals(Constants.SERVER_PORT));
               var launcher = msg.Options.Find((opt) => opt.Key.Equals(Constants.DIGISTAT_LAUNCHER));
               var deviceId = msg.Options.Find((opt) => opt.Key.Equals("DEVICEID"));

               Notify(new MobileConfig
               {
                  DeviceID = deviceId.Value.ToString(),
                  DigistatLauncher = launcher.Value.ToString().ToLower() == "true",
                  ServerPort = Int32.Parse(port.Value.ToString()),
                  ServerAddress = server.Value.ToString()
               });
            } 
            catch (Exception)
            {
               Notify(null);
            }
         }
      }

      public Task<MobileConfig> GetConfiguration(string deviceID)
      {
         var message = MobileServiceHelper.NewGetConfiguration(deviceID);
         Send(message);
         return WaitForResults();
      }
   }
}
