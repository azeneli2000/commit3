using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
//using System.Web.Helpers;
using Configurator.Std.BL;
using Configurator.Std.BL.Hubs;
using Configurator.Std.BL.Monitoring;
using Configurator.Std.BL.ReportMaster;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Extensions;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.SystemMonitoring;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Monitoring;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ConfiguratorWeb.App.Controllers
{

   //[System.Web.Mvc.RoutePrefix("IT Supervision/Dashboard")]
   public class SystemMonitoringController : DigistatWebControllerBase
   {
      private const string SMARTSV = "SMARTSUPERVISOR";
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly INetworksManager mobjNetworksManager;
      private readonly IPermissionsService mobjPermSvc;
      IMonitoringRawResponseManager mobjMonRespMan;
      IMonitoringRawRequestManager mobjMonReqMan;
      IUserReportManager mobjUsrRep;
      IReportTemplateManager mobjrepoRep;
      IMonitoringResultManager mobjMonResult;
      IReportGeneratorService mobjMonReport;
      IReportTemplateManager mobjMonReportTemplate;

      JsonSerializerSettings mobJsonSerializerSettings;   

      public SystemMonitoringController(IDigistatConfiguration config, IMessageCenterService msgcenter,
        ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
        ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IMessageCenterManager msgCtrMgr,
        IPermissionsService permSvc, INetworksManager netMgr, IReportTemplateManager repoRep,
      IMonitoringRawResponseManager monRespMan, IMonitoringRawRequestManager monReqMan, IUserReportManager usrRep, IMonitoringResultManager monResuMan, IReportGeneratorService reportRep, IReportTemplateManager reportRepTemplate)
     : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjNetworksManager = netMgr;
         mobjPermSvc = permSvc;
         mobjMonRespMan = monRespMan;
         mobjMonReqMan = monReqMan;
         mobjUsrRep = usrRep;
         mobjrepoRep = repoRep;
         mobjMonResult = monResuMan;
         mobjMonReport = reportRep;
         mobjMonReportTemplate = reportRepTemplate;
         mobJsonSerializerSettings = new JsonSerializerSettings();
         mobJsonSerializerSettings.ContractResolver = new DefaultContractResolver();
         mobJsonSerializerSettings.Converters.Add(new StringEnumConverter());
      }

      [Route("ITSupervision/Dashboard")]
      [ApiExplorerSettings(IgnoreApi = true)] //DO NOT REMOVE 
      public IActionResult MainMonitoring()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSupervisionMenuView, CurrentUser))
         {
            ViewBag.SitePath = "IT Supervision > Dashboard";
            return View("MainMonitoringView");
         }
         else
         {
            return View("NotAuthorized");
         }

      }

      public IActionResult UserIssuesMonitoring()
      {
         return PartialView("_UserIssuesMonitoring");
      }

      public IActionResult ReportingMonitoring()
      {
         return PartialView("_ReportingMonitoring");
      }

      public JsonResult ReadFilteredUiItems([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            IQueryable<UserReport> ret = mobjUsrRep.GetUserReports(request.Page,request.PageSize).OrderBy(a => a.Id);

            //IDictionary<string, string> remapping = new Dictionary<string, string>();
            //remapping.Add("PatientName", "PatientFullName");
            //remapping.Add("Description", "ReportDescription");
            //request = request.FilterAttributesMapping(remapping);


            foreach (var fdc in request.Filters.ToFilterDescriptor())
            {

               if (fdc.Member.Equals("PatientName"))
               {
                  fdc.Member = "PatientFullName";
               }
               else
               if (fdc.Member.Equals("Description"))
               {
                  fdc.Member = "ReportDescription";
               }
            }

            foreach (var ord in request.Sorts)
            {
               if (ord.Member.Equals("PatientName"))
               {
                  ord.Member = "PatientFullName";
               }
               else
               if (ord.Member.Equals("Description"))
               {
                  ord.Member = "ReportDescription";
               }
            }

            DataSourceResult datares = ret.ToDataSourceResult(request, model => UserReportViewModelBuilder.Build(model));
            return new JsonResult(datares, mobJsonSerializerSettings);

         }
         catch (Exception exc)
         {
            mobjLogSvc.ErrorException(exc, "Error retrieving user reports");
            return Json(new { errorMessage = exc.Message, success = false });
         }
      }

      public JsonResult ReadReportingItems([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            //IEnumerable<MonitoringDesktop> ret = mobjMonRespMan.GetAvaliableDesktop();

            IQueryable<ReportTemplate> ret = mobjrepoRep.GetReportTemplatesMonitoring();
            DataSourceResult datares = ret.ToDataSourceResult(request, model => ReportTemplateViewModelBuilder.Build(model));
            return new JsonResult(datares, mobJsonSerializerSettings);

         }
         catch (Exception exc)
         {
            mobjLogSvc.ErrorException(exc, "Error retrieving report templates");
            return Json(new { errorMessage = exc.Message, success = false });
         }
      }

      public JsonResult ReadDesktopItems([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            List<MonitoringDesktop> ret = mobjMonRespMan.GetAvaliableDesktop().ToList();
            foreach (var y in ret)
            {
               if (y.isNetworkEnabled)
                  y.Status = StatusNetwork.ACTIVE;
               else
                  y.Status = StatusNetwork.INACTIVE;
            }
            DataSourceResult datares = ret.ToDataSourceResult(request);
            return new JsonResult(datares, mobJsonSerializerSettings);

         }
         catch (Exception exc)
         {
            mobjLogSvc.ErrorException(exc, "Error retrieving report templates");
            return Json(new { errorMessage = exc.Message, success = false });
         }
      }

      public JsonResult GetMonitoringResults([DataSourceRequest] DataSourceRequest request,bool value)
      {
         try { 
         IQueryable<MonitoringResult> model = null;
         if(value)
             model = mobjMonResult.GetLastMonitoringResultsCheckFilter(true);
         else
            model = mobjMonResult.GetLastMonitoringResultsCheckFilter(false);

         foreach (var fdc in request.Filters.ToFilterDescriptor())
         {

            if (fdc.Member == "mre_Value")
            {
               if (fdc.Value.ToString() == "No Errors")
               {
                  fdc.Value = "";
                  if (fdc.Operator.Equals(Kendo.Mvc.FilterOperator.Contains) || fdc.Operator.Equals(Kendo.Mvc.FilterOperator.StartsWith))
                  {
                     fdc.Operator = Kendo.Mvc.FilterOperator.IsEqualTo;
                  }
                  if (fdc.Operator.Equals(Kendo.Mvc.FilterOperator.DoesNotContain))
                  {
                     fdc.Operator = Kendo.Mvc.FilterOperator.IsNotEqualTo;
                  }
               }

            }
         }

         DataSourceResult data = model.ToDataSourceResult(request);
         return new JsonResult(data, mobJsonSerializerSettings);
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error retrieving Monitoring Result");
            return Json(new { errorMessage = e.Message, success = false });
         }
      }
      [HttpGet]
      public IActionResult CurrentUiItem(int selectedItem)
      {
         try
         {
            UserReport objSingleUserIssuesItem = mobjUsrRep.GetUserReportById(selectedItem);
            UserReportViewModel model = UserReportViewModelBuilder.Build(objSingleUserIssuesItem);
            return PartialView("_UserIssuesDetails", model);
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error retrieving user report with id {selectedItem}");
            return StatusCode(StatusCodes.Status403Forbidden);
         }

      }




      [HttpGet]
      public IActionResult InstantiateChart(string nameAppliaction, string indicator, string component, int hour, double diffHours)
      {
         try
         {
            ChartModel model = new ChartModel();
            model.nameAppliaction = nameAppliaction;
            model.indicator = indicator;
            model.component = component;
            model.hour = hour;
            model.LastUpdateIndicator = diffHours;
            return PartialView("_MonitoringChart", model);
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error while instatiating chart for Application= {nameAppliaction}, Component= {component}, Indicator= {indicator}, Hour= {hour}");
            return StatusCode(StatusCodes.Status500InternalServerError);
         }

      }

      [HttpPost]
      public JsonResult GetChartIndicator([DataSourceRequest] DataSourceRequest request, ChartModel model, int hour)
      {
         try
         {

            string endDate = DateTime.UtcNow.AddHours(-hour).ToString("yyyy-MM-dd HH:mm tt");

            Dictionary<DateTime, double> objDbResult = mobjMonRespMan.GetNodeValueChart(model.nameAppliaction, model.component, model.indicator, endDate);
            objDbResult.Keys.OrderByDescending(x => x.Hour);
   
            foreach (var yy in objDbResult)
            {
               ChartModel2 singleitem = new ChartModel2();
               singleitem.date = yy.Key.ToString("o");
               singleitem.value = yy.Value;
               model.chart.Add(singleitem);
            }

            DataSourceResult datares = model.chart.ToDataSourceResult(request);



            return new JsonResult(datares, mobJsonSerializerSettings);

         }
         catch (Exception exc)
         {
            mobjLogSvc.ErrorException(exc, $"Error retrieving chart data for Application= {model.nameAppliaction}, Component= {model.component}, Indicator= {model.indicator}, Hour= {model.hour}");
            return Json(new { errorMessage = exc.Message, success = false });
         }

      }

      [HttpGet]
      public ActionResult DownloadSnapshot(int Id)
      {
         try
         {
            var img = mobjUsrRep.GetSnapshot(Id);
            return File(img, "application/octet-stream", "snapshot.png");
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error retrieving snapshot with ID= {Id}");
            return StatusCode(StatusCodes.Status500InternalServerError);
         }
      }

      [HttpPost]
      public IActionResult EditDetail(UserReportViewModel model)
      {
         try
         {
            DateTime newDate = DateTime.Now;
            mobjUsrRep.UpdateUserReport(model.Id, (short)model.Status, newDate, model.StatusNote);

            return Json(new { errorMessage = "", success = true });
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error editing details for user report with Id= {model.Id}");
            return Json(new { errorMessage = "Error in Update Operation", success = false });
         }

      }

      public ActionResult MainView()
      {

         return PartialView("_mainViewDefinitive");

      }

      public JsonResult GetServers([DataSourceRequest] DataSourceRequest request)
      {
         //TODO: query filters
         try
         {
            int reqFreqMin = 2;
            if (int.TryParse(mobjSysOptSvc.GetSystemOption(SMARTSV, string.Empty, string.Empty, string.Empty, "MonitoringServerComponentsInterval", false)?.Value, out var sysReqFreq))
            {
               reqFreqMin = sysReqFreq;
            }
            var model = MonitoringInfo.BuildList(mobjMonRespMan.GetLastMonitoringInfo(MonitoringData.MonitoringType.SystemInfo, reqFreqMin, out DateTime dtmLastRequest));

            //DataSourceResult data = model.ToDataSourceResult(request);
            //return new JsonResult(data);
            var dtNextUpdate = dtmLastRequest.AddMinutes(reqFreqMin);
            var data = Json(new { result = new { Data = model, NextUpdate = dtNextUpdate, UpdateFreq = reqFreqMin, success = true } }, mobJsonSerializerSettings);
            return data;
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error Retrieving Servers Data");
            return Json(new { errorMessage = "Error Retrieving Servers Data", success = false });
         }
      }
      public JsonResult GetServices([DataSourceRequest] DataSourceRequest request)
      {
         //TODO: query filters
         try
         {


            if (!mobjSysOptSvc.CheckIfSystemOptionApplicationIsLoaded(SMARTSV))
            {
               mobjSysOptSvc.ReloadSystemOptions(SMARTSV);
            }

            var objOptionAlwaysOn = mobjSysOptSvc.GetSystemOption(SMARTSV, string.Empty, string.Empty, string.Empty, "AlwaysOnServicesList", false);
            int reqFreqMin = 2;
            if (int.TryParse(mobjSysOptSvc.GetSystemOption(SMARTSV, string.Empty, string.Empty, string.Empty, "MonitoringServerComponentsInterval", false)?.Value, out var sysReqFreq))
            {
               reqFreqMin = sysReqFreq;
            }

            List<MonitoringInfo> model = MonitoringInfo.BuildList(mobjMonRespMan.GetLastMonitoringInfo(MonitoringData.MonitoringType.Service, reqFreqMin, out DateTime dtmLastRequest)).ToList();
            if (objOptionAlwaysOn != null && !string.IsNullOrEmpty(objOptionAlwaysOn.Value))
            {
               string[] services = objOptionAlwaysOn.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);

               foreach (string service in services)
               {
                  var obj = model.Where(a => a.Data.Name.Equals(service, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();


                  if (obj == null)
                  {
                     //add
                     var monData = new MonitoringData();
                     monData.Name = service;
                     monData.Type = MonitoringData.MonitoringType.Service;
                     monData.Anomalies.Add(new Anomaly("NO_DATA", Anomaly.SeverityValue.Error, $"{service} is not responding, verify the log file in the application folder"));
                     model.Add(new MonitoringInfo(monData));
                  }
               }

            }

            model.Sort((x, y) => x.StatusValue - y.StatusValue);
            //DataSourceResult data = model.ToDataSourceResult(request);
            //return new JsonResult(data);
            var dtNextUpdate = dtmLastRequest.AddMinutes(reqFreqMin);
            var data = Json(new { result = new { Data = model, NextUpdate = dtNextUpdate, UpdateFreq = reqFreqMin, success = true } }, mobJsonSerializerSettings);

            return data;

         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error Retrieving Services Data");
            return Json(new { errorMessage = "Error Retrieving Services Data", success = false });
         }
      }

      public ActionResult ServerView()
      {

         return PartialView("_ServerMonitoring");


      }


      [HttpGet]
      public ActionResult GetMonitoringReport(string Id)
      {
         try
         {

            var objparameters = new Dictionary<string, object>();
            string strCurrUser = CurrentUser != null ? CurrentUser.Id : "N/A";
            //objparameters.Add("USER", strCurrUser);

            var objReport = mobjMonReportTemplate.Get(Id);
            MemoryStream objStream = mobjMonReport.GenerateReport(objReport.Name, objparameters, objReport.Application, Digistat.FrameworkStd.Enums.ReportExportFormats.PDF);
      
            return File(objStream, "application/pdf", "SupervisionTemplate.pdf");

         }
         catch (Exception ex)
         {
            return null;
         }


      }


      [HttpGet]
      public IActionResult GetDesktopHWInfoPartial(string Hostname)
      {

         ViewData["Hostname"] = Hostname;
         return PartialView("_DesktopHWInfo");

      }

      public JsonResult GetHostnameHWInfo([DataSourceRequest] DataSourceRequest request, string hostname)
      {
         try
         {
            var objMonitoringInfo = MonitoringInfo.Build(mobjMonRespMan.GetLastMonitoringInfoByHostname(hostname));
            var tempList = new List<MonitoringInfo>();
            tempList.Add(objMonitoringInfo);
            DataSourceResult data = tempList.ToDataSourceResult(request);
            return new JsonResult(data, mobJsonSerializerSettings);
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error Retrieving Hostname -{hostname}- HW Info");
            return Json(new { errorMessage = e.Message, success = false });
         }
      }

    

      //public JsonResult CheckHostinNetwork(string hostname)
      //{
      //   try
      //   {
      //      var data = mobjMonRespMan.CheckHostAvaliable(hostname);

      //      return new JsonResult(data);
      //   }
      //   catch (Exception e)
      //   {
      //      mobjLogSvc.ErrorException(e, $"Error Retrieving Hostname -{hostname}- HW Info");
      //      return Json(new { errorMessage = e.Message, success = false });
      //   }
      //}

   }
}
