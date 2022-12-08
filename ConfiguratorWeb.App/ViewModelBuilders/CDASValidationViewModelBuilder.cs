using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class CDASValidationViewModelBuilder
   {
      public static CDASValidationViewModel Build(Digistat.FrameworkStd.UMSLegacy.CDASConfigurationValidation source)
      {
         CDASValidationViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new CDASValidationViewModel
               {
                 //Content = source.Content,
                 ID = source.ID,
                 Reason = source.Reason,
                 Signature = source.Reason,
                 TimeStamp = source.TimeStamp,
                 UserID = source.UserID
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<CDASValidationViewModel> BuildList(List<Digistat.FrameworkStd.UMSLegacy.CDASConfigurationValidation> source)
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
