using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model.Online;
using NPOI.SS.Formula;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class ValidationGroupViewModelBuilder
    {
        public static ValidationGroupViewModel Build(ValidationGroup source)
        {
         ValidationGroupViewModel objDest = null;
            try
            {
                if (source != null)
                {
                  objDest = new ValidationGroupViewModel
                  {
                     ID = source.ID,
                     IsDeleted = source.IsDeleted,
                     IsGlobal = source.IsGlobal,
                     Name = source.Name,
                     Index = source.Index,
                     LastUpdate = source.LastUpdate,
                     Parameters = ValidationParameterViewModelBuilder.BuildList(source.Parameters),
                     LocationIds = (source.Locations != null) ? source.Locations.Select(p => p.LocationID).ToList() : new List<int>(),
                  };
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<ValidationGroupViewModel> BuildList(IEnumerable<ValidationGroup> source)
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

        static string getUserById(string guid)
        {
            
            return "";
        }
    }
}