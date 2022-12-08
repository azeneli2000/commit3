using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

namespace ConfiguratorWeb.App.Controllers
{
    [DigConfigFilterAttribute]
    [DigistatAuthFilterAttribute]
    public class TemplateController : DigistatWebControllerBase
    {

        private readonly IMessageCenterManager mobjMsgCtrMgr;
        private readonly ITelligenceServerManager mobjTelMgrServer;
        private readonly ITelligenceSystemManager mobjTelMgrSystem;
        private readonly ITelligenceDeviceManager mobjTelMgrDevice;
        private readonly ITelligenceConfigClientManager mobjTelCfgClient;
        private readonly INetworksManager mobjNetworksManager;
        private readonly IBedsManager mobjBedManager;
        private readonly INetworkBedLinkManager mobjNetBedLinkMgr;

        public TemplateController(IDigistatConfiguration config, IMessageCenterService msgcenter,
          ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
          ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IMessageCenterManager msgCtrMgr,
          ITelligenceServerManager telMgrServer, ITelligenceSystemManager telMgrSystem, ITelligenceDeviceManager telMgrDevice,
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
        }



        #region Template

        private string siteRoot = "Template > ";

        public IActionResult ListWithGrid()
        {
            ViewBag.SitePath = "Template";
            return View();
        }
        public IActionResult ListWithGridOldMenu()
        {
            ViewBag.SitePath = siteRoot + "ListWithGrid Old Menu";
            return View();
        }

        public JsonResult GetAllSystems()
        {
            IEnumerable<TelligenceSystem> objAllSystems = mobjTelMgrSystem.GetAll();
            return new JsonResult(TelligenceSystemViewModelBuilder.BuildList(objAllSystems));

        }
        public JsonResult ReadTelligenceDevicesWithBeds([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<TelligenceDeviceViewModel> objAllServers = TelligenceDeviceViewModelBuilder.BuildList(mobjTelMgrDevice.GetAllWithBeds());
            DataSourceResult data = objAllServers.ToDataSourceResult(request);
            return new JsonResult(data);
        }

        public JsonResult ReadTelligenceDevices([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<TelligenceDeviceViewModel> objAllServers = TelligenceDeviceViewModelBuilder.BuildList(mobjTelMgrDevice.GetAll());
            DataSourceResult data = objAllServers.ToDataSourceResult(request);
            return new JsonResult(data);
        }

        public JsonResult GetAll()
        {
            IEnumerable<TelligenceDevice> objAllServers = mobjTelMgrDevice.GetAll();
            return new JsonResult(objAllServers);

        }

        public ActionResult GetDevice(int id)
        {
            TelligenceDeviceViewModel model = new TelligenceDeviceViewModel();
            if (id != 0)
            {
                TelligenceDevice tserver = mobjTelMgrDevice.Get(id);
                model = TelligenceDeviceViewModelBuilder.Build(tserver);
            }

            return PartialView("_ItemDetail", model);

        }




        public JsonResult DeleteDevice(int id)
        {
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
                if (model.ID == 0) //create
                {
                    objTLServer = mobjTelMgrDevice.CreateDevice(TelligenceDeviceModelBuilder.Build(model), null, null, model.CreateNetwork, model.CreatePortServer);
                }
                else //update
                {
                    objTLServer = mobjTelMgrDevice.UpdateDevice(TelligenceDeviceModelBuilder.Build(model), null, null, model.CreateNetwork, model.CreatePortServer);
                }
                if (objTLServer != null)
                {
                    bolSuccess = true;
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


        public JsonResult ImportAllTelligenceDevices(int serverID)
        {
            try
            {
                List<Tuple<int, string>> objMsgList = mobjTelCfgClient.ImportAllTLDevices(serverID);
                return new JsonResult(objMsgList);
            }
            catch (Exception ex)
            {
                JsonResult objRet = new JsonResult(ex);
                objRet.StatusCode = 500;
                return objRet;

            }
        }

        public JsonResult GetTelligenceSystems()
        {
            try
            {
                //TelligenceConfigClient objClient = new TelligenceConfigClient(mobjSysOptSvc, mobjConfiguration, mobjLogSvc);
                Dictionary<int, string> objTLSystems = mobjTelCfgClient.GetAvailableTLSystems();
                var objSelectList = objTLSystems.Select(x => new SelectListItem
                {
                    Value = x.Key.ToString(),
                    Text = x.Value
                }).ToList();
                return new JsonResult(objSelectList);
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
            List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedManager.GetAll()).ToList();
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
            return View("_BedSelection", NetworkViewModelBuilder.Build(objNetwork));
        }

        public JsonResult GetAllDeviceTypes()
        {
            Dictionary<int, string> objItems = mobjTelMgrDevice.GetTLDeviceTypes();
            List<SelectListItem> objToRender = objItems.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
            return new JsonResult(objToRender);

        }



    }
}
