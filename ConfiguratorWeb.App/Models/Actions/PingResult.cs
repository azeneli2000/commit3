using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Actions
{
   public class PingResult
   {
      public string Hostname { get; set; }
      public int Time { get; set; }
   }
}
