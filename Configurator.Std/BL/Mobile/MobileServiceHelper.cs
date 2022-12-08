using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.Mobile;
using Digistat.FrameworkStd.UMSLegacy;
using System;
using System.Collections.Generic;
using System.Text;
//using UMS.Framework.ContextManager;

namespace Configurator.Std.BL.Mobile
{
   public class MobileServiceHelper
   {
      private static readonly string APP = "CONFIGURATORWEB";

      public static MCMessage NewGetDevices()
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = UMSMessageExtendedCodes.messageTypeRemoteCommandGetDevices;

         return objMessage;
      }

      public static MCMessage NewSendConfiguration(string url, int port, bool launcher, string deviceId)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_SET_CONFIGURATION;
         objMessage.AddOption(Constants.SERVER_ADDRESS, url);
         objMessage.AddOption(Constants.SERVER_PORT, port.ToString());
         objMessage.AddOption(Constants.DIGISTAT_LAUNCHER, launcher.ToString());
         if (!string.IsNullOrEmpty(deviceId))
         {
            objMessage.AddOption("DEVICEID", deviceId);
         }

         return objMessage;
      }

      public static MCMessage NewGetConfiguration(string deviceId)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_GET_CONFIGURATION;
         objMessage.AddOption("DEVICEID", deviceId);

         return objMessage;
      }

      public static MCMessage NewGetLogFile(string deviceID, DateTime? date)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.GETLOGFILE;
         objMessage.AddOption("DEVICEID", deviceID);

         if (date.HasValue)
         {
            objMessage.AddOption("DATE", date.Value.ToString("yyyy-MM-dd"));
         }
         return objMessage;
      }

      public static MCMessage NewMobileDisconnect(string deviceID)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_DISCONNECT;
         objMessage.AddOption("DEVICEID", deviceID);

         return objMessage;
      }

      public static MCMessage NewCommunication(Communication objCommunication)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILECOMMUNICATION;
         objMessage.AddOption("DEVICEID", objCommunication.DeviceID);
         objMessage.AddOption("TITLE", objCommunication.Title ?? "");
         objMessage.AddOption("MESSAGE", objCommunication.Message);
         objMessage.AddOption("ACTION", objCommunication.Action ?? "");
         objMessage.AddOption("PRIORITY", objCommunication.Priority.ToString());

         return objMessage;
      }

      public static MCMessage NewMobileSystemStatus(string deviceID)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILESYSTEMSTATUS;
         objMessage.AddOption("DEVICEID", deviceID);

         return objMessage;
      }

        public static MCMessage NewMobileSourceRef(string deviceID)
        {
            var objMessage = new MCMessage();
            objMessage.Encrypt = true;
            objMessage.SourceApp = APP;
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
            objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
            objMessage.Message = Constants.MOBILESOURCEREF;
            objMessage.AddOption("DEVICEID", deviceID);

            return objMessage;
        }

        public static MCMessage NewMobileServerStatus()
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.GET_MOBILE_SERVER_STATUS;

         return objMessage;
      }

      public static MCMessage NewAPK(string filename, string version)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_APK_UPLOADED;
         objMessage.AddOption(Constants.FILENAME, filename);
         objMessage.AddOption(Constants.VERSION, version);
         return objMessage;
      }

      public static MCMessage NewRemoveAPK(string version, string filename)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_APK_REMOVED;
         objMessage.AddOption(Constants.FILENAME, filename ?? string.Empty);
         objMessage.AddOption(Constants.VERSION, version);
         return objMessage;
      }

      public static MCMessage NewPosition(string positionCode)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_POSITION_ADDED;
         objMessage.AddOption(Constants.MOBILE_POSITION_CODE, positionCode ?? string.Empty);
         return objMessage;
      }

      public static MCMessage UpdatePosition(string positionCode)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_POSITION_UPDATED;
         objMessage.AddOption(Constants.MOBILE_POSITION_CODE, positionCode ?? string.Empty);
         return objMessage;
      }

      public static MCMessage DeletePosition(string positionCode)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = ApplicationCodes.UMSMobileService.GetDisplayAttribute();
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
         objMessage.Message = Constants.MOBILE_POSITION_REMOVED;
         objMessage.AddOption(Constants.MOBILE_POSITION_CODE, positionCode ?? string.Empty);
         return objMessage;
      }
   }
}
