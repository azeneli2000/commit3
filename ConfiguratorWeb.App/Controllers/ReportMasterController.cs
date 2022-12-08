using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Std.BL.ReportMaster;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Xml;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Filters;

namespace ConfiguratorWeb.App.Controllers
{
    public class ReportMasterController : DigistatWebControllerBase
   {
      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private readonly IPermissionsService mobjPermSvc;
      private ILoggerService mobjLog;
      private IReportTemplateManager mobjReport;


      public ReportMasterController(IDigistatConfiguration config, IMessageCenterService msgcenter,
      ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc,
      IReportTemplateManager repSvc,IPermissionsService permSvc
        )
      : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjReport = repSvc;
         mobjPermSvc = permSvc;
      }
      public IActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > Common > Report Master Templates";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      #region Report Master
      public IActionResult Templates()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > Common > Report Master Templates";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
   

      public JsonResult ReadReportTemplates([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            IQueryable<ReportTemplate> objAllTemplates = mobjReport.GetQueryable().Where(a => a.Current && !a.ValidToDate.HasValue);

            DataSourceResult data = objAllTemplates.ToDataSourceResult(request, model => ReportTemplateViewModelBuilder.Build(model));

            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      
      public JsonResult GetReportTemplatesName([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            IQueryable<ReportTemplate> objAllTemplates = mobjReport.GetQueryable().Where(a => a.Current && !a.ValidToDate.HasValue);

            DataSourceResult data = objAllTemplates.ToDataSourceResult(request, model => ReportTemplateViewModelBuilder.Build(model));

            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      public ActionResult GetReportTemplate(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            ReportTemplateViewModel model = new ReportTemplateViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
               model = ReportTemplateViewModelBuilder.Build(mobjReport.Get(id));
            }

            return PartialView("_ReportTemplate", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult UploadReportFile(string Id, IFormFile file)
      {
         bool bolSuccess = false;
         string strMessageError = "";
         string strFileName = "";
         string strContent = "";
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateEdit, CurrentUser))
            {

               StringBuilder result = new StringBuilder();
               if (file == null)
               {
                  throw new Exception("No files received");
               }
               else
               {
                  using (var reader = new StreamReader(file.OpenReadStream()))
                  {
                     while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                  }
               }
               //ReportTemplate entity = mobjReport.Get(Id);
               //entity.Stream = result.ToString();
               // mobjReport.Update(entity);
               strFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
               bolSuccess = true;
               strContent = result.ToString();
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         catch (Exception ex)
         {
            bolSuccess = false;
            strMessageError = ex.Message;
         }

         return Json(new { errorMessage = strMessageError, success = bolSuccess, fileUploaded= strFileName, fileContent=strContent });//RedirectToAction("Drivers");
      }

      public ActionResult Upload(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateEdit, CurrentUser))
         {
            ReportTemplateViewModel model = new ReportTemplateViewModel();
            if (id != Guid.Empty.ToString())
            {
               model = ReportTemplateViewModelBuilder.Build(mobjReport.Get(id));
            }

            return PartialView("_UploadReportWindow", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult DownloadReportFile(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            ReportTemplateViewModel model = new ReportTemplateViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
               model = ReportTemplateViewModelBuilder.Build(mobjReport.Get(id));
            }

            // return File(model.Stream, System.Net.Mime.MediaTypeNames.Application.Octet, model.Filename);

            if (string.IsNullOrWhiteSpace(model.Filename))
               return Content("Filename not present");
            //if (string.IsNullOrWhiteSpace(model.Stream))
            //   return Content("File content is empty");

            byte[] content = null;
            using (var memory = new MemoryStream(Encoding.UTF8.GetBytes(model.Stream ?? ""))) 
            {
               memory.Position = 0;
               content = memory.ToArray();
            }
               
            return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, model.Filename);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      [DisableRequestSizeLimit]
      [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
      public JsonResult SaveReportDetail(ReportTemplateViewModel model)
      {
         string messageError = string.Empty;
         ReportTemplate reportTemplate = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateEdit, CurrentUser))
            {
               if (model.ID == Guid.Empty) //create
               {
                  reportTemplate = mobjReport.Create(ReportTemplateEntityBuilder.Build(model));
               }
               else //update
               {
                  reportTemplate = mobjReport.Update(ReportTemplateEntityBuilder.Build(model));
               }
               if (reportTemplate != null)
               {
                  bolSuccess = true;
               }
            }
             else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });

         }
         catch (Exception e)
         {
            return Json(new { errorMessage = e.Message, success = bolSuccess });
         }

      }

      [HttpPost]
      public ActionResult DeleteReportTemplate([DataSourceRequest] DataSourceRequest request, ReportTemplateViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateEdit, CurrentUser))
            {
               mobjReport.Remove(model.ID.ToString());
            }
            else
            {
               ModelState.AddModelError("", CommonStrings.NO_VALID_PERMISSION);
            }

         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json(new { errorMessage = string.Empty, success = true });
      }


    

      [HttpPost]
      // call-back for save the designed report 
      public ActionResult SaveDesignedReport(string reportID, string reportUUID)
      {
         ViewBag.Message = String.Format("Confirmed {0} {1}", reportID, reportUUID);

         //if (reportID == "DesignReport")
         //{
         //   //Save report in cache

         //   // If the report is saved to the server /*************************************/

         //   Stream reportForSave = Request.InputStream;

         //  // string pathToSave = Server.MapPath("~/App_Data/DesignedReports/test.frx");


         //   using (FileStream file = new FileStream(pathToSave, FileMode.Create))
         //   {
         //      reportForSave.CopyTo(file);
         //   }
         //   /*************************************/
         //}
         return View();

      }

      public JsonResult ReadReportTemplateParams(string templateName)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
         {
            Dictionary<string,string> dicOfParameters= new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(templateName))
            {
               ReportTemplate model =  mobjReport.GetReportTemplates().FirstOrDefault(w=>w.Name==templateName);
               if (model!= null)
               {
                  dicOfParameters = mobjReport.GetParametersFromTemplateXml(model.Stream);
               }
            }

            return Json(dicOfParameters.Select(s=>new {Name=s.Key,Value=s.Value }).ToList());
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      
      #endregion

   }


}