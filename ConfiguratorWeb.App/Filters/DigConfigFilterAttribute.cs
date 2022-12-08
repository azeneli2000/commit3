using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Filters
{
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
   public class DigConfigFilterAttribute : TypeFilterAttribute
   {
      public DigConfigFilterAttribute()
          : base(typeof(DigConfigFilter))
      {
      }
   }
}
