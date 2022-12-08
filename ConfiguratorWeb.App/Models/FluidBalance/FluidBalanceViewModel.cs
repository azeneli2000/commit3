using Digistat.FrameworkStd.Model.FluidBalance;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Digistat.FrameworkStd.Enums.EnumModeType;

namespace ConfiguratorWeb.App.Models.FluidBalance
{
   public class FluidBalanceViewModel
   {

      [Required]
      public int Id { get; set; }

      [Required]
      [StringLength(50)]
      public string Name { get; set; }
      
      public string Description { get; set; }

      //[Required(AllowEmptyStrings = true)]
      //[StringLength(255)]
      public string Labels { get; set; }

      public ModeType Mode { get; set; }
      public string Sql { get; set; }
      public bool Permanent { get; set; }
      public bool Once { get; set; }

      public string ModeText{ get; set; }

      public int? IdLocation { get; set; }
      public virtual LocationViewModel Location { get; set; } = new LocationViewModel();
   }
}
