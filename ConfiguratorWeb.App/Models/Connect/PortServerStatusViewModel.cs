using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class PortServerStatusViewModel
    {
        public int ID { get; set; }
        public short Port { get; set; }
        public string State { get; set; }
        public string CableId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string SerialNumber { get; set; }

        public string DeviceDescription { get; set; }

        public Nullable<int> DriverID { get; set; }
        public Nullable<System.DateTime> LastConnection { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }

        public string DeviceHash { get; set; }

        public BedViewModel Bed { get; set; }
    }
}
