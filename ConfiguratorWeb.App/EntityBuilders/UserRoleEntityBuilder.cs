using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class UserRoleEntityBuilder
   {

      public static UserRole Build(UserRoleViewModel source)
      {
         UserRole objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new UserRole
               {
                  
                  RoleID = source.RoleID,
                  UserID = source.UserID
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<UserRole> BuildList(IEnumerable<UserRoleViewModel> source)
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
