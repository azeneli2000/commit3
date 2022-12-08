using ConfiguratorWeb.App.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
   public class StdScoreDescriptionViewModel
   {
      public Guid ID { get; set; }

      public Guid DatasetID { get; set; }

      [Required, TranslatedDisplay("Minimum")]
      public double MinValue { get; set; }

      [Required, TranslatedDisplay("Maximum")]
      public double MaxValue { get; set; }

      [Required, TranslatedDisplay("Description")]
      public string Description { get; set; }

      [MaxLength(10), TranslatedDisplay("Color Code"), RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$", ErrorMessage = "{0} is invalid.")]
      public string ColorCode { get; set; }
   }
}