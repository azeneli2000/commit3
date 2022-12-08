using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class UserPermissionViewModelBuilder
    {
        public static UserPermissionViewModel Build(UserPermission source)
        {
            UserPermissionViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new UserPermissionViewModel
                    {
                        PermissionName = source.PermissionName,
                        Allow = source.Allow

                    };
                }
            }
            catch (Exception)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<UserPermissionViewModel> BuildList(IEnumerable<UserPermission> source)
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