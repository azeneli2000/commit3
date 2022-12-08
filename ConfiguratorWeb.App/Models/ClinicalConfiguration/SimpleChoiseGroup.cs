using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   
   public partial class SimpleChoiceGroupViewModel
   {
      [TranslatedDisplayAttribute("Group")]
      public string Group { get; set; } // nvarchar(20)
      
      [TranslatedDisplayAttribute("Childs")]
      public int Childs { get; set; } 
   }

}
