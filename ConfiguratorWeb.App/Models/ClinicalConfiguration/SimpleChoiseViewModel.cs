using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   
   public partial class SimpleChoiceViewModel
   {
      [Key]
      //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      [TranslatedDisplay("Id")]
      public int ID { get; set; } // int
      [MaxLength(20)]
      [TranslatedDisplayAttribute("Group")]
      public string Group { get; set; } // nvarchar(20)
      [MaxLength(250)]
      [TranslatedDisplayAttribute("Choice")]
      public string Choice { get; set; } // nvarchar(250)
      [TranslatedDisplayAttribute("Index")]
      public int? Index { get; set; } // int
   }

}
