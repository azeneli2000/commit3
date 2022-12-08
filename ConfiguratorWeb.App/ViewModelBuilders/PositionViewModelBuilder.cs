using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class PositionViewModelBuilder
   {
      public static PositionViewModel Build(PositionAssociation source)
      {
         PositionViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new PositionViewModel()
               {
                  PositionCode = source.PositionCode,
                  Description = source.Description,
                  BedList = source.PositionBedLinks != null ? BuildNetworkBedLinks(source.PositionBedLinks).OrderBy(a => a.IdLocation) : null,
                  SavedPositionCode = source.PositionCode,
               };

               objDest.LinkedBedNumber = objDest.BedList.Count();
            }
         }
         catch (Exception)
         {

            throw;
         }
         return objDest;
      }

      private static IEnumerable<BedViewModel> BuildNetworkBedLinks(ICollection<PositionBedLink> objBedLinks)
      {
         List<BedViewModel> objret = new List<BedViewModel>();
         if (objBedLinks != null)
         {
            foreach (PositionBedLink objBed in objBedLinks)
            {
               if (objBed != null && objBed.Bed != null)
               {
                  objret.Add(BedViewModelBuilder.Build(objBed.Bed));
               }

            }
         }
         return objret.AsEnumerable();
      }

      public static IEnumerable<PositionViewModel> BuildList(IEnumerable<PositionAssociation> source)
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
