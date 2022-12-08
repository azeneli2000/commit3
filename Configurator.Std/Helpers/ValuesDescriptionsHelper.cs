using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.Helpers
{
   public static class ValuesDescriptionsHelper
   {

      public static List<int> DataBitsValues = new List<int> { 7, 8 };

      public static string GetHandshakeDescription(System.IO.Ports.Handshake value)
      {

         Dictionary<System.IO.Ports.Handshake, string> descriptions = new Dictionary<System.IO.Ports.Handshake, string>
         {
            { System.IO.Ports.Handshake.None, "NONE" },
            { System.IO.Ports.Handshake.XOnXOff, "XON/XOFF" },
            { System.IO.Ports.Handshake.RequestToSend, "RTS" },
            { System.IO.Ports.Handshake.RequestToSendXOnXOff, "RTS+XON/XOFF" },
         };

         return string.Format("{0} - {1}", (int)value, descriptions[value]);
      }

      public static string GetParityDescription(System.IO.Ports.Parity value)
      {

         Dictionary<System.IO.Ports.Parity, string> descriptions = new Dictionary<System.IO.Ports.Parity, string>
         {
            { System.IO.Ports.Parity.None, "NONE" },
            { System.IO.Ports.Parity.Even, "EVEN" },
            { System.IO.Ports.Parity.Mark, "MARK" },
            { System.IO.Ports.Parity.Odd, "ODD" },
            { System.IO.Ports.Parity.Space, "SPACE" },
         };

         return string.Format("{0} - {1}", (int)value, descriptions[value]);
      }

      public static string GetStopBitsDescription(System.IO.Ports.StopBits value)
      {

         Dictionary<System.IO.Ports.StopBits, string> descriptions = new Dictionary<System.IO.Ports.StopBits, string>
         {
            { System.IO.Ports.StopBits.None, "" },
            { System.IO.Ports.StopBits.One, "1" },
            { System.IO.Ports.StopBits.OnePointFive, "1.5" },
            { System.IO.Ports.StopBits.Two, "2" },
         };

         return string.Format("{0}", descriptions[value]);
      }

   }
}
