using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfiguratorWeb.App.Models.General
{
   public class DropdownModel
   {
      public string Text { get; set; }
      public string Value { get; set; }
      public SelectListGroup Group  { get; set; }
   }
}
