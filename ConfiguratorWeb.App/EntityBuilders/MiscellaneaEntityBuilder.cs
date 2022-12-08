using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class MiscellaneaEntityBuilder
   {
      public static Miscellanea Build(MiscellaneaViewModel source)
      {
         Miscellanea objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new Miscellanea
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
      public static IEnumerable<Miscellanea> BuildList(IEnumerable<MiscellaneaViewModel> source)
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
 