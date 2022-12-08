using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class UserRoleViewModelBuilder
    {
        public static UserRoleViewModel Build(UserRole source)
        {
            UserRoleViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new UserRoleViewModel
                    {
                       RoleID = source.RoleID,
                       UserID = source.UserID,
                       RoleRef = RoleViewModelBuilder.Build(source.RoleRef)

                    };
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<UserRoleViewModel> BuildList(IEnumerable<UserRole> source)
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