using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IRolesManager : Digistat.Dal.Interfaces.IDalManagerBase<Role>
   {
      Role Get(int id);

      bool Delete(int id);

      /// <summary>
      /// Returns a list of all available roles, with, for each, the list of permissions and users
      /// </summary>
      /// <returns></returns>
      List<Role> GetAll();

      /// <summary>
      /// Returns a list of all available roles, with only ID and name of each item.
      /// Other properties shall not be set
      /// </summary>
      /// <returns></returns>
      List<Role> GetAllFast();
   }
}