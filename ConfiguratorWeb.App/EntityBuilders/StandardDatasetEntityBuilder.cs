using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Model.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class StandardDatasetEntityBuilder
   {
      public static StandardDataset Build(StandardDatasetViewModel source)
      {
         StandardDataset objDest = null;
         try
         {
            if (source != null)
            {
               var objLocations = source.LocationIds?.Count > 0
                   ? source.LocationIds.Select(x => new StandardDatasetLocation() { LocationID = x, StandardDatasetID = source.sd_ID })
                   : Enumerable.Empty<StandardDatasetLocation>();

               objDest = new StandardDataset
               {
                  sd_DateTimeDeletedUTC = source.sd_DateTimeDeletedUTC,
                  sd_DefaultInterval = source.sd_Timing == Timing.Fixed
                       ? Math.Max(source.sd_DefaultInterval ?? 0, 10)
                       : null,
                  sd_EnabledByDefault = source.sd_EnabledByDefault,
                  sd_ID = source.sd_ID,
                  sd_IntervalScript = source.sd_Timing == Timing.Variable ? source.sd_IntervalScript : null,
                  sd_IsPaged = source.sd_IsPaged,
                  sd_Name = source.sd_Name,
                  sd_HL7_Score = source.sd_HL7_Score,
                  sd_Ocr = source.sd_Ocr,
                  sd_OcrDevice = source.sd_Ocr ? (int)source.sd_OcrDevice : 0,
                  sd_Reminder = source.sd_Reminder,
                  sd_Script = source.sd_Script,
                  sd_ScoreDescriptionScript = source.sd_ScoreDescriptionScript,
                  sd_Timing = (int)source.sd_Timing,
                  sd_Type = (int)source.sd_Type,
                  sd_HelpLink = source.sd_HelpLink,
                  sd_Global = source.sd_EnabledByDefault ? source.sd_Global : true, // default to true
                  Locations = source.sd_EnabledByDefault ? objLocations.ToList() : new List<StandardDatasetLocation>()
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<StandardDataset> BuildList(IEnumerable<StandardDatasetViewModel> source)
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