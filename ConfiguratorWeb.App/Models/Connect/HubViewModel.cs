using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class HubViewModel
   {
      public int HubID { get; set; }
      public string HubDescription { get; set; }
      public string HubLocation { get; set; }
      public string HostName { get; set; }
      public int FatherHubID { get; set; }
      public bool DBConnected { get; set; }
      public string DBConnectionString { get; set; }

   }
}