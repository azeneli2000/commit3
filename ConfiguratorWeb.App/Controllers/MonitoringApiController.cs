using System;
using System.Collections.Generic;
using Configurator.Std.BL.Monitoring;
using ConfiguratorWeb.App.Models.MonitoringApi;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using Digistat.FrameworkWebExtensions.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;


namespace ConfiguratorWeb.App.Controllers
{
   [ApiController]
   [Route("api/[controller]/[action]")]
   public class MonitoringApiController : Controller 
   {
      private ILoggerService mobjLogSvc;
      private IMonitoringRawResponseManager mobjMonRawResult;
      private IMonitoringRawRequestManager  mobjMonRawRequest;
      private ICurrentSystemErrorStatusManager  mobjCurrentSystemErrorStatusMgr;
      private IMemoryCache mobjCache;
      private ISystemOptionsService mobjSysOptSvc;
      protected readonly IDigistatConfiguration mobjDigistatConfig;

      public MonitoringApiController(IDigistatConfiguration config,
                                     ISystemOptionsService sysOptSvc,
                                     ILoggerService logSvc,
                                     IMonitoringRawRequestManager monRawRequest,
                                     IMonitoringRawResponseManager monRawResult,
                                     IMemoryCache memoryCache,
                                     ICurrentSystemErrorStatusManager currentSystemErrorStatusMgr)
      {

         mobjDigistatConfig = config;
         mobjLogSvc = logSvc;
         mobjMonRawResult = monRawResult;
         mobjMonRawRequest = monRawRequest;
         mobjCache = memoryCache;
         mobjSysOptSvc = sysOptSvc;
         mobjCurrentSystemErrorStatusMgr = currentSystemErrorStatusMgr;
      }


      /// <summary>
      /// Get Latest data of monitoring (Client,Service, SystemInfo)
      /// </summary>
      /// <returns></returns>
      /// <remarks>
      /// The functionality is protected by authentication.
      /// </remarks>
      [HttpGet]
      [ServiceFilter(typeof(BasicAuthFilter))]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status200OK, "Return Data",typeof(List<MonitoringDataViewModel>))]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]
      [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returned status code when user could not be authenticated (i.e. bad username or password)")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned status code when a generic error happen writing the message")]
      public JsonResult GetLastMonitoringData()
      {
         var result = new List<MonitoringDataViewModel>();
         
         var idClient = mobjMonRawRequest.GetLastRequest(MonitoringData.MonitoringType.Client)?.mrq_id??-1;
         var idService = mobjMonRawRequest.GetLastRequest(MonitoringData.MonitoringType.Service)?.mrq_id??-1;
         var idSysInfo = mobjMonRawRequest.GetLastRequest(MonitoringData.MonitoringType.SystemInfo)?.mrq_id??-1;
         if (idClient >= 0)
         {
            var clients = mobjMonRawResult.GetMonitoringResponseByRequestId(idClient);
            if (clients != null && clients.Count > 0)
            {
               foreach (var row in clients)
               {
                  try
                  {
                     result.Add(MonitoringDataViewModelBuilder.Build(row));
                  }
                  catch (Exception e)
                  {
                     mobjLogSvc.ErrorException(e,"Error building MonitoringDataView");
                  }
               }

            }
         }
         if (idService>=0)
         {
            var services = mobjMonRawResult.GetMonitoringResponseByRequestId(idService);
            if (services!= null && services.Count>0)
            {
               foreach (var row in services)
               {
                  try
                  {
                     result.Add(MonitoringDataViewModelBuilder.Build(row));
                  }
                  catch (Exception e)
                  {
                     mobjLogSvc.ErrorException(e,"Error building MonitoringDataView");
                  }
                  
               }
            }
         }
         if (idSysInfo >= 0)
         {
            var sysInfo = mobjMonRawResult.GetMonitoringResponseByRequestId(idSysInfo);
            foreach (var row in sysInfo)
            {
               try
               {
                  result.Add(MonitoringDataViewModelBuilder.Build(row));
               }
               catch (Exception e)
               {
                  mobjLogSvc.ErrorException(e,"Error building MonitoringDataView");
               }
            }
         }


         return Json(result) ;
      }
      
      /// <summary>
      /// Get Latest data of monitoring with Unite format
      /// </summary>
      /// <returns></returns>
      /// <remarks>
      /// </remarks>
      [HttpGet]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status200OK, "Return Data",typeof(MonitoringDataForUniteViewModel))]
      [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned status code when a generic error happen writing the message")]
      public JsonResult GetLastMonitoringDataForUnite()
      {
         var result = new MonitoringDataForUniteViewModel();
         int reqFreqMin = 1;
         var applicationName = "SMARTSUPERVISOR";
         if (!mobjSysOptSvc.CheckIfSystemOptionApplicationIsLoaded(applicationName))
         {
            mobjSysOptSvc.ReloadSystemOptions(applicationName);
         }

         var optionName = "MonitoringServerComponentsInterval";

         if (int.TryParse(mobjSysOptSvc.GetSystemOption(applicationName , string.Empty, string.Empty, string.Empty, optionName, false)?.Value, out var sysReqFreq))
         {
            reqFreqMin = sysReqFreq>0?sysReqFreq:1;
         }
         int cacheServiceId;
         List<MonitoringData> services = null;
         var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(reqFreqMin * 60 / 2));
         // Look for cache key.
         if (!mobjCache.TryGetValue("MonitoringApi_ServiceId", out cacheServiceId))
         {
            // Key not in cache, so get data.
            cacheServiceId =  mobjMonRawRequest.GetLastRequest(MonitoringData.MonitoringType.Service)?.mrq_id??-1;

            // Save data in cache.
            mobjCache.Set("MonitoringApi_ServiceId", cacheServiceId, cacheEntryOptions);


               // Key not in cache, so get data.
            services = mobjMonRawResult.GetMonitoringResponseByRequestId(cacheServiceId);

            mobjCache.Set("MonitoringApi_ServiceData", services, cacheEntryOptions);
            
         }


         if (cacheServiceId>=0)
         {
            

            // Look for cache key.
            if (services == null)
               mobjCache.TryGetValue("MonitoringApi_ServiceData", out services);

            if (services != null && services.Count > 0)
            {
               try
               {
                  result = MonitoringDataForUniteViewModelBuilder.Build(services,mobjLogSvc);
               }
               catch (Exception e)
               {
                  mobjLogSvc.ErrorException(e, "Error building MonitoringDataView");
               }
            }
         }
         return Json(result);
      }
      
      /// <summary>
      /// Get Latest Status error
      /// </summary>
      /// <returns></returns>
      /// <remarks>
      /// </remarks>
      [HttpGet]
      [Produces("application/json")]
      [SwaggerResponse(StatusCodes.Status200OK, "Return json string")]
      [SwaggerResponse(StatusCodes.Status204NoContent, "No Content")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned status code when a generic error occurs. Details are logged.")]
      //[SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned status code when a generic error happen writing the message")]
      public ActionResult Reliability()
      {
         
         try
         {
            var result =  mobjCurrentSystemErrorStatusMgr.GetLastValidStatus();
            if (result != null)
            {
               return Content(result.SystemStatus,"application/json" );

            }
            else
            {
               return StatusCode(StatusCodes.Status204NoContent);
            }
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e, "Error on MonitoringAPIController/Reliability");
            return StatusCode(500);
         }
         
      }
      
   }

}

