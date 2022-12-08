using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.BL.CDSS
{
   class SyncToken
   {
      public SyncToken()
      {
         Completed = false;
         Answer = new CDSSAnswer();
      }
      public string Token { get; set; }
      public bool Completed { get; set; }

      public CDSSAnswer Answer {get; set; }
   }
}
