using Digistat.FrameworkStd.Model;
using System.Collections.Generic;

namespace Configurator.Std.BL
{
   public interface IActualDevicesManager : Digistat.Dal.Interfaces.IDalManagerBase<ActualDevice>
   {
      void Delete(int entityId);
      ActualDevice Get(int id);
      IEnumerable<ActualDevice> GetByDeviceType(int deviceType);
      IEnumerable<ActualDevice> GetByDeviceTypeDeviceName(int deviceType, string deviceName);

   }
}