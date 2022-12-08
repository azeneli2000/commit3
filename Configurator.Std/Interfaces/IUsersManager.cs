using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IUsersManager : Digistat.Dal.Interfaces.IDalManagerBase<User>
   {
      /// <summary>
      /// Check if <c>abbreviation</c> already exist
      /// <p>If <b>userId</b> is not null && is not "", the query exclude from result the userId</p>
      /// </summary>
      /// <param name="abbreviation"></param>
      /// <param name="userId"></param>
      /// <returns></returns>
      bool CheckAbbreviationExists(string abbreviation,string userId="");  
      bool CheckIfUserCanBeDeleted(string userId);  
      string GenerateAbbreviation(string firstName, string secondName);
      User Get(string id);
      User GetByUsername(string username);
      void Remove(string userId);
      void WriteExternalPassword(string username, string password);
      void WriteLastLoginAndNetworkLogin(string userId, string lastLoginDate, string hostname);

      List<Permission> GetAllowablePermissionsForUser(string userID);

      List<Permission> GetAllowablePermissionForRoles(int[] roles);

      List<Permission> GetDeniablePermissionForRoles(int[] roles);

      List<Permission> GetDeniablePermissionForUser(string userID);

      /// <summary>
      /// Returns a list of users for a RoleID
      /// </summary>
      /// <param name="roleID">RoleID</param>
      /// <param name="excludeDisabled">If true, returns only not-disabled users</param>
      /// <returns></returns>
      List<User> GetUsersByRoleID(int roleID,bool excludeDisabled);

      string CreateUserPin();
   }
}