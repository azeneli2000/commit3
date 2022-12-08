using System;
using System.Collections.Generic;
using System.Linq;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.Actions;
using Digistat.FrameworkStd.Model;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class SimpleChoiceGroupViewModelBuilder
   {
      public static SimpleChoiceGroupViewModel Build(KeyValuePair<string,int> source)
      {
         SimpleChoiceGroupViewModel objDest = null;
         try
         {
            //if (source != null)
            //{
               objDest = new SimpleChoiceGroupViewModel()
               {
                  Group = source.Key,
                  Childs = source.Value
               };
               
            //}
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }

      public static IEnumerable<SimpleChoiceGroupViewModel> BuildList(Dictionary<string,int> source)
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