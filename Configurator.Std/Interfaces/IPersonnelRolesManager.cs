using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IPersonnelRolesManager : Digistat.Dal.Interfaces.IDalManagerBase<PersonnelRole>
   {
      PersonnelRole Get(string guid);
   }
}