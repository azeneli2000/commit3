using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
   public class Schedule
   {
      public int PlanningType { get; set; } = -1;
      public int? HowManyTimes { get; set; }
      public int? SchedPolicy { get; set; }
      public string StartingFrom { get; set; }
      public string Hours { get; set; }
      public string[] aHours { get; set; }
      public string Days { get; set; }
      public string[] aDays { get; set; }
      public string EveryTimesValue { get; set; }
      public int? EveryTimesUnit { get; set; }
      public string InTimesValue { get; set; }
      public int? InTimesUnit { get; set; }
      public string CustomOrders { get; set; }
      public string Condition { get; set; }
      public DateTime? Tolerance { get; set; } = null;
      public string strTolerance { get; set; }
      public bool Repeat { get; set; }
      public int TimeChangeType { get; set; }
   }
}
