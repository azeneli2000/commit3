using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class SystemOptionsRowViewModel
    {
        public string Application { get; set; }
        public string Description { get; set; }
        public string GUID { get; set; }
        public string HospitalUnit { get; set; }
        public string HostName { get; set; }
        public bool IsSystem { get; set; }
        public int Level { get; set; }
        public double LowerLimit { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public double UpperLimit { get; set; }
        public string UserName { get; set; }
        public string Value { get; set; }
    }
}