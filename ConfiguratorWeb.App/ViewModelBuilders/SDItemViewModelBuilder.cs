using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.DictionaryTerms;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Extensions;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.Model.Vitals;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class SDItemViewModelBuilder
    {
        public static SDItemViewModel Build(StandardDatasetItem source)
        {
            SDItemViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new SDItemViewModel
                    {
                        Description = source.si_Description,
                        ID = source.si_ID,
                        Index = source.si_Index,
                        Label = source.si_Label,
                        Max = source.si_Max,
                        Min = source.si_Min,
                        Name = source.si_Name,
                        PlaceHolder = source.si_PlaceHolder,
                        Required = source.si_Required && (ParamType)source.si_Type != ParamType.Boolean, // A boolean is never required because it has always a value (true or false).
                        ScoreID = source.si_score_ID,
                        Script = source.si_Script,
                        StandardDatasetID = source.si_sd_ID,
                        StdParameterID = source.si_par_ID,
                        StdUnitOfMeasureID = source.si_uom_ID,
                        ItemType = (ParamType)source.si_Type,
                        Unit = source.si_Unit,
                        DefaultValueType = (DatasetItemDefaultValueType)source.si_DefaultValueType,
                        DefaultValue = source.si_DefaultValue.NullIfEmpty(),
                        ConditionalSubItemId = source.si_Conditional_li_ID,
                        HL7Identifier = source.si_HL7_Identifier,
                        HL7UnitIdentifier = source.si_HL7_UnitIdentifier,
                        OcrParameter = source.si_OcrParameter
                    };

                    if (objDest.DefaultValueType == DatasetItemDefaultValueType.Static && objDest.DefaultValue != null)
                    {
                        switch (objDest.ItemType)
                        {
                            case ParamType.Numeric:
                            case ParamType.String:
                            case ParamType.StringWithPresets:
                                objDest.DefaultValueDisplay = objDest.DefaultValue;
                                break;

                            case ParamType.Boolean:
                                if (bool.TryParse(objDest.DefaultValue, out var bolValue) && bolValue)
                                {
                                    objDest.DefaultValue = bool.TrueString.ToLowerInvariant();
                                    objDest.DefaultValueDisplay = GeneralDictionaryTerms.YES;
                                }
                                else
                                {
                                    objDest.DefaultValue = bool.FalseString.ToLowerInvariant();
                                    objDest.DefaultValueDisplay = GeneralDictionaryTerms.NO;
                                }
                                break;

                            case ParamType.List:
                                if (source.StandardDatasetSubItems?.Count > 0)
                                {
                                    var strDefaultValueListLabel = source.StandardDatasetSubItems
                                        .OrderBy(x => x.li_Index)
                                        .FirstOrDefault(x => x.li_Code == objDest.DefaultValue)?.li_Label;
                                    objDest.DefaultValueDisplay = strDefaultValueListLabel != null ? $"{objDest.DefaultValue} - {strDefaultValueListLabel}" : objDest.DefaultValue;
                                }
                                break;

                            case ParamType.NumericList:
                                objDest.DefaultValueDisplay = objDest.DefaultValue;
                                break;

                            case ParamType.DateTime:
                                objDest.DefaultValueDisplay = DateTimeHelper.DateFromUnixTimeStamp(objDest.DefaultValue)?.ToString();
                                break;

                            case ParamType.Date:
                                objDest.DefaultValueDisplay = DateTimeHelper.DateFromUnixTimeStamp(objDest.DefaultValue)?.ToShortDateString();
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch
            {
            }

            return objDest;
        }

        public static IEnumerable<SDItemViewModel> BuildList(IEnumerable<StandardDatasetItem> source)
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