using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Model.Integration.Telligence;

namespace ConfiguratorWeb.App.Models
{
   public class ListWithGridViewModel
   {
      public int ID { get; set; }

      [TranslatedDisplayAttribute("Telligence System")]
      public int? tl_ty_ID { get; set; }

      [TranslatedDisplayAttribute("Telligence LocationID")]
      public int? TLLocationID { get; set; }
      [TranslatedDisplayAttribute("Telligence DeviceID")]
      public int? TLDeviceID { get; set; }
      [TranslatedDisplayAttribute("ID Network")]
      public int? NetworkID { get; set; }
      [TranslatedDisplayAttribute("Port Server")]
      public int tl_psv_ID { get; set; }
      [TranslatedDisplayAttribute("IP Address")]
      public string tl_IPAddress { get; set; }
      [TranslatedDisplayAttribute("MAC Address")]
      public string tl_MACAddress { get; set; }

      public TelligenceSystem tl_ty_ { get; set; }

      [TranslatedDisplayAttribute("Device Type")]
      public TelligenceXMLRPCClient.Entities.StaffStationTypes tl_DeviceType { get; set; }

      public bool CreateNetwork { get; set; }

      public bool CreatePortServer { get; set; }

      [TranslatedDisplayAttribute("Port Server")]
      public bool HasPortServer { get; set; }

      [TranslatedDisplayAttribute("Network")]
      public bool HasNetwork { get; set; }

      [TranslatedDisplayAttribute("Name")]
      public string TLDeviceName { get; set; }

      [TranslatedDisplayAttribute("Location")]
      public string  TLLocationDescriptor { get; set; }

      [TranslatedDisplayAttribute("Beds")]
      public int BedCount { get; set; }




   }
}
