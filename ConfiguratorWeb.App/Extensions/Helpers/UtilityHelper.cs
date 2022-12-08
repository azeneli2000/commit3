using ConfiguratorWeb.App.Enums;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ConfiguratorWeb.App.Extensions.Helpers
{
   public static class UtilityHelper
   {
      public static List<SelectListItem> EnumToStringList<T>(bool addEmptyItem, T selectedElement, bool valAsInteger=false)
      {
         Type enumType = typeof(T);

         if (enumType.BaseType != typeof(Enum))
            throw new ArgumentException("T is not System.Enum");

         List<SelectListItem> enumValList = new List<SelectListItem>();
         if (addEmptyItem)
         {
            enumValList.Add(new SelectListItem { Text = "", Value = "", Selected = ((selectedElement != null && !string.IsNullOrWhiteSpace(selectedElement.ToString())) ? true : false) });
         }

         foreach (var e in Enum.GetValues(typeof(T)))
         {
            var fi = e.GetType().GetField(e.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            //Assumption : enum underlying type is int or short
            if(Enum.GetUnderlyingType(typeof(T))==typeof(int))
            {
               if (selectedElement != null)
               {
                  enumValList.Add(new SelectListItem { Text = (attributes.Length > 0) ? attributes[0].Description : e.ToString(), Value = (valAsInteger ? ((int)(e)).ToString() : (e).ToString()), Selected = ((e).ToString().Equals(selectedElement.ToString()) ? true : false) });
               }
               else
               {
                  enumValList.Add(new SelectListItem { Text = (attributes.Length > 0) ? attributes[0].Description : e.ToString(), Value = (valAsInteger ? ((int)(e)).ToString() : (e).ToString()) });
               }
            }
            else
            {
               if (selectedElement != null)
               {
                  enumValList.Add(new SelectListItem { Text = (attributes.Length > 0) ? attributes[0].Description : e.ToString(), Value = (valAsInteger ? ((short)(e)).ToString() : (e).ToString()), Selected = ((e).ToString().Equals(selectedElement.ToString()) ? true : false) });
               }
               else
               {
                  enumValList.Add(new SelectListItem { Text = (attributes.Length > 0) ? attributes[0].Description : e.ToString(), Value = (valAsInteger ? ((short)(e)).ToString() : (e).ToString()) });
               }
            }
         }

         return enumValList;
      }

      public static Dictionary<int, string> EnumDictionary<T>()
      {
         if (!typeof(T).IsEnum)
            throw new ArgumentException("Type must be an enum");
         return Enum.GetValues(typeof(T))
             .Cast<T>()
             .ToDictionary(t => (int)(object)t, t => t.ToString());
      }

      public static List<SelectListItem>  EnumToListSelectItem<T>()
      {
         if (!typeof(T).IsEnum)
            throw new ArgumentException("Type must be an enum");
         var dict= Enum.GetValues(typeof(T))
             .Cast<T>()
             .ToDictionary(t => (int)(object)t, t => t.ToString());
         return dict.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
      }

      public static List<SelectListItem> DictionaryToListSelectItem<T>(Dictionary<T, string> source, bool addEmptyItem, T selectedElement)
      {
         List<SelectListItem> valList = new List<SelectListItem>();
         if (addEmptyItem) { valList.Add(new SelectListItem { Text = "", Value = "" }); }

         valList.AddRange(source.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }));

         if (selectedElement != null)
         {
            var current = valList.SingleOrDefault(x => x.Value == selectedElement.ToString());
            if (current != null) { current.Selected = true; }
         }

         return valList;
      }



      /// <summary>
      /// Extensions methods that convert string to stream
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public static Stream ToStream(this string value) => ToStream(value, Encoding.UTF8);

      public static Stream ToStream(this string value, Encoding encoding)
                                => new MemoryStream(encoding.GetBytes(value ?? string.Empty));

      public static bool ConvertStringToBool(string value)
      {
         return value == "1" ? true : false;
      }

      public static string ConvertBoolToString(string value)
      {
         return string.Equals(bool.TrueString, value, StringComparison.OrdinalIgnoreCase) ? "1" : "0";
      }

      public static DateTime? GetDateFromString(string value, string format)
      {
         DateTime? resDate = null;
         try
         {
            if (!string.IsNullOrWhiteSpace(value))
            {
               resDate = DateTime.ParseExact(value, format,
                                       System.Globalization.CultureInfo.InvariantCulture);
            }
         }
         catch (Exception)
         {

         }
         return resDate;
      }     
      public static T To<T>(this string text)
      {
         return (T)Convert.ChangeType(text, typeof(T));
      }

      public static String IsMDIDevice(TelligenceDevice  telligenceDevice)
      {
         if (telligenceDevice != null && Configurator.Std.Helpers.TelligenceHelper.IsMDIDevice(telligenceDevice))
         {
            return CommonStrings.NUMERIC_TRUE;
         }
         return CommonStrings.NUMERIC_FALSE;
      }

      #region TEST

      public static List<MenuViewModel> GetMenuList()
      {
         return GetMenuList("");
      }

      public static List<MenuViewModel> GetMenuList(string product)
      {
         //string product = "DIGISTAT";
         //IProductVersionCore prod = mobjDigEnvironmentService.GetProductVersion();
         //if (prod != null)
         //{
         //   product = prod.Product;
         //}
         List<MenuViewModel> fixedMenu = new List<MenuViewModel>();
         try
         {
            fixedMenu.Add(new MenuViewModel { Id = 1, ParentId = null, Enabled = true, Text = "General" });
            fixedMenu.Add(new MenuViewModel { Id = 9, ParentId = 1, Enabled = true, Text = "System Configuration" });
            fixedMenu.Add(new MenuViewModel { Id = 10, ParentId = 9, Enabled = true, Text = "System Options", Url = new ActionUrl { Action = "SystemOptions", Controller = "SystemConfiguration" } });
            fixedMenu.Add(new MenuViewModel { Id = 11, ParentId = 9, Enabled = true, Text = "Network Configuration", Url = new ActionUrl { Action = "NetworkConfiguration", Controller = "Network" } });
            fixedMenu.Add(new MenuViewModel { Id = 12, ParentId = 9, Enabled = true, Text = "Bed", Url = new ActionUrl { Action = "Beds", Controller = "SystemConfiguration" } });
            fixedMenu.Add(new MenuViewModel { Id = 13, ParentId = 9, Enabled = true, Text = "Locations", Url = new ActionUrl { Action = "Locations", Controller = "SystemConfiguration" } });
            fixedMenu.Add(new MenuViewModel { Id = 112, ParentId = 9, Enabled = true, Text = "Hospital Units", Url = new ActionUrl { Action = "HospitalUnits", Controller = "SystemConfiguration" } });

            fixedMenu.Add(new MenuViewModel { Id = 14, ParentId = 1, Enabled = true, Text = "System Administration" });
            fixedMenu.Add(new MenuViewModel { Id = 15, ParentId = 14, Enabled = true, Text = "User Accounts", Url = new ActionUrl { Action = "UserAccounts", Controller = "SystemAdministration" } });
            fixedMenu.Add(new MenuViewModel { Id = 16, ParentId = 14, Enabled = true, Text = "Permissions", Url = new ActionUrl { Action = "Permissions", Controller = "SystemAdministration" } });
            fixedMenu.Add(new MenuViewModel { Id = 75, ParentId = 14, Enabled = true, Text = "Roles", Url = new ActionUrl { Action = "Roles", Controller = "SystemAdministration" } });

            fixedMenu.Add(new MenuViewModel { Id = 2, ParentId = null, Enabled = true, Text = "Connect" });
            fixedMenu.Add(new MenuViewModel { Id = 17, ParentId = 2, Enabled = true, Text = "Drivers" });
            fixedMenu.Add(new MenuViewModel { Id = 18, ParentId = 17, Enabled = true, Text = "Drivers", Url = new ActionUrl { Action = "Drivers", Controller = "Connect" } });
            fixedMenu.Add(new MenuViewModel { Id = 19, ParentId = 17, Enabled = true, Text = "Device Driver Management", Url = new ActionUrl { Action = "DeviceDriver", Controller = "Connect" } });
            fixedMenu.Add(new MenuViewModel { Id = 20, ParentId = 17, Enabled = true, Text = "Monitor", Url = new ActionUrl { Action = "Monitor", Controller = "Connect" } });
            fixedMenu.Add(new MenuViewModel { Id = 21, ParentId = 17, Enabled = true, Text = "Actual Device", Url = new ActionUrl { Action = "ActualDevice", Controller = "Connect" } });
            fixedMenu.Add(new MenuViewModel { Id = 22, ParentId = 17, Enabled = true, Text = "Actual Device Images", Url = new ActionUrl { Action = "ActualDeviceImages", Controller = "Connect" } });
            fixedMenu.Add(new MenuViewModel { Id = 110, ParentId = 2, Enabled = true, Text = "Port Servers" });
            fixedMenu.Add(new MenuViewModel { Id = 111, ParentId = 110, Enabled = true, Text = "Port Servers", Url = new ActionUrl { Action = "PortServerList", Controller = "ConnectPlus" } });




            fixedMenu.Add(new MenuViewModel { Id = 4, ParentId = null, Enabled = true, Text = "Mobile", Url = new ActionUrl { Action = "Index", Controller = "Mobile" } });
            fixedMenu.Add(new MenuViewModel { Id = 40, ParentId = 4, Enabled = true, Text = "Monitor", Url = new ActionUrl { Action = "Monitor", Controller = "Mobile" } });
            fixedMenu.Add(new MenuViewModel { Id = 41, ParentId = 4, Enabled = true, Text = "Apk Updates", Url = new ActionUrl { Action = "ApkUpdate", Controller = "Apk" } });


            fixedMenu.Add(new MenuViewModel { Id = 5, ParentId = null, Enabled = true, Text = "Report Master", Url = new ActionUrl { Action = "Index", Controller = "ReportMaster" } });
            fixedMenu.Add(new MenuViewModel { Id = 24, ParentId = 5, Enabled = true, Text = "Templates", Url = new ActionUrl { Action = "Templates", Controller = "ReportMaster" } });


            fixedMenu.Add(new MenuViewModel { Id = 201, ParentId = null, Enabled = true, Text = "Vitals", Url = new ActionUrl { Action = "Index", Controller = "Vitals" } });
            



            fixedMenu.Add(new MenuViewModel { Id = 35, ParentId = null, Enabled = true, Text = "Integrations" });
            fixedMenu.Add(new MenuViewModel { Id = 36, ParentId = 35, Enabled = true, Text = "Ascom Telligence" });
            fixedMenu.Add(new MenuViewModel { Id = 37, ParentId = 36, Enabled = true, Text = "Servers", Url = new ActionUrl { Action = "Servers", Controller = "Telligence" } });
            fixedMenu.Add(new MenuViewModel { Id = 38, ParentId = 36, Enabled = true, Text = "Systems", Url = new ActionUrl { Action = "Systems", Controller = "Telligence" } });
            fixedMenu.Add(new MenuViewModel { Id = 38, ParentId = 36, Enabled = true, Text = "Devices", Url = new ActionUrl { Action = "Devices", Controller = "Telligence" } });
            fixedMenu.Add(new MenuViewModel { Id = 38, ParentId = 36, Enabled = true, Text = "Import", Url = new ActionUrl { Action = "Import", Controller = "Telligence" } });





            fixedMenu.Add(new MenuViewModel { Id = 25, ParentId = null, Enabled = true, Text = "Actions" });
            fixedMenu.Add(new MenuViewModel { Id = 26, ParentId = 25, Enabled = true, Text = "Network Probe", Url = new ActionUrl { Action = "NetworkProbe", Controller = "Actions" } });
            fixedMenu.Add(new MenuViewModel { Id = 27, ParentId = 25, Enabled = true, Text = "Privacy Logout", Url = new ActionUrl { Action = "PrivacyLogout", Controller = "Actions" } });
            fixedMenu.Add(new MenuViewModel { Id = 28, ParentId = 25, Enabled = true, Text = $"Shut down every {product}", Url = new ActionUrl { Action = "Shutdown", Controller = "Actions" } });
            fixedMenu.Add(new MenuViewModel { Id = 29, ParentId = 25, Enabled = true, Text = "Change MessageCenter", Url = new ActionUrl { Action = "ChangeMessageCenter", Controller = "Actions" } });
            fixedMenu.Add(new MenuViewModel { Id = 30, ParentId = 25, Enabled = true, Text = "High Availability Monitor", Url = new ActionUrl { Action = "HAMonitor", Controller = "Actions" } });

            fixedMenu.Add(new MenuViewModel { Id = 99, ParentId = null, Enabled = true, Text = "Template", Url = new ActionUrl { Action = "ListWithGrid", Controller = "Template" } });

            fixedMenu.Add(new MenuViewModel { Id = 100, ParentId = null, Enabled = true, Text = "Dictionary" , Url = new ActionUrl { Action = "Index", Controller = "Dictionary" } });

         }
         catch (Exception)
         {

            throw;
         }

         return fixedMenu;
      }

      public static List<string> GetApplicationsList()
      {
         List<string> resList = new List<string>();
         resList.Add("BROWSER");
         resList.Add("CODEFINDERWEB");
         resList.Add("CONTROLBAR");
         resList.Add("DAS");
         resList.Add("DIGISTATMOBILE");
         resList.Add("DIGISTATWEB");
         resList.Add("FORMSWEB");
         resList.Add("IDENTITY");
         resList.Add("REPORTMASTER");
         resList.Add("SMARTCENTRAL");
         resList.Add("SMARTCENTRALMOBILE");
         resList.Add("TURBOPATIENT");
         resList.Add("TURBOPATIENTWEB");
         resList.Add("VITALSIGNS");
         resList.Add("VITALSIGNSNET");
         resList.Add("VOICE");
         return resList;
      }
      #region Commented

      ///// <summary>
      ///// IDLocation LocationName   LocationIndex hu_GUID  LocationCode
      ///// 1	ICU	0	NULL ICU
      ///// 2	pippero	1	NULL NULL
      ///// </summary>
      ///// <returns></returns>
      //public static List<LocationViewModel> GetLocations()
      //{
      //   List<LocationViewModel> locations = new List<LocationViewModel>();
      //   locations.Add(new LocationViewModel { LocationCode = "ICU", LocationIndex = 0, ID = 1, LocationName = "ICU" });
      //   locations.Add(new LocationViewModel { LocationCode = "PIP", LocationIndex = 1, ID = 2, LocationName = "pippero" });
      //   return locations;
      //}


      //public static IEnumerable<UserViewModel> GetUsers()
      //{
      //   List<UserViewModel> objList = new List<UserViewModel>();
      //   objList.Add(new UserViewModel { Id = "1", FirstName = "Jose Luis", LastName = "del Rio Rubio", Abbrev = "JDE", EMail = "jdelrio@sjdhospitalbarcelona.org", UserName = "jdelrio", PermissionLevel = 70, PermissionModifier = "Z" });
      //   objList.Add(new UserViewModel { Id = "2", FirstName = "Joan", LastName = "Calzada Hernandez", Abbrev = "JCA", EMail = "jcalzada@sjdhospitalbarcelona.org", UserName = "jcalzada", PermissionLevel = 70, PermissionModifier = "SFOD" });
      //   objList.Add(new UserViewModel { Id = "3", FirstName = "Maria Dulcis", LastName = "Lopez Villellas", Abbrev = "MLO", EMail = "dulcis@sjdhospitalbarcelona.org", UserName = "Dulcis", PermissionLevel = 70, PermissionModifier = "Z" });
      //   objList.Add(new UserViewModel { Id = "4", FirstName = "Maria Jose", LastName = "Perez Recio", Abbrev = "MPE", EMail = "mjperez@sjdhospitalbarcelona.org", UserName = "mjperez", PermissionLevel = 70, PermissionModifier = "Z" });
      //   objList.Add(new UserViewModel { Id = "5", FirstName = "Omar", LastName = "Rodriguez Forner", Abbrev = "ORO", EMail = "orodriguezf@sjdhospitalbarcelona.org", UserName = "orodriguezf", PermissionLevel = 70, PermissionModifier = "SFO" });
      //   objList.Add(new UserViewModel { Id = "6", FirstName = "Siscu", LastName = "Torrents Gomez", Abbrev = "STO", EMail = "torrents@sjdhospitalbarcelona.org", UserName = "torrents", PermissionLevel = 70, PermissionModifier = "Z" });
      //   return objList;
      //}

      //public static IEnumerable<PermissionViewModel> GetPermissions()
      //{
      //   List<PermissionViewModel> objList = new List<PermissionViewModel>();
      //   objList.Add(new PermissionViewModel { Id = 1, FunctionName = "SSC.SYSTEMOPTIONS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 2, FunctionName = "DAS.DRIVER.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 3, FunctionName = "SSC.BEDS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1002, FunctionName = "REPORTMASTER.TEMPLATE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1003, FunctionName = "SSC.NETWORKS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1004, FunctionName = "MENU;QUIT", PermissionCode = "Z", PriorityLevel = 0 });
      //   objList.Add(new PermissionViewModel { Id = 1005, FunctionName = "CHANGE APPLICATION", PermissionCode = "Z", PriorityLevel = 0 });
      //   objList.Add(new PermissionViewModel { Id = 1006, FunctionName = "MENU;QUIT;QUIT DIGISTAT", PermissionCode = "Z", PriorityLevel = 0 });
      //   objList.Add(new PermissionViewModel { Id = 1007, FunctionName = "MENU;ABOUT", PermissionCode = "Z", PriorityLevel = 0 });
      //   objList.Add(new PermissionViewModel { Id = 1008, FunctionName = "VITALSIGNS.DATASET_SETTINGS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1009, FunctionName = "VITALSIGNS.DATASET_ENABLE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1010, FunctionName = "VITALSIGNS.DATASET_ADD", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1011, FunctionName = "VITALSIGNS.DATASET_EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1012, FunctionName = "SSC.LOCATIONS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1013, FunctionName = "MENU;SYSTEM CONFIGURATION;MESSAGE CENTER CONFIGURATION", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1014, FunctionName = "MENU;QUIT;SHUT DOWN AND RESTART", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1015, FunctionName = "MENU;SYSTEM ADMINISTRATION;SHUT DOWN EVERY DIGISTAT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1016, FunctionName = "MENU;SYSTEM ADMINISTRATION;PRIVACY LOGOUT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1017, FunctionName = "MENU;STATISTICS;QUERY ASSISTANT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1018, FunctionName = "PATIENT SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1019, FunctionName = "PATIENT NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1020, FunctionName = "PATIENT TRANSFER", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1021, FunctionName = "PATIENT ADMIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1022, FunctionName = "PATIENT EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1023, FunctionName = "PATIENT DISMISS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1024, FunctionName = "PATIENT REMOVE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1025, FunctionName = "ACCESS ALL LOCATIONS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1026, FunctionName = "INFO", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1027, FunctionName = "MENU;SYSTEM REPORTS;PRINT SCREEN", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1028, FunctionName = "MENU;SYSTEM ADMINISTRATION;NETWORK PROBE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1029, FunctionName = "MENU;CHANGE PASSWORD", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1030, FunctionName = "MENU;SYSTEM ADMINISTRATION;SYSTEM LOG", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1031, FunctionName = "MENU;SYSTEM ADMINISTRATION;APPLICATION LICENSING", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1032, FunctionName = "PATIENT NOTES", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1033, FunctionName = "QUERY DO", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1034, FunctionName = "QUERY EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1035, FunctionName = "QUERY SETUP", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1036, FunctionName = "BROWSER OPEN NEW DOC", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1037, FunctionName = "CONTROLBAR MULTIMONITOR OPEN", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1038, FunctionName = "MENU;MODULES CONFIGURATION;DAS-PORTS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1039, FunctionName = "OSM.OPERATIONKITSETUP.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1040, FunctionName = "OSM.OPERATIONKITMANAGEMENT.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1041, FunctionName = "OSM.GENERICKITMANAGEMENT.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1042, FunctionName = "OSM.EMERGENCYKITASSOCIATION.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1043, FunctionName = "OSM.RESOURCETRANSFER.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1044, FunctionName = "OSM.RESOURCEREQUEST.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1045, FunctionName = "OSM.ORDERS.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1046, FunctionName = "OSM.UNKNOWNRESOURCE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1047, FunctionName = "OSM.UNKNOWNRESOURCE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1048, FunctionName = "OSM.OPERATIONSELECTIONRESOURCESETUP.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1049, FunctionName = "OSM.CONSIGNMENTSTOCKINTRANSIT.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1050, FunctionName = "OSM.OPERATIONSELECTIONRESOURCERETURNWASTE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1051, FunctionName = "OSM.OPERATIONSELECTIONRESOURCELOAN.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1052, FunctionName = "OSM.PICKING.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1053, FunctionName = "OSM.RETURN.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1054, FunctionName = "OSM.WASTE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1055, FunctionName = "OSM.REFILL.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1056, FunctionName = "OSM.INVENTORY.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1057, FunctionName = "OSM.EXPIRED.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1058, FunctionName = "OSM.ADMINISTRATIVEDISCHARGE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1059, FunctionName = "OSM.CABINETS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1060, FunctionName = "OSM.CONSUMPTIVEUSAGES.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1061, FunctionName = "OSM.EMERGENCY.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1062, FunctionName = "OSM.TROLLEY.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1063, FunctionName = "OSM.COSTCENTERPICKING.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1064, FunctionName = "OSM.COSTCENTERRETURN.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1065, FunctionName = "OSM.ALLOCATE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1066, FunctionName = "OSM.MOVEMENTS.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1067, FunctionName = "OSM.KITTRACKING.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1068, FunctionName = "OSM.RESOURCETRANSFERONOPERATION.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1069, FunctionName = "OSM.BASKETREFILL.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1070, FunctionName = "OSM.GENERICKIT.OPERATION.ASSOCIATION", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1071, FunctionName = "OSM.GENERICKIT.OPERATION.ASSOCIATION.WITHANOMALIES", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1072, FunctionName = "OSM.GENERICKIT.OPERATION.ASSOCIATION.EXPIRED", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1073, FunctionName = "OSM.EMERGENCYKIT.OVERRIDE.WARNING", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1074, FunctionName = "OSM.GENERICKIT.GIVEBACK", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1075, FunctionName = "OSM.GENERICKIT.TRANSFER", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1076, FunctionName = "OSM.GENERICKIT.TRANSFER.NOTVALID", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1077, FunctionName = "OSM.GENERICKIT.TRANSFER.EXPIRED", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1078, FunctionName = "OSM.GENERICKIT.NEWKIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1079, FunctionName = "OSM.GENERICKIT.NEWKIT.WITHANOMALIES", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1080, FunctionName = "OSM.OPERATIONKIT.COMPOSITION.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1081, FunctionName = "OSM.OPERATIONKIT.COMPOSITION.CREATE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1082, FunctionName = "OSM.OPERATIONKIT.GIVEBACK", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1083, FunctionName = "OSM.RESOURCEREQUEST.NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1084, FunctionName = "OSM.RESOURCEREQUEST.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1085, FunctionName = "OSM.RESOURCEREQUEST.DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1086, FunctionName = "OSM.RESOURCEREQUEST.FILL", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1087, FunctionName = "OSM.STANDARDRESOURCE.NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1088, FunctionName = "OSM.OPERATIONSELECTIONRESOURCESETUP.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1089, FunctionName = "OSM.OPERATIONSELECTIONRESOURCESETUP.RETURN", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1090, FunctionName = "OSM.UNKNOWNRESOURCE.DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1091, FunctionName = "OSM.FORCESTOPSCAN", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1092, FunctionName = "OSM.FORCEUNLOCKWORKSTATION", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1093, FunctionName = "OSM.OPERATIONSELECTIONSTOCKINTRANSIT.RETURN", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1094, FunctionName = "OSM.ALLOWPICKSTOCKQUANTITYSMALLERIFLOCKED", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1095, FunctionName = "OSM.OPERATIONSELECTIONRESOURCETRANSFERONOPERATION.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1096, FunctionName = "SSC.CALENDARSCHEDULE.FIRSTAVAILABLE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1097, FunctionName = "SSC.CALENDARSCHEDULE.FIRSTEMPTY", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1098, FunctionName = "SSC.CALENDARSCHEDULE.TOMORROW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1099, FunctionName = "SSC.OPERATIONLIST.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1100, FunctionName = "SSC.OPERATIONLIST.REPORTS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1101, FunctionName = "SSC.OPERATIONLIST.CUSTOMITEM_1", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1102, FunctionName = "SSC.CUSTOMFILTERGLOBALASSOCIATION.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1103, FunctionName = "SSC.CUSTOMFILTERASSOCIATION.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1104, FunctionName = "SSC.CUSTOMFILTERCLEARASSOCIATION.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1105, FunctionName = "SSC.CUSTOMFILTERCLEARGLOBALASSOCIATION.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1106, FunctionName = "SSC.OPERATIONRECORD.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1107, FunctionName = "SSC.OPERATIONRECORD.NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1108, FunctionName = "SSC.OPERATIONRECORD.DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1109, FunctionName = "SSC.OPERATIONRECORD.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1110, FunctionName = "SSC.OPERATIONRECORD.SETSTATUS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1111, FunctionName = "SSC.OPERATIONRECORD.SCHEDULE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1112, FunctionName = "SSC.OPERATIONRECORD.SCHEDULEASRESERVE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1113, FunctionName = "SSC.OPERATIONRECORD.SCHEDULEWITHHELP", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1114, FunctionName = "SSC.OPERATIONRECORD.RESCHEDULE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1115, FunctionName = "SSC.OPERATIONRECORD.REPORTS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1116, FunctionName = "SSC.OPERATIONRECORD.PATIENT.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1117, FunctionName = "SSC.OPERATIONRECORD.OPERATION.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1118, FunctionName = "SSC.OPERATIONRECORD.OPERATION.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1119, FunctionName = "SSC.OPERATIONRECORD.OPERATION.OTHERS.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1120, FunctionName = "SSC.OPERATIONRECORD.OPERATION.OTHERS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1121, FunctionName = "SSC.OPERATIONRECORD.OPERATION.DURATION", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1122, FunctionName = "SSC.OPERATIONRECORD.OPERATION.ALLERGIES.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1123, FunctionName = "SSC.OPERATIONRECORD.OPERATION.TDISEASES.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1124, FunctionName = "SSC.OPERATIONRECORD.OPERATION.INFECTIONS.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1125, FunctionName = "SSC.OPERATIONRECORD.OPERATION.SITE.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1126, FunctionName = "SSC.OPERATIONRECORD.OPERATION.POSITION.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1127, FunctionName = "SSC.OPERATIONRECORD.OPERATION.ANESTHESIA.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1128, FunctionName = "SSC.OPERATIONRECORD.DIGISTATCODE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1129, FunctionName = "SSC.OPERATIONRECORD.REQUIREMENTS.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1130, FunctionName = "SSC.OPERATIONRECORD.NEEDEDDEVICES.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1131, FunctionName = "SSC.OPERATIONRECORD.STAFF.HU.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1132, FunctionName = "SSC.OPERATIONRECORD.STAFF.PLANNED.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1133, FunctionName = "SSC.OPERATIONRECORD.COSTCENTER.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1134, FunctionName = "SSC.OPERATIONRECORD.MATERIALS.SELECT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1135, FunctionName = "SSC.OPERATIONRECORD.MATERIALS.NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1136, FunctionName = "SSC.SCHEDULE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1137, FunctionName = "SSC.SCHEDULE.ELECTIVE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1138, FunctionName = "SSC.SCHEDULE.EMERGENCY.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1139, FunctionName = "SSC.SCHEDULE.REPORTS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1140, FunctionName = "SSC.OPERATIONSCHEDULE.LOCK.LEVEL.1", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1141, FunctionName = "SSC.OPERATIONSCHEDULE.LOCK.LEVEL.2", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1142, FunctionName = "SSC.OPERATIONSCHEDULE.LOCK.LEVEL.3", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1143, FunctionName = "SSC.OPERATIONSCHEDULE.UNLOCK.LEVEL.1", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1144, FunctionName = "SSC.OPERATIONSCHEDULE.UNLOCK.LEVEL.2", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1145, FunctionName = "SSC.OPERATIONSCHEDULE.UNLOCK.LEVEL.3", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1146, FunctionName = "SSC.STAFF.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1147, FunctionName = "SSC.SCHEDULE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1148, FunctionName = "SSC.CALENDAROVERVIEW.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1149, FunctionName = "SSC.STAFFMANAGEMENT.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1150, FunctionName = "SSC.STAFFMANAGEMENT.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1151, FunctionName = "SSC.STAFFMANAGEMENT.REPORTS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1152, FunctionName = "SSC.STAFFMANAGEMENT.OVERVIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1153, FunctionName = "CODEFINDER ADD NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1154, FunctionName = "CODEFINDER EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1155, FunctionName = "CODEFINDER DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1156, FunctionName = "CODEFINDER OPEN", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1157, FunctionName = "DIARY VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1158, FunctionName = "SCORE.CONFIGURATOR.NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1159, FunctionName = "SCORE.CONFIGURATOR.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1160, FunctionName = "SCORE.CONFIGURATOR.DISABLE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1161, FunctionName = "SCORE.CONFIGURATOR.EDIT.SYSTEM", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1162, FunctionName = "SCORE.CONFIGURATOR.DISABLE.SYSTEM", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1163, FunctionName = "SSC.CONFIGURATOR.SAMPLE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1164, FunctionName = "SSC.CONFIGURATOR.DIAGNOSISLAYERCONFIG.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1165, FunctionName = "SSC.CONFIGURATOR.DIAGNOSISLAYERCONFIG.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1166, FunctionName = "SSC.CONFIGURATOR.DIAGNOSISLAYER.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1167, FunctionName = "SSC.CONFIGURATOR.DIAGNOSISLAYER.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1168, FunctionName = "SSC.CONFIGURATOR.DIAGNOSIS.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1169, FunctionName = "SSC.CONFIGURATOR.DIAGNOSIS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1170, FunctionName = "SSC.CONFIGURATOR.DIAGNOSISSDLAYERLINK.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1171, FunctionName = "SSC.CONFIGURATOR.DIAGNOSISSDLAYERLINK.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1172, FunctionName = "SSC.CONFIGURATOR.STANDARDPHRASE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1173, FunctionName = "SSC.CONFIGURATOR.STANDARDPHRASE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1174, FunctionName = "SSC.CONFIGURATOR.COSTCENTRETYPE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1175, FunctionName = "SSC.CONFIGURATOR.COSTCENTRETYPE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1176, FunctionName = "SSC.CONFIGURATOR.COSTCENTRE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1177, FunctionName = "SSC.CONFIGURATOR.COSTCENTRE.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1178, FunctionName = "SSC.CONFIGURATOR.HU_CC_LINK.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1179, FunctionName = "SSC.CONFIGURATOR.HU_CC_LINK.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1180, FunctionName = "SSC.CONFIGURATOR.CC_CC_LINK.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1181, FunctionName = "SSC.CONFIGURATOR.CC_CC_LINK.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1182, FunctionName = "SMARTCENTRAL_MYPATIENT_CHANGE", PermissionCode = "Z", PriorityLevel = 90 });
      //   objList.Add(new PermissionViewModel { Id = 1183, FunctionName = "SMARTCENTRAL_ALLOW_NOTIFICATION", PermissionCode = "Z", PriorityLevel = 70 });
      //   objList.Add(new PermissionViewModel { Id = 1184, FunctionName = "SCC.USERS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1185, FunctionName = "SCC.PERMISSIONS.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1186, FunctionName = "DASHBOARD VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1187, FunctionName = "IMAGEBANKNET.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1188, FunctionName = "IMAGEBANKNET.EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1189, FunctionName = "IMAGEBANKNET.DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1190, FunctionName = "IMAGEBANKNET.CAPTURE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1191, FunctionName = "IMAGEBANKNET.EXPORT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1192, FunctionName = "IMAGEBANKNET.SETUP", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1193, FunctionName = "SCORE VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1194, FunctionName = "SCORE NEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1195, FunctionName = "SCORE EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1196, FunctionName = "SCORE NOT OWNED EDIT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1197, FunctionName = "SCORE SETUP", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1198, FunctionName = "SCORE DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1199, FunctionName = "SCORE NOT OWNED DELETE", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1200, FunctionName = "SCORE PRINT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1201, FunctionName = "NOTES", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1202, FunctionName = "MESSENGER NEW MAIL", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1203, FunctionName = "MESSENGER VIEW OUTBOX SYSTEM MAIL", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1204, FunctionName = "MESSENGER SEND TO GROUP", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1205, FunctionName = "MESSENGER SEND TO USERS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1206, FunctionName = "MESSENGER SEND TO PATIENT", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1207, FunctionName = "MESSENGER SEND TO HU", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1208, FunctionName = "FORM 1- UtilityICU", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1209, FunctionName = "MENU;SYSTEM CONFIGURATION;EDIT TABLES AND FIELDS", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1210, FunctionName = "VITALSIGNS.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1211, FunctionName = "VOICE.VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1212, FunctionName = "VOICE.NOTE_ADD", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1213, FunctionName = "PATIENT_DATA_VIEW", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1214, FunctionName = "FLUID BALANCE: DATA ENTRY", PermissionCode = "Z", PriorityLevel = 98 });
      //   objList.Add(new PermissionViewModel { Id = 1215, FunctionName = "FORMS DESIGN", PermissionCode = "Z", PriorityLevel = 98 });
      //   return objList;
      //}

      //      public static IEnumerable<DriverViewModel> GetDrivers()
      //      {
      //         List<DriverViewModel> objList = new List<DriverViewModel>();

      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "0454c271-55bc-4358-8a8e-24643b9bffc6",
      //            Version = 1,
      //            DriverName = "Carefusion_Demo",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 229377,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"[HostName]\" SocketPort=\"66000\" ComPort=\"1\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"TestParam1\" Value=\"TestValue1\" Description=\"Here there is a description about this specific parameter and how to configure and use it\"/><CustomParam  Name=\"TestParam2\" Value=\"TestValue2\" Description=\"\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Carefusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "1.0",
      //            FormatStyle = @"\dev{1}  
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%} , \s1{%8016#1.value%} \s3minutes|}}else{\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r1\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r1\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r1\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}} ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "198dbc1b-b663-42dc-b4d5-0711f7f5f2b9",
      //            Version = 1,
      //            DriverName = "Evita_Demo",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 229599,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"[HostName]\" SocketPort=\"66000\" ComPort=\"1\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"TestParam1\" Value=\"TestValue1\" Description=\"Here there is a description about this specific parameter and how to configure and use it\"/><CustomParam  Name=\"TestParam2\" Value=\"TestValue2\" Description=\"\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Draeger",
      //            Device = "Evita",
      //            DriverModel = "Evita XL",
      //            DeviceType = "3",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "1.0",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2\if({%4001.value%} < 15 || {%4001.value%} > 20){\cFF0000\if({%4001.value%} < 15){ LOW}else{ HIGH}}  
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel { Id = "58cea85d-da42-4c3d-96c8-542d29a401a9", Version = 1, DriverName = "MEDIBUSX", DriverVersion = "1.0", IsWrapper = false, StreamSize = 269954, FileCount = 5, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"7\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"SkipErrors\" Value=\"false\" Description=\"Skip protocol errors\"/></CustomParameters></CommConfiguration>", Manufacturer = "DRAGER", Device = "Medibus.X protocol devices", DriverModel = "Medibus.X", DeviceType = "3", DriverVersionBuild = "1.2", HardwareRelease = "", SoftwareRelease = "", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "58cea85d-da42-4c3d-96c8-542d29a401a9",
      //            Version = 2,
      //            DriverName = "MEDIBUSX",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 269954,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"7\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"SkipErrors\" Value=\"false\" Description=\"Skip protocol errors\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "DRAGER",
      //            Device = "Medibus.X protocol devices",
      //            DriverModel = "Medibus.X",
      //            DeviceType = "3",
      //            DriverVersionBuild = "1.2",
      //            HardwareRelease = "",
      //            SoftwareRelease = "",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2\if({%4001.value%} < 15 || {%4001.value%} > 20){\cFF0000\if({%4001.value%} < 15){ LOW}else{ HIGH}}  
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "58cea85d-da42-4c3d-96c8-542d29a401a9",
      //            Version = 3,
      //            DriverName = "MEDIBUSX",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 269954,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"7\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"SkipErrors\" Value=\"false\" Description=\"Skip protocol errors\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "DRAGER",
      //            Device = "Medibus.X protocol devices",
      //            DriverModel = "Medibus.X",
      //            DeviceType = "3",
      //            DriverVersionBuild = "1.2",
      //            HardwareRelease = "",
      //            SoftwareRelease = "",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%4512.value%} 
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}
      //\r7\s1 {%4520.name%}: {%4520.value%} \s3{%4520.unit%}
      //\r7\s1 {%4510.name%}: {%4510.value%} \s3{%4510.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel { Id = "6f972c1f-8c82-4416-bffd-5f384a84ea90", Version = 1, DriverName = "PRISMAFLEX", DriverVersion = "1.0", IsWrapper = false, StreamSize = 244032, FileCount = 5, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1,0\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"PrismaFlex\" SocketPort=\"3002\" ComPort=\"1\" Baud=\"10020\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>", Manufacturer = "Gambro Lundia AB (Hospal)", Device = "PrismaFlex", DriverModel = "PrismaFlex", DeviceType = "5", DriverVersionBuild = "1.4", HardwareRelease = "", SoftwareRelease = "4.XX", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel { Id = "6f972c1f-8c82-4416-bffd-5f384a84ea90", Version = 2, DriverName = "PRISMAFLEX", DriverVersion = "1.0", IsWrapper = false, StreamSize = 244032, FileCount = 5, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1,0\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"PrismaFlex\" SocketPort=\"3002\" ComPort=\"1\" Baud=\"10020\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>", Manufacturer = "Gambro Lundia AB (Hospal)", Device = "PrismaFlex", DriverModel = "PrismaFlex", DeviceType = "5", DriverVersionBuild = "1.4", HardwareRelease = "", SoftwareRelease = "4.XX", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "6f972c1f-8c82-4416-bffd-5f384a84ea90",
      //            Version = 3,
      //            DriverName = "PRISMAFLEX",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 244032,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1,0\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"PrismaFlex\" SocketPort=\"3002\" ComPort=\"1\" Baud=\"10020\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Gambro Lundia AB (Hospal)",
      //            Device = "PrismaFlex",
      //            DriverModel = "PrismaFlex",
      //            DeviceType = "5",
      //            DriverVersionBuild = "1.4",
      //            HardwareRelease = "",
      //            SoftwareRelease = "4.XX",
      //            FormatStyle = @"\dev{5}
      //\r1\s1 {%5020.name%} {%5020.value%}  \s3{%5020.unit%} 
      //\r2\s1 {%5500.name%} {%5500.value%}  \s3{%5500.unit%} 
      //\r3\s1 {%5001.name%} {%5001.value%}  \s3{%5001.unit%}
      //\r4\s1 {%5002.name%} {%5002.value%}  \s3{%5002.unit%}
      //\r5\s1 {%5003.name%} {%5003.value%}  \s3{%5003.unit%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel { Id = "8a12071f-dbb0-48bd-915c-aefad2cca20d", Version = 1, DriverName = "SERVOU", DriverVersion = "1.0", IsWrapper = false, StreamSize = 240894, FileCount = 5, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"6\" Baud=\"38400\" DataBits=\"8\" Parity=\"2\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>", Manufacturer = "Maquet", Device = "Servo", DriverModel = "Servo-U, Servo-n, Servo-air", DeviceType = "3", DriverVersionBuild = "1.3", HardwareRelease = "", SoftwareRelease = "0001", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "8a12071f-dbb0-48bd-915c-aefad2cca20d",
      //            Version = 2,
      //            DriverName = "SERVOU",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 240894,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"6\" Baud=\"38400\" DataBits=\"8\" Parity=\"2\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Maquet",
      //            Device = "Servo",
      //            DriverModel = "Servo-U, Servo-n, Servo-air",
      //            DeviceType = "3",
      //            DriverVersionBuild = "1.3",
      //            HardwareRelease = "",
      //            SoftwareRelease = "0001",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2\if({%4001.value%} < 15 || {%4001.value%} > 20){\cFF0000\if({%4001.value%} < 15){ LOW}else{ HIGH}}  
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "8a12071f-dbb0-48bd-915c-aefad2cca20d",
      //            Version = 3,
      //            DriverName = "SERVOU",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 240894,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"6\" Baud=\"38400\" DataBits=\"8\" Parity=\"2\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Maquet",
      //            Device = "Servo",
      //            DriverModel = "Servo-U, Servo-n, Servo-air",
      //            DeviceType = "3",
      //            DriverVersionBuild = "1.3",
      //            HardwareRelease = "",
      //            SoftwareRelease = "0001",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%4512.value%} 
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}
      //\r7\s1 {%4520.name%}: {%4520.value%} \s3{%4520.unit%}
      //\r7\s1 {%4510.name%}: {%4510.value%} \s3{%4510.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "8a12071f-dbb0-48bd-915c-aefad2cca20d",
      //            Version = 4,
      //            DriverName = "SERVOU",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 240894,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"6\" Baud=\"38400\" DataBits=\"8\" Parity=\"2\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Maquet",
      //            Device = "Servo-U",
      //            DriverModel = "Servo-U, Servo-n, Servo-air",
      //            DeviceType = "3",
      //            DriverVersionBuild = "1.3",
      //            HardwareRelease = "",
      //            SoftwareRelease = "0001",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%4512.value%} 
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}
      //\r7\s1 {%4520.name%}: {%4520.value%} \s3{%4520.unit%}
      //\r7\s1 {%4510.name%}: {%4510.value%} \s3{%4510.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel { Id = "a19d82f3-b698-4bf7-b922-60f48599c09d", Version = 1, DriverName = "SERVO", DriverVersion = "1.0", IsWrapper = false, StreamSize = 249741, FileCount = 5, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"4\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>", Manufacturer = "Maquet", Device = "Servo", DriverModel = "Servo-i, Servo-s, Servo Ventilator 300/300A", DeviceType = "3", DriverVersionBuild = "3.4", HardwareRelease = "4.0", SoftwareRelease = "", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "a19d82f3-b698-4bf7-b922-60f48599c09d",
      //            Version = 2,
      //            DriverName = "SERVO",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 249741,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"4\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Maquet",
      //            Device = "Servo",
      //            DriverModel = "Servo-i, Servo-s, Servo Ventilator 300/300A",
      //            DeviceType = "3",
      //            DriverVersionBuild = "3.4",
      //            HardwareRelease = "4.0",
      //            SoftwareRelease = "",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2\if({%4001.value%} < 15 || {%4001.value%} > 20){\cFF0000\if({%4001.value%} < 15){ LOW}else{ HIGH}}  
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "a19d82f3-b698-4bf7-b922-60f48599c09d",
      //            Version = 3,
      //            DriverName = "SERVO",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 249741,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"4\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Maquet",
      //            Device = "Servo",
      //            DriverModel = "Servo-i, Servo-s, Servo Ventilator 300/300A",
      //            DeviceType = "3",
      //            DriverVersionBuild = "3.4",
      //            HardwareRelease = "4.0",
      //            SoftwareRelease = "",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%4512.value%} 
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}
      //\r7\s1 {%4520.name%}: {%4520.value%} \s3{%4520.unit%}
      //\r7\s1 {%4510.name%}: {%4510.value%} \s3{%4510.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "a19d82f3-b698-4bf7-b922-60f48599c09d",
      //            Version = 4,
      //            DriverName = "SERVO",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 249741,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"1\" ReceivingDataMode=\"0\" ConnectionType=\"1\" Hostname=\"\" SocketPort=\"0\" ComPort=\"4\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters></CustomParameters></CommConfiguration>",
      //            Manufacturer = "Maquet",
      //            Device = "Servo-i",
      //            DriverModel = "Servo-i, Servo-s, Servo Ventilator 300/300A",
      //            DeviceType = "3",
      //            DriverVersionBuild = "3.4",
      //            HardwareRelease = "4.0",
      //            SoftwareRelease = "",
      //            FormatStyle = @"\dev{3}  
      //\r1\s1 {%4512.value%} 
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%4001.unit%} \s2
      //\r1\s1 {%4017.name%}: \s2 {%4017.value%} \s3{%4017.unit%}  
      //\r2\s1 PSF: {%4061.value%} \s3{%4061.unit%}  
      //\r3\s1 {%4013.name%}: {%4013.value%} \s3{%4013.unit%}  
      //\r4\s1 {%4020.name%}: {%4020.value%} \s3{%4020.unit%}  
      //\r5\s1 {%7133.name%}: {%7133.value%} \s3{%7133.unit%}  
      //\r6\s1 {%4006.name%}: {%4006.value%} \s3{%4006.unit%}  
      //\r7\s1 {%4010.name%}: {%4010.value%} \s3{%4010.unit%}
      //\r7\s1 {%4520.name%}: {%4520.value%} \s3{%4520.unit%}
      //\r7\s1 {%4510.name%}: {%4510.value%} \s3{%4510.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel { Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29", Version = 1, DriverName = "AGW", DriverVersion = "1.0", IsWrapper = false, StreamSize = 256396, FileCount = 6, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>", Manufacturer = "CareFusion", Device = "AGW", DriverModel = "AGW", DeviceType = "1", DriverVersionBuild = "5.0", HardwareRelease = "", SoftwareRelease = "1.4", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 2,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}
      //{%8005#X.name%}
      //{%8003#X.name%} - {%8001#X.name%}
      //{%8007#X.name%} {%8010#X.name%}{%8009#X.name%} ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 3,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%} , \s1{%8016#1.value%} \s3minutes|}}else{\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r1\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r1\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r1\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}} ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 4,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%} , \s1{%8016#1.value%} \s3minutes|}}else{\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r1\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r1\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r1\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}} ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 5,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s2Carefusion Pumps \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%} , \s1{%8016#1.value%} \s3minutes|}}else{\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%} , \s1{%8016#5.value%} \s3minutes|}}else{\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%} , \s1{%8016#6.value%} \s3minutes|}}else{\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%} , \s1{%8016#7.value%} \s3minutes|}}else{\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%} , \s1{%8016#8.value%} \s3minutes|}}else{\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%} , \s1{%8016#9.value%} \s3minutes|}}else{\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%} , \s1{%8016#10.value%} \s3minutes|}}else{\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%} , \s1{%8016#11.value%} \s3minutes|}}else{\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%} , \s1{%8016#12.value%} \s3minutes|}}else{\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 6,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%} , \s1{%8016#1.value%} \s3minutes|}}else{\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%} , \s1{%8016#5.value%} \s3minutes|}}else{\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%} , \s1{%8016#6.value%} \s3minutes|}}else{\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%} , \s1{%8016#7.value%} \s3minutes|}}else{\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%} , \s1{%8016#8.value%} \s3minutes|}}else{\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%} , \s1{%8016#9.value%} \s3minutes|}}else{\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%} , \s1{%8016#10.value%} \s3minutes|}}else{\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%} , \s1{%8016#11.value%} \s3minutes|}}else{\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%} , \s1{%8016#12.value%} \s3minutes|}}else{\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 7,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%} , \s1{%8016#1.value%} \s3minutes|}}else{\?{ , {%8009#1.value%} \s3{%8009#1.unit%}\s1 , {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%} , \s1{%8016#5.value%} \s3minutes|}}else{\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%} , \s1{%8016#6.value%} \s3minutes|}}else{\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%} , \s1{%8016#7.value%} \s3minutes|}}else{\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%} , \s1{%8016#8.value%} \s3minutes|}}else{\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%} , \s1{%8016#9.value%} \s3minutes|}}else{\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%} , \s1{%8016#10.value%} \s3minutes|}}else{\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%} , \s1{%8016#11.value%} \s3minutes|}}else{\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%} , \s1{%8016#12.value%} \s3minutes|}}else{\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 8,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%} , \s1{%8016#2.value%} \s3minutes|}}else{\?{ , {%8009#2.value%} \s3{%8009#2.unit%}\s1 , {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%} , \s1{%8016#3.value%} \s3minutes|}}else{\?{ , {%8009#3.value%} \s3{%8009#3.unit%}\s1 , {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%} , \s1{%8016#4.value%} \s3minutes|}}else{\?{ , {%8009#4.value%} \s3{%8009#4.unit%}\s1 , {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%} , \s1{%8016#5.value%} \s3minutes|}}else{\?{ , {%8009#5.value%} \s3{%8009#5.unit%}\s1 , {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%} , \s1{%8016#6.value%} \s3minutes|}}else{\?{ , {%8009#6.value%} \s3{%8009#6.unit%}\s1 , {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%} , \s1{%8016#7.value%} \s3minutes|}}else{\?{ , {%8009#7.value%} \s3{%8009#7.unit%}\s1 , {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%} , \s1{%8016#8.value%} \s3minutes|}}else{\?{ , {%8009#8.value%} \s3{%8009#8.unit%}\s1 , {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%} , \s1{%8016#9.value%} \s3minutes|}}else{\?{ , {%8009#9.value%} \s3{%8009#9.unit%}\s1 , {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%} , \s1{%8016#10.value%} \s3minutes|}}else{\?{ , {%8009#10.value%} \s3{%8009#10.unit%}\s1 , {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%} , \s1{%8016#11.value%} \s3minutes|}}else{\?{ , {%8009#11.value%} \s3{%8009#11.unit%}\s1 , {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%} , \s1{%8016#12.value%} \s3minutes|}}else{\?{ , {%8009#12.value%} \s3{%8009#12.unit%}\s1 , {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 9,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s2 \dev{1}{%8003#X.name%} {%8001#X.name%}
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%} - \s1{%8016#1.value%} \s3minutes|}}else{\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%} - \s1{%8016#2.value%} \s3minutes|}}else{\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%} - \s1{%8016#3.value%} \s3minutes|}}else{\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%} - \s1{%8016#4.value%} \s3minutes|}}else{\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%} - \s1{%8016#5.value%} \s3minutes|}}else{\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%} - \s1{%8016#6.value%} \s3minutes|}}else{\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%} - \s1{%8016#7.value%} \s3minutes|}}else{\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%} - \s1{%8016#8.value%} \s3minutes|}}else{\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%} - \s1{%8016#9.value%} \s3minutes|}}else{\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%} - \s1{%8016#10.value%} \s3minutes|}}else{\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%} - \s1{%8016#11.value%} \s3minutes|}}else{\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%} - \s1{%8016#12.value%} \s3minutes|}}else{\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 10,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s1 \dev{1}{%8003#X.name%} {%8001#X.name%}
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%} - \s1{%8016#1.value%} \s3minutes|}}else{\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%} - \s1{%8016#2.value%} \s3minutes|}}else{\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%} - \s1{%8016#3.value%} \s3minutes|}}else{\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%} - \s1{%8016#4.value%} \s3minutes|}}else{\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%} - \s1{%8016#5.value%} \s3minutes|}}else{\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%} - \s1{%8016#6.value%} \s3minutes|}}else{\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%} - \s1{%8016#7.value%} \s3minutes|}}else{\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%} - \s1{%8016#8.value%} \s3minutes|}}else{\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%} - \s1{%8016#9.value%} \s3minutes|}}else{\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%} - \s1{%8016#10.value%} \s3minutes|}}else{\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%} - \s1{%8016#11.value%} \s3minutes|}}else{\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%} - \s1{%8016#12.value%} \s3minutes|}}else{\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 11,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  {%8003#X.name%} {%8001#X.name%}
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%} - \s1{%8016#1.value%} \s3minutes|}}else{\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%} - \s1{%8016#2.value%} \s3minutes|}}else{\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%} - \s1{%8016#3.value%} \s3minutes|}}else{\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%} - \s1{%8016#4.value%} \s3minutes|}}else{\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%} - \s1{%8016#5.value%} \s3minutes|}}else{\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%} - \s1{%8016#6.value%} \s3minutes|}}else{\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%} - \s1{%8016#7.value%} \s3minutes|}}else{\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%} - \s1{%8016#8.value%} \s3minutes|}}else{\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%} - \s1{%8016#9.value%} \s3minutes|}}else{\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%} - \s1{%8016#10.value%} \s3minutes|}}else{\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%} - \s1{%8016#11.value%} \s3minutes|}}else{\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%} - \s1{%8016#12.value%} \s3minutes|}}else{\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 12,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1 \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%} - \s1{%8016#1.value%} \s3minutes|}}else{\?{ - {%8009#1.value%} \s3{%8009#1.unit%}\s1 - {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%} - \s1{%8016#2.value%} \s3minutes|}}else{\?{ - {%8009#2.value%} \s3{%8009#2.unit%}\s1 - {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%} - \s1{%8016#3.value%} \s3minutes|}}else{\?{ - {%8009#3.value%} \s3{%8009#3.unit%}\s1 - {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%} - \s1{%8016#4.value%} \s3minutes|}}else{\?{ - {%8009#4.value%} \s3{%8009#4.unit%}\s1 - {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%} - \s1{%8016#5.value%} \s3minutes|}}else{\?{ - {%8009#5.value%} \s3{%8009#5.unit%}\s1 - {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%} - \s1{%8016#6.value%} \s3minutes|}}else{\?{ - {%8009#6.value%} \s3{%8009#6.unit%}\s1 - {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%} - \s1{%8016#7.value%} \s3minutes|}}else{\?{ - {%8009#7.value%} \s3{%8009#7.unit%}\s1 - {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%} - \s1{%8016#8.value%} \s3minutes|}}else{\?{ - {%8009#8.value%} \s3{%8009#8.unit%}\s1 - {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%} - \s1{%8016#9.value%} \s3minutes|}}else{\?{ - {%8009#9.value%} \s3{%8009#9.unit%}\s1 - {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%} - \s1{%8016#10.value%} \s3minutes|}}else{\?{ - {%8009#10.value%} \s3{%8009#10.unit%}\s1 - {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%} - \s1{%8016#11.value%} \s3minutes|}}else{\?{ - {%8009#11.value%} \s3{%8009#11.unit%}\s1 - {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%} - \s1{%8016#12.value%} \s3minutes|}}else{\?{ - {%8009#12.value%} \s3{%8009#12.unit%}\s1 - {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 13,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2Carefusion Pumps \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 {%8003#X.name%} {%8001#X.name%} \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 14,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s2Carefusion Pumps \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1 Carefusion Pumps\?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 Carefusion Pumps\?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 Carefusion Pumps\?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 Carefusion Pumps\?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 Carefusion Pumps\?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 Carefusion Pumps\?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 Carefusion Pumps\?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 Carefusion Pumps\?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1Carefusion Pumps \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 15,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"
      //\dev{1}  
      //\r1\s2{%8021#X.name%} \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1 {%8021#X.name%} \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 {%8021#X.name%} \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 {%8021#X.name%} \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8021#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 {%8021#X.name%} \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1{%8021#X.name%}s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 16,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s2{%8021#X.name%} \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1 {%8021#X.name%} \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 {%8021#X.name%} \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 {%8021#X.name%} \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8021#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 {%8021#X.name%} \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1{%8021#X.name%} \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 17,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s1 {%8021#X.name%} \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 {%8021#X.name%} \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 {%8021#X.name%} \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8021#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 {%8021#X.name%} \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 {%8021#X.name%} \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1{%8021#X.name%} \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 18,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s2{%8018#X.name%} \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1 {%8018#X.name%} \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 {%8018#X.name%} \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 {%8018#X.name%} \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 {%8018#X.name%} \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8018#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 {%8018#X.name%} \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 {%8018#X.name%} \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 {%8018#X.name%} \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1{%8018#X.name%}s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 19,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2{%8018#X.name%} \if('{%8003#1.value%}'!='aa'){}{}  
      //\r1\s1 {%8018#X.name%} \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 {%8018#X.name%} \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 {%8018#X.name%} \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 {%8018#X.name%} \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8018#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 {%8018#X.name%} \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 {%8018#X.name%} \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 {%8018#X.name%} \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1{%8018#X.name%}s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 20,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  
      //\r1\s2 \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1  \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8003#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 21,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1}  {%8018#X.name%}
      //\r1\s2 \if('{%8003#6.value%}'!='aa'){}{}  
      //\r1\s1  \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8003#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 22,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2  {%8018#X.name%} 
      //\r1\s1  \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 {%8003#X.name%} \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 23,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2  {%8018#X.name%} 
      //\r1\s1  \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1{%8018#X.name%}  \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 24,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2  {%8018#X.name%} 
      //\r1\s1  \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1{%8018#X.value%}  \?{P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 25,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2  {%8018#X.name%} 
      //\r1\s1  \?{P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?{P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?{P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?{P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \?{%8018#5.value%} {P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?{P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?{P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?{P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?{P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?{P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?{P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?{P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 26,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2  {%8018#X.name%} 
      //\r1\s1  \? {%8003#1.name%} {P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \? {%8003#2.name%} {P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \? {%8003#3.name%} {P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \? {%8003#4.name%} {P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8003#5.name%} {P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \? {%8003#6.name%} {P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8003#7.name%} {P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \? {%8003#8.name%} {P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \? {%8003#9.name%} {P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? {%8003#10.name%} {P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8003#11.name%} {P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8003#12.name%} {P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 27,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s2  {%8003#X.name%} 
      //\r1\s1  \? {%8003#1.name%} {P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \? {%8003#2.name%} {P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \? {%8003#3.name%} {P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \? {%8003#4.name%} {P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8003#5.name%} {P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \? {%8003#6.name%} {P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8003#7.name%} {P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \? {%8003#8.name%} {P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \? {%8003#9.name%} {P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? {%8003#10.name%} {P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8003#11.name%} {P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8003#12.name%} {P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 28,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1  \? {%8003#1.name%} {P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \? {%8003#2.name%} {P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \? {%8003#3.name%} {P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \? {%8003#4.name%} {P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8003#5.name%} {P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \? {%8003#6.name%} {P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8003#7.name%} {P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \? {%8003#8.name%} {P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \? {%8003#9.name%} {P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? {%8003#10.name%} {P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8003#11.name%} {P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8003#12.name%} {P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 29,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1  \? {%8003#1.name%} {P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \? {%8003#2.name%} {P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \? {%8003#3.name%} {P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \? {%8003#4.name%} {P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8003#5.value%} {P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \? {%8003#6.name%} {P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8003#7.name%} {P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \? {%8003#8.name%} {P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \? {%8003#9.name%} {P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? {%8003#10.name%} {P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8003#11.name%} {P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8003#12.name%} {P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 30,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1  \? {%8003#1.name%} {P{%8007#1.devicenumber%}|P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \? {%8003#2.name%} {P{%8007#2.devicenumber%}|P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \? {%8003#3.name%} {P{%8007#3.devicenumber%}|P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \? {%8003#4.name%} {P{%8007#4.devicenumber%}|P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%} {P{%8007#5.devicenumber%}|P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \? {%8003#6.name%} {P{%8007#6.devicenumber%}|P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8003#7.name%} {P{%8007#7.devicenumber%}|P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \? {%8003#8.name%} {P{%8007#8.devicenumber%}|P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \? {%8003#9.name%} {P{%8007#9.devicenumber%}|P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? {%8003#10.name%} {P{%8007#10.devicenumber%}|P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8003#11.name%} {P{%8007#11.devicenumber%}|P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8003#12.name%} {P{%8007#12.devicenumber%}|P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 31,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1  \? {%8003#1.name%} { |P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \? {%8003#2.name%} { |P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \? {%8003#3.name%} { |P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \? {%8003#4.name%} { |P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%} { |P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \? {%8003#6.name%} { |P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8003#7.name%} { |P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \? {%8003#8.name%} { |P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \? {%8003#9.name%} { |P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? {%8003#10.name%} { |P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8003#11.name%} { |P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8003#12.name%} { |P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 32,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1  \? { |P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?  { |P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?  { |P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?  { |P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%} { |P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1  \?  { |P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?  { |P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?  { |P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?  { |P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?  { |P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?  { |P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?  { |P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 33,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1  \? { |P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1  \?  { |P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1  \?  { |P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1  \?  { |P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%} { |P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \?{ |P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \?  { |P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1  \?  { |P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1  \?  { |P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \?  { |P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \?  { |P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \?  { |P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 34,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1 \? { |P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \? { |P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \? { |P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \? { |P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%} { |P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \? { |P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? { |P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \? { |P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \? { |P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1s \? { |P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? { |P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? { |P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 35,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1 \? {%8018#1.value%}{ |P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \? {%8018#2.value%}{ |P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \? {%8018#3.value%}{ |P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \? {%8018#4.value%}{ |P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%}{ |P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \? {%8018#6.value%}{ |P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8018#7.value%}{ |P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \? {%8018#8.value%}{ |P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \? {%8018#9.value%}{ |P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \? {%8018#10.value%}{ |P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8018#11.value%}{ |P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8018#12.value%}{ |P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "b364feb8-cf42-4df3-83b6-b6f1f1dd3c29",
      //            Version = 36,
      //            DriverName = "AGW",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 256396,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"1\" SupportedCommConnectionType=\"\" ReceivingDataMode=\"0\" ConnectionType=\"0\" Hostname=\"127.0.0.1\" SocketPort=\"5020\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"RecordData\" Value=\"false\" Description=\"If true, all incoming messages will be recorded on a xml file.\"/><CustomParam  Name=\"DemoMode\" Value=\"false\" Description=\"If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "CareFusion",
      //            Device = "AGW",
      //            DriverModel = "AGW",
      //            DeviceType = "1",
      //            DriverVersionBuild = "5.0",
      //            HardwareRelease = "",
      //            SoftwareRelease = "1.4",
      //            FormatStyle = @"\dev{1} 
      //\r1\s1 \? {%8018#1.value%} { |P{%8007#1.devicenumber%}} : {%8003#1.value%} \if('{%8007#1.value%}'!=''){{%8007#1.value%} \s3{%8007#1.unit%}\s1}else{}\if('{%8016#1.value%}'!=''){\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}  \s1{%8016#1.value%} \s3minutes|}}else{\?{  {%8009#1.value%} \s3{%8009#1.unit%}\s1  {%8001#1.value%} \s3{%8001#1.unit%}|}}  
      //\r2\s1 \? {%8018#2.value%} { |P{%8007#2.devicenumber%}} : {%8003#2.value%} \if('{%8007#2.value%}'!=''){{%8007#2.value%} \s3{%8007#2.unit%}\s1}else{}\if('{%8016#2.value%}'!=''){\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}  \s1{%8016#2.value%} \s3minutes|}}else{\?{  {%8009#2.value%} \s3{%8009#2.unit%}\s1  {%8001#2.value%} \s3{%8001#2.unit%}|}}  
      //\r3\s1 \? {%8018#3.value%} { |P{%8007#3.devicenumber%}} : {%8003#3.value%} \if('{%8007#3.value%}'!=''){{%8007#3.value%} \s3{%8007#3.unit%}\s1}else{}\if('{%8016#3.value%}'!=''){\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}  \s1{%8016#3.value%} \s3minutes|}}else{\?{  {%8009#3.value%} \s3{%8009#3.unit%}\s1  {%8001#3.value%} \s3{%8001#3.unit%}|}}  
      //\r4\s1 \? {%8018#4.value%} { |P{%8007#4.devicenumber%}} : {%8003#4.value%} \if('{%8007#4.value%}'!=''){{%8007#4.value%} \s3{%8007#4.unit%}\s1}else{}\if('{%8016#4.value%}'!=''){\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}  \s1{%8016#4.value%} \s3minutes|}}else{\?{  {%8009#4.value%} \s3{%8009#4.unit%}\s1  {%8001#4.value%} \s3{%8001#4.unit%}|}}
      //\r1\s1 \? {%8018#5.value%} { |P{%8007#5.devicenumber%}} : {%8003#5.value%} \if('{%8007#5.value%}'!=''){{%8007#5.value%} \s3{%8007#5.unit%}\s1}else{}\if('{%8016#5.value%}'!=''){\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}  \s1{%8016#5.value%} \s3minutes|}}else{\?{  {%8009#5.value%} \s3{%8009#5.unit%}\s1  {%8001#5.value%} \s3{%8001#5.unit%}|}}  
      //\r2\s1 \? {%8018#6.value%} { |P{%8007#6.devicenumber%}} : {%8003#6.value%} \if('{%8007#6.value%}'!=''){{%8007#6.value%} \s3{%8007#6.unit%}\s1}else{}\if('{%8016#6.value%}'!=''){\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}  \s1{%8016#6.value%} \s3minutes|}}else{\?{  {%8009#6.value%} \s3{%8009#6.unit%}\s1  {%8001#6.value%} \s3{%8001#6.unit%}|}}  
      //\r3\s1 \? {%8018#7.value%} { |P{%8007#7.devicenumber%}} : {%8003#7.value%} \if('{%8007#7.value%}'!=''){{%8007#7.value%} \s3{%8007#7.unit%}\s1}else{}\if('{%8016#7.value%}'!=''){\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}  \s1{%8016#7.value%} \s3minutes|}}else{\?{  {%8009#7.value%} \s3{%8009#7.unit%}\s1  {%8001#7.value%} \s3{%8001#7.unit%}|}}  
      //\r3\s1 \? {%8018#8.value%} { |P{%8007#8.devicenumber%}} : {%8003#8.value%} \if('{%8007#8.value%}'!=''){{%8007#8.value%} \s3{%8007#8.unit%}\s1}else{}\if('{%8016#8.value%}'!=''){\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}  \s1{%8016#8.value%} \s3minutes|}}else{\?{  {%8009#8.value%} \s3{%8009#8.unit%}\s1  {%8001#8.value%} \s3{%8001#8.unit%}|}}  
      //\r3\s1 \? {%8018#9.value%} { |P{%8007#9.devicenumber%}} : {%8003#9.value%} \if('{%8007#9.value%}'!=''){{%8007#9.value%} \s3{%8007#9.unit%}\s1}else{}\if('{%8016#9.value%}'!=''){\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}  \s1{%8016#9.value%} \s3minutes|}}else{\?{  {%8009#9.value%} \s3{%8009#9.unit%}\s1  {%8001#9.value%} \s3{%8001#9.unit%}|}}  
      //\r3\s1 \? {%8018#10.value%} { |P{%8007#10.devicenumber%}} : {%8003#10.value%} \if('{%8007#10.value%}'!=''){{%8007#10.value%} \s3{%8007#10.unit%}\s1}else{}\if('{%8016#10.value%}'!=''){\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}  \s1{%8016#10.value%} \s3minutes|}}else{\?{  {%8009#10.value%} \s3{%8009#10.unit%}\s1  {%8001#10.value%} \s3{%8001#10.unit%}|}}  
      //\r3\s1 \? {%8018#11.value%} { |P{%8007#11.devicenumber%}} : {%8003#11.value%} \if('{%8007#11.value%}'!=''){{%8007#11.value%} \s3{%8007#11.unit%}\s1}else{}\if('{%8016#11.value%}'!=''){\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}  \s1{%8016#11.value%} \s3minutes|}}else{\?{  {%8009#11.value%} \s3{%8009#11.unit%}\s1  {%8001#11.value%} \s3{%8001#11.unit%}|}}  
      //\r3\s1 \? {%8018#12.value%} { |P{%8007#12.devicenumber%}} : {%8003#12.value%} \if('{%8007#12.value%}'!=''){{%8007#12.value%} \s3{%8007#12.unit%}\s1}else{}\if('{%8016#12.value%}'!=''){\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}  \s1{%8016#12.value%} \s3minutes|}}else{\?{  {%8009#12.value%} \s3{%8009#12.unit%}\s1  {%8001#12.value%} \s3{%8001#12.unit%}|}}  
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel { Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed", Version = 1, DriverName = "eGateway", DriverVersion = "1.0", IsWrapper = false, StreamSize = 283085, FileCount = 6, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>", Manufacturer = "MindRay", Device = "eGateway", DriverModel = "eGateway", DeviceType = "2", DriverVersionBuild = "1.0", HardwareRelease = "1.0", SoftwareRelease = "6.3.0", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel { Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed", Version = 2, DriverName = "eGateway", DriverVersion = "1.0", IsWrapper = false, StreamSize = 283085, FileCount = 6, EntryExe = "", Note = "", ComToRegister = "", DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>", Manufacturer = "MindRay", Device = "eGateway", DriverModel = "eGateway", DeviceType = "2", DriverVersionBuild = "1.0", HardwareRelease = "1.0", SoftwareRelease = "6.3.0", FormatStyle = @"", AlarmSupport = 2, UseDynamicParameters = false });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 3,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{1}
      //\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} &lt; 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 4,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{1}
      //\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW} 
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 5,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW} 
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 6,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r1@1\s1 SPO2: {%3008.value%} s3{%1002.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW} 
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 7,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r1@1\s1 SPO2: {%3008.value%} s3{%3008.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW} 
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 8,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  {%3009.timestamp:HH:mm:ss%}
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  {%3011.timestamp:HH:mm:ss%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  {%3013.timestamp:HH:mm:ss%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 9,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  {%3009.timestamp:HH:mm:ss%}
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  {%3011.timestamp:HH:mm:ss%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  {%3013.timestamp:HH:mm:ss%}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 10,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  {%3009.timestamp:HH:mm:ss%}
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  {%3011.timestamp:HH:mm:ss%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  {%3013.timestamp:HH:mm:ss%}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 11,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.1",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{1}
      //\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 12,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.1",
      //            IsWrapper = false,
      //            StreamSize = 283085,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"127.0.0.1:56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  {%3009.timestamp:HH:mm:ss%}
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  {%3011.timestamp:HH:mm:ss%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  {%3013.timestamp:HH:mm:ss%}
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 13,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.2",
      //            IsWrapper = false,
      //            StreamSize = 296230,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 14,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.2",
      //            IsWrapper = false,
      //            StreamSize = 296230,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  {%3009.timestamp:HH:mm:ss%}
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  {%3011.timestamp:HH:mm:ss%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  {%3013.timestamp:HH:mm:ss%}
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 15,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.3",
      //            IsWrapper = false,
      //            StreamSize = 283590,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  {%3009.timestamp:HH:mm:ss%}
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  {%3011.timestamp:HH:mm:ss%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  {%3013.timestamp:HH:mm:ss%}
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 16,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.3",
      //            IsWrapper = false,
      //            StreamSize = 283590,
      //            FileCount = 6,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 17,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%} 
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%} 
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%} 
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 18,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%} 
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 19,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 HR: {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r1\s1 {%4144.name%}: {%4144.value%} \s3{%4144.unit%}  
      //\r1\s1 {%4001.name%}: {%4001.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3016.name%}: {%3016.value%} \s3{%3016.unit%}  
      //\r1\s1 {%3017.name%}: {%3017.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3032.name%}: {%3032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4138.name%}: {%4138.value%} \s3{%4138.unit%}  
      //\r1\s1 {%3008.name%}: {%3008.value%} \s3{%3008.unit%}
      //\r1\s1 {%3091.name%}: {%3091.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3050.name%}: {%3050.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3055.name%}: {%3055.value%} \s3{%3017.unit%}  
      //\r1\s1 {%4032.name%}: {%4032.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3056.name%}: {%3056.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3034.name%}: {%3034.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3057.name%}: {%3057.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3033.name%}: {%3033.value%} \s3{%3017.unit%}  
      //\r1\s1 {%7201.name%}: {%7201.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3018.name%}: {%3018.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3019.name%}: {%3019.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3020.name%}: {%3020.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3090.name%}: {%3090.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3031.name%}: {%3031.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3059.name%}: {%3059.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3043.name%}: {%3043.value%} \s3{%3017.unit%}  
      //\r1\s1 {%6001.name%}: {%6001.value%} \s3{%6001.unit%} 
      //\r1\s1 {%6002.name%}: {%6002.value%} \s3{%6002.unit%} 
      //\r1\s1 {%6003.name%}: {%6003.value%} \s3{%6003.unit%} 
      //\r1\s1 {%6020.name%}: {%6020.value%} \s3{%6020.unit%} 
      //\r1\s1 {%9007.name%}: {%9007.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3035.name%}: {%3035.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3036.name%}: {%3036.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3009.name%}: {%3009.value%} \s3{%3017.unit%} 
      //\r1\s1 {%3011.name%}: {%3011.value%} \s3{%3017.unit%}  
      //\r1\s1 {%3013.name%}: {%3013.value%} \s3{%3017.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 20,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 {%2001.name%}: {%2001.value%} {%2001.unit%}
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e1 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e1 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}
      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 21,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 {%2001.name%}: {%2001.value%} {%2001.unit%}
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 {%3008.name%}: {%3008.value%} {%3008.unit%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 22,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 {%2001.name%}: {%2001.value%} {%2001.unit%}
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 {%3008.name%}: {%3008.value%} {%3008.unit%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 23,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.4",
      //            IsWrapper = false,
      //            StreamSize = 284955,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 {%2001.name%}: {%2001.value%} {%2001.unit%}
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 {%4122.name%}: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 {%3008.name%}: {%3008.value%} {%3008.unit%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 24,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.5",
      //            IsWrapper = false,
      //            StreamSize = 286963,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 {%2001.name%}: {%2001.value%} {%2001.unit%}
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 {%4122.name%}: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 {%3008.name%}: {%3008.value%} {%3008.unit%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 25,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.6",
      //            IsWrapper = false,
      //            StreamSize = 285420,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 {%2001.name%}: {%2001.value%} {%2001.unit%}
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 {%4122.name%}: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 {%3008.name%}: {%3008.value%} {%3008.unit%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 26,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.0.6",
      //            IsWrapper = false,
      //            StreamSize = 285420,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/><CustomParam  Name=\"UseDaylightSavingCorrection\" Value=\"true\" Description=\"True to automatically correct the sender’s time zone in the received DateTime fields.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 27,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.1",
      //            IsWrapper = false,
      //            StreamSize = 285896,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.1",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 28,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.2",
      //            IsWrapper = false,
      //            StreamSize = 285896,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.1",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 29,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.2",
      //            IsWrapper = false,
      //            StreamSize = 285896,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.1",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 30,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.2.1",
      //            IsWrapper = false,
      //            StreamSize = 286440,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.2",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 31,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.3",
      //            IsWrapper = false,
      //            StreamSize = 303352,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.3",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 32,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.3.1",
      //            IsWrapper = false,
      //            StreamSize = 303362,
      //            FileCount = 5,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.3",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 33,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.3.2",
      //            IsWrapper = false,
      //            StreamSize = 344937,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.3",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 34,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.4",
      //            IsWrapper = false,
      //            StreamSize = 344937,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.4",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //\r1 ECG: {%2001.value%} ppm
      //\e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //\e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //\e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //\e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //\e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //\e3 Resp: {%4122.value%} {%4122.unit%}
      //\e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //\e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //\e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //\e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //\e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //\e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //\e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //\e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //\e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //\e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //\e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //\e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //\e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //\e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //\e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //\e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //\e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //\e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //\e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //\e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //\e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //\e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //\e9 FP: {%3008.value%}
      //\e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //\e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //\e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 35,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.4.1",
      //            IsWrapper = false,
      //            StreamSize = 345063,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.4",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //                                \r1 ECG: {%2001.value%} ppm
      //                                \e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //                                \e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //                                \e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //                                \e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //                                \e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //                                \e3 Resp: {%4122.value%} {%4122.unit%}
      //                                \e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //                                \e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //                                \e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //                                \e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //                                \e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //                                \e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //                                \e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //                                \e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //                                \e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //                                \e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //                                \e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //                                \e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //                                \e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //                                \e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //                                \e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //                                \e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //                                \e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //                                \e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //                                \e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //                                \e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //                                \e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //                                \e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //                                \e9 FP: {%3008.value%}
      //                                \e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //                                \e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //                                \e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //                                ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 36,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.4.2",
      //            IsWrapper = false,
      //            StreamSize = 345249,
      //            FileCount = 8,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.4",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //                                \r1 ECG: {%2001.value%} ppm
      //                                \e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //                                \e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //                                \e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //                                \e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //                                \e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //                                \e3 Resp: {%4122.value%} {%4122.unit%}
      //                                \e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //                                \e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //                                \e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //                                \e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //                                \e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //                                \e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //                                \e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //                                \e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //                                \e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //                                \e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //                                \e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //                                \e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //                                \e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //                                \e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //                                \e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //                                \e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //                                \e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //                                \e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //                                \e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //                                \e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //                                \e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //                                \e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //                                \e9 FP: {%3008.value%}
      //                                \e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //                                \e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //                                \e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //                                ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 37,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.4.2",
      //            IsWrapper = false,
      //            StreamSize = 348335,
      //            FileCount = 8,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.5",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //                                \r1 ECG: {%2001.value%} ppm
      //                                \e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //                                \e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //                                \e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //                                \e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //                                \e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //                                \e3 Resp: {%4122.value%} {%4122.unit%}
      //                                \e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //                                \e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //                                \e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //                                \e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //                                \e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //                                \e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //                                \e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //                                \e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //                                \e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //                                \e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //                                \e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //                                \e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //                                \e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //                                \e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //                                \e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //                                \e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //                                \e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //                                \e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //                                \e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //                                \e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //                                \e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //                                \e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //                                \e9 FP: {%3008.value%}
      //                                \e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //                                \e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //                                \e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //                                ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "cfeef1e1-293d-4d4c-b8e9-fdca65883eed",
      //            Version = 38,
      //            DriverName = "eGateway",
      //            DriverVersion = "1.5",
      //            IsWrapper = false,
      //            StreamSize = 348335,
      //            FileCount = 8,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2,0\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"0\" Hostname=\"LocalHost\" SocketPort=\"56800\" ComPort=\"0\" Baud=\"0\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"0\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"UseUnitInBedName\" Value=\"false\" Description=\"If true add the unit id in the bed identifier unitId^roomId^bedId, otherwise use only roomId^bedId.\"/><CustomParam  Name=\"AlarmSocket\" Value=\"56801\" Description=\"Is the alarm socket where the Mindray eGateway is configured to send the alert events.\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "MindRay",
      //            Device = "eGateway",
      //            DriverModel = "eGateway",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.5",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "6.3.0",
      //            FormatStyle = @"\dev{2}
      //                                \r1 ECG: {%2001.value%} ppm
      //                                \e1 {%4001.name%}: {%4001.value%} {%4001.unit%}
      //                                \e2 {%3015.name%}: {%3015.value%} {%3015.unit%}
      //                                \e2 {%3016.name%}: {%3016.value%} {%3016.unit%}
      //                                \e2 {%3017.name%}: {%3017.value%} {%3017.unit%}
      //                                \e3 {%4138.name%}: {%4138.value%} {%4138.unit%}
      //                                \e3 Resp: {%4122.value%} {%4122.unit%}
      //                                \e3 {%7010.name%}: {%7010.value%} {%7010.unit%}
      //                                \e4 {%3032.name%}: {%3032.value%} {%3032.unit%}
      //                                \e5 {%3091.name%}: {%3091.value%} {%3091.unit%}
      //                                \e6 {%3050.name%}: {%3050.value%} {%3050.unit%}
      //                                \e7 {%3055.name%}: {%3055.value%} {%3055.unit%}
      //                                \e8 {%4032.name%}: {%4032.value%} {%4032.unit%}
      //                                \e9 {%3056.name%}: {%3056.value%} {%3056.unit%}
      //                                \e9 {%3034.name%}: {%3034.value%} {%3034.unit%}
      //                                \e9 {%3057.name%}: {%3057.value%} {%3057.unit%}
      //                                \e9 {%3033.name%}: {%3033.value%} {%3033.unit%}
      //                                \e9 {%7201.name%}: {%7201.value%} {%7201.unit%}
      //                                \e9 {%3018.name%}: {%3018.value%} {%3018.unit%}
      //                                \e9 {%3019.name%}: {%3019.value%} {%3019.unit%}
      //                                \e9 {%3020.name%}: {%3020.value%} {%3020.unit%}
      //                                \e9 {%3090.name%}: {%3090.value%} {%3090.unit%}
      //                                \e9 {%3031.name%}: {%3031.value%} {%3031.unit%}
      //                                \e9 {%3059.name%}: {%3059.value%} {%3059.unit%}
      //                                \e9 {%3043.name%}: {%3043.value%} {%3043.unit%}
      //                                \e9 {%6001.name%}: {%6001.value%} {%6001.unit%}
      //                                \e9 {%9007.name%}: {%9007.value%} {%9007.unit%}
      //                                \e9 {%3035.name%}: {%3035.value%} {%3035.unit%}
      //                                \e9 {%3036.name%}: {%3036.value%} {%3036.unit%}
      //                                \e9 FP: {%3008.value%}
      //                                \e9 {%3009.name%}: {%3009.value%} {%3009.unit%}
      //                                \e9 {%3011.name%}: {%3011.value%} {%3011.unit%}
      //                                \e9 {%3013.name%}: {%3013.value%} {%3013.unit%}

      //                                ",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });
      //         objList.Add(new DriverViewModel
      //         {
      //            Id = "e00e89a2-400e-469f-8c1f-fb8b8e489536",
      //            Version = 1,
      //            DriverName = "Carescape_Demo",
      //            DriverVersion = "1.0",
      //            IsWrapper = false,
      //            StreamSize = 229593,
      //            FileCount = 7,
      //            EntryExe = "",
      //            Note = "",
      //            ComToRegister = "",
      //            DefaultCommConfiguration = "<CommConfiguration SupportedDriverType=\"2\" SupportedCommConnectionType=\"0\" ReceivingDataMode=\"1\" ConnectionType=\"1\" Hostname=\"[HostName]\" SocketPort=\"66000\" ComPort=\"1\" Baud=\"9600\" DataBits=\"8\" Parity=\"0\" HandShake=\"0\" TCPCommType=\"1\" StopBits=\"1\" USBProducerId=\"\" USBVendorId=\"\" USBSerialId=\"\" SmartCableId=\"\" RtsEnabled=\"0\" DtrEnabled=\"0\"><CustomParameters><CustomParam  Name=\"TestParam1\" Value=\"TestValue1\" Description=\"Here there is a description about this specific parameter and how to configure and use it\"/><CustomParam  Name=\"TestParam2\" Value=\"TestValue2\" Description=\"\"/></CustomParameters></CommConfiguration>",
      //            Manufacturer = "GE",
      //            Device = "Carescape",
      //            DriverModel = "650",
      //            DeviceType = "2",
      //            DriverVersionBuild = "1.0",
      //            HardwareRelease = "1.0",
      //            SoftwareRelease = "1.0",
      //            FormatStyle = @"\dev{2}
      //\r1\s1 {%2001.name%}: {%2001.value%} \s3{%2001.unit%}  
      //\r1@1\s1 AP: {%3015.value:3%}/{%3016.value%} ({%3017.value%}) \s3{%3015.unit%}  
      //\r1@1\s1 NBP: {%3011.value%}/{%3009.value%} ({%3013.value%})\s3{%3009.unit%}  
      //\r2\s1 {%7010.name%}: {%7010.value%} \s3{%7010.unit%} \if({%7010.value%} < 90){\cFF0000LOW}",
      //            AlarmSupport = 2,
      //            UseDynamicParameters = false
      //         });

      //         return objList;
      //      }

      //public static List<DriverCapabilityViewModel> GetCapabilities()
      //{
      //   List<DriverCapabilityViewModel> objList = new List<DriverCapabilityViewModel>();
      //   objList.Add(new DriverCapabilityViewModel { IdParameter = 1, IDUnit = 12, DeviceID = "21", DeviceText = "Barometric Pressure", DeviceUnitText = "mbar", Name = "Pbaro", Unit = "mbar", Mnemonic = "MDC_PRESS_BAROMETRIC", Sporadic = DriverSporadic.ONLINE.ToString(), Type = "VEN", Enabled = true });
      //   objList.Add(new DriverCapabilityViewModel { IdParameter = 2, IDUnit = 13, DeviceID = "22", DeviceText = "Mvespont", DeviceUnitText = "breaths/min", Name = "RR", Unit = "bpm", Mnemonic = "MDC_AWAY_RESP_RATE", Sporadic = DriverSporadic.ONLINE.ToString(), Type = "VEN", Enabled = false });
      //   objList.Add(new DriverCapabilityViewModel { IdParameter = 3, IDUnit = 14, DeviceID = "23", DeviceText = "Total Peep", DeviceUnitText = "ml", Name = "VTe", Unit = "mL", Mnemonic = "MDC_VENT_VOL_TIDAL_EXP", Sporadic = DriverSporadic.ONLINE.ToString(), Type = "VEN", Enabled = true });
      //   objList.Add(new DriverCapabilityViewModel { IdParameter = 4, IDUnit = 15, DeviceID = "24", DeviceText = "O2 Concentration", DeviceUnitText = "ml", Name = "VT", Unit = "mL", Mnemonic = "MDC_AWAY_RESP_RATE", Sporadic = DriverSporadic.ONLINE.ToString(), Type = "VEN", Enabled = false });
      //   objList.Add(new DriverCapabilityViewModel { IdParameter = 5, IDUnit = 16, DeviceID = "25", DeviceText = "I:E Ratio", DeviceUnitText = "l/min", Name = "MVe", Unit = "L/min", Mnemonic = "MDC_AWAY_RESP_RATE", Sporadic = DriverSporadic.ONLINE.ToString(), Type = "VEN", Enabled = false });
      //   objList.Add(new DriverCapabilityViewModel { IdParameter = 6, IDUnit = 17, DeviceID = "26", DeviceText = "Edi min", DeviceUnitText = "l/min", Name = "MVin", Unit = "L/min", Mnemonic = "MDC_VENT_VOL_TIDAL_EXP", Sporadic = DriverSporadic.ONLINE.ToString(), Type = "VEN", Enabled = true });
      //   return objList;
      //}

      //public static List<DriverEventCatalogViewModel> GetEventsCatalog()
      //{
      //   List<DriverEventCatalogViewModel> objList = new List<DriverEventCatalogViewModel>();
      //   objList.Add(new DriverEventCatalogViewModel { Code = "400", Level = "HighPriorityAlarm", Class = "Physiological", Text = "O2 conc. too high - alarm", ShortText = "O2 conc. too high - alarm" });
      //   objList.Add(new DriverEventCatalogViewModel { Code = "401", Level = "HighPriorityAlarm", Class = "Physiological", Text = "O2 conc. too high - alarm", ShortText = "O2 conc. too high - alarm" });
      //   objList.Add(new DriverEventCatalogViewModel { Code = "403", Level = "HighPriorityAlarm", Class = "Physiological", Text = "EtCO2 conc. too high - alarm", ShortText = "EtCO2 conc. too high - alarm" });
      //   objList.Add(new DriverEventCatalogViewModel { Code = "404", Level = "HighPriorityAlarm", Class = "Physiological", Text = "EtCO2 conc. too low - alarm", ShortText = "EtCO2 conc. too low - alarm" });
      //   objList.Add(new DriverEventCatalogViewModel { Code = "405", Level = "HighPriorityAlarm", Class = "Technical", Text = "Gas supply alarm", ShortText = "Gas supply alarm" });
      //   objList.Add(new DriverEventCatalogViewModel { Code = "406", Level = "HighPriorityAlarm", Class = "Technical", Text = "Battery alarm", ShortText = "Battery alarm" });
      //   return objList;
      //}

      //public static List<ActualDeviceViewModel> GetActualDevices()
      //{
      //   List<ActualDeviceViewModel> objList = new List<ActualDeviceViewModel>();
      //   objList.Add(new ActualDeviceViewModel { Id = 1, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-1", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 2, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-2", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 3, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-1", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 4, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-2", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 5, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-1", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 6, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1231", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 7, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1241", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 8, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1251", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 9, DeviceID = 1, Name = "Enteral", SerialNumber = "1261", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 10, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1271", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 11, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1281", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 12, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1291", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 13, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1301", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 14, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1311", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 15, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1321", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 16, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-2", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 17, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1232", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 18, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1242", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 19, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-3", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 20, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-4", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 21, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-5", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 22, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-6", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 23, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-7", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 24, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-8", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 25, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-9", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 26, DeviceID = 2, Name = "PICCO", SerialNumber = "MON-ABCXYZ-10", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 27, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-3", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 28, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-4", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 29, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-5", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 30, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-6", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 31, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-7", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 32, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-8", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 33, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-9", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 34, DeviceID = 3, Name = "EVITA4", SerialNumber = "VEN-ABCXYZ-10", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 35, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-3", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 36, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1233", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 37, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1243", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 38, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1253", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 39, DeviceID = 1, Name = "Enteral", SerialNumber = "1263", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 40, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-4", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 41, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1234", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 42, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1244", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 43, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1254", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 44, DeviceID = 1, Name = "Enteral", SerialNumber = "1264", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 45, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1274", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 46, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1284", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 47, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-5", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 48, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1235", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 49, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1245", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 50, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1255", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 51, DeviceID = 1, Name = "Enteral", SerialNumber = "1265", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 52, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1275", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 53, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1285", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 54, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1295", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 55, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1305", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 56, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1315", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 57, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1325", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 58, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-6", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 59, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1236", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 60, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1246", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 61, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-7", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 62, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1237", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 63, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1247", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 64, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1257", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 65, DeviceID = 1, Name = "Enteral", SerialNumber = "1267", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 66, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-8", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 67, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1238", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 68, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1248", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 69, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1258", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 70, DeviceID = 1, Name = "Enteral", SerialNumber = "1268", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 71, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1278", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 72, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1288", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 73, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-9", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 74, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1239", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 75, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1249", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 76, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1259", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 77, DeviceID = 1, Name = "Enteral", SerialNumber = "1269", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 78, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1279", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 79, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1289", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 80, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1299", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 81, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1309", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 82, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1319", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 83, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1329", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 84, DeviceID = 1, Name = "AGW", SerialNumber = "AGWSERIAL-10", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 85, DeviceID = 1, Name = "Alaris GH", SerialNumber = "12310", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 86, DeviceID = 1, Name = "Alaris GP", SerialNumber = "12410", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 87, DeviceID = 1, Name = "AGW", SerialNumber = "", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 88, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07633", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 89, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1252", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 90, DeviceID = 1, Name = "Enteral", SerialNumber = "1262", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 91, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1272", Label = "òùòàòòùòùò", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 92, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1282", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 93, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1292", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 94, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1302", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 95, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1312", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 96, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1322", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 97, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1273", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 98, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1283", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 99, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1293", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 100, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1303", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 101, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1313", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 102, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1323", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 103, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1294", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 104, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1304", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 105, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1314", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 106, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1324", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 107, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1256", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 108, DeviceID = 1, Name = "Enteral", SerialNumber = "1266", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 109, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1276", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 110, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1286", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 111, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1296", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 112, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1306", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 113, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1316", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 114, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1326", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 115, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1277", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 116, DeviceID = 1, Name = "Alaris GW", SerialNumber = "1287", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 117, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1297", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 118, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1307", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 119, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1317", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 120, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1327", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 121, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1298", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 122, DeviceID = 1, Name = "Alaris CC", SerialNumber = "1308", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 123, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1318", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 124, DeviceID = 1, Name = "Alaris PK", SerialNumber = "1328", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 125, DeviceID = 1, Name = "Alaris CC", SerialNumber = "12510", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 126, DeviceID = 1, Name = "Enteral", SerialNumber = "12610", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 127, DeviceID = 1, Name = "Alaris GP", SerialNumber = "12710", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 128, DeviceID = 1, Name = "Alaris GW", SerialNumber = "12810", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 129, DeviceID = 1, Name = "Alaris CC", SerialNumber = "12910", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 130, DeviceID = 1, Name = "Alaris CC", SerialNumber = "13010", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 131, DeviceID = 1, Name = "Alaris GH", SerialNumber = "13110", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 132, DeviceID = 1, Name = "Alaris PK", SerialNumber = "13210", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 133, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07630", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 134, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07993", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 135, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-17023", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 136, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-17002", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 137, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-D2-8F-DC", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 138, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04817", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 139, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-C5-01-68", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 140, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-D2-90-73", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 141, DeviceID = 2, Name = "", SerialNumber = "", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 142, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04877", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 143, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-16963", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 144, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-D2-90-60", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 145, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-03609", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 146, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-05984", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 147, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-75915", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 148, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-03806", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 149, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-CD-83-E6", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 150, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-17036", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 151, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04892", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 152, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-09256", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 153, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-D2-91-FD", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 154, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-62573", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 155, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04948", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 156, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-17001", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 157, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-03614", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 158, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04835", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 159, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-D2-93-48", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 160, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-17054", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 161, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-05315", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 162, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-16979", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 163, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-63186", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 164, DeviceID = 1, Name = "Alaris CC", SerialNumber = "8000-00000", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 165, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04990", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 166, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-03726", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 167, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8000-75853", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 168, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1350-31221", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 169, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-76154", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 170, DeviceID = 1, Name = "Alaris CC", SerialNumber = "8003-29007", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 171, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-77567", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 172, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-05938", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 173, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-04783", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 174, DeviceID = 1, Name = "Alaris GS", SerialNumber = "8001-15982", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 175, DeviceID = 2, Name = "BIG_DIPPER", SerialNumber = "00-0B-AB-04-9B-D2-93-CC", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 176, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-05783", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 177, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07782", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 178, DeviceID = 1, Name = "Alaris GP", SerialNumber = "4700-05326", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 179, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-16932", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 180, DeviceID = 1, Name = "Alaris GP", SerialNumber = "8026-12741", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 181, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "1351-94772", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 182, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-24906", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 183, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07748", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 184, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07517", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 185, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-16945", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 186, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-01654", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 187, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-01751", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 188, DeviceID = 1, Name = "Alaris GP", SerialNumber = "8006-07052", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 189, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07741", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 190, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07703", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 191, DeviceID = 1, Name = "Alaris CC_G", SerialNumber = "3700-07986", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 192, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-16953", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 193, DeviceID = 1, Name = "Alaris GP", SerialNumber = "8026-12886", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 194, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-16977", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 195, DeviceID = 99, Name = "Other", SerialNumber = "0", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 196, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-50963", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 197, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "1351-93706", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 198, DeviceID = 1, Name = "Alaris GH", SerialNumber = "1350-38799", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 199, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-63187", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 200, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8002-76658", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 201, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "2700-13784", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 202, DeviceID = 1, Name = "Alaris GP", SerialNumber = "1350-29725", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 203, DeviceID = 1, Name = "Alaris GH_G", SerialNumber = "1352-05864", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 204, DeviceID = 1, Name = "Alaris GH", SerialNumber = "8000-00000", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 205, DeviceID = 3, Name = "Servo", SerialNumber = "2301939", Label = "", Mobile = false });
      //   objList.Add(new ActualDeviceViewModel { Id = 206, DeviceID = 3, Name = "Servo", SerialNumber = "?SERVO-U015", Label = "", Mobile = false });
      //   return objList;
      //}

      //public static List<DeviceDriverViewModel> GetDeviceDrivers()
      //{
      //   List<DeviceDriverViewModel> objList = new List<DeviceDriverViewModel>();

      //   //objDeviceDrivers.Add(new SelectListItem { Value = "1", Text = "DriverConnectionType_Socket" });
      //   //objDeviceDrivers.Add(new SelectListItem { Value = "2", Text = "DriverConnectionType_Rs232" });

      //   //objDeviceDrivers.Add(new SelectListItem { Value = "1", Text = "DriverSocketType_TCPClient" });
      //   //objDeviceDrivers.Add(new SelectListItem { Value = "2", Text = "DriverSocketType_TCPServer" });
      //   objList.Add(new DeviceDriverViewModel { Id = 1, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = true, AutoStartDriver = true, AutoStartWatchDog = false, ComputerName = "mew", Address = "172.21.7.29", BedLink = "S15", BedAssociation = GetBedAssociation("S15"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.29" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 2, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.30", BedLink = "S16", BedAssociation = GetBedAssociation("S16"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.30" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 3, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.31", BedLink = "S17", BedAssociation = GetBedAssociation("S17"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.31" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 4, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.32", BedLink = "S18", BedAssociation = GetBedAssociation("S18"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.32" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 5, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.33", BedLink = "S19", BedAssociation = GetBedAssociation("S19"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.33" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 6, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.34", BedLink = "S20", BedAssociation = GetBedAssociation("S20"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.34" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 7, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.35", BedLink = "S21", BedAssociation = GetBedAssociation("S21"), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.35" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 8, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.37", BedLink = "", BedAssociation = GetBedAssociation(""), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.37" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 9, Name = "AGW", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = false, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "172.21.7.38", BedLink = "S24", ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPClient", HostName = "172.21.7.38" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 10, Name = "eGateway", Version = "1.0", DriverType = DriverType.MultiBed, LogEnabled = true, AutoStartDriver = true, AutoStartWatchDog = false, ComputerName = "mew", Address = "Localhost", BedLink = "Multi Bed", BedAssociation = GetBedAssociated(), ConnectionType = "DriverConnectionType_Socket", Socket = new DeviceDriverSocketViewModel { SocketType = "DriverSocketType_TCPServer", HostName = "LocalHost", Port = 56800 } });
      //   objList.Add(new DeviceDriverViewModel { Id = 11, Name = "PRISMAFLEX", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = true, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "COM3", BedLink = "S23", BedAssociation = GetBedAssociation("S23"), ConnectionType = "DriverConnectionType_Rs232", SerialPort = new DeviceDriverSerialPortViewModel { SerialPort = "COM3", BitsPerSeconds = "10020", Parity = "NONE", DataBits = "8", StopBits = "1", Handshake = "0 - NONE", DataMode = "HEX" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 12, Name = "SERVO", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = true, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "COM4", BedLink = "S23", BedAssociation = GetBedAssociation("S23"), ConnectionType = "DriverConnectionType_Rs232", SerialPort = new DeviceDriverSerialPortViewModel { SerialPort = "COM4", BitsPerSeconds = "9600", Parity = "EVEN", DataBits = "8", StopBits = "1", Handshake = "1 - XON/XOFF", DataMode = "HEX" } });
      //   objList.Add(new DeviceDriverViewModel { Id = 13, Name = "SERVOU", Version = "1.0", DriverType = DriverType.SingleBed, LogEnabled = true, AutoStartDriver = true, AutoStartWatchDog = true, ComputerName = "mew", Address = "COM4", BedLink = "S23", BedAssociation = GetBedAssociation("S23"), ConnectionType = "DriverConnectionType_Rs232", SerialPort = new DeviceDriverSerialPortViewModel { SerialPort = "COM4", BitsPerSeconds = "38400", Parity = "EVEN", DataBits = "8", StopBits = "1", Handshake = "0 - NONE", DataMode = "HEX" } });

      //   return objList;
      //}


      //public static List<SelectListItem> GetDeviceDriversList()
      //{
      //   List<SelectListItem> objDeviceDrivers = new List<SelectListItem>();
      //   objDeviceDrivers.Add(new SelectListItem { Value = "1", Text = "Carefusion_Demo 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "2", Text = "Evita_Demo 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "3", Text = "AGW 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "4", Text = "MEDIBUSX 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "5", Text = "PRISMAFLEX 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "6", Text = "SERVOU 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "7", Text = "AGW 5.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "8", Text = "SERVO 1.0" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "8", Text = "eGateway 1.5" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "8", Text = "Carescape_Demo 1.0" });
      //   return objDeviceDrivers;
      //}

      //public static List<SelectListItem> GetConnectionTypeList()
      //{
      //   List<SelectListItem> objDeviceDrivers = new List<SelectListItem>();
      //   objDeviceDrivers.Add(new SelectListItem { Value = "1", Text = "DriverConnectionType_Socket" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "2", Text = "DriverConnectionType_Rs232" });
      //   return objDeviceDrivers;
      //}

      //public static List<SelectListItem> GetSocketTypeList()
      //{
      //   List<SelectListItem> objDeviceDrivers = new List<SelectListItem>();
      //   objDeviceDrivers.Add(new SelectListItem { Value = "1", Text = "DriverSocketType_TCPClient" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "2", Text = "DriverSocketType_TCPServer" });
      //   return objDeviceDrivers;
      //}

      //public static List<SelectListItem> GetSerialPortsList()
      //{
      //   List<SelectListItem> objDeviceDrivers = new List<SelectListItem>();
      //   objDeviceDrivers.Add(new SelectListItem { Value = "1", Text = "COM1" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "2", Text = "COM2" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "3", Text = "COM3" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "4", Text = "COM4" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "5", Text = "COM5" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "6", Text = "COM6" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "7", Text = "COM7" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "8", Text = "COM8" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "9", Text = "COM9" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "10", Text = "COM10" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "11", Text = "COM11" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "12", Text = "COM12" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "13", Text = "COM13" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "14", Text = "COM14" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "15", Text = "COM15" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "16", Text = "COM16" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "17", Text = "COM17" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "18", Text = "COM18" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "19", Text = "COM19" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "20", Text = "COM20" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "21", Text = "COM21" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "22", Text = "COM22" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "23", Text = "COM23" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "24", Text = "COM24" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "25", Text = "COM25" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "26", Text = "COM26" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "27", Text = "COM27" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "28", Text = "COM28" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "29", Text = "COM29" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "30", Text = "COM30" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "31", Text = "COM31" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "32", Text = "COM32" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "33", Text = "COM33" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "34", Text = "COM34" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "35", Text = "COM35" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "36", Text = "COM36" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "37", Text = "COM37" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "38", Text = "COM38" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "39", Text = "COM39" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "40", Text = "COM40" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "41", Text = "COM41" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "42", Text = "COM42" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "43", Text = "COM43" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "44", Text = "COM44" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "45", Text = "COM45" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "46", Text = "COM46" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "47", Text = "COM47" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "48", Text = "COM48" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "49", Text = "COM49" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "50", Text = "COM50" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "51", Text = "COM51" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "52", Text = "COM52" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "53", Text = "COM53" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "54", Text = "COM54" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "55", Text = "COM55" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "56", Text = "COM56" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "57", Text = "COM57" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "58", Text = "COM58" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "59", Text = "COM59" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "60", Text = "COM60" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "61", Text = "COM61" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "62", Text = "COM62" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "63", Text = "COM63" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "64", Text = "COM64" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "65", Text = "COM65" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "66", Text = "COM66" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "67", Text = "COM67" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "68", Text = "COM68" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "69", Text = "COM69" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "70", Text = "COM70" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "71", Text = "COM71" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "72", Text = "COM72" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "73", Text = "COM73" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "74", Text = "COM74" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "75", Text = "COM75" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "76", Text = "COM76" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "77", Text = "COM77" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "78", Text = "COM78" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "79", Text = "COM79" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "80", Text = "COM80" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "81", Text = "COM81" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "82", Text = "COM82" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "83", Text = "COM83" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "84", Text = "COM84" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "85", Text = "COM85" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "86", Text = "COM86" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "87", Text = "COM87" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "88", Text = "COM88" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "89", Text = "COM89" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "90", Text = "COM90" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "91", Text = "COM91" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "92", Text = "COM92" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "93", Text = "COM93" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "94", Text = "COM94" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "95", Text = "COM95" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "96", Text = "COM96" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "97", Text = "COM97" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "98", Text = "COM98" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "99", Text = "COM99" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "100", Text = "COM100" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "101", Text = "COM101" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "102", Text = "COM102" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "103", Text = "COM103" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "104", Text = "COM104" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "105", Text = "COM105" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "106", Text = "COM106" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "107", Text = "COM107" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "108", Text = "COM108" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "109", Text = "COM109" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "110", Text = "COM110" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "111", Text = "COM111" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "112", Text = "COM112" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "113", Text = "COM113" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "114", Text = "COM114" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "115", Text = "COM115" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "116", Text = "COM116" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "117", Text = "COM117" });
      //   objDeviceDrivers.Add(new SelectListItem { Value = "118", Text = "COM118" });
      //   return objDeviceDrivers;
      //}


      //public static List<SelectListItem> GetBitsPerSecondList()
      //{
      //   List<SelectListItem> objBitsPerSecond = new List<SelectListItem>();
      //   objBitsPerSecond.Add(new SelectListItem { Value = "1", Text = "300" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "2", Text = "600" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "3", Text = "1200" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "4", Text = "2400" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "5", Text = "4800" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "6", Text = "9600" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "7", Text = "14400" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "8", Text = "19200" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "9", Text = "38400" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "10", Text = "57600" });
      //   objBitsPerSecond.Add(new SelectListItem { Value = "11", Text = "115200" });
      //   return objBitsPerSecond;
      //}

      //public static List<SelectListItem> GetParityList()
      //{
      //   List<SelectListItem> objParity = new List<SelectListItem>();
      //   objParity.Add(new SelectListItem { Value = "1", Text = "NONE" });
      //   objParity.Add(new SelectListItem { Value = "2", Text = "ODD" });
      //   objParity.Add(new SelectListItem { Value = "3", Text = "EVEN" });
      //   objParity.Add(new SelectListItem { Value = "4", Text = "MARK" });
      //   objParity.Add(new SelectListItem { Value = "5", Text = "SPACE" });

      //   return objParity;
      //}

      //public static List<SelectListItem> GetDataBitsList()
      //{
      //   List<SelectListItem> objList = new List<SelectListItem>();
      //   objList.Add(new SelectListItem { Value = "1", Text = "7" });
      //   objList.Add(new SelectListItem { Value = "2", Text = "8" });

      //   return objList;
      //}

      //public static List<SelectListItem> GetStopBitsList()
      //{
      //   List<SelectListItem> objParity = new List<SelectListItem>();
      //   objParity.Add(new SelectListItem { Value = "1", Text = "1" });
      //   objParity.Add(new SelectListItem { Value = "2", Text = "1.5" });
      //   objParity.Add(new SelectListItem { Value = "3", Text = "2" });

      //   return objParity;
      //}

      //public static List<SelectListItem> GetHandshakeList()
      //{
      //   List<SelectListItem> objParity = new List<SelectListItem>();
      //   objParity.Add(new SelectListItem { Value = "1", Text = "0 - NONE" });
      //   objParity.Add(new SelectListItem { Value = "2", Text = "1 - XON/XOFF" });
      //   objParity.Add(new SelectListItem { Value = "3", Text = "2 - RTS" });
      //   objParity.Add(new SelectListItem { Value = "4", Text = "3 - RTS + XON/XOFF" });
      //   return objParity;
      //}

      //public static List<SelectListItem> GetDataModeList()
      //{
      //   List<SelectListItem> objParity = new List<SelectListItem>();
      //   objParity.Add(new SelectListItem { Value = "1", Text = "0 - TEXT" });
      //   objParity.Add(new SelectListItem { Value = "2", Text = "1 - HEX" });
      //   objParity.Add(new SelectListItem { Value = "3", Text = "2 - OTHER" });
      //   return objParity;
      //}

      //public static List<BedAssociationViewModel> GetBedAssociation(string bedName)
      //{
      //   List<BedAssociationViewModel> objBedAssociation = new List<BedAssociationViewModel>();

      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S15", Bedcode = "15", DriverSideBedName = "", Enabled = (bedName == "S15" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S16", Bedcode = "16", DriverSideBedName = "", Enabled = (bedName == "S16" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S17", Bedcode = "17", DriverSideBedName = "", Enabled = (bedName == "S17" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S18", Bedcode = "18", DriverSideBedName = "", Enabled = (bedName == "S18" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S19", Bedcode = "19", DriverSideBedName = "", Enabled = (bedName == "S19" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S20", Bedcode = "20", DriverSideBedName = "", Enabled = (bedName == "S20" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S21", Bedcode = "21", DriverSideBedName = "", Enabled = (bedName == "S21" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S24", Bedcode = "24", DriverSideBedName = "", Enabled = (bedName == "S24" ? true : false), Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S23", Bedcode = "23", DriverSideBedName = "", Enabled = (bedName == "S23" ? true : false), Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S19", Bedcode = "19", DriverSideBedName = "", Enabled = (bedName == "S19" ? true : false), Watchdog = false });



      //   return objBedAssociation;

      //}

      //public static List<BedAssociationViewModel> GetBedAssociated()
      //{
      //   List<BedAssociationViewModel> objBedAssociation = new List<BedAssociationViewModel>();

      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S15", Bedcode = "15", DriverSideBedName = "^S15", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S16", Bedcode = "16", DriverSideBedName = "^S16", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S17", Bedcode = "17", DriverSideBedName = "^S17", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S18", Bedcode = "18", DriverSideBedName = "^S18", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S19", Bedcode = "19", DriverSideBedName = "^S19", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S20", Bedcode = "20", DriverSideBedName = "^S20", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S21", Bedcode = "21", DriverSideBedName = "^S21", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S24", Bedcode = "24", DriverSideBedName = "^S24", Enabled = false, Watchdog = true });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S23", Bedcode = "23", DriverSideBedName = "", Enabled = true, Watchdog = false });
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S19", Bedcode = "19", DriverSideBedName = "^S19", Enabled = false, Watchdog = true });

      //   //multibed
      //   objBedAssociation.Add(new BedAssociationViewModel { Location = "UCIP", BedName = "S15", Bedcode = "15", DriverSideBedName = "", Enabled = true, Watchdog = false });

      //   return objBedAssociation;

      //}

      //public static List<CustomParametersViewModel> GetCustomParameters()
      //{
      //   List<CustomParametersViewModel> objList = new List<CustomParametersViewModel>();
      //   objList.Add(new CustomParametersViewModel { ID = 1, Name = "RecordData", Value = "false", Description = "If true, all incoming messages will be recorded on a xml file." });
      //   objList.Add(new CustomParametersViewModel { ID = 1, Name = "DemoMode", Value = "false", Description = "If true, the xml recorded filename, present in the bed deviceside, will be used to simulate data." });

      //   return objList;
      //}

      //public static List<HospitalUnitViewModel> GetHospitalUnits()
      //{
      //   List<HospitalUnitViewModel> objHU = new List<HospitalUnitViewModel>();
      //   objHU.Add(new HospitalUnitViewModel { GUID = "6A390D9D-A8BB-4F03-B6D5-BFE1B6312C2F", ExternalKey = "USC", Name = "USC", Description = "Semi critical", ShortName = "USC", Code = "USC" });
      //   objHU.Add(new HospitalUnitViewModel { GUID = "7EA96F79-F292-453F-9363-321A3EBDB44D", ExternalKey = "", Name = "HOSPITAL SANT JOAN DE DÉU", Description = "", ShortName = "HOSPITAL SANT JOAN DE DÉU", Code = "HSJD" });
      //   objHU.Add(new HospitalUnitViewModel { GUID = "D064ABF6-79C0-4F0A-A3F8-1709E8443D27", ExternalKey = "UCIP", Name = "UCIP", Description = "Critical", ShortName = "UCIP", Code = "UCIP" });
      //   return objHU;
      //}

      //public static List<UserRoleViewModel> GetUserRoles()
      //{
      //   List<UserRoleViewModel> objList = new List<UserRoleViewModel>();
      //   objList.Add(new UserRoleViewModel { RoleID = 1, RoleGroup = "Allied health professionals", RoleName = "Art therapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 2, RoleGroup = "Allied health professionals", RoleName = "Diagnostic radiographer" });
      //   objList.Add(new UserRoleViewModel { RoleID = 3, RoleGroup = "Allied health professionals", RoleName = "Dietitian" });
      //   objList.Add(new UserRoleViewModel { RoleID = 4, RoleGroup = "Allied health professionals", RoleName = "Dramatherapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 5, RoleGroup = "Allied health professionals", RoleName = "Music therapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 6, RoleGroup = "Allied health professionals", RoleName = "Occupational therapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 7, RoleGroup = "Allied health professionals", RoleName = "Operating department practitioner" });
      //   objList.Add(new UserRoleViewModel { RoleID = 8, RoleGroup = "Allied health professionals", RoleName = "Orthoptist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 9, RoleGroup = "Allied health professionals", RoleName = "Osteopath" });
      //   objList.Add(new UserRoleViewModel { RoleID = 10, RoleGroup = "Allied health professionals", RoleName = "Physiotherapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 11, RoleGroup = "Allied health professionals", RoleName = "Podiatrist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 12, RoleGroup = "Allied health professionals", RoleName = "Prosthetist/orthotist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 13, RoleGroup = "Allied health professionals", RoleName = "Speech and language therapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 14, RoleGroup = "Allied health professionals", RoleName = "Therapeutic radiographer" });
      //   objList.Add(new UserRoleViewModel { RoleID = 15, RoleGroup = "Ambulance service team", RoleName = "Ambulance care assistant and Patient Transport Service (PTS) driver" });
      //   objList.Add(new UserRoleViewModel { RoleID = 16, RoleGroup = "Ambulance service team", RoleName = "Call handler/emergency medical dispatcher" });
      //   objList.Add(new UserRoleViewModel { RoleID = 17, RoleGroup = "Ambulance service team", RoleName = "Emergency care assistant" });
      //   objList.Add(new UserRoleViewModel { RoleID = 18, RoleGroup = "Ambulance service team", RoleName = "Experienced paramedic" });
      //   objList.Add(new UserRoleViewModel { RoleID = 19, RoleGroup = "Ambulance service team", RoleName = "Paramedic" });
      //   objList.Add(new UserRoleViewModel { RoleID = 20, RoleGroup = "Ambulance service team", RoleName = "Patient Transport Service (PTS) call handler" });
      //   objList.Add(new UserRoleViewModel { RoleID = 21, RoleGroup = "Dental team", RoleName = "Dental hygienist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 22, RoleGroup = "Dental team", RoleName = "Dental nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 23, RoleGroup = "Dental team", RoleName = "Dental technician/dental technologist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 24, RoleGroup = "Dental team", RoleName = "Dental therapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 25, RoleGroup = "Dental team", RoleName = "Dentist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 26, RoleGroup = "Doctors", RoleName = "Acute internal medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 27, RoleGroup = "Doctors", RoleName = "Allergy" });
      //   objList.Add(new UserRoleViewModel { RoleID = 28, RoleGroup = "Doctors", RoleName = "Anaesthesia" });
      //   objList.Add(new UserRoleViewModel { RoleID = 29, RoleGroup = "Doctors", RoleName = "Cardiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 30, RoleGroup = "Doctors", RoleName = "Cardiothoracic surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 31, RoleGroup = "Doctors", RoleName = "Chemical pathology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 32, RoleGroup = "Doctors", RoleName = "Child and adolescent psychiatry" });
      //   objList.Add(new UserRoleViewModel { RoleID = 33, RoleGroup = "Doctors", RoleName = "Clinical genetics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 34, RoleGroup = "Doctors", RoleName = "Clinical neurophysiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 35, RoleGroup = "Doctors", RoleName = "Clinical oncology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 36, RoleGroup = "Doctors", RoleName = "Clinical pharmacology and therapeutics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 37, RoleGroup = "Doctors", RoleName = "Clinical radiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 38, RoleGroup = "Doctors", RoleName = "Community sexual and reproductive health" });
      //   objList.Add(new UserRoleViewModel { RoleID = 39, RoleGroup = "Doctors", RoleName = "Dermatology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 40, RoleGroup = "Doctors", RoleName = "Emergency medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 41, RoleGroup = "Doctors", RoleName = "Endocrinology and diabetes" });
      //   objList.Add(new UserRoleViewModel { RoleID = 42, RoleGroup = "Doctors", RoleName = "Forensic psychiatry" });
      //   objList.Add(new UserRoleViewModel { RoleID = 43, RoleGroup = "Doctors", RoleName = "Gastroenterology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 44, RoleGroup = "Doctors", RoleName = "General internal medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 45, RoleGroup = "Doctors", RoleName = "General practice  (GP)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 46, RoleGroup = "Doctors", RoleName = "General psychiatry" });
      //   objList.Add(new UserRoleViewModel { RoleID = 47, RoleGroup = "Doctors", RoleName = "General surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 48, RoleGroup = "Doctors", RoleName = "Genitourinary medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 49, RoleGroup = "Doctors", RoleName = "Geriatric medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 50, RoleGroup = "Doctors", RoleName = "Haematology (doctor" });
      //   objList.Add(new UserRoleViewModel { RoleID = 51, RoleGroup = "Doctors", RoleName = "Histopathology (doctor)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 52, RoleGroup = "Doctors", RoleName = "Immunology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 53, RoleGroup = "Doctors", RoleName = "Infectious diseases" });
      //   objList.Add(new UserRoleViewModel { RoleID = 54, RoleGroup = "Doctors", RoleName = "Intensive care medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 55, RoleGroup = "Doctors", RoleName = "Liaison psychiatry" });
      //   objList.Add(new UserRoleViewModel { RoleID = 56, RoleGroup = "Doctors", RoleName = "Medical microbiology and virology (doctor)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 57, RoleGroup = "Doctors", RoleName = "Medical oncology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 58, RoleGroup = "Doctors", RoleName = "Medical ophthalmology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 59, RoleGroup = "Doctors", RoleName = "Medical psychotherapy" });
      //   objList.Add(new UserRoleViewModel { RoleID = 60, RoleGroup = "Doctors", RoleName = "Metabolic Medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 61, RoleGroup = "Doctors", RoleName = "Neurology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 62, RoleGroup = "Doctors", RoleName = "Neurosurgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 63, RoleGroup = "Doctors", RoleName = "Nuclear medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 64, RoleGroup = "Doctors", RoleName = "Obstetrics and gynaecology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 65, RoleGroup = "Doctors", RoleName = "Occupational medicine<" });
      //   objList.Add(new UserRoleViewModel { RoleID = 66, RoleGroup = "Doctors", RoleName = "Old age psychiatry" });
      //   objList.Add(new UserRoleViewModel { RoleID = 67, RoleGroup = "Doctors", RoleName = "Ophthalmology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 68, RoleGroup = "Doctors", RoleName = "Oral and maxillofacial surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 69, RoleGroup = "Doctors", RoleName = "Otorhinolaryngology (ear, nose and throat surgery, ENT)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 70, RoleGroup = "Doctors", RoleName = "Paediatric cardiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 71, RoleGroup = "Doctors", RoleName = "Paediatric surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 72, RoleGroup = "Doctors", RoleName = "Paediatrics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 73, RoleGroup = "Doctors", RoleName = "Palliative medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 74, RoleGroup = "Doctors", RoleName = "Pharmaceutical medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 75, RoleGroup = "Doctors", RoleName = "Plastic surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 76, RoleGroup = "Doctors", RoleName = "Psychiatry of intellectual disability (PID)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 77, RoleGroup = "Doctors", RoleName = "Public health (doctor)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 78, RoleGroup = "Doctors", RoleName = "Rehabilitation medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 79, RoleGroup = "Doctors", RoleName = "Renal medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 80, RoleGroup = "Doctors", RoleName = "Respiratory medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 81, RoleGroup = "Doctors", RoleName = "Rheumatology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 82, RoleGroup = "Doctors", RoleName = "Sport and exercise medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 83, RoleGroup = "Doctors", RoleName = "Stroke medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 84, RoleGroup = "Doctors", RoleName = "Trauma and orthopaedic surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 85, RoleGroup = "Doctors", RoleName = "Tropical medicine" });
      //   objList.Add(new UserRoleViewModel { RoleID = 86, RoleGroup = "Doctors", RoleName = "Urology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 87, RoleGroup = "Doctors", RoleName = "Vascular surgery" });
      //   objList.Add(new UserRoleViewModel { RoleID = 88, RoleGroup = "Health informatics", RoleName = "Clinical informatics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 89, RoleGroup = "Health informatics", RoleName = "Education and training roles" });
      //   objList.Add(new UserRoleViewModel { RoleID = 90, RoleGroup = "Health informatics", RoleName = "Health records and patient administration" });
      //   objList.Add(new UserRoleViewModel { RoleID = 91, RoleGroup = "Health informatics", RoleName = "Information and communication technology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 92, RoleGroup = "Health informatics", RoleName = "Information management staff" });
      //   objList.Add(new UserRoleViewModel { RoleID = 93, RoleGroup = "Health informatics", RoleName = "Library, knowledge and information services<" });
      //   objList.Add(new UserRoleViewModel { RoleID = 94, RoleGroup = "Health informatics", RoleName = "Project and programme management " });
      //   objList.Add(new UserRoleViewModel { RoleID = 95, RoleGroup = "Health science", RoleName = "Analytical toxicology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 96, RoleGroup = "Health science", RoleName = "Anatomical pathology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 97, RoleGroup = "Health science", RoleName = "Audiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 98, RoleGroup = "Health science", RoleName = "Biomedical science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 99, RoleGroup = "Health science", RoleName = "Blood sciences" });
      //   objList.Add(new UserRoleViewModel { RoleID = 100, RoleGroup = "Health science", RoleName = "Cancer genomics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 101, RoleGroup = "Health science", RoleName = "Cardiac sciences" });
      //   objList.Add(new UserRoleViewModel { RoleID = 102, RoleGroup = "Health science", RoleName = "Cellular sciences" });
      //   objList.Add(new UserRoleViewModel { RoleID = 103, RoleGroup = "Health science", RoleName = "Clinical biochemistry" });
      //   objList.Add(new UserRoleViewModel { RoleID = 104, RoleGroup = "Health science", RoleName = "Clinical bioinformatics (genomics)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 105, RoleGroup = "Health science", RoleName = "Clinical bioinformatics (health informatics)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 106, RoleGroup = "Health science", RoleName = "Clinical bioinformatics (physical sciences)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 107, RoleGroup = "Health science", RoleName = "Clinical immunology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 108, RoleGroup = "Health science", RoleName = "Clinical measurement" });
      //   objList.Add(new UserRoleViewModel { RoleID = 109, RoleGroup = "Health science", RoleName = "Clinical or medical technologist in medical physics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 110, RoleGroup = "Health science", RoleName = "Clinical perfusion science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 111, RoleGroup = "Health science", RoleName = "Clinical pharmaceutical science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 112, RoleGroup = "Health science", RoleName = "Clinical photography" });
      //   objList.Add(new UserRoleViewModel { RoleID = 113, RoleGroup = "Health science", RoleName = "Critical care science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 114, RoleGroup = "Health science", RoleName = "Cytopathology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 115, RoleGroup = "Health science", RoleName = "Decontamination and sterile services" });
      //   objList.Add(new UserRoleViewModel { RoleID = 116, RoleGroup = "Health science", RoleName = "Gastrointestinal physiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 117, RoleGroup = "Health science", RoleName = "Genomic counselling" });
      //   objList.Add(new UserRoleViewModel { RoleID = 118, RoleGroup = "Health science", RoleName = "Genomics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 119, RoleGroup = "Health science", RoleName = "Haematology (healthcare scientist)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 120, RoleGroup = "Health science", RoleName = "Hearing aid dispenser" });
      //   objList.Add(new UserRoleViewModel { RoleID = 121, RoleGroup = "Health science", RoleName = "Histocompatibility and immunogenetics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 122, RoleGroup = "Health science", RoleName = "Histopathology (healthcare scientist)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 123, RoleGroup = "Health science", RoleName = "Imaging (ionising)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 124, RoleGroup = "Health science", RoleName = "Imaging (non-ionising)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 125, RoleGroup = "Health science", RoleName = "Infection sciences" });
      //   objList.Add(new UserRoleViewModel { RoleID = 126, RoleGroup = "Health science", RoleName = "Medical device risk management and governance" });
      //   objList.Add(new UserRoleViewModel { RoleID = 127, RoleGroup = "Health science", RoleName = "Medical engineering" });
      //   objList.Add(new UserRoleViewModel { RoleID = 128, RoleGroup = "Health science", RoleName = "Microbiology (healthcare scientist)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 129, RoleGroup = "Health science", RoleName = "Neurophysiology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 130, RoleGroup = "Health science", RoleName = "Nuclear medicine (healthcare scientist)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 131, RoleGroup = "Health science", RoleName = "Ophthalmic and vision science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 132, RoleGroup = "Health science", RoleName = "Radiation physics and radiation safety physics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 133, RoleGroup = "Health science", RoleName = "Radiotherapy physics" });
      //   objList.Add(new UserRoleViewModel { RoleID = 134, RoleGroup = "Health science", RoleName = "Reconstructive science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 135, RoleGroup = "Health science", RoleName = "Rehabilitation engineering" });
      //   objList.Add(new UserRoleViewModel { RoleID = 136, RoleGroup = "Health science", RoleName = "Renal technology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 137, RoleGroup = "Health science", RoleName = "Reproductive science and andrology" });
      //   objList.Add(new UserRoleViewModel { RoleID = 138, RoleGroup = "Health science", RoleName = "Respiratory physiology and sleep sciences" });
      //   objList.Add(new UserRoleViewModel { RoleID = 139, RoleGroup = "Health science", RoleName = "Urodynamic science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 140, RoleGroup = "Health science", RoleName = "Vascular science" });
      //   objList.Add(new UserRoleViewModel { RoleID = 141, RoleGroup = "Health science", RoleName = "Virology (healthcare scientist)" });
      //   objList.Add(new UserRoleViewModel { RoleID = 142, RoleGroup = "Management", RoleName = "Administrative management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 143, RoleGroup = "Management", RoleName = "Clinical manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 144, RoleGroup = "Management", RoleName = "Communications and corporate affairs" });
      //   objList.Add(new UserRoleViewModel { RoleID = 145, RoleGroup = "Management", RoleName = "Decontamination services management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 146, RoleGroup = "Management", RoleName = "Design and engineering" });
      //   objList.Add(new UserRoleViewModel { RoleID = 147, RoleGroup = "Management", RoleName = "Estates manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 148, RoleGroup = "Management", RoleName = "Facilities management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 149, RoleGroup = "Management", RoleName = "Finance manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 150, RoleGroup = "Management", RoleName = "Hotel services management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 151, RoleGroup = "Management", RoleName = "Human resources (HR) manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 152, RoleGroup = "Management", RoleName = "Integrated urgent care/NHS 111 team leader" });
      //   objList.Add(new UserRoleViewModel { RoleID = 153, RoleGroup = "Management", RoleName = "Performance and quality management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 154, RoleGroup = "Management", RoleName = "Project management and procurement" });
      //   objList.Add(new UserRoleViewModel { RoleID = 155, RoleGroup = "Management", RoleName = "Project manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 156, RoleGroup = "Management", RoleName = "Purchasing and contract management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 157, RoleGroup = "Management", RoleName = "Strategic management" });
      //   objList.Add(new UserRoleViewModel { RoleID = 158, RoleGroup = "Management", RoleName = "Practice manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 159, RoleGroup = "Medical associate professions", RoleName = "Midwife" });
      //   objList.Add(new UserRoleViewModel { RoleID = 160, RoleGroup = "Nursing", RoleName = "Adult nurse " });
      //   objList.Add(new UserRoleViewModel { RoleID = 161, RoleGroup = "Nursing", RoleName = "Children's nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 162, RoleGroup = "Nursing", RoleName = "District nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 163, RoleGroup = "Nursing", RoleName = "General practice nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 164, RoleGroup = "Nursing", RoleName = "Learning disability nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 165, RoleGroup = "Nursing", RoleName = "Mental health nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 166, RoleGroup = "Nursing", RoleName = "Neonatal nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 167, RoleGroup = "Nursing", RoleName = "Prison nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 168, RoleGroup = "Nursing", RoleName = "Theatre nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 169, RoleGroup = "Pharmacy", RoleName = "Pharmacist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 170, RoleGroup = "Pharmacy", RoleName = "Pharmacy assistant" });
      //   objList.Add(new UserRoleViewModel { RoleID = 171, RoleGroup = "Pharmacy", RoleName = "Pharmacy technician" });
      //   objList.Add(new UserRoleViewModel { RoleID = 172, RoleGroup = "Psychological therapies", RoleName = "Assistant clinical psychologist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 173, RoleGroup = "Psychological therapies", RoleName = "Clinical neuropsychologist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 174, RoleGroup = "Psychological therapies", RoleName = "Clinical psychologist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 175, RoleGroup = "Psychological therapies", RoleName = "Counselling psychologist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 176, RoleGroup = "Psychological therapies", RoleName = "Counsellor" });
      //   objList.Add(new UserRoleViewModel { RoleID = 177, RoleGroup = "Psychological therapies", RoleName = "Health psychologist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 178, RoleGroup = "Psychological therapies", RoleName = "High intensity therapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 179, RoleGroup = "Psychological therapies", RoleName = "Psychological wellbeing practitioner" });
      //   objList.Add(new UserRoleViewModel { RoleID = 180, RoleGroup = "Psychological therapies", RoleName = "Psychotherapist" });
      //   objList.Add(new UserRoleViewModel { RoleID = 181, RoleGroup = "Public health", RoleName = "Director of public health" });
      //   objList.Add(new UserRoleViewModel { RoleID = 182, RoleGroup = "Public health", RoleName = "Environmental health professional" });
      //   objList.Add(new UserRoleViewModel { RoleID = 183, RoleGroup = "Public health", RoleName = "Health trainer" });
      //   objList.Add(new UserRoleViewModel { RoleID = 184, RoleGroup = "Public health", RoleName = "Health visitor" });
      //   objList.Add(new UserRoleViewModel { RoleID = 185, RoleGroup = "Public health", RoleName = "Occupational health nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 186, RoleGroup = "Public health", RoleName = "Public health academic" });
      //   objList.Add(new UserRoleViewModel { RoleID = 187, RoleGroup = "Public health", RoleName = "Public health consultants and specialists" });
      //   objList.Add(new UserRoleViewModel { RoleID = 188, RoleGroup = "Public health", RoleName = "Public health knowledge and intelligence professional" });
      //   objList.Add(new UserRoleViewModel { RoleID = 189, RoleGroup = "Public health", RoleName = "Public health manager" });
      //   objList.Add(new UserRoleViewModel { RoleID = 190, RoleGroup = "Public health", RoleName = "Public health nurse" });
      //   objList.Add(new UserRoleViewModel { RoleID = 191, RoleGroup = "Public health", RoleName = "Public health practitioner" });
      //   objList.Add(new UserRoleViewModel { RoleID = 192, RoleGroup = "Public health", RoleName = "School nurse" });
      //   return objList;
      //}

      //public static List<ReportTemplateViewModel> GetReportTemplates()
      //{
      //   List<ReportTemplateViewModel> reportTemplates = new List<ReportTemplateViewModel>();
      //   //reportTemplates.Add(new ReportTemplateViewModel { ID = Guid.Parse("bd7af968-a614-4ef7-9415-ce32d88f2b70"), Version = 1, Current = true, UserID = Guid.Parse("4e762a2f-ce9f-4ee1-8a15-376d2225f76e"), UserVersion = 1, CreationDate = DateTime.Now, Name = "Dashboard", Author = "ASCOM", Description = "Shows a Dashboard", Stream = "", Filename = "EventStatisticsReport.frx", Application= "SMARTCENTRAL", Module = "" });
      //   //reportTemplates.Add(new ReportTemplateViewModel { ID = Guid.Parse("0fdcb1c5-dfb8-443b-81b2-e54b5e7d37ff"), Version = 2, Current = true, UserID = Guid.Parse("DF6F4065-8D92-42B1-AB22-26D29F35A21F"), UserVersion = 8, CreationDate = DateTime.Now, Name = "VSScore", Author = "ASCOM", Description = "VitalSigns Viewer score report", Stream = "", Filename = "VSScores.frx", Application = "VITALSIGNSNET", Module = "VITALSIGNSNET" });
      //   //reportTemplates.Add(new ReportTemplateViewModel { ID = Guid.Parse("fe205573-4771-4cb2-ab82-4a4120f26781"), Version = 2, Current = true, UserID = Guid.Parse("DF6F4065-8D92-42B1-AB22-26D29F35A21F"), UserVersion = 8, CreationDate = DateTime.Now, Name = "VSValues", Author = "ASCOM", Description = "VitalSigns value report", Stream = "", Filename = "VSValues.frx", Application = "VITALSIGNSNET", Module = "" });
      //   //reportTemplates.Add(new ReportTemplateViewModel { ID = Guid.Parse("6b209271-e854-476b-a952-71fbe7ce5c36"), Version = 1, Current = true, UserID = Guid.Parse("4e762a2f-ce9f-4ee1-8a15-376d2225f76e"), UserVersion = 1, CreationDate = DateTime.Now, Name = "DeviceEvents", Author = "ASCOM", Description = "Show all device events", Stream = "", Filename = "DeviceEvents.frx", Application = "SMARTCENTRAL", Module = "" });
      //   return reportTemplates;
      //}

      

      //public static List<MenuViewModel> GetMenuList()
      //{
      //   List<MenuViewModel> fixedMenu = new List<MenuViewModel>();
      //   try
      //   {
      //      fixedMenu.Add(new MenuViewModel { Id = 1, ParentId = null, Enabled = true, Text = "General" });
      //      fixedMenu.Add(new MenuViewModel { Id = 9, ParentId = 1, Enabled = true, Text = "System Configuration" });
      //      fixedMenu.Add(new MenuViewModel { Id = 10, ParentId = 9, Enabled = true, Text = "Administrator/Application SystemOptions", Url = new ActionUrl { Action = "SystemOptions", Controller = "SystemConfiguration" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 11, ParentId = 9, Enabled = true, Text = "Network Configuration", Url = new ActionUrl { Action = "NetworkConfiguration", Controller = "SystemConfiguration" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 12, ParentId = 9, Enabled = true, Text = "Bed", Url = new ActionUrl { Action = "Bed", Controller = "SystemConfiguration" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 13, ParentId = 9, Enabled = true, Text = "Location", Url = new ActionUrl { Action = "Locations", Controller = "SystemConfiguration" } });

      //      fixedMenu.Add(new MenuViewModel { Id = 14, ParentId = 1, Enabled = true, Text = "System Administration" });
      //      fixedMenu.Add(new MenuViewModel { Id = 15, ParentId = 14, Enabled = true, Text = "User Accounts", Url = new ActionUrl { Action = "UserAccounts", Controller = "SystemAdministration" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 16, ParentId = 14, Enabled = true, Text = "Permissions", Url = new ActionUrl { Action = "Permissions", Controller = "SystemAdministration" } });

      //      fixedMenu.Add(new MenuViewModel { Id = 2, ParentId = null, Enabled = true, Text = "Connect" });
      //      fixedMenu.Add(new MenuViewModel { Id = 17, ParentId = 2, Enabled = true, Text = "Drivers" });
      //      fixedMenu.Add(new MenuViewModel { Id = 18, ParentId = 17, Enabled = true, Text = "Drivers", Url = new ActionUrl { Action = "Drivers", Controller = "Connect" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 19, ParentId = 17, Enabled = true, Text = "Device Driver Management", Url = new ActionUrl { Action = "DeviceDriver", Controller = "Connect" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 20, ParentId = 17, Enabled = true, Text = "Monitor", Url = new ActionUrl { Action = "Monitor", Controller = "Connect" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 21, ParentId = 17, Enabled = true, Text = "Actual Device", Url = new ActionUrl { Action = "ActualDevice", Controller = "Connect" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 22, ParentId = 17, Enabled = true, Text = "Actual Device Images", Url = new ActionUrl { Action = "ActualDeviceImages", Controller = "Connect" } });


      //      fixedMenu.Add(new MenuViewModel { Id = 3, ParentId = null, Enabled = true, Text = "Messenger", Url = new ActionUrl { Action = "Index", Controller = "Messenger" } });


      //      fixedMenu.Add(new MenuViewModel { Id = 4, ParentId = null, Enabled = true, Text = "Mobile", Url = new ActionUrl { Action = "Index", Controller = "Mobile" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 23, ParentId = 4, Enabled = true, Text = "Monitor", Url = new ActionUrl { Action = "Monitor", Controller = "Mobile" } });

      //      fixedMenu.Add(new MenuViewModel { Id = 5, ParentId = null, Enabled = true, Text = "Report Master", Url = new ActionUrl { Action = "Index", Controller = "ReportMaster" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 24, ParentId = 5, Enabled = true, Text = "Templates", Url = new ActionUrl { Action = "Templates", Controller = "ReportMaster" } });


      //      fixedMenu.Add(new MenuViewModel { Id = 6, ParentId = null, Enabled = true, Text = "Scoring", Url = new ActionUrl { Action = "Index", Controller = "Mobile" } });
      //      //   fixedMenu.Add(new MenuViewModel { Id = 7, ParentId = null, Enabled = true, Text = "Alaris Infusion Central", Url = new ActionUrl { Action = "Index", Controller = "AlarisInfusionCentral" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 8, ParentId = null, Enabled = true, Text = "Product Version", Url = new ActionUrl { Action = "Index", Controller = "ProductVersion" } });

      //      fixedMenu.Add(new MenuViewModel { Id = 35, ParentId = null, Enabled = true, Text = "Integrations" });
      //      fixedMenu.Add(new MenuViewModel { Id = 36, ParentId = 35, Enabled = true, Text = "Ascom Telligence" });
      //      fixedMenu.Add(new MenuViewModel { Id = 37, ParentId = 36, Enabled = true, Text = "Telligence Servers", Url = new ActionUrl { Action = "Servers", Controller = "Telligence" } });

      //      fixedMenu.Add(new MenuViewModel { Id = 25, ParentId = null, Enabled = true, Text = "Actions" });
      //      fixedMenu.Add(new MenuViewModel { Id = 26, ParentId = 25, Enabled = true, Text = "Network Probe", Url = new ActionUrl { Action = "NetworkProbe", Controller = "Actions" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 27, ParentId = 25, Enabled = true, Text = "Privacy Logout", Url = new ActionUrl { Action = "PrivacyLogout", Controller = "Actions" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 28, ParentId = 25, Enabled = true, Text = "Shut down every {product}", Url = new ActionUrl { Action = "Shutdown", Controller = "Actions" } });
      //      fixedMenu.Add(new MenuViewModel { Id = 29, ParentId = 25, Enabled = true, Text = "Change MessageCenter", Url = new ActionUrl { Action = "ChangeMessageCenter", Controller = "Actions" } });
      //   }
      //   catch (Exception ex)
      //   {

      //      throw;
      //   }

      //   return fixedMenu;
      //}

      #endregion

      #endregion
   }
}
