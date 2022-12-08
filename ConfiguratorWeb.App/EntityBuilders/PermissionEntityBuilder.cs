using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.EntityBuilders
{
    public static class PermissionEntityBuilder
    {
        public static Permission Build(PermissionViewModel source)
        {
            Permission objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new Permission
                    {
                        FunctionName = source.FunctionName,
                        Id= (source.Id.HasValue?source.Id.Value:0),
                        PermissionCode = source.PermissionCode,
                        PriorityLevel = source.PriorityLevel,
                        Module = source.ModuleName,
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

        public static IEnumerable<Permission> BuildList(IEnumerable<PermissionViewModel> source)
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