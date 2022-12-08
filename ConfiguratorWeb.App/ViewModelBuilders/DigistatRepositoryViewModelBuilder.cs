using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DigistatRepositoryViewModelBuilder
   {
      public static DigistatRepositoryViewModel Build(DigistatRepository source)
      {
         DigistatRepositoryViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DigistatRepositoryViewModel
               {
                 Application = source.Application,
                 ID = source.ID,
                 FileName = source.FileName,
                 LastUpdate = source.LastUpdate,
                 Size = source.Size,
                 //Stream = source.Stream,
                 Type = (DigistatRepositoryType)source.Type
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<DigistatRepositoryViewModel> BuildList(IEnumerable<DigistatRepository> source)
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
