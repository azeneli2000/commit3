using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums.Vitals;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
    public class SDItemViewModel
    {
        [TranslatedDisplay("ID")]
        public Guid ID { get; set; }

        [TranslatedDisplay("Standard Dataset ID")]
        public Guid StandardDatasetID { get; set; }

        [TranslatedDisplay("Name")]
        [Required]
        public string Name { get; set; }

        [TranslatedDisplay("Label")]
        [Required]
        public string Label { get; set; }

        [TranslatedDisplay("Unit")]
        public string Unit { get; set; }

        [TranslatedDisplay("Type")]
        public ParamType ItemType { get; set; }

        [TranslatedDisplay("Description")]
        public string Description { get; set; }

        [TranslatedDisplay("Index")]
        public int? Index { get; set; }

        [TranslatedDisplay("Placeholder"), RegularExpression("^@[a-zA-Z0-9]+$", ErrorMessage = "The {0} field is invalid.")]
        public string PlaceHolder { get; set; }

        [TranslatedDisplay("Required")]
        public bool Required { get; set; }

        [TranslatedDisplay("Minimum Value")]
        public string Min { get; set; }

        [TranslatedDisplay("Maximum Value")]
        public string Max { get; set; }

        [TranslatedDisplay("Parameter ID")]
        public int? StdParameterID { get; set; }

        [TranslatedDisplay("Unit of Measure ID")]
        public int? StdUnitOfMeasureID { get; set; }

        [TranslatedDisplay("Score ID")]
        public Guid? ScoreID { get; set; }

        [TranslatedDisplay("Script")]
        public string Script { get; set; }

        [TranslatedDisplay("Default value type")]
        public DatasetItemDefaultValueType DefaultValueType { get; set; }

        [TranslatedDisplay("Default value")]
        public string DefaultValue { get; set; }

        public string DefaultValueDisplay { get; set; }

        public Guid? ConditionalItemId { get; set; }

        public Guid? ConditionalSubItemId { get; set; }

        [TranslatedDisplay("HL7 message id")]
        public string HL7Identifier { get; set; }

        [TranslatedDisplay("HL7 message unit id")]
        public string HL7UnitIdentifier { get; set; }

        [TranslatedDisplay("OCR Parameter")]
        public int OcrParameter { get; set; }

        public string ItemTypeName
        {
            get { return ItemType.ToString(); }
        }
    }
}