using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class RolePermissionEntityBuilder
   {

      public static RolePermission Build(RolePermissionViewModel source)
      {
         RolePermission objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new RolePermission
               {
                  RoleID = source.RoleID,
                  Allow = source.Allow,
                  PermissionName = source.PermissionName
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<RolePermission> BuildList(IEnumerable<RolePermissionViewModel> source)
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
