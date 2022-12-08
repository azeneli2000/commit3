using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Mobile
{
   class AsyncServerStatusDispatcher : AsyncDispatcher<ServerStatus>
   {
      public AsyncServerStatusDispatcher(IMessageCenterService msgCtrSvc, ILoggerService logSvc) : base(msgCtrSvc, logSvc, 5000)
      {

      }

      public async Task<ServerStatus> RequestServerStatus()
      {
         try
         {
            var now = DateTime.UtcNow;

            Send(MobileServiceHelper.NewMobileServerStatus());
            var ret = await WaitForResults();
            ret.RoundTrip = (int)(DateTime.UtcNow - now).TotalMilliseconds;

            return ret;
         }
         catch (Exception e)
         {
            throw e;
         }
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         if (msg.Message == Constants.MOBILE_SERVER_STATUS)
         {
            Notify(new ServerStatus
            {
               Status = "OK"
            });
         }
      }
   }
}
