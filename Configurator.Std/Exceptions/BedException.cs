using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Exceptions
{
   public class BedException : Exception
   {
      public BedException()
      {
      }

      public BedException(string msg) : base (msg)
      {
         
      }

   }
}
