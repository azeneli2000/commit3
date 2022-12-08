using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.UMSLegacy;

using Configurator.Std.BL.Configurator;

namespace Configurator.Std.BL.DasDrivers
{
   public class AsyncDasDispatcher : AsyncDispatcher<List<DasInstance>>
   {
      private readonly IConfiguratorWebConfiguration mobjDigConfig;

      private List<DasInstance> instances = new List<DasInstance>();

      public AsyncDasDispatcher(IMessageCenterService msgCtrSvc, IConfiguratorWebConfiguration digConfig, ILoggerService logSvc) : base(msgCtrSvc,logSvc, null, digConfig.DasInstancesRequestTimeout)
      {
         mobjDigConfig = digConfig;
      }

      public bool HasInstancesResult()
      {
         return instances.Any();
      }

      public async Task<List<DasInstance>> GetDasInstances()
      {
         var response = new TaskCompletionSource<List<DasInstance>>();
         response.TrySetResult(instances);
         return await response.Task;
      }

      public async Task<List<DasInstance>> RequestDasInstances()
      {
         try
         {
            instances = new List<DasInstance>();
          
            Send(new MCMessage
            {
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute(),
               SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper(),
               SourceApp = mobjDigConfig.ModuleName,
               PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand(),
               Message = UMSMessageExtendedCodes.messagePing,
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
            case UMSMessageExtendedCodes.messagePong:
               
               var data = msg.Options.Find((opt) => opt.Key.ToString().ToUpper().Equals("DASNAME"));
               if (data.Value != null)
               {
                  //Build object
                  DasInstance objDas = new DasInstance();
                  objDas.Name = msg.GetSafeOptionValueAsString("DASNAME");
                  objDas.ComputerName = msg.SourceHost; //"DAS3PLUS§VDEV-SRV";
                  objDas.Version = msg.GetSafeOptionValueAsString("DASVERSION");
                  //New feature to exclude slave from Das Node List
                  string dastype = msg.GetSafeOptionValueAsString("DASTYPE");
                  if (dastype.ToUpper() != "SLAVE")
                  {
                     //Prevent duplications
                     if (!instances.Any(x => x.Name == objDas.Name && x.Version == objDas.Version))
                     {
                        instances.Add(objDas);
                     }
                  }

                  //Notify(results);
               }
               break;
         }
      }
   }
}
