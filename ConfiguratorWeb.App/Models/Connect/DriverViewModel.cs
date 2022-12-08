using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class DriverViewModel
   {
      public DriverViewModel()
      {
         EventCatalog = new List<DriverEventCatalogViewModel>();
         Capabilities = new List<DriverCapabilityViewModel>();
         DriverFilesStatusDescription = "Driver files missing";
      }

      public string Id { get; set; }

      [TranslatedDisplayAttribute("Version")]
      public int Version { get; set; }
      public bool Current { get; set; }
      public DateTime? ValidToDate { get; set; }

      [TranslatedDisplayAttribute("Driver Name")]
      [Required(ErrorMessage = "Driver Name is required")]
      [RegularExpression(@"^[a-zA-Z0-9_ ]{1,30}", ErrorMessage = @"Allowed only a-Z, 0-9 or (' ','_') ")]
      public string DriverName { get; set; }

      [Required(ErrorMessage = "Driver Version is required")]
      [TranslatedDisplayAttribute("User Version")]
      public string DriverVersion { get; set; }
      public bool IsWrapper { get; set; }
      //public byte[] Stream { get; set; }
      //public int StreamSize { get; set; }
      public int FileCount { get; set; }

      public bool HasContentInStream { get; set; }

      public string EntryExe { get; set; }
      public DateTime? LastStreamUpdate { get; set; }

      [TranslatedDisplayAttribute("Notes")]
      public string Note { get; set; }
      public string ComToRegister { get; set; }
      public string DefaultCommConfiguration { get; set; }

      [TranslatedDisplayAttribute("Manufacturer")]
      public string Manufacturer { get; set; }
      public string Device { get; set; }

      [TranslatedDisplayAttribute("Model")]
      public string DriverModel { get; set; }

      [TranslatedDisplayAttribute("Type")]
      public string DeviceType { get; set; }

      [TranslatedDisplayAttribute("Device Type")]
      public string DeviceTypeDesc { get; set; }

      [TranslatedDisplayAttribute("Version Build")]
      public string DriverVersionBuild { get; set; }

      [TranslatedDisplayAttribute("Hardware Release")]
      public string HardwareRelease { get; set; }

      [TranslatedDisplayAttribute("Software Release")]
      public string SoftwareRelease { get; set; }
      public string FormatStyle { get; set; }
      public string RemappedEvents { get; set; }

      [TranslatedDisplayAttribute("Alarms Management")]
      public AlarmSupportTypes AlarmSupport { get; set; }
      public string AlarmSupportDescription { get; set; }

      [TranslatedDisplayAttribute("Alarm System Type")]
      public short AlarmSystemType { get; set; }
      public string AlarmSystemTypeDescription { get; set; }

      public bool? RunAsDLL { get; set; }


      [TranslatedDisplayAttribute("Dynamic Parameters")]
      public bool UseDynamicParameters { get; set; }

      /// <summary>
      /// Used to identify location for temporary cached files
      /// </summary>
      public string BinariesCacheIdentifier { get; set; }

      public string DriverFilesStatusDescription { get; set; }

      public bool DriverFileExisting { get; set; }

      /// <summary>
      /// Used only to update driver binaries
      /// </summary>
      public IEnumerable<IFormFile> DriverFiles { get; set; }


      public IEnumerable<DriverCapabilityViewModel> Capabilities { get; set; }
      public IEnumerable<DriverEventCatalogViewModel> EventCatalog { get; set; }
      public DriverInfoViewModel Info { get; set; }

      public bool IsBinFile { get; set; }

      public bool KeepCapabilities { get; set; }

      public bool KeepSmartCentralFormatString { get; set; }
      public string CapabilitiesChanged { get; set; }
      public string CapabilitiesSerialize { get; set; }
      public string EventCatalogChanged { get; set; }
      public string EventCatalogSerialize { get; set; }
   }
}