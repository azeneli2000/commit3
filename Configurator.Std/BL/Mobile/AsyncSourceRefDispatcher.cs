using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Mobile
{
    public class AsyncSourceRefDispatcher : AsyncDispatcher<SourceRef>
    {

        public AsyncSourceRefDispatcher(IMessageCenterService msgCtrSvc, ILoggerService logSvc) : base(msgCtrSvc, logSvc)
        {
        }

        public async Task<SourceRef> RequestSourceRef(string deviceID)
        {
            try
            {
                Send(MobileServiceHelper.NewMobileSourceRef(deviceID));
                return await WaitForResults();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected override void OnUMSMessageReceived(MCMessage msg)
        {
            if (msg.Message == UMSMessageExtendedCodes.messageTypeSourceRef)
            {
                var dev = msg.Options.Find((opt) => opt.Key.Equals("DEVICEID"));
                var sourceReference = msg.Options.Find((opt) => opt.Key.Equals("SOURCEREFERENCE"));

                Notify(new SourceRef
                {
                    DeviceID = dev.Value.ToString(),
                    SourceReference = sourceReference.Value.ToString()
                });
            }
        }
    }
}
