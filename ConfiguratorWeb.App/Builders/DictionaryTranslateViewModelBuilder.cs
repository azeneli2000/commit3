using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Builders
{
   public static class DictionaryTranslateViewModelBuilder
   {
      public static DictionaryTranslateViewModel Build(Dictionary source)
      {
         DictionaryTranslateViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DictionaryTranslateViewModel
               {
                  DictionaryKey = source.DictionaryKey,
                  Module        = source.Module     ,   
                  Language      = source.Language   ,  
                  Description   = source.Description,
                  Value         = source.Value      ,
                  IsSystem      = source.IsSystem ??false  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<DictionaryTranslateViewModel> BuildList(IEnumerable<Dictionary> source)
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
