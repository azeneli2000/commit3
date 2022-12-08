using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class HospitalUnitRowViewModel
    {
        public string Beeper { get; set; }
        public string CellPhone { get; set; }
        public string Code { get; set; }
        public string CostUnit { get; set; }
        public bool Current { get; set; }
        public string Description { get; set; }
        public string ExternalKey { get; set; }
        public string ID { get; set; }
        public bool InheritsSlots { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string ParentID { get; set; }
        public string Phone { get; set; }
        public string ShortName { get; set; }
        public short Type { get; set; }
        public string ValidToDate { get; set; }
        public int Version { get; set; }
    }
}