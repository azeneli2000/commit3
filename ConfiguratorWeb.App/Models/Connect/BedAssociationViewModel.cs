using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class BedAssociationViewModel
    {
      public int BedId { get; set; }
      public int BedIndex { get; set; }
      public int LocationId { get; set; }
      public string Bedcode { get; set; }
        public string BedName { get; set; }
        public bool Watchdog { get; set; }
        public bool Enabled { get; set; }
        public string Location { get; set; }
        public string DriverSideBedName { get; set; }
    }
}