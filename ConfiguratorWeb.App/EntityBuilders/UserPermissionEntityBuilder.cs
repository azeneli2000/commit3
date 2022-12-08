using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class UserPermissionEntityBuilder
   {

      public static UserPermission Build(UserPermissionViewModel source)
      {
         UserPermission objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new UserPermission
               {
                  
                PermissionName = source.PermissionName,
                Allow = source.Allow
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<UserPermission> BuildList(IEnumerable<UserPermissionViewModel> source)
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
