using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class DriverEventCatalogEntityBuilder
   {
      public static DriverRepositoryEventCatalog Build(DriverEventCatalogViewModel source)
      {
         DriverRepositoryEventCatalog objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DriverRepositoryEventCatalog
               {
                  Id = source.Code,
                  Type= source.Level,
                  Class= source.Class,
                  DescriptionShort= source.ShortText!=null?source.ShortText:string.Empty,
                  DescriptionLong = source.LongText !=null?source.LongText:string.Empty,
                  NewClass = source.DriverEventClass!=null && source.DriverEventClass.ClassId>=-1 ?source.DriverEventClass.ClassId: source.NewClass,
                  NewLevel = source.DriverEventLevel!=null && source.DriverEventLevel.LevelId>=-1 ?source.DriverEventLevel.LevelId: source.NewLevel,
                  TextENG  = source.TextENG,
                  TextENGShort = source.TextENGShort,
                  TextUser = source.TextUser,
                  TextUserShort = source.TextUserShort
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DriverRepositoryEventCatalog> BuildList(IEnumerable<DriverEventCatalogViewModel> source)
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
