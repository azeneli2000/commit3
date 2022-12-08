using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class PermissionViewModelBuilder
    {
        public static PermissionViewModel Build(Permission source)
        {
            PermissionViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new PermissionViewModel
                    {
                        FunctionName=source.FunctionName,
                        Id=source.Id,
                        PermissionCode=source.PermissionCode,
                        PriorityLevel=source.PriorityLevel,
                        ModuleName = source.Module,
                        Description = source.Description
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<PermissionViewModel> BuildList(IEnumerable<Permission> source)
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