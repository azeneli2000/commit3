using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class UserEntityBuilder
   {

      public static User Build(UserViewModel source)
      {
         User objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new User
               {
                  Abbrev = source.Abbrev,
                  AccountDisabled = source.AccountDisabled,
                  AccountNeverExpires = source.AccountNeverExpires,
                  Address = source.Address,
                  AuthenticationServer = source.AuthenticationServer,
                  CannotChangePassword = source.CannotChangePassword,
                  Current = source.Current,
                  ECode = source.ECode,
                  EMail = source.EMail,
                  ExternalKey = source.ExternalKey,
                  FirstName = source.FirstName,
                  Formal = source.Formal,
                  HospitalUnitGUIDAccessibles = source.HospitalUnitGUIDAccessibles,
                  Id = source.Id,
                  Language = source.Language,
                  LastLoginDate = source.LastLoginDate,
                  LastName = source.LastName,
                  LastPasswordChange = source.LastPasswordChange,
                  LoginCounter = source.LoginCounter,
                  MiddleInitial = source.MiddleInitial,
                  MustChangePassword = source.MustChangePassword,
                  MyBedGUIDs = source.MyBedGUIDs,
                  MyLocationGUIDs = source.MyLocationGUIDs,
                  MyPatientGUIDs = source.MyPatientGUIDs,
                  Notes = source.Notes,
                  Password = source.Password,
                  PasswordNeverExpires = source.PasswordNeverExpires,
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
                  Pin = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.EncryptString(source.Pin, null),
                  Roles = source.UserRoles != null ? UserRoleEntityBuilder.BuildList(source.UserRoles).ToList() : new List<UserRole>(),
                  UserPermissions = source.Permissions!=null?UserPermissionEntityBuilder.BuildList(source.Permissions).ToList(): new List<UserPermission>(),
                  BadgeID = source.BadgeID,
                  BadgePIN = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.EncryptString(source.BadgePIN, null)
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<User> BuildList(IEnumerable<UserViewModel> source)
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
