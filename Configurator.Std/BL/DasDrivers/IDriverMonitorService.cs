using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configurator.Std.BL.DasDrivers
{
   public interface IDriverMonitorService
   {
      Task<List<DasInstance>> GetDasInstances();
      Task<Dictionary<string, List<DriverStatus>>> GetDasInstanceStatus(string comupterName);
      void RestartDriver(int deviceDriverId, int processId, string dasBroker);
      void KillProcess(int deviceDriverId, int processId, string dasBroker);
   }
}