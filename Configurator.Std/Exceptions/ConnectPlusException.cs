using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Exceptions
{
   public class ConnectPlusException : Exception
   {
      public ConnectPlusException()
      {
      }

      public ConnectPlusException(string msg) : base (msg)
      {
         
      }

   }
}
