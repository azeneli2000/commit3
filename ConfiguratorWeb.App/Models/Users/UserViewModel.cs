using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models
{
   public class UserViewModel
   {
      public UserViewModel()
      {
         Personnel = new PersonnelViewModel();
         PersonnelRole = new PersonnelRoleViewModel();
         UserRole = new UserRoleViewModel();
         UserRoles = new List<UserRoleViewModel>();
         Permissions = new List<UserPermissionViewModel>();


      }
      public string Id { get; set; }

      //[Key]
      //[Column(Order = 1)]
      //[DatabaseGenerated(DatabaseGeneratedOption.None)]

      public int Version { get; set; }

      [StringLength(15)]
      public string ValidToDate { get; set; }

      public bool Current { get; set; }

      //  [MaxLength(50),MinLength(0)]
      public string ExternalKey { get; set; }

      [TranslatedDisplayAttribute("First Name")]
      public string FirstName { get; set; }

      [StringLength(50)]
      [TranslatedDisplayAttribute("Last Name")]
      public string LastName { get; set; }

      [StringLength(10)]
      [TranslatedDisplayAttribute("Middle Initial(s)")]
      public string MiddleInitial { get; set; }

      [StringLength(12)]
      [TranslatedDisplayAttribute("Short Name")]
      public string ShortName { get; set; }

      [Required]
      [TranslatedDisplayAttribute("Abbrev.")]
      [StringLength(3)]
      public string Abbrev { get; set; }

      [StringLength(10)]
      [TranslatedDisplayAttribute("Title (Dr., Prof.)")]
      public string Title { get; set; }

      [StringLength(20)]
      [TranslatedDisplayAttribute("Registration Number")]
      public string RegistrationNumber { get; set; }

      [StringLength(50)]
      [TranslatedDisplayAttribute("Electronic Code")]
      public string ECode { get; set; }

      [StringLength(25)]
      [TranslatedDisplayAttribute("Role")]
      public string Role { get; set; }

      public bool? Formal { get; set; }

      
      [EmailAddress(ErrorMessage = "The address is not valid ")]
      [TranslatedDisplayAttribute("E-mail")]
      public string EMail { get; set; }

      [StringLength(20)]
      [TranslatedDisplayAttribute("Telephone")]
      public string Telephone { get; set; }

      [StringLength(50)]
      [TranslatedDisplayAttribute("Address")]
      public string Address { get; set; }

      [StringLength(50)]
      [Required]
      [TranslatedDisplayAttribute("Username")]
      public string UserName { get; set; }

      [StringLength(250)]
      [TranslatedDisplayAttribute("Password")]
      public string Password { get; set; }

      [StringLength(250)]
      [TranslatedDisplayAttribute("Confirm Password")]
      [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
      public string ConfirmPassword { get; set; }

      [TranslatedDisplayAttribute("User must change password at next login")]
      public bool MustChangePassword { get; set; }

      [TranslatedDisplayAttribute("User cannot change password")]
      public bool CannotChangePassword { get; set; }

      [TranslatedDisplayAttribute("Password never expires")]
      public bool PasswordNeverExpires { get; set; }

      [TranslatedDisplayAttribute("Account never expires")]
      public bool AccountNeverExpires { get; set; }

      [StringLength(15)]
      [TranslatedDisplayAttribute("Last Login Date")]
      public string LastLoginDate { get; set; }

      [TranslatedDisplayAttribute("Login Counter")]
      public int? LoginCounter { get; set; }

      [StringLength(250)]
      [TranslatedDisplayAttribute("Notes")]
      public string Notes { get; set; }

      [StringLength(15)]
      [TranslatedDisplayAttribute("Last Password Change")]
      public string LastPasswordChange { get; set; }

      [StringLength(6)]
      public string TotLoginTime { get; set; }

      [TranslatedDisplayAttribute("Account is disabled")]
      public bool AccountDisabled { get; set; }

      [StringLength(250)]
      [TranslatedDisplayAttribute("Authentication Server")]
      public string AuthenticationServer { get; set; }

      [StringLength(36)]
      public string PersonnelGUID { get; set; }


      public UserRoleViewModel UserRole { get; set; }

      //[Required]
      [TranslatedDisplayAttribute("Permission Level")]
      public short? PermissionLevel { get; set; } // the permission level will be calculated as 

      [StringLength(50)]
      [TranslatedDisplayAttribute("Permission Modifier")]
      public string PermissionModifier { get; set; }

      public string MyLocationGUIDs { get; set; }

      public string MyPatientGUIDs { get; set; }

      public string MyBedGUIDs { get; set; }

      public string HospitalUnitGUIDAccessibles { get; set; }

      [StringLength(50)]
      [TranslatedDisplayAttribute("Language")]
      public string Language { get; set; }

      public string Pin { get; set; }


      public PersonnelViewModel Personnel { get; set; }
      public PersonnelRoleViewModel PersonnelRole { get; set; }

      public List<UserRoleViewModel> UserRoles { get; set; }
      public string Name { get { return string.Format("{0} {1}", FirstName, LastName); } }

      public List<UserPermissionViewModel> Permissions { get; set; }

      public string AllowedPermissions { get; set; }

      public string DeniedPermissions { get; set; }

      public string RolesString { get; set; }

      [StringLength(250)]
      public string BadgeID { get; set; }

      public string BadgePIN { get; set; }
      
   }
}
