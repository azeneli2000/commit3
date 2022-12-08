using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;

using Digistat.FrameworkStd.Model;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Model.OranJ;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Helpers;

namespace Configurator.Std.BL
{
   public class UsersManager : DalManagerBase<User>, IUsersManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IPermissionsService mobjPermSvc;

      public UsersManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService,IPermissionsService permSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjPermSvc = permSvc;
      }



      #endregion

      #region Data reading functions


      //public SystemOption Get(string guid, IEnumerable<System.Linq.Expressions.Expression<Func<User, object>>> includePredicates = null) {
      public User Get(string id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for User with id {0}", id);

         User result = null;

         try
         {
         
            //TODO Trace
            mobjLoggerService.Info("Reading User with id {0} from DB", id);
            result = mobjDbContext.Set<User>()
               .Where(x => x.Id == id && x.Current == true).SingleOrDefault();

            if (result != null)
            {
               //Load Roles
               var objRepo = mobjDbContext.Set<UserRole>().Include(p=>p.RoleRef).ThenInclude(p=>p.Permissions);
               List<UserRole> objRoles = objRepo.Where(p => p.UserID == result.Id).ToList();
               result.Roles = objRoles;

               //Load userpermissions
               var objRepoPerm = mobjDbContext.Set<UserPermission>();
               result.UserPermissions = objRepoPerm.Where(p => p.UserID == result.Id).ToList();

               //decrypt password
               result.DecryptPassword();

            }



            //TODO Trace
            mobjLoggerService.Info("User with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading User with guid {0} from DB", id);
            throw new Exception(string.Format("Error reading User with guid {0} from DB", id), e);
         }


         return result;

      }

      public User GetByUsername(string username)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for User  {0}", username);

         User result = null;

         try
         {
            //Set detached
            //context.Configuration.ProxyCreationEnabled = false;

            IQueryable<User> repository = mobjDbContext.Set<User>();

            //if (includePredicates != null && includePredicates.Count() > 0) {
            //   includePredicates.ToList().ForEach(x => repository = repository.Include(x));
            //}


            //TODO Trace
            mobjLoggerService.Info("Reading user {0} from DB", username);
            result = repository.Where(x => x.UserName == username && x.Current == true).SingleOrDefault();

            if (result != null)
            {
               result.DecryptPassword();
            }

            //TODO Trace
            mobjLoggerService.Info("User {0} retrived from DB", username);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading User {0} from DB", username);
            throw new Exception(string.Format("Error reading User  {0} from DB", username), e);
         }


         return result;

      }

      public bool CheckAbbreviationExists(string abbreviation, string userId="")
      {
         //TODO Trace
         mobjLoggerService.Info("Checking if {0} user abbreviation exists", abbreviation);
         if (string.IsNullOrWhiteSpace(userId) )
         {
            return this.Exists(x => x.Abbrev == abbreviation);
         }
         else
         {
            return this.Exists(x => x.Abbrev == abbreviation && x.Id != userId);
         }
         
      }
      public bool CheckIfUserCanBeDeleted(string userId)
      {
         //TODO Trace
         mobjLoggerService.Info("Checking if user {0} is ActualPlannedResource", userId);

         bool retVal = (mobjDbContext.Set<ActualPlannedResource>().OrderByDescending(o =>o.DateTimeDataEntryUTC).FirstOrDefault(p => p.us_ID == userId) ==null);
         return retVal;
      }

      #endregion

      #region Data Writing functions


      public new User Create(User user)
      {

         //TODO Trace
         mobjLoggerService.Info("Creating new User {0} ({1})", user.UserName, user.Abbrev);

         user.EncryptPassword();

         try
         {

            mobjDbContext.BeginTransaction();

            var userRepository = mobjDbContext.Set<User>();

            //Prevent duplications
            User loadedUser = userRepository.Where(x => x.UserName == user.UserName || x.Abbrev == user.Abbrev).FirstOrDefault();
            if (loadedUser != null)
            {
               if (loadedUser.UserName == user.UserName)
               {
                  throw new Exception(string.Format("Unable to create user {0}; user name already exists.", user.UserName));
               }

               if (loadedUser.Abbrev == user.Abbrev)
               {
                  throw new Exception(string.Format("Unable to crate user {0}; user abbreviation {1} already exists.", user.UserName, user.Abbrev));
               }
            }

            //Set current set as record
            user.Id = Guid.NewGuid().ToString();
            user.Version = 1;
            user.Current = true;

            //Create new record
            userRepository.Add(user);

            mobjDbContext.SaveChanges();

            //Create Roles

            //If no roles are set, set User as default
            if (user.Roles == null)
            {
               user.Roles = new List<UserRole>();
            }
            if (user.Roles.Where(p=>p.RoleID == mobjPermSvc.GetUserRoleID()).Count()==0)
            {
               user.Roles.Add(new UserRole { RoleID = mobjPermSvc.GetUserRoleID(), UserID = user.Id });
            }

            var db = mobjDbContext.Set<UserRole>();
            foreach(UserRole ur in user.Roles)
            {
               ur.UserID = user.Id;
               ur.RoleRef = mobjDbContext.Set<Role>().Where(p => p.Id == ur.RoleID).FirstOrDefault();
            }
            db.AddRange(user.Roles);
            mobjDbContext.SaveChanges();

            //Create permissions
            if (user.UserPermissions != null && user.UserPermissions.Count>0)
            {
               var dbUsrPerm = mobjDbContext.Set<UserPermission>();
               foreach (UserPermission up in user.UserPermissions)
               {
                  up.UserID = user.Id;
               }
               dbUsrPerm.AddRange(user.UserPermissions);
               mobjDbContext.SaveChanges();
            }
            


            //Send notification to Message Center

            mobjMsgCtrMgr.SendUserEdited(user.Id);

            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("User with {0} succesfully created with id {1}", user.UserName, user.Id);

            return user;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error creating user {0}", user.UserName);
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }

      }

      public new User Update(User user)
      {

         //TODO Trace
         mobjLoggerService.Info("Updating User with id {0} and version {1}", user.Id, user.Version);

         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<User>();

            User loadedUser = repository.SingleOrDefault(x => x.Id == user.Id && x.Current == true);
            if (loadedUser == null)
            {
               throw new Exception(string.Format("Unable to update user with id {0}; user not found.", user.Id));
            }
            if (user.Version != loadedUser.Version)
            {
               throw new Exception(string.Format("Unable to update user with id {0}; user version ({1}) is different from expected current version ({2}).", user.Id, loadedUser.Version, user.Version));
            }

            bool passwordChanged = !string.IsNullOrEmpty(user.Password) && user.Password != loadedUser.Password;
            user.EncryptPassword();

            //Create new record for updated entity
            User newUser = user.CreateUpdatedClone();

            if (passwordChanged) {
               newUser.LastPasswordChange = DateTime.Now.DatetimeUMSDBToString();
            }

            repository.Add(newUser);

            //Set current record as updated
            loadedUser.Current = false;
            loadedUser.ValidToDate = DateTime.Now.DatetimeUMSDBToString();

            mobjDbContext.SaveChanges();

            //Update Roles

            //If role "user" does not exists in list, add it
            if (user.Roles.Where(p => p.RoleID == mobjPermSvc.GetUserRoleID()).Count() == 0)
            {
               user.Roles.Add(new UserRole { RoleID = mobjPermSvc.GetUserRoleID(), UserID = user.Id });
            }

            foreach (UserRole ur in user.Roles)
            {
               ur.UserID = loadedUser.Id;
               ur.RoleRef = mobjDbContext.Set<Role>().Where(p => p.Id == ur.RoleID).FirstOrDefault();
            }




            var db = mobjDbContext.Set<UserRole>().Where(p => p.UserID == loadedUser.Id).ToList();
            mobjDbContext.RemoveRange(db);
            mobjDbContext.SaveChanges();

            db.Clear();
            db.AddRange(user.Roles);
            mobjDbContext.AddRange(db);
            mobjDbContext.SaveChanges();



            //Create permissions
            if (user.UserPermissions != null && user.UserPermissions.Count > 0)
            {
               var dbUsrPerm = mobjDbContext.Set<UserPermission>();
               var existingUSrPerm = dbUsrPerm.Where(p => p.UserID == user.Id).ToList();
               dbUsrPerm.RemoveRange(existingUSrPerm);
               mobjDbContext.SaveChanges();
               existingUSrPerm.Clear();
               foreach(UserPermission up in user.UserPermissions)
               {
                  up.UserID = user.Id;
               }
               dbUsrPerm.AddRange(user.UserPermissions);
               mobjDbContext.SaveChanges();
            }


            //Send notification to Message Center
            mobjMsgCtrMgr.SendUserEdited(user.Id);

            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("User with id {0} updated succesfully", user.Id);

            return newUser;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error updating user with id {0}", user.Id);
            string message = string.Format("Error updating user with id {0}", user.Id);
            throw new Exception(message, e);
         }

      }

      /// <summary>
      /// Disable a user. 
      /// </summary>
      public void Remove(string userId)
      {

         //TODO Trace
         mobjLoggerService.Info("Disabling User with id {0}", userId);

         try
         {

            mobjDbContext.BeginTransaction();

            var userRepository = mobjDbContext.Set<User>();

            User user = userRepository.SingleOrDefault(x => x.Id == userId && (x.Current ?? false));
            if (user == null)
            {
               throw new Exception(string.Format("Unable to disable user with id {0}; user not found.", userId));
            }

            //Create new record for updated entity
            User newUser = user.CreateUpdatedClone();
            newUser.AccountDisabled = true;
            newUser.ValidToDate = null;
            userRepository.Add(newUser);

            //Set current record as updated
            user.Current = false;
            user.ValidToDate = DateTime.Now.DatetimeUMSDBToString();

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendUserDeleted(user.Id);

            mobjDbContext.CommitTransaction();
            //TODO Trace
            mobjLoggerService.Info("User with {0} disabled succesfully", userId);
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error disabling user with id {0}", userId);
            string message = string.Format("Error disabling user with id {0}", userId);
            throw new Exception(message, e);
         }

      }


      private bool CheckPinAlreadyExists(string pin)
      {
         string strPinEncrypted = UMSFrameworkCompatibility.EncryptString(pin,null);
         var userRepository = mobjDbContext.Set<User>();
         return ( userRepository.Where(x => x.Pin != null && x.Pin == strPinEncrypted).Count() >0);
      }

      public string CreateUserPin()
      {
         bool bolIsAlreadyPresent = true;
         int intIterationMax = 50;
         int intCount = 0;
         int intPin = 0;
         Random objrdm = new Random();
         while(bolIsAlreadyPresent && intCount<intIterationMax)
         {
            intPin = objrdm.Next(10000, 99999);
            bolIsAlreadyPresent = CheckPinAlreadyExists(intPin.ToString());
            intCount++;
         }
         if(intCount>= intIterationMax)
         {
            mobjLoggerService.Error("Pin cannot be generated");
            return null;
         }
         else
         {
            return intPin.ToString();
         }
         
      }

      /// <summary>
      /// Update an existing systemoption value
      /// </summary>
      public void WriteExternalPassword(string username, string password)
      {

         //TODO Trace
         mobjLoggerService.Info("Updating User {0} password", username);

         try
         {

            mobjDbContext.BeginTransaction();

            User user = mobjDbContext.Set<User>().SingleOrDefault(x => x.UserName == username && x.Current == true);
            if (user == null)
            {
               throw new Exception(string.Format("Unable to update user {0} password; user not found.", username));
            }


            if (password == user.Password)
            {
               return;
            }

            //Update 
            user.Password = password;
            user.EncryptPassword();

            //user.LastPasswordChange = DateTime.Now.DatetimeUMSDBToString();

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();
            //TODO Trace
            mobjLoggerService.Info("{0} password updated succesfully", username);
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error updating {0} password", username);
            string message = string.Format("Error updating {0} password", username);
            throw new Exception(message, e);
         }

      }

      /// <summary>
      /// Update the last login informations
      /// </summary>
      public void WriteLastLoginAndNetworkLogin(string userId, string lastLoginDate, string hostname)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for User with id {0}", userId);

         try
         {

            mobjDbContext.BeginTransaction();

            //Get user
            User user = mobjDbContext.Set<User>().SingleOrDefault(x => x.Id == userId && x.Current == true);
            if (user == null)
            {
               throw new Exception(string.Format("Unable to update last login date for user {0}; user not found.", userId));
            }

            //Update 
            user.LastLoginDate = lastLoginDate;
            user.LoginCounter = (user.LoginCounter + 1);

            Network network = mobjDbContext.Set<Network>().SingleOrDefault(x => x.HostName == hostname);
            if (network != null)
            {
               //Update 
               network.UserId = userId;
               network.UserVersion = user.Version;
               network.LastConnection = DateTime.Now;
               network.HostName = hostname;
            }

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

         }
         catch (Exception e)
         {

            mobjDbContext.RollbackTransaction();

            mobjLoggerService.ErrorException(e, "Error updating User {0} last login", userId);
            throw new Exception(string.Format("Error updating User {0} last login", userId), e);
         }


      }


      public List<User> GetUsersByRoleID(int roleID,bool excludeDisabled)
      {
         List<User> objRet = null;
         try
         {
            List<string> objuserList = mobjDbContext.Set<UserRole>().Where(p => p.RoleID == roleID).Select(p => p.UserID).ToList();
            
            if (objuserList != null && objuserList.Count>0)
            {
               
               objRet = mobjDbContext.Set<User>()
                  .Where(p => objuserList.Contains(p.Id) && p.Current==true)
                  .Select(p=> new User { Id =p.Id, FirstName = p.FirstName, LastName = p.LastName, UserName = p.UserName,AccountDisabled = p.AccountDisabled })
                  .ToList();
               if (excludeDisabled)
               {
                  objRet.RemoveAll(p => p.AccountDisabled == true);
               }
            }
            
            
         }
         catch (Exception e)
         {
            string errMsg = string.Format("Error getting GetUsersByRoleID for RoleID {0} ", roleID);
            mobjLoggerService.ErrorException(e, errMsg); ;
            throw new Exception(errMsg, e);
         }
         return objRet;
      }

      /// <summary>
      /// Retieve a list of permission NOT included in those defined in roles (allowable as user exceptions)
      /// </summary>
      /// <param name="roles"></param>
      /// <returns></returns>
      public List<Permission> GetAllowablePermissionForRoles(int[] roles)
      {
         List<Permission> objRet = null;
         try
         {

            IQueryable<RolePermission> objRpAlreadyAllowed = mobjDbContext.Set<RolePermission>().Where(p => roles.Contains(p.RoleID) && p.Allow==true);
            objRet = mobjDbContext.Set<Permission>().Where(x => !objRpAlreadyAllowed.Any(y => y.PermissionName == x.FunctionName)).ToList();
         }
         catch (Exception e)
         {
            string errMsg = string.Format("Error getting GetAllowablePermissionForRoles for Roles {0} ", string.Join(";",roles));
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      /// <summary>
      /// Retieve a list of permission included in those defined in roles (deniable as user exceptions)
      /// </summary>
      /// <param name="roles"></param>
      /// <returns></returns>
      public List<Permission> GetDeniablePermissionForRoles(int[] roles)
      {
         List<Permission> objRet = null;
         try
         {

            IQueryable<RolePermission> objRpAlreadyAllowed = mobjDbContext.Set<RolePermission>().Where(p => roles.Contains(p.RoleID) && p.Allow == true);
            objRet = mobjDbContext.Set<Permission>().Where(x => objRpAlreadyAllowed.Any(y => y.PermissionName == x.FunctionName)).ToList();
         }
         catch (Exception e)
         {
            string errMsg = string.Format("Error getting GetDeniablePermissionForRoles for Roles {0} ", string.Join(";", roles));
            mobjLoggerService.ErrorException(e, errMsg); ;
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      public List<Permission> GetAllowablePermissionsForUser(string userID)
      {
         List<Permission> objRet = null;
         try
         {
            User objUser = Get(userID);
            objUser.PermissionsFlat = UtilityHelper.FlatPermissionsForUser(objUser);
            objRet = mobjDbContext.Set<Permission>().Where(p => !objUser.PermissionsFlat.Contains(p.FunctionName)).OrderBy(p => p.FunctionName).ToList();
            if(objUser.UserPermissions!=null)
            {
               var objUsrPermAllowed = objUser.UserPermissions.Where(p => p.Allow == true).ToList();
               foreach(UserPermission up in objUsrPermAllowed)
               {
                  objRet.Add(new Permission { FunctionName = up.PermissionName });
               }
            }
            
         }
         catch (Exception e)
         {
            string errMsg = string.Format("Error getting GetAllowablePermissionsForUser for userID {0} ", userID);
            mobjLoggerService.ErrorException(e, errMsg); ;
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      public List<Permission> GetDeniablePermissionForUser(string userID)
      {
         List<Permission> objRet = null;
         try
         {
            User objUser = Get(userID);
            objUser.PermissionsFlat = UtilityHelper.FlatPermissionsForUser(objUser);

            //objRet = mobjDbContext.Set<Permission>().Where(p => objUser.PermissionsFlat.Contains(p.FunctionName))
            //   .GroupBy(p => p.FunctionName).Select(p => p.First()).ToList();

            string strUserPermissions = string.Join(",", objUser.PermissionsFlat.Select(p => "'" + p + "'"));
            string sqlQuery = @"select distinct(FunctionName), IDPermission, PermissionCode, PriorityLevel, ModuleName, FunctionDescription from Permissions 
                             where FunctionName in (" + strUserPermissions + ")";

                objRet = mobjDbContext.Set<Permission>().FromSqlRaw(sqlQuery).ToList();
                //objRet = mobjDbContext.Set<Permission>().FromSql(sqlQuery).ToList();


               

            if (objUser.UserPermissions != null)
            {
               var objDeniedUsrPerm = objUser.UserPermissions.Where(p => p.Allow == false).ToList();
               foreach (UserPermission up in objDeniedUsrPerm)
               {
                  objRet.Add(new Permission { FunctionName = up.PermissionName });
               }
            }
         }
         catch (Exception e)
         {
            string errMsg = string.Format("Error getting GetDeniablePermissionForUser for userID {0} ", userID);
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      #endregion

      #region Abbreviation Management [Copy of UMSLogicLayer]

      /// <summary>
      /// Generate an unique abbrevation for a new User
      /// </summary>
      /// <param name="firstName"></param>
      /// <param name="secondName"></param>
      /// <returns></returns>
      public string GenerateAbbreviation(string firstName, string secondName)
      {
         string strAbbreviation = string.Empty;
         string strSource1 = string.Empty;
         string strSource2 = string.Empty;

         if (firstName.Length > 2 && secondName.Length == 0)
         {
            strAbbreviation = firstName.Substring(0, 3).ToUpper();
            if (!this.CheckAbbreviationExists(strAbbreviation))
            {
               return strAbbreviation;    // WARNING: Return
            }
         }

         strSource1 = firstName.Substring(0, 1) + ".";
         strSource2 = strSource1;

         if (firstName.Length > 1 && secondName.Length > 1)
         {
            strSource1 = firstName.Substring(0, 2);
            strSource2 = secondName.Substring(0, 2);
         }

         if (firstName.Length == 1 && secondName.Length == 1)
         {
            strSource1 = firstName.Substring(0, 1) + secondName.Substring(0, 1);
            strSource2 = secondName.Substring(0, 1) + secondName.Substring(0, 1);
         }

         strSource1 = strSource1.ToUpper();
         strSource2 = strSource2.ToUpper();

         strAbbreviation = strSource1.Substring(0, 1) + strSource2;

         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }

         strAbbreviation = strSource1 + strSource2.Substring(0, 1);
         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }


         strAbbreviation = NonsenseAlgorithm(strSource1, strSource2);
         if (!string.IsNullOrWhiteSpace(strAbbreviation)) return strAbbreviation;


         // Try to generate a numeric abbreviation of 3 chars
         int intCounter = 0;
         Random objRand = new Random();
         while (intCounter < 1000)
         {
            strAbbreviation = objRand.Next(0, 999).ToString("000");
            if (!this.CheckAbbreviationExists(strAbbreviation))
            {
               return strAbbreviation;    // WARNING: Return
            }
            intCounter++;
         }

         // Try to generate a alpha-numeric abbreviation of 3 chars
         intCounter = 0;
         objRand = new Random();
         string strAbbreviation2;
         while (intCounter < 10000)
         {
            strAbbreviation = GenerateAbbrevFromInt(objRand.Next(0, 238000));
            strAbbreviation2 = "*" + strAbbreviation.Substring(1, 2);
            if (!this.CheckAbbreviationExists(strAbbreviation2))
            {
               return strAbbreviation2;    // WARNING: Return
            }
            if (!this.CheckAbbreviationExists(strAbbreviation))
            {
               return strAbbreviation;    // WARNING: Return
            }
            intCounter++;
         }

         return "???";
      }

      /// <summary>
      /// Generate a 3 char abbreviation form an integer value
      /// Value must be between 0 and 238328
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      private string GenerateAbbrevFromInt(int value)
      {
         byte value1 = (byte)(value % 62);
         value = value / 62;
         byte value2 = (byte)(value % 62);
         value = value / 62;
         byte value3 = (byte)(value % 62);

         return GenerateAbbrevIntToChar(value3).ToString() +
            GenerateAbbrevIntToChar(value2).ToString() +
            GenerateAbbrevIntToChar(value1).ToString();
      }

      /// <summary>
      /// Convert an integer between 0 and 61 in a corresponding char
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      private char GenerateAbbrevIntToChar(byte value)
      {
         // Check if value is too big
         if (value > 61)
         {
            value = 61;
         }

         byte[] abytTemp = { 0 };
         // If value < 10 => It is a char between 0-9
         byte bytOffset = 48;
         if ((value >= 10) && (value < 36))
         {
            // If value >= 10 and < 36  => It is a char between A-Z
            bytOffset = 65 - 10;
         }
         if (value >= 36)
         {
            // If value >= 36  => It is a char between a-z
            bytOffset = 97 - 36;
         }
         abytTemp[0] = (byte)(bytOffset + value);
         return System.Text.ASCIIEncoding.ASCII.GetChars(abytTemp)[0];
      }

      private string NonsenseAlgorithm(string strSource1, string strSource2)
      {
         //Refactored, nothing more
         string strAbbreviation = string.Empty;

         strAbbreviation = strSource1.Substring(0, 1) + strSource2.Substring(0, 1) + "0";
         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }

         strAbbreviation = strSource1.Substring(0, 1) + "1" + strSource2.Substring(0, 1);
         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }

         strAbbreviation = "2" + strSource1.Substring(0, 1) + strSource2.Substring(0, 1);
         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }

         strAbbreviation = strSource2.Substring(0, 1) + strSource1.Substring(0, 1) + "3";
         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }

         strAbbreviation = strSource2.Substring(0, 1) + "4" + strSource1.Substring(0, 1);
         if (!this.CheckAbbreviationExists(strAbbreviation))
         {
            return strAbbreviation;    // WARNING: Return
         }

         return string.Empty;

         //Original block from UMSLogicLayer

         //bool bolFound = false;
         //for (int j = 0; j < 5; j++)
         //{
         //   if (j == 0)
         //   {
         //      strAbbreviation = strSource1.Substring(0, 1) + strSource2.Substring(0, 1) + j.ToString();
         //   }
         //   if (j == 1)
         //   {
         //      strAbbreviation = strSource1.Substring(0, 1) + j.ToString() + strSource2.Substring(0, 1);
         //   }
         //   if (j == 2)
         //   {
         //      strAbbreviation = j.ToString() + strSource1.Substring(0, 1) + strSource2.Substring(0, 1);
         //   }
         //   if (j == 3)
         //   {
         //      strAbbreviation = strSource2.Substring(0, 1) + strSource1.Substring(0, 1) + j.ToString();
         //   }
         //   if (j == 4)
         //   {
         //      strAbbreviation = strSource2.Substring(0, 1) + j.ToString() + strSource1.Substring(0, 1);
         //   }
         //   if (j == 5)
         //   {
         //      strAbbreviation = j.ToString() + strSource2.Substring(0, 1) + strSource1.Substring(0, 1);
         //   }
         //   if (!this.CheckAbbreviationExists(strAbbreviation))
         //   {
         //      bolFound = true;
         //      break;
         //   }
         //}

         //if (bolFound)
         //{
         //   return strAbbreviation;    // WARNING: Return
         //}
      }

      #endregion

   }
}
