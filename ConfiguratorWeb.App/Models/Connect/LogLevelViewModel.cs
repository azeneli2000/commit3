using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{   
    public class LogLevelViewModel
   {
        public string Label { get; set; }
        public DASLogLevel LogLevel { get; set; }
        public bool Value { get; set; }
   }

}