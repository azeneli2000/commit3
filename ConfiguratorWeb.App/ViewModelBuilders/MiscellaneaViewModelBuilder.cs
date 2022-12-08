using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class MiscellaneaViewModelBuilder
   {
      public static MiscellaneaViewModel Build(Miscellanea source)
      {
         MiscellaneaViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new MiscellaneaViewModel
               {
                 Id = source.Id,
                 Key = source.Key,
                 Value = source.Value
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<MiscellaneaViewModel> BuildList(IEnumerable<Miscellanea> source)
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
