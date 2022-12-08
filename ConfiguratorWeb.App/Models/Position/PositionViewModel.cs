using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class PositionViewModel
   {
      public PositionViewModel()
      {
         SavedPositionCode = null;
      }
      public string PositionCode { get; set; }
      public string Description { get; set; }
      public int LinkedBedNumber { get; set; }
      public IEnumerable<BedViewModel> BedList { get; set; }
      public string SavedPositionCode { get; set; }
   }
}
