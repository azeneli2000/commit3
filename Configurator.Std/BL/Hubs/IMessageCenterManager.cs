using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Digistat.FrameworkStd.Model.Ips;
using System;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Hubs
{
   public interface IMessageCenterManager
   {
      void SendDriverDeleted(string driverName, string driverGUID);

      void SendDriverDeleted(string driverName, string driverGUID, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendDriverEdited(string driverName, string driverGUID, bool genericChanged = true, bool formatChanged = false, bool capabilitiesChanged = false, bool saveTableChanged = false);

      void SendDriverEdited(string driverName, string driverGUID, bool genericChanged, bool formatChanged, bool capabilitiesChanged, bool saveTableChanged, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendDeviceDriverAdded(int deviceDriverId, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null);

      void SendDeviceDriverEdited(int deviceDriverId, bool logOnly, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null);

      void SendDeviceDriverDeleted(int deviceDriverId, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null);

      void SendRestartDriver(int deviceDriverId, int processId, string dasBroker, bool kill, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = ApplicationCodes.Das3);

      void SendOutputStateNotification(int locationId, int bedId, int patientId, bool isSystem, DestinationHostCodes? destinationHost = null, ApplicationCodes? destinationApp = null);

      Task<bool> RequestOutputStateUpdate(DasOutputState model);

      void SendActualDeviceUpdated(DestinationHostCodes destinationHost, ApplicationCodes destinationApp, ActualDevice objDevice);

      void SendPermissionEdited(Permission permission);

      void SendPermissionEdited(Permission permission, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendSystemOptionEdited(SystemOption option);

      void SendSystemOptionEdited(SystemOption option, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendUserDeleted(string userGUID);

      void SendUserDeleted(string userGUID, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendUserEdited(string userGUID);

      void SendUserEdited(string userGUID, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendNetworkEdited(Network network, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendNetworkEdited(Network network);

      void SendBedEdited(Bed bed, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendBedEdited(Bed bed);

      void SendBedConfig(DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendBedConfig();

      public void SendWaveformRuleEdited(WaveformSnapshotToUniteRule waveformRule);

      public void SendWaveformRuleEdited(WaveformSnapshotToUniteRule waveformRule, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendReportTemplateEdited(ReportTemplate template, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendReportTemplateEdited(ReportTemplate template);

      void LogoutAllHosts(string sourceUser);

      void LogoutAllHosts(string sourceUser, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendPortServerEdited(PortServer ps, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendPortServerEdited(PortServer ps);

      void SendPortServerRemoved(PortServer ps, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendPortServerRemoved(PortServer ps);

      void ShutdownAllHosts(string sourceUser);

      void ShutdownAllHosts(string sourceUser, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void ChangeMessageCenterChange(string messageCenterAddress);

      void SendCDSSEdited(Digistat.FrameworkStd.Model.CDSS.CDSSRule rule);

      void SendCDSSCreated(Digistat.FrameworkStd.Model.CDSS.CDSSRule rule);

      void SendCDSSDeleted(Digistat.FrameworkStd.Model.CDSS.CDSSRule rule);

      void SendCDSSCompileTestRequest(int ruleId);

      void SendCDSSRunTestRequest(int ruleId, string testInputValues);

      void SendCDSSGetDllListRequest();

      void SendRoleEdited(Role r);

      void SendRoleEdited(Role r, DestinationHostCodes destinationHost, ApplicationCodes destinationApp);

      void SendConfigurationValidated();

      void SendStandardParameterUpdated(string strParID, string action);

      void SendStandardParameterRefresh();

      void SendOnlineValidationGroupEdited(int vgID);

      void SendOnlineValidationGroupAdded(int vgID);

      void SendOnlineValidationGroupDeleted(int vgID);

      void SendOnlineValidationGroupSectionEdited(int vgsID);

      void SendOnlineValidationGroupSectionAdded(int vgsID);

      void SendOnlineValidationGroupSectionDeleted(int vgsID);

      void SendVitalsConfigUpdated(Guid standardDatasetId, bool fromImport = false);

      void SendOnlineQueryCreated(int oqID);
      void SendOnlineQueryEdited(int oqID);

      void SendOnlineQueryDeleted(int oqID);

      void SendExportJobDeleted(int jobID);

      void SendExportJobEdited(int jobID);

      void SendExportJobCreated(int jobID);
   }
}