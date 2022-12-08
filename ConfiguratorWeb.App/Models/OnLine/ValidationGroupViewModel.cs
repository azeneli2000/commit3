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
   public class ValidationGroupViewModel
   {
      public ValidationGroupViewModel()
      {

         Parameters = new List<ValidationParameterViewModel>();
         LocationIds = new List<int>();

      }


      public int ID { get; set; }
      [Required]
      public string Name { get; set; }
      public DateTime LastUpdate { get; set; }

      public bool IsGlobal { get; set; }

      public bool IsDeleted { get; set; }

      public int? Index { get; set; }

      public List<int> LocationIds { get; set; }

      public ICollection<ValidationParameterViewModel> Parameters { get; set; }

      public string ValidationParameterSerialized { get; set; }

   }
}
