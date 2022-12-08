using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class StandardScoreDescriptionEntityBuilder
   {
      public static StandardDatasetScoreDescription Build(StdScoreDescriptionViewModel source)
      {
         StandardDatasetScoreDescription objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new StandardDatasetScoreDescription
               {
                     dsr_ID = source.ID,
                     dsr_ColorCode = source.ColorCode,
                     dsr_Description = source.Description,
                     dsr_MaxValue = source.MaxValue,
                     dsr_MinValue = source.MinValue,
                     dsr_sd_ID = source.DatasetID
               };
            }
         }
         catch
         {
         }

         return objDest;
      }

      public static IEnumerable<StandardDatasetScoreDescription> BuildList(IEnumerable<StdScoreDescriptionViewModel> source)
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
