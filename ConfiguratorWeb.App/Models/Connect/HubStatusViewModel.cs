using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class HubStatusViewModel
   {
      public int HubID { get; set; }
      public string HubDescription { get; set; }
      public bool HubOnline { get; set; }
      public bool HubDBOnline { get; set; }
      public string HubLocation { get; set; }
      public string HubOnLineImg { get; set; }
      public string HubDBOnLineImg { get; set; }
      public string Throughput { get; set; }
      public int FatherHubID { get; set; }
      public string HostName { get; set; }
   }
}