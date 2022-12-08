using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;

namespace ConfiguratorWeb.App
{
   /// <summary>
   /// Implement this class to register at startup all needed systemoptions. All Classes that implement ISystemOptionCollection will
   /// be automatically loaded. Class must provide a Readonly property (Collection) that returns the list of needed systemoptions.
   /// </summary>
   public class ConfiguratorWebPermissionCollection : IPermissionCollection
   {
      private Permissions mobjCollection = null;
      private readonly IDigistatConfiguration mobjDigCfg;
      public ConfiguratorWebPermissionCollection(IDigistatConfiguration digCfg)
      {
         mobjDigCfg = digCfg;
         mobjCollection = new Permissions();
         PopulateSystemOptionCollection();
      }

      private void PopulateSystemOptionCollection()
      {

         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionaryAdd, mobjDigCfg.ModuleName, "Permission to add the \"Dictionary\" translation");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionaryEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Dictionary\" translation");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionaryView, mobjDigCfg.ModuleName,"Permission to access \"Dictionary\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionaryKeyNew, mobjDigCfg.ModuleName, "Permission to add the \"Dictionary\" keys");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionaryKeyDelete, mobjDigCfg.ModuleName, "Permission to delete the \"Dictionary\" keys");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionarySysEdit, mobjDigCfg.ModuleName, "Permission to edit the System \"Dictionary\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.DictionaryDelete, mobjDigCfg.ModuleName, "Permission to delete the \"Dictionary\" translation");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Bed\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionBedsView, mobjDigCfg.ModuleName,"Permission to access \"Bed\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionCDASValidationEdit, mobjDigCfg.ModuleName, "Permission to edit the \"CDAS Validation\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionCDSSdit, mobjDigCfg.ModuleName, "Permission to edit the \"CDSS\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionCDSSView, mobjDigCfg.ModuleName,"Permission to access \"CDSS\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionChangeMessageCenter, mobjDigCfg.ModuleName);
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListEdit, mobjDigCfg.ModuleName,"Permission to edit clinical configuration (simple choices-custom list)");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListView, mobjDigCfg.ModuleName,"Permission to access \"Clinical Configuration\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionClinicalLogView, mobjDigCfg.ModuleName,"Permission to access \"Clinical Log\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionConfigurator, mobjDigCfg.ModuleName);
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionDigistatLogView, mobjDigCfg.ModuleName,"Permission to access \"Digistat Log\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Driver\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionDriverView, mobjDigCfg.ModuleName,"Permission to access \"Driver\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Export Scheduler\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerView, mobjDigCfg.ModuleName,"Permission to access \"Export Scheduler\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionFBEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Fluid Balance\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionFBView, mobjDigCfg.ModuleName,"Permission to access \"Fluid Balance\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Hospital units\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, mobjDigCfg.ModuleName,"Permission to access \"Hospital units\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Telligence integration\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, mobjDigCfg.ModuleName,"Permission to access \"Telligence integration\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionLocationsEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Location\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, mobjDigCfg.ModuleName,"Permission to access \"Location\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionLogoutAllConfiguration, mobjDigCfg.ModuleName);
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionMessageFlowView, mobjDigCfg.ModuleName,"Permission to access \"Message Flow\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Miscellanea\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaView, mobjDigCfg.ModuleName,"Permission to access \"Miscellanea\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Mobile\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionMobileView, mobjDigCfg.ModuleName,"Permission to access \"Mobile\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionNetworkProbeView, mobjDigCfg.ModuleName,"Permission to access \"Probe\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionNetworkEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Network\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionNetworkView, mobjDigCfg.ModuleName,"Permission to access \"Network\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionPermissionsEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Permissions\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionPermissionsView, mobjDigCfg.ModuleName,"Permission to access \"Permissions\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Online Custom Query\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersView, mobjDigCfg.ModuleName,"Permission to access \"Online Custom Query\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Report master template\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, mobjDigCfg.ModuleName,"Permission to access \"Report master template\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Roles\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionRolesView, mobjDigCfg.ModuleName,"Permission to access \"Roles\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionShutdownAllConfiguration, mobjDigCfg.ModuleName);
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Standard parameters\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEditSystem, mobjDigCfg.ModuleName, "Permission to edit the System \"Standard parameters\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, mobjDigCfg.ModuleName,"Permission to access \"Standard parameters\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionSupervisionMenuView, mobjDigCfg.ModuleName,"Permission to access \"Monitoring\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionSystemAdministratorEdit, mobjDigCfg.ModuleName, "Permission to edit the \"System Administrator\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionSystemAdministratorView, mobjDigCfg.ModuleName,"Permission to access \"System Administrator\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, mobjDigCfg.ModuleName, "Permission to edit the \"System Option\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, mobjDigCfg.ModuleName,"Permission to access \"System Option\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionTherapyEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Therapy\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionTherapyView, mobjDigCfg.ModuleName,"Permission to access \"Therapy\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionUserEdit, mobjDigCfg.ModuleName, "Permission to edit the \"User\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionUserView, mobjDigCfg.ModuleName,"Permission to access \"User\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Online Validation groups\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionValidationView, mobjDigCfg.ModuleName,"Permission to access \"Online Validation groups\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Vitals dataset\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, mobjDigCfg.ModuleName,"Permission to access \"Vitals dataset\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionWebModulesEdit, mobjDigCfg.ModuleName, "Permission to edit the \"Web Modules\" configuration");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionWebModulesView, mobjDigCfg.ModuleName,"Permission to access \"Web Modules\" menu entry");
         mobjCollection.AddPermission(Configurator.Std.Defs.Permissions.permissionVitalImport, mobjDigCfg.ModuleName, "Allow dataset import");


      }

      public Permissions Collection
      {
         get
         {
            return mobjCollection;
         }
      }
   }
}
