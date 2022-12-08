using ConfiguratorWeb.App.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class StandardUnitViewModel
   {
      [TranslatedDisplayAttribute("ID")]
      public int ID { get; set; }

      [TranslatedDisplayAttribute("Description")]
      public string Description { get; set; }

      [TranslatedDisplayAttribute("Print")]
      public string Print { get; set; }

      [TranslatedDisplayAttribute("UCUM Case Sensitive")]
      public string UCUMCaseSensitive { get; set; }

      [TranslatedDisplayAttribute("UCUM Case Insensitive")]
      public string UCUMCaseInsensitive { get; set; }

      [TranslatedDisplayAttribute("Notes")]
      public string Notes { get; set; }

      [TranslatedDisplayAttribute("Is System")]
      public bool IsSystem { get; set; }
   }
}