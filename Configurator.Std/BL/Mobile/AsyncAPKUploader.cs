using System;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;

namespace Configurator.Std.BL.Mobile
{
   public class AsyncAPKUploader : AsyncDispatcher<bool>
   {
      public AsyncAPKUploader(IMessageCenterService msgCtrSvc, ILoggerService logSvc, string session, int timeoutms = 60000)
         : base(msgCtrSvc, logSvc, session, timeoutms)
      {
      }

      public async Task<bool> NewAPK(string filename, string version)
      {
         try
         {
            Send(MobileServiceHelper.NewAPK(filename, version));
            return await WaitForResults();
         }
         catch (Exception e)
         {
            throw e;
         }
      }
      public async Task<bool> RemoveAPK(DigistatMobileAPK apk)
      {
         try
         {
            Send(MobileServiceHelper.NewRemoveAPK(apk.Version, apk.File));
            return await WaitForResults();
         }
         catch (Exception e)
         {
            throw e;
         }

      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         switch (msg.Message)
         {
            case Constants.MOBILE_APK_ADDED:
            case Constants.MOBILE_APK_DELETED:
               {
                  try
                  {
                     var data = msg.Options.Find((opt) => opt.Key.Equals("SUCCESS"));
                     var result = !string.IsNullOrWhiteSpace(data.Value.ToString()) && data.Value.ToString() == "true";
                     Notify(result);
                  }
                  catch (Exception e)
                  {
                     mobjLogSvc.ErrorException(e, "Error handling {0}", msg.Message);
                     Notify(false);
                  }
                  break;
               }
         }
      }
   }
}
