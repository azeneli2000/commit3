using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class LocationEntityBuilder
   {

      public static Location Build(LocationViewModel source)
      {
         Location objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new Location
               {
                  HospitalUnitGuid = source.HospitalUnitGUID,
                  HospitalUnitName = source.HospitalUnitName,
                  Id = source.ID,
                  LocationCode = source.LocationCode,
                  LocationIndex = source.LocationIndex,
                  LocationName = source.LocationName,
                  UniteCode = source.UniteCode
               };
            }
         }
         catch (Exception)
         {
            //TODO
         }

         return objDest;
      }

   }
}
