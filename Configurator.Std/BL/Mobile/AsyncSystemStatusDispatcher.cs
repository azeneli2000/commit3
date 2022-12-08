
using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Mobile
{
   public class AsyncSystemStatusDispatcher : AsyncDispatcher<SystemStatus>
   {

      public AsyncSystemStatusDispatcher(IMessageCenterService msgCtrSvc, ILoggerService logSvc) : base(msgCtrSvc, logSvc)
      {
      }

      public async Task<SystemStatus> RequestSystemStatus(string deviceID)
      {
         try
         {
            Send(MobileServiceHelper.NewMobileSystemStatus(deviceID));
            return await WaitForResults();
         }
         catch (Exception e)
         {
            throw e;
         }
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         if (msg.Message == UMSMessageExtendedCodes.messageTypeSystemStatus)
         {
            var dev = msg.Options.Find((opt) => opt.Key.Equals("DEVICEID"));
            var info = msg.Options.Find((opt) => opt.Key.Equals("INFO"));

            Notify(new SystemStatus
            {
               DeviceID = dev.Value.ToString(),
               Info = info.Value.ToString()
            });
         }
      }
   }
}
