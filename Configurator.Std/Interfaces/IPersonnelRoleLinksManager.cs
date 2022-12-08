using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IPersonnelRoleLinksManager : Digistat.Dal.Interfaces.IDalManagerBase<PersonnelRoleLink>
   {
      IEnumerable<PersonnelRoleLink> Find(Expression<Func<PersonnelRoleLink, bool>> filterPredicate, bool loadPersonnel = true, bool loadRole = false, int pageNumber = 0, int pageSize = 0);
      IEnumerable<PersonnelRoleLink> GetByPersonnel(string personnelGuid, bool loadRole = true, bool loadPersonnel = false);
      IEnumerable<PersonnelRoleLink> GetByRole(string personnelRoleGuid, bool loadPersonnel = true, bool loadRole = false);
      void Remove(PersonnelRoleLink entity);
   }
}