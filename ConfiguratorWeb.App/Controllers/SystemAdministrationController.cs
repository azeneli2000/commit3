using System;
using System.Collections.Generic;
using System.Linq;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Configurator.Std.BL;
using Configurator.Std.BL.Hubs;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.Services;

namespace ConfiguratorWeb.App.Controllers
{
   public class SystemAdministrationController : DigistatWebControllerBase
   {
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IUsersManager mobjUsersManager;
      private readonly IPermissionsManager mobjPermissionsManager;
      private readonly IPersonnelManager mobjPersonnelManager;
      private readonly IPersonnelRolesManager mobjPersonnelRolesManager;
        private readonly IPermissionsService mobjPermSvc;
      private readonly IRolesManager mobjRoleManager;
      private readonly IMenuService mobjMenuSvc;

      public SystemAdministrationController(IDigistatConfiguration config, IMessageCenterService msgcenter,
         ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
         ISystemOptionsService sysOptSvc, IMessageCenterManager msgCtrMgr, IMenuService menuService,
         IUsersManager usersManager, IPermissionsManager permissionsManager, IPermissionsService permSvc, 
         IPersonnelManager personnelManager, IPersonnelRolesManager personnelRolesManager,IRolesManager roleManager)
     : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjUsersManager = usersManager;
         mobjPermissionsManager = permissionsManager;
         mobjPersonnelManager = personnelManager;
         mobjPersonnelRolesManager = personnelRolesManager;
         mobjPermSvc = permSvc;
         mobjRoleManager = roleManager;
         mobjMenuSvc = menuService;
      }

      // GET: SystemAdministration
      public ActionResult Index()
      {

         return View();
      }

      public ActionResult UserAccounts()
      {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
            {
                ViewBag.SitePath = "General > System Administration > User Accounts";
                return View();
            }
            else
            {
                return View("NotAuthorized");
            }
      }


      public JsonResult ReadUsers([DataSourceRequest] DataSourceRequest request,short showDisabled)
      {
        if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
        {
            IQueryable<User> objAllUsers = null;
            if (showDisabled==0)
            {
               objAllUsers = mobjUsersManager.GetQueryable().Where(a => (a.Current.HasValue && a.Current.Value==true)  && !(a.AccountDisabled.HasValue && a.AccountDisabled.Value == true));
            }
            else
            {
               objAllUsers = mobjUsersManager.GetQueryable().Where(a => (a.Current.HasValue && a.Current.Value==true) && (a.AccountDisabled.HasValue && a.AccountDisabled.Value == true));
            }
            DataSourceResult data = null;
            try
            {
               data = objAllUsers.ToDataSourceResult(request, model => UserViewModelBuilder.Build(model));
               return new JsonResult(data);
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadUsers");
               return Json(new { errorMessage = "Error on ReadUsers", success = false });
            }
        }
        else
        {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
        }
      }


      public JsonResult GeneratePin()
      {
         try
         {
            return new JsonResult(mobjUsersManager.CreateUserPin());
         }
         catch(Exception)
         {
            throw;
         }
      }

      


      public ActionResult GetUser(string id)
      {
        if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
        {
            UserViewModel model = new UserViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                User user = mobjUsersManager.Get(id);
                model = UserViewModelBuilder.Build(user);

               //Build allowed and denied string array from collection
               if (user.UserPermissions != null)
               {
                  List<UserPermission> objUsrPermissions = user.UserPermissions.Where(p => p.Allow).ToList();
                  model.AllowedPermissions = String.Join("#§#",objUsrPermissions.Select(p=>p.PermissionName));

                  List<UserPermission> objUsrPermissionsDenied = user.UserPermissions.Where(p => p.Allow==false).ToList();
                  model.DeniedPermissions = String.Join("#§#", objUsrPermissionsDenied.Select(p => p.PermissionName));
               }


            }

            return PartialView("_UserTabStrip", model);
        }
        else
        {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
        }

        }

      public JsonResult GetDeniablePermissionsForUser(string userID)
      {
         bool bolSuccess = false;
         string messageError = string.Empty;
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
         {
            try
            {
               try
               {
                  List<Permission> objPerm = mobjUsersManager.GetDeniablePermissionForUser(userID);
                  return new JsonResult(objPerm);
               }
               catch (Exception e)
               {
                  return Json(new { errorMessage = e.Message, success = false });
               }
            }
            catch(Exception e)
            {
               return Json(new { errorMessage = e.Message, success = false });
            }
            
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public JsonResult GetAllowablePermissionsForFoles(string roles)
      {
         bool bolSuccess = false;
         string messageError = string.Empty;
         
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
         {
            try
            {
               int[] aroles = { };
               if (!string.IsNullOrEmpty(roles))
               {
                  aroles = roles.Split(";",StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
               }
               List<Permission> objPerm = mobjUsersManager.GetAllowablePermissionForRoles(aroles);
               return new JsonResult(objPerm);
            }
            catch (Exception e)
            {
               return Json(new { errorMessage = e.Message, success = false });
            }

         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetDeniablePermissionsForFoles(string roles)
      {
         bool bolSuccess = false;
         string messageError = string.Empty;

         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
         {
            try
            {
               int[] aroles = { };
               if (!string.IsNullOrEmpty(roles))
               {
                  aroles = roles.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
               }
               List<Permission> objPerm = mobjUsersManager.GetDeniablePermissionForRoles(aroles);
               return new JsonResult(objPerm);
            }
            catch (Exception e)
            {
               return Json(new { errorMessage = e.Message, success = false });
            }

         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetAllowablePermissionsForUser(string userID)
      {
         bool bolSuccess = false;
         string messageError = string.Empty;
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
         {
            try
            {
               List<Permission> objPerm = mobjUsersManager.GetAllowablePermissionsForUser(userID);
               return new JsonResult(objPerm);
            }
            catch (Exception e)
            {
               return Json(new { errorMessage = e.Message, success = false });
            }
            
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public JsonResult SaveUserDetail(UserViewModel model)
      {
         //update YES/NO props
         string messageError = string.Empty;
         User objUser = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserEdit, CurrentUser))
            {
               //Build permissions collection from string
               
               if (!string.IsNullOrEmpty(model.AllowedPermissions))
               {
                  List<string> objAllowedExceptions = model.AllowedPermissions.Split("#§#").ToList();
                  foreach(string s in objAllowedExceptions)
                  {
                     if (!string.IsNullOrEmpty(s))
                     {
                        model.Permissions.Add(new UserPermissionViewModel { PermissionName = s, Allow = true });
                     }
                  }
               }
               if (!string.IsNullOrEmpty(model.DeniedPermissions))
               {
                  List<string> objDeniedExceptions = model.DeniedPermissions.Split("#§#").ToList();
                  foreach (string s in objDeniedExceptions)
                  {
                     if (!string.IsNullOrEmpty(s))
                     {
                        model.Permissions.Add(new UserPermissionViewModel { PermissionName = s, Allow = false });
                     }
                  }
               }


               if (string.IsNullOrWhiteSpace(model.Id)) //create
               {


                  if (!mobjUsersManager.CheckAbbreviationExists(model.Abbrev))
                  {
                     objUser = mobjUsersManager.Create(UserEntityBuilder.Build(model));
                  }
               }
               else //update
               {
                  if (!mobjUsersManager.CheckAbbreviationExists(model.Abbrev, model.Id))
                  {
                     objUser = mobjUsersManager.Update(UserEntityBuilder.Build(model));
                  }
                  else
                  {
                     messageError =
                        mobjDicSvc.XLate(
                           $"This abbreviation ({model.Abbrev}) cannot be used. Please correct and try again.");
                  }

               }



               //using(var manager = new UsersManager(Security.SecurityManagerFactory.GetCurrent().CurrentUserIdentifier))
                //{

                //}

                if (objUser != null)
                {
                    bolSuccess = true;
                }
            }
            else
            {
                bolSuccess = false;
                messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
                    

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }
      [HttpPost]
      public JsonResult  DisableUser(string id)
      {
         string messageError = string.Empty;
         User objUser = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserEdit, CurrentUser))
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    try
                    {
                        mobjUsersManager.Remove(id);
                        bolSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        messageError = ex.Message;
                    }

                }
                else //update
                {
                    messageError = "user not defined";
                }
            }
            else
            {
                bolSuccess = false;
                messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
                    

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
      [HttpGet]
      public JsonResult GetAbbreviation(string firstname, string lastname)
      {
         //update YES/NO props
         string messageError = string.Empty;

         try
         {
            string strAbbrev = mobjUsersManager.GenerateAbbreviation(firstname, lastname);
            return Json(new { message = strAbbrev, errorMessage = messageError, success = true });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }

      [HttpGet]
      public JsonResult CheckAbbreviation(string abbrev, string userId)
      {
         //update YES/NO props
         string messageError = string.Empty;

         try
         {
            bool bolAbbrevExists = mobjUsersManager.CheckAbbreviationExists(abbrev,userId);

            //check moved in "CheckAbbreviationExists" function
            ////if we're editing the user and the abbreviation already exists, we should check whether the user having userId has this abbrev
            //if (bolAbbrevExists && !string.IsNullOrWhiteSpace(userId))
            //{
            //   User objCurrentUser = mobjUsersManager.Get(userId);
            //   if (objCurrentUser.Abbrev == abbrev)
            //   {
            //      bolAbbrevExists = false;
            //   }
            //}

            return Json(new { message = bolAbbrevExists, errorMessage = messageError, success = true });

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }
      [HttpGet]
      public JsonResult  CheckIfUserCanBeDeleted(string userId)
      {
         string messageError = string.Empty;

         try
         {
            bool bolIsInPlannedResources = mobjUsersManager.CheckIfUserCanBeDeleted(userId);
            if (!bolIsInPlannedResources )
            {
               messageError = "the user is in planned resourses";
            }

            return Json(new { message = "", errorMessage = messageError, success = bolIsInPlannedResources });


         

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      #region Permissions

      public ActionResult Permissions()
      {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsView, CurrentUser))
            {
                ViewBag.SitePath = "General > System Administration > Permissions ";
                return View();
            }
            else
            {
                return View("NotAuthorized");
            }

        }

      public ActionResult GetPermission(int id)
      {
        if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsView, CurrentUser))
        {
            Permission permission = mobjPermissionsManager.Get(id);
            PermissionViewModel model = PermissionViewModelBuilder.Build(permission);

            return PartialView("_PermissionTabStrip", model);
        }
        else
        {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
        }
        

      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public JsonResult SavePermissionDetail(PermissionViewModel model)
      {
         string messageError = string.Empty;
         Permission objPermission = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsEdit, CurrentUser))
            {
                if (!model.Id.HasValue) //create
                {
                    objPermission = mobjPermissionsManager.Create(PermissionEntityBuilder.Build(model));
                }
                else //update
                {
                    objPermission = mobjPermissionsManager.Update(PermissionEntityBuilder.Build(model));
                    
                }
                if (objPermission != null)
                {
                    bolSuccess = true;
                }
                //Clean cache
                mobjPermSvc.ClearCache();
                mobjMenuSvc.ClearCache();
            }
            else
            {
                messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
                

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }

      public JsonResult ReadPermissions([DataSourceRequest] DataSourceRequest request)
      {
         IQueryable<Permission> objAll = mobjPermissionsManager.GetQueryable();

         //List<PermissionViewModel> objAll = UtilityHelper.GetPermissions().ToList();
         DataSourceResult data = objAll.ToDataSourceResult(request, model => PermissionViewModelBuilder.Build(model));
         return new JsonResult(data);
         //return new JsonResult { Data = data, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
      }

      public ActionResult GetPermissionList()
      {
         List<SelectListItem> objPermission = new List<SelectListItem>();
         var objPermissionStored = mobjPermissionsManager.GetAll();
         return Json(objPermissionStored.Select(a => new PermissionViewModel {
            FunctionName = a.FunctionName,
            Id = a.Id,
            Description =a.Description,
            ModuleName =a.Module==null?string.Empty:a.Module
         }));
      }


      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult CreatePermission([DataSourceRequest] DataSourceRequest request, PermissionViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsEdit, CurrentUser))
            {
                Permission permission = PermissionEntityBuilder.Build(model);

                mobjPermissionsManager.Create(permission);
            }
            else
            {
                ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }
               
         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json((new[] { model }).ToDataSourceResult(request, ModelState));
      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult UpdatePermission([DataSourceRequest] DataSourceRequest request, PermissionViewModel model)
      {

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsEdit, CurrentUser))
            {
                Permission permission = PermissionEntityBuilder.Build(model);

                mobjPermissionsManager.Update(permission);
            }
            else
            {
                ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }
               
         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json(new[] { model }.ToDataSourceResult(request, ModelState));
      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult DeletePermission([DataSourceRequest] DataSourceRequest request, PermissionViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionPermissionsEdit, CurrentUser))
            {
                mobjPermissionsManager.Delete(model.Id.Value);
                //Clean cache
                mobjPermSvc.ClearCache();
                mobjMenuSvc.ClearCache();
            }
            else
            {
                ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }
               

         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json(new[] { model }.ToDataSourceResult(request, ModelState));
      }


      #endregion

      #region Personnel

      public JsonResult ReadPersonnel([DataSourceRequest] DataSourceRequest request)
      {

         IQueryable<Personnel> objAll = mobjPersonnelManager.GetQueryable().Where(a => a.Current);

         DataSourceResult data = objAll.ToDataSourceResult(request, model => PersonnelViewModelBuilder.Build(model));
         return new JsonResult(data);
         //return new JsonResult { Data = data, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
      }

      #endregion

      #region Role

      [HttpPost]
      public ActionResult DeleteRole(string id)
      {
         bool bolSuccess = false;
         string errMsg = string.Empty;
         try
         {
            
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
            {
               if(!string.IsNullOrEmpty(id))
               {
                  int intID = 0;
                  if (int.TryParse(id,out intID))
                  {
                     if(intID > 99) // Role IDs from 0-99 are reserved for system roles, the restriction is present in the database (dbo.Roles)
                     {
                        mobjPermSvc.GetAdminRoleID();
                        mobjRoleManager.Delete(intID);
                        //Clean cache
                        mobjPermSvc.ClearCache();
                        mobjMenuSvc.ClearCache();
                        bolSuccess = true;
                     }
                     else
                     {
                        errMsg = "This is a System Role and cannot be deleted.";
                        
                     }
                  }
                  else
                  {
                     errMsg = "Invalid role ID.";
                  }
               }
               else
               {
                  errMsg = "Invalid role ID.";
               }
            }
            else
            {
               errMsg = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = errMsg, success = bolSuccess });

         }
         catch (Exception e)
         {
            errMsg = e.Message;
            return Json(new { errorMessage = errMsg, success = bolSuccess });
         }

       
      }



      public JsonResult ReadRole([DataSourceRequest] DataSourceRequest request)
      {

         IQueryable<PersonnelRole> objAll = mobjPersonnelRolesManager.GetQueryable().Where(a => a.Current);

         //List<PersonnelRoleViewModel> objAll = new List<PersonnelRoleViewModel>();
         DataSourceResult data = objAll.ToDataSourceResult(request, model => PersonnelRoleViewModelBuilder.Build(model));
         return new JsonResult(data);
         //return new JsonResult { Data = data, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
      }

      #endregion

      #region Enabled Users

      public JsonResult ReadEnabledUsers([DataSourceRequest] DataSourceRequest request,short? minLevel)
      {

        if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
        {
            IQueryable<User> objAll = mobjUsersManager.GetQueryable().Where(a => a.Current ?? false);
            objAll = objAll.Where(x => !(x.AccountDisabled ?? false) && (x.Current ?? false));
            if (minLevel.HasValue)
            {
                objAll = objAll.Where(x => x.PermissionLevel >= minLevel);
            }
            DataSourceResult data = objAll.ToDataSourceResult(request, model => UserViewModelBuilder.Build(model));
            return new JsonResult(data);
        }
        else
        {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
        }
                
      }

      #endregion


      #region Roles

      public ActionResult Roles()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Administration > Roles ";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }

      }


      public JsonResult ReadRoles([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesView, CurrentUser))
         {
            List<Role> objAllRoles = mobjRoleManager.GetAll();
            DataSourceResult data = null;
            try
            {
                    data = objAllRoles.ToDataSourceResult(request, model => RoleViewModelBuilder.Build(model));
               return new JsonResult(data);
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadRoles");
               return Json(new { errorMessage = "Error on ReadRoles", success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public IActionResult GetRoles([FromBody] IEnumerable<UserRoleViewModel> usrRoles)
      {

         List<RoleViewModel> model = RoleViewModelBuilder.BuildList(mobjRoleManager.GetAll()).OrderBy(o => o.RoleName).ToList();

         if (usrRoles != null)
         {
            var usrRolesGrouped = usrRoles.GroupBy(p => p.RoleID).ToList();
            foreach(var objGroup in usrRolesGrouped)
            {
               RoleViewModel objBed = model.Where(a => a.Id == objGroup.Key).FirstOrDefault();
               objBed.Selected = true;
            }
         }
         return Json(new { content = this.RenderViewAsync("_RolesSelection", model, true) });
      }

      public ActionResult GetRole(string Id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesView, CurrentUser))
         {
            RoleViewModel model = new RoleViewModel();
            if (!string.IsNullOrWhiteSpace(Id))
            {
               Role objRole = mobjRoleManager.Get(Convert.ToInt32(Id));
               model = RoleViewModelBuilder.Build(objRole);
            }
                ViewData["CurrentUser"] = CurrentUser;
               //var b=  mobjPermSvc.CheckPermission("CONF.DICTIONARY.ADD", CurrentUser);
               // var c = mobjPermSvc.CheckPermission("lksdsdkjh34", CurrentUser);
                return PartialView("_RoleTabStrip", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetUsersForRole(int RoleID)
      {
         bool bolSuccess = false;
         string messageError = string.Empty;
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesView, CurrentUser))
         {
            try
            {
               var objUsers = UserViewModelBuilder.BuildList(mobjUsersManager.GetUsersByRoleID(RoleID,true));
               if (objUsers != null)
               {
                  objUsers.OrderBy(p => p.UserName);
               }
               return new JsonResult(objUsers.ToDataSourceResult(new DataSourceRequest()));
            }
            catch (Exception e)
            {
               return Json(new { errorMessage = e.Message, success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      [RequestFormSizeLimit(valueCountLimit: 90000)]
      public JsonResult SaveRoleDetail(RoleViewModel model)
      {
         //update YES/NO props
         bool bolSuccess = false;
         string messageError = string.Empty;
         try
         {
            if (model.Permissions == null)
            {
               model.Permissions = new List<RolePermissionViewModel>();
            }
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
            {
               if (model.Permissions.Any(p => string.IsNullOrEmpty(p.PermissionName)))
               {
                  messageError = mobjDicSvc.XLate("Permission without name is not allowed.");
               }

               Role objRole;
               if (!model.Id.HasValue || model.Id == -1) //create
               {
                  objRole = mobjRoleManager.Create(RoleEntityBuilder.Build(model));
               }
               else //update
               {
                  objRole = mobjRoleManager.Update(RoleEntityBuilder.Build(model));
               }

               mobjPermSvc.ClearCache();
               mobjMenuSvc.ClearCache();

               if (objRole != null)
               {
                  bolSuccess = true;
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      #endregion
   }
}
