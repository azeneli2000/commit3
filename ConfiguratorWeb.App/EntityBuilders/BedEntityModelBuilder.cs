using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class BedEntityModelBuilder
   {
      public static Bed Build(BedViewModel source)
      {
         Bed objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new Bed
               {
                  Id = source.BedId,
                  Index = source.BedIndex,
                  BedCode = source.BedCode,
                  IdLocation=source.IdLocation,
                  Name = source.BedName,
                  Properties = source.Properties,
                  RoomName = source.RoomName,
                  UniteCode = source.UniteCode,
                  IdPatient = source.PatientId.HasValue?source.PatientId:0,
                  Location = source.Location!=null? LocationEntityBuilder.Build(source.Location):null,
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<Bed> BuildList(IEnumerable<BedViewModel> source)
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
 