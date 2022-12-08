using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Builders
{
   public static class TelligenceSystemViewModelBuilder
   {
      public static TelligenceSystemViewModel Build(TelligenceSystem source)
      {
         TelligenceSystemViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceSystemViewModel
               {
                  ID = source.ty_ID,
                  ServerID = source.ty_ts_ID.Value,
                  MDIEncryptionKey = source.ty_MDIEncKey,
                  MDIPort = source.ty_MDIPort!=null?source.ty_MDIPort.Value:0,
                  TLSystemGUID = source.ty_telGUID,
                  TelligenceServerDescription = source.ty_ts_ != null ? source.ty_ts_.ts_serverurl : null,
                  HostID = source.ty_hostID,
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceSystemViewModel> BuildList(IEnumerable<TelligenceSystem> source)
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
