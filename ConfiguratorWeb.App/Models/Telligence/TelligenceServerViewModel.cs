using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   public class TelligenceServerViewModel
   {
      public TelligenceServerViewModel()
      {

      }


      public int ID { get; set; }
      [Required]
      [TranslatedDisplayAttribute("Server URL")]
      public string ServerURL { get; set; }

      [TranslatedDisplayAttribute("IMT Bridge WebAPI URL")]
      public string IMTBridgeWebAPIUrl { get; set; }

      [TranslatedDisplayAttribute("IMT Bridge UserName")]
      public string IMTBridgeUsername { get; set; }

      [TranslatedDisplayAttribute("IMT Bridge Password")]
      public string IMTBridgePassword { get; set; }

      [TranslatedDisplayAttribute("Config Handler URL")]
      public string TLConfigHandlerURL { get; set; }

      [TranslatedDisplayAttribute("Config Handler Username")]
      public string TLConfigHandlerUsername { get; set; }


      [TranslatedDisplayAttribute("Config Handler Password")]
      public string TLConfigHandlerPassword { get; set; }



   }
}
