using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   public class TelligenceSystemViewModel
   {
      public TelligenceSystemViewModel()
      {

      }
      
      public int ID { get; set; }

      public int ServerID { get; set; }

      [TranslatedDisplayAttribute("Telligence Server")]
      public string TelligenceServerDescription { get; set; }

      [Required]
      [TranslatedDisplayAttribute("Telligence System GUID")]
      public string TLSystemGUID { get; set; }

      [TranslatedDisplayAttribute("MDI Encryption Key")]
      public string MDIEncryptionKey { get; set; }

      [TranslatedDisplayAttribute("MDI Port")]
      public int MDIPort { get; set; }

      [TranslatedDisplayAttribute("Telligence Host ID")]
      public int? HostID { get; set; }




   }
}
