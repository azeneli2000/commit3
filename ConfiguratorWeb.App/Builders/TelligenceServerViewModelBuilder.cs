using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Builders
{
   public static class TelligenceServerViewModelBuilder
   {
      public static TelligenceServerViewModel Build(TelligenceServer source)
      {
         TelligenceServerViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceServerViewModel
               {
                  ID = source.ts_ID,
                  ServerURL = source.ts_serverurl,
                  IMTBridgeUsername = source.ts_ImtBridgeWebApiUsername,
                  IMTBridgeWebAPIUrl = source.ts_ImtBridgeWebApiURL,
                  IMTBridgePassword = source.ts_ImtBridgeWebApiPassword,
                  TLConfigHandlerPassword = source.ts_cfgHandlerPassword,
                  TLConfigHandlerURL = source.ts_cfgHandlerURL,
                  TLConfigHandlerUsername = source.ts_cfgHandlerUsername
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceServerViewModel> BuildList(IEnumerable<TelligenceServer> source)
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
