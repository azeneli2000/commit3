using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.OnLine
{
   public class ValidationSectionViewModel
   {
      public ValidationSectionViewModel()
      {

       
      }


      public int ID { get; set; }
      [Required]
      public string Name { get; set; }

      public int Index { get; set; }
   }
}
