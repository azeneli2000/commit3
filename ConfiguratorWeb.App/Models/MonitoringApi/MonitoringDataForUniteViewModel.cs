using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Digistat.FrameworkStd.Model.Monitoring.MonitoringData;

namespace ConfiguratorWeb.App.Models.MonitoringApi
{
   public class MonitoringDataForUniteViewModel
   {

      // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
      public string SupervisorName { get; set; }
      public List<Application> Applications { get; set; }

   }

   public class Status
   {
      public DateTime StatusDateTime { get; set; }
      public int StatusCode { get; set; }
      public string Message { get; set; }
      public object Comment { get; set; }
      public object Information { get; set; }
   }

   public class Descriptor
   {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Version { get; set; }
      public string Description { get; set; }
      public int MaxHistoryLength { get; set; }
      public string PathToExecutable { get; set; }
      public string Arguments { get; set; }
      public string ApplicationPath { get; set; }
      public int RedStatusAppStatLogLevel { get; set; }
      public int YellowStatusAppStatLogLevel { get; set; }
      public int PersistentAppStatLogId { get; set; }
   }

   public class Supervisor
   {
      public Status Status { get; set; }
      public Descriptor Descriptor { get; set; }
      public bool IsFailed { get; set; }
      public bool IsStopped { get; set; }
   }

   public class Application
   {
      public string Path { get; set; }
      public string Name { get; set; }
      public Status Status { get; set; }
      public List<Supervisor> Supervisors { get; set; }
      public List<object> Children { get; set; }
   }

}
