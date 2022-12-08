using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class BedViewModelBuilder
   {
      public static BedViewModel Build(Bed source)
      {
         BedViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new BedViewModel
               {
                  BedId = source.Id,
                  BedIndex = source.Index,
                  BedCode = source.BedCode,
                  IdLocation = source.IdLocation,
                  BedName = source.Name,
                  PatientId = source.IdPatient,
                  Properties = source.Properties,
                  UniteCode = source.UniteCode,
                  RoomName = source.RoomName,
                  Location = (source.Location != null ? LocationViewModelBuilder.Build(source.Location) : new LocationViewModel { ID = (source.IdLocation.HasValue ? source.IdLocation.Value : 0)})
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<BedViewModel> BuildList(IEnumerable<Bed> source)
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
