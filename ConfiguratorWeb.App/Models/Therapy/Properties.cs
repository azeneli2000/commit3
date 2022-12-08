using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
   public class Properties
   {
      public int PreparationType { get; set; }
      public List<BoolStringPair> PreparationType2 = new List<BoolStringPair>();
      public int ValidationType { get; set; }
      public int SecondSignatureType { get; set; }
      public int SecondSignatureExecType { get; set; }
      public int FluidBalanceType { get; set; }
      public string PrescNotes { get; set; }
      public string HelpKeyPresc { get; set; }
      public string HelpKeyExec { get; set; }
      public string Barcodes { get; set; }
      public string Notes { get; set; }
   }


}
