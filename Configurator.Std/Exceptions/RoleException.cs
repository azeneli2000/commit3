using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Exceptions
{
   public class RoleException : Exception
   {
      public RoleException()
      {
      }

      public RoleException(string msg) : base (msg)
      {
         
      }

   }
}
