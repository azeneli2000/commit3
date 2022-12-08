using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Defs
{
    public static class Permissions
    {
        public const string permissionSimpleChoiceView = "SSC.SIMPLECHOICE.VIEW";
        public const string permissionSimpleChoiceEdit = "SSC.SIMPLECHOICE.EDIT";
        public const string permissionHolidaysView = "SSC.HOLIDAYS.VIEW";
        public const string permissionHolidaysEdit = "SSC.HOLIDAYS.EDIT";
        public const string permissionCalendarExceptionView = "SSC.CALENDAREXCEPTION.VIEW";
        public const string permissionCalendarExceptionEdit = "SSC.CALENDAREXCEPTION.EDIT";
        public const string permissionUserPersonnelRelationView = "SSC.USERPERSONNELRELATION.VIEW";
        public const string permissionUserPersonnelRelationEdit = "SSC.USERPERSONNELRELATION.EDIT";
        public const string permissionUserHospitalUnitsRelationView = "SSC.USERHOSPITALUNITSRELATION.VIEW";
        public const string permissionUserHospitalUnitsRelationEdit = "SSC.USERHOSPITALUNITSRELATION.EDIT";
        public const string permissionHospitalUnitsRelationView = "SSC.HOSPITALUNITSRELATION.VIEW";
        //public const string permissionHospitalUnitsRelationEdit = "SSC.HOSPITALUNITSRELATION.EDIT";
        public const string permissionSlotTemplateView = "SSC.SLOTTEMPLATE.VIEW";
        public const string permissionSlotTemplateEdit = "SSC.SLOTTEMPLATE.EDIT";
        public const string permissionActualSlotView = "SSC.ACTUALSLOT.VIEW";
        public const string permissionActualSlotEdit = "SSC.ACTUALSLOT.EDIT";
        public const string permissionStandardDevicesView = "SSC.STANDARDDEVICES.VIEW";
        public const string permissionStandardDevicesEdit = "SSC.STANDARDDEVICES.EDIT";
        public const string permissionStandardRequirementView = "SSC.STANDARDREQUIREMENT.VIEW";
        public const string permissionStandardRequirementEdit = "SSC.STANDARDREQUIREMENT.EDIT";
        public const string permissionOperatingRoomDevicesRelationView = "SSC.OPERATINGROOMDEVICERELATION.VIEW";
        public const string permissionOperatingRoomDevicesRelationEdit = "SSC.OPERATINGROOMDEVICERELATION.EDIT";
        public const string permissionStandardOperationView = "SSC.STANDARDOPERATION.VIEW";
        public const string permissionStandardOperationEdit = "SSC.STANDARDOPERATION.EDIT";
        public const string permissionOperatingBlockRelationView = "SSC.OPERATINGBLOCKRELATION.VIEW";
        public const string permissionOperatingBlockRelationEdit = "SSC.OPERATINGBLOCKRELATION.EDIT";
        public const string permissionHospitalUnitsView = "SSC.HOSPITALUNITS.VIEW";
        public const string permissionHospitalUnitsEdit = "SSC.HOSPITALUNITS.EDIT";
        public const string permissionOperatingBlockView = "SSC.OPERATINGBLOCK.VIEW";
        public const string permissionOperatingBlockEdit = "SSC.OPERATINGBLOCK.EDIT";
        public const string permissionOperatingRoomView = "SSC.OPERATINGROOM.VIEW";
        public const string permissionOperatingRoomEdit = "SSC.OPERATINGROOM.EDIT";
        public const string permissionPersonnelView = "SSC.PERSONNEL.VIEW";
        public const string permissionPersonnelEdit = "SSC.PERSONNEL.EDIT";
        public const string permissionSystemOptionsView = "SSC.SYSTEMOPTIONS.VIEW";
        public const string permissionSystemOptionsEdit = "SSC.SYSTEMOPTIONS.EDIT";
        public const string permissionAdministrationSystemOptionsView = "SSC.ADMINISTRATIONSYSTEMOPTIONS.VIEW";
        // the permissionAdministrationSystemOptionsEdit doesn't exists because the administration system options and system options use the same form
        public const string permissionStandardResourceView = "SSC.STANDARDRESOURCE.VIEW";
        public const string permissionStandarResourceEdit = "SSC.STANDARDRESOURCE.EDIT";
        public const string permissionStandardKitView = "SSC.STANDARDKIT.VIEW";
        public const string permissionStandardKitEdit = "SSC.STANDARDKIT.EDIT";
        public const string permissionStandardPlannedResourceView = "SSC.STANDARDPLANNEDRESOURCE.VIEW";
        public const string permissionStandardPlannedResourceEdit = "SSC.STANDARDPLANNEDRESOURCE.EDIT";
        public const string permissionLocationsView = "SSC.LOCATIONS.VIEW";
        public const string permissionLocationsEdit = "SSC.LOCATIONS.EDIT";
        public const string permissionBedsView = "SSC.BEDS.VIEW";
        public const string permissionBedsEdit = "SSC.BEDS.EDIT";
        public const string permissionReleasesView = "SSC.RELEASES.VIEW";
        public const string permissionReleasesEdit = "SSC.RELEASES.EDIT";
        public const string permissionClinicalSupervisorView = "SSC.CLINICALSUPERVISOR.VIEW";
        //public const string permissionClinicalSupervisorEdit = "SSC.CLINICALSUPERVISOR.EDIT";
        public const string permissionSystemAdministratorView = "SSC.SYSTEMADMINISTRATOR.VIEW";
        public const string permissionSystemAdministratorEdit = "SSC.SYSTEMADMINISTRATOR.EDIT";
        public const string permissionNetworkView = "SSC.NETWORKS.VIEW";
        public const string permissionNetworkEdit = "SSC.NETWORKS.EDIT";
        public const string permissionPersonnelRolesView = "SSC.PERSONNELROLES.VIEW";
        public const string permissionPersonnelRolesEdit = "SSC.PERSONNELROLES.EDIT";
        public const string permissionDefaultNamedFormatView = "SSC.NAMEDFORMAT.VIEW";
        public const string permissionDefaultNamedFormatEdit = "SSC.NAMEDFORMAT.EDIT";

        public const string permissionPermissionsView = "SCC.PERMISSIONS.VIEW";
        public const string permissionPermissionsEdit = "SCC.PERMISSIONS.EDIT";
        public const string permissionUserView = "SCC.USERS.VIEW";
        public const string permissionUserEdit = "SCC.USERS.EDIT";
        public const string permissionCDASValidationEdit = "SCC.CDASVALIDATION.EDIT";

        public const string permissionDriverView = "DAS.DRIVER.VIEW";
        public const string permissionDriverEdit = "DAS.DRIVER.EDIT";

        public const string permissionVitalSignsView = "VITALSIGNS.CONFIG_VIEW";
        public const string permissionVitalSignsEdit = "VITALSIGNS.CONFIG_EDIT";
        public const string permissionVitalImport = "VITALSIGNS.CONFIG_IMPORT";

        public const string permissionMobileView = "MOBILE.CONFIG_VIEW";
        public const string permissionMobileEdit = "MOBILE.CONFIG_EDIT";


        public const string permissionIntegrationTelligenceView = "INTEGRATIONS.TELLIGENCE.CONFIG_VIEW";
        public const string permissionIntegrationTelligenceEdit = "INTEGRATIONS.TELLIGENCE.CONFIG_EDIT";

        public const string permissionReportMasterTemplateView = "REPORTMASTER.TEMPLATE.VIEW";
        public const string permissionReportMasterTemplateEdit = "REPORTMASTER.TEMPLATE.EDIT";

        public const string permissionConfigurator = "CONFIGURATOR";


        public const string permissionChangeMessageCenter = "CONFIG.CHANGEMC";
        public const string permissionShutdownAllConfiguration = "MENU;SYSTEM ADMINISTRATION;SHUT DOWN EVERY DIGISTAT";
        public const string permissionLogoutAllConfiguration = "MENU;SYSTEM ADMINISTRATION;PRIVACY LOGOUT";

        public const string permissionDictionarySystem = "CONF.DICTIONARY.EDITSYSTEM";

        public const string permissionHAMonitorView = "CONFIG.HAMONITOR.VIEW";
        public const string permissionHAMonitorActivate = "CONFIG.HAMONITOR.ACTIVATE";

        public const string DictionaryView = "CONF.DICTIONARY.VIEW";
        public const string DictionaryAdd = "CONF.DICTIONARY.ADD";
        public const string DictionaryEdit = "CONF.DICTIONARY.EDIT";
        public const string DictionaryDelete = "CONF.DICTIONARY.DELETE";
        public const string DictionaryKeyDelete = "CONF.DICTIONARYKEY.DELETE";
        public const string DictionaryKeyNew = "CONF.DICTIONARYKEY.NEW";
        public const string DictionarySysEdit = "CONF.DICTIONARY.CONFIG_SYS_EDIT";


        public const string permissionMiscellaneaView = "CONF.MISCELLANEA.VIEW";
        public const string permissionMiscellaneaEdit = "CONF.MISCELLANEA.EDIT";
        public const string permissionMiscellaneaDelete = "CONF.MISCELLANEA.DELETE";

        public const string permissionDigistatRepositoryView = "CONF.DIGISTATREPOSITORY.VIEW";
        public const string permissionDigistatRepositoryEdit = "CONF.DIGISTATREPOSITORY.EDIT";

        public const string permissionClinicalCustomListView = "CONF.CLINICALCUSTOMLIST.VIEW";
        public const string permissionClinicalCustomListEdit = "CONF.CLINICALCUSTOMLIST.EDIT";

        public const string permissionCDSSView = "CONF.CDSS.VIEW";
        public const string permissionCDSSdit = "CONF.CDSS.EDIT";

        public const string permissionStandardParametersView = "CONF.STANDARDPARAMETERS.VIEW";
        public const string permissionStandardParametersEdit = "CONF.STANDARDPARAMETERS.EDIT";
        public const string permissionStandardParametersEditSystem = "CONF.STANDARDPARAMETERS.EDITSYSTEM";

        public const string permissionClinicalLogView = "CONF.CLINICALLOGS.VIEW";
        public const string permissionDigistatLogView = "CONF.DIGISTATLOGS.VIEW";
        public const string permissionNetworkProbeView = "CONF.NETWORKSPROBE.VIEW";

        public const string permissionAdminAboutView = "CONF.ADMINABOUT.VIEW";

        public const string permissionRolesView = "CONF.ROLES.VIEW";
        public const string permissionRolesEdit = "CONF.ROLES.EDIT";

        public const string permissionValidationView = "CONF.ONLINEVALIDATION.VIEW";
        public const string permissionValidationEdit = "CONF.ONLINEVALIDATION.EDIT";

        public const string permissionCoronellaView = "CONF.CORNELLA.VIEW";
        public const string permissionMessageFlowView = "CONF.MESSAGEFLOW.VIEW";
        public const string permissionFBView = "CONF.FLUIDBALANCE.VIEW";
        public const string permissionFBEdit = "CONF.FLUIDBALANCE.EDIT";
        public const string permissionWebModulesView = "CONF.WEBMODULES.VIEW";
        public const string permissionWebModulesEdit = "CONF.WEBMODULES.EDIT";

        public const string permissionSupervisionMenuView = "CONF.SUPERVISION.VIEW";
        public static string permissionQueryParametersView = "CONF.ONLINEQUERYPARAMETERS.VIEW";
        public static string permissionQueryParametersEdit = "CONF.ONLINEQUERYPARAMETERS.EDIT";

        public const string permissionExportSchedulerView = "CONF.EXPORTSCHEDULER.VIEW";
        public const string permissionExportSchedulerEdit = "CONF.EXPORTSCHEDULER.EDIT";

        public const string permissionTherapyView = "CONF.THERAPY.VIEW";
        public const string permissionTherapyEdit = "CONF.THERAPY.EDIT";
        public const string permissionMonitoringMenuView = "SYSTEMMONITORING.VIEW";

        public const string permissionDiaryWebView = "CONF.DIARYWEB.VIEW";
        public const string permissionDiaryWebEdit = "CONF.DIARYWEB.EDIT";

        public const string permissionWaveformRulesView = "CONF.WAVEFORMRULES.VIEW";
        public const string permissionWaveformRulesEdit = "CONF.WAVEFORMRULES.EDIT";

        #region Stock Management

        public const string permissionStockManagementView = "CONF.STOCKMANAGEMENT.VIEW";

        public const string permissionStockManagementDetailView = "CONF.STOCKMANAGEMENTDETAIL.VIEW";
        public const string permissionStockManagementDetailEdit = "CONF.STOCKMANAGEMENTDETAIL.EDIT";

        public const string permissionStockManagementOperatingBlocksView = "CONF.STOCKMANAGEMENTOPERATINGBLOCKS.VIEW";
        public const string permissionStockManagementOperatingRoomsView = "CONF.STOCKMANAGEMENTOPERATINGROOMS.VIEW";

        #endregion
    }
}
