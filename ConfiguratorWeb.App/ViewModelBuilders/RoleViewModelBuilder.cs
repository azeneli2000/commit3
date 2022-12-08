using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class RoleViewModelBuilder
    {
        public static RoleViewModel Build(Role source)
        {
            RoleViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new RoleViewModel
                    {
                       RoleName = source.RoleName,
                       Id = source.Id,
                       UserCount = source.UsersCount,
                       Permissions = source.Permissions!=null?RolePermissionViewModelBuilder.BuildList(source.Permissions).ToList():null,
                        

                    };
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<RoleViewModel> BuildList(IEnumerable<Role> source)
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