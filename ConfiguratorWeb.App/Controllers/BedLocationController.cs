using System;
using System.Collections.Generic;
using System.Linq;
using Configurator.Std.BL;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;


namespace ConfiguratorWeb.App.Controllers
{
    public class BedLocationController : DigistatWebControllerBase
   {
      private readonly ISystemOptionsManager mobjSystemOptionsManager;
      private readonly INetworksManager mobjNetworksManager;
      private readonly IBedsManager mobjBedsManager;
      private readonly ILocationManager mobjLocManager;
      private readonly INetworkBedLinkManager mobjNetBedLnkManager;
      private readonly IPermissionsService mobjPermSvc;
      public BedLocationController(IDigistatConfiguration config, IMessageCenterService msgcenter,
         ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
         ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IPermissionsService permSvc,
         ISystemOptionsManager systemOptionsManager, INetworksManager networksManager, IBedsManager bedsManager
         , ILocationManager logMgr, INetworkBedLinkManager nblMgr) 
         : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjSystemOptionsManager = systemOptionsManager;
         mobjNetworksManager = networksManager;
         mobjBedsManager = bedsManager;
         mobjLocManager = logMgr;
         mobjNetBedLnkManager = nblMgr;
         mobjPermSvc = permSvc;
      }

      public IActionResult Index()
      {
         return View();
      }


      public JsonResult GetLocations([DataSourceRequest] DataSourceRequest request)
      {
         string messageError = string.Empty;
         try
         {
            IEnumerable<Location> objLocList =  mobjLocManager.GetAllWithBedCounts();
            DataSourceResult data = objLocList.ToDataSourceResult(request, model =>LocationViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }


      public JsonResult GetLocationList()
      {
         string messageError = string.Empty;
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionUserView, CurrentUser))
         {
            try
            {
               IEnumerable<Location> objLocList = mobjLocManager.GetLocations();
               return new JsonResult(objLocList);
            }
            catch (Exception ex)
            {
               return Json(new { errorMessage = ex.Message, success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }




      public JsonResult GetBedsForLocation([DataSourceRequest] DataSourceRequest request,int locationID, int? networkID)
      {
         string messageError = string.Empty;
         try
         {
            ICollection<NetworkBedLink> objLinks = null;
            if(networkID.HasValue)
            {
               Network objNetwork = mobjNetworksManager.GetWithBeds(networkID.Value);
               objLinks = objNetwork.NetworkBedLinks;
            }

            IEnumerable<Bed> objBedList = mobjBedsManager.Find(p => p.IdLocation == locationID);
            //IEnumerable<BedViewModel> objBedViewList = BedViewModelBuilder.BuildList(objBedList);
            List<BedViewModel> objBedViewList = BedViewModelBuilder.BuildList(objBedList).ToList();
            if (objLinks!=null)
            {
               foreach (NetworkBedLink objNetBedLink in objLinks)
               {

                  int intFound = objBedViewList.Where(p => p.BedId == objNetBedLink.IdBed).Count();
                  if(intFound >0)
                  {
                     objBedViewList.Where(p => p.BedId == objNetBedLink.IdBed).First().Selected = true;
                  }
               }
            }
            DataSourceResult data = objBedViewList.ToDataSourceResult(request, model=>model);
            return new JsonResult(data);
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

   }
}