using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class RoleEntityBuilder
   {

      public static Role Build(RoleViewModel source)
      {
         Role objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new Role
               {
                  Id = (int)(source.Id.HasValue?source.Id:0),
                  Permissions = RolePermissionEntityBuilder.BuildList(source.Permissions).ToList(),
                  RoleName = source.RoleName


               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<Role> BuildList(IEnumerable<RoleViewModel> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
