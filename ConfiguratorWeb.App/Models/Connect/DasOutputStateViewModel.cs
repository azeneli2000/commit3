using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.UMSLegacy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class DasOutputStateViewModel
   {
      public DasOutputStateViewModel()
      {
         //defaults
         this.IsNew = true;
         this.LocationId = 0;
         this.BedId = 0;
         this.PatientId = 0;
         this.IsSystem = true;
         this.SamplingSeconds = 60;
         this.StartDateUtc = new DateTime(1753, 1, 1);
         this.StopDateUtc = new DateTime(1753, 1, 1);
         this.Type = UMSFrameworkParser.GetDefaultOutputType();
      }

      public bool IsNew { get; set; }

      public int LocationId { get; set; }
      [TranslatedDisplay("Location")]
      public string LocationDescription { get; set; }

      public int BedId { get; set; }
      [TranslatedDisplay("Bed")]
      public string BedDescription { get; set; }

      public int PatientId { get; set; }
      public string PatientDescription { get; set; }

      public short Type { get; set; }
      [TranslatedDisplay("Type")]
      public string TypeDescription { get; set; }

      public bool IsSystem { get; set; }

      [TranslatedDisplay("Sampling")]
      public int SamplingSeconds { get; set; }

      [TranslatedDisplay("Start")]
      public DateTime? StartDateUtc { get; set; }

      [TranslatedDisplay("Stop")]
      public DateTime? StopDateUtc { get; set; }
    
   }
}