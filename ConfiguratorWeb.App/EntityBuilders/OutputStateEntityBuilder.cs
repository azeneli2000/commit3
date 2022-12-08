using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class OutputStateEntityBuilder
   {
      public static DasOutputState Build(DasOutputStateViewModel source)
      {
         DasOutputState objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DasOutputState
               {
                  LocationId = source.LocationId,
                  BedId = source.BedId,
                  PatientId = source.PatientId,
                  Type = source.Type,
                  IsSystem = source.IsSystem,
                  SamplingSeconds = source.SamplingSeconds,
                  StartDateUtc = (source.StartDateUtc.HasValue? source.StartDateUtc.Value:new DateTime(1753,1,1)),
                  StopDateUtc = (source.StopDateUtc.HasValue ? source.StopDateUtc.Value : new DateTime(1753, 1, 1))                  
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DasOutputState> BuildList(IEnumerable<DasOutputStateViewModel> source)
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
