using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Configurator.Std.Exceptions;
using Microsoft.Data.SqlClient;

namespace Configurator.Std.BL
{
   public class RolesManager : DalManagerBase<Role>, IRolesManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDictionaryService mobjDicSvc;
      private readonly IDigistatConfiguration mobjDigCfg;

      public RolesManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService,IDictionaryService dicSvc,IDigistatConfiguration digCfg)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjDicSvc = dicSvc;
         mobjDigCfg = digCfg;
     
      }

      public Role Get(int id)
      {
         Role result = null;
         try
         {
            result = mobjDbContext.Set<Role>().Include(x => x.Permissions).Where(x => x.Id == id ).SingleOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Role with ID {0} from DB", id);
            throw new Exception(string.Format("Error reading Role with ID {0} from DB", id), e);
         }
         return result;
      }
      
      
      /// <summary>
      /// Returns a list of all available roles, with only ID and name for each.
      /// Other properties shall not be set
      /// </summary>
      /// <returns></returns>
      public List<Role> GetAllFast()
      {
         List<Role> objRet = null;
         try
         {
            var objRepoRole = mobjDbContext.Set<Role>();
            objRet = objRepoRole.ToList();
         }
         catch(Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading GetAllFast Role");
            throw new Exception(string.Format("Error reading GetAllFast Role"), e);
         }
         
         return objRet;
      }

      /// <summary>
      /// Returns a list of all available roles, with, for each, the list of permissions and users
      /// </summary>
      /// <returns></returns>
      public List<Role> GetAll()
      {
         List<Role> objRet = null;
         try
         {
            var objRepoUser = mobjDbContext.Set<User>();
            var objRepoRole = mobjDbContext.Set<Role>();
            var objRolePerm = mobjDbContext.Set<RolePermission>();


                string strSqlRoleAndUserCount = @"select roles.rol_ID,roles.rol_Description,isnull(UserCount,0) as UsersCount from roles
                    left join
                    (select RoleID,count(*) as UserCount from UserRoles ur
                    left join users u on ur.UserID = u.us_ID and u.us_Current = 1
                    where ISNULL(u.us_AccountDisabled,0)=0
                     group by RoleID) as urg
                    on roles.rol_ID = urg.RoleID
                ";
                using (var objConn = new SqlConnection(mobjDigCfg.ConnectionString))
                {
                    using (var objCommand = new SqlCommand(strSqlRoleAndUserCount, objConn))
                    {
                        objConn.Open();
                        using (var objReader = objCommand.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                if (objRet == null)
                                    objRet = new List<Role>();
                                objRet.Add(new Role()
                                {
                                    Id = (int)objReader.GetSqlInt32(0),
                                    RoleName = objReader.GetString(1),
                                    UsersCount = (int)objReader.GetSqlInt32(2)
                                }); 
                            }
                        }
                    }
                }

                List<RolePermission> objPermList = objRolePerm.ToList();

                if (objRet != null)
                {
                    foreach (Role r in objRet)
                    {
                        r.Permissions = objPermList.Where(p => p.RoleID == r.Id).ToList();
                    }
                }
                
                

            }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading GetAll Role");
            throw new Exception(string.Format("Error reading GetAll Role"), e);
         }
         
         return objRet;
      }


      public new Role Create(Role objRole)
      {
         Role objRet = null;
         try
         {

            if (string.IsNullOrEmpty(objRole.RoleName))
            {
               throw new RoleException(mobjDicSvc.XLate("A role must have a name"));
            }

            mobjDbContext.BeginTransaction();
            //Add entry in roles
            var objRoleRepo = mobjDbContext.Set<Role>();
            //Check if a role with the same name already exists
            int intRoleExists = objRoleRepo.Where(p => p.RoleName.Trim().ToUpper() == objRole.RoleName.Trim().ToUpper()).Count();
            if(intRoleExists>0)
            {
               mobjDbContext.RollbackTransaction();
               throw new RoleException($"A role with name {objRole.RoleName} already exists");
            }
            else
            {
               //Detach permissions from role
               List<RolePermission> objListRp = new List<RolePermission>();
               if (objRole != null)
               {
                  foreach (RolePermission rp in objRole.Permissions)
                  {
                     objListRp.Add(new RolePermission { Allow = rp.Allow, PermissionName = rp.PermissionName, RoleID = rp.RoleID });
                  }
               }
               objRole.Permissions.Clear();

               objRoleRepo.Add(objRole);
               mobjDbContext.SaveChanges();



               if (objListRp != null)
               {
                  foreach (RolePermission rp in objListRp)
                  {
                     rp.RoleID = objRole.Id;
                  }

                  var rolPermRepo = mobjDbContext.Set<RolePermission>();
                  rolPermRepo.AddRange(objListRp);
                  mobjDbContext.SaveChanges();
               }
               Permission p = new Permission();
               mobjMsgCtrMgr.SendPermissionEdited(p);

               mobjMsgCtrMgr.SendRoleEdited(objRole);


               mobjDbContext.CommitTransaction();
               objRet = objRole;
            }
            
         }
         catch (Exception e)
         {
            if (e is NetworkCreationException || e is RoleException)
            {
               throw;
            }
            mobjDbContext.RollbackTransaction();
            string errMsg = "Error Creating a Role";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      public new Role Update(Role objRole)
      {
         Role objRet = null;
         try
         {

            Role objToUpdate = mobjDbContext.Set<Role>().Include(p=>p.Permissions).Where(x => x.Id == objRole.Id).FirstOrDefault();
            if (objToUpdate != null)
            {
               objToUpdate.RoleName = objRole.RoleName;
               mobjDbContext.BeginTransaction();
               objToUpdate.RoleName = objRole.RoleName;
               mobjDbContext.SaveChanges();
               
               objToUpdate.Permissions.Clear();
               if (objRole.Permissions != null)
               {
                  foreach (RolePermission rp in objRole.Permissions)
                  {
                     //Exclude duplicates
                     if (objToUpdate.Permissions.Where(p => p.PermissionName == rp.PermissionName).Count() == 0)
                     {
                        if (!string.IsNullOrEmpty(rp.PermissionName))
                        {
                           rp.RoleID = objToUpdate.Id;
                           objToUpdate.Permissions.Add(rp);
                        }
                        
                     }
                     
                  }
               }
               mobjDbContext.SaveChanges();

               Permission prr = new Permission();
               mobjMsgCtrMgr.SendPermissionEdited(prr);

               mobjMsgCtrMgr.SendRoleEdited(objToUpdate);

               mobjDbContext.CommitTransaction();
               objRet = objToUpdate;
            }
            else
            {
               //TODO: Notfounderror
            }

        
         }
         catch (Exception e)
         {
            if (e is NetworkCreationException)
            {
               throw;
            }
            mobjDbContext.RollbackTransaction();
            string roleID = objRole != null ? objRole.Id.ToString() : "unknown";
            string errMsg = string.Format("Error Updating Role {0}", roleID);
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }

      public bool Delete(int id)
      {
         bool bolRet = false;
         try
         {
            var objrepo = mobjDbContext.Set<Role>();
            Role objRoleFound = objrepo.Where(p => p.Id == id).FirstOrDefault();

            var objRepoRolePerm = mobjDbContext.Set<RolePermission>();
            var objRepoRoleUser = mobjDbContext.Set<UserRole>();

            if (objRoleFound != null)
            {
               mobjDbContext.BeginTransaction();

               var objRoleUser = objRepoRoleUser.Where(p => p.RoleID == id).ToList();
               if (objRoleUser != null)
               {
                  objRepoRoleUser.RemoveRange(objRoleUser);
               }


               var objRolePerm = objRepoRolePerm.Where(p => p.RoleID == id).ToList();
               if (objRolePerm != null)
               {
                  objRepoRolePerm.RemoveRange(objRolePerm);
               }

               objrepo.Remove(objRoleFound);
               mobjDbContext.SaveChanges();
               Permission prr = new Permission();
               mobjMsgCtrMgr.SendPermissionEdited(prr);

               mobjMsgCtrMgr.SendRoleEdited(objRoleFound);

               mobjDbContext.CommitTransaction();
            }
            
            bolRet = true;
         }
         catch(Exception e)
         {
            mobjDbContext.RollbackTransaction();
            string errMsg = string.Format("Error Deleting Role {0}", id);
            mobjLoggerService.ErrorException(e, errMsg);
         }
         return bolRet;
      }


      #endregion

   }
}
