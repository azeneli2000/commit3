using Configurator.Std.BL;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using ConfiguratorWeb.App.Extensions;
using Digistat.FrameworkStd.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Configurator.Std.BL.ReportMaster;
using ConfiguratorWeb.App.SysOptionConfig;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using Configurator.Std.Exceptions;
using Configurator.Std.Enums;
using ConfiguratorWeb.App.Models.ReportMaster;
using SmartCentralConfig = ConfiguratorWeb.App.SysOptionConfig.SmartCentralConfig;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using Digistat.FrameworkStd.Model.ControlBar;
using ConfiguratorWeb.App.Models.General;
using Digistat.FrameworkStd.SysOptionConfig;

namespace ConfiguratorWeb.App.Controllers
{
   public class SystemConfigurationController : DigistatWebControllerBase
   {

      private readonly ISystemOptionsManager mobjSystemOptionsManager;
      private readonly INetworksManager mobjNetworksManager;
      private readonly IBedsManager mobjBedsManager;
      private readonly IUsersManager mobjUsersManager;
      private readonly ILocationManager mobjLocationManager;
      private readonly IHospitalUnitsManager mobjHospitalUnitsManager;
      private readonly IPermissionsService mobjPermSvc;
      private readonly IMiscellaneousManager mobjMiscMgr;
      private readonly ISystemValidationManager mobjSysValidMgr;
      private readonly IDigistatRepositoryManager mobjDigrepoMgr;
      private readonly IStandardParametersManager mobjParMgr;
      private readonly IStandardUnitsManager mobjUnitMgr;
      private readonly IStandardDeviceTypesManager mobjStandardDeviceTypesManager;
      private readonly IRolesManager mobjRoleManager;
      private IReportTemplateManager mobjReport;

      private readonly ILoggerService mobjLoggerService;
      // private readonly LocationManager mobjLocationsManager;
      protected readonly IWebModulesManager mobjWebModulesManager;
      private readonly IDBWebModulesManager mobjdbWebModuleMgr;
      private readonly IHttpContextAccessor mobjHttpContext;
      private string anErrorOccurredInDeserializationCouldBeDueToAnInvalidXmlSeeLogForDetails = "An error occurred in deserialization. Could be due to an invalid XML. See log for details";

      public SystemConfigurationController(IDigistatConfiguration config, IMessageCenterService msgcenter, IDictionaryService dicSvc, ISystemOptionsService sysOptSvc, ISynchronizationService syncSvc, IDnsCacherService dnsSvc, ILoggerService logSvc, ISystemOptionsManager systemOptionsManager, INetworksManager networksManager, IBedsManager bedsManager
         , IUsersManager usersManager, ILocationManager locationManager, IHospitalUnitsManager huManager, IDictionaryService dicService, IPermissionsService permSvc
         ,IMiscellaneousManager miscManager, ISystemValidationManager sysValidMgr, IDigistatRepositoryManager digRepoMgr
         , IStandardParametersManager parMgr,IStandardUnitsManager unitMgr
         , IStandardDeviceTypesManager standardDeviceTypesManager ,IReportTemplateManager repSvc, IWebModulesManager webModulesManager
         , IDBWebModulesManager dbWebModuleMgr, IRolesManager roleManager
         , IHttpContextAccessor httpContext
         )
            : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)

      {
         mobjSystemOptionsManager = systemOptionsManager;
         mobjNetworksManager = networksManager;
         mobjBedsManager = bedsManager;
         mobjUsersManager = usersManager;
         mobjLocationManager = locationManager;
         mobjHospitalUnitsManager = huManager;
         mobjPermSvc = permSvc;
         mobjMiscMgr = miscManager;
         mobjSysValidMgr = sysValidMgr;
         mobjDigrepoMgr = digRepoMgr;
         mobjParMgr = parMgr;
         mobjUnitMgr = unitMgr;
         mobjStandardDeviceTypesManager = standardDeviceTypesManager;
         mobjLoggerService = logSvc;
         mobjReport = repSvc;
         mobjWebModulesManager = webModulesManager;
         mobjdbWebModuleMgr = dbWebModuleMgr;
         mobjHttpContext = httpContext;
         mobjRoleManager = roleManager;
      }

      // GET: SystemConfiguration
      public ActionResult Index()
      {
         return View();
      }

      public ActionResult SystemOptions()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > System Options";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public ActionResult HospitalUnits()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > Hospital Units";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public ActionResult Beds()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > Bed";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }

      }
      public ActionResult BedsList()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > BedsList";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public ActionResult Locations()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > Locations";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }


      public ActionResult Miscellanea()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > Miscellanea";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }

      }



      #region SystemOptions

      public IActionResult GetSystemOption(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            SystemOption systemOption = mobjSystemOptionsManager.Get(id);
            SystemOptionViewModel model = SystemOptionViewModelBuilder.Build(systemOption);

            //If it's binary I need to retrieve the name of the file. 
            if(model.Type== OptionType.Binary)
            {
               var objRepo = mobjDigrepoMgr.Get(model.Value);
               model.ValueDisplayBinary = objRepo != null? objRepo.FileName:mobjDicSvc.XLate("Choose a file ...");
            }

            return View("_SystemOption", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveSystemOptionDetail(SystemOptionViewModel model)
      {
         string messageError = string.Empty;
         SystemOption objSystemOption = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, CurrentUser))
            {
               string strUsrAbbrev = CurrentUser != null ? CurrentUser.Abbrev : string.Empty;
               string strHostname = CurrentNetwork != null ? CurrentNetwork.HostName : string.Empty;
               if (string.IsNullOrWhiteSpace(model.GUID)) //create
               {
                  objSystemOption = mobjSystemOptionsManager.Create(SystemOptionEntityBuilder.Build(model), strUsrAbbrev, strHostname);
               }
               else //update
               {
                  objSystemOption = mobjSystemOptionsManager.Update(SystemOptionEntityBuilder.Build(model), strUsrAbbrev, strHostname);
               }


               if (objSystemOption != null)
               {
                  bolSuccess = true;
               }

            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }




      public JsonResult ReadSystemOptions([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
            {
               request.RenameRequestFilterMember("UserName", "UserAbbreviation");
               request.RenameRequestFilterMember("HospitalUnitName", "HospitalUnit.ShortName");
               bool isGeneralAppSelected = false;

               if (request.Filters.Any())
               {
                  foreach (var fdc in request.Filters.ToFilterDescriptor())
                  {
                     if (fdc.Member == "Application")
                     {
                        if (fdc.Value.ToString() == CommonStrings.GENERAL_APPLICATION_FILTER)
                        {
                           fdc.Value = null;
                           isGeneralAppSelected = true;
                        }
                     }
                  }
               }
               IQueryable<SystemOption> objAll = mobjSystemOptionsManager.GetSystemOptions(isGeneralAppSelected);

               DataSourceResult data = objAll.ToDataSourceResult(
               request.SortAttributesMapping(SystemOptionViewModelExtensions.SortMappings)
                   .GroupAttributesMapping(SystemOptionViewModelExtensions.GroupMappings)
                   //.FilterAttributesMapping(SystemOptionViewModelExtensions.FilterMappings)
                   , model => SystemOptionViewModelBuilder.Build(model, (request.PageSize == 0)));
               return new JsonResult(data);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception exc)
         {
            mobjLoggerService.Error("ReadSystemOptions: {0}",exc.Message);
            return Json(new { errorMessage = exc.Message, success = false });
         }

      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult CreateSystemOption([DataSourceRequest] DataSourceRequest request, SystemOptionViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, CurrentUser))
            {
               SystemOption systemOption = SystemOptionEntityBuilder.Build(model);

               mobjSystemOptionsManager.Create(systemOption);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json((new[] { model }).ToDataSourceResult(request, ModelState));
      }

      [HttpPost]
      public JsonResult DeleteSystemOption(string guid)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, CurrentUser))
            {
               if (!string.IsNullOrWhiteSpace(guid))
               {

                  mobjSystemOptionsManager.Delete(guid);
                  bolSuccess = true;
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
      public IActionResult editLogOption(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            
            xmlString = xmlString.Replace("\r\n","");
            XmlSerializer serializer = new XmlSerializer(typeof(LogOptionsViewModel));
            //xmlString= System.Net.WebUtility.HtmlDecode(xmlString);

            LogOptionsViewModel logOptionsViewModel = new LogOptionsViewModel();
            using (MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString))) 
            {
               try
               {
                  var x = serializer.Deserialize(memStream);
                  logOptionsViewModel = (LogOptionsViewModel)x;
               }
               catch (Exception e)
               {
                  Console.WriteLine(e);
               }
            }

            return View("_LogOptions", logOptionsViewModel);
            
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      
      private string normalizeUriString(IHttpContextAccessor myHE, string uriString)
      {
         try
         {

            if (!uriString.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
               var context = myHE;
               Uri uri = new Uri(context.HttpContext.Request.Scheme + "://" + context.HttpContext.Request.Host.Value +
                                 (uriString.StartsWith("/") ? "" : "/") + uriString);
               uriString = uri.AbsoluteUri;
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            mobjLoggerService.ErrorException(e,"Error in normalizeUriString");
         }


         return uriString;
      }
      public IActionResult EditBrowserModulesXml(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            if(string.IsNullOrEmpty(xmlString))
            {
               //If no xml is provided, use the default one.
               var bm = new BrowserModules();
               bm.Add(new BrowserModule());
               xmlString = bm.Serialize();
            }
            xmlString = xmlString.Replace("\r\n","");
            if (xmlString.Contains("&") && xmlString.Split("&amp;").Length != xmlString.Split("&").Length)
            {
               //todo: convert "&" to "&amp"
               var aXml = xmlString.Split("&");
               for (int i = 0; i < aXml.Length; i++)
               {
                  if (i+1 < aXml.Length)
                  {
                     if (!aXml[i+1].StartsWith("amp;"))
                     {
                        aXml[i + 1] = "amp;" + aXml[i + 1];
                     }
                  }
               }
               xmlString =String.Join("&",aXml);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Models.SystemOptions.BrowserModulesViewModels));
            //xmlString= System.Net.WebUtility.HtmlDecode(xmlString);
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
            var browserModuleViewModule=new Models.SystemOptions.BrowserModulesViewModels();
            try
            {
               var x = serializer.Deserialize(memStream);
               browserModuleViewModule = (Models.SystemOptions.BrowserModulesViewModels) x;
               if (browserModuleViewModule != null)
               {
                  foreach (Models.SystemOptions.Module m in browserModuleViewModule.Module) 
                  {
                     if (string.IsNullOrEmpty(m.LeaveConfirmation))
                     {
                        m.LeaveConfirmation = false.ToString();
                     }
                     if (string.IsNullOrEmpty(m.MandatoryUser))
                     {
                        m.MandatoryUser = false.ToString();
                     }
                  }
               }
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }

            List<WebModule> webModules = null;
            try
            {
               webModules = mobjdbWebModuleMgr.GetAllModules();
               if (webModules.Count > 0)
               {
                  foreach (WebModule module in webModules)
                  {
                     module.Url =  normalizeUriString(mobjHttpContext, module.Url??"" );
                  }
               }
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               mobjLoggerService.ErrorException(e,"Error in EditBrowserModulesXml");
            }
            ViewBag.WebModulesList = webModules;

            try
            {
               return PartialView("_BrowserModuleOptions", browserModuleViewModule);
            }
            catch (Exception e)
            {
               mobjLoggerService.ErrorException(e,"Error calling _BrowserModuleOptions");
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false });
            }
            
            
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      public IActionResult EditReportMasterConfigXml(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            
            xmlString = xmlString.Replace("\r\n","");
            
            XmlSerializer serializer = new XmlSerializer(typeof(ReportMasterViewModel));

            ReportMasterViewModel reportMasterViewModel = new ReportMasterViewModel();
            using (MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString))) 
            {
               try
               {
                  var x = serializer.Deserialize(memStream);
                  reportMasterViewModel = (ReportMasterViewModel)x;
               }
               catch (Exception e)
               {
                  Console.WriteLine(e);
               }
            }

            var listName = new List<string>();
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionReportMasterTemplateView, CurrentUser))
            {
               //IEnumerable<ReportTemplate> objAllTemplates 
               listName = mobjReport.GetQueryable().Where(a => a.Current && !a.ValidToDate.HasValue).Select(s=>s.Name).ToList();
            }
            ViewData["ReportTemplatesListName"] = listName;

            return PartialView("_ReportMaster", reportMasterViewModel);
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }
      public IActionResult EditSmartCentralConfigEditorXml(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            
            xmlString = xmlString.Replace("\r\n","");

            SmartCentralConfig sm = null; //  SmartCentralConfig.Deserialize(xmlString);
            try
            {
               sm = SmartCentralConfig.Deserialize(xmlString);
            }
            catch (Exception e)
            {
               mobjLoggerService.ErrorException(e, "Error in EditSmartCentralConfigEditorXml, xml: {0}",xmlString);
               return StatusCode(StatusCodes.Status500InternalServerError, Json(new { errorMessage = anErrorOccurredInDeserializationCouldBeDueToAnInvalidXmlSeeLogForDetails, success = false }));
            }

            var lstDeviceTypeOrder = new List<( int Value,string Name)>();
            foreach (SmartCentralConfig.DeviceType dt in sm.DeviceTypeOrder)
            {
               lstDeviceTypeOrder.Add(((int)dt ,typeof(DeviceType).GetEnumName( dt )));
               //lstDeviceTypeOrder.Add(((DeviceType)dt  ,typeof(DeviceType).GetEnumName( dt )));
            }
            
            //lstDeviceTypeOrder.Add((DeviceType.BloodFiltration            ,typeof(DeviceType).GetEnumName(DeviceType.BloodFiltration             )));
            //lstDeviceTypeOrder.Add((DeviceType.BloodGasAnalyzer           ,typeof(DeviceType).GetEnumName(DeviceType.BloodGasAnalyzer            )));
            //lstDeviceTypeOrder.Add((DeviceType.HeartLungMachine           ,typeof(DeviceType).GetEnumName(DeviceType.HeartLungMachine            )));
            //lstDeviceTypeOrder.Add((DeviceType.Incubator                  ,typeof(DeviceType).GetEnumName(DeviceType.Incubator                   )));
            //lstDeviceTypeOrder.Add((DeviceType.InfusionPump               ,typeof(DeviceType).GetEnumName(DeviceType.InfusionPump                )));
            //lstDeviceTypeOrder.Add((DeviceType.LaboratoryInformationSystem,typeof(DeviceType).GetEnumName(DeviceType.LaboratoryInformationSystem )));
            //lstDeviceTypeOrder.Add((DeviceType.OtherComplex               ,typeof(DeviceType).GetEnumName(DeviceType.OtherComplex                )));
            //lstDeviceTypeOrder.Add((DeviceType.OtherSimple                ,typeof(DeviceType).GetEnumName(DeviceType.OtherSimple                 )));
            //lstDeviceTypeOrder.Add((DeviceType.PatientMonitor             ,typeof(DeviceType).GetEnumName(DeviceType.PatientMonitor              )));
            //lstDeviceTypeOrder.Add((DeviceType.PulmonaryVentilator        ,typeof(DeviceType).GetEnumName(DeviceType.PulmonaryVentilator         )));

            ViewData["DeviceTypeOrderListName"] = lstDeviceTypeOrder;

            return PartialView("_SmartCentralConfig", sm);
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }
      public IActionResult editCameraConfigEditorXml(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            if (xmlString == null)
            {
               xmlString = "";
            }
            xmlString = xmlString.Replace("\r\n","");
            CameraConfigs sm = null;

            try
            {
               sm = CameraConfigs.Deserialize(xmlString);
            }
            catch (Exception e)
            {
               mobjLoggerService.ErrorException(e, "Error in editCameraConfigEditorXml, xml: {0}",xmlString);
               return StatusCode(StatusCodes.Status500InternalServerError, Json(new { errorMessage = anErrorOccurredInDeserializationCouldBeDueToAnInvalidXmlSeeLogForDetails, success = false }));
            }


            return PartialView("_CameraConfig", sm);
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }


      public IActionResult EditSMTPConfigurationXml(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            if (xmlString == null)
            {
               xmlString = "";
            }
            xmlString = xmlString.Replace("\r\n", "");
            try
            {
               SMTPConfiguration sm = SMTPConfiguration.Deserialize(xmlString);
               return PartialView("_SMTPConfig", sm);
            }
            catch (Exception e)
            {
               mobjLoggerService.ErrorException(e, "Error in EditSMTPConfigurationXml, xml: {0}",xmlString);
               return StatusCode(StatusCodes.Status500InternalServerError, Json(new { errorMessage = anErrorOccurredInDeserializationCouldBeDueToAnInvalidXmlSeeLogForDetails, success = false }));
            }
            
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public IActionResult EditSMTPUserReportConfigurationXML(string xmlString)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            if (xmlString == null)
            {
               xmlString = "";
            }
            xmlString = xmlString.Replace("\r\n", "");
            try
            {
               SMTPUserReportConfiguration sm = SMTPUserReportConfiguration.Deserialize(xmlString);
               return PartialView("_UserReportConfig", sm);
            }
            catch (Exception e)
            {
               mobjLoggerService.ErrorException(e, "Error in EditSMTPUserReportConfigurationXML");
               return StatusCode(StatusCodes.Status500InternalServerError, Json(new { errorMessage = "An error occurred in deserialization. Could be due to an invalid XML. See log for details", success = false }));
               //               return Json(new { errorMessage = "An error occurred in deserialization. See log for details", success = false });
            }

         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public IActionResult SerializeUserReportConfiguration([FromBody]SMTPUserReportConfiguration config)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, CurrentUser))
         {
            try
            {
               string strXml = SMTPUserReportConfiguration.Serialize(config);
               return Json(new { success = true, data = strXml });
            }
            catch (Exception e)
            {
               mobjLoggerService.ErrorException(e, "Error in SerializeUserReportConfiguration");
               return Json(new { errorMessage = "An error occurred in serialization. See log for details", success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult SMTPTestServerConfiguration(SMTPConfiguration config, string senderAddress,string destinationAddress)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, CurrentUser))
         {
            if(!string.IsNullOrEmpty(senderAddress) && !string.IsNullOrEmpty(destinationAddress))
            {
               try
               {
                  using (SmtpClient client = new SmtpClient(config.Hostname, config.Port))
                  {
                     client.EnableSsl = config.EnableSSL;
                     client.UseDefaultCredentials = true;
                     if (!string.IsNullOrEmpty(config.Username))
                     {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(config.Username, config.Password);
                     }
                     MailAddress objSender = new MailAddress(senderAddress);
                     MailAddress objRecipient = new MailAddress(destinationAddress);
                     MailMessage message = new MailMessage(objSender, objRecipient);
                     message.Subject = "This is a test from Digistat Configurator";
                     message.Body = "This is a test from Digistat Configurator";

                     client.Send(message);
                     mobjLoggerService.Write(100, $"SMTP Test done - SenderAddress: {senderAddress} - DestinationAddress: {destinationAddress}", EventLogEntryType.Information, CurrentUser.Abbrev, LogLevel.Always);
                     return Json(new { success = true });
                  }

               }
               catch (Exception e)
               {
                  if (e.InnerException != null && e.InnerException is SocketException)
                  {
                     mobjLoggerService.ErrorException(e, "Error in SMTPTestServerConfiguration");
                     return Json(new { errorMessage = $"A connection error occurred to {config.Hostname}:{config.Port} ", success = false });
                  }
                  else
                  {
                     mobjLoggerService.ErrorException(e, "Error in SMTPTestServerConfiguration");
                     return Json(new { errorMessage = "An error occurred in SMTPTestServerConfiguration: " + e.ToString(), success = false });
                  }
                  
               }
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate("Sender and Destination addresses are mandatory"), success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult SerializeSMTPConfiguration(SMTPConfiguration config)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsEdit, CurrentUser))
         {
            try
            {
               string strXml = SMTPConfiguration.SerializeAndEncryptPassword(config);
               return Json(new { success= true,data = strXml });
            }
            catch(Exception e)
            {
               mobjLoggerService.ErrorException(e, "Error in SerializeSMTPConfiguration");
               return Json(new { errorMessage = "An error occurred in serialization. See log for details", success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

    

      public IActionResult editReportMasterConfig(string guid)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            SystemOption systemOption = mobjSystemOptionsManager.Get(guid);
            SystemOptionViewModel model = SystemOptionViewModelBuilder.Build(systemOption);

            //If it's binary I need to retrieve the name of the file. 
            if (model.Type != OptionType.Text)
            {
               return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
            }

            string xmlString = model.Value.Replace("\r\n","");
            //if (!xmlString.Trim().StartsWith("<?xml"))
            //{
            //   xmlString = "<?xml version=\"1.0\"?>" + xmlString;
            //}
            XmlSerializer serializer = new XmlSerializer(typeof(ReportMasterViewModel));
            //xmlString= System.Net.WebUtility.HtmlDecode(xmlString);
            
            ReportMasterViewModel reportMasterViewModel = new ReportMasterViewModel();
            using (MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {               
               try
               {
                  var x = serializer.Deserialize(memStream);
                  reportMasterViewModel = (ReportMasterViewModel)x;
               }
               catch (Exception e)
               {
                  Console.WriteLine(e);
               }
            }
            return PartialView("_ReportMaster", reportMasterViewModel);
         }
         else
         {
            return Json(new {errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false});
         }
      }

      public IActionResult GetUsers(string username, string id)
      {
         // IQueryable<ConfiguratorWeb.Core.Model.User> objAllUsers = new UsersManager(Security.SecurityManagerFactory.GetCurrent().CurrentUserIdentifier).GetQueriable().Where(a => a.Current && !a.AccountDisabled);
         IQueryable<User> objAllUsers = mobjUsersManager.GetQueryable().Where(x => !(x.AccountDisabled ?? false) && (x.Current ?? false));
         ViewBag.FormDetailId = id;
         List<UserViewModel> objUserModels = UserViewModelBuilder.BuildList(objAllUsers).ToList();
         return View("_Users", objUserModels);
      }

      public IActionResult GetHospitalUnits(string huID, string soID,string huToExcludeID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, CurrentUser))
         {
            var objAll = mobjHospitalUnitsManager.GetQueryable().Where(x => x.Current).OrderBy(o => o.ShortName);

            List<HospitalUnitViewModel> model = objAll.Select(x => HospitalUnitViewModelBuilder.Build(x)).ToList();
            if (!string.IsNullOrEmpty(huToExcludeID))
            {
               model.RemoveAll(p => p.GUID == huToExcludeID);
            }

            //List<HospitalUnitViewModel> model = UtilityHelper.GetHospitalUnits();

            ViewBag.huID = huID ?? string.Empty;
            ViewBag.soID = soID ?? string.Empty;

            return View("_HospitalUnits", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      public IActionResult GetHospitalUnit(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, CurrentUser))
         {
            HospitalUnitViewModel model = new HospitalUnitViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
               HospitalUnit hospitalUnit = mobjHospitalUnitsManager.Get(id);
               if (!string.IsNullOrEmpty(hospitalUnit.ParentID))
               {
                  hospitalUnit.Parent = mobjHospitalUnitsManager.Get(hospitalUnit.ParentID);
               }
               model = HospitalUnitViewModelBuilder.Build(hospitalUnit);
            }
            return View("_HospitalUnitDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }


      public IActionResult EditDefaultRoles(string value,string callbackFunction)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
            {
               List<Role> objRoles = mobjRoleManager.GetAllFast();
               List<Models.RoleViewModel> objRoleVM = ViewModelBuilders.RoleViewModelBuilder.BuildList(objRoles).ToList();
               objRoleVM = objRoleVM.OrderBy(p => p.Id).ToList();

               if (!string.IsNullOrEmpty(value))
               {
                  List<int> objRoleIds = new List<int>();
                  List<string> objSlices = value.Split(';').ToList();
                  if (objSlices != null)
                  {
                     foreach (string s in objSlices)
                     {
                        int tmpOut = -1;
                        if (int.TryParse(s, out tmpOut))
                        {
                           objRoleIds.Add(tmpOut);
                        }
                     }
                  }
                  if (objRoleIds != null && objRoleIds.Count() > 0)
                  {
                     foreach (RoleViewModel objRole in objRoleVM)
                     {
                        if (objRoleIds.Contains(objRole.Id.Value))
                        {
                           objRole.Selected = true;
                        }
                        else
                        {
                           objRole.Selected = false;
                        }
                     }
                  }
               }
               ViewBag.CallBackFunction = callbackFunction;
               return PartialView("_RoleSelector", objRoleVM);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         catch(Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error in EditDefaultRoles");
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false });
         }
         
      }

      [HttpPost]
      public JsonResult SaveHospitalUnitDetail(HospitalUnitViewModel model)
      {
         string messageError = string.Empty;
         HospitalUnit objHospitalUnit = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsEdit, CurrentUser))
            {
               if (string.IsNullOrWhiteSpace(model.GUID)) //create
               {
                  objHospitalUnit = mobjHospitalUnitsManager.Create(HospitalUnitEntityBuilder.Build(model));
               }
               else //update
               {
                  objHospitalUnit = mobjHospitalUnitsManager.Update(HospitalUnitEntityBuilder.Build(model));
               }


               if (objHospitalUnit != null)
               {
                  bolSuccess = true;
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }


            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      [HttpPost]
      public JsonResult TryToSerializeXml(string webString, string specificClass)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (string.IsNullOrWhiteSpace(webString)) //create
            {
               return Json(new { errorMessage = mobjDicSvc.XLate("xml is empty"), success = bolSuccess });
            }

            try
            {
               string baseXml = String.Empty;
               switch (specificClass)
               {
                  case "ReportMasterConfig":
                     try
                     {
                        ReportMasterConfig.Deserialize(webString);
                     }
                     catch (Exception)
                     {
                        baseXml = new ReportMasterConfig().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }

                     break;
                  case "LogOptions":

                     Tuple<bool, string> tLog = LogOptionsConfig.Deserialize(webString);
                     if (!tLog.Item1)
                     {
                        //   throw new Exception(tLog.Item2);
                        baseXml = new LogOptionsConfig().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }
                     break;

                  case "SmartCentralConfig":
                     try
                     {
                        SmartCentralConfig.Deserialize(webString);
                     }
                     catch (Exception)
                     {
                        baseXml = new SmartCentralConfig().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }
                     break;

                  case "CustomModuleConfig":

                     try
                     {
                        UMSCustomModuleConfig.Deserialize(webString);
                     }
                     catch (Exception)
                     {

                        baseXml = new UMSCustomModuleConfig().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }
                     break;

                  case "UserPermissionProfileConfig":
                     try
                     {
                        UserPermissionProfileConfig.Deserialize(webString);
                     }
                     catch (Exception)
                     {
                        baseXml = new UserPermissionProfileConfig().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }

                     break;

                  case "SmartCentralTrendsConfig":
                     try
                     {
                        TrendChartConfiguration.Deserialize(webString);
                     }
                     catch (Exception)
                     {
                        baseXml = new TrendChartConfiguration().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }
                     break;

                  case "SmartCentralGridConfig":

                     try
                     {
                        GridConfiguration.Deserialize(webString);
                     }
                     catch (Exception)
                     {
                        baseXml = new GridConfiguration().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }
                     break;

                  case "SmartCentralAlarmAggregatorsConfig":
                     try
                     {
                        AlarmAggregators.Deserialize(webString);
                     }
                     catch (Exception e)
                     {
                        baseXml = new AlarmAggregators().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }

                     break;

                  case "CameraConfig":
                     try
                     {
                        CameraConfigs.Deserialize(webString);
                     }
                     catch (Exception)
                     {
                        baseXml = new CameraConfigs().Serialize();
                        validateXmlByStringXsd(webString, baseXml);
                     }
                     break;

                  default:
                     StringReader objStringReader = new StringReader(webString);
                     XmlTextReader objReader = new XmlTextReader(objStringReader);
                     while (objReader.Read())
                     {
                        switch (objReader.NodeType)
                        {
                           case XmlNodeType.Element:
                              break;
                           default:
                              break;
                        }
                     }
                     objReader.Close();
                     objStringReader.Close();
                     objStringReader.Dispose();
                     break;

               }
               bolSuccess = true;
            }
            catch (XmlException xe)
            {
               messageError = xe.Message;
            }
            catch (Exception e)
            {
               messageError = e.Message;

            }
            return Json(new { errorMessage = messageError, success = bolSuccess });

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      [HttpPost]
      public JsonResult TryToGetDefaultXml(string specificClass)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {

            string baseXml = String.Empty;
            try
            {

               switch (specificClass)
               {
                  case "ReportMasterConfig":

                     baseXml = new ReportMasterConfig().Serialize();

                     break;
                  case "BrowserModules":

                     var bm = new BrowserModules();
                     bm.Add(new BrowserModule());
                     baseXml = bm.Serialize();

                     break;
                  case "LogOptions":


                     baseXml = new LogOptionsConfig().Serialize();
                     break;

                  case "SmartCentralConfig":
                     baseXml = new SmartCentralConfig().Serialize();
                     break;

                  case "CustomModuleConfig":

                     baseXml = new UMSCustomModuleConfig().Serialize();
                     break;

                  case "UserPermissionProfileConfig":
                     baseXml = new UserPermissionProfileConfig().Serialize();
                     break;

                  case "SmartCentralTrendsConfig":
                     baseXml = new TrendChartConfiguration().Serialize();
                     break;

                  case "SmartCentralGridConfig":

                     baseXml = new GridConfiguration().Serialize();
                     break;

                  case "SmartCentralAlarmAggregatorsConfig":
                     baseXml = new AlarmAggregators().Serialize();
                     break;

                  case "CameraConfig":

                     baseXml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CameraConfigs xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Cameras><CameraConfig><BedId>1</BedId><BedName>ICU - 1</BedName><HomeURL>rtsp://192.168.0.10/patientbed.mp4</HomeURL><DetailURL>rtsp://192.168.0.10/patientbed.mp4</DetailURL><User>admin</User><Password>password</Password><CameraMaxYPercInMixMode>30</CameraMaxYPercInMixMode><MobileProxyProtocol>0</MobileProxyProtocol><VlcCommandLineHome><VlcCommand><CommandText>:rtsp-user=admin</CommandText></VlcCommand><VlcCommand><CommandText>:rtsp-pwd=password</CommandText></VlcCommand></VlcCommandLineHome><VlcCommandLineDetail><VlcCommand><CommandText>:rtsp-user=admin</CommandText></VlcCommand><VlcCommand><CommandText>:rtsp-pwd=password</CommandText></VlcCommand></VlcCommandLineDetail></CameraConfig><CameraConfig><BedId>2</BedId><BedName>ICU - 2</BedName><MobileURL>rtsp://192.168.0.10:1234</MobileURL><CameraMaxYPercInMixMode>40</CameraMaxYPercInMixMode><MobileProxyProtocol>0</MobileProxyProtocol><VlcCommandLineHome /><VlcCommandLineDetail /></CameraConfig></Cameras></CameraConfigs>";
                     baseXml = new CameraConfigs().Default();
                     break;

                  default:
                     break;

               }
               bolSuccess = true;
            }
            catch (XmlException xe)
            {
               messageError = xe.Message;
            }
            catch (Exception e)
            {
               messageError = e.Message;

            }
            return Json(new { errorMessage = messageError, success = bolSuccess, value = baseXml });

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      private void validateXmlByStringXsd(string webString, string baseXml)
      {
         XmlSchemaSet schemaSet = new XmlSchemaSet();
         XmlSchemaInference schema = new XmlSchemaInference();

         using (XmlReader reader = XmlReader.Create(new StringReader(baseXml))) 
         {
            schemaSet = schema.InferSchema(reader);
         }

         MemoryStream ms = new MemoryStream();
         foreach (XmlSchema s in schemaSet.Schemas())
         {
            s.Write(ms);
         }

         ms.Seek(0, SeekOrigin.Begin);
         using (StreamReader readerX = new StreamReader(ms)) 
         {
            //WTF??? Maybe for check??? 
            string icsSD = readerX.ReadToEnd();
         }
         
         XmlReaderSettings readerSettings = new XmlReaderSettings();
         readerSettings.Schemas.Add(schemaSet);
         readerSettings.ValidationType = ValidationType.Schema;
         readerSettings.ValidationEventHandler += new ValidationEventHandler(ReaderSettings_ValidationEventHandler);

         using (XmlReader readerXml = XmlReader.Create(new StringReader(webString), readerSettings)) 
         {
            //WTF??? Maybe for check??? If Read method fails returns false!!!
            while (readerXml.Read()) { }
         }            
      }

      private void ReaderSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
      {
         if (e.Severity == XmlSeverityType.Warning)
         {
            //Console.Write("WARNING: ");
            //Console.WriteLine(e.Message);
         }
         else if (e.Severity == XmlSeverityType.Error)
         {
            throw new XmlException("ERROR: " + e.Message);
         }
      }

      #endregion

      #region Locations

      public IActionResult GetLocation(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
         {
            LocationViewModel model = new LocationViewModel();

            if (id > 0)
            {
               Location objLocation = mobjLocationManager.Get(id);
               model = LocationViewModelBuilder.Build(objLocation);
            }
            return View("_Location", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveLocationDetail(LocationViewModel model)
      {
         string messageError = string.Empty;
         Location objLocation = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsEdit, CurrentUser))
            {
               if (model.ID == 0) //create
               {
                  objLocation = mobjLocationManager.Create(LocationEntityBuilder.Build(model));
               }
               else //update
               {
                  objLocation = mobjLocationManager.Update(LocationEntityBuilder.Build(model));
               }

               if (objLocation != null)
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
         catch (BedException ex)
         {
            messageError = mobjDicSvc.XLate(string.Format(ex.Message, model.LocationName));
            return Json(new { errorMessage = messageError, success = false });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }

      public JsonResult ReadLocations([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
         {
            IQueryable<Location> objAll = mobjLocationManager.GetLocations();

            DataSourceResult data = objAll.ToDataSourceResult(request, model => LocationViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }



      [HttpPost]
      public ActionResult DeleteLocation(int ID)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsEdit, CurrentUser))
            {
               if (ID > 0)
               {
                  if (mobjLocationManager.LocationCanBeDeleted(ID))
                  {
                     mobjLocationManager.Delete(ID);
                     bolSuccess = true;
                  }
                  else
                  {
                     messageError = "This location can't be deleted. Check beds, networks or operating blocks.";
                  }
               }

               return Json(new { errorMessage = messageError, success = bolSuccess });
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }

         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      public ActionResult FilterGridApplications()
      {
         return Json(UtilityHelper.GetApplicationsList());

      }

      #endregion


      #region Beds

      public IActionResult GetBed(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsView, CurrentUser))
         {
            BedViewModel model = new BedViewModel();
            if (id > 0)
            {
               Bed bed = mobjBedsManager.Get(id);
               model = BedViewModelBuilder.Build(bed);
            }
            return View("_Bed", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }
      
      public IActionResult GetBedsLocations()
      {
         List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedsManager.GetBedsWithFullData()).ToList();
         var partialViewHtml =  this.RenderViewAsync("_BedsList", model, true);
         return Json(new { content = partialViewHtml });
      }
      public IActionResult FixBedIndex()
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            int retValue = -1;
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, CurrentUser))
            {
               
                  retValue = mobjBedsManager.FixBedsIndex();
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess, retValue = retValue });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
      
      public JsonResult MoveBed(int BedID, int direction)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            int newIndex = -1;
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, CurrentUser))
            {
               if (BedID != 0)
               {
                  //Do bed movement
                  newIndex = mobjBedsManager.MoveBed(BedID, (MoveDirection)direction);
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess, indexPage = newIndex });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      public IActionResult FixLocationsIndex()
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            int retValue = -1;
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsEdit, CurrentUser))
            {
               
               retValue = mobjLocationManager.FixLocationsIndex();
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess, retValue = retValue });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
      public JsonResult MoveLocation(int LocationID, int direction)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsEdit, CurrentUser))
            {
               if (LocationID != 0)
               {
                  //Do location movement
                  mobjLocationManager.MoveLocation(LocationID, (MoveDirection)direction);
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveBedDetail(BedViewModel model)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, CurrentUser))
            {
               if (model != null)
               {
                  if (model.BedId == 0)
                  {
                     if (mobjMessageCenterSvc.BedCount >= 0)
                     {
                        if (mobjMessageCenterSvc.BedCount <= mobjBedsManager.GetQueryable().Count())
                        {
                           bolSuccess = false;
                           messageError = mobjDicSvc.XLate(CommonStrings.WARN_BEDLICENSE_LIMIT);
                           return Json(new { errorMessage = messageError, success = bolSuccess });
                        }
                     }

                     mobjBedsManager.Create(BedEntityModelBuilder.Build(model));
                  }
                  else
                  {
                     mobjBedsManager.Update(BedEntityModelBuilder.Build(model));
                  }

               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (BedException ex)
         {
            messageError = mobjDicSvc.XLate(string.Format(ex.Message, model.BedName, model.Location != null ? model.Location.LocationName : string.Empty));
            return Json(new { errorMessage = messageError, success = false });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }

      public JsonResult ReadBeds([DataSourceRequest] DataSourceRequest request, int? pageIndex)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsView, CurrentUser))
         {
            IQueryable<Bed> objAll = mobjBedsManager.GetQueryable(new List<Expression<Func<Bed, object>>>() { x => x.Location });
            if (objAll != null)
            {
               objAll = objAll.OrderBy(p => p.Location.LocationIndex).ThenBy(p => p.Index);
            }

            if (pageIndex.HasValue)
            {
               request.Page = pageIndex.Value;
            }
            DataSourceResult data = objAll.ToDataSourceResult(request, model => BedViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      public ActionResult CreateBed([DataSourceRequest] DataSourceRequest request, BedViewModel model)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, CurrentUser))
            {
               if (model != null)
               {
                  mobjBedsManager.Create(BedEntityModelBuilder.Build(model));
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
            return Json(new { errorMessage = e.Message, success = false });
         }

         //return Json((new[] { model }).ToDataSourceResult(request, ModelState));
      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult DeleteBed(int bedId)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (bedId > 0)
            {
               if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, CurrentUser))
               {
                  string strRet = mobjBedsManager.Delete(bedId);
                  if (string.IsNullOrEmpty(strRet))
                  {
                     bolSuccess = true;
                  }
                  else
                  {
                     bolSuccess = false;
                     messageError = string.Format(mobjDicSvc.XLate("Cannot delete bed {0} due to references to tables : {1} ", StringParseMethod.Js), bedId, strRet);
                  }
               }
               else
               {
                  messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               }
            }
            else
            {
               messageError = "incorrect id:{0}".FormatWith(bedId);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }


      }

      public IActionResult GetLocations(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
         {
            List<LocationViewModel> model = LocationViewModelBuilder.BuildList(mobjLocationManager.GetAll()).ToList();
            ViewBag.FormDetailId = id;
            return View("_LocationsModal", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      #endregion

      #region HospitalUnits

      public JsonResult ReadHospitalUnits([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsView, CurrentUser))
            {
               List<HospitalUnit> objAll = mobjHospitalUnitsManager.GetList().ToList();
               foreach(HospitalUnit hu in objAll)
               {
                  if (hu.ParentID != null)
                  {
                     hu.Parent = objAll.Where(p => p.GUID == hu.ParentID && hu.Current).FirstOrDefault();
                  }
               }
               DataSourceResult data = objAll.ToDataSourceResult(request, model => HospitalUnitViewModelBuilder.Build(model));
               return new JsonResult(data);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception exc)
         {
            return Json(new { errorMessage = exc.Message, success = false });
         }

      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult CreateHospitalUnit([DataSourceRequest] DataSourceRequest request, HospitalUnitViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsEdit, CurrentUser))
            {
               HospitalUnit objHospitalUnit = HospitalUnitEntityBuilder.Build(model);
               mobjHospitalUnitsManager.Create(objHospitalUnit);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json((new[] { model }).ToDataSourceResult(request, ModelState));
      }

      [HttpPost]
      public JsonResult DeleteHospitalUnit(string guid)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionHospitalUnitsEdit, CurrentUser))
            {
               if (!string.IsNullOrWhiteSpace(guid))
               {
                  bolSuccess = mobjHospitalUnitsManager.Delete(guid, out messageError);
                  if (!bolSuccess && !string.IsNullOrWhiteSpace(messageError))
                  {
                     messageError = mobjDicSvc.XLate($"Cannot delete bed with guid {guid} due to references to tables : {messageError} ", StringParseMethod.Js);
                  }
               }
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      #endregion


      #region Miscellanea
      public JsonResult ReadMiscellanea([DataSourceRequest] DataSourceRequest request)
      {
         IQueryable<Miscellanea> objAll = mobjMiscMgr.GetQueryable();
         DataSourceResult data = objAll.ToDataSourceResult(request, model => MiscellaneaViewModelBuilder.Build(model));
         return new JsonResult(data);
      }

      public IActionResult GetMiscellanea(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaView, CurrentUser))
         {
            MiscellaneaViewModel model = new MiscellaneaViewModel();
            if (id > 0)
            {
               Miscellanea misc = mobjMiscMgr.GetFromID(id);
               model = MiscellaneaViewModelBuilder.Build(misc);
            }
            return View("_Miscellanea", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      [HttpPost]
      public JsonResult SaveMiscellaneaDetail(MiscellaneaViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaView, CurrentUser))
            {
               Miscellanea objMiscellanea = MiscellaneaEntityBuilder.Build(model);
               if (objMiscellanea.Id != 0)
               {
                  mobjMiscMgr.Update(objMiscellanea);
               }
               else
               {
                  mobjMiscMgr.Create(objMiscellanea);
               }
               
            }
            else
            {
               ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }
            return Json(new { errorMessage = string.Empty, success =true });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

         
      }


      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult CreateMiscellanea([DataSourceRequest] DataSourceRequest request, MiscellaneaViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaEdit, CurrentUser))
            {
               Miscellanea objMiscellanea = MiscellaneaEntityBuilder.Build(model);

               mobjMiscMgr.Create(objMiscellanea);
            }
            else
            {
               ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }

         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json((new[] { model }).ToDataSourceResult(request, ModelState));
      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult UpdateMiscellanea([DataSourceRequest] DataSourceRequest request, MiscellaneaViewModel model)
      {

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaEdit, CurrentUser))
            {
               Miscellanea objMiscellanea = MiscellaneaEntityBuilder.Build(model);

               mobjMiscMgr.Update(objMiscellanea);
            }
            else
            {
               ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }

         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json(new[] { model }.ToDataSourceResult(request, ModelState));
      }

      //[AcceptVerbs(HttpVerbs.Post)]
      [HttpPost]
      public ActionResult DeleteMiscellanea([DataSourceRequest] DataSourceRequest request, MiscellaneaViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMiscellaneaDelete, CurrentUser))
            {
               mobjMiscMgr.Delete(model.Id);
            }
            else
            {
               ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }


         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
         }

         return Json(new[] { model }.ToDataSourceResult(request, ModelState));
      }


      #endregion


      #region SystemValidation

      public ActionResult SystemValidation()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionSystemOptionsView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > System Validation";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult ReadSystemValidations([DataSourceRequest] DataSourceRequest request)
      {
       
         List<CDASValidationViewModel> objList = CDASValidationViewModelBuilder.BuildList(mobjSysValidMgr.GetValidations()).ToList();
         return new JsonResult(objList.ToDataSourceResult(request));
      }



      public ActionResult CanValidateConfiguration()
      {
         var msgList = mobjSysValidMgr.CanValidateConfiguration();
         if (!msgList.Any())
         {
            return Json(new { errorMessage = string.Empty, success = true });
         }
         else
         {
            return Json(new { errorMessage = msgList.FirstOrDefault(), success = false });
         }
      }

      public ActionResult ValidateConfiguration(string reason)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDASValidationEdit, CurrentUser))
         {
            var msgList = mobjSysValidMgr.ValidateCurrentConfiguration(CurrentUser.UserName, reason);
            if (!msgList.Any())
            {
               return Json(new { errorMessage = string.Empty, success = true });
            }
            else
            {
               return Json(new { errorMessage = msgList.FirstOrDefault(), success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      #endregion

      #region StandardParameters
      public ActionResult StandardParameters()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > Standard Parameters";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      
      public JsonResult ReadStandardParameters([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, CurrentUser))
            {

               IQueryable<StandardParameter> objAll = mobjParMgr.GetQueryable();

               DataSourceResult data = objAll.OrderBy(f=>f.Id).ToDataSourceResult(
               request
                   //.SortAttributesMapping(SystemOptionViewModelExtensions.SortMappings)
                   //.GroupAttributesMapping(SystemOptionViewModelExtensions.GroupMappings)
                   //.FilterAttributesMapping(SystemOptionViewModelExtensions.FilterMappings)
                   , model => StandardParameterViewModelBuilder.Build(model));
               return new JsonResult(data);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception exc)
         {
            return Json(new { errorMessage = exc.Message, success = false });
         }

      }
      
      public IActionResult GetStandardParameter(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, CurrentUser))
         {
            StandardParameterViewModel model =new StandardParameterViewModel();
            if (id>0)
            {
               StandardParameter standardParameter = mobjParMgr.Get(id);
               model= StandardParameterViewModelBuilder.Build(standardParameter);
            }
            else
            {
               if (!mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEdit, CurrentUser))
                  return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
            // 2019/12/12 - No more allowed
            ViewBag.SystemModifiabled = false; //mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEditSystem,CurrentUser);
            return View("_StandardParameter", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public IActionResult RefreshStandardParameterAtConnect()
      {
         try
         {
            mobjParMgr.SendRefreshMessageToConnect();
            return Json(new { errorMessage = string.Empty, success = true });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      
      public IActionResult SaveStandardParamDetail(StandardParameterViewModel model)
      {
         string messageError = "";
         StandardParameter objStandardParameter = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEdit, CurrentUser))
            {
               var st = Newtonsoft.Json.JsonConvert.SerializeObject(model);
               if (model.IsNew) //create
               {
                  objStandardParameter = mobjParMgr.Create(StandardParameterEntityBuilder.Build(model));
                  if (objStandardParameter != null)
                     mobjLoggerService.Write(100, $"StandardParameter {model.ID} created:{st}", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);
               }
               else //update
               {
                  if (model.IsSystem ) //&& 2019/12/12 - No more allowed 
                      //!mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEditSystem, CurrentUser))
                  {
                     bolSuccess = false;
                     messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
                  }
                  else
                  {
                     
                     objStandardParameter = mobjParMgr.Update(StandardParameterEntityBuilder.Build(model));
                     
                     if (objStandardParameter != null)
                        mobjLoggerService.Write(100, $"StandardParameter {model.ID} updated:{st}", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);
                  }
                  
               }


               if (objStandardParameter != null)
               {
                  bolSuccess = true;
               }
               else
               {
                  messageError = mobjDicSvc.XLate(CommonStrings.NO_RECORD_SAVED);
               }

            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      public JsonResult CheckStandardParameterId(int id)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;

         if (id<=0)
         {
            return Json(new { errorMessage = mobjDicSvc.XLateJs("Id not valid"), success = false });
         }
         
         var objStandardParameter = mobjParMgr.Get(id);
         if (objStandardParameter != null && objStandardParameter.Id != 0)
         {
            messageError = mobjDicSvc.XLateJs("Id already used by {0}").FormatWith(objStandardParameter.Print);
            bolSuccess = false;
         }
         
         return Json(new { errorMessage = messageError, success = bolSuccess });
      }
 
      public JsonResult DeleteStandardParameter(int id)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersEdit, CurrentUser))
            {
               var model = mobjParMgr.Get(id);
               if (model == null)
               {
                  return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_RECORD_FOUND), success = false });
               }

               var st = Newtonsoft.Json.JsonConvert.SerializeObject(model);
               
               if (model.IsSystem)
               {
                  bolSuccess = false;
                  messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               }
               else
               {
                  mobjParMgr.Delete(id);
                  bolSuccess = true;
                  mobjLoggerService.Write(100, $"StandardParameter {model.Id} deleted:{st}", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);
               }

            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
      #endregion

      #region WebModules

      public JsonResult ReadWebModules([DataSourceRequest] DataSourceRequest request)
      {
         try
         {

            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWebModulesView, CurrentUser))
            {

               IQueryable<WebModule> objAll = mobjWebModulesManager.GetQueryable();

            DataSourceResult data = objAll.OrderBy(f => f.ID).ToDataSourceResult(
            request, model => WebModuleViewModelBuilder.Build(model));
            return new JsonResult(data);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception exc)
         {
            return Json(new { errorMessage = exc.Message, success = false });
         }

      }

      public ActionResult WebModules()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWebModulesView, CurrentUser))
         {
            ViewBag.SitePath = "General > System Configuration > Web Modules";
         return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult UpdateWebModules([DataSourceRequest] DataSourceRequest request, WebModuleViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWebModulesEdit, CurrentUser))
            {
               if (model != null && ModelState.IsValid)
               {
                  mobjWebModulesManager.Update(WebModuleEntityBuilder.Build(model));
                  mobjLoggerService.Write(100, $"WebModule {model.ModuleName} updated with URL {model.ModuleURL}", EventLogEntryType.Information, CurrentUser.Abbrev, 0, LogType.SYS, mobjConfiguration.ModuleName);
               }
            }
            else
            {
               ModelState.AddModelError("", mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }
         }
         catch (ArgumentException ae)
         {
            ModelState.AddModelError(ae.ParamName, ae.Message);
         }
         catch (Exception e)
         {
            ModelState.AddModelError("", e.Message);
            mobjLoggerService.ErrorException(e,"An error occurred while updating WebModules");
         }
         return Json(new[] { model }.ToDataSourceResult(request, ModelState));
      }

      #endregion WebModules


   }
}
