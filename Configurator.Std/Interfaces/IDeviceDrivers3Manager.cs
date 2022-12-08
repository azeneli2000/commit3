using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IDeviceDrivers3Manager  : Digistat.Dal.Interfaces.IDalManagerBase<DeviceDriver3>
   {
      void Delete(int id);
      DeviceDriver3 Get(int id);
      IQueryable<DeviceDriver3> GetDeviceDrivers();

      bool CheckCableIDAlreadyExists(DeviceDriver3 objDevice,int id);

   }
}