using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class HubDataFlowViewModel
   {
      public int HubID { get; set; }
      public string DataMeasure { get; set; }
      public string DataUnit { get; set; }
      public string DataValue { get; set; }
   }
}