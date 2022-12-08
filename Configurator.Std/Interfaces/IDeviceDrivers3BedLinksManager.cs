using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IDeviceDrivers3BedLinksManager : Digistat.Dal.Interfaces.IDalManagerBase<DeviceDriver3BedLink>
   {
      DeviceDriver3BedLink Get(int idDeviceDriver, int idBed, bool loadDeviceDriver = false, bool loadBed = false);
      void Delete(int idDeviceDriver);
      IEnumerable<DeviceDriver3BedLink> GetByBedId(int bedId, bool loadDeviceDriver = false);
      IEnumerable<DeviceDriver3BedLink> GetByDeviceDriverId(int deviceDriverId, bool loadBed = false);
      IEnumerable<DeviceDriver3BedLink> GetByDeviceDriverIds(IEnumerable<int> deviceDriverIds, bool loadBed = false);
   }
}