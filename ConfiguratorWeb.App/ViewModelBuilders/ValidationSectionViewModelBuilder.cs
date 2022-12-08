using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model.Online;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class ValidationSectionViewModelBuilder
    {
        public static ValidationSectionViewModel Build(ValidationSection source)
        {
            ValidationSectionViewModel objDest = null;
            try
            {
                if (source != null)
                {
                  objDest = new ValidationSectionViewModel
                  {
                     ID = source.ID,
                     Name = source.Name,
                     Index = source.Index,
                  };
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<ValidationSectionViewModel> BuildList(IEnumerable<ValidationSection> source)
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