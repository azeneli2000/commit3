using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Actions
{
   public class ServiceStatusViewModel
   {
      public string Application { get; set; }

      public string Hostname { get; set; }

      public string Status { get; set; }

      public DateTime LastUpdateUTC { get; set; }

      public string LastUpdateString { get { return LastUpdateUTC.ToLocalTime().ToString(); } }
   }
}
