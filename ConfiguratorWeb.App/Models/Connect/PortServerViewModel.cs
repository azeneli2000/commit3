using ConfiguratorWeb.App.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class PortServerViewModel
   {
      [TranslatedDisplayAttribute("ID")]
      public int ID { get; set; }

      [TranslatedDisplayAttribute("Type")]
      public Digistat.FrameworkStd.Enums.PortServerType Type { get; set; }


      public string TypeDescription { get; set; }

      [TranslatedDisplayAttribute("Port Count")]
      public int PortCount { get; set; }

      [Required]
      [TranslatedDisplayAttribute("Address")]
      public string Address { get; set; }

      [Required]
      public int? FirstPort { get; set; }

      [TranslatedDisplayAttribute("Admin. URI")]
      public string AdministativeURI { get; set; }

      [TranslatedDisplayAttribute("Username")]
      public string UserName { get; set; }
      [TranslatedDisplayAttribute("Password")]
      public string Password { get; set; }
      [TranslatedDisplayAttribute("Encryption Key")]
      public string EncryptionKey { get; set; }
      [TranslatedDisplayAttribute("Auth Info")]
      public string AuthInfo { get; set; }
      [TranslatedDisplayAttribute("Bed ID")]
      public int IDBED { get; set; }      
      [TranslatedDisplayAttribute("Bed")]
      public string BedName { get; set; }

      public DateTime UpdateDate { get; set; }
      [TranslatedDisplayAttribute("DAS Broker")]
      public string DASBroker { get; set; }

      public bool IsTelligenceType { get; set; }

   }
}