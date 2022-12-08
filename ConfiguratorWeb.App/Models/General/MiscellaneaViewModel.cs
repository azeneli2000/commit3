using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class MiscellaneaViewModel
   {
      public MiscellaneaViewModel()
      {
      }

      public int Id { get; set; }
      [Required]
      public string Key { get; set; }

      public string Value { get; set; }

   }
}
