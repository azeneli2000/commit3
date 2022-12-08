using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Exceptions
{
   public class ConnectException : Exception
   {
      public ConnectException()
      {
      }

      public ConnectException(string msg) : base (msg)
      {
         
      }

   }
}
