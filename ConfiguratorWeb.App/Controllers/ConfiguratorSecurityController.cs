using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.Helpers;

using Digistat.FrameworkWebExtensions.Controllers;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Authentication;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Enums;

namespace ConfiguratorWeb.App.Controllers
{
   [DigConfigFilterAttribute]
   public class ConfiguratorSecurityController : DigistatWebControllerBase   {

      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private readonly IAuthenticationManager mobjDigAuthenticationService;
      private readonly IPermissionsService mobjpermService;
      private ILoggerService mobjLog;

      public ConfiguratorSecurityController(IDigistatConfiguration config, IMessageCenterService msgcenter, IAuthenticationManager authenticationService,
         ISynchronizationService syncSvc, IDictionaryService dicSvc, 
         IDnsCacherService dnssvc, ILoggerService logsvc, 
         ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc,IPermissionsService permSvc)
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjDigAuthenticationService = authenticationService;
         mobjpermService = permSvc;
      }

      public IActionResult Login()
      {
         return View("Login");
      }

      public IActionResult LoginCheck(string username, string password)
      {
         try
         {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
               //UserManager userManager = new UserManager(mobjDigistatConfig, mobjSystemOptionsService);
               LoginResult objLoginRes = mobjDigAuthenticationService.Login(username, password, false, CurrentNetwork.HostName, false);
               if (objLoginRes != null && objLoginRes.Status == UserLoginStatus.Success)
               {
                  User objUser = objLoginRes.User;
                  if (objUser != null)
                  {
                     CurrentUser = objUser;
                     //Check Configurator Permission
                     if (mobjpermService.CheckPermission(Configurator.Std.Defs.Permissions.permissionConfigurator,CurrentUser))
                     {
                        
                        return Json(new { success = true, redirectTo = Url.Action("Index", "Home") });
                     }
                     else
                     {
                        CurrentUser = null;
                        ViewBag.Message = XLate(CommonStrings.NO_VALID_PERMISSION);
                        return Json(new { success = false, redirectTo = "", errorMessage = XLate(CommonStrings.NO_VALID_PERMISSION) });
                     }
                     
                  }
               }
               else
               {
                  //Log only if objLoginResult is null (something weird occurred!)
                  if (objLoginRes == null)
                  {
                     mobjLogSvc.Write(100, $"Login Failed for user {username}: LoginResult = null", Digistat.FrameworkStd.Enums.EventLogEntryType.Warning, null, 0, LogType.CLN, mobjDigistatConfig.ModuleName);
                  }
                  
               }
            }
            else
            {
               ViewBag.Message = XLate("Wrong username or password");
               return Json(new { success = false, redirectTo = "", errorMessage = XLate("Wrong username or password") });

            }
            return Json(new { success = false, redirectTo = "", errorMessage = XLate("Wrong username or password") });
         }
         catch (Exception ex)
         { 
            mobjLogSvc.Write(100, "Error on Configurator Web Login : " + ex.ToString(), Digistat.FrameworkStd.Enums.EventLogEntryType.Error);
            //return View("_ErrorMessagePartial", ex.ToString());
            ViewBag.Message = "Error on Configurator Login : " + ex.ToString();
            return Json(new { success = false, redirectTo = "", errorMessage = "System not available, please try later"});
         }
      }

      [HttpGet]
      public ActionResult LogOff()
      {
         AppHttpContext.Current.Session.Clear();
         CurrentUser = null;
         return RedirectToAction("Login","ConfiguratorSecurity");

      }
   }
}
