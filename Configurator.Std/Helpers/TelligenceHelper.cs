using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.Text;
using TelligenceXMLRPCClient.Entities;

namespace Configurator.Std.Helpers
{
   public static class TelligenceHelper
   {
      public  static bool IsMDIDevice(TelligenceDevice objDevice)
      {
        if (objDevice.tl_DeviceType != null)
        {
            StaffStationTypes objSStype = (StaffStationTypes)objDevice.tl_DeviceType;
            return (objSStype == StaffStationTypes.MDI || objSStype == StaffStationTypes.Hybrid);
        }
        else
        {
            return false;
        }
      }
   }
}
