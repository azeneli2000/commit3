using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class SystemOptionEntityBuilder
   {

      public static SystemOption Build(SystemOptionViewModel source)
      {
         SystemOption objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new SystemOption
               {
                  Application = source.Application,
                  Description = source.Description,
                  Guid = source.GUID,
                  HospitalUnitGUID = source.HospitalUnitGUID,
                  HostName = source.HostName,
                  IsSystem = source.IsSystem,
                  Level = source.Level,
                  LowerLimit = source.LowerLimit,
                  Name = source.Name,
                  Type = (int?)source.Type,
                  UpperLimit = source.UpperLimit,
                  UserAbbreviation = source.UserName,
                  Value =(source.Type==Digistat.FrameworkStd.Enums.OptionType.Bool? UtilityHelper.ConvertBoolToString(source.Value):Convert.ToString(source.Value??"")),
                  
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

   


      public static IEnumerable<SystemOption> BuildList(IEnumerable<SystemOptionViewModel> source)
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
