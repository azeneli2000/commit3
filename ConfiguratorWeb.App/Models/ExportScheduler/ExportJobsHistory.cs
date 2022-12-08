using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfiguratorWeb.App.Models.ExportScheduler
{
   public partial class ExportJobsHistory
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public virtual int historyId { get; set; } // int

      public virtual int jobID { get; set; } // int

      [MaxLength(100)]
      public virtual string runningHostname { get; set; } // nvarchar(100)

      [MaxLength]
      public virtual string runningQueryParameterResult { get; set; } // nvarchar(max)

      [MaxLength(20)]
      public virtual string runningTriggered { get; set; } // nvarchar(20)

      public virtual DateTime? runningDateTime { get; set; } // datetime

      public virtual int? runningElapsedTime { get; set; } // int

      public virtual int? runningStatusCode { get; set; } // int

      [MaxLength]
      public virtual string runningStatusMessage { get; set; } // nvarchar(max)

      public override bool Equals(object obj)
      {
         return obj is ExportJobsHistory history &&
                historyId == history.historyId &&
                jobID == history.jobID &&
                runningHostname == history.runningHostname &&
                runningQueryParameterResult == history.runningQueryParameterResult &&
                runningTriggered == history.runningTriggered &&
                runningDateTime == history.runningDateTime &&
                runningElapsedTime == history.runningElapsedTime &&
                runningStatusCode == history.runningStatusCode &&
                runningStatusMessage == history.runningStatusMessage;
      }
   }
}
