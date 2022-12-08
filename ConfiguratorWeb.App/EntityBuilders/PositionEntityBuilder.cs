using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class PositionEntityBuilder
   {
      public static PositionAssociation Build(PositionViewModel source)
      {
         PositionAssociation objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new PositionAssociation
               {
                  PositionCode = source.PositionCode,
                  Description = source.Description,
                  PositionBedLinks = BuildPositionBedLinks(source.PositionCode, source.BedList),

               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      
      private static ICollection<PositionBedLink> BuildPositionBedLinks(string positionCode, IEnumerable<BedViewModel> objBeds)
      {
         ICollection<PositionBedLink> objret = new List<PositionBedLink>();
         if (objBeds != null)
         {
            foreach (BedViewModel objBed in objBeds)
            {
               PositionBedLink objBedLink = new PositionBedLink();
               objBedLink.BedId = objBed.BedId;
               objBedLink.PositionCode = positionCode;
               objret.Add(objBedLink);
            }
         }
         return objret;
      }

      public static IEnumerable<PositionAssociation> BuildList(IEnumerable<PositionViewModel> source)
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
