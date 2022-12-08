using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Digistat.FrameworkStd.Enums;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class SystemOptionViewModelBuilder
   {
      public static SystemOptionViewModel Build(SystemOption source, bool mapEmptyApplicationToGenericFilter =false)
      {
         SystemOptionViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new SystemOptionViewModel
               {
                  Application = source.Application,
                  Description = source.Description,
                  GUID = source.Guid,
                  HospitalUnitGUID = source.HospitalUnitGUID,
                  HostName = source.HostName,
                  IsSystem = source.IsSystem,
                  Level = source.Level,
                  LowerLimit = source.LowerLimit.HasValue ? source.LowerLimit.Value : 0,
                  Name = source.Name,
                  Type = (OptionType)source.Type,
                  UpperLimit = source.UpperLimit.HasValue ? source.UpperLimit.Value : 0,
                  UserName = source.UserAbbreviation,
                  Value = source.Value,
                  HospitalUnit = HospitalUnitViewModelBuilder.Build(source.HospitalUnit),
                  User = UserViewModelBuilder.Build(source.User),
                  ValueDisplayBinary = (source.Type==(int)Digistat.FrameworkStd.Enums.OptionType.Binary?source.Value:source.Value)
               };
               if (mapEmptyApplicationToGenericFilter)
               {
                  objDest.Application = (string.IsNullOrWhiteSpace(source.Application) ? CommonStrings.GENERAL_APPLICATION_FILTER : source.Application);
               }
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<SystemOptionViewModel> BuildList(IEnumerable<SystemOption> source)
      {
         try
         {
            return source.Select(m=>Build(m,false));
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}