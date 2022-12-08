using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Configurator.Std.Exceptions;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using ConfiguratorWeb.App.Filters;

namespace ConfiguratorWeb.App.Controllers
{
    public class NetworkController : DigistatWebControllerBase
    {
        private readonly ISystemOptionsManager mobjSystemOptionsManager;
        private readonly INetworksManager mobjNetworksManager;
        private readonly IBedsManager mobjBedsManager;
        protected readonly IPermissionsService mobjPermSvc;
        private readonly ILocationManager mobjLocationManager;
        public NetworkController(IDigistatConfiguration config, IMessageCenterService msgcenter, ISystemOptionsManager systemOptionsManager, ISystemOptionsService sysOptSvc, ISynchronizationService syncSvc, IDnsCacherService dnsSvc, ILoggerService logSvc,
            INetworksManager networksManager, IBedsManager bedsManager, IPermissionsService permSvc,
            IDictionaryService dicSvc, ILocationManager locationManager) : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)
        {
            mobjSystemOptionsManager = systemOptionsManager;
            mobjNetworksManager = networksManager;
            mobjBedsManager = bedsManager;
            mobjPermSvc = permSvc;
            mobjLocationManager = locationManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NetworkConfiguration(int id)
        {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkView, CurrentUser))
            {
                ViewBag.SitePath = "General > System Configuration > Network Configuration";
                ViewBag.DetailId = id;
                return View();
            }
            else
            {
                return View("NotAuthorized");
            }
        }

        public JsonResult ReadNetworks([DataSourceRequest] DataSourceRequest request)
        {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkView, CurrentUser))
            {
                IEnumerable<Network> objAllNetworks = mobjNetworksManager.GetList();
                DataSourceResult data = objAllNetworks.ToDataSourceResult(request, model => NetworkViewModelBuilder.Build(model));
                return new JsonResult(data);
                //return new JsonResult { Data = data, MaxJsonLength = Int32.MaxValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

        }

        public IActionResult GetNetwork(int id)
        {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkView, CurrentUser))
            {
                NetworkViewModel model = new NetworkViewModel();
                if (id > 0)
                {
                    Network network = mobjNetworksManager.Get(id);
                    model = NetworkViewModelBuilder.Build(network);

                }
                return View("_NetworkTabStrip", model);
            }
            else
            {
                return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
        }
        public JsonResult ReadLinkedSystemOptions([DataSourceRequest] DataSourceRequest request, string hostname)
        {
            IQueryable<SystemOption> objAll = null;
            if (!string.IsNullOrEmpty(hostname))
            {
                objAll = mobjSystemOptionsManager.GetQueryable().Where(a => a.HostName == hostname);
                DataSourceResult data = objAll.ToDataSourceResult(request, model => SystemOptionViewModelBuilder.Build(model));
                return new JsonResult(data);
            }
            else
            {
                List<object> objNull = new List<object>();
                return new JsonResult(objNull);
            }

        }

        //[AcceptVerbs(HttpVerbs.Post)]
        [RequestFormSizeLimit(valueCountLimit: 20000)]
        [HttpPost]
        public JsonResult SaveNetworkDetail(NetworkViewModel model)
        {
            //update YES/NO props
            string messageError = string.Empty;
            Network objNetwork = null;
            bool bolSuccess = false;

            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkEdit, CurrentUser))
                {
                    //Server Validation
                    if (model.Type == Digistat.FrameworkStd.Enums.NetworkTypeEnum.BedSide && model.BedList != null && model.BedList.Count() > 1)
                    {
                        messageError = mobjDicSvc.XLate("A bedside network can be bound to One bed only");
                    }

                    //Old configurator use a default
                    if (model.BedID == null)
                        model.BedID = 0;

                    if (model.LocationID == null)
                        model.LocationID = 0;

                    if (string.IsNullOrEmpty(messageError))
                    {
                        if (model.Id == 0) //create
                        {
                            objNetwork = mobjNetworksManager.CreateNetwork(NetworkEntityBuilder.Build(model));
                        }
                        else //update
                        {
                            objNetwork = mobjNetworksManager.UpdateNetwork(NetworkEntityBuilder.Build(model));
                        }

                        if (objNetwork != null)
                        {
                            bolSuccess = true;
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
            catch (NetworkCreationException ex)
            {
                mobjLogSvc.ErrorException(ex, "Error saving Network");
                return Json(new { errorMessage = ex.Message, success = false });
            }
            catch (Exception ex)
            {
                mobjLogSvc.ErrorException(ex, "Error saving Network");
                return Json(new { errorMessage = ex.Message, success = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteNetwork(int id)
        {


            string messageError = string.Empty;
            bool bolSuccess = false;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionNetworkEdit, CurrentUser))
                {
                    if (id > 0)
                    {
                        mobjNetworksManager.Delete(id);
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
                mobjLogSvc.ErrorException(ex, "Error deleting Network");
                return Json(new { errorMessage = ex.Message, success = false });
            }
        }

        //[HttpPost("{networkId}", Name = "GetBedsLocations")]
        public IActionResult GetBedsLocations(int networkId, [FromBody] IEnumerable<BedViewModel> bedList)
        {



            List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjBedsManager.GetBedsWithFullData()).OrderBy(o => o.IdLocation).ThenBy(o => o.BedIndex).ToList();
            foreach (BedViewModel bed in bedList)
            {
                BedViewModel objBed = model.Where(a => a.BedId == bed.BedId && a.IdLocation == bed.IdLocation).FirstOrDefault();
                if (objBed != null)
                    objBed.Selected = true;
            }

            ViewBag.NetworkId = Convert.ToString(networkId);
            //return View("_BedsLocations", model);
            return Json(new { content = this.RenderViewAsync("_BedsLocations", model, true) });
        }

        public IActionResult GetLocations(string networkId, string selectedLocID)
        {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
            {
                ViewBag.NetworkId = networkId;
                ViewBag.SelectedLocID = selectedLocID;
                IQueryable<Location> objAll = mobjLocationManager.GetLocations();

                List<LocationViewModel> model = objAll.Select(x => LocationViewModelBuilder.Build(x)).ToList();

                return View("_Locations", model);
            }
            else
            {
                return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
        }


      public IActionResult GetDigistatDesktopModules()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionLocationsView, CurrentUser))
         {
            return View("_DesktopModules");
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
    }
}