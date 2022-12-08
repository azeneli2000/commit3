using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Digistat.FrameworkStd.Model.Integration.Telligence;
using ConfiguratorWeb.App.Models;

namespace ConfiguratorWeb.App.Builders
{
   public static class TelligenceServerModelBuilder
   {

      public static TelligenceServer Build(TelligenceServerViewModel source)
      {
         TelligenceServer objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceServer
               {
                  ts_ID = source.ID,
                  ts_serverurl = source.ServerURL,
                  ts_ImtBridgeWebApiPassword = source.IMTBridgePassword,
                  ts_ImtBridgeWebApiURL = source.IMTBridgeWebAPIUrl,
                  ts_ImtBridgeWebApiUsername = source.IMTBridgeUsername,
                  ts_cfgHandlerUsername = source.TLConfigHandlerUsername,
                  ts_cfgHandlerPassword = source.TLConfigHandlerPassword,
                  ts_cfgHandlerURL = source.TLConfigHandlerURL
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceServer> BuildList(IEnumerable<TelligenceServerViewModel> source)
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
