using Digistat.FrameworkStd.Model.Vitals;
using System;

namespace Configurator.Std.Helpers
{
    internal static class VitalsHelper
    {
        public static void GuardGuid(Guid guid, string errorMessage)
        {
            if (guid == Guid.Empty)
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        public static StandardDataset Clone(StandardDataset source)
        {
            var objDatasetDestination = new StandardDataset
            {
                sd_Published = false
            };

            Copy(source, objDatasetDestination);

            return objDatasetDestination;
        }

        public static void Copy(StandardDataset source, StandardDataset destination)
        {
            destination.sd_ID = source.sd_ID;
            destination.sd_Name = source.sd_Name;
            destination.sd_HL7_Score = source.sd_HL7_Score;
            destination.sd_Timing = source.sd_Timing;
            destination.sd_DefaultInterval = source.sd_DefaultInterval;
            destination.sd_Reminder = source.sd_Reminder;
            destination.sd_EnabledByDefault = source.sd_EnabledByDefault;
            destination.sd_Type = source.sd_Type;
            destination.sd_IsPaged = source.sd_IsPaged;
            destination.sd_Script = source.sd_Script;
            destination.sd_ScoreDescriptionScript = source.sd_ScoreDescriptionScript;
            destination.sd_IntervalScript = source.sd_IntervalScript;
            destination.sd_Ocr = source.sd_Ocr;
            destination.sd_OcrDevice = source.sd_OcrDevice;
            destination.sd_Global = source.sd_Global;
            destination.sd_HelpLink = source.sd_HelpLink;
        }

        public static StandardDatasetItem Clone(StandardDatasetItem source)
        {
            var objItemDestination = new StandardDatasetItem();

            Copy(source, objItemDestination);

            return objItemDestination;
        }

        public static void Copy(StandardDatasetItem source, StandardDatasetItem destination)
        {
            destination.si_ID = source.si_ID;
            destination.si_sd_ID = source.si_sd_ID;
            destination.si_Name = source.si_Name;
            destination.si_Label = source.si_Label;
            destination.si_Unit = source.si_Unit;
            destination.si_Type = source.si_Type;
            destination.si_Description = source.si_Description;
            destination.si_Index = source.si_Index;
            destination.si_PlaceHolder = source.si_PlaceHolder;
            destination.si_Required = source.si_Required;
            destination.si_Min = source.si_Min;
            destination.si_Max = source.si_Max;
            destination.si_par_ID = source.si_par_ID;
            destination.si_uom_ID = source.si_uom_ID;
            destination.si_score_ID = source.si_score_ID;
            destination.si_Script = source.si_Script;
            destination.si_DefaultValueType = source.si_DefaultValueType;
            destination.si_DefaultValue = source.si_DefaultValue;
            destination.si_Conditional_li_ID = source.si_Conditional_li_ID;
            destination.si_HL7_Identifier = source.si_HL7_Identifier;
            destination.si_HL7_UnitIdentifier = source.si_HL7_UnitIdentifier;
            destination.si_OcrParameter = source.si_OcrParameter;
        }

        public static StandardDatasetSubItems Clone(StandardDatasetSubItems source)
        {
            var objSubItemDestination = new StandardDatasetSubItems();

            Copy(source, objSubItemDestination);

            return objSubItemDestination;
        }

        public static void Copy(StandardDatasetSubItems source, StandardDatasetSubItems destination)
        {
            destination.li_ID = source.li_ID;
            destination.li_si_ID = source.li_si_ID;
            destination.li_Label = source.li_Label;
            destination.li_Code = source.li_Code;
            destination.li_Index = source.li_Index;
            destination.li_Value = source.li_Value;
            destination.li_ColorCode = source.li_ColorCode;
        }

        public static StandardDatasetScoreDescription Clone(StandardDatasetScoreDescription source)
        {
            var objScoreDescriptionDestination = new StandardDatasetScoreDescription();

            Copy(source, objScoreDescriptionDestination);

            return objScoreDescriptionDestination;
        }

        public static void Copy(StandardDatasetScoreDescription source, StandardDatasetScoreDescription destination)
        {
            destination.dsr_ID = source.dsr_ID;
            destination.dsr_sd_ID = source.dsr_sd_ID;
            destination.dsr_MinValue = source.dsr_MinValue;
            destination.dsr_MaxValue = source.dsr_MaxValue;
            destination.dsr_Description = source.dsr_Description;
            destination.dsr_ColorCode = source.dsr_ColorCode;
        }
    }
}