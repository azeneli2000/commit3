using ConfiguratorWeb.App.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.General
{
   public class WebModuleViewModel
   {
      public int Id { get; set; }

      [TranslatedDisplayAttribute("Module Name")]
      public string ModuleName { get; set; }

      [TranslatedDisplayAttribute("URL")]
      public string ModuleURL { get; set; }

      [TranslatedDisplayAttribute("Active")]
      public bool Active { get; set; }

   }
}
