using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Configurator.Std.BL;
using Configurator.Std.BL.Hubs;
using ConfiguratorWeb.App.Models.Actions;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfiguratorWeb.App.Controllers
{
   public class ActionsController : DigistatWebControllerBase
   {
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private ILoggerService mobjLog;
      private readonly IPermissionsService mobjPermSvc;
      private readonly IServiceStatusManager mobjSTManager;
      private readonly IClinicalLogQueueManager  mobjClqManager;
      private readonly IDigistatLogManager  mobjDigLogManager;

      public ActionsController(IDigistatConfiguration config, IMessageCenterService msgcenter,
          IMessageCenterManager msgCtrMgr,
          ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc
          , ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IPermissionsService permSvc
          , IServiceStatusManager stmanager
          , IClinicalLogQueueManager clqManager, IDigistatLogManager digistatLogManager
          )
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjPermSvc = permSvc;
         mobjSTManager = stmanager;
         mobjClqManager = clqManager;
         mobjDigLogManager = digistatLogManager;
         //mobjMessageCenterSvc.OnMessageReceived += MobjMessageCenterSvc_OnMessageReceived;


      }

      

      // GET: Actions
      public ActionResult Index()
      {
         return View();
      }


      public PingResult Ping(string hostname)
      {
         
         PingResult objResult = new PingResult();
         objResult.Hostname = hostname;
         objResult.Time = -1;
         if (hostname!= null && hostname.Contains("@"))
         {
            hostname = hostname.Substring(hostname.IndexOf("@") + 1);
         }
         System.Net.NetworkInformation.Ping objPing = new System.Net.NetworkInformation.Ping();
         try
         {
            System.Net.NetworkInformation.PingReply objPingResult = objPing.Send(hostname, 1000);

            objResult.Time = (int)objPingResult.RoundtripTime;
         }
         catch (Exception)
         {

         }
         objPing.Dispose();
         return objResult;
      }

     

      public IActionResult ProbeAllNetwork()
      {
         try
         {
            Digistat.FrameworkStd.MessageCenter.MCMessage objMessage = new Digistat.FrameworkStd.MessageCenter.MCMessage();
            objMessage.SourceApp = mobjConfiguration.ModuleName;
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.DestinationApp = "ALL";
            objMessage.DestinationHost = "ALL";
            objMessage.Message = "PING";
            objMessage.PacketType = "PING";
            objMessage.PatientID = "0"; 
            mobjMessageCenterSvc.SendMessage(objMessage);
            return Ok();
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error probing network");
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }
      
      public IActionResult LogoutAllHosts()
      {
         try
         {
            mobjMsgCtrMgr.LogoutAllHosts( CurrentUser.Abbrev);
            
            return Ok();
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide());
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }
      public IActionResult ShutDownAllHosts()
      {
         try
         {
            mobjMsgCtrMgr.ShutdownAllHosts( CurrentUser.Abbrev);
            
            return Ok();
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide());
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }
      
      public IActionResult ChangeMessageCenterToAll(string newMC, string newIstance)
      {
         if (String.IsNullOrWhiteSpace(newMC) || String.IsNullOrWhiteSpace(newIstance) )
         {
            return StatusCode(StatusCodes.Status500InternalServerError,"message center or istance not valid");
         }
         try
         {
            mobjMsgCtrMgr.ChangeMessageCenterChange( newMC + ":" + newIstance);
            
            return Ok();
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error sending {0} message", UMSFrameworkParser.GetMessageTypeRemoteCommandLogoutAndHide());
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      public ActionResult NetworkProbe()
      {
         ViewBag.SitePath = "Actions > Network Probe";
         return View();
      }

      public ActionResult NetworkProbePartial()
      {
         return PartialView("NetworkProbe");
      }

      public ActionResult HAMonitor()
      {
         SystemOption objSysOption = mobjSysOptSvc.CheckAndCreateSystemOption(
            applicationName: mobjDigistatConfig.ModuleName, userAbbrev: null, hospitalUnitShortName: null, hostName: null
            , optionName: "HAofflineTimeout", value: "60", description: "number of seconds after which an idle service is considered offline (min 30sec)"
            , type: Digistat.FrameworkStd.Enums.OptionType.Integer, upperLimit: 10800, lowerLimit: 30
            , level: 0, isSystem: true);
         int mintHAofflineTimeoutInSeconds = 30;
         int.TryParse(objSysOption.Value, out mintHAofflineTimeoutInSeconds);

         //mobjSystemOptionsService.GetSystemOption(applicationName: "");
         ViewBag.HAofflineTimeoutInSeconds = mintHAofflineTimeoutInSeconds;
         ViewBag.IsHAEnabled = mobjDigEnvironmentService.IsHAEnabled;
         ViewBag.SitePath = "Actions > High Availability Monitor";
         return View();
      }

      public ActionResult HAMonitorPartial()
      {
         SystemOption objSysOption = mobjSysOptSvc.CheckAndCreateSystemOption(
            applicationName: mobjDigistatConfig.ModuleName, userAbbrev: null, hospitalUnitShortName: null, hostName: null
            , optionName: "HAofflineTimeout", value: "60", description: "number of seconds after which an idle service is considered offline (min 30sec)"
            , type: Digistat.FrameworkStd.Enums.OptionType.Integer, upperLimit: 10800, lowerLimit: 30
            , level: 0, isSystem: true);
         int mintHAofflineTimeoutInSeconds = 30;
         int.TryParse(objSysOption.Value, out mintHAofflineTimeoutInSeconds);

         //mobjSystemOptionsService.GetSystemOption(applicationName: "");
         ViewBag.HAofflineTimeoutInSeconds = mintHAofflineTimeoutInSeconds;
         ViewBag.IsHAEnabled = mobjDigEnvironmentService.IsHAEnabled;
         return PartialView("HAMonitor");
      }

      public JsonResult ReadHA([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHAMonitorView, CurrentUser))
         {
            IEnumerable<ServiceStatus> objAll = mobjSTManager.GetServiceStatuses();

            DataSourceResult data = objAll.ToDataSourceResult(request, model => ServiceStatusViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult ActivateHANode(string Application, string Host)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHAMonitorActivate, CurrentUser))
         {
            mobjSTManager.SetActive(Application, Host);
            return Json(new { errorMessage = string.Empty, success = true }); ;
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public ActionResult PrivacyLogout()
      {
         string product = "DIGISTAT";
         IProductVersionCore prod = mobjDigEnvironmentService.GetProductVersion();
         if (prod != null)
         {
            product = prod.Product;
         }

         ViewBag.Product = product;
         ViewBag.SitePath = "Actions > Privacy Logout";
         return View();
      }

      public ActionResult Shutdown()
      {
         string product = "DIGISTAT";
         IProductVersionCore prod = mobjDigEnvironmentService.GetProductVersion();
         if (prod != null)
         {
            product = prod.Product;
         }

         ViewBag.Product = product;
         ViewBag.SitePath = $"Actions > Shut down every {product}";
         return View();
      }

      public ActionResult ChangeMessageCenter()
      {
         string product = "DIGISTAT";
         IProductVersionCore prod = mobjDigEnvironmentService.GetProductVersion();
         if (prod != null)
         {
            product = prod.Product;
         }

         ViewBag.Product = product;
         ViewBag.MessageCenter = mobjDigistatConfig.MessageCenter;
         ViewBag.Istance  = mobjDigistatConfig.MessageCenterInstance;
         ViewBag.SitePath = "Actions > Change MessageCenter";
         return View();
      }

      #region ClinicalLog

      public ActionResult ClinicalLog()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalLogView, CurrentUser))
         {
            ViewBag.SitePath = "Actions > Clinical Log";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult ReadClinicalLogs([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalLogView, CurrentUser))
         {
            IQueryable<Digistat.FrameworkStd.Model.ClinicalLogQueue> objAll = mobjClqManager.GetClinicalLogs();

            try
            {
               DataSourceResult data = objAll.Include(i=>i.Patient).ToDataSourceResult(request, model => model);
               //Console.WriteLine(data.Data.to);
               return new JsonResult(data);
            }
            catch (Exception e)
            {
               mobjLog.ErrorException(e,"Error on {0} ->",MethodBase.GetCurrentMethod().Name);
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      public class ClinicalLogPriority { public string Priority { get; set; } }
      public IActionResult ReadClinicalLogsPriority()
      {
         IEnumerable<String> objAll = mobjClqManager.GetDistinctClinicalLogsPriority();
         try
         {
            List<ClinicalLogPriority> al = new List<ClinicalLogPriority>();
            foreach (string s in objAll) al.Add(new ClinicalLogPriority(){Priority = s});
            
            return new JsonResult(al.ToDataSourceResult(new DataSourceRequest()));
         }
         catch (Exception e)
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public class ClinicalLogTask { public string Task { get; set; } }
      public IActionResult ReadClinicalLogsTask()
      {
         
         IEnumerable<String> objAll = mobjClqManager.GetDistinctClinicalLogsTask();
         try
         {
            List<ClinicalLogTask> al = new List<ClinicalLogTask>();
            foreach (string s in objAll) al.Add(new ClinicalLogTask(){Task = s});
            
            return new JsonResult(al.ToDataSourceResult(new DataSourceRequest()));
         }
         catch (Exception e)
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      #endregion
      #region DigistatLog

      

      
      public ActionResult DigistatLog()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDigistatLogView, CurrentUser))
         {
            ViewBag.SitePath = "Actions > Digistat Log";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult ReadDigistatLogs([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDigistatLogView, CurrentUser))
         {
            
            try
            {
               IQueryable<Digistat.FrameworkStd.Model.Log> objAll = mobjDigLogManager.GetDigistatLogs();

               DataSourceResult data = objAll.ToDataSourceResult(request, model => model);
               //Console.WriteLine(data.Data.to);
               return new JsonResult(data);
            }
            catch (Exception e)
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_RECORD_FOUND), success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      public class DigistatLogPriority { public string Priority { get; set; } }
      public IActionResult ReadDigistatLogsPriority()
      {
         IEnumerable<String> objAll = mobjDigLogManager.GetDistinctDigistatLogsPriority();
         try
         {
            List<DigistatLogPriority> al = new List<DigistatLogPriority>();
            foreach (string s in objAll) al.Add(new DigistatLogPriority(){Priority = s});
            
            return new JsonResult(al.ToDataSourceResult(new DataSourceRequest()));
         }
         catch (Exception e)
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public class DigistatLogTask { public string Task { get; set; } }
      public IActionResult ReadDigistatLogsTask()
      {
         
         IEnumerable<String> objAll = mobjDigLogManager.GetDistinctDigistatLogsTask();
         try
         {
            List<DigistatLogTask> al = new List<DigistatLogTask>();
            foreach (string s in objAll) al.Add(new DigistatLogTask(){Task = s});
            
            return new JsonResult(al.ToDataSourceResult(new DataSourceRequest()));
         }
         catch (Exception e)
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      #endregion
      
      public ActionResult MessageFlow()
      {
         ViewBag.SitePath = "Actions > Message Flow";
         return View();
      }
   }
   
}

