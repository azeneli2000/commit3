using Configurator.Std.BL.Configurator;
using Configurator.Std.BL.DasDrivers;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.CDSS;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Digistat.FrameworkStd.Model.Ips;
using Digistat.FrameworkStd.UMSLegacy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Hubs
{
   public class MessageCenterManager : IMessageCenterManager
   {
      private readonly IMessageCenterService mobjMsgCtrSvc;
      private readonly IConfiguratorWebConfiguration mobjDigConfig;
      private readonly ILoggerService mobjLoggerService;
      //private readonly string currentUserIdentifier;

      public MessageCenterManager(IMessageCenterService msgCtrSvc, IConfiguratorWebConfiguration digConfig, ISynchronizationService syncService, ILoggerService loggerService)
      {
         mobjMsgCtrSvc = msgCtrSvc;
         mobjDigConfig = digConfig;
         mobjLoggerService = loggerService;
         //currentUserIdentifier = syncService.GetCurrentUser().Abbrev;
      }

      #region Network

      public void SendNetworkEdited(Network network, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncNetworkConfig(), network.Id);

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncNetworkConfig();
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ID", network.Id.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("HOSTNAME", network.HostName));
            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncNetworkConfig(), network.Id);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeSyncNetworkConfig(), network.Id);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncNetworkConfig(), network.Id);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type SendDriverEdited
      /// </summary>
      public void SendNetworkEdited(Network network)
      {
         SendNetworkEdited(network, DestinationHostCodes.All, ApplicationCodes.All);
      }

      #endregion Network

      #region System Option

      /// <summary>
      /// Send a SYNC message of type messageTypeSyncSystemConfig (System Option Created or Edited)
      /// </summary>
      public void SendSystemOptionEdited(SystemOption option)
      {
         SendSystemOptionEdited(option, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type messageTypeSyncSystemConfig (System Option Created or Edited
      /// </summary>
      /// <param name="userGUID">User GUID</param>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendSystemOptionEdited(SystemOption option, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncSystemConfig(), option.Guid);

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncSystemConfig();
            objMessage.Options.Add(new System.Collections.DictionaryEntry("TYPE", "SYSTEMOPTION"));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("APPS", option.Application ?? string.Empty));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("NAMES", option.Name));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("GUID", option.Guid));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncSystemConfig(), option.Guid);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeSyncSystemConfig(), option.Guid);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncSystemConfig(), option.Guid);
            throw;
         }
      }

      #endregion System Option

      #region StandardParameter

      public void SendStandardParameterUpdated(string strParID, string action)
      {
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeStandardParameterUpdated();
            objMessage.Options.Add(new System.Collections.DictionaryEntry("PARID", strParID));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ACTION", action));
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeStandardParameterUpdated(), strParID);
            throw;
         }
      }

      public void SendStandardParameterRefresh()
      {
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeStandardParameterUpdated();
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ACTION", "REFRESH"));
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeStandardParameterUpdated());
            throw;
         }
      }

      #endregion StandardParameter

      #region Role

      public void SendRoleEdited(Role r)
      {
         SendRoleEdited(r, DestinationHostCodes.All, ApplicationCodes.All);
      }

      public void SendRoleEdited(Role r, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeRoleEdited(), r.Id);

         var objMessage = new MCMessage();
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
         objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeRoleEdited();
         objMessage.Options.Add(new System.Collections.DictionaryEntry("RoleID", r.Id.ToString()));

         //TODO Trace
         mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeRoleEdited(), r.Id);
         mobjMsgCtrSvc.SendMessage(objMessage);
         //TODO GetMessageTypeRoleEdited
         mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeRoleEdited(), r.Id);
      }

      #endregion Role

      #region Permissions

      /// <summary>
      /// Send a SYNC message of type messageTypeSyncPermissions (Permission Created or Edited)
      /// </summary>
      public void SendPermissionEdited(Permission permission)
      {
         SendPermissionEdited(permission, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type messageTypeSyncPermissions (Permission Created or Edited
      /// </summary>
      /// <param name="permission">Permission entity</param>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendPermissionEdited(Permission permission, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //Old message version for compatibility with old applications
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncPermissions(), permission.Id);

            var objMessage = new MCMessage();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncPermissions();

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncPermissions(), permission.Id);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeSyncPermissions(), permission.Id);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncPermissions(), permission.Id);
            throw;
         }

         try
         {
            if (permission != null)
            {
               //Current message version
               //TODO Trace
               mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncPermissionEdited(), permission.Id);

               var objMessage = new MCMessage();
               objMessage.SourceApp = mobjDigConfig.ModuleName;
               objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
               objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
               objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
               objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
               objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncPermissionEdited();
               objMessage.Options.Add(new System.Collections.DictionaryEntry("IDPermission", permission.Id.ToString()));
               objMessage.Options.Add(new System.Collections.DictionaryEntry("FunctionName", permission.FunctionName));
               objMessage.Options.Add(new System.Collections.DictionaryEntry("PermissionModifier", permission.PermissionCode));
               objMessage.Options.Add(new System.Collections.DictionaryEntry("PermissionLevel", permission.PriorityLevel.ToString()));

               //TODO Trace
               mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncPermissionEdited(), permission.Id);
               mobjMsgCtrSvc.SendMessage(objMessage);
               //TODO Trace
               mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeSyncPermissionEdited(), permission.Id);
            }
            else
            {
               mobjLoggerService.Info("Message {0} not sent. Permission is empty", UMSFrameworkParser.GetMessageTypeSyncPermissionEdited(), permission.Id);
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncPermissionEdited(), permission.Id);
            throw;
         }
      }

      #endregion Permissions

      #region Users

      /// <summary>
      /// Send a SYNC message of type UserEdited (User Created or Edited)
      /// </summary>
      public void SendUserEdited(string userGUID)
      {
         SendUserEdited(userGUID, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type UserEdited (User Created or Edited
      /// </summary>
      /// <param name="userGUID">User GUID</param>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendUserEdited(string userGUID, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncUserEdited(), userGUID);

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncUserEdited();
            objMessage.Options.Add(new System.Collections.DictionaryEntry("USERID", userGUID));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncUserEdited(), userGUID);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeSyncUserEdited(), userGUID);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncUserEdited(), userGUID);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type UserDeleted
      /// </summary>
      public void SendUserDeleted(string userGUID)
      {
         SendUserDeleted(userGUID, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type UserDeleted
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendUserDeleted(string userGUID, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncUserDeleted(), userGUID);

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncUserDeleted();
            objMessage.Options.Add(new System.Collections.DictionaryEntry("USERID", userGUID));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncUserDeleted(), userGUID);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} sent", UMSFrameworkParser.GetMessageTypeSyncUserDeleted(), userGUID);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1}", UMSFrameworkParser.GetMessageTypeSyncUserDeleted(), userGUID);
            throw;
         }
      }

      #endregion Users

      #region Driver

      /// <summary>
      /// Send a SYNC message of type DriverDeleted
      /// </summary>
      public void SendDriverDeleted(string driverName, string driverGUID)
      {
         SendDriverDeleted(driverName, driverGUID, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type DriverDeleted
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendDriverDeleted(string driverName, string driverGUID, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDriverDeleted, driverGUID, driverName);

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSMessageExtendedCodes.messageTypeSyncDriverDeleted;
            objMessage.Options.Add(new System.Collections.DictionaryEntry("NAME", driverName));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ID", driverGUID));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDriverDeleted, driverGUID, driverName);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} - {2} sent", UMSMessageExtendedCodes.messageTypeSyncDriverDeleted, driverGUID, driverName);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDriverDeleted, driverGUID, driverName);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type SendDriverEdited
      /// </summary>
      public void SendDriverEdited(string driverName, string driverGUID, bool genericChanged = true, bool formatChanged = false, bool capabilitiesChanged = false, bool saveTableChanged = false)
      {
         SendDriverEdited(driverName, driverGUID, genericChanged, formatChanged, capabilitiesChanged, saveTableChanged, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type SendDriverEdited
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendDriverEdited(string driverName, string driverGUID, bool genericChanged, bool formatChanged, bool capabilitiesChanged, bool saveTableChanged, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDriverEdited, driverGUID, driverName);

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSMessageExtendedCodes.messageTypeSyncDriverEdited;
            objMessage.Options.Add(new System.Collections.DictionaryEntry("NAME", driverName));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ID", driverGUID));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("GENERICCHANGED", genericChanged.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("FORMATCHANGED", formatChanged.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("CAPABILITIESCHANGED", capabilitiesChanged.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("DASACQTABLE_NEWVERSION", saveTableChanged.ToString()));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDriverEdited, driverGUID, driverName);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for {1} - {2} sent", UMSMessageExtendedCodes.messageTypeSyncDriverEdited, driverGUID, driverName);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDriverEdited, driverGUID, driverName);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type DeviceDriverEdited
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendDeviceDriverEdited(int deviceDriverId, bool logOnly, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for {1} with OnlyLog = {2}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverEdited, deviceDriverId, logOnly);

            if (!destinationHost.HasValue) destinationHost = DestinationHostCodes.All;
            if (!destinationApp.HasValue) destinationApp = ApplicationCodes.All;

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSMessageExtendedCodes.messageTypeSyncDeviceDriverEdited;
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ID", deviceDriverId.ToString()));
            if (logOnly)
            {
               objMessage.Options.Add(new System.Collections.DictionaryEntry("ONLYLOG", Convert.ToByte(logOnly).ToString()));
            }
            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for Device Driver {1} with OnlyLog {2}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverEdited, deviceDriverId, logOnly);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for Device Driver {1} with OnlyLog {2} sent", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverEdited, deviceDriverId, logOnly);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for {1} - {2}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverEdited, deviceDriverId, logOnly);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type DeviceDriverAdded
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendDeviceDriverAdded(int deviceDriverId, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for Device Driver {1}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverAdded, deviceDriverId);

            if (!destinationHost.HasValue) destinationHost = DestinationHostCodes.All;
            if (!destinationApp.HasValue) destinationApp = ApplicationCodes.All;

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSMessageExtendedCodes.messageTypeSyncDeviceDriverAdded;
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ID", deviceDriverId.ToString()));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for Device Driver {1}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverAdded, deviceDriverId);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for Device Driver {1} sent", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverAdded, deviceDriverId);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for Device Driver {1}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverAdded, deviceDriverId);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type DeviceDriverDeleted
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendDeviceDriverDeleted(int deviceDriverId, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for Device Driver {1}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverDeleted, deviceDriverId);

            if (!destinationHost.HasValue) destinationHost = DestinationHostCodes.All;
            if (!destinationApp.HasValue) destinationApp = ApplicationCodes.All;

            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSMessageExtendedCodes.messageTypeSyncDeviceDriverDeleted;
            objMessage.Options.Add(new System.Collections.DictionaryEntry("ID", deviceDriverId.ToString()));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for Device Driver {1}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverDeleted, deviceDriverId);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for Device Driver {1} sent", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverDeleted, deviceDriverId);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for Device Driver {1}", UMSMessageExtendedCodes.messageTypeSyncDeviceDriverDeleted, deviceDriverId);
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type RestartDriver
      /// </summary>
      /// <param name="destinationHost">Destination Host (default ALL)</param>
      /// <param name="destinationApp">Destination App (default DAS3)</param>
      public void SendRestartDriver(int deviceDriverId, int processId, string dasBroker, bool kill, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = ApplicationCodes.Das3)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for Device Driver {1}, process {2}, broker {3} and {4}kill", UMSMessageExtendedCodes.messageRestartDriver, deviceDriverId, processId, dasBroker, kill ? "" : "not ");

            if (!destinationHost.HasValue) destinationHost = DestinationHostCodes.All;
            if (!destinationApp.HasValue) destinationApp = ApplicationCodes.All;

            var objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = destinationHost.GetDisplayAttribute(),
               DestinationApp = destinationApp.GetDisplayAttribute(),
               SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper(),
               SourceApp = mobjDigConfig.ModuleName,
               PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand(),
               Message = UMSMessageExtendedCodes.messageRestartDriver
            };
            objMessage.Options.Add(new System.Collections.DictionaryEntry("DEVICEDRIVERID", deviceDriverId.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("PROCESSID", processId.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("DASBROKER", dasBroker));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("KILL", Convert.ToByte(kill).ToString()));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for Device Driver {1}, process {2}, broker {3}", UMSMessageExtendedCodes.messageRestartDriver, deviceDriverId, processId, dasBroker);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for Device Driver {1}, process {2}, broker {3} sent", UMSMessageExtendedCodes.messageRestartDriver, deviceDriverId, processId, dasBroker);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for Device Driver {1}, process {2}, broker {3} and {4}kill", UMSMessageExtendedCodes.messageRestartDriver, deviceDriverId, deviceDriverId, processId, dasBroker, kill ? "" : "not ");
            throw;
         }
      }

      /// <summary>
      /// Send a SYNC message of type RestartDriver
      /// </summary>
      /// <param name="destinationHost">Destination Host (default ALL)</param>
      /// <param name="destinationApp">Destination App (default DAS3)</param>
      public void SendOutputStateNotification(int locationId, int bedId, int patientId, bool isSystem, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Creating {0} message for Output state with Location {1}, Bed {2}, Patient {3} and {4}System", UMSMessageExtendedCodes.messageDBOutputStateNotificationRequest, locationId, bedId, patientId, isSystem ? "" : "not ");

            if (!destinationHost.HasValue) destinationHost = DestinationHostCodes.All;
            if (!destinationApp.HasValue) destinationApp = ApplicationCodes.All;

            var objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = destinationHost.GetDisplayAttribute(),
               DestinationApp = destinationApp.GetDisplayAttribute(),
               SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper(),
               SourceApp = mobjDigConfig.ModuleName,
               PacketType = UMSFrameworkParser.GetPacketTypeSync(),
               Message = UMSMessageExtendedCodes.messageDBOutputStateNotificationRequest
            };
            objMessage.Options.Add(new System.Collections.DictionaryEntry("LOCATION", locationId.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("BED", bedId.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("PATIENT", patientId.ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("SYSTEM", Convert.ToByte(isSystem).ToString()));
            objMessage.Options.Add(new System.Collections.DictionaryEntry("REQUESTID", string.Empty));

            //TODO Trace
            mobjLoggerService.Info("Sending {0} message for output state with location {1}, bed {2}, patient {3}", UMSMessageExtendedCodes.messageDBOutputStateNotificationRequest, locationId, bedId, patientId);
            mobjMsgCtrSvc.SendMessage(objMessage);
            //TODO Trace
            mobjLoggerService.Info("Message {0} for output state with location {1}, bed {2}, patient {3} sent", UMSMessageExtendedCodes.messageDBOutputStateNotificationRequest, locationId, bedId, patientId);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message for for output state with location {1}, bed {2}, patient {3} and {4}system", UMSMessageExtendedCodes.messageDBOutputStateNotificationRequest, locationId, bedId, patientId, isSystem ? "" : "not ");
            throw;
         }
      }

      /// <summary>
      /// Send a command request requiring to create/update a record on DASOutputState table
      /// </summary>
      /// <remarks>Method never used and never tested, requires revision to response message management</remarks>
      /// <returns>After sending the request waits 3 seconds for the failure message, after timeout the operation is assumed to be executed correctly</returns>
      public async Task<bool> RequestOutputStateUpdate(DasOutputState model)
      {
         using (var mgr = new AsyncDasOutputStateDispatcher(mobjMsgCtrSvc, mobjDigConfig, mobjLoggerService))
         {
            var data = await mgr.SendOutputStateUpdated(model);

            return data;
         }
      }

      #endregion Driver

      #region BedLocations

      /// Send a SYNC message of type DriverDeleted
      /// </summary>
      public void SendBedConfig()
      {
         SendBedConfig(DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Send a SYNC message of type SendDriverEdited
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendBedConfig(DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncBedConfig();
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncBedConfig());
            throw;
         }
      }

      public void SendBedEdited(Bed bed, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncBedEdited();
            objMessage.AddOption("IDBed", bed.Id.ToString());
            objMessage.AddOption("BedIndex", bed.Index.ToString());
            objMessage.AddOption("BedName", bed.Name);
            objMessage.AddOption("LocationRef", bed.IdLocation.ToString());
            if (bed.Location != null)
            {
               objMessage.AddOption("LocationName", bed.Location.LocationName);
            }
            objMessage.AddOption("PatientRef", bed.IdPatient.ToString());
            objMessage.AddOption("BedCode", bed.BedCode);
            objMessage.AddOption("UniteCode", bed.UniteCode);
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncBedConfig());
            throw;
         }
      }

      public void SendBedEdited(Bed bed)
      {
         SendBedEdited(bed, DestinationHostCodes.All, ApplicationCodes.All);
      }

      #endregion BedLocations

      #region WaveformRules

      public void SendWaveformRuleEdited(WaveformSnapshotToUniteRule waveformRule)
      {
         SendWaveformRuleEdited(waveformRule, DestinationHostCodes.All, ApplicationCodes.All);
      }

      public void SendWaveformRuleEdited(WaveformSnapshotToUniteRule waveformRule, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            var objMessage = new MCMessage();
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncWaveFormRuleChanged();            
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncWaveFormRuleChanged());
            throw;
         }
      }

      #endregion WaveformRules

      #region ActualDevice

      /// <summary>
      /// Send a SYNC message of type SendDriverEdited
      /// </summary>
      /// <param name="destinationHost">Destination Host</param>
      /// <param name="destinationApp">Destination App</param>
      public void SendActualDeviceUpdated(DestinationHostCodes destinationHost, ApplicationCodes destinationApp, ActualDevice objDevice)
      {
         try
         {
            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeActualDeviceUpdate();
            objMessage.AddOption("AD_ID", objDevice.Id.ToString(CultureInfo.InvariantCulture));
            objMessage.AddOption("DeviceName", objDevice.Name);
            objMessage.AddOption("DeviceSerial", objDevice.SerialNumber);
            objMessage.AddOption("DeviceLabel", objDevice.Label);
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncBedConfig());
            throw;
         }
      }

      #endregion ActualDevice

      #region PortServer

      public void SendPortServerEdited(PortServer ps, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
            objMessage.Message = UMSMessageExtendedCodes.messagePortServerUpdatedOrCreated;
            objMessage.AddOption("ID", ps.ID.ToString());
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncBedConfig());
            throw;
         }
      }

      public void SendPortServerEdited(PortServer ps)
      {
         SendPortServerEdited(ps, DestinationHostCodes.All, ApplicationCodes.All);
      }

      public void SendPortServerRemoved(PortServer ps, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
            objMessage.Message = UMSMessageExtendedCodes.messagePortServerDeleted;
            objMessage.AddOption("ID", ps.ID.ToString());
            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncBedConfig());
            throw;
         }
      }

      public void SendPortServerRemoved(PortServer ps)
      {
         SendPortServerRemoved(ps, DestinationHostCodes.All, ApplicationCodes.All);
      }

      #endregion PortServer

      #region ReportTemplate

      public void SendReportTemplateEdited(ReportTemplate template)
      {
         SendReportTemplateEdited(template, DestinationHostCodes.All, ApplicationCodes.All);
      }

      public void SendReportTemplateEdited(ReportTemplate template, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            var objMessage = new MCMessage();
            // construct the message
            objMessage.DestinationHost = destinationHost.GetDisplayAttribute();
            objMessage.DestinationApp = destinationApp.GetDisplayAttribute();
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeReportTemplateEdited();
            objMessage.AddOption("NAME", template.Name);
            objMessage.AddOption("APPLICATION", template.Application);
            objMessage.AddOption("MODULE", template.Module);

            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeSyncBedConfig());
            throw;
         }
      }

      #endregion ReportTemplate

      #region "RCMD - Shutdown,Logout Applications"

      /// <summary>
      /// Logout all hosts
      /// </summary>
      /// <param name="sourceUser"></param>
      public void LogoutAllHosts(string sourceUser)
      {
         LogoutAllHosts(sourceUser, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Logout all hosts
      /// </summary>
      /// <param name="sourceUser"></param>
      /// <param name="destinationHost"></param>
      /// <param name="destinationApp"></param>
      public void LogoutAllHosts(string sourceUser, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         try
         {
            mobjLoggerService.Info("Creating {0} message for destination [{1}]", UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide(), destinationHost.GetDisplayAttribute());

            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = destinationHost.GetDisplayAttribute(),
               DestinationApp = destinationApp.GetDisplayAttribute()
            };
            objMessage.AddOption("USER", sourceUser);
            objMessage.AddOption("WORKSTATION", UMSFrameworkParser.GetWorkstationName().ToUpper());
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
            objMessage.Message = UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide();

            mobjMsgCtrSvc.SendMessage(objMessage);
            mobjLoggerService.Info("Message {0} for destination [{1}] sent", UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide(), destinationHost.GetDisplayAttribute());
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide());
            throw;
         }
      }

      /// <summary>
      /// Shutdown all hosts
      /// </summary>
      /// <param name="sourceUser"></param>
      public void ShutdownAllHosts(string sourceUser)
      {
         ShutdownAllHosts(sourceUser, DestinationHostCodes.All, ApplicationCodes.All);
      }

      /// <summary>
      /// Shutdown all hosts
      /// </summary>
      /// <param name="sourceUser"></param>
      /// <param name="destinationHost"></param>
      /// <param name="destinationApp"></param>
      public void ShutdownAllHosts(string sourceUser, DestinationHostCodes destinationHost, ApplicationCodes destinationApp)
      {
         var messageType = UMSFrameworkParser.GetMessageTypeRemoteCommandShutdownAll();
         try
         {
            mobjLoggerService.Info("Creating {0} message for destination [{1}]", messageType, destinationHost.GetDisplayAttribute());

            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = destinationHost.GetDisplayAttribute(),
               DestinationApp = destinationApp.GetDisplayAttribute()
            };
            objMessage.AddOption("USER", sourceUser);
            objMessage.AddOption("WORKSTATION", UMSFrameworkParser.GetWorkstationName().ToUpper());
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();
            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
            mobjLoggerService.Info("Message {0} for destination [{1}] sent", messageType, destinationHost.GetDisplayAttribute());
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      /// <summary>
      /// Change MessageCenter
      /// </summary>
      public void ChangeMessageCenterChange(string messageCenterAddress)
      {
         var messageType = UMSFrameworkParser.GetMessageTypeRemoteCommandChangeMessageCenter();
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };
            objMessage.AddOption("ADDRESS", messageCenterAddress);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
            mobjLoggerService.Info("Message {0} for new MC [{1}] sent", messageType, messageCenterAddress);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      public void SendCDSSCreated(CDSSRule rule)
      {
         SendCDSSMessage(rule, 0);
      }

      public void SendCDSSEdited(CDSSRule rule)
      {
         SendCDSSMessage(rule, 1);
      }

      public void SendCDSSDeleted(CDSSRule rule)
      {
         SendCDSSMessage(rule, 2);
      }

      public void SendCDSSRunTestRequest(int idTempRule, string testInputValues)
      {
         var messageType = UMSFrameworkParser.GetMessageCDSSCompileTest();

         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            objMessage.AddOption("ID", idTempRule.ToString());
            objMessage.AddOption("INPUTVALUES", testInputValues);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();

            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      public void SendCDSSCompileTestRequest(int idTempRule)
      {
         var messageType = UMSFrameworkParser.GetMessageCDSSCompileTest();

         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            objMessage.AddOption("ID", idTempRule.ToString());
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();

            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      public void SendCDSSGetOutputParametersRequest(string ruleMethod, string dllName)
      {
         var messageType = UMSFrameworkParser.GetMessageCDSSGetRuleOutputParameters();

         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            objMessage.AddOption("NAME", ruleMethod);
            objMessage.AddOption("DLLFILENAME", dllName);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();

            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      public void SendCDSSGetDllListRequest()
      {
         var messageType = UMSFrameworkParser.GetMessageCDSSGetRuleOutputParameters();

         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            //objMessage.AddOption("ID", idTempRule.ToString());
            //objMessage.AddOption("INPUTVALUES", testInputValues);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();

            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      /// <summary>
      ///
      /// </summary>
      /// <param name="rule"></param>
      /// <param name="type">0 = created, 1=edited, 2 =deleted</param>
      private void SendCDSSMessage(CDSSRule rule, int type)
      {
         var messageType = UMSFrameworkParser.GetMessageCDSSRuleEdited();
         switch (type)
         {
            case 0:
               messageType = UMSFrameworkParser.GetMessageCDSSRuleCreated();
               break;

            case 1:
               messageType = UMSFrameworkParser.GetMessageCDSSRuleEdited();
               break;

            case 2:
               messageType = UMSFrameworkParser.GetMessageCDSSRuleDelete();
               break;
         }
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            objMessage.AddOption("ID", rule.Id.ToString());
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();

            objMessage.Message = messageType;

            mobjMsgCtrSvc.SendMessage(objMessage);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
      }

      #endregion "RCMD - Shutdown,Logout Applications"

      #region CDASValidation

      public void SendConfigurationValidated()
      {
         MCMessage objMessage = new MCMessage
         {
            // construct the message
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncConfigurationValidated();
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      #endregion CDASValidation

      #region OnlineValidation

      public void SendOnlineValidationGroupEdited(int vgID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncValidationGroupUpdated();
         objMessage.AddOption("VGID", vgID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendOnlineValidationGroupAdded(int vgID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncValidationGroupAdded();
         objMessage.AddOption("VGID", vgID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendOnlineValidationGroupDeleted(int vgID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncValidationGroupDeleted();
         objMessage.AddOption("VGID", vgID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendOnlineValidationGroupSectionEdited(int vgsID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncValidationGroupSectionUpdated();
         objMessage.AddOption("VGSID", vgsID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendOnlineValidationGroupSectionAdded(int vgsID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncValidationGroupSectionAdded();
         objMessage.AddOption("VGSID", vgsID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendOnlineValidationGroupSectionDeleted(int vgsID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncValidationGroupSectionDeleted();
         objMessage.AddOption("VGSID", vgsID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      #endregion OnlineValidation

      #region OnlineQueries

      public void SendOnlineQueryDeleted(int queryID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncOnlineQueryDeleted();
         objMessage.AddOption("QUERYID", queryID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendOnlineQueryEdited(int queryID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncOnlineQueryUpdated();
         objMessage.AddOption("QUERYID", queryID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }
      public void SendOnlineQueryCreated(int queryID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncOnlineQueryAdded();
         objMessage.AddOption("QUERYID", queryID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      #endregion OnlineQueries

      #region ExportJobs

      public void SendExportJobDeleted(int jobID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageExportSchedulerJobEdited();
         objMessage.AddOption("ACTION", "DELETE");
         objMessage.AddOption("JOBID", jobID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendExportJobEdited(int jobID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageExportSchedulerJobEdited();
         objMessage.AddOption("ACTION", "EDIT");
         objMessage.AddOption("JOBID", jobID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      public void SendExportJobCreated(int jobID)
      {
         MCMessage objMessage = new MCMessage
         {
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjDigConfig.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = UMSFrameworkParser.GetMessageExportSchedulerJobEdited();
         objMessage.AddOption("ACTION", "ADD");
         objMessage.AddOption("JOBID", jobID.ToString());
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      #endregion ExportJobs

      #region Vitals

      public void SendVitalsConfigUpdated(Guid standardDatasetId, bool fromImport = false)
      {
         var objMessage = new MCMessage()
         {
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute(),
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            PacketType = UMSFrameworkParser.GetPacketTypeSync(),
            Message = UMSFrameworkParser.GetMessageTypeVitalsConfigUpdated(),
            SourceHost = UMSFrameworkParser.GetWorkstationName(),
            SourceApp = mobjDigConfig.ModuleName,
            PatientID = null,
            Options = new List<DictionaryEntry>()
            {
               new DictionaryEntry("STANDARDDATASETID", standardDatasetId),
               new DictionaryEntry("FROM_IMPORT", fromImport ? bool.TrueString : Boolean.FalseString)
            }
         };

         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      #endregion Vitals
   }
}