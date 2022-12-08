using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Controllers
{
    public class ProvisionController : DigistatWebControllerBase
    {

        private readonly IPermissionsService mobjPermSvc;

        public ProvisionController(
         IDigistatConfiguration config,
         IMessageCenterService msgcenter,
         ISynchronizationService syncSvc,
         IDictionaryService dicSvc,
         IDnsCacherService dnsSvc,
         ILoggerService logSvc,
         ISystemOptionsService sysOptSvc,         
         IPermissionsService permSvc) : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)
        {
            mobjPermSvc = permSvc;
        }

        public ActionResult ProvisionConfig()
        {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
            {
                ViewBag.SitePath = "Mobile > Provision Config";
                return View();
            }
            else
            {
                return View("NotAuthorized");
            }
        }
    }
}
