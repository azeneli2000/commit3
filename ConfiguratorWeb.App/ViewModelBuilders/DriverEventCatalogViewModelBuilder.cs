using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Digistat.FrameworkStd.Enums;
using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.UMSLegacy;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DriverEventCatalogViewModelBuilder
   {
      public static DriverEventCatalogViewModel Build(DriverRepositoryEventCatalog source)
      {
         DriverEventCatalogViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DriverEventCatalogViewModel
               {
                  Class = source.Class,
                  ClassDescription = UMSFrameworkParser.GetAlarmClassDescription(source.Class),
                  Level = source.Type,
                  LevelDescription = UMSFrameworkParser.GetEventTypeDescription(source.Type),
                  LongText = source.DescriptionLong,
                  ShortText = source.DescriptionShort,
                  Code = source.Id,
                  NewClass = source.NewClass,
                  NewClassDescription = UMSFrameworkParser.GetAlarmClassDescription((short)source.NewClass),
                  NewLevel = source.NewLevel,
                  NewLevelDescription = UMSFrameworkParser.GetEventTypeDescription((short)source.NewLevel),
                  TextENG = source.TextENG,
                  TextENGShort = source.TextENGShort,
                  TextUser = source.TextUser,
                  TextUserShort = source.TextUserShort,
                  DriverEventClass = new DriverEventClassViewModel { ClassId = source.NewClass, ClassName = source.NewClass == -1 ? string.Empty : UMSFrameworkParser.GetAlarmClassDescription((short)source.NewClass) },
                  DriverEventLevel = new DriverEventLevelViewModel { LevelId = source.NewLevel, LevelName = source.NewLevel == -1 ? string.Empty : UMSFrameworkParser.GetEventTypeDescription((short)source.NewLevel) }

               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DriverEventCatalogViewModel> BuildList(IEnumerable<DriverRepositoryEventCatalog> source)
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