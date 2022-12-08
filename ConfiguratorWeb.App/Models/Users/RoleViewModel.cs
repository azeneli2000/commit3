using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
   public class RoleViewModel
   {
      public int? Id { get; set; }
      [DisplayName("Name")]
      [Required]
      public string RoleName { get; set; }
      public List<RolePermissionViewModel> Permissions { get; set; }
      public bool Selected { get; set; }
      public int UserCount { get; set; }
   }
}