using ConfiguratorWeb.App.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Telligence
{
   public class TelligenceCfgSettingsViewModel
   {
      [TranslatedDisplayAttribute("Server URL")]
      public string ServerURL { get; set; }

      [TranslatedDisplayAttribute("User Name")]
      public string UserName { get; set; }

      [TranslatedDisplayAttribute("Password")]
      public string Password { get; set; }
   }
}
