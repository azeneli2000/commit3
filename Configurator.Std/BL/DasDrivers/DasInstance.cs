using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.BL.DasDrivers
{
   public class DasInstance
   {
      public DasInstance() {
         Name = string.Empty;
         ComputerName = string.Empty;
         Version = string.Empty;
      }


      public string Name { get; set; }
 
      public string ComputerName { get; set; }

      public string Version { get; set; }

   public override string ToString()
      {
         return Name + " " + Version;
      }

   }
}
