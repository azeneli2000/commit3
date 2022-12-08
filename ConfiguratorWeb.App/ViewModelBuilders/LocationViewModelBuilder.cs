using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class LocationViewModelBuilder
   {
      public static LocationViewModel Build(Location source)
      {
         LocationViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new LocationViewModel
               {
                  ID = source.Id,
                  HospitalUnitGUID = source.HospitalUnitGuid,
                  LocationCode = source.LocationCode,
                  LocationIndex = source.LocationIndex,
                  LocationName = source.LocationName,
                  HospitalUnitName = source.HospitalUnitName,
                  UniteCode = source.UniteCode,
               };
            }
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }
      public static IEnumerable<LocationViewModel> BuildList(IEnumerable<Location> source)
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
