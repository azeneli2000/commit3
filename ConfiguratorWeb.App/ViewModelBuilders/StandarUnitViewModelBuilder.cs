using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class StandarUnitViewModelBuilder
   {
      public static StandardUnitViewModel Build(StandardUnit source)
      {
         StandardUnitViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new StandardUnitViewModel
               {
               Description = source.Description,
               ID = source.Id,
               IsSystem = source.IsSystem,
               Notes = source.Notes,
               Print = source.Print,
               UCUMCaseSensitive = source.UCUMCaseSensitive,
               UCUMCaseInsensitive = source.UCUMCaseInsensitive
               };
            }
         }
         catch
         {
         }

         return objDest;
      }

      public static IEnumerable<StandardUnitViewModel> BuildList(IEnumerable<StandardUnit> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch
         {
            throw;
         }
      }
   }
}
