using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.FluidBalance;
using Digistat.FrameworkStd.Model.FluidBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Digistat.FrameworkStd.Enums.EnumModeType;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public class FluidBalanceViewModelBuilder
   {

      public static FluidBalanceViewModel Build(FluidBalanceItemModel source)
      {
         FluidBalanceViewModel objDest = null;
         string strMode = ""; 
         try
         {
            switch(source.Mode)
            {
               case 1:
                  strMode = ModeType.FluidInput.ToString();
                  break;
               case 2:
                  strMode = ModeType.FluidOutput.ToString();
                  break;
               case 3:
                  strMode = ModeType.BloodInput.ToString();
                  break;
               case 4:
                  strMode = ModeType.BloodOutput.ToString();
                  break;
               case 0:
                  strMode = ModeType.Other.ToString();
                  break;


            }
            if (source != null)
            {

               objDest = new FluidBalanceViewModel
               {
                  Id = source.Id,
                  Name = source.Name,
                  Description = source.Description,
                  Labels = source.Labels,
                  Mode = (ModeType)source.Mode,
                  Sql = source.Sql,
                  Permanent = source.Permanent,
                  Once = source.Once,
                  ModeText = strMode,
                  IdLocation = source.IdLocation,
                  Location = (source.Location != null ? LocationViewModelBuilder.Build(source.Location) : new LocationViewModel { ID = (source.IdLocation.HasValue ? source.IdLocation.Value : 0) })
               };
              
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<FluidBalanceViewModel> BuildList(IEnumerable<FluidBalanceItemModel> source)
      {
         try
         {
            return source.Select(m =>Build(m));
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
