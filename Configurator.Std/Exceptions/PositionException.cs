using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Exceptions
{
   public class PositionException : Exception
   {
      public PositionException(string msg) : base(msg)
      {
      }
      
   }
}
