using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ConfiguratorWeb.App.Models.Therapy
{
   public class TherapyConfigModel
   {
      public string NameTherapyFilter { get; set; }
      public string DescriptionTherapyFilter { get; set; }
      public string TypeTherapyFilter { get; set; }
      public int SelectedItem { get; set; } = -1;

      public List<TherapyItemModel> FilteredActions { get; set; } = null;
   }
}
