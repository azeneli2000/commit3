using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public enum Status
   {
      OPENED = 0,
      CLOSED = 1
   }
   public class UserReportViewModel
   {
      public int Id { get; set; }

      //public DateTime? IssuedOnUTC { get; set; }
      public DateTime IssuedOnUTC { get; set; }
      public string Hostname { get; set; }

      public string GUID { get; set; }

      public int Version { get; set; }


      public string CurrentModule { get; set; }

      public string Description { get; set; }

      public int Snapshot { get; set; }

      public Status Status { get; set; }

      public string StatusChangeDateUTC { get; set; }

      public string StatusNote { get; set; }

      //public int? IdPatient { get; set; }
      //public  Patient Patient { get; set; } = new Patient();

      public string PatientName { get; set; }
      public string LocationName { get; set; }
      public string HU { get; set;  }

      public string StatusText { get; set; }

   }
}
