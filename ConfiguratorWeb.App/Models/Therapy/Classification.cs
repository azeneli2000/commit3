using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
   public class Classification
   {
      public enum Class
      {
         Electrolytes,
         NotSpecified = 100
      }
      public enum SearchLevelTherapy
      {

         SearchLevelAll = 0,
         [Display(Name = "Level 1")]
         SearchLevelOnlyAction = 1,
         [Display(Name = "Level 2")]
         SearchLevelOnlyMixture = 2,
         [Display(Name = "Level 3")]
         SearchLevelOnlyProtocol = 3,
         [Display(Name = "Level 4")]
         SearchLevelActionAndMixture = 4,
         [Display(Name = "Level 5")]
         SearchLevelActionAndProtocol = 5,
         [Display(Name = "Level 6")]
         SearchLevelMixtureAndProtocol = 6,
      } 
   
   public string Category { get; set; }
      public int? ClassCat { get; set; }
      public int? LocationRef { get; set; }
      public bool Incomplete { get; set; }
      public bool Obsolete { get; set; }
      //public bool CanBeMixtureComponent { get; set; }
      public string LocationName { get; set; }

      public int? SearchLevel { get; set; }
      public SearchLevelTherapy EnumSearchLevel { get; set; }

      public List<StringsPair> Prescription { get; set; } = new List<StringsPair>();
      public List<StringsPair> QuickAction { get; set; } = new List<StringsPair>();
   }
}
