using System;
using System.Collections.Generic;
using System.Linq;
using Configurator.Std.BL;
using Configurator.Std.BL.DasDrivers;
using Configurator.Std.BL.Hubs;
using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConfiguratorWeb.App.Controllers
{
   public class DigistatRepositoryController : DigistatWebControllerBase
   {
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private ILoggerService mobjLog;
      private readonly IPermissionsService mobjPermSvc;
      private readonly IDigistatRepositoryManager mobjDigRepoMgr;

      public DigistatRepositoryController(IDigistatConfiguration config, IMessageCenterService msgcenter,
          IMessageCenterManager msgCtrMgr,
          ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc
          , ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IPermissionsService permSvc
         ,IDigistatRepositoryManager digRepoMgr
         )
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjPermSvc = permSvc;
         mobjDigRepoMgr = digRepoMgr;
      }      

      // GET: Actions
      public ActionResult Index(string strID)
      {
         ViewBag.CurrentRepoID = strID;
         return View("_Index");
      }

      public JsonResult ReadDigistatRepository([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDigistatRepositoryView, CurrentUser))
         {
            IQueryable<Digistat.FrameworkStd.Model.DigistatRepository> objAll = mobjDigRepoMgr.GetQueryable();
            DataSourceResult data = objAll.ToDataSourceResult(request, model => DigistatRepositoryViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public FileResult DownloadDigistatRepository(string repoID)
      {
         DigistatRepository objRepo =  mobjDigRepoMgr.Get(repoID);
         if(objRepo!=null)
         {
            if((Digistat.FrameworkStd.Enums.DigistatRepositoryType)objRepo.Type== DigistatRepositoryType.ARCHIVE)
            {
               //Decompress archive content and returns all files
               byte[] fileContent =  mobjDigRepoMgr.UncompressArchive(objRepo.FileName,objRepo.Stream);
               return File(fileContent, "application/x-msdownload", objRepo.FileName + ".zip");
            }
            else
            {
               var fileName = objRepo.FileName;
               byte[] fileBytes = objRepo.Stream;
               return File(fileBytes, "application/x-msdownload", fileName);
            }
            
         }
         else
         {
            return null;
         }
      }

      public ActionResult DeleteDigistatRepository(string repoID)
      {
         try
         {
            mobjDigRepoMgr.Delete(repoID);
            return Json(new { errorMessage =string.Empty, success = true });
         }
         catch(Exception e)
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false });
         }
      }

      public ActionResult UploadDialog()
      {
         return View("_UploadDialog");
      }

      [RequestFormSizeLimit(valueCountLimit: 2000000)]
      public ActionResult UploadFile(IEnumerable<IFormFile> UploadFile,bool repostorearchive,string repoarchivename)
      {

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDigistatRepositoryEdit, CurrentUser))
            {

               if (UploadFile == null)
               {
                  throw new Exception("File not selected");
               }

               if (UploadFile.Count() > 1 || repostorearchive)
               {
                  if (!string.IsNullOrEmpty(repoarchivename))
                  {
                     
                     List<CachedFile> objFileList = new List<CachedFile>();
                     foreach(IFormFile f in UploadFile)
                     {
                        CachedFile objF = new CachedFile(f.FileName, ConversionsHelper.StreamToByteArray(f.OpenReadStream()));
                        objFileList.Add(objF);
                     }
                     byte[] abytArchiveContent = mobjDigRepoMgr.CreateArchiveForDigistatRepository(objFileList,repoarchivename);

                     mobjDigRepoMgr.Create(repoarchivename, abytArchiveContent,true);

                     return Json(new { success = true });
                  }
                  else
                  {
                     return Json(new { errorMessage = mobjDicSvc.XLate("Please provide a name for archive"), success = false });
                  }
               }
               else
               {
                  var fileObj = UploadFile.First();

                  var fileArray = ConversionsHelper.StreamToByteArray(fileObj.OpenReadStream());  //model.DriverFiles.Select(x => new Configurator.Std.BL.DasDrivers.CachedFile(x.FileName, x.OpenReadStream())).ToList();

                  mobjDigRepoMgr.Create(fileObj.FileName, fileArray,false);

                  return Json(new { success = true });
               }
               
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error on DigistatRepository.UploadFile" );
            return Json(new { errorMessage = mobjDicSvc.XLate(e.Message), success = false });
         }


      }


   }
}