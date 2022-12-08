using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.Models;

using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using Digistat.FrameworkWebExtensions.Attributes;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Digistat.FrameworkStd.UMSLegacy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http.Extensions;
using ConfiguratorWeb.App.Helpers;

namespace ConfiguratorWeb.App.Controllers
{
   [DigConfigFilterAttribute]
   [DigistatAuthFilterAttribute]
   public class HomeController : DigistatWebControllerBase   {


      protected readonly IDigistatConfiguration mobjDigistatConfig;
      protected readonly ISystemOptionsService mobjSystemOptionsService;
      protected readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      protected readonly IControlBarConfiguration mobjCtrlbCfg;
      private ILoggerService mobjLog;
      private IWebHostEnvironment mobjEnv;

      public HomeController(IDigistatConfiguration config, IMessageCenterService msgcenter,
       ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, 
       ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc
       //, IControlBarConfiguration ctrlbCfg
       , IWebHostEnvironment env

       )
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         //mobjCtrlbCfg = ctrlbCfg;
         mobjEnv = env;
      }

      public IActionResult Index()
      {
         return View();
      }


      public IActionResult SystemInfo()
      {
         ViewBag.MessageCenter = mobjDigistatConfig.MessageCenter + ":" + mobjDigistatConfig.MessageCenterInstance;
         ViewBag.SessionStorage = mobjDigistatConfig.SessionStorage;
         ViewBag.IsUS = mobjDigEnvironmentService.IsUS;
         ViewBag.HACurrMessageCenter = string.Empty;
         ViewBag.HostName = mobjSyncSvc.GetCurrentNetwork().HostName;
         ViewBag.HostIP = mobjSyncSvc.GetCurrentNetwork().IpAddress;
         try
         {
            
            var ips = Dns.GetHostAddresses(Dns.GetHostName());

            var result = ips.FirstOrDefault(x => {
               
               return x.AddressFamily == AddressFamily.InterNetwork;
            });
            var localIpAddress = result?.ToString()??"n/d";
            ViewBag.ServerName = Dns.GetHostName();//Environment.MachineName;
            ViewBag.ServerIP = localIpAddress;
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
         
         if (mobjDigEnvironmentService.IsHAEnabled)
         {
            ViewBag.HACurrMessageCenter = mobjDigEnvironmentService.CurrMessageCenterInstance + ":" + mobjDigEnvironmentService.CurrMessageCenterPort;
         }

         string compiledVersion = "1.0.0";
         try
         {
            compiledVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            compiledVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            //string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
            //string assemblyVersion2 = Assembly.LoadFile(assembly.Location).GetName().Version.ToString(); 
            //string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion; 
            //string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            compiledVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            //compiledVersion += " build date: " + GetBuildDate(Assembly.GetEntryAssembly());
         }
         catch (Exception)
         {
//            throw;
         }
         ViewBag.CompiledVersion =compiledVersion;
         //ViewBag.CurrentCulture = mobjCtrlbCfg.Culture ?? "not set";

         if (!string.IsNullOrEmpty(mobjDigistatConfig.ConnectionString))
         {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(mobjDigistatConfig.ConnectionString);
            ViewBag.Server = builder.DataSource;
            ViewBag.DB = builder.InitialCatalog;
         }
         return View();
      }

      //public JsonResult LoadDriver()
      //{
      //   DasDriverInfoExtended objDriverInfo = null;
      //   try
      //   {

      //      DASCapabilitiesReader objReader = new DASCapabilitiesReader();
      //      objDriverInfo =  objReader.Reader(@"C:\UMS\WebNetCore\ConfiguratorWeb\ConfiguratorWeb.App\bin\TEST\UMS.DAS.DriverTestV2.exe");
      //   }
      //   catch(System.Exception exc)
      //   {
      //      return Json(new { errorMessage = exc.ToString() });
      //   }
         
      //   return Json(objDriverInfo);
      //}

      public IActionResult About()
      {
         ViewData["Message"] = "Your application description page.";

         return View();
      }

      public IActionResult Contact()
      {
         ViewData["Message"] = "Your contact page.";

         return View();
      }

      public IActionResult Privacy()
      {
         return View();
      }
     

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         var objError = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
         var strErrorPath = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Path;

         if (objError != null)
         {
            mobjLogSvc.ErrorException(objError, strErrorPath != null ? $"Error on \"{strErrorPath}\"" : "Global error");
         }
         bool bolIsAjaxRequest = HttpContext.Request.IsAjaxRequest();
        if (bolIsAjaxRequest)
        {
            return new JsonResult(new { IsSuccess = false, StatusCode = HttpStatusCode.InternalServerError, Message = objError.Message });
        }
        else
        {
            return View("Error");
        }
         //return HttpContext.Request.IsAjaxRequest()
         //   ? //(IActionResult)AjaxResult.Error(HttpStatusCode.InternalServerError).WithMessage(mobjDicSvc.XLate(GlobalsUIDef.Localization.GENERIC_ERROR_MESSAGE))
         //   new JsonResult(new {IsSuccess=false,StatusCode=HttpStatusCode.InternalServerError,Message = objError.Message})
         //   : View("Error");

         //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
      private static DateTime GetBuildDate(Assembly assembly)
      {
         const string BuildVersionMetadataPrefix = "+build";

         var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
         if (attribute?.InformationalVersion != null)
         {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
               value = value.Substring(index + BuildVersionMetadataPrefix.Length);
               if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
               {
                  return result;
               }
            }
         }

         return DateTime.MinValue;
      }
 
   }
}
