using ConfiguratorWeb.App.Models.General;
using Digistat.FrameworkStd.Model.ControlBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public class WebModuleViewModelBuilder
   {
      public static WebModuleViewModel Build(WebModule source)
      {
         WebModuleViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new WebModuleViewModel
               {
                  Id = source.ID,
                  ModuleName = source.ModuleName,
                  ModuleURL = source.Url,
                  Active = source.IsActive
               };
            }
         }
         catch
         {
         }

         return objDest;
      }
   }
}
