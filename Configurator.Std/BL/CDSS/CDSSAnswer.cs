using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.BL.CDSS
{
   public class CDSSAnswer
   {
      public CDSSAnswer()
      {
         success = false;
         messagges = new List<string>();
      }
      public CDSSAnswer(bool _success,IEnumerable<string> _messagges)
      {
         success = _success;
         messagges = _messagges;
      }
      public bool success;
      public IEnumerable<string> messagges;
   }
}
