using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class ReportTemplateEntityBuilder
   {
      public static ReportTemplate Build(ReportTemplateViewModel source)
      {
         ReportTemplate objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ReportTemplate
               {
                  ID = source.ID.ToString(),
                  Application = source.Application,
                  Author = source.Author,
                  Current = source.Current,
                  Description = source.Description,
                  Filename = source.Filename,
                  Module = source.Module,
                  Name = source.Name,
                  Stream = source.Stream,
                  UserID =  source.UserID.ToString(),
                  UserVersion = source.UserVersion,
                  ValidToDate = source.ValidToDate,
                  Version = source.Version,
                  CreationDate =source.CreationDate
               };

               //DateTime dtCreationDate;
               //bool bolDate = DateTime.TryParse(source.CreationDate, out dtCreationDate);
               //objDest.CreationDate = dtCreationDate;
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<ReportTemplate> BuildList(IEnumerable<ReportTemplateViewModel> source)
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
