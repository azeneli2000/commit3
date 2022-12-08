using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public class QuantityDoseMixtureBuilder
   {

      
         public string Component { get; set; }
         public string Concentration { get; set; }
         public int Dose { get; set; }
         public int Volume { get; set; }
      
   }
}
