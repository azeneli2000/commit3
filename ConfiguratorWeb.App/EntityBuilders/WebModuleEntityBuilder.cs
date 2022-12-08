using ConfiguratorWeb.App.Models.General;
using Digistat.FrameworkStd.Model.ControlBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class WebModuleEntityBuilder
   {
      public static WebModule Build(WebModuleViewModel source)
      {
         WebModule objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new WebModule
               {
                  ID = source.Id,
                  ModuleName = source.ModuleName,
                  Url = source.ModuleURL,
                  IsActive = source.Active,
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<WebModule> BuildList(IEnumerable<WebModuleViewModel> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
