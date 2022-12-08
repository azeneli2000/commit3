using System;
using System.Globalization;
using System.Threading.Tasks;

using Configurator.Std.BL.Configurator;

using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Digistat.FrameworkStd.UMSLegacy;

namespace Configurator.Std.BL.DasDrivers
{
   public class AsyncDasOutputStateDispatcher : AsyncDispatcher<bool>
   {

      private readonly IConfiguratorWebConfiguration mobjDigConfig;
      //private bool result;

      public AsyncDasOutputStateDispatcher(IMessageCenterService msgCtrSvc, IConfiguratorWebConfiguration digConfig, ILoggerService logSvc, int timeoutms = 3000) : base(msgCtrSvc, logSvc, "REQUESTID", timeoutms)
      {
         mobjDigConfig = digConfig;
      }

      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         if (msg.Message == UMSMessageExtendedCodes.messageDBOutputStateNotificationRequest)
         {
            //var data = msg.Options.Find((opt) => opt.Key.Equals("SUCCESS"));
            //var result = !string.IsNullOrWhiteSpace(data.Value.ToString()) && data.Value.ToString() == "true";
            Notify(true);
         }

         if (msg.Message == UMSMessageExtendedCodes.messageDBOutputStateUpdateFailure)
         {
            //var data = msg.Options.Find((opt) => opt.Key.Equals("SUCCESS"));
            //var result = !string.IsNullOrWhiteSpace(data.Value.ToString()) && data.Value.ToString() == "true";
            Notify(false);
         }
      }

      public Task<bool> SendOutputStateUpdated(DasOutputState das)
      {

         var message = new MCMessage {
            DestinationHost = ApplicationCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.Das3.GetDisplayAttribute(),
            PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand(),
            SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper(),
            SourceApp = mobjDigConfig.ModuleName,
            Message = UMSMessageExtendedCodes.messageDBOutputStateUpdateRequest,
         };

         message.AddOption("LOCATION", das.LocationId.ToString(CultureInfo.InvariantCulture));
         message.AddOption("BED", das.BedId.ToString(CultureInfo.InvariantCulture));
         message.AddOption("PATIENT", das.PatientId.ToString(CultureInfo.InvariantCulture));
         message.AddOption("SYSTEM", das.IsSystem ? "1" : "0");
         message.AddOption("SAMPLING", das.SamplingSeconds.ToString(CultureInfo.InvariantCulture));
         message.AddOption("TYPE", das.Type.ToString());
         message.AddOption("START", das.StartDateUtc.ToString("O"));
         message.AddOption("STOP", das.StopDateUtc.ToString("O"));

         Send(message);

         return WaitForResults();
      }
      
   }
}
