using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class DriverInstanceStatusViewModel
   {
      public int HubID { get; set; }
      public int DriverID { get; set; }
      public int DriverInstanceID { get; set; }
      public string DriverName { get; set; }
      public string MachineName { get; set; }
      public string DeviceName { get; set; }
      public string DeviceSerial { get; set; }
      public bool DriverInstanceStatus { get; set; }
      public string DriverInstanceStatusImg { get; set; }
      public string DriverVersion { get; set; }
      
   }
}