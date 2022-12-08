using Configurator.Std.BL.Hubs;
using ConfiguratorWeb.App.Filters;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkWebExtensions.Attributes;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Configurator.Std.BL.ExportScheduler;
using Configurator.Std.BL.ReportMaster;
using CronExpressionDescriptor;
using Cronos;
using Digistat.FrameworkStd.Model.Export;
using Digistat.FrameworkStd.UMSLegacy;
using FastReport;
using FastReport.Data;
using FastReport.Utils;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ConfiguratorWeb.App.Controllers
{
   [DigConfigFilterAttribute]
   [DigistatAuthFilterAttribute]
   public class ExportSchedulerController : DigistatWebControllerBase
   {

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IExportJobsManager mobjExportJobsManager;

      private readonly IPermissionsService mobjPermSvc;
      private readonly IReportTemplateManager mobjReport;


      public ExportSchedulerController(IDigistatConfiguration config, IMessageCenterService msgcenter,
         ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
         ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IMessageCenterManager msgCtrMgr,
         IPermissionsService permSvc, IReportTemplateManager repSvc,
         IExportJobsManager exportJobsManager
         )
         : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjPermSvc = permSvc;
         mobjExportJobsManager = exportJobsManager;
         mobjReport = repSvc;
      }


      public IActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > Export Scheduler > Jobs";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }


      public JsonResult ReadJobs([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerView, CurrentUser))
         {
            IEnumerable<ExportJobs> objAllValidationGroups = mobjExportJobsManager.GetAll();
            DataSourceResult data = objAllValidationGroups.ToDataSourceResult(request,
               model => model /*ValidationGroupViewModelBuilder.Build(model)*/);
            return new JsonResult(data);
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }

      }

      public IActionResult JobDetail(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerEdit, CurrentUser))
         {
            ExportJobs model = new ExportJobs();

            if (id > 0)
            {
                model = mobjExportJobsManager.GetById(id);
               //model = ValidationGroupViewModelBuilder.Build(objValGroup);
            }

            try
            {
               //List<ReportTemplate> objAllTemplates = mobjReport.GetReportTemplates().Where(a => a.Current && !a.ValidToDate.HasValue).ToList();
               var listName = new List<string>();
               //if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
               //{
                  //IEnumerable<ReportTemplate> objAllTemplates 
                  listName = mobjReport.GetQueryable().Where(a => a.Current && !a.ValidToDate.HasValue).Select(s=>s.Name).ToList();
               //}
               ViewData["ReportTemplatesListName"] = listName;
            }
            catch (Exception e)
            {
               ViewData["ReportTemplatesListName"] = new List<string>();
            }
            try
            {
               return View("_JobTabs", model);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false});
            }
            
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }
      public IActionResult JobExport([FromRoute]int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerEdit, CurrentUser))
         {
            ExportJobs model = new ExportJobs();

            if (id > 0)
            {
                model = mobjExportJobsManager.GetById(id);
                if (model.exj_ID >0)
                {
                   model.exj_LastRunDateTime = null;
                   model.exj_LastRunMessage = String.Empty;
                   model.exj_LastRunStatusCode = null;
                   model.exj_LastUpdate = null;
                   model.exj_UserID = String.Empty;
                   if (!String.IsNullOrWhiteSpace(model.exj_FileSystemPassword))
                   {
                      model.exj_FileSystemPassword = UMSFrameworkParser.Base64Encode(UMSFrameworkParser.Encrypt(model.exj_FileSystemPassword,string.Empty));
                   }
                }
               //model = ValidationGroupViewModelBuilder.Build(objValGroup);
            }


            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject( model)));
            return  File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, $"Job_{id}_{DateTime.Now.ToString("s")}.json");

         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }
      [RequestFormSizeLimit(valueCountLimit: 200000)]
      public ActionResult JobImport([FromRoute]int id,IEnumerable<IFormFile>  files )
      {

         try
         {
            if (!mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerEdit, CurrentUser))
            {
               throw new Exception(mobjDicSvc.XLate("Import function is not allowed."));
            }
            ExportJobs model = new ExportJobs();
            if (files != null)
            {
               
               foreach (var file in files)
               {
                  var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                  if (file.Length > 0)
                  {
                     var sr = new StreamReader(file.OpenReadStream());
                     var y = sr.ReadToEnd();
                     try
                     {
                        model = JsonConvert.DeserializeObject<ExportJobs>(y);
                        model.exj_ID= id;
                        model.exj_LastRunDateTime = null;
                        model.exj_LastRunMessage = String.Empty;
                        model.exj_LastRunStatusCode = null;
                        model.exj_LastUpdate = null;
                        model.exj_UserID = String.Empty;
                        if (!String.IsNullOrWhiteSpace(model.exj_FileSystemPassword))
                        {
                           model.exj_FileSystemPassword = UMSFrameworkParser.Decrypt(UMSFrameworkParser.Base64Decode(model.exj_FileSystemPassword),string.Empty);
                        }
                     }
                     catch (Exception e)
                     {
                        Console.WriteLine(e);
                        throw;
                     }
                     
                  }

                  
               }
            }

            try
            {
               var listName = new List<string>();
               listName = mobjReport.GetQueryable().Where(a => a.Current && !a.ValidToDate.HasValue).Select(s=>s.Name).ToList();
               ViewData["ReportTemplatesListName"] = listName;
            }
            catch (Exception e)
            {
               ViewData["ReportTemplatesListName"] = new List<string>();
            }
            try
            {
               return View("_JobTabs", model);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false});
            }
         }
         catch (Exception)
         {
            throw;
         }


      }


      public IActionResult JobHistory([DataSourceRequest] DataSourceRequest request,int jobId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerView, CurrentUser))
         {
            List<ExportJobsHistory> model = new List<ExportJobsHistory>();

            try
            {
               List<ExportJobsHistory> jobsHistories = mobjExportJobsManager.GetHistoryById(jobId).OrderByDescending((a)=>a.exjh_StartDateTime ).ToList();
               //return Json(new {data=model, success = true});
               //DataSourceResult data = jobsHistories.ToDataSourceResult(request,
               //   model => model /*ValidationGroupViewModelBuilder.Build(model)*/);
               //return new JsonResult(data);
               return new JsonResult(jobsHistories);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false});
            }
            
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }

      public PartialViewResult LoadGeneralTabView()
      {
          return PartialView("_JobGeneral");
      }


      #region Save


      [RequestFormSizeLimit(valueCountLimit: 20000)]
      [HttpPost]
      public JsonResult SaveJobDetail(ExportJobs model)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerEdit, CurrentUser))
            {

               //   if (!string.IsNullOrWhiteSpace(model.ValidationParameterSerialized))
               //   {
               //      model.Parameters = JsonConvert.DeserializeObject<ICollection<ValidationParameterViewModel>>(model.ValidationParameterSerialized);
               //   }
               //   ValidationGroup vgItem = ValidationGroupBuilder.Build(model);
               model.exj_LastUpdate = DateTime.Now;
               model.exj_UserID = CurrentUser != null ? CurrentUser.Id : string.Empty;

               
               var currentUserAbbrev = CurrentUser != null ? CurrentUser.Abbrev : string.Empty;

               bolSuccess = model.exj_ID != 0
                   ? mobjExportJobsManager.Update(model, currentUserAbbrev)
                   : mobjExportJobsManager.Create(model, currentUserAbbrev);

            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new {errorMessage = messageError, success = bolSuccess});
         }
         catch (Exception ex)
         {
            return Json(new {errorMessage = ex.Message, success = false});
         }
      }


      public JsonResult DeleteJob(int jobID)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionExportSchedulerEdit, CurrentUser))
            {
               if (jobID != 0)
               {
                  bolSuccess = mobjExportJobsManager.Delete(jobID, CurrentUser != null ? CurrentUser.Abbrev : string.Empty, CurrentUser != null ? CurrentUser.Id : string.Empty);
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new {errorMessage = messageError, success = bolSuccess});
         }
         catch (Exception ex)
         {
            return Json(new {errorMessage = ex.Message, success = false});
         }
      }

      #endregion


      public IActionResult TestQuery(string query)
      {
         bool bRet=false;
         string errorMessage = "";
         string result = "";
         try
         {
            result = mobjExportJobsManager.TestQuery(query);
            bRet = true;
         }
         catch (Exception e)
         {
            errorMessage = mobjDicSvc.XLate(e.Message);
         }

         return new JsonResult(new { success = bRet, errorMessage = errorMessage ,result=result});
      }

      public IActionResult CheckCronString(string query)
      {
         bool bRet=true;
         string errorMessage = "";
         string result = "";
         try
         {
            var currentTimeUtc = DateTimeOffset.UtcNow.AddSeconds(-20);
            var timeZone = TimeZoneInfo.Local;
            string[] aSchedule = query.Split(";");
            foreach (string schedule in aSchedule)
            {
               if (schedule.Trim().Length ==0)
               {
                  continue;
               }
               try
               {
                  var crontabSchedule = CronExpression.Parse(schedule, CronFormat.Standard);
                  var cc = crontabSchedule.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Local);
                  string description = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(schedule, new Options(){Locale = "en-GB"});
                  //mobjLogSvc.Debug(string.Format("job '{1}'({0}) next time:{2}",cronExpression, job.exj_Name, cc));
                  result += "{4}{0} -> {1} ({2}: {3})".FormatWith(schedule,description,mobjDicSvc.XLate("Next occurrency"), cc.ToString(),result.Length>0?";":"");
               }
               catch (Exception e)
               {
                  bRet = false;
                  result = "";
                  errorMessage = "{0} '{1}' -> {2};".FormatWith( mobjDicSvc.XLate("Error parsing string"),schedule, mobjDicSvc.XLate(e.Message));
                  break;
                  
               }
               
            }
            
         }
         catch (Exception e)
         {
            errorMessage = mobjDicSvc.XLate(e.Message);
         }

         return new JsonResult(new { success = bRet, errorMessage = errorMessage ,result=result.Split(";")});


      }

      public IActionResult GetReportParameters(string templateName)
      {
         
         var objReturn = new CallResult();
         try
         {
            ReportTemplate objReportTemplate =  mobjReport.GetReportTemplates().FirstOrDefault(w=>w.Name==templateName);
            if (objReportTemplate != null)
            {
               RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
               Report objReport = new Report();
               objReport.Report.LoadFromString(objReportTemplate.Stream);
               var objReportParameters = new List<string>();
               foreach (FastReport.Data.Parameter rptParam in objReport.Parameters)
               {
                  if (rptParam != null)
                  {
                     if (!objReportParameters.Contains(rptParam.Name))
                     {
                        objReportParameters.Add(rptParam.Name);
                     }
                  }
               }
               
               objReturn.Result = objReportParameters;
               objReturn.Success = true;
            }
            else
            {
               objReturn.ErrorMessage = mobjDicSvc.XLate("No report founded");
            }

            

         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e,$"GetReportParameters in error:{e.Message}");
            return BadRequest(objReturn.ErrorMessage??e.Message);
         }

         return Ok(objReturn);
      }
   }
}



