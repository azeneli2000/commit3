using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Model.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class StandardDatasetViewModelBuilder
    {
        public static StandardDatasetViewModel Build(StandardDataset source)
        {
            StandardDatasetViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new StandardDatasetViewModel
                    {
                        sd_DateTimeDeletedUTC = source.sd_DateTimeDeletedUTC,
                        sd_DefaultInterval = source.sd_DefaultInterval,
                        sd_EnabledByDefault = source.sd_EnabledByDefault,
                        sd_ID = source.sd_ID,
                        sd_IntervalScript = source.sd_IntervalScript,
                        sd_IsPaged = source.sd_IsPaged,
                        sd_Name = source.sd_Name,
                        sd_HL7_Score = source.sd_HL7_Score,
                        sd_Ocr = source.sd_Ocr,
                        sd_OcrDevice = source.sd_Ocr ? source.sd_OcrDevice : 0,
                        sd_Reminder = source.sd_Reminder,
                        sd_Script = source.sd_Script,
                        sd_ScoreDescriptionScript = source.sd_ScoreDescriptionScript,
                        sd_Timing = (Timing)source.sd_Timing,
                        sd_Type = (DatasetType)source.sd_Type,
                        sd_HelpLink = source.sd_HelpLink,
                        sd_Global = source.sd_Global,
                        sd_Published = source.sd_Published,
                        LocationIds = (source.Locations ?? Enumerable.Empty<StandardDatasetLocation>())
                         .Select(x => x.LocationID)
                         .ToList()
                    };
                }
            }
            catch (Exception)
            {
            }

            return objDest;
        }

        public static IEnumerable<StandardDatasetViewModel> BuildList(IEnumerable<StandardDataset> source)
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