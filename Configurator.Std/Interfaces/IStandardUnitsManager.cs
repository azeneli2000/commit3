using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IStandardUnitsManager : Digistat.Dal.Interfaces.IDalManagerBase<StandardUnit>
   {
      void Delete(int entityId);
      StandardUnit Get(int id);

      List<StandardUnit> GetMulti(List<int> ids);
   }
}