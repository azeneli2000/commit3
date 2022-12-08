using Configurator.Std.BL.Mobile;
using Configurator.Std.BL.Mobile.Utils;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Configurator.Std.BL.Configurator;

namespace ConfiguratorWeb.App.Controllers.Mobile
{
   using FileCache = SelfExpiringDictionary<string, string>;
   public class ApkController : DigistatWebControllerBase
   {
      private static readonly FileCache mobjCache;
      


      static ApkController()
      {
         mobjCache = new FileCache(60 * 1000, 60 * 1000);
         mobjCache.Evict += (k, v) =>
         {
            if (mobjCache.WithValue(v).Any())
            {
               Remove(v);
            }
         };
      }
      
      //private static string TMP_OUTPUT_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"UMS\Digistat\tmp\apk");

      private readonly IMobileServiceManager mobileService;
      private readonly IPermissionsService mobjPermSvc;
      private readonly IConfiguratorWebConfiguration mobjConfconfig;

      public ApkController(
         IDigistatConfiguration config,
         IMessageCenterService msgcenter,
         ISynchronizationService syncSvc,
         IDictionaryService dicSvc,
         IDnsCacherService dnsSvc,
         ILoggerService logSvc,
         ISystemOptionsService sysOptSvc,
         IPermissionsService permSvc,
         IMobileServiceManager mobileService,
         IConfiguratorWebConfiguration confConfiguration) : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)
      {
         mobjConfconfig = confConfiguration;
         if (!Directory.Exists(mobjConfconfig.ApkCache))
            Directory.CreateDirectory(mobjConfconfig.ApkCache);
         mobjPermSvc = permSvc;
         this.mobileService = mobileService;
      }
      public ActionResult ApkUpdate()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
         {
            ViewBag.SitePath = "Mobile > APK Uploads";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult GetApkList([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
         {
            var list = new ApkRepository(mobjConfiguration).GetApkList();

            return new JsonResult(list.ToDataSourceResult(request));
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public async Task<ActionResult> Upload(IEnumerable<IFormFile> files, string metaData, string version)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
         {
            if (version == null || files == null)
            {
               return StatusCode(StatusCodes.Status400BadRequest);
            }

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(metaData));

            JsonSerializer serializer = new JsonSerializer();
            ChunkMetaData chunkData;
            using (StreamReader streamReader = new StreamReader(ms))
            {
               chunkData = (ChunkMetaData)serializer.Deserialize(streamReader, typeof(ChunkMetaData));
            }

            string path = Path.Combine(mobjConfconfig.ApkCache, chunkData.FileName);
            if (chunkData.ChunkIndex == 0)
            {
               RemoveFromUploads(path);
            }
            else
            {
               if (!System.IO.File.Exists(path))
               {
                  return StatusCode(StatusCodes.Status500InternalServerError);
               }
            }

            foreach (var file in files)
            {
               await AppendToFile(path, file);
            }

            FileResult fileBlob = new FileResult
            {
               uploaded = chunkData.TotalChunks - 1 <= chunkData.ChunkIndex,
               fileUid = chunkData.UploadUid
            };

            mobjCache.Add(fileBlob.fileUid, path);
            return Json(fileBlob);
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      [HttpPost]
      public async Task<ActionResult> Notify(ApkMetadata objData)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
         {
            string uploaded = null;
            mobjCache.Remove(objData.Session);

            try
            {
               uploaded = Path.Combine(mobjConfconfig.ApkCache, objData.Filename);
               if (await mobileService.AddApk(objData, uploaded))
               {
                  return Ok();
               }

               return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (TaskCanceledException timeout)
            {
               mobjLogSvc.ErrorException(timeout, "Request timeout for {0} {1} {2}", objData.Filename, objData.Version, objData.Session);
               return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "Cannot finalize  {0} {1} {2}", objData.Filename, objData.Version, objData.Session);
               return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
               try
               {
                  if (uploaded != null)
                  {
                     RemoveFromUploads(uploaded);
                  }
                  
               }
               catch (Exception e)
               {
                  mobjLogSvc.ErrorException(e, "Cannot remove {0}", objData.Filename);
               }
            }
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      public async Task<ActionResult> RemoveApk([DataSourceRequest] DataSourceRequest request, DigistatMobileAPK apk)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
         {
            try
            {
               if (apk == null)
               {
                  return StatusCode(StatusCodes.Status400BadRequest);
               }

               if (await mobileService.RemoveApk(apk) == false)
               {
                  mobjLogSvc.Error("Cannot remove {0}", apk.Name);
                  return StatusCode(StatusCodes.Status404NotFound);
               }
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Cannot remove {0}", apk.Name);
            }

            return Json(new[] { apk }.ToDataSourceResult(request, ModelState));
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      private async Task<bool> AppendToFile(string fullPath, IFormFile content)
      {
         try
         {
            using (FileStream stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
               await content.CopyToAsync(stream);
               return true;
            }
         }
	 //Pain and Fear
         catch (IOException)
         {
            throw;
         }
      }

      private static void ClearFolder(string folder)
      {
         DirectoryInfo di = new DirectoryInfo(folder);
         foreach (FileInfo file in di.GetFiles())
         {
            file.Delete();
         }
      }
      
      private static void RemoveFromUploads(string fullFileName)
      {
         
         Remove(fullFileName);
      }

      private static void Remove(string filePath)
      {
         if (System.IO.File.Exists(filePath))
         {
            System.IO.File.Delete(filePath);
         }

      }
   }

   class ChunkMetaData
   {
      public string UploadUid { get; set; }
      public string FileName { get; set; }
      public string RelativePath { get; set; }
      public string ContentType { get; set; }
      public long ChunkIndex { get; set; }
      public long TotalChunks { get; set; }
      public long TotalFileSize { get; set; }
   }

   class FileResult
   {
      public bool uploaded { get; set; }
      public string fileUid { get; set; }
   }
}
