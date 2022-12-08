
using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Mobile
{
   public class AsyncCommunicationDispatcher : AsyncDispatcher<bool>
   {
      public AsyncCommunicationDispatcher(IMessageCenterService msgCtrSvc, ILoggerService logSvc) : base(msgCtrSvc,logSvc)
      {
      }
      public async Task<bool> SendCommunication(Communication objCommunication)
      {
         try
         {
            Send(MobileServiceHelper.NewCommunication(objCommunication));
            return await WaitForResults();
         }
         catch (Exception e)
         {
            throw e;
         }
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         if (msg.Message == UMSMessageExtendedCodes.messageTypeCommunication)
         {
            var data = msg.Options.Find((opt) => opt.Key.Equals("SUCCESS"));
            var result = !string.IsNullOrWhiteSpace(data.Value.ToString()) && data.Value.ToString() == "true"; 
            Notify(result);
         }
      }
   }
}
