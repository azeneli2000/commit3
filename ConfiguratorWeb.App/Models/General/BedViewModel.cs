using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class BedViewModel
   {
      public BedViewModel()
      {
         Location = new LocationViewModel();
      }

      public int BedId { get; set; }

      public int? BedIndex { get; set; }

      [Required]
      [StringLength(15)]
      public string BedName { get; set; }

      public int? IdLocation { get; set; }

      public int? PatientId { get; set; }

      public string Properties { get; set; }

      public string RoomName { get; set; }

      public string BedCode { get; set; }

      public string UniteCode { get; set; }

      [Required]
      public LocationViewModel Location { get; set; }

     // public PatientViewModel Patient { get; set; }
     
      public string BedLocation { get => BedId + "_" + IdLocation; }

      public bool Selected { get; set; }
      public string Default { get; set; }
   }
}
