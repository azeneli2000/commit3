using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IStandardDeviceTypesManager :  Digistat.Dal.Interfaces.IDalManagerBase<StandardDeviceType>
   {
      void Delete(int entityId);
      StandardDeviceType Get(int id);
   }
}