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
   public static class StandardParameterViewModelBuilder
   {
      public static StandardParameterViewModel Build(StandardParameter source)
      {
         StandardParameterViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new StandardParameterViewModel
               {
                 CaseSensitive = source.CaseSensitive,
                 Classes = source.Classes,
                 DataType = source.DataType,
                 Description = source.Description,
                 Devices = source.Devices,
                 ID = source.Id,
                 IsSystem = source.IsSystem,
                 Mnemonic = source.Mnemonic,
                 Notes = source.Notes,
                 Parameters = source.Parameters,
                 Print = source.Print,
                 Type = source.Type,
                 UCUMCaseSensitive = source.UCUMCaseSensitive,
                 UOMIds = source.UOMIds,
                 IsNew = false
               };
            }
         }
         catch
         {
         }

         return objDest;
      }

      public static IEnumerable<StandardParameterViewModel> BuildList(IEnumerable<StandardParameter> source)
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
