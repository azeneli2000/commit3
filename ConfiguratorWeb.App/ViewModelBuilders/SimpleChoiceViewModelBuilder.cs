using System;
using System.Collections.Generic;
using System.Linq;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.Actions;
using Digistat.FrameworkStd.Model;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class SimpleChoiceViewModelBuilder
   {
      public static SimpleChoiceViewModel Build(SimpleChoice source)
      {
         SimpleChoiceViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new SimpleChoiceViewModel()
               {
                  ID = source.ID,
                  Group = source.Group,
                  Choice = source.Choice,
                  Index = source.Index
               };
               
            }
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }

      public static IEnumerable<SimpleChoiceViewModel> BuildList(IEnumerable<SimpleChoice> source)
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