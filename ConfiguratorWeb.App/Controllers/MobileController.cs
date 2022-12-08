
using ConfiguratorWeb.App.Models.Mobile;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Digistat.FrameworkWebExtensions.Controllers;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;
using Configurator.Std.BL;

namespace ConfiguratorWeb.App.Controllers
{
   public class MobileController : DigistatWebControllerBase
   {
      private readonly IMobileManager mobile;
      private readonly IMobileServiceManager mobileService;
      private readonly IPermissionsService mobjPermSvc;

      public MobileController(
         IDigistatConfiguration config,
         IMessageCenterService msgcenter,
         ISynchronizationService syncSvc,
         IDictionaryService dicSvc,
         IDnsCacherService dnsSvc,
         ILoggerService logSvc,
         ISystemOptionsService sysOptSvc,
         IMobileManager mobileMgr,
         IPermissionsService permSvc,
         IMobileServiceManager mobileServiceMgr) : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)
      {
         mobile = mobileMgr;
         mobileService = mobileServiceMgr;
         mobjPermSvc = permSvc;
      }

      public ActionResult Monitor()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
         {
            ViewBag.SitePath = "Mobile > Monitor";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
         
      }

      public ActionResult MonitorPartial()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
         {
            return PartialView("Monitor");
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      public async Task<IActionResult> GetDevices([DataSourceRequest] DataSourceRequest request)
      {
         return await RequestDevices(request);
      }

      [HttpPost]
      public IActionResult SendDisconnect(MobileDevice request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
            {
               if (mobile.Disconnect(request.DeviceID))
               {
                  return Ok();
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }
               
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error disconnecting device {0}", request.DeviceID);
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      [HttpPost]
      public async Task<IActionResult> GetConfiguration(string DeviceID)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
            {
               MobileConfig config = await mobile.GetConfiguration(DeviceID);
               if (config != null)
               {
                  return new JsonResult(config);
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }

         }
         catch (TaskCanceledException timeout)
         {
            mobjLogSvc.ErrorException(timeout, "Request timed out");
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Cannot send configuration");
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      [HttpPost]
      public async Task<IActionResult> SendConfiguration(MobileConfig configuration)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
            {
               if (await mobile.SetConfiguration(configuration))
               {
                  return Ok();
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }

         }
         catch (TaskCanceledException timeout)
         {
            mobjLogSvc.ErrorException(timeout, "Request timed out");
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Cannot send configuration");
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      [HttpPost]
      public IActionResult GetLogFile(LogRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
            {
               if (mobile.RequestLogs(request.DeviceID, request.Date))
               {
                  return Ok();
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error requesting logs for device {0}", request.DeviceID);
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      [HttpPost]
      public async Task<IActionResult> SendCommunication(Communication request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
            {
               if (await mobile.SendCommunication(request))
               {
                  return Ok();
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }
         }
         catch (TaskCanceledException timeout)
         {
            mobjLogSvc.ErrorException(timeout, "Request timed out for device {0}", request.DeviceID);
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Cannot send message to device {0}", request.DeviceID);
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      [HttpPost]
      public async Task<IActionResult> DoSystemCheck(MobileDevice request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
            {
               var sysStatus = await mobile.RequestSystemStatus(request.DeviceID);
               if (sysStatus != null)
               {
                  return new JsonResult(sysStatus);
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }
         }
         catch (TaskCanceledException timeout)
         {
            mobjLogSvc.ErrorException(timeout, "Request timed out for device {0}", request.DeviceID);
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Cannot send syschk to device {0}", request.DeviceID);
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

        [HttpPost]
        public async Task<IActionResult> DoSourceRef(MobileDevice request)
        {
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
                {
                    var sourceRef = await mobile.RequestSourceRef(request.DeviceID);
                    if (sourceRef != null)
                    {
                        return new JsonResult(sourceRef);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch (TaskCanceledException timeout)
            {
                mobjLogSvc.ErrorException(timeout, "Request timed out for device {0}", request.DeviceID);
            }
            catch (Exception ex)
            {
                mobjLogSvc.ErrorException(ex, "Cannot send sourceref to device {0}", request.DeviceID);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

      [HttpPost]
      public async Task<IActionResult> GetServerStatus(MobileDevice request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
            {
               var status = await mobileService.GetServerStatus();
               if (status != null)
               {
                  return new JsonResult(status);
               }
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }

            return StatusCode(StatusCodes.Status404NotFound);
         }
         catch (TaskCanceledException timeout)
         {
            mobjLogSvc.ErrorException(timeout, "Server unavailable");
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Generic error");
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      [HttpPost]
      private async Task<IActionResult> RequestDevices(DataSourceRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
            {
               var data = await mobileService.GetDevices();
               return new JsonResult(data.ToDataSourceResult(request));
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }
         }
         catch (TaskCanceledException timeout)
         {
            mobjLogSvc.ErrorException(timeout, "Request timed out ");
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Request fetch devices ");
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }
   }
}
