using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Digistat.FrameworkWebExtensions.Attributes;
using Digistat.FrameworkWebExtensions.Controllers;
using Digistat.FrameworkStd.Interfaces;
using Configurator.Std.BL.Hubs;
using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.Builders;
using ConfiguratorWeb.App.Models;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using Configurator.Std.BL.Vitals;
using Microsoft.AspNetCore.Mvc.Rendering;
using ConfiguratorWeb.App.ViewModelBuilders;
using ConfiguratorWeb.App.Models.Telligence;
using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.EntityBuilders;
using Microsoft.AspNetCore.Http;
using Configurator.Std.Helpers;
using Configurator.Std.BL.DasDrivers;
using System.Net.Http;
using System.Text;

namespace ConfiguratorWeb.App.Controllers
{
   [DigConfigFilterAttribute]
   [DigistatAuthFilterAttribute]
   public class TelligenceController : DigistatWebControllerBase
   {

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly ITelligenceServerManager mobjTelMgrServer;
      private readonly ITelligenceSystemManager mobjTelMgrSystem;
      private readonly ITelligenceDeviceManager mobjTelMgrDevice;
      private readonly ITelligenceConfigClientManager mobjTelCfgClient;
      private readonly INetworksManager mobjNetworksManager;
      private readonly IBedsManager mobjBedManager;
      private readonly INetworkBedLinkManager mobjNetBedLinkMgr;
      private readonly IPermissionsService mobjPermSvc;

      public TelligenceController(IDigistatConfiguration config, IMessageCenterService msgcenter,
        ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
        ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IMessageCenterManager msgCtrMgr,
        ITelligenceServerManager telMgrServer, ITelligenceSystemManager telMgrSystem, ITelligenceDeviceManager telMgrDevice, IPermissionsService permSvc,
        ITelligenceConfigClientManager telCfgClient, INetworksManager netMgr, IBedsManager bedMgr, INetworkBedLinkManager netBedLinkMgr)
     : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjTelMgrServer = telMgrServer;
         mobjTelMgrSystem = telMgrSystem;
         mobjTelMgrDevice = telMgrDevice;
         mobjTelCfgClient = telCfgClient;
         mobjNetworksManager = netMgr;
         mobjBedManager = bedMgr;
         mobjNetBedLinkMgr = netBedLinkMgr;
         mobjPermSvc = permSvc;
      }

      #region servers

      public IActionResult Servers()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            ViewBag.SitePath = "Integrations > Ascom Telligence > Servers";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }

      }

      public JsonResult ReadTelligenceServers([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceServerViewModel> objAllServers = TelligenceServerViewModelBuilder.BuildList(mobjTelMgrServer.GetAll());
            DataSourceResult data = objAllServers.ToDataSourceResult(request);
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetAllServers()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceServer> objAllServers = mobjTelMgrServer.GetAll();
            return new JsonResult(objAllServers);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      public ActionResult GetServer(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            TelligenceServerViewModel model = new TelligenceServerViewModel();
            if (id != 0)
            {
               TelligenceServer tserver = mobjTelMgrServer.Get(id);
               model = TelligenceServerViewModelBuilder.Build(tserver);
            }

            return PartialView("_ServerDetail", model);
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }

      }


      public JsonResult DeleteServer(int id)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               string strRet = mobjTelMgrServer.Delete(id);
               if (string.IsNullOrEmpty(strRet))
               {
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = strRet, success = false });
               }
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

      [HttpPost]
      public JsonResult SaveServerDetails(TelligenceServerViewModel model)
      {
         //update YES/NO props
         string messageError = string.Empty;
         TelligenceServer objTLServer = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               if (model.ID == 0) //create
               {
                  objTLServer = mobjTelMgrServer.Create(TelligenceServerModelBuilder.Build(model));
               }
               else //update
               {
                  objTLServer = mobjTelMgrServer.Update(TelligenceServerModelBuilder.Build(model));
               }

               if (objTLServer != null)
               {
                  bolSuccess = true;
               }

            }
            else
            {
               messageError = mobjDicSvc.XLate(CommonStrings.GENERAL_APPLICATION_FILTER);
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }

      #endregion

      #region systems
      public IActionResult Systems()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            ViewBag.SitePath = "Integrations > Ascom Telligence > Systems";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }


      public JsonResult GetAllSystems()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceSystem> objAllSystems = mobjTelMgrSystem.GetAll();
            return new JsonResult(TelligenceSystemViewModelBuilder.BuildList(objAllSystems));
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult ReadTelligenceSystems([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceSystemViewModel> objAllSystems = TelligenceSystemViewModelBuilder.BuildList(mobjTelMgrSystem.GetAllSystems());
            DataSourceResult data = objAllSystems.ToDataSourceResult(request);
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }



      public ActionResult GetSystem(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            TelligenceSystemViewModel model = new TelligenceSystemViewModel();
            if (id != 0)
            {
               TelligenceSystem tserver = mobjTelMgrSystem.Get(id);
               model = TelligenceSystemViewModelBuilder.Build(tserver);
            }

            return PartialView("_SystemDetail", model);
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }


      [HttpPost]
      public JsonResult SaveSystemDetail(TelligenceSystemViewModel model)
      {
         //update YES/NO props
         string messageError = string.Empty;
         TelligenceSystem objTLServer = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               if (model.ID == 0) //create
               {
                  objTLServer = mobjTelMgrSystem.Create(TelligenceSystemModelBuilder.Build(model));
               }
               else //update
               {
                  objTLServer = mobjTelMgrSystem.Update(TelligenceSystemModelBuilder.Build(model));
               }



               if (objTLServer != null)
               {
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
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }
      public JsonResult DeleteSystem(int id)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               string strRet = mobjTelMgrSystem.Delete(id);
               if (string.IsNullOrEmpty(strRet))
               {
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = strRet, success = false });
               }
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
      #endregion

      #region devices

      public IActionResult Import()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            ViewBag.SitePath = "Integrations > Ascom Telligence > Import";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult Devices(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            ViewBag.SitePath = "Integrations > Ascom Telligence > Devices";
            ViewBag.NetworkId = id;
            return View("Devices");
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult PortServer(int id)
      {
         TelligenceDevice tserver = mobjTelMgrDevice.GetAllDevices().FirstOrDefault(w => w.tl_psv_ID == id);
         if (tserver != null && tserver.tl_NetworkID != null)
         {
            return Devices(tserver.tl_NetworkID.Value);
         }
         else
         {
            ErrorViewModel objError = new ErrorViewModel();
            objError.Message = $"No Telligence devices found related to Port Server with ID {id}";
            return View("Error", objError);
         }
      }

      public JsonResult ReadTelligenceDevicesWithBeds([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceDeviceViewModel> objAllServers = TelligenceDeviceViewModelBuilder.BuildList(mobjTelMgrDevice.GetAllWithBeds());
            DataSourceResult data = objAllServers.ToDataSourceResult(request);
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult ReadTelligenceDevices([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceDeviceViewModel> objAllServers = TelligenceDeviceViewModelBuilder.BuildList(mobjTelMgrDevice.GetAll());
            DataSourceResult data = objAllServers.ToDataSourceResult(request);
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetAll()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            IEnumerable<TelligenceDevice> objAllServers = mobjTelMgrDevice.GetAll();
            return new JsonResult(objAllServers);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public ActionResult GetDevice(int id, [FromServices] IPortServerManager portServerManager)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
         {
            var alsPortGroups = new short[] { 0, 1, 2, 3 };
            TelligenceDeviceViewModel model = new TelligenceDeviceViewModel();

            if (id != 0)
            {
               TelligenceDevice tserver = mobjTelMgrDevice.Get(id);
               model = TelligenceDeviceViewModelBuilder.Build(tserver);

               var bolIsMDIDevice = TelligenceHelper.IsMDIDevice(tserver);

               if (model.NetworkID != null)
               {
                  List<BedViewModel> objBedList = BedViewModelBuilder.BuildList(mobjBedManager.GetBedsWithFullData()).ToList();
                  Network objNetwork = mobjNetworksManager.GetWithBeds(model.NetworkID.Value);

                  if (objNetwork != null)
                  {
                     model.BedList = objNetwork.NetworkBedLinks == null
                         ? new List<BedViewModel>()
                         : objNetwork.NetworkBedLinks.Join(objBedList, nbl => nbl.IdBed, b => b.BedId, (nbl, b) => b).ToList();
                  }

                  if (bolIsMDIDevice)
                  {
                     var objPortServer = portServerManager.GetWithBeds(tserver.tl_psv_ID);
                     if (objPortServer != null)
                     {
                        if (objPortServer.IDBED > 0)
                        {
                           var objBed = objBedList.FirstOrDefault(b => b.BedId == objPortServer.IDBED);
                           model.PortServerBedList = BuildPortServerBedLinks(alsPortGroups, objBed);
                        }
                        else if (objPortServer.IDBED == -1 && objPortServer.PortServerBedLinks != null)
                        {
                           var portServerBedList = new List<PortServerBedLinkViewModel>();
                           foreach (var portGroup in alsPortGroups)
                           {
                              var intBedId = objPortServer.PortServerBedLinks.FirstOrDefault(psbl => psbl.PortGroup == portGroup)?.BedId;
                              var objPortServerBedLink = new PortServerBedLinkViewModel()
                              {
                                 Bed = objBedList.FirstOrDefault(b => b.BedId == intBedId),
                                 PortGroup = portGroup
                              };
                              portServerBedList.Add(objPortServerBedLink);

                           }
                           model.PortServerBedList = portServerBedList;
                        }
                     }
                  }

               }
               ViewBag.IsMDIDevice = bolIsMDIDevice ? CommonStrings.NUMERIC_TRUE : CommonStrings.NUMERIC_FALSE;
            }
            else
            {
               ViewBag.IsMDIDevice = CommonStrings.NUMERIC_FALSE;
            }

            if (!model.PortServerBedList.Any())
            {
               model.PortServerBedList = BuildPortServerBedLinks(alsPortGroups, null);
            }

            return PartialView("_DeviceDetail", model);
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      public JsonResult DeleteDevice(int id)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               string strRet = mobjTelMgrDevice.Delete(id);
               if (string.IsNullOrEmpty(strRet))
               {
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = strRet, success = false });

               }
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
         throw new NotImplementedException();

      }

      [HttpPost]
      public JsonResult SaveDeviceDetail(TelligenceDeviceViewModel model)
      {
         //update YES/NO props
         string messageError = string.Empty;
         TelligenceDevice objTLServer = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               var objPortServerBeds = (model.PortServerBedList ?? Enumerable.Empty<PortServerBedLinkViewModel>())
                   .Where(b => b.Bed?.BedId > 0)
                   .GroupBy(b => b.PortGroup)
                   .ToDictionary(g => g.Key, g => g.First().Bed.BedId);

               if (model.ID == 0) //create
               {
                  objTLServer = mobjTelMgrDevice.CreateDevice(TelligenceDeviceModelBuilder.Build(model),
                      BedEntityModelBuilder.BuildList(model.BedList).ToList(),
                      objPortServerBeds,
                      model.CreateNetwork,
                      model.CreatePortServer);
               }
               else //update
               {
                  objTLServer = mobjTelMgrDevice.UpdateDevice(TelligenceDeviceModelBuilder.Build(model),
                      BedEntityModelBuilder.BuildList(model.BedList).ToList(),
                      objPortServerBeds,
                      model.CreateNetwork,
                      model.CreatePortServer);
               }
               if (objTLServer != null)
               {
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
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }

      #endregion


      #region TelligenceConfigHandler Connection


      private string SerializeTelligenceImportResult(List<Tuple<int,string>> objMessages)
      {
         StringBuilder objBuilder = new StringBuilder();
         if (objMessages != null)
         {
            foreach(Tuple<int,string> objElement in objMessages)
            {
               objBuilder.AppendLine($"[{objElement.Item1}] {objElement.Item2};");
            }
         }
         return objBuilder.ToString();
      }

      public JsonResult ImportAllTelligenceDevices(int serverID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
         {
            try
            {
               List<Tuple<int, string>> objMsgList = mobjTelCfgClient.ImportAllTLDevices(serverID);

               mobjLogSvc.Write(100, $"Telligence Devices import results: {SerializeTelligenceImportResult(objMsgList)}", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,CurrentUser.Abbrev,0);
               return new JsonResult(objMsgList);
            }
            catch (Exception ex)
            {
               JsonResult objRet = null;
               if (ex.InnerException != null)
               {
                  string strHandledMsg = string.Empty;
                  if (ex.InnerException is System.UriFormatException)
                  {
                     strHandledMsg = mobjDicSvc.XLate("URL not correctly formatted. Please check that Telligence Server URL is a correct URL");

                  }
                  else
                  {
                     if (ex.InnerException is CookComputing.XmlRpc.XmlRpcException && ex.InnerException.Message.Trim().ToUpper() == "NOT FOUND")
                     {
                        strHandledMsg = mobjDicSvc.XLate("Telligence config handler not found. Please check Telligence service handler service is installed and server url is correctly configured.");
                     }
                     else
                     {
                        if (ex.InnerException.InnerException != null)
                        {
                           if(ex.InnerException.InnerException is HttpRequestException)
                           {
                              strHandledMsg = mobjDicSvc.XLate("Unable to connecto to Telligence config handler. Please check Telligence service handler service is installed and server url is correctly configured.");
                           }
                        }
                     }
                  }

                  objRet = new JsonResult(strHandledMsg);
               }
               else
               {
                  objRet = new JsonResult(ex);
               }
               objRet.StatusCode = 500;
               return objRet;

            }

         }
         else
         {
            JsonResult objresp = null;
            objresp = new JsonResult(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            objresp.StatusCode = 403;
            return objresp;
         }
      }



      public JsonResult GetTelligenceSystems()
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceView, CurrentUser))
            {
               Dictionary<int, string> objTLSystems = mobjTelCfgClient.GetAvailableTLSystems();
               var objSelectList = objTLSystems.Select(x => new SelectListItem
               {
                  Value = x.Key.ToString(),
                  Text = x.Value
               }).ToList();
               return new JsonResult(objSelectList);
            }
            else
            {
               JsonResult objresp = null;
               objresp = new JsonResult(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
               objresp.StatusCode = 403;
               return objresp;
            }
         }
         catch (Exception ex)
         {
            JsonResult objRet = new JsonResult(ex);
            objRet.StatusCode = 500;
            return objRet;

         }

      }


      public JsonResult GetTelligenceDevices([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            //TelligenceConfigClient objClient = new TelligenceConfigClient(mobjSysOptSvc, mobjConfiguration, mobjLogSvc);
            var objTLDevices = mobjTelCfgClient.GetAvailableTLDevices();
            DataSourceResult data = objTLDevices.ToDataSourceResult(request);
            return new JsonResult(data);
         }
         catch (Exception ex)
         {
            JsonResult objRet = new JsonResult(ex);
            objRet.StatusCode = 500;
            return objRet;
         }
      }

      #endregion


      public ActionResult GetTellXMLRpcSettings()
      {
         TelligenceConfigHandlerConfiguration objConfig = mobjTelCfgClient.GetCurrentSetting();
         return PartialView("_TLXmlRpcSettings", TelligenceCfgSettingsViewModelBuilder.Build(objConfig));
      }


      public JsonResult SaveXMLRPCSettings(TelligenceCfgSettingsViewModel model)
      {
         TelligenceConfigHandlerConfiguration objConfig = TelligenceCfgSettingsBuilder.Build(model);
         mobjTelCfgClient.SetSettings(objConfig);
         return Json(new { success = true });
      }


      public ActionResult GetTelligenceSystemsFromRemote()
      {
         return PartialView("_TLxmlRpcSystems");

      }

      public ActionResult GetTelligenceDevicesFromRemote()
      {
         return PartialView("_TLXmlRpcDevices");
      }

      [HttpGet]
      public IActionResult DisplayBedLocation(int networkId, int TLdeviceID)
      {
         List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedManager.GetBedsWithFullData()).ToList();
         Network objNetwork = mobjNetworksManager.GetWithBeds(networkId);
         if (objNetwork != null)
         {
            ICollection<NetworkBedLink> objBeds = objNetwork.NetworkBedLinks;
            if (objNetwork.NetworkBedLinks != null)
            {
               foreach (NetworkBedLink objNetBedLink in objNetwork.NetworkBedLinks)
               {
                  BedViewModel objBed = model.Where(a => a.BedId == objNetBedLink.IdBed).FirstOrDefault();
                  if (objBed != null)
                     objBed.Selected = true;
               }
            }
         }
         ViewBag.NetworkId = Convert.ToString(networkId);
         ViewBag.TLDeviceID = Convert.ToString(TLdeviceID);
         return View("_BedSelection", model);
      }

      public JsonResult GetAllDeviceTypes()
      {
         Dictionary<int, string> objItems = mobjTelMgrDevice.GetTLDeviceTypes();
         List<SelectListItem> objToRender = objItems.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
         return new JsonResult(objToRender);

      }

      [HttpPost]
      public JsonResult UpdateBeds([FromBody] IEnumerable<BedViewModel> bedList, int networkID, int tlDeviceID)
      {

         try
         {
            mobjTelMgrDevice.UpdateNetworkBedLink(BedEntityModelBuilder.BuildList(bedList).ToList(), networkID);

            return Json(new { errorMessage = string.Empty, success = true });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.InnerException?.Message ?? ex.Message, success = false });
         }
      }

      private ICollection<PortServerBedLinkViewModel> BuildPortServerBedLinks(short[] portGroups, BedViewModel bed)
      {
         return portGroups
             .Select(pg => new PortServerBedLinkViewModel
             {
                Bed = bed,
                PortGroup = pg
             }).ToList();
      }
   }
}
