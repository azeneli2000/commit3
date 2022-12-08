using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.SystemMonitoring
{
   public class ChartModel2
   {
      public double value { get; set; }

      public string date { get; set; }


     }
}
