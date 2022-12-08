using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.Telligence;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class TelligenceCfgSettingsViewModelBuilder
   {
      public static TelligenceCfgSettingsViewModel Build(Configurator.Std.BL.Vitals.TelligenceConfigHandlerConfiguration source)
      {
         TelligenceCfgSettingsViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceCfgSettingsViewModel
               {
                  ServerURL = source.ServerURL,
                  Password = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.DecryptString(source.Password, null),
                  UserName = source.Username
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceCfgSettingsViewModel> BuildList(IEnumerable<Configurator.Std.BL.Vitals.TelligenceConfigHandlerConfiguration> source)
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
