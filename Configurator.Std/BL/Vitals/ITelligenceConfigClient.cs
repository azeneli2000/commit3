using System;
using System.Collections.Generic;
using TelligenceXMLRPCClient.Entities;

namespace Configurator.Std.BL.Vitals
{
   public interface ITelligenceConfigClientManager
   {
      List<TLDeviceDetail> GetAvailableTLDevices();
      Dictionary<int, string> GetAvailableTLSystems();
      TelligenceConfigHandlerConfiguration GetCurrentSetting();
      void SetSettings(TelligenceConfigHandlerConfiguration objToSet);

      List<Tuple<int, string>> ImportAllTLDevices(int serverID);
   }
}