using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class PersonnelRoleViewModelBuilder
    {
        public static PersonnelRoleViewModel Build(PersonnelRole source)
        {
            PersonnelRoleViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new PersonnelRoleViewModel
                    {
                        Code=source.Code,
                        Current=source.Current,
                        Description=source.Description,
                        ExternalKey =source.ExternalKey,
                        GUID=source.Guid,
                        Index=source.Index,
                        InPlanningByDefault=source.InPlanningByDefault,
                        IsFirstSurgeon=source.IsFirstSurgeon,
                        Name=source.Name,
                        Notes=source.Notes,
                        ValidToDate=source.ValidToDate,
                        Version=source.Version
                        
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<PersonnelRoleViewModel> BuildList(IEnumerable<PersonnelRole> source)
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