using System;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models.Actions
{
   //[Table("CORONELLA")]
   public partial class CoronellaViewModel
   {
      public virtual long id { get; set; } // bigint, null
      [MaxLength(50)]
      public virtual string presidio { get; set; } // varchar(50), null
      public virtual int? slot { get; set; } // int, null
      [MaxLength(16)]
      public virtual string pazienteCF { get; set; } // char(16), null
      [MaxLength(1000)]
      public virtual string pazienteAltro { get; set; } // nvarchar(1000), null
      public virtual int? hr { get; set; } // int, null
      public virtual int? rr { get; set; } // int, null
      public virtual double? temp { get; set; } // float, null
      public virtual int? spo2 { get; set; } // int, null
      public virtual short? pressHi { get; set; } // smallint, null
      public virtual short? pressLow { get; set; } // smallint, null
      public virtual short? ews { get; set; } // smallint, null
      public virtual bool? com { get; set; } // bit, null
      public virtual DateTime? hrLastTime { get; set; } // datetime, null
      public virtual DateTime? rrLastTime { get; set; } // datetime, null
      public virtual DateTime? tempLastTime { get; set; } // datetime, null
      public virtual DateTime? spo2LastTime { get; set; } // datetime, null
      public virtual DateTime? pressHiLastTime { get; set; } // datetime, null
      public virtual DateTime? pressLowLastTime { get; set; } // datetime, null
      public virtual DateTime? ewsLastTime { get; set; } // datetime, null
      public virtual DateTime? comLastTime { get; set; } // datetime, null

   }
}


