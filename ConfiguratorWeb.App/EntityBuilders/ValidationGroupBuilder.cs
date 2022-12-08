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
   public static class ValidationGroupBuilder
   {

      public static ValidationGroup Build(ValidationGroupViewModel source)
      {
         ValidationGroup objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ValidationGroup
               {
                  
                ID = source.ID,
                Index = source.Index,
                IsDeleted = source.IsDeleted,
                IsGlobal = source.IsGlobal,
                LastUpdate = source.LastUpdate,
                Locations = BuildLocationList(source.LocationIds,source.ID),
                Name = source.Name,
                Parameters = ValidationParameterBuilder.BuildList(source.Parameters)
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<ValidationGroup> BuildList(IEnumerable<ValidationGroupViewModel> source)
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


      private static ICollection<ValidationGroupLocation> BuildLocationList(List<int> LocationIDs,int GroupID)
      {
         ICollection<ValidationGroupLocation> objRet = new List<ValidationGroupLocation>();
         if (LocationIDs != null)
         {
            foreach(int i in LocationIDs)
            {
               ValidationGroupLocation objVGLoc = new ValidationGroupLocation()
               {
                  LocationID = i,
                  ValidationGroupID = GroupID
               };
               objRet.Add(objVGLoc);
            }
         }
         return objRet;
      }
   }
}
