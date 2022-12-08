using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class PersonnelViewModelBuilder
    {
        public static PersonnelViewModel Build(Personnel source)
        {
            PersonnelViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new PersonnelViewModel
                    {
                        Beeper = source.Beeper,
                        CellPhone = source.CellPhone,
                        Code = source.Code,
                        Current = source.Current,
                        Description = source.Description,
                        EMailAddress = source.EmailAddress,
                        ExternalKey = source.ExternalKey,
                        FirstName = source.FirstName,
                        ID = source.Id,
                        Initial = source.Initial,
                        LastName = source.LastName,
                        Name = source.Name,
                        Notes = source.Notes,
                        Qualification = source.Qualification,
                        TelephoneNumber = source.TelephoneNumber,
                        Title = source.Title,
                        ValidToDate = source.ValidToDate,
                        Version = source.Version
                    };
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return objDest;
        }

        public static IEnumerable<PersonnelViewModel> BuildList(IEnumerable<Personnel> source)
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