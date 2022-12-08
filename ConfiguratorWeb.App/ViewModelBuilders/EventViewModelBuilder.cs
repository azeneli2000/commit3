using System;
using System.Linq;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System.Collections.Generic;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class EventViewModelBuilder
   {
      public static DriverRepositoryEventCatalogViewModel Build(DriverRepositoryEventCatalog source)
      {
         DriverRepositoryEventCatalogViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DriverRepositoryEventCatalogViewModel
               {
                  Id = source.Id,
                  DescriptionShort = source.DescriptionShort,
                  DescriptionLong = source.DescriptionLong
               };
            }
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }
      public static IEnumerable<DriverRepositoryEventCatalogViewModel> BuildList(IEnumerable<DriverRepositoryEventCatalog> source)
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
