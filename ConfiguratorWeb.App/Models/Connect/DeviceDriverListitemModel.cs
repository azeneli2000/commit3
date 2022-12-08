using ConfiguratorWeb.App.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class DeviceDriverListitemModel
   {

      public int Id { get; set; }

      [Display(Name = "Type")]
      public string DriverType { get; set; }

      public string ComputerName { get; set; }

      public string DeviceName { get; set; }

      //public string CommConfiguration { get; set; }

      [Display(Name = "WD Auto")]
      public bool AutoStartWatchDog { get; set; }

      [Display(Name = "Logging")]
      public bool LogEnabled { get; set; }

      public string IdDriverRepository { get; set; }

      [Display(Name = "Auto")]
      public bool AutoStartDriver { get; set; }

      public string Description { get; set; }

      public string Name { get; set; }
      public string BedLink { get; set; }
      public string Version { get; set; }
      public string Address { get; set; }

      [Display(Name = "Alarm")]
      public string AlarmSystemType { get; set; }

   }
}