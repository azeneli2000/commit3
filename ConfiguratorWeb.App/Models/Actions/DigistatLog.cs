using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class DigistatLog
   {
      public int Id { get; set; }
      public DateTime DateTime { get; set; }
      public string ComputerName { get; set; }
      public string Task { get; set; }
      public string User { get; set; }
      public int? PatientRef { get; set; }
      public int? LocationRef { get; set; }
      public int? BedRef { get; set; }
      public string Type { get; set; }
      public string Priority { get; set; }
      public string Area { get; set; }
      public int? Code { get; set; }
      public string Message { get; set; }
   }
}
