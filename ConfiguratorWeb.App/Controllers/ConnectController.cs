using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkWebExtensions.Controllers;
using Configurator.Std.Exceptions;
using Configurator.Std.BL;
using Configurator.Std.BL.DasDrivers;
using Configurator.Std.BL.Configurator;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.ViewModelBuilders;
using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.Extensions;
using Configurator.Std.BL.Excel;
using System.Data;
using Configurator.Std.Interfaces;
using Digistat.FrameworkStd.Exceptions;
using Digistat.FrameworkStd.Model.Ips;
using Kendo.Mvc;
using Configurator.Std.Enums;
using System.Linq.Expressions;

namespace ConfiguratorWeb.App.Controllers
{
   public class ConnectController : DigistatWebControllerBase
   {
      private readonly IConfiguratorWebConfiguration mobjConfWebConfig;
      private readonly IDeviceDrivers3Manager mobjDeviceDrivers3Manager;
      private readonly IDriverRepositoriesManager mobjRepositoriesManager;
      private readonly IDeviceDrivers3BedLinksManager mobjDeviceDrivers3BedLinkManager;
      private readonly IDriverManager mobjDasDriverManager;
      private readonly IDriverMonitorService mobjDriverMonitorSvc;
      private readonly IActualDevicesManager mobjActualDevicesManager;
      private readonly IActualDeviceImagesManager mobjActualDeviceImagesManager;
      private readonly IDasOutputStateManager mobjDasOutputStateManager;
      private readonly IBedsManager mobjBedsManager;
      private readonly ILocationManager mobjLocationsManager;
      private readonly IStandardDeviceTypesManager mobjDevTypeMgr;
      private readonly IPermissionsService mobjPermSvc;
      private readonly IDigistatEnvironmentService mobjEnvSvc;
      private readonly IDvdDrvLinkManager mobjDvdDrvLinkMgr;
      private readonly IWaveformManager mobjWaveformManager;
      private readonly IDriverRepositoriesManager mobjDriverRepositoryManager;
      private readonly IStandardParametersManager mobjStandardParametersManager;
      private readonly string mstrCDSS_HARDWARE_ID ="ASCOM CDSS SERVER";
      
      public ConnectController(IConfiguratorWebConfiguration config, IMessageCenterService msgcenter, ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc,
       IDeviceDrivers3Manager deviceDriverManager, IDeviceDrivers3BedLinksManager deviceDriverBedlinkManager, IDriverRepositoriesManager repManager, IDriverManager dasDriverManager, IDriverMonitorService driverMonitorSvc,
       IActualDevicesManager actualDevicesManager, IPermissionsService permSvc, IActualDeviceImagesManager actualDeviceImagesManager, IDasOutputStateManager dasOutputStateManager,
       IBedsManager bedsManager, ILocationManager locationsManager, IStandardDeviceTypesManager stdDevTypeMgr,IDigistatEnvironmentService EnvSvc,
       IDvdDrvLinkManager dvdDrvLinkMgr, IWaveformManager waveformManager, IDriverRepositoriesManager driverRepositoryManager, IStandardParametersManager StandardParametersManager)
    : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjConfWebConfig = config;
         mobjDeviceDrivers3Manager = deviceDriverManager;
         mobjDeviceDrivers3BedLinkManager = deviceDriverBedlinkManager;
         mobjRepositoriesManager = repManager;
         mobjDasDriverManager = dasDriverManager;
         mobjDriverMonitorSvc = driverMonitorSvc;
         mobjActualDevicesManager = actualDevicesManager;
         mobjActualDeviceImagesManager = actualDeviceImagesManager;
         mobjDasOutputStateManager = dasOutputStateManager;
         mobjBedsManager = bedsManager;
         mobjLocationsManager = locationsManager;
         mobjDevTypeMgr = stdDevTypeMgr;
         mobjPermSvc = permSvc;
         mobjEnvSvc = EnvSvc;
         mobjDvdDrvLinkMgr = dvdDrvLinkMgr;
         mobjWaveformManager = waveformManager;
         mobjDriverRepositoryManager = driverRepositoryManager;
         mobjStandardParametersManager = StandardParametersManager;  
      }

      // GET: Connect (Never used????)
      public ActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Index";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      #region Drivers

      public ActionResult Drivers()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Drivers";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public ActionResult GetDriver(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            DriverViewModel model = new DriverViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
               DriverRepository objDriver = mobjRepositoriesManager.Get(id, true, true);

               //ViewBag.EventClassList = UtilityHelper.EnumToStringList<Digistat.FrameworkStd.Enums.AlarmClass>(true, Digistat.FrameworkStd.Enums.AlarmClass.Other);
               //ViewBag.EventLevelList = UtilityHelper.EnumToStringList<Digistat.FrameworkStd.Enums.EventType>(true, Digistat.FrameworkStd.Enums.EventType.None);

               //ViewBag.EventClassList = UtilityHelper.DictionaryToListSelectItem(
               //   UMSFrameworkParser.GetAlarmClassOptionList().ToDictionary(x => x.Value, x => x.Value), 
               //   true,
               //   UMSFrameworkParser.GetAlarmClassDescription(UMSFrameworkParser.GetAlarmClassDefaultValue()));
               //ViewBag.EventLevelList = UtilityHelper.DictionaryToListSelectItem(
               //   UMSFrameworkParser.GetEventTypeOptionList().ToDictionary(x => x.Value, x => x.Value), 
               //   true,
               //   UMSFrameworkParser.GetAlarmClassDescription(UMSFrameworkParser.GetEventTypeDefaultValue()));

               model = DriverViewModelBuilder.Build(objDriver, mobjDicSvc);
            }
            ViewBag.CDSS_ID = "driver00-CDSS-read-only-000000000000";
            ViewBag.CDSS_HARDWARE_ID = mstrCDSS_HARDWARE_ID; ;

            return PartialView("_DriverTabStrip", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }
      public JsonResult ReadDrivers([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            IQueryable<DriverRepository> objAll = null;
            objAll = mobjRepositoriesManager.GetQueryable()
            .Where(a => a.Current)
             .Select(x => new DriverRepository
             {
                    //AlarmSupport = x.AlarmSupport,
                    AlarmSystemType = x.AlarmSystemType,
                    //AlarmSystemTypeDescription = x.AlarmSystemTypeDescription,
                    //Capabilities = x.Capabilities,
                    //ComToRegister = x.ComToRegister,
                    Current = x.Current,
                    //DefaultCommConfiguration = x.DefaultCommConfiguration,
                    Device = x.Device,
                DeviceType = x.DeviceType,
                DeviceTypeDesc = x.DeviceTypeDesc,
                DriverName = x.DriverName,
                DriverVersion = x.DriverVersion,
                DriverVersionBuild = x.DriverVersionBuild,
                    //EntryExe = x.EntryExe,
                    //EventsMapping = x.EventsMapping,
                    //FileCount = x.FileCount,
                    //FormatStyle = x.FormatStyle,
                    //HardwareRelease = x.HardwareRelease,
                    Id = x.Id,
                    //IsBinFile = x.IsBinFile,
                    //IsWrapper = x.IsWrapper,
                    //LastStreamUpdate = x.LastStreamUpdate,
                    Manufacturer = x.Manufacturer,
                Model = x.Model,
                Note = x.Note,
                    //RemappedEvents = x.RemappedEvents,
                    //  RunAsDLL = x.RunAsDLL,
                    //SoftwareRelease = x.SoftwareRelease,
                    //UseDynamicParameters = x.UseDynamicParameters,
                    //ValidToDate = x.ValidToDate,
                    Version = x.Version
             })
            .OrderBy(b => b.DriverName);

            DataSourceResult data = null;
            try
            {
               request.RenameRequestFilterMember("DriverModel", "Model");
               //request.RenameRequestFilterMember("AlarmSystemTypeDescription", "AlarmSystemType");
               var sort = new Dictionary<string, string[]>()
                  {
                     {"DriverModel", new []{"Model"}}
                  };
               data = objAll.ToDataSourceResult(request.SortAttributesMapping(sort), model => DriverViewModelBuilder.Build(model, mobjDicSvc));
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadDrivers");
               return Json(new { errorMessage = "Error on ReadDrivers", success = false });
            }

            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      [HttpPost]
      [RequestFormSizeLimit(valueCountLimit: 200000)]
      public JsonResult SaveDriverDetail(DriverViewModel model)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {

               if (!string.IsNullOrWhiteSpace(model.EventCatalogSerialize))
               {
                  model.EventCatalog = JsonConvert.DeserializeObject<IEnumerable<DriverEventCatalogViewModel>>(model.EventCatalogSerialize);
               }

               var duplicatedCapabilities = model.Capabilities.Where(x => x.MustBeSaved == true).GroupBy(x => new { x.IdParameter, x.StandardParameterIDAlias }).Select(g =>
                  new { g.Key.IdParameter, g.Key.StandardParameterIDAlias, MyCount = g.Count() }).Where(x => x.MyCount > 1);
               if (duplicatedCapabilities != null && duplicatedCapabilities.Any())
               {
                  return Json(new { errorMessage = "Some capabilities are duplicated (ex: {0})".FormatWith(duplicatedCapabilities.FirstOrDefault().IdParameter), success = bolSuccess });
                  //throw new ArgumentException("Some capabilities are duplicated (ex: {0})".FormatWith(duplicatedCapabilities.FirstOrDefault().IdParameter), "");
               }
               //if (model.Capabilities.Count() ==0 && !string.IsNullOrWhiteSpace(model.CapabilitiesSerialize))
               //{
               //   model.Capabilities = JsonConvert.DeserializeObject<IEnumerable<DriverCapabilityViewModel>>(model.CapabilitiesSerialize);
               //}
               //Check if both or none values are set for NewClass and NewLevel
               if (model.EventCatalog.Any(x => (x.DriverEventClass.ClassId == -1) != (x.DriverEventLevel.LevelId == -1)))
               {
                  throw new ArgumentException("Remapped event Class and Remapped Event Level must be both configured!");
               }

               if (!string.IsNullOrEmpty(model.DefaultCommConfiguration))
               {
                  model.DefaultCommConfiguration = System.Net.WebUtility.HtmlDecode(model.DefaultCommConfiguration);
               }

               DriverRepository objDriver = DriverEntityBuilder.Build(model);
               //Reload file
               bool bolFileHasChanged = false;
               if (!string.IsNullOrEmpty(model.BinariesCacheIdentifier))
               {
                  string tempDir = Path.Combine(mobjConfWebConfig.DasDriversCachePath, model.BinariesCacheIdentifier);
                  //Load Stream
                  objDriver.Stream = mobjDasDriverManager.GetFileStream(objDriver, model.BinariesCacheIdentifier);
                  if (objDriver.Stream != null)
                  {
                     objDriver.StreamSize = objDriver.Stream.Length;
                     objDriver.LastStreamUpdate = DateTime.Now;
                     //CA1304 e CA1307 consigliano name.EndsWith(".LOG", StringComparison.InvariantCultureIgnoreCase) non lo sostituisco per non ritestare.... 
                     // ....alla prima occasione sarebbe meglio farlo
                     objDriver.FileCount = System.IO.Directory.GetFiles(tempDir).Where(name => !name.ToUpper().EndsWith(".LOG")).Count();
                     bolFileHasChanged = true;
                  }
               }

               if (string.IsNullOrWhiteSpace(model.Id)) //create
               {
                  if (model.HardwareRelease == mstrCDSS_HARDWARE_ID && mobjRepositoriesManager.CheckCdssAlreadyExists())
                  {
                     return Json(new { errorMessage = mobjDicSvc.XLate("An CDSS Server Driver already exist."), success = false });
                  }
                  mobjRepositoriesManager.Create(objDriver);
               }
               else //update
               {
                  if (model.HardwareRelease == mstrCDSS_HARDWARE_ID && model.Id != mobjRepositoriesManager.GetCdssGuid())
                  {
                     return Json(new { errorMessage = mobjDicSvc.XLate("An CDSS Server Driver already exist."), success = false });
                  }
                  mobjRepositoriesManager.Update(objDriver, bolFileHasChanged);
               }

               //Remove cached files
               if (bolFileHasChanged || string.IsNullOrWhiteSpace(model.Id))
               {
                  //If it is not uploaded a new driver file, is not necessary
                  mobjDasDriverManager.RemoveCachedDriver(model.BinariesCacheIdentifier);
               }
               
               
               bolSuccess = true;
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });

         }
         catch (ConnectException cex)
         {
            return Json(new { errorMessage = cex.Message, success = false, show = true });
         }
         catch (ArgumentException aex)
         {
            return Json(new { errorMessage = aex.Message, success = false, show = true });
         }
         catch (Exception ex)
         {

            return Json(new { errorMessage = ex.Message, success = false, show = false });
         }
      }

      [HttpPost]
      public JsonResult DeleteDriver(string id)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {

               mobjRepositoriesManager.Remove(id);
               bolSuccess = true;
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            messageError = ex.Message;
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }

      }


      [HttpPost]
      public ActionResult ExportFromGrid(string contentType, string base64, string fileName)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      public ActionResult ExportEventsCatalog(string contentType, string base64, string fileName)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public FileResult ExportEventCatalog(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            DriverRepository driver = mobjRepositoriesManager.Get(id, false, true);

            if (driver == null)
            {
               throw new Exception(string.Format("Unable to find driver with id {0}", id));
            }


            var file = new EventMappingExport(mobjLogSvc).GetDriverRepositoryXls(driver);

            return File(file.Content, file.Mimetype, file.Name);
         }
         else
         {
            return null;
         }

      }

      [RequestFormSizeLimit(valueCountLimit: 200000)]
      public ActionResult ImportEventsCatalog(IEnumerable<IFormFile> EventsCatalog)
      {

         try
         {
            if (mobjEnvSvc.PreventDeviceEventRemapping)
            {
               throw new Exception(mobjDicSvc.XLate("Import function is not allowed."));
            }
            // The Name of the Upload component is "files"
            if (EventsCatalog == null)
            {
               throw new Exception(mobjDicSvc.XLate("Excel file not selected"));
            }

            var export = EventsCatalog.First();
            var model = new EventMappingExport(mobjLogSvc).GetDriverRepositoryFromXls(export.OpenReadStream(), Path.GetExtension(export.FileName));

            // Return an empty string to signify success
            return Json(DriverViewModelBuilder.Build(model, mobjDicSvc));
            //return Json(model);
         }
         catch (Exception)
         {

            throw;
         }
      }

      //[HttpPost]
      //public ActionResult Export_Capabilities(string contentType, string base64, string fileName)
      //{
      //   var fileContents = Convert.FromBase64String(base64);

      //   return File(fileContents, contentType, fileName);
      //}

      public FileResult DownloadDriverFiles(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            var file = mobjRepositoriesManager.DownloadDriver(id);
            return File(file.Content, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
         }
         else
         {
            return null;
         }
      }

      public FileResult ExportDriverFiles(string id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            var file = mobjRepositoriesManager.ExportDriver(id);
            return File(file.Content, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
         }
         else
         {
            return null;
         }

      }

      [RequestFormSizeLimit(valueCountLimit: 200000)]
      public ActionResult UploadDriverFiles(DriverViewModel model)
      {
         string errMsg = string.Empty;
         //ViewBag.CDSS_ID ="driver00-CDSS-read-only-000000000000";
         try
         {


            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               if (model.DriverFiles == null)
               {
                  throw new Exception(mobjDicSvc.XLate("No driver files received", Digistat.FrameworkStd.Enums.StringParseMethod.Js));
               }

               var driverFiles = model.DriverFiles.Select(x => new Configurator.Std.BL.DasDrivers.CachedFile(System.IO.Path.GetFileName(x.FileName), x.OpenReadStream())).ToList();

               model.BinariesCacheIdentifier = string.IsNullOrWhiteSpace(model.Id) ? Guid.NewGuid().ToString() : model.Id;

               string strFileID = model.BinariesCacheIdentifier;


               DriverRepository driver = DriverEntityBuilder.Build(model);


               //By default remapped events are kept, so last parameter is true
               DriverRepository updatedDriver = mobjRepositoriesManager.UploadDriver(model.BinariesCacheIdentifier, driver, driverFiles, model.KeepCapabilities, model.KeepSmartCentralFormatString, true);
               var oldCapabilities = model.Capabilities;

               model = DriverViewModelBuilder.Build(updatedDriver, mobjDicSvc);
               model.BinariesCacheIdentifier = strFileID;
               model.DriverFilesStatusDescription = "Driver files modified";

            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

         }
         catch (Exception e)
         {

            if (e.InnerException != null && e.InnerException is ConnectDriverLoadException)
            {
               ConnectDriverLoadException cdExc = (ConnectDriverLoadException)e.InnerException;
               switch (cdExc.ErrorType)
               {
                  case DriverLoadErrorType.CommunicationTimeout:
                     errMsg = "A timeout occurred while waiting for driver data. This may be due to network issues";
                     break;
                  case DriverLoadErrorType.EmptyMessageReceived:
                     errMsg = "No data received from driver. This may be due to network issues";
                     break;
                  case DriverLoadErrorType.NoPortAvailable:
                     errMsg = "No port available for TCP Listener. See logs for details.";
                     break;
                  case DriverLoadErrorType.IncorrectMessageReceived:
                     errMsg = "Incorrect message received from Driver. Cannot parse content.";
                     break;
               }
            }
            else
            {
               errMsg = e.Message;
            }
            return Json(new { success = false, errorMessage = errMsg, content = "" });

         }

         return Json(new { success = true, errorMessage = "", content = this.RenderViewAsync("_DriverTabStrip", model, true) });

         //return PartialView("_DriverTabStrip", model);
      }
      public IActionResult GetEventsList(string driverId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            /*List<LocationViewModel> model = LocationViewModelBuilder.BuildList(mobjDr..GetAll()).ToList();
            ViewBag.FormDetailId = id;*/
            /*return View("_EventsModal", model);*/
            return null;

         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      #endregion

      #region DeviceDrivers

      public ActionResult DeviceDriver()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Device Driver Management";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public ActionResult GetDeviceDriver(int id)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
            {
               DeviceDriverViewModel model;

               var beds = mobjBedsManager.GetBedsWithFullData().ToList();

               if (id != 0)
               {
                  DeviceDriver3 deviceDriver = mobjDeviceDrivers3Manager.Get(id);
                  model = DeviceDriverViewModelBuilder.Build(deviceDriver, beds, mobjDicSvc);
               }
               else
               {
                  model = new DeviceDriverViewModel(beds);
               }
               return PartialView("_DeviceDriverTabStrip", model);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         catch(Exception e)
         {
            mobjLogSvc.ErrorException(e, $"Error on GetDeviceDriver for id {id}");
            throw;
         }         
      }

      public JsonResult GetStandardDeviceTypesForList()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            List<StandardDeviceType> objDevTypes = mobjDevTypeMgr.GetAll().ToList();
            Dictionary<string, string> objDicDevs = new Dictionary<string, string>();
            objDicDevs.Add("DEVICE TYPES", "");
            foreach (StandardDeviceType std in objDevTypes)
            {
               objDicDevs.Add(std.Description, "\\dev{" + std.Id.ToString() + "}");
            }
            SelectList objRet = new SelectList(objDicDevs, "Value", "Key");
            return new JsonResult(objRet);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetStandardDeviceTypeAsDictionary()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, CurrentUser))
         {
            List<StandardDeviceType> objDevTypes = mobjDevTypeMgr.GetAll().ToList();
            Dictionary<string, string> objDicDevs = new Dictionary<string, string>();
            foreach (StandardDeviceType std in objDevTypes)
            {
               objDicDevs.Add(std.Id.ToString(), std.Description);
            }
            SelectList objRet = new SelectList(objDicDevs, "Value", "Key");
            return new JsonResult(objRet);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult ReadDeviceDrivers([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            IQueryable<DeviceDriver3> objAll = null;
            objAll = mobjDeviceDrivers3Manager.GetDeviceDrivers();

            var sort = new Dictionary<string, string[]>()
            {
               {"Name", new []{"Repository.DriverName"}},
               {"BedLink", new []{"BedLinks.First().Bed?.Name"}},
               {"Description", new []{"DeviceName"}},
               {"Address", new []{"CommConfigurationObject.Hostname","CommConfigurationObject.ComPort"}},
               {"Version", new []{"Repository.DriverVersion"}}
            };

            if (request.Filters.Count > 0)
            {
               request.RenameRequestFilterMember("Name", "Repository.DriverName");
               request.RenameRequestFilterMember("Description", "DeviceName");
               request.RenameRequestFilterMember("Address", "CommConfigurationObject.Hostname");
               request.RenameRequestFilterMember("Version", "Repository.DriverVersion");
            }

            DataSourceResult data = null;
            try
            {
               /////WARNING
               //This portion of code loads DeviceDriversBedsLinks after kendo grid loading to add this entity to the output.
               //This nonsensical logic is necessary because Entity Framework 3.1 does not parse linq JoinGroup method.
               //To permit DB side filtering / paging and grouping I load plain results using Kendo Queryable Extensions, then I integrate the resultset with separately loaded BedLinks.
               data = objAll.ToDataSourceResult(request.SortAttributesMapping(sort));

               IEnumerable<DeviceDriver3> res = (IEnumerable<DeviceDriver3>)data.Data;
               IEnumerable<DeviceDriver3BedLink> bedlinks = new List<DeviceDriver3BedLink>();

               if (res.Any())
               {
                  bedlinks = mobjDeviceDrivers3BedLinkManager.GetByDeviceDriverIds(res.Select(x => x.Id), true);
               }
               
               data.Data = res.Select(model => DeviceDriverListitemModelBuilder.Build(model, bedlinks.Where(y => y.DeviceDriverId == model.Id), mobjDicSvc));               
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadDeviceDrivers");
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false });
            }
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      [RequestFormSizeLimit(valueCountLimit: 500000)]
      public JsonResult SaveDeviceDriverDetail(DeviceDriverViewModel model)
      {
         //update YES/NO props
         string messageError = string.Empty;
         bool bolSuccess = true;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               DriverCommConfiguation config = mobjRepositoriesManager.GetConfiguration(model.IdDriverRepository);
               IEnumerable<CustomParametersViewModel> customParameters = new List<CustomParametersViewModel>();

               if (model.CustomParametersJson != null && model.CustomParametersJson.Length > 0)
               {
                  customParameters = JsonConvert.DeserializeObject<IEnumerable<CustomParametersViewModel>>(model.CustomParametersJson);
                  var customParameterNames = customParameters.Select(cp => cp.Name).ToList();
                  if (customParameterNames.Distinct().Count() != customParameterNames.Count())
                  {
                     throw new Exception(mobjDicSvc.XLate("Comunication custom parameter with same name is not allowed."));
                  }
               }

               model.CustomParameters = customParameters;

               if (model.BedAssociation == null)
               {
                  List<Bed> beds = mobjBedsManager.GetBedsWithFullData().ToList();
                  IEnumerable<BedAssociationViewModel> lstBedAssociation = beds.Select(x => new BedAssociationViewModel
                  {
                     BedId = x.Id,
                     LocationId = x.IdLocation.Value,
                     Bedcode = x.BedCode,
                     BedName = x.Name,
                     Location = x.Location.LocationName,
                     DriverSideBedName = string.Empty,
                     Watchdog = false,
                     Enabled = false,
                  }).OrderBy(x => x.Location).ThenBy(x => x.BedName);
                  IEnumerable<DeviceDriver3BedLink> lstAssociatedBed = new List<DeviceDriver3BedLink>();

                  if (model.BedLinkAssociationSerialize != null && model.BedLinkAssociationSerialize.Length > 0)
                  {
                     lstAssociatedBed = JsonConvert.DeserializeObject<IEnumerable<DeviceDriver3BedLink>>(model.BedLinkAssociationSerialize);
                  }

                  var listBeds = DeviceDriverViewModelBuilder.UpdateBedsAssociations(lstBedAssociation, lstAssociatedBed);

                  if (model.BedAssociationChanged != null && model.BedAssociationChanged.Length > 0)
                  {
                     DataSet changedData = JsonConvert.DeserializeObject<DataSet>("{" + String.Format(" \"Tab\" : {0} ", model.BedAssociationChanged) + "}");
                     foreach (DataRow x in changedData.Tables[0].Rows)
                     {
                        var item = listBeds.Find(a => a.BedId == x["BedID"].ToString().To<int>());
                        if (item != null)
                        {
                           item.Enabled = (bool)x["Enabled"];
                           item.DriverSideBedName = x["DriverSideBedName"].ToString();
                           item.Watchdog = (bool)x["Watchdog"];
                        }
                     }
                  }
                  model.BedAssociation = listBeds;
               }
               DeviceDriver3 deviceDriverModel = DeviceDriverEntityBuilder.Build(model, config);

               if (mobjDeviceDrivers3Manager.CheckCableIDAlreadyExists(deviceDriverModel, model.Id))
               {
                  messageError = mobjDicSvc.XLate(string.Format("SMART CABLE ID {0} already exists.", deviceDriverModel.CommConfigurationObject.SmartCableId));
                  bolSuccess = false;
               }
               else
               {
                  DeviceDriver3 ret = null;
                  if (model.Id == 0) //create
                  {
                     ret= mobjDeviceDrivers3Manager.Create(deviceDriverModel);
                  }
                  else //update
                  {
                     ret = mobjDeviceDrivers3Manager.Update(deviceDriverModel);
                  }
                  //TODO: SAVE "dvd_drv_link"
                  if (ret != null)
                  {
                     //var test = mobjDvdDrvLinkMgr.GetAll();
                     if (!mobjDvdDrvLinkMgr.Exists(f => f.drv_Id == ret.IdDriverRepository && f.dvd_ID == ret.Id))
                     {
                        var ddl = new DeviceDriver_Driver_Link
                        {
                           drv_Id = ret.IdDriverRepository,
                           dvd_ID = ret.Id
                        };
                        mobjDvdDrvLinkMgr.Create(ddl);
                     }
                  }                  
                  bolSuccess = true;
               }
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            bolSuccess = false;
            messageError = ex.Message;
         }

         return Json(new { errorMessage = messageError, success = bolSuccess });
      }

      [HttpPost]
      public JsonResult DeleteDeviceDriver(string id)
      {
         //update YES/NO props
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               if (!string.IsNullOrWhiteSpace(id) && int.TryParse(id, out int _id))
               {
                  mobjDeviceDrivers3Manager.Delete(_id);
                  bolSuccess = true;
               }
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            ModelState.AddModelError("ErrorMessage", ex.Message);
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      #endregion

      #region ActualDevices

      public ActionResult ActualDevice()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Actual Device";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }

      }
      private void ModifyFiltersActualDevices(IEnumerable<IFilterDescriptor> filters)
      {
         var lstDeviceType = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDeviceTypeEnumList();
         for (var index = 0; index < lstDeviceType.Count; index++)
         {
            var valueTuple = lstDeviceType[index];
            valueTuple.Item1 = mobjDicSvc.XLate(valueTuple.Item1, Digistat.FrameworkStd.Enums.StringParseMethod.Html);
         }

         if (filters.Any())
         {
            foreach (var filter in filters)
            {
               var descriptor = filter as FilterDescriptor;
               if (descriptor != null && descriptor.Member == "DeviceTypeDescription")
               {
                  descriptor.Member = "DeviceType";
                  var en = lstDeviceType.FirstOrDefault(delegate((string, int) tuple)
                  {
                     return tuple.Item1 == descriptor.Value.ToString();

                  });
                  if (en.ToTuple() != null)
                  {
                     descriptor.Value = en.Item2.ToString();
                  }

               }
               else if (filter is CompositeFilterDescriptor)
               {
                  ModifyFiltersActualDevices(((CompositeFilterDescriptor)filter).FilterDescriptors);
               }
            }
         }
      }
      public JsonResult ReadActualDevices([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {

            IQueryable<ActualDevice> objAll = mobjActualDevicesManager.GetQueryable();

            var sort = new Dictionary<string, string[]>()
            {
               {"DeviceTypeDescription", new []{"DeviceType"}}
            };

            if (request.Filters.Any())
            {
               ModifyFiltersActualDevices(request.Filters);
            }

            DataSourceResult data = null;
            try
            {
               data = objAll.ToDataSourceResult(request.SortAttributesMapping(sort));

               var res = (IEnumerable<ActualDevice>)data.Data;
               var thumbs = new Dictionary<int, DeviceImageInfo>();

               if (res.Any())
               {
                  thumbs = mobjActualDeviceImagesManager.GetThumbnailForDevices(res);
                 
               }
               if (thumbs == null)
               {
                  thumbs = new Dictionary<int, DeviceImageInfo>();
               }
               data.Data = res.Select(
                  model => ActualDeviceViewModelBuilder.Build(model, 
                     mobjDicSvc, thumbs.ContainsKey(model.Id) ? thumbs[model.Id] : null));
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadActualDrivers");
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false });
            }
            return new JsonResult(data);
            

            //DataSourceResult data = objAll.ToDataSourceResult(request.SortAttributesMapping(sort), model => ActualDeviceViewModelBuilder.Build(model, mobjDicSvc, mobjActualDeviceImagesManager));
            //return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult UpdateActualDevice([DataSourceRequest] DataSourceRequest request, ActualDeviceViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               if (model != null && ModelState.IsValid)
               {
                  mobjActualDevicesManager.Update(ActualDeviceEntityBuilder.Build(model));
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
         }
         return Json(new[] { model }.ToDataSourceResult(request, ModelState));
      }

      #endregion

      #region DeviceDriverImages

      public ActionResult ActualDeviceImages()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Actual Device Images";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult ReadActualDeviceImages([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            IQueryable<ActualDeviceImage> objAll = mobjActualDeviceImagesManager.GetQueryable().Select(x => new ActualDeviceImage
            {
               ActualDeviceName = x.ActualDeviceName,
               ActualDeviceSerial = x.ActualDeviceSerial,
               DeviceType = x.DeviceType,
               Extension = x.Extension,
               ImageVersion = x.ImageVersion,
               Thumbnail = x.Thumbnail
            });
            var sort = new Dictionary<string, string[]>()
            {
               {"DeviceTypeDescription", new []{"DeviceType"}},
               {"DeviceSerialNumber", new []{"ActualDeviceSerial"}},
               {"DeviceName", new []{"ActualDeviceName"}}
            };
            DataSourceResult data = objAll.ToDataSourceResult(request.SortAttributesMapping(sort), model => ActualDeviceImageViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      public ActionResult GetActualDeviceImage(int deviceType, string deviceName, string deviceSerial)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ActualDeviceImageViewModel model;

            //For backward compatibility
            if (deviceName == null) { deviceName = string.Empty; }
            if (deviceSerial == null) { deviceSerial = string.Empty; }

            if (deviceType != 0)
            {
               ActualDeviceImage actualImage = mobjActualDeviceImagesManager.Get(deviceType, deviceName, deviceSerial, true);
               model = ActualDeviceImageViewModelBuilder.Build(actualImage);
            }
            else
            {
               model = new ActualDeviceImageViewModel();
            }

            return PartialView("_ActualDeviceImageDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      [HttpPost]
      public JsonResult SaveActualDeviceImageDetail(ActualDeviceImageViewModel model)
      {
         //update YES/NO props
         string messageError = string.Empty;
         bool bolSuccess = true;
         string tempDir = string.Empty;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               ActualDeviceImage objImage = ActualDeviceImageEntityBuilder.Build(model);

               //Reload file
               if (!string.IsNullOrEmpty(model.BinariesCacheIdentifier))
               {
                  tempDir = Path.Combine(mobjConfWebConfig.DeviceImageCachePath, model.BinariesCacheIdentifier);

                  string fullFileName = System.IO.Directory.GetFiles(tempDir, "*.*").FirstOrDefault();
                  if (!string.IsNullOrEmpty(fullFileName))
                  {
                     objImage.Image = System.IO.File.ReadAllBytes(fullFileName);
                  }

                  if (objImage.Image != null)
                  {
                     objImage.Extension = Path.GetExtension(fullFileName).TrimStart('.');
                     objImage.Thumbnail = ConversionsHelper.ImageToThumbnail(objImage.Image);
                     objImage.ImageVersion = 1;
                  }
               }

               if (model.IsNewRecord) //create
               {
                  mobjActualDeviceImagesManager.Create(objImage);
               }
               else //update
               {
                  mobjActualDeviceImagesManager.Update(objImage);
               }

               if (!string.IsNullOrWhiteSpace(tempDir))
               {
                  Directory.Delete(tempDir, true);
               }
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });

         }
         catch (Exception ex)
         {
            bolSuccess = false;
            messageError = ex.Message;
         }

         return Json(new { errorMessage = messageError, success = bolSuccess });

      }

      [HttpPost]
      public JsonResult DeleteActualDeviceImage(int deviceType, string deviceName, string deviceSerialNumber)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               mobjActualDeviceImagesManager.Delete(deviceType, deviceName, deviceSerialNumber);
               return Json(new { errorMessage = string.Empty, success = true });
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


      [RequestFormSizeLimit(valueCountLimit: 200000)]
      public ActionResult UploadDeviceImage(IEnumerable<IFormFile> ImageFile)
      {

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               // The Name of the Upload component is "files"
               if (ImageFile == null)
               {
                  throw new Exception("Image not selected");
               }

               var image = ImageFile.First();

               var imageFile = ConversionsHelper.StreamToByteArray(image.OpenReadStream());  //model.DriverFiles.Select(x => new Configurator.Std.BL.DasDrivers.CachedFile(x.FileName, x.OpenReadStream())).ToList();

               string strFileID = Guid.NewGuid().ToString();

               mobjActualDeviceImagesManager.UploadImage(strFileID, image.FileName, imageFile);


               // Return an empty string to signify success
               return Json(new { BinariesCacheIdentifier = strFileID });
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         catch (Exception)
         {

            throw;
         }


      }




      public FileResult DownloadDeviceImage(int deviceType, string deviceName, string deviceSerialNumber)
      {
         var file = mobjActualDeviceImagesManager.DownloadImage(deviceType, deviceName, deviceSerialNumber);

         return File(file.Content, file.Mimetype, file.Name);
      }

      #endregion

      #region Monitor

      public ActionResult Monitor()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Monitor";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public async Task<IActionResult> GetDasInstancesList()
      {
         try
         {
            var data = (await mobjDriverMonitorSvc.GetDasInstances()).Select(x => new { ComputerName = x.ComputerName, Name = x.Name, Version = x.Version, DisplayName = x.Name + " " + x.Version });

            return Json(data);
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

      public async Task<IActionResult> GetDasInstanceStatus(string computerName)
      {
         try
         {
            var resultSet = await mobjDriverMonitorSvc.GetDasInstanceStatus(computerName);

            var data = DasStatusViewModelBuilder.BuildList(resultSet, mobjBedsManager, mobjDicSvc);

            return Json(data);
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

      public IActionResult RestartDriver(int deviceDriverId, int processId, string dasBroker)
      {
         try
         {
            mobjDriverMonitorSvc.RestartDriver(deviceDriverId, processId, dasBroker);
            return Json("SUCCESS");
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Request driver {0}, process {1} to restart on broker {2}", deviceDriverId, processId, dasBroker);
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      public IActionResult KillProcess(int deviceDriverId, int processId, string dasBroker)
      {
         try
         {
            mobjDriverMonitorSvc.KillProcess(deviceDriverId, processId, dasBroker);
            return Json("SUCCESS");
         }
         catch (Exception ex)
         {
            mobjLogSvc.ErrorException(ex, "Request driver {0}, process {1} to restart on broker {2}", deviceDriverId, processId, dasBroker);
         }
         return StatusCode(StatusCodes.Status500InternalServerError);
      }

      public IActionResult GetDASBrokerList(string selectedDASBroker, string idField, string idModel)
      {
         ViewBag.selectedDASBroker = selectedDASBroker;
         ViewBag.IdToSet = idField;
         ViewBag.idModel = idModel;
         return View("_DASBrokers");
      }

      #endregion

      #region Collect

      public ActionResult Collect()
      {

         var requestCultureFeature = Request.HttpContext.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();

         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Collect";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public ActionResult GetOutputState(int locationId, int bedId, int patientId, bool isNew)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            DasOutputStateViewModel model = new DasOutputStateViewModel();
            if (locationId >= 0 && bedId >= 0 && !isNew)
            {
               Digistat.FrameworkStd.Model.DAS3Plus.DasOutputState obj = mobjDasOutputStateManager.Get(locationId, bedId, patientId);
               model = OutputStateViewModelBuilder.Build(obj, mobjDicSvc);
            }

            return PartialView("_OutputStateDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      public JsonResult ReadOutputStates([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            IQueryable<Digistat.FrameworkStd.Model.DAS3Plus.DasOutputState> objAll = mobjDasOutputStateManager.GetDasOutputStates();

            //check if main row existits, it not insert it
            if (objAll.Where(a => a.BedId == 0 && a.LocationId == 0 && a.PatientId == 0).FirstOrDefault() == null)
            {
               Digistat.FrameworkStd.Model.DAS3Plus.DasOutputState obj = OutputStateEntityBuilder.Build(new DasOutputStateViewModel() { LocationId = 0, PatientId = 0, BedId = 0 });
               mobjDasOutputStateManager.Create(obj);
            }

            var sort = new Dictionary<string, string[]>()
            {
               {"TypeDescription", new []{"Type"} },
               {"LocationDescription", new []{ "Location.LocationName" } },
               {"BedDescription", new []{"Bed.Name" } },
               {"PatientDescription", new []{ "Patient.FamilyName" , "Patient.GivenName" } },
            };

            DataSourceResult data = null;
            try
            {
               data = objAll.ToDataSourceResult(request.SortAttributesMapping(sort), model => OutputStateViewModelBuilder.Build(model, mobjDicSvc, true));
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadOutputStates");
               return Json(new { errorMessage = "Error on ReadOutputStates", success = false });
            }

            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      [HttpPost]
      public JsonResult SaveOutputStateDetail(DasOutputStateViewModel model)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (!model.IsSystem)
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.CANNOT_BE_MODIFIED), success = false });
            }
            //get model by key
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               Digistat.FrameworkStd.Model.DAS3Plus.DasOutputState obj = OutputStateEntityBuilder.Build(model);

               if (model.IsNew) //create
               {
                  obj.StartDateUtc = new DateTime(1753, 1, 1);
                  obj.StopDateUtc = new DateTime(1753, 1, 1);
                  mobjDasOutputStateManager.Create(obj);
               }
               else //update
               {
                  Digistat.FrameworkStd.Model.DAS3Plus.DasOutputState objDASOutput = mobjDasOutputStateManager.Get(model.LocationId, model.BedId, model.PatientId);
                  //check the key for the model that should be updated
                  if (objDASOutput != null && objDASOutput.PatientId == obj.PatientId && objDASOutput.LocationId == obj.LocationId && objDASOutput.BedId == obj.BedId && objDASOutput.IsSystem)
                  {
                     objDASOutput.SamplingSeconds = obj.SamplingSeconds;
                     objDASOutput.Type = obj.Type;
                     mobjDasOutputStateManager.Update(objDASOutput);
                  }
                  else
                  {
                     return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.CANNOT_BE_MODIFIED), success = false });
                  }
               }

               bolSuccess = true;
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });

         }
         catch (ConnectException cex)
         {
            return Json(new { errorMessage = cex.Message, success = false, show = true });
         }
         catch (ArgumentException aex)
         {
            return Json(new { errorMessage = aex.Message, success = false, show = true });
         }
         catch (Exception ex)
         {

            return Json(new { errorMessage = ex.Message, success = false, show = false });
         }
      }

      [HttpPost]
      public JsonResult DeleteOutputState(int locationId, int bedId, int patientId)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverEdit, CurrentUser))
            {
               mobjDasOutputStateManager.Delete(locationId, bedId, patientId);
               bolSuccess = true;
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            messageError = ex.Message;
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }

      }

      public IActionResult GetLocations()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
         {
            IQueryable<Location> objAll = mobjLocationsManager.GetLocations();
            List<LocationViewModel> model = objAll.Select(x => LocationViewModelBuilder.Build(x)).ToList();
            return View("_Locations", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public IActionResult GetBedsLocations()
      {
         List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedsManager.GetBedsWithFullData()).ToList();
         //int selLoc = 0;

         //if (!string.IsNullOrEmpty(selectedLocID))
         //{
         //   bool isInt = Int32.TryParse(selectedLocID, out selLoc);
         //   if (isInt && selLoc>0)
         //   {
         //      return Json(new { content = this.RenderViewAsync("_Beds", model.Where(a => a.IdLocation.Value == selLoc), true) }); 
         //   }

         //}
         return Json(new { content = this.RenderViewAsync("_Beds", model, true) });
      }

      #endregion


      #region Waveform snapshoot 2Unite Rules

      public ActionResult WaveformRules()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            ViewBag.SitePath = "Connect > Drivers > Waveforms";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult GetWaveformRules([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWaveformRulesView, CurrentUser))
         {
            var objAll = mobjWaveformManager.GetAllInclude().OrderBy(wfr => wfr.Priority);
            DataSourceResult data = null;
            try
            {
               data = objAll.ToDataSourceResult(request, model => model );
            }
            catch (Exception e)
            {
               mobjLogSvc.ErrorException(e, "Error on ReadDrivers");
               return Json(new { errorMessage = "Error on ReadDrivers", success = false });
            }
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult GetDriverRepositoryEventCatalogs(string driverId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDriverView, CurrentUser))
         {
            var model = mobjDriverRepositoryManager.GetEventsByDriverId(driverId);
            ViewBag.driverId = driverId;
            return View("_Events", EventViewModelBuilder.BuildList(model));
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetStandardParametersForWaveform(string driverId)
      {         
         List<StandardParameter> result;
         try
         {
            result = mobjStandardParametersManager.GetListByDriverIdOfWaveformType(driverId);
            result = result.OrderBy(sp => sp.Print).ToList();
            result.Insert(0,new StandardParameter
            {
               Id = 0,
               Print = mobjDicSvc.XLate("Any dynamic parameter"),
               IsVariableContentWaveform = true
            });
            return Json(result);
         }
         catch (Exception ex)
         {
            var errorMessage = ex.Message;
            return Json(new { errorMessage = errorMessage, success = false });
         }
      }

      public IActionResult GetWaveformRuleDetail(int Id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWaveformRulesView, CurrentUser))
         {
            var model = mobjWaveformManager.GetAllInclude().FirstOrDefault(w => w.Id == Id);
            if (model == null)
            {
               model = new WaveformSnapshotToUniteRule() {
                  IdParam = -1
               };               
            }
            return PartialView("_WaveformRuleDetail", WaveformSnapshotToUniteRuleViewModelBuilder.Build(model));
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveWaveformRuleDetail(
         int? Id, int? Priority, string IdDriver, int? IdLocation,
         string IdLinkEvent, int? IdParam, string Description)
      {
         var bolIsNotValid = false;
         var lstErrors = new List<string>();
         if (Id == null)
         {
            Id = 0;
         }
         if (Priority.GetValueOrDefault(0) == 0 && Id > 0)
         {
            bolIsNotValid = true;
            lstErrors.Add(mobjDicSvc.XLate("Priority is not valid"));
         }
         if (string.IsNullOrWhiteSpace(IdDriver))
         {
            bolIsNotValid = true;
            lstErrors.Add(mobjDicSvc.XLate("Driver is not valid"));
         }
         if (IdParam.GetValueOrDefault(-1) < 0)
         {
            bolIsNotValid = true;
            lstErrors.Add(mobjDicSvc.XLate("Parameter is not valid"));
         }
         if(!String.IsNullOrEmpty(Description) && Description.Length >= 255){
            bolIsNotValid = true;
            lstErrors.Add(mobjDicSvc.XLate("Description is too long"));
         }
         if (bolIsNotValid)
         {
            return Json(new { errorMessage = lstErrors, success = false });
         }

         var model = new WaveformSnapshotToUniteRule
         {
            Id = Id.Value,
            Priority = Priority.Value,
            IdDriver = IdDriver,
            IdLocation = IdLocation,
            IdLinkEvent = IdLinkEvent,
            IdParam = IdParam.Value,
            Description = Description,
         };

         string messageError = string.Empty;
         try
         {
            bool success;
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWaveformRulesEdit, CurrentUser))
            {
               if (mobjWaveformManager.Exists(wfr =>
                     wfr.IdDriver == model.IdDriver && wfr.IdLocation == model.IdLocation &&
                     wfr.IdLinkEvent == model.IdLinkEvent && wfr.IdParam == model.IdParam &&
                     wfr.Id != model.Id
                  ))
               {
                  throw new Exception(mobjDicSvc.XLate("A waveform rule with same parameters already exists."));
               }

               if (model.Priority == 0 && model.Id != 0)
               {
                  // todo controllare che non restituisce null => sempre vero perchè non è int nullable
                  throw new Exception(mobjDicSvc.XLate("The priority can not be zero"));
               }
               if (mobjWaveformManager.IsPriorityAlreadyAssigned(model.Priority, model.Id))
               {
                  throw new Exception(mobjDicSvc.XLate("The same priority is already assigned."));
               }

               WaveformSnapshotToUniteRule result = null;
               if (model.Id == 0) //create
               {
                  model.Priority = mobjWaveformManager.GetLowerPriority(model.Id);
                  result = mobjWaveformManager.Create(model);
                  success = true;
               }
               else //update
               {                  
                  result = mobjWaveformManager.Update(model);
                  success = true;
               }
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               success = false;
            }
            return Json(new { errorMessage = messageError, success = success });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false, show = false });
         }
      }

      [HttpPost]
      public JsonResult DeleteWaveformRule(int id)
      {
         string messageError = string.Empty;
         bool success = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionWaveformRulesEdit, CurrentUser))
            {
               var deleteResult = mobjWaveformManager.Delete(id);
               success = true;
            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               success = false;
            }
            return Json(new { errorMessage = messageError, success = success });
         }
         catch (Exception ex)
         {
            messageError = ex.Message;
            return Json(new { errorMessage = messageError, success = success });
         }
      }

      public JsonResult MoveWaveformRule(int waveformRuleId, int direction)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            int newAssignedPriority = -1;
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionBedsEdit, CurrentUser))
            {
               if (waveformRuleId != 0)
               {
                  newAssignedPriority = mobjWaveformManager.MoveWaveformRule(waveformRuleId, (MoveDirection) direction);
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess, indexPage = newAssignedPriority });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      #endregion
   }
}
