using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Online;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class ValidationSectionBuilder
   {

      public static ValidationSection Build(ValidationSectionViewModel source)
      {
         ValidationSection objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ValidationSection
               {
                  
                ID = source.ID,
                Index = source.Index,
                Name = source.Name,
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<ValidationSection> BuildList(IEnumerable<ValidationSectionViewModel> source)
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
