using ConfiguratorWeb.App.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class StandardParameterViewModel
   {
      public StandardParameterViewModel()
      {
         ID = 0;
         Mnemonic = String.Empty;
         Type = "PARAMETER";
         DataType = "STRING";
         Description = String.Empty;
         Print = String.Empty;
         CaseSensitive = String.Empty;
         UOMIds = "8";
         UCUMCaseSensitive = "NA";
         Classes = String.Empty;
         Devices = String.Empty;
         Parameters = String.Empty;
         Notes = String.Empty;
         IsSystem = false;
         IsNew = true;
      }

      public override bool Equals(object obj)
      {
         return base.Equals(obj);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      public override string ToString()
      {
         
         //ID = 0;
         //Mnemonic = String.Empty;
         //Type = "PARAMETER";
         //DataType = "STRING";
         //Description = String.Empty;
         //Print = String.Empty;
         //CaseSensitive = String.Empty;
         //UOMIds = "8";
         //UCUMCaseSensitive = "NA";
         //Classes = String.Empty;
         //Devices = String.Empty;
         //Parameters = String.Empty;
         //Notes = String.Empty;
         //IsSystem = false;
         //IsNew = true;
         return base.ToString();
      }


      [TranslatedDisplayAttribute("ID")]
      [Required]
      public int ID { get; set; }

      [TranslatedDisplayAttribute("Mnemonic")]
      [MaxLength(50)]
      public string Mnemonic { get; set; }

      [TranslatedDisplayAttribute("Type")]
      [MaxLength(50)]
      public string Type { get; set; }

      [TranslatedDisplayAttribute("DataType")]
      [Required]
      [MaxLength(50)]
      public string DataType { get; set; }

      [TranslatedDisplayAttribute("Description")]
      [Required]
      [MaxLength(255)]
      public string Description { get; set; }

      [TranslatedDisplayAttribute("Print")]
      [Required]
      [MaxLength(50)]
      public string Print { get; set; }

      [TranslatedDisplayAttribute("Case Sensitive")]
      //[Required]
      [MaxLength(50)]
      public string CaseSensitive { get; set; }

      [TranslatedDisplayAttribute("UOM list")]
      //[Required]
      [MaxLength(255)]
      public string UOMIds { get; set; }

      [TranslatedDisplayAttribute("UCUMCaseSensitive")]
      [MaxLength(255)]
      public string UCUMCaseSensitive { get; set; }

      [TranslatedDisplayAttribute("Classes")]
      //[Required]
      [MaxLength(255)]
      public string Classes { get; set; }

      [TranslatedDisplayAttribute("Devices")]
      //[Required]
      [MaxLength(255)]
      public string Devices { get; set; }

      [TranslatedDisplayAttribute("Parameters")]
      //[Required]
      [MaxLength(50)]
      public string Parameters { get; set; }

      [TranslatedDisplayAttribute("Notes")]
      [MaxLength(255)]
      public string Notes { get; set; }

      [TranslatedDisplayAttribute("IsSystem")]
      public bool IsSystem { get; set; }

      public bool IsNew { get; set; }
   }
}