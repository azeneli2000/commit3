
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
   public class DasStatusViewModel
   {

      [TranslatedDisplay("Broker")]
      public string DasBroker { get; set; }

      public int DeviceDriverId { get; set; }
      public int ProcessId { get; set; }

      [TranslatedDisplay("Name")]
      public string Name { get; set; }

      [TranslatedDisplay("Version")]
      public string Version { get; set; }

      [TranslatedDisplay("Last Dataset")]
      public string LastDatasetElaspedTime { get; set; }

      [TranslatedDisplay("Process Status")]
      public string ProcessStatus { get; set; }

      public string ProcessStatusClass { get; set; }

      //[TranslatedDisplay("Process Status")]
      //public string ProcessStatusDescription { get; set; }

      public int DriverStatus { get; set; }

      public string DriverStatusClass { get; set; }

      [TranslatedDisplay("Driver Status")]
      public string DriverStatusDescription { get; set; }

      public int DeviceStatus { get; set; }

      public string DeviceStatusClass { get; set; }

      [TranslatedDisplay("Device Status")]
      public string DeviceStatusDescription { get; set; }

      [TranslatedDisplay("Device Message")]
      public string DeviceMessage { get; set; }

      [TranslatedDisplay("Address")]
      public string Address { get; set; }

      [TranslatedDisplay("Bed")]
      public string BedName { get; set; }

   }
}