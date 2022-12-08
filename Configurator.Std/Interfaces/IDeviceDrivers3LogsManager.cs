using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IDeviceDrivers3LogsManager : Digistat.Dal.Interfaces.IDalManagerBase<DeviceDriver3Log>
   {
      IEnumerable<DeviceDriver3Log> GetByDeviceDriverId(int deviceDriverId);
   }
}