using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace ConfiguratorWeb.App.Controllers
{
    public class DigistatConfigControllerBaseController : DigistatWebControllerBase
   {

      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private ILoggerService mobjLog;

      public DigistatConfigControllerBaseController(IDigistatConfiguration config, IMessageCenterService msgcenter,
       ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc)
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
      }

        public IActionResult Index()
        {
            return View();
        }
    }
}