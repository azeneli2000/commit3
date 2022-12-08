using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class DeviceDriverBedLinkViewModel
    {
        public int DeviceDriverId { get; set; }

        public int BedId { get; set; }

        public string DriverSideBedName { get; set; }

        public bool DriverEnabled { get; set; }

        public bool WatchDogEnabled { get; set; }

        public bool WatchDogEnable { get; set; }

        public virtual BedViewModel Bed { get; set; }

        public virtual DeviceDriverViewModel DeviceDriver3 { get; set; }
    }
}