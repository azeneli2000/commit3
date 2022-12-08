using Configurator.Std.BL.Vitals;
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
   public static class TelligenceCfgSettingsBuilder
   {
      public static TelligenceConfigHandlerConfiguration Build(TelligenceCfgSettingsViewModel source)
      {
         TelligenceConfigHandlerConfiguration objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceConfigHandlerConfiguration
               {
                  ServerURL = source.ServerURL,
                  Password = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.EncryptString(source.Password, null),
                  Username = source.UserName
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceConfigHandlerConfiguration> BuildList(IEnumerable<TelligenceCfgSettingsViewModel> source)
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
