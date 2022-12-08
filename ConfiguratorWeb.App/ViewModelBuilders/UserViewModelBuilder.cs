using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class UserViewModelBuilder
   {
      public static UserViewModel Build(User source)
      {
         UserViewModel objDest = null;
         try
         {
            if (source != null)
            {
               // DateTime objLoginDate = DateTime.ParseExact(source.LastLoginDate, "yyyyMMdd hhmmss", System.Globalization.CultureInfo.InvariantCulture);

               objDest = new UserViewModel
               {
                  Abbrev = source.Abbrev,
                  AccountDisabled = source.AccountDisabled ?? false,
                  AccountNeverExpires = source.AccountNeverExpires ?? false,
                  Address = source.Address,
                  AuthenticationServer = source.AuthenticationServer,
                  CannotChangePassword = source.CannotChangePassword ?? false,
                  Current = source.Current ?? false,
                  ECode = source.ECode,
                  EMail = source.EMail,
                  ExternalKey = source.ExternalKey,
                  FirstName = source.FirstName,
                  Formal = source.Formal,
                  HospitalUnitGUIDAccessibles = source.HospitalUnitGUIDAccessibles,
                  Id = source.Id,
                  Language = source.Language,
                  LastLoginDate =UtilityHelper.GetDateFromString(source.LastLoginDate,"yyyyMMdd hhmmss")?.ToShortDateString(),
                  LastName = source.LastName,
                  LastPasswordChange = source.LastPasswordChange,
                  LoginCounter = source.LoginCounter,
                  MiddleInitial = source.MiddleInitial,
                  MustChangePassword = source.MustChangePassword ?? false,
                  MyBedGUIDs = source.MyBedGUIDs,
                  MyLocationGUIDs = source.MyLocationGUIDs,
                  MyPatientGUIDs = source.MyPatientGUIDs,
                  Notes = source.Notes,
                  Password = source.Password,
                  PasswordNeverExpires = source.PasswordNeverExpires ?? false,
                  PermissionLevel = source.PermissionLevel,
                  PermissionModifier = source.PermissionModifier,
                  PersonnelGUID = source.PersonnelGUID,
                  RegistrationNumber = source.RegistrationNumber,
                  Role = source.Role,
                  ShortName = source.ShortName,
                  Telephone = source.Telephone,
                  Title = source.Title,
                  UserName = source.UserName,
                  ValidToDate = source.ValidToDate,
                  Version = source.Version,
                  Pin = source.Pin!=null? Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.DecryptString(source.Pin, null):null,
                  UserRoles = source.Roles!=null? UserRoleViewModelBuilder.BuildList(source.Roles).ToList():new List<UserRoleViewModel>(),
                  Permissions = source.UserPermissions!=null?UserPermissionViewModelBuilder.BuildList(source.UserPermissions).ToList():new List<UserPermissionViewModel>(),
                  RolesString = BuildRoleString(source),
                  BadgeID = source.BadgeID,
                  BadgePIN = source.BadgePIN != null ? Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.DecryptString(source.BadgePIN, null) : null,
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }


      public static string BuildRoleString(User source)
      {
         string strRet = null;
         
         if(source!=null && source.Roles != null)
         {
            System.Text.StringBuilder objBuilder = new System.Text.StringBuilder();
            foreach(UserRole ur in source.Roles)
            {
               if(ur!=null && ur.RoleRef != null)
               {
                  objBuilder.Append(ur.RoleRef.RoleName);
                  objBuilder.Append(';');
               }
            }
         }
         return strRet;
      }

      public static IEnumerable<UserViewModel> BuildList(IEnumerable<User> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
