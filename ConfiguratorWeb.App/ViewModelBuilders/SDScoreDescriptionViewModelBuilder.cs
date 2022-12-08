using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model.Vitals;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class SDScoreDescriptionViewModelBuilder
    {
        public static StdScoreDescriptionViewModel Build(StandardDatasetScoreDescription source)
        {
            StdScoreDescriptionViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new StdScoreDescriptionViewModel
                    {
                        ID = source.dsr_ID,
                        DatasetID = source.dsr_sd_ID,
                        ColorCode = source.dsr_ColorCode,
                        Description = source.dsr_Description,
                        MaxValue = source.dsr_MaxValue,
                        MinValue = source.dsr_MinValue
                    };
                }
            }
            catch
            {
            }

            return objDest;
        }

        public static IEnumerable<StdScoreDescriptionViewModel> BuildList(IEnumerable<StandardDatasetScoreDescription> source)
        {
            try
            {
                return source.Select(Build);
            }
            catch
            {
                throw;
            }
        }
    }
}