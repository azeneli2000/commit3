using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
   public class RolePermissionViewModel
   {
      [UIHint("PermissionDropDownEditor")]
      public RolePermissionViewModel PermissionModel { get; set; }

      public int RoleID { get; set; }
      [Required]
      [DisplayName("Permission Name")]
      public string PermissionName { get; set; }
      [DisplayName("Allow")]
      public bool Allow { get; set; }
   }
}