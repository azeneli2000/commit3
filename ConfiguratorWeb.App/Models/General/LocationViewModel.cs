using System.ComponentModel.DataAnnotations;
namespace ConfiguratorWeb.App.Models
{
   public class LocationViewModel
   {
      public int ID { get; set; }
      [Required]
      public string LocationName { get; set; }
      public int? LocationIndex { get; set; }
      public string HospitalUnitGUID { get; set; }
      public string LocationCode { get; set; }
      public string HospitalUnitName { get; set; }

      public string UniteCode { get; set; }
   }
}
