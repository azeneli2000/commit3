using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Mobile;

using Configurator.Std.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Configurator.Std.BL.Mobile
{
   public class AsyncMobileDeviceDispatcher : AsyncDispatcher<List<MobileDevice>>
   {
      private readonly IDigistatConfiguration mobjDigConfig;

      public AsyncMobileDeviceDispatcher(IMessageCenterService msgCtrSvc, IDigistatConfiguration digConfig, ILoggerService logSvc) : base(msgCtrSvc,logSvc)
      {
         mobjDigConfig = digConfig;
      }

      public async Task<List<MobileDevice>> RequestDevices()
      {
         try
         {
            Send(MobileServiceHelper.NewGetDevices());
            return await WaitForResults();
         }
         catch (Exception e)
         {
            throw e;
         }
      }

      public async Task<IEnumerable<MobileDevice>> FetchDeviceList()
      {
         return await Task.Run(() =>
         {
            using (var context = new DigistatDBContext(mobjDigConfig))
            {
               
               return context.Set<Network>().Where(x => x.Type == (int)NetworkTypeEnum.Mobile && x.LastConnection.HasValue && EF.Functions.DateDiffDay(DateTime.UtcNow, x.LastConnection) <= 365)
                  .OrderByDescending(x => x.LastConnection)
                  .Select(n => new MobileDevice(n.HostName, n.LastConnection))
                  .ToList();
            }
         });
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         switch (msg.Message)
         {
            case UMSMessageExtendedCodes.messageTypeSyncDevices:
               
               var data = msg.Options.Find((opt) => opt.Key.Equals(UMSMessageExtendedCodes.messageTypeSyncDevices));
               if (data.Value != null) {
                  var devices = SerializationHelper.Deserialize<List<MobileDevice>>(data.Value.ToString());

                  Notify(devices.OrderByDescending(x => x.LastKeepAlive).ToList());
               }
               break;
         }
      }
   }
}
