using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums.Vitals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
    public class StandardDatasetViewModel
    {
        [TranslatedDisplay("ID")]
        public Guid sd_ID { get; set; }

        [TranslatedDisplay("Name")]
        [Required]
        public string sd_Name { get; set; }

        [TranslatedDisplay("HL7 identifier for score")]
        public string sd_HL7_Score { get; set; }

        [TranslatedDisplay("Timing")]
        public Timing sd_Timing { get; set; }

        [TranslatedDisplay("Default Interval (minutes)")]
        public int? sd_DefaultInterval { get; set; }

        [TranslatedDisplay("Reminder")]
        public bool sd_Reminder { get; set; }

        [TranslatedDisplay("Enabled by Default")]
        public bool sd_EnabledByDefault { get; set; }

        [TranslatedDisplay("Deletion Date")]
        public DateTime? sd_DateTimeDeletedUTC { get; set; }

        [TranslatedDisplay("Type")]
        public DatasetType sd_Type { get; set; }

        [TranslatedDisplay("Paged")]
        public bool sd_IsPaged { get; set; }

        [TranslatedDisplay("Script")]
        public string sd_Script { get; set; }

        [TranslatedDisplay("Score description script")]
        public string sd_ScoreDescriptionScript { get; set; }

        [TranslatedDisplay("Interval Script")]
        public string sd_IntervalScript { get; set; }

        [TranslatedDisplay("OCR")]
        public bool sd_Ocr { get; set; }

        [TranslatedDisplay("OCR Device")]
        public int sd_OcrDevice { get; set; }

        [TranslatedDisplay("OCR Image")]
        public bool UseOcrImage { get; set; }

        [Url, TranslatedDisplay("Help link")]
        public string sd_HelpLink { get; set; }

        [TranslatedDisplay("Published")]
        public bool sd_Published { get; set; }

        public bool sd_Global { get; set; } = true;

        public IList<int> LocationIds { get; set; }

        public string TypeName
        {
            get { return sd_Type.ToString(); }
        }

        public string TimingName
        {
            get { return sd_Timing.ToString(); }
        }
    }
}