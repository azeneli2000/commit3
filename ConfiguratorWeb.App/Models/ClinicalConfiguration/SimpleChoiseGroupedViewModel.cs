﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   
   public partial class SimpleChoiceGroupedViewModel
   {
      [Key]
      [MaxLength(20)]
      [TranslatedDisplayAttribute("Group")]
      public string Group { get; set; } // nvarchar(20)
      
      [TranslatedDisplayAttribute("Choices")]
      public List<SimpleChoiceViewModel> Choices { get; set; } 
      
      public List<SimpleChoiceViewModel> LastSavedChoices { get; set; } 

   }

}
