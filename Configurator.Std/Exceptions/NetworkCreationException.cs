using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Exceptions
{
   public class NetworkCreationException : Exception
   {
      public NetworkCreationException()
      {
      }

      public NetworkCreationException(string msg) : base(msg)
      {

      }
   }
}
