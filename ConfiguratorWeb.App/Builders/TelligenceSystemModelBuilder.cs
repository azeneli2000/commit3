using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Builders
{
   public static class TelligenceSystemModelBuilder
   {

      public static TelligenceSystem Build(TelligenceSystemViewModel source)
      {
         TelligenceSystem objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceSystem
               {
                  ty_ID = source.ID,
                  ty_ts_ID = source.ServerID,
                  ty_MDIEncKey = source.MDIEncryptionKey,
                  ty_MDIPort = source.MDIPort,
                  ty_telGUID = source.TLSystemGUID,
                  ty_hostID = source.HostID
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceSystem> BuildList(IEnumerable<TelligenceSystemViewModel> source)
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
