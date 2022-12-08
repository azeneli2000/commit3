using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.SystemMonitoring
{
   public class ChartModel
   {
      public string nameAppliaction { get; set; }
      public string indicator { get; set; }

      public string component { get; set; }
      public int hour { get; set; }

      public List<ChartModel2> chart = new   List<ChartModel2>();

      public double LastUpdateIndicator { get; set; }


 
   }
}
