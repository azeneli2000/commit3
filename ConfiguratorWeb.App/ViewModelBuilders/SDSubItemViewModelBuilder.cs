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
   public static class SDSubItemViewModelBuilder
   {
      public static SDSubItemViewModel Build(StandardDatasetSubItems source)
      {
         SDSubItemViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new SDSubItemViewModel
               {
                  ID = source.li_ID,
                  ItemID = source.li_si_ID,
				  Code = source.li_Code,
                  SubItemIndex = source.li_Index,
                  ColorCode = source.li_ColorCode,
                  SubItemValue = source.li_Value,
                  Label = source.li_Label

               };
            }
         }
         catch
         {
         }

         return objDest;
      }

      public static IEnumerable<SDSubItemViewModel> BuildList(IEnumerable<StandardDatasetSubItems> source)
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
