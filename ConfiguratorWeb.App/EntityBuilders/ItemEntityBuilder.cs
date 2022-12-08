using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Model.Vitals;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class ItemEntityBuilder
    {
        public static StandardDatasetItem Build(SDItemViewModel source)
        {
            StandardDatasetItem objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new StandardDatasetItem
                    {
                        si_Description = source.Description,
                        si_ID = source.ID,
                        si_Index = source.Index,
                        si_Label = source.Label,
                        si_Max = source.Max,
                        si_Min = source.Min,
                        si_Name = source.Name,
                        si_par_ID = source.StdParameterID,
                        si_PlaceHolder = source.PlaceHolder,
                        si_Required = source.Required && source.ItemType != ParamType.Boolean, // A boolean is never required because it has always a value (true or false).
                        si_score_ID = source.ScoreID,
                        si_Script = source.Script,
                        si_sd_ID = source.StandardDatasetID,
                        si_Type = (int)source.ItemType,
                        si_Unit = source.Unit,
                        si_uom_ID = source.StdUnitOfMeasureID,
                        si_DefaultValueType = (byte)source.DefaultValueType,
                        si_DefaultValue = source.DefaultValue,
                        si_Conditional_li_ID = source.ConditionalSubItemId,
                        si_HL7_Identifier = source.HL7Identifier,
                        si_HL7_UnitIdentifier = source.HL7UnitIdentifier,
                        si_OcrParameter = source.OcrParameter > 0 ? source.OcrParameter : 0
                    };
                }
            }
            catch
            {
            }

            return objDest;
        }

        public static IEnumerable<StandardDatasetItem> BuildList(IEnumerable<SDItemViewModel> source)
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