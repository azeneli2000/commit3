using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IPermissionsManager : Digistat.Dal.Interfaces.IDalManagerBase<Permission>
   {
      void Delete(int entityId);
      Permission Get(int id);
   }
}