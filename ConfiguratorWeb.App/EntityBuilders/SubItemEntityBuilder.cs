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
   public static class SubItemEntityBuilder
   {
      public static StandardDatasetSubItems Build(SDSubItemViewModel source)
      {
         StandardDatasetSubItems objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new StandardDatasetSubItems
               {
                 li_ColorCode = source.ColorCode,
                 li_ID = source.ID,
                 li_Index = source.SubItemIndex,
                 li_Label = source.Label,
				 li_Code = source.Code,
                 li_si_ID = source.ItemID,
                 li_Value = source.SubItemValue
               };
            }
         }
         catch
         {
         }

         return objDest;
      }

      public static IEnumerable<StandardDatasetSubItems> BuildList(IEnumerable<SDSubItemViewModel> source)
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
