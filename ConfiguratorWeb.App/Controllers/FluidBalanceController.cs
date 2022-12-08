using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Std.BL;
using ConfiguratorWeb.App.Extensions;
using ConfiguratorWeb.App.Models.FluidBalance;
using ConfiguratorWeb.App.ViewModelBuilders;
using ConfiguratorWeb.App.EntityBuilders;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.FluidBalance;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConfiguratorWeb.App.Controllers
{

  
   public class FluidBalanceController : DigistatWebControllerBase
   {
      public readonly IFluidBalanceManager mobjFluidBalanceDataManager;
      private readonly IPermissionsService mobjPermSvc;
      public FluidBalanceController(IDigistatConfiguration config, IMessageCenterService msgcenter,
       ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IPermissionsService permSvc,
       IFluidBalanceManager fluidDataManager)
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjPermSvc = permSvc;
         mobjFluidBalanceDataManager = fluidDataManager;
      }
      public IActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > Fluid Balance > Fluid Balance Items";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public IActionResult FluidBalanceItems()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > Fluid Balance > Fluid Balance Items";
            return View("FluidBalanceConfig");
         }
         else
         {
            return View("NotAuthorized");
         }
      }


      public JsonResult ReadFilteredFluidBalanceItems([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBView, CurrentUser))
            {
               IQueryable<FluidBalanceItemModel> ret = mobjFluidBalanceDataManager.GetFBStandarItem();
               if (request.Sorts?.Count == 0)
               {
                  ret = ret.OrderBy(i => i.Index);
               }

               IEnumerable<FluidBalanceViewModel> model = FluidBalanceViewModelBuilder.BuildList(ret); 
    
               DataSourceResult datares = model.ToDataSourceResult(request);



               return new JsonResult(datares);
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

      [HttpGet]
      public IActionResult CurrentFluidBalanceItem(int selectedItem)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBView, CurrentUser))
         {
            if (selectedItem > 0)
            {
               FluidBalanceItemModel objSingleFluidBalanceItem = mobjFluidBalanceDataManager.GetFBStandarItemById(selectedItem);
               FluidBalanceViewModel model = FluidBalanceViewModelBuilder.Build(objSingleFluidBalanceItem);
               return PartialView("_FluidBalanceDetails", model);
            }
            else
            {
               FluidBalanceViewModel objNewFluidBalaceitem = new FluidBalanceViewModel(); 
               return PartialView("_FluidBalanceDetails", objNewFluidBalaceitem);
            }
         }
         else
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      [HttpPost]
      public JsonResult SaveDetail(FluidBalanceViewModel model)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBEdit, CurrentUser))
         {
            string messageError = string.Empty;
            FluidBalanceItemModel objFb = new FluidBalanceItemModel();
            bool bolSuccess = false;
            try
            {
               if (model.Labels == null)
               {
                  model.Labels = string.Empty;
               }
               if (model.Id <= 0)
               {
                  
                  objFb = mobjFluidBalanceDataManager.CreateFBStandardItem(FluidBalanceEntityBuilder.Build(model));
               }
               else
               {
                  objFb = mobjFluidBalanceDataManager.UpdateFBStandardItem(FluidBalanceEntityBuilder.Build(model));
               }
               if (objFb != null)
               {
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
               return Json(new { errorMessage = ex.Message, success = false });
            }
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      [HttpPost]
      public JsonResult DeleteDetail(int Id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionFBEdit, CurrentUser))
         {
            bool bolSuccess = false;
            string strRet = mobjDicSvc.XLate("Error in Delete Operation");
            try
            {
               bolSuccess = mobjFluidBalanceDataManager.DeleteFBStandardItem(Id);
               if (bolSuccess)
               {
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = strRet, success = false });
               }
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

      [HttpPost]
      public JsonResult CheckFBItemLocationAssociationIsValid([DataSourceRequest] DataSourceRequest request, int? locationID, string strFBItemName, short intFBItemMode, int FBItemID)
      {
         string messageError = string.Empty;
         try
         {
            string trimmedName = strFBItemName.Trim();
            IEnumerable<FluidBalanceItemModel> objEntityList = mobjFluidBalanceDataManager.Find(p => p.IdLocation == locationID  && p.Name.Trim() == trimmedName && p.Mode == intFBItemMode && p.Id != FBItemID);
            if (objEntityList.Count() <= 0)
            {
               return Json(new { errorMessage = string.Empty, success = true });
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate("An item with the same name and mode already exists for this location"), success = false });
            }
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
   }
}