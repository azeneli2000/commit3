using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class RolePermissionViewModelBuilder
   {
      public static RolePermissionViewModel Build(RolePermission source)
      {
         RolePermissionViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new RolePermissionViewModel
               {
                  PermissionName = source.PermissionName,
                  Allow = source.Allow,
                  RoleID = source.RoleID,
                  PermissionModel = new RolePermissionViewModel { RoleID = source.RoleID, PermissionName = source.PermissionName },

               };
            }
         }
         catch (Exception)
         {

            throw;
         }
         return objDest;
      }

      public static IEnumerable<RolePermissionViewModel> BuildList(IEnumerable<RolePermission> source)
      {
         try
         {
            if (source != null)
            {
               return source.Select(Build);
            }
            else
            {
               return null;
            }
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}