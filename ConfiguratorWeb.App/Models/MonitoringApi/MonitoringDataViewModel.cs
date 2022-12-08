using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Digistat.FrameworkStd.Model.Monitoring.MonitoringData;

namespace ConfiguratorWeb.App.Models.MonitoringApi
{
   public class MonitoringDataViewModel
   {

      /// <summary>
      /// Component that has generated the statistics (DAS, CONTROLBAR, ...)
      /// </summary>
      public string Module { get; set; }

      /// <summary>
      /// Unique identifier to be monitored for presence in the periodic check
      /// (i.e. DASName@Pool for DAS, name of the service for other services, CONTROLBAR)
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Hostname that has generated the message
      /// </summary>
      public string Hostname { get; set; }

      /// <summary>
      /// Current active modules
      /// </summary>
      public string SubModules { get; set; }

      /// <summary>
      /// Current user logged in
      /// </summary>
      public string CurrentUser { get; set; }

      /// <summary>
      /// Statistics type
      /// </summary>
      public MonitoringType Type { get; set; } = MonitoringType.Client;

      /// <summary>
      /// List of indicators components
      /// </summary>
      public List<MonitoringComponent> Components {  get; set; } = new List<MonitoringComponent>();

      /// <summary>
      /// List of anomalies
      /// </summary>
      public List<Anomaly> Anomalies { set; get; } = new List<Anomaly>();

      /// <summary>
      /// Arrival time of the statistic
      /// </summary>
      public DateTime? ReceivingTimeUTC { get; set; }



   }
}
