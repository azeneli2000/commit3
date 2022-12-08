using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.UMSLegacy;
using Microsoft.Extensions.Caching.Memory;
using NPOI.SS.Formula.Functions;

namespace ConfiguratorWeb.App.Services
{
    public class MenuService : IMenuService
    {

        private const string MENU_CACHE_PREFIX = "MENU_CACHE_";
        private const string USER_CACHE_PREFIX = "USER_CACHE_";

        private readonly IDigistatConfiguration mobjDigistatConfig;
        private readonly ISynchronizationService mobjSyncSvc;
        private readonly IPermissionsService mobjPermSvc;
        private readonly IDictionaryService mobjDicSvc;
        private readonly IDigistatEnvironmentService mobjEnvSvc;
        private readonly IMessageCenterService mobjMsgCtr;
        private IMemoryCache mobjMemCache;


        private List<string> mobjMenuCacheKeyList;

        /******** VERY BIG TODO: ******/
        /* Handle permission change message to resync che cache */





        public MenuService(ISynchronizationService syncSvc, IPermissionsService permSvc, IDictionaryService dicSvc, IDigistatEnvironmentService envSvc,
           IMemoryCache memCache, IMessageCenterService msgCtrSvc, IDigistatConfiguration config)
        {
            mobjSyncSvc = syncSvc;
            mobjPermSvc = permSvc;
            mobjDicSvc = dicSvc;
            mobjEnvSvc = envSvc;
            mobjMemCache = memCache;
            mobjMsgCtr = msgCtrSvc;
            mobjMenuCacheKeyList = new List<string>();
            mobjMsgCtr.OnMessageReceived += MobjMsgCtr_OnMessageReceived;
            mobjDigistatConfig = config;



        }

        private void MobjMsgCtr_OnMessageReceived(object sender, Digistat.FrameworkStd.MessageCenter.MCMessage msg)
        {
            if (msg.Message == UMSFrameworkParser.GetMessageTypeSyncPermissions())
            {
                ClearCache();
            }
        }

        public void ClearCache()
        {
            if (mobjMemCache != null && mobjMenuCacheKeyList != null)
            {
                foreach (string s in mobjMenuCacheKeyList)
                {
                    mobjMemCache.Remove(s);
                }
            }
        }

        public List<MenuViewModel> GetMenuForUser(User usr)
        {
            List<MenuViewModel> objMenuret = null;
            if (usr != null)
            {
                string strHash = CalculateHash(usr);

                if (!mobjMemCache.TryGetValue(MENU_CACHE_PREFIX + strHash, out objMenuret))
                {
                    // Key not in cache, so get data.
                    objMenuret = CreateMenuForUser(usr);

                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(120));

                    // Add in key list
                    mobjMenuCacheKeyList.Add(MENU_CACHE_PREFIX + strHash);

                    // Save data in cache.
                    mobjMemCache.Set(MENU_CACHE_PREFIX + strHash, objMenuret, cacheEntryOptions);


                }

                User userCurrent = null;
                if (!mobjMemCache.TryGetValue(USER_CACHE_PREFIX + strHash, out userCurrent))
                {
                    // Key not in cache, so get data.
                    userCurrent = usr;

                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(120));

                    // Add in key list
                    mobjMenuCacheKeyList.Add(USER_CACHE_PREFIX + strHash);

                    // Save data in cache.
                    mobjMemCache.Set(USER_CACHE_PREFIX + strHash, userCurrent, cacheEntryOptions);


                }
            }
            return objMenuret;
        }

        public List<MenuViewModel> GetMenuForCurrentUser()
        {
            User currUsr = mobjSyncSvc.GetCurrentUser();
            return GetMenuForUser(currUsr);
        }
        public string GetCurrentUserName()
        {
            User currUsr = mobjSyncSvc.GetCurrentUser();
            return currUsr != null ? currUsr.UserName : string.Empty;
        }
        public string GetCurrentUserAbbrev()
        {
            User currUsr = mobjSyncSvc.GetCurrentUser();
            return currUsr != null ? currUsr.Abbrev : string.Empty;
        }


        private List<MenuViewModel> CreateMenuForUser(User usr)
        {
            List<MenuViewModel> objList = new List<MenuViewModel>();
            string strProduct = "DIGISTAT";
            IProductVersionCore objVersion = mobjEnvSvc.GetProductVersion();
            if (objVersion != null)
            {
                strProduct = objVersion.Product;
            }

            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionConfigurator, usr))
            {
                objList.Add(new MenuViewModel { Id = 1, ParentId = null, Enabled = true, Text = "General", Description = "Description General" });

                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.DictionaryView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDASValidationEdit, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, usr)
                    || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWebModulesView, usr)
                )
                {


                    objList.Add(new MenuViewModel
                    {
                        Id = 9,
                        ParentId = 1,
                        Enabled = true,
                        Text = "System Configuration",
                        Description = "Description System Configuration"
                    });
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 10,
                            ParentId = 9,
                            Enabled = true,
                            Text = "System Options",
                            Description = "Modify Product options",
                            Url = new ActionUrl { Action = "SystemOptions", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 11,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Network Configuration",
                            Description = "Configure workstations or mobile devices",
                            Url = new ActionUrl { Action = "NetworkConfiguration", Controller = "Network" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 112,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Hospital Units",
                            Description = "Manage Hospital Units",
                            Url = new ActionUrl { Action = "HospitalUnits", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 13,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Locations",
                            Description = "Manage wards or units in the Healthcare Structure",
                            Url = new ActionUrl { Action = "Locations", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 12,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Bed",
                            Description = "Manage Beds in Locations",
                            Url = new ActionUrl { Action = "Beds", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.DictionaryView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 32,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Dictionary",
                            Description = "Configure Product texts and related translations",
                            Url = new ActionUrl { Action = "Index", Controller = "Dictionary" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 77,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Miscellanea",
                            Description = "Manage Miscellanea table",
                            Url = new ActionUrl { Action = "Miscellanea", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDASValidationEdit, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 78,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Validation",
                            Description = "Configuration Validation",
                            Url = new ActionUrl { Action = "SystemValidation", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 79,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Standard Parameters",
                            Description = "Configuration Standard Parameters",
                            Url = new ActionUrl { Action = "StandardParameters", Controller = "SystemConfiguration" }
                        });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWebModulesView, usr))
                    {
                        objList.Add(new MenuViewModel
                        {
                            Id = 80,
                            ParentId = 9,
                            Enabled = true,
                            Text = "Web Modules",
                            Description = "Configure Web Modules",
                            Url = new ActionUrl { Action = "WebModules", Controller = "SystemConfiguration" }
                        });
                    }
                }

                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemAdministratorView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 14, ParentId = 1, Enabled = true, Text = "System Administration", Description = "Manage user accounts and permissions" });
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 15, ParentId = 14, Enabled = true, Text = "User Accounts", Description = "Manage User Accounts", Url = new ActionUrl { Action = "UserAccounts", Controller = "SystemAdministration" } });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 16, ParentId = 14, Enabled = true, Text = "Permissions", Description = "Manage User Permissions", Url = new ActionUrl { Action = "Permissions", Controller = "SystemAdministration" } });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 71, ParentId = 14, Enabled = true, Text = "Roles", Description = "Manage Roles", Url = new ActionUrl { Action = "Roles", Controller = "SystemAdministration" } });
                    }
                }

                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 700, ParentId = 1, Enabled = true, Text = "Clinical Configuration", Description = "Description Clinical Configuration" });
                    objList.Add(new MenuViewModel { Id = 701, ParentId = 700, Enabled = true, Text = "Custom Lists", Description = "Custom Lists configuration", Url = new ActionUrl { Action = "CustomLists", Controller = "ClinicalConfiguration" } });
                }



                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 2, ParentId = null, Enabled = true, Text = "Connect", Description = "Description Connect" });
                    objList.Add(new MenuViewModel { Id = 17, ParentId = 2, Enabled = true, Text = "Drivers", Description = "Manage Medical Device Drivers" });
                    objList.Add(new MenuViewModel { Id = 18, ParentId = 17, Enabled = true, Text = "Drivers", Description = "Manage Medical Device Drivers", Url = new ActionUrl { Action = "Drivers", Controller = "Connect" } });
                    objList.Add(new MenuViewModel { Id = 19, ParentId = 17, Enabled = true, Text = "Device Driver Management", Description = "Manage drivers for installed Medical Devices", Url = new ActionUrl { Action = "DeviceDriver", Controller = "Connect" } });
                    objList.Add(new MenuViewModel { Id = 20, ParentId = 17, Enabled = true, Text = "Monitor", Description = "View status of installed drivers", Url = new ActionUrl { Action = "Monitor", Controller = "Connect" } });
                    objList.Add(new MenuViewModel { Id = 21, ParentId = 17, Enabled = true, Text = "Actual Device", Description = "Manage patient devices", Url = new ActionUrl { Action = "ActualDevice", Controller = "Connect" } });
                    objList.Add(new MenuViewModel { Id = 22, ParentId = 17, Enabled = true, Text = "Actual Device Images", Description = "Manage image of patient devices", Url = new ActionUrl { Action = "ActualDeviceImages", Controller = "Connect" } });
                    objList.Add(new MenuViewModel { Id = 23, ParentId = 17, Enabled = true, Text = "Collect", Description = "Data collection configuration", Url = new ActionUrl { Action = "Collect", Controller = "Connect" } });
                    objList.Add(new MenuViewModel { Id = 110, ParentId = 2, Enabled = true, Text = "Port Servers", Description = "Description Port Servers" });
                    objList.Add(new MenuViewModel { Id = 111, ParentId = 110, Enabled = true, Text = "Port Servers", Description = "Manage Port Servers", Url = new ActionUrl { Action = "PortServerList", Controller = "ConnectPlus" } });
                    objList.Add(new MenuViewModel { Id = 120, ParentId = 2, Enabled = true, Text = "Waveforms ", Description = "Manage Waveforms" });
                    objList.Add(new MenuViewModel { Id = 121, ParentId = 120, Enabled = true, Text = "Waveform Rules ", Description = "Manage Rules for waveform generation", Url = new ActionUrl { Action = "WaveformRules", Controller = "Connect" } });
                }

                if (
                    mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, usr)
                    ||
                    mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, usr)
                    ||
                    mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDSSView, usr)
                    ||
                    mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, usr)
                    ||
                    mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBView, usr)
                )
                {
                    objList.Add(new MenuViewModel { Id = 200, ParentId = null, Enabled = true, Text = "Modules", Description = "Description Modules" });



                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 210, ParentId = 200, Enabled = true, Text = "Vitals", Description = "Description Vitals" });
                        objList.Add(new MenuViewModel
                        {
                            Id = 211,
                            ParentId = 210,
                            Enabled = true,
                            Text = "Dataset",
                            Description = "Configure Vitals Datasets",
                            Url = new ActionUrl { Action = "Index", Controller = "Vitals" }
                        });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 215, ParentId = 200, Enabled = true, Text = "Common", Description = "Description Common" });
                        //objList.Add(new MenuViewModel { Id = 5, ParentId = null, Enabled = true, Text = "Report Master", Description = "Description Report Master", Url = new ActionUrl { Action = "Index", Controller = "ReportMaster" } });
                        objList.Add(new MenuViewModel { Id = 24, ParentId = 215, Enabled = true, Text = "Report Master Templates", Description = "Configure Report Master Templates", Url = new ActionUrl { Action = "Templates", Controller = "ReportMaster" } });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDSSView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 230, ParentId = 200, Enabled = true, Text = "CDSS", Description = "Description CDSS" });
                        objList.Add(new MenuViewModel
                        {
                            Id = 231,
                            ParentId = 230,
                            Enabled = true,
                            Text = "Rules",
                            Description = "Configure CDSS Rules",
                            Url = new ActionUrl { Action = "Index", Controller = "CDSS" }
                        });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, usr)
                        || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersView, usr)
                        )
                    {
                        objList.Add(new MenuViewModel { Id = 240, ParentId = 200, Enabled = true, Text = "OnLine", Description = "OnlineWeb Configurator" });
                        if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, usr))
                            objList.Add(new MenuViewModel
                            {
                                Id = 241,
                                ParentId = 240,
                                Enabled = true,
                                Text = "Validation Groups",
                                Description = "Configure Validation Groups and Parameters",
                                Url = new ActionUrl { Action = "Index", Controller = "OnlineValidation" }
                            });
                        if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersView, usr))
                            objList.Add(new MenuViewModel
                            {
                                Id = 242,
                                ParentId = 240,
                                Enabled = true,
                                Text = "Custom Queries",
                                Description = "Configure Custom Queries",
                                Url = new ActionUrl { Action = "QueriesParameters", Controller = "OnlineValidation" }
                            });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionTherapyView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 243, ParentId = 200, Enabled = true, Text = "Therapy", Description = "Therapy Configuration" });
                        objList.Add(new MenuViewModel { Id = 244, ParentId = 243, Enabled = true, Text = "Standard Actions", Description = "Standard Actions Configuration", Url = new ActionUrl { Action = "TherapyConfig", Controller = "Therapy" } });
                        objList.Add(new MenuViewModel { Id = 245, ParentId = 243, Enabled = true, Text = "Profiles", Description = "Profiles Configuration", Url = new ActionUrl { Action = "ProfilesConfig", Controller = "Therapy" } });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 250, ParentId = 200, Enabled = true, Text = "Fluid Balance", Description = "Fluid Balance Configurator" });
                        objList.Add(new MenuViewModel { Id = 251, ParentId = 250, Enabled = true, Text = "Fluid Balance Items", Description = "Fluid Balance Configuration", Url = new ActionUrl { Action = "FluidBalanceItems", Controller = "FluidBalance" } });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 260, ParentId = 200, Enabled = true, Text = "Export Scheduler", Description = "Configurator" });
                        objList.Add(new MenuViewModel
                        {
                            Id = 261,
                            ParentId = 260,
                            Enabled = true,
                            Text = "Jobs",
                            Description = "Configure jobs for Export Scheduler",
                            Url = new ActionUrl { Action = "Index", Controller = "ExportScheduler" }
                        });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 270, ParentId = 200, Enabled = true, Text = "Diary", Description = "Diary Configurator" });
                        objList.Add(new MenuViewModel { Id = 271, ParentId = 270, Enabled = true, Text = "Configure", Description = "Diary Configuration", Url = new ActionUrl { Action = "Index", Controller = "DiaryWeb" } });
                    }
                }
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 4, ParentId = null, Enabled = true, Text = "Mobile", Description = "Description Mobile", Url = new ActionUrl { Action = "Index", Controller = "Mobile" } });
                    //objList.Add(new MenuViewModel { Id = 40, ParentId = 4, Enabled = true, Text = "Monitor", Description = "View status of mobile devices", Url = new ActionUrl { Action = "Monitor", Controller = "Mobile" } });
                    objList.Add(new MenuViewModel { Id = 41, ParentId = 4, Enabled = true, Text = "Apk Updates", Description = "Manage mobile application updates", Url = new ActionUrl { Action = "ApkUpdate", Controller = "Apk" } });
                    objList.Add(new MenuViewModel { Id = 42, ParentId = 4, Enabled = true, Text = "Wearables Config", Description = "Create QR Codes for wearables", Url = new ActionUrl { Action = "WearablesConfig", Controller = "Wearables" } });
                    objList.Add(new MenuViewModel { Id = 43, ParentId = 4, Enabled = true, Text = "Positions Config", Description = "Manage position association", Url = new ActionUrl { Action = "PositionConfig", Controller = "Position" } });
                    objList.Add(new MenuViewModel { Id = 44, ParentId = 4, Enabled = true, Text = "Device Manager", Description = "Provision devices", Url = new ActionUrl { Action = "ProvisionConfig", Controller = "Provision" } });
                }



                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 35, ParentId = null, Enabled = true, Text = "Integrations", Description = "Description Integrations" });
                    objList.Add(new MenuViewModel { Id = 36, ParentId = 35, Enabled = true, Text = "Ascom Telligence", Description = "Manage integration with Acom Telligence" });
                    objList.Add(new MenuViewModel { Id = 37, ParentId = 36, Enabled = true, Text = "Servers", Description = "Manage Telligence servers", Url = new ActionUrl { Action = "Servers", Controller = "Telligence" } });
                    objList.Add(new MenuViewModel { Id = 38, ParentId = 36, Enabled = true, Text = "Systems", Description = "Manage Telligence systems", Url = new ActionUrl { Action = "Systems", Controller = "Telligence" } });
                    objList.Add(new MenuViewModel { Id = 38, ParentId = 36, Enabled = true, Text = "Devices", Description = "Manage Telligence devices (Staff Stations)", Url = new ActionUrl { Action = "Devices", Controller = "Telligence" } });
                    objList.Add(new MenuViewModel { Id = 38, ParentId = 36, Enabled = true, Text = "Import", Description = "Import Telligence network configuration", Url = new ActionUrl { Action = "Import", Controller = "Telligence" } });
                }



                objList.Add(new MenuViewModel { Id = 900, ParentId = null, Enabled = true, Text = "Utilities", Description = "System utilities,logs, monitoring" });
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalLogView, usr)
                   || mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDigistatLogView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 901, ParentId = 900, Enabled = true, Text = "Logs", Description = "Logs" });

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalLogView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 910, ParentId = 901, Enabled = true, Text = "Clinical Log", Description = "Displays  the Clinical Log", Url = new ActionUrl { Action = "ClinicalLog", Controller = "Actions" } });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDigistatLogView, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 911, ParentId = 901, Enabled = true, Text = "Digistat Log", Description = "Displays  the Digistat Log", Url = new ActionUrl { Action = "DigistatLog", Controller = "Actions" } });
                    }

                }

                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMessageFlowView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 902, ParentId = 900, Enabled = true, Text = "Messages", Description = "UMS Messages" });
                    objList.Add(new MenuViewModel { Id = 920, ParentId = 902, Enabled = true, Text = "Message Flow", Description = "Displays  the messages dispatched by message center ", Url = new ActionUrl { Action = "MessageFlow", Controller = "Actions" } });
                }

                //if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMonitoringMenuView, usr))
                //{
                //   objList.Add(new MenuViewModel { Id = 903, ParentId = 900, Enabled = true, Text = "Monitoring", Description = "System Monitoring" });
                //   objList.Add(new MenuViewModel { Id = 930, ParentId = 903, Enabled = true, Text = "System Monitoring", Description = "Displays  Digistat System and services information", Url = new ActionUrl { Action = "MainMonitoring", Controller = "SystemMonitoring" } });
                //}


                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLogoutAllConfiguration, usr) ||
                   mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionShutdownAllConfiguration, usr) ||
                   mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionChangeMessageCenter, usr)
                   )
                {
                    objList.Add(new MenuViewModel { Id = 904, ParentId = 900, Enabled = true, Text = "Actions", Description = "Actions" });
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLogoutAllConfiguration, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 940, ParentId = 904, Enabled = true, Text = "Privacy Logout", Description = "Force logout on every workstation", Url = new ActionUrl { Action = "PrivacyLogout", Controller = "Actions" } });
                    }

                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionShutdownAllConfiguration, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 941, ParentId = 904, Enabled = true, Text = $"Shut down every {strProduct} workstation", Url = new ActionUrl { Action = "Shutdown", Controller = "Actions" } });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionChangeMessageCenter, usr))
                    {
                        objList.Add(new MenuViewModel { Id = 942, ParentId = 904, Enabled = true, Text = "Change MessageCenter", Description = "Redirect all connected workstations or mobile devices to another MessageCenter", Url = new ActionUrl { Action = "ChangeMessageCenter", Controller = "Actions" } });
                    }
                    if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkProbeView, usr))
                    {
                        //objList.Add(new MenuViewModel { Id = 912, ParentId = 904, Enabled = true, Text = "Network Probe", Description = "Probe configured workstations or mobile devices", Url = new ActionUrl { Action = "NetworkProbe", Controller = "Actions" } });
                    }
                }




                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSupervisionMenuView, usr))
                {
                    //objList.Add(new MenuViewModel { Id = 905, ParentId = 900, Enabled = true, Text = "High Availability", Description = "High Availability" });
                    //objList.Add(new MenuViewModel { Id = 914, ParentId = 905, Enabled = true, Text = "Monitor", Description = "Displays  the status of services running in High Availability mode", Url = new ActionUrl { Action = "HAMonitor", Controller = "Actions" } });
                    objList.Add(new MenuViewModel { Id = 906, ParentId = 900, Enabled = true, Text = "IT Supervision", Description = "" });
                    objList.Add(new MenuViewModel { Id = 9145, ParentId = 906, Enabled = true, Text = "Dashboard", Description = "", Url = new ActionUrl { Action = "MainMonitoring", Controller = "SystemMonitoring" } });
                }

                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStockManagementView, usr))
                {
                    objList.Add(new MenuViewModel { Id = 800, ParentId = null, Enabled = true, Text = "Stock Management", Description = "Stock" });
                    objList.Add(new MenuViewModel { Id = 810, ParentId = 800, Enabled = true, Text = "Stock resource relation", Description = "Manage Stock resource relation", Url = new ActionUrl { Action = "Details", Controller = "StockManagement" } });
                    objList.Add(new MenuViewModel { Id = 811, ParentId = 800, Enabled = true, Text = "Rooms", Description = "Manage rooms", Url = new ActionUrl { Action = "Rooms", Controller = "StockManagement" } });
                    objList.Add(new MenuViewModel { Id = 812, ParentId = 800, Enabled = true, Text = "Operating Blocks", Description = "Manage operating blocks", Url = new ActionUrl { Action = "OperatingBlocks", Controller = "StockManagement" } });
                }



            }

            return objList;
        }

        private string CalculateHash(User usr)
        {
            string strRet = null;
            if (usr != null)
            {
                strRet = usr.PermissionLevel.ToString() + "|" + usr.PermissionModifier;
            }
            return strRet;
        }
    }
}
