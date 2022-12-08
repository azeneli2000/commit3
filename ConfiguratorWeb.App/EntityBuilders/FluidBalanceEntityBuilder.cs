using ConfiguratorWeb.App.Models.FluidBalance;
using Digistat.FrameworkStd.Model.FluidBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public class FluidBalanceEntityBuilder
   {

      public static FluidBalanceItemModel Build(FluidBalanceViewModel source)
      {
         FluidBalanceItemModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new FluidBalanceItemModel
               {
                 
                  Id = source.Id,
                  Name = source.Name,
                  Description = source.Description,
                  Labels = source.Labels,
                  Sql = source.Sql,
                  Permanent = source.Permanent,
                  Once = source.Once,
                  Mode = (short)source.Mode,
                  IdLocation = source.IdLocation,
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }




      public static IEnumerable<FluidBalanceItemModel> BuildList(IEnumerable<FluidBalanceViewModel> source)
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

