using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

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

namespace ConfiguratorWeb.App.Controllers
{
   public class DAS3Controller : DigistatWebControllerBase
   {

      protected readonly IDigistatConfiguration mobjDigistatConfig;
      protected readonly ISystemOptionsService mobjSystemOptionsService;
      protected readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      protected readonly IStandardParametersManager mobjParMgr;
      protected readonly IStandardUnitsManager mobjUnitMgr;
      protected readonly IPermissionsService mobjPermSvc;

      public DAS3Controller(IDigistatConfiguration config, IMessageCenterService msgcenter, ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc,
         IDigistatEnvironmentService digEnvSvc, IStandardParametersManager parMgr,IStandardUnitsManager unitMgr
         ,IPermissionsService permSvc)
      : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjParMgr = parMgr;
         mobjUnitMgr = unitMgr;
         mobjPermSvc = permSvc;
      }


     
      public JsonResult GetStandardParameters([DataSourceRequest] DataSourceRequest request,int? devTypeID,string driverID)
      {
         try
         {
            DataSourceResult data = null;
            if (devTypeID.HasValue)
            {
               List<StandardParameter> objAllparameters = mobjParMgr.GetListByDeviceId(devTypeID.Value);
               data = objAllparameters.ToDataSourceResult(request, model => StandardParameterViewModelBuilder.Build(model));
            }
            else
            {
               if (!string.IsNullOrEmpty(driverID))
               {
                  List<StandardParameter> objAllparameters = mobjParMgr.GetListByDriverID(driverID);
                  data = objAllparameters.ToDataSourceResult(request, model => StandardParameterViewModelBuilder.Build(model));
               }
               else
               {
                  IQueryable<StandardParameter> objAllparameters = mobjParMgr.GetQueryable();
                  data = objAllparameters.ToDataSourceResult(request, model => StandardParameterViewModelBuilder.Build(model));
               }
               
            }
            return new JsonResult(data);
         }
         catch(Exception e)
         {
            return Json(new { errorMessage = e.Message, success = false });
         }
         
      }



      public ActionResult GetStandardParameterListDialog(string selectFunction,string dialogID
         )
      {
         ViewBag.selectFunction = selectFunction;
         ViewBag.dialogID = dialogID;
         return View("_StandardParameterList");
      }

      /// <summary>
      /// Returns a selector for StandardParameter
      /// </summary>
      /// <param name="parID">parID to be automatically selected when dialog loads. 0 = no paramter will be selected</param>
      /// <param name="selectFunction">Caller JS select function to be invoked on parameter selection</param>
      /// <param name="dialogID"></param>
      /// <param name="driverID"></param>
      /// <returns></returns>
      public ActionResult GetStandardParameterSelectorDialog(int parID,string selectFunction,string dialogID,string driverID="")
      {
         ViewBag.selectFunction = selectFunction;
         ViewBag.selectedID = parID;
         ViewBag.dialogID = dialogID;
         ViewBag.driverID = driverID;
         return View("_StandardParameterSelector");
      }

      public ActionResult GetStandardUnitSelectorDialog(int uomID,string selectFunction,string dialogID,string parRelatedUoMs)
      {

         ViewBag.selectFunction = selectFunction;
         ViewBag.selectedID = uomID;
         ViewBag.dialogID = dialogID;
         ViewBag.parRelatedUoMs = parRelatedUoMs;
         return View("_StandardUnitSelector");
      }

      public JsonResult GetStandardUnits([DataSourceRequest] DataSourceRequest request,string parRelatedUOMs)
      {

         DataSourceResult data = null;
         if (!string.IsNullOrEmpty(parRelatedUOMs))
         {
            string[] astrUoms = parRelatedUOMs.Split(';', StringSplitOptions.RemoveEmptyEntries);
            List<int> objUoms = new List<int>();
            if (astrUoms.Length > 0)
            {
               for(var i = 0; i < astrUoms.Length; i++)
               {
                  if (astrUoms[i].Trim().Length > 0)
                  {
                     int tmpVal = 0;
                     if (int.TryParse(astrUoms[i], out tmpVal))
                     {
                        objUoms.Add(tmpVal);
                     }
                  }
               }
               
            }
            List<StandardUnit> objData =  mobjUnitMgr.GetMulti(objUoms);
            data = objData.ToDataSourceResult(request, model => StandarUnitViewModelBuilder.Build(model));
         }
         else
         {
            IQueryable<StandardUnit> objAllparameters = mobjUnitMgr.GetQueryable();
            data = objAllparameters.ToDataSourceResult(request, model => StandarUnitViewModelBuilder.Build(model));
         }
         return new JsonResult(data);
      }

      public ActionResult GetStandardUnitListDialog(string selectFunction)
      {
         ViewBag.selectFunction = selectFunction;
         return View("_standardUnitList");
      }

      public JsonResult GetStandardParameterByDeviceTypeID(int devID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, CurrentUser))
         {
            List<StandardParameter> mobjList = mobjParMgr.GetListByDeviceId(devID);
            return new JsonResult(mobjList);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public JsonResult GetStandardParameterByDriverID(string drvID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStandardParametersView, CurrentUser))
         {
            List<StandardParameter> mobjList = mobjParMgr.GetListByDriverID(drvID);
            return new JsonResult(mobjList);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


   }
}