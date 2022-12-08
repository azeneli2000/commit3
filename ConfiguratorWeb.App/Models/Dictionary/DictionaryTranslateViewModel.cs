using System.ComponentModel.DataAnnotations;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   public class DictionaryTranslateViewModel
   {
      //public int IsNewRecord { get; set; }

      [Required]
      public string DictionaryKey { get; set; }
      public string Module { get; set; }
      public string Language { get; set; }
      public string Description { get; set; }
      [Required]
      public string Value { get; set; }
      //[TranslatedDisplayAttribute("NIs System")]
      //[UIHint()]
      public bool IsSystem { get; set; }

   }
}
