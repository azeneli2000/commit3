using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.BL.DasDrivers
{
   public enum DriverUpdateMethods
   {
      DontUpdate,
      UpdateUsingCachedIndex,
      UpdateUsingCachedDriverFiles,
      UpdateUsingClass,
   }
}
