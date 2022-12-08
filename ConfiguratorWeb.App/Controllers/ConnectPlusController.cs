using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Digistat.FrameworkWebExtensions.Controllers;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;

using Configurator.Std.BL;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.ViewModelBuilders;
using Configurator.Std.BL.DasDrivers;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Microsoft.AspNetCore.Mvc.Rendering;
using Configurator.Std.BL.DAS3Plus;

namespace ConfiguratorWeb.App.Controllers
{
    public class ConnectPlusController : DigistatWebControllerBase
    {
        protected readonly IDigistatConfiguration mobjDigistatConfig;
        protected readonly ISystemOptionsService mobjSystemOptionsService;
        protected readonly IDigistatEnvironmentService mobjDigEnvironmentService;
        protected readonly IPortServerManager mobjPortSrvMgr;
        protected readonly IDASBrokerManager mobjBrokerMgr;
        private readonly IBedsManager mobjBedsManager;
        private ILoggerService mobjLog;


        public ConnectPlusController(IDigistatConfiguration config, IMessageCenterService msgcenter,
        ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc
           , IPortServerManager psMgr, IDASBrokerManager brokerMgr, IBedsManager bedsManager)
        : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
        {
            mobjDigistatConfig = config;
            mobjSystemOptionsService = sysOptSvc;
            mobjLog = logsvc;
            mobjDigEnvironmentService = digEnvSvc;
            mobjPortSrvMgr = psMgr;
            mobjBrokerMgr = brokerMgr;
            mobjBedsManager = bedsManager;
        }

        // GET: Connect
        //public ActionResult PortServerList()
        //{
        //   ViewBag.SitePath = "Connect > Port Servers > Port Servers";
        //   return View("PortServers");
        //}
        public ActionResult PortServerList(int id)
        {
            ViewBag.SitePath = "Connect > Port Servers > Port Servers";
            ViewBag.DetailId = id;
            return View("PortServers");
        }

        public JsonResult ReadPortServers([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<PortServer> objAllSystems = mobjPortSrvMgr.GetAll();
            List<Bed> objAllBeds = mobjBedsManager.GetBedsWithFullData().ToList();
            DataSourceResult data = objAllSystems.ToDataSourceResult(request, model => PortServerViewModelBuilder.Build(model, objAllBeds));
            return new JsonResult(data);
        }


        public JsonResult GetPortServersShort()
        {
            List<PortServer> objPortServers = mobjPortSrvMgr.GetTelligencePortServer().ToList();
            objPortServers.Insert(0, new PortServer() { ID = 0, Address = string.Empty });
            return new JsonResult(PortServerViewModelBuilder.BuildList(objPortServers));
        }


        public JsonResult GetDASBrokers()
        {
            return new JsonResult(GetDASBrokersList());
        }

        public List<SelectListItem> GetDASBrokersList(bool insertEmptyRow = true)
        {
            List<DASBroker> objDASBrokers = mobjBrokerMgr.GetList();
            if (insertEmptyRow)
            {
                objDASBrokers.Insert(0, new DASBroker() { Hostname = string.Empty });
            }
            List<SelectListItem> objToRender = objDASBrokers.GroupBy(g => g.Hostname).Select(c => new SelectListItem { Text = c.Key, Value = c.Key }).ToList();
            return objToRender;
        }

        public JsonResult ReadDASBrokers([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult data = GetDASBrokersList(false).ToDataSourceResult(request);
            return new JsonResult(data);
        }

        public IActionResult Get(int ID)
        {
            PortServer objPs = new PortServer();
            if (ID != 0)
            {
                objPs = mobjPortSrvMgr.Get(ID);
            }

            return View("_PortServerDetails", PortServerViewModelBuilder.Build(objPs));
        }

        public JsonResult GetAllPortServerTypes()
        {
            Dictionary<int, string> objItems = mobjPortSrvMgr.GetPortServerTypes();
            List<SelectListItem> objToRender = objItems.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
            return new JsonResult(objToRender);

        }


        public JsonResult GetPortServerStatus([DataSourceRequest] DataSourceRequest request, int psID, [FromServices] IBedsManager bedsManager)
        {
            try
            {
                var objPSstatus = mobjPortSrvMgr.GetPortServerStatus(psID);
                var objPortServer = mobjPortSrvMgr.GetWithBeds(psID);
                var objBedList = BedViewModelBuilder.BuildList(bedsManager.GetBedsWithFullData()).ToList();
                var objPortServerBedsDict = objPortServer != null
                    ? objPortServer.PortServerBedLinks.ToDictionary(psbl => psbl.PortId, psbl => objBedList.FirstOrDefault(b => b.BedId == psbl?.BedId))
                    : new Dictionary<short, BedViewModel>();

                DataSourceResult data = objPSstatus.ToDataSourceResult(request, model => PortServerStatusViewModelBuilder.Build(model, objPortServerBedsDict));

                return new JsonResult(data);
            }
            catch
            {
                return Json(new { errorMessage = mobjDicSvc.XLate("An internal error occurred"), success = false });
            }
        }

        public IActionResult GetPortServerStatusView(int ID)
        {
            ViewBag.PSSID = ID;
            return View("_PortServerStatus");
        }


        public JsonResult DeletePortServer(int ID)
        {
            try
            {
                mobjPortSrvMgr.Delete(ID);
                return Json(new { errorMessage = string.Empty, success = true });
            }
            catch (Exception ex)
            {
                return Json(new { errorMessage = ex.Message, success = false });
            }
        }

        public JsonResult SavePortServer(PortServerViewModel model)
        {
            string messageError = string.Empty;
            PortServer objPS = null;
            bool bolSuccess = false;
            model.UpdateDate = DateTime.Now;
            try
            {
                if (model.ID == 0) //create
                {
                    objPS = mobjPortSrvMgr.Create(PortServerEntityModelBuilder.Build(model));
                }
                else //update
                {
                    objPS = mobjPortSrvMgr.Update(PortServerEntityModelBuilder.Build(model));
                }
                if (objPS != null)
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

        public IActionResult GetBedsAndLocations(int bedId)
        {

            List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedsManager.GetBedsWithFullData()).OrderBy(o => o.IdLocation).ThenBy(o => o.BedIndex).ToList();
            BedViewModel objBed = model.Where(a => a.BedId == bedId).FirstOrDefault();
            if (objBed != null)
                objBed.Selected = true;

            ViewBag.BedId = Convert.ToString(bedId);
            //return View("_BedsLocations", model);
            return Json(new { content = this.RenderViewAsync("_BedLocation", model, true) });
        }

        public JsonResult GetBedsList()
        {

            List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedsManager.GetBedsWithFullData()).ToList();

            return Json(model);
        }
    }
}
