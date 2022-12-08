using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;

using Configurator.Std.BL.Configurator;

namespace Configurator.Std.BL.DasDrivers
{
   public class AsyncDasInstanceDispatcher : AsyncDispatcher<Dictionary<string, List<DriverStatus>>>
   {
      private readonly IConfiguratorWebConfiguration mobjDigConfig;


      private Dictionary<string, List<DriverStatus>> dasBrokers = new Dictionary<string, List<DriverStatus>>();

      public AsyncDasInstanceDispatcher(IMessageCenterService msgCtrSvc, IConfiguratorWebConfiguration digConfig, ILoggerService logSvc) : base(msgCtrSvc,logSvc,null, digConfig.DasDriversStatusRequestTimeout)
      {
         mobjDigConfig = digConfig;
      }


      public bool HasBrokersResult()
      {
         return dasBrokers.Any();
      }

      public async Task<Dictionary<string, List<DriverStatus>>> GetDriversStatus()
      {
         var response = new TaskCompletionSource<Dictionary<string, List<DriverStatus>>>();
         response.TrySetResult(dasBrokers);
         return await response.Task;
      }


      public async Task<Dictionary<string, List<DriverStatus>>> RequestDriversStatus(string computerName)
      {
         try
         {
            Send( new MCMessage {
               DestinationHost = string.Format("!{0}", computerName),
               DestinationApp = ApplicationCodes.Das3.GetDisplayAttribute(),
               PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand(),
               SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper(),
               SourceApp = mobjDigConfig.ModuleName,               
               Message = UMSMessageExtendedCodes.messageStatusRequest,

            });

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
            case UMSMessageExtendedCodes.messageStatusResponse:

               var data = msg.Options.Find((opt) => opt.Key.ToString().ToUpper().Equals("STATUS"));
               if (data.Value != null)
               {
                  var drivers = SerializationHelper.Deserialize<DriverStatusList>(data.Value.ToString());


                  //Prevent duplications
                  if (!dasBrokers.Keys.Contains(msg.SourceHost)) { dasBrokers.Add(msg.SourceHost, drivers.DriverStatus); }
                  else
                  {
                     dasBrokers[msg.SourceHost].AddRange(drivers.DriverStatus);
                  }

                  //Notify(drivers.DriverStatus);
               }
               break;

               //var data = msg.Options.Find((opt) => opt.Key.Equals(UMSMessageExtendedCodes.messageStatusResponse));
               //if (data.Value != null) {
               //   var devices = SerializationHelper.Deserialize<List<DasDriverStatus>>(data.Value.ToString());

               //   Notify(devices.OrderByDescending(x => x.DasBroker).ToList());
               //}
               //break;
         }
      }
   }
}
