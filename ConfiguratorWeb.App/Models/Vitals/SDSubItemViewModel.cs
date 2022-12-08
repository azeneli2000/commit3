using ConfiguratorWeb.App.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
    public class SDSubItemViewModel
    {
        public Guid ID { get; set; }

        [Required]
        public Guid ItemID { get; set; }

        [Required, MaxLength(10), TranslatedDisplay("Code")]
        public string Code { get; set; }

        [Required, MaxLength(250), TranslatedDisplay("Label")]
        public string Label { get; set; }

        [TranslatedDisplay("Index")]
        public int? SubItemIndex { get; set; }

        [Required, MaxLength(3), RegularExpression("(\\d\\.\\d)|(\\d{0,3})", ErrorMessage = "Only numbers or point are allowed."), TranslatedDisplay("Value")]
        public string SubItemValue { get; set; }

        [MaxLength(10), TranslatedDisplay("Color Code"), RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$", ErrorMessage = "{0} is invalid.")]
        public string ColorCode { get; set; }
    }
}