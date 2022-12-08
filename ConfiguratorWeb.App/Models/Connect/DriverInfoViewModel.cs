using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class DriverInfoViewModel
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        [DisplayName("Supported Devices")]
        public string SupportedDevices { get; set; }
        public string Type { get; set; }

        [DisplayName("Hardware Release")]
        public string HardwareRelease { get; set; }

        [DisplayName("Software Release")]
        public string SoftwareRelease { get; set; }

        [DisplayName("Dynamic Parameters")]
        public bool DynamicParameters { get; set; }

        [DisplayName("Alarm Management")]
        public string AlarmManagement { get; set; }

        [DisplayName("Driver Version")]
        public string DriverVersion { get; set; }
    }
}