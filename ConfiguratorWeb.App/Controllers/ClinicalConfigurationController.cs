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
using System.Xml;
using System.Xml.Schema;
using ConfiguratorWeb.App.SysOptionConfig;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Kendo.Mvc;
using Digistat.FrameworkWebExtensions.Controllers;
using NPOI.OpenXmlFormats.Dml;
using Configurator.Std.Exceptions;
using Configurator.Std.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConfiguratorWeb.App.Controllers
{
   public class ClinicalConfigurationController : DigistatWebControllerBase
   {

      //private readonly ISystemOptionsManager mobjSystemOptionsManager;
      private readonly ISimpleChoiceManager mobjSimpleChoiceManager;
      //private readonly IBedsManager mobjBedsManager;
      //private readonly IUsersManager mobjUsersManager;
      //private readonly ILocationManager mobjLocationManager;
      //private readonly IHospitalUnitsManager mobjHospitalUnitsManager;
      private readonly IPermissionsService mobjPermSvc;
      //private readonly IMiscellaneousManager mobjMiscMgr;
      //private readonly ISystemValidationManager mobjSysValidMgr;
      // private readonly LocationManager mobjLocationsManager;


      public ClinicalConfigurationController(IDigistatConfiguration config, IMessageCenterService msgcenter, IDictionaryService dicSvc
         , ISystemOptionsService sysOptSvc, ISynchronizationService syncSvc, IDnsCacherService dnsSvc, ILoggerService logSvc
         , IDictionaryService dicService, IPermissionsService permSvc , ISimpleChoiceManager simpleChoiceManager
         )
            : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)

      {
         mobjSimpleChoiceManager = simpleChoiceManager;
       
         mobjPermSvc = permSvc;
      }

      //public ActionResult Index()
      //{
      //   return View();
      //}

      public ActionResult CustomLists()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListView, CurrentUser))
         {
            ViewBag.SitePath = "General > Clinical Configuration > Custom Lists";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult ReadSimpleChoicesGroup([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            Dictionary<string,int> simpleChoices = mobjSimpleChoiceManager.GetAllGroup();
            List<SimpleChoiceGroupViewModel> objList = SimpleChoiceGroupViewModelBuilder.BuildList(simpleChoices).ToList();

            var ret = new JsonResult(objList.ToDataSourceResult(request));
            return ret;
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
         return new JsonResult(new {succes=false});
      }
      public JsonResult ReadSimpleChoices([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            var simpleChoices = mobjSimpleChoiceManager.GetAll().OrderBy(o=>o.Group).ThenBy(o=>o.Index);
            List<SimpleChoiceViewModel> objList = SimpleChoiceViewModelBuilder.BuildList(simpleChoices).ToList();

            var ret = new JsonResult(objList.ToDataSourceResult(request));
            return ret;
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
         return new JsonResult(new {succes=false});
      }

     
      public IActionResult GetSimpleChoice(string groupId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListView, CurrentUser))
         {
            SimpleChoiceGroupedViewModel model = new SimpleChoiceGroupedViewModel(){Group = groupId};
            if (groupId.Trim().Length > 0)
            {
               IEnumerable<SimpleChoice> choises = mobjSimpleChoiceManager.GetGroupChoises(groupId);
               model.Choices = SimpleChoiceViewModelBuilder.BuildList(choises).ToList();
               //model.LastSavedChoices = SimpleChoiceViewModelBuilder.BuildList(choises).ToList();

            }

            ViewBag.ChoiseGroup = groupId;
            return View("_SimpleChoise", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }
      public JsonResult GetGroupedSimpleChoices([DataSourceRequest] DataSourceRequest request,string parentId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListView, CurrentUser))
         {
            List<SimpleChoiceViewModel> model = new List<SimpleChoiceViewModel>();
            if (parentId.Trim().Length> 0)
            {
               IEnumerable<SimpleChoice> choises = mobjSimpleChoiceManager.GetGroupChoises(parentId);
               model = SimpleChoiceViewModelBuilder.BuildList(choises).ToList();
               //model.LastSavedChoices = SimpleChoiceViewModelBuilder.BuildList(choises);

            }
            
            var ret = new JsonResult(model.ToDataSourceResult(request));
            return ret;
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public IActionResult CreateDetails(string groupId )
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (!mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListEdit, CurrentUser))
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               return Json(new { errorMessage = messageError, success = bolSuccess });
            }
            SimpleChoiceGroupedViewModel model = new SimpleChoiceGroupedViewModel(){Group = groupId};
            SimpleChoice choisesNew = new SimpleChoice();
            if (groupId != null && groupId.Length > 0)
            {

               
               var onDb = mobjSimpleChoiceManager.GetGroupChoises(groupId);
               if (!onDb.Any())
               {
                  choisesNew.Group = groupId;
                  choisesNew.Choice = "-";
                  choisesNew.Index = 1;

                  var obj = mobjSimpleChoiceManager.CreateGroup(choisesNew);
                  bolSuccess = obj != null;
                  
                  if (groupId.Trim().Length > 0)
                  {
                     IEnumerable<SimpleChoice> choises = mobjSimpleChoiceManager.GetGroupChoises(groupId);
                     model.Choices = SimpleChoiceViewModelBuilder.BuildList(choises).ToList();
                     //model.LastSavedChoices = SimpleChoiceViewModelBuilder.BuildList(choises).ToList();

                  }
               }
               else
               {
                  messageError = mobjDicSvc.XLate("List alreay exists");
               }
            }

            if (bolSuccess)
            {
               ViewBag.ChoiseGroup = groupId;
               return View("_SimpleChoise", model);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }
      public IActionResult SaveDetails(string stringifyGrid )
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (!mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListEdit, CurrentUser))
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               return Json(new { errorMessage = messageError, success = bolSuccess });
            }

            List<SimpleChoice> choisesNew = new List<SimpleChoice>();
            if (stringifyGrid != null && stringifyGrid.Length>0)
            {
               try
               {
                  choisesNew = JsonConvert.DeserializeObject<List<SimpleChoice>>(stringifyGrid);   
               }
               catch (Exception e)
               {
                  
               }
               

               //List<SimpleChoice> simpleChoices = Sim
            }

            if (choisesNew.Count==0)
            {
               messageError = "Nothing to save";
            }

            var onDb = mobjSimpleChoiceManager.GetGroupChoises(choisesNew[0].Group);
            if (onDb.Any())
            {
               bolSuccess = mobjSimpleChoiceManager.UpdateGroup(choisesNew);
            }
            
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }


      }

      public IActionResult DeleteGroup(string groupId)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (!mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionClinicalCustomListEdit, CurrentUser))
            {
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               return Json(new { errorMessage = messageError, success = bolSuccess });
            }

            List<SimpleChoice> choisesNew = new List<SimpleChoice>();
            if (groupId != null && groupId.Length > 0)
            {


               var onDb = mobjSimpleChoiceManager.GetGroupChoises(groupId);
               if (onDb.Any())
               {
                  bolSuccess = mobjSimpleChoiceManager.DeleteGroup(groupId);
               }
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
   }

   
}