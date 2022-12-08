using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Models.MonitoringApi;
using Digistat.FrameworkStd.Model.Monitoring;
using Digistat.FrameworkStd.UMSLegacy;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public class MonitoringDataViewModelBuilder
   {
      public static MonitoringDataViewModel Build(MonitoringData s)
      {
         MonitoringDataViewModel objDest = null;
         try
         {
            if (s != null)
            {
               DateTime receivingTimeUtc ;
               DateTime.TryParse(s.ReceivingTimeUTC, out receivingTimeUtc);
               objDest = new MonitoringDataViewModel
               {

                  Module = s.Module,
                  Name = s.Name,
                  Hostname = s.Hostname,
                  SubModules = s.SubModules,
                  CurrentUser = s.CurrentUser,
                  Type = s.Type,
                  Anomalies = s.Anomalies,
                  Components = s.Components,
                  ReceivingTimeUTC =  receivingTimeUtc
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<MonitoringDataViewModel> BuildList(IEnumerable<MonitoringData> source)
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
