using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class ReportTemplateViewModelBuilder
   {
      public static ReportTemplateViewModel Build(ReportTemplate source)
      {
         ReportTemplateViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ReportTemplateViewModel
               {
                  ID=(string.IsNullOrWhiteSpace(source.ID)?Guid.Empty: Guid.Parse(source.ID)),
                  Application=source.Application,
                  Author=source.Author,
                  CreationDate=source.CreationDate,
                  Current=source.Current,
                  Description=source.Description,
                  Filename=source.Filename,
                  Module=source.Module,
                  Name=source.Name,
                  Stream=source.Stream,
                  UserID= (string.IsNullOrWhiteSpace(source.UserID) ? Guid.Empty : Guid.Parse(source.UserID)),
                  UserVersion=source.UserVersion,
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

      public static IEnumerable<ReportTemplateViewModel> BuildList(IEnumerable<ReportTemplate> source)
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
