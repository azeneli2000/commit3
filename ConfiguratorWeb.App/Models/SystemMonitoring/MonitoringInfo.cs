using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.SystemMonitoring
{
   public class MonitoringInfo 
   {
      public MonitoringData Data { get; set; }
      public MonitoringState Status { get; set; } = MonitoringState.OK;
      public List<string> Messages { get; set; } = new List<string>();

      public int StatusValue { get { return (int)Status; } }


      public MonitoringInfo(MonitoringData objData)
      {
         Status = MonitoringState.KO;
         Messages.Add("No data available");
         Data = objData;
         if (objData != null)
         {
            if (objData.Anomalies.Count() == 0)
            {
               Status = MonitoringState.OK;
               Messages.Clear();
            }
            else
            {
               Anomaly.SeverityValue severity = 0;
              
               foreach (var anomaly in objData.Anomalies)
               {
                  if (anomaly.Severity > severity)
                  {
                     severity = anomaly.Severity;
                  }
                  Messages.Add(anomaly.Description);
               }

               switch (severity)
               {
                  case Anomaly.SeverityValue.Information:
                     Status = MonitoringState.OK;
                     break;
                  case Anomaly.SeverityValue.Warning:
                     Status = MonitoringState.Warning;
                     break;
                  case Anomaly.SeverityValue.Error:
                     Status = MonitoringState.KO;
                     break;
               }
            }
         }
      }

      public static MonitoringInfo Build(MonitoringData source)
      {
         MonitoringInfo objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new MonitoringInfo(source);
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<MonitoringInfo> BuildList(IEnumerable<MonitoringData> source)
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
