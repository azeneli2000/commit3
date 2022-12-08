using Configurator.Std.BL;
using Configurator.Std.BL.Mobile;
using Configurator.Std.Exceptions;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Filters;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Controllers
{
    public class PositionController : DigistatWebControllerBase
   {
      private readonly IPermissionsService mobjPermSvc;
      private readonly IPositionService mobjPositionService;

      public PositionController(
       IDigistatConfiguration config,
       IMessageCenterService msgcenter,
       ISynchronizationService syncSvc,
       IDictionaryService dicSvc,
       IDnsCacherService dnsSvc,
       ILoggerService logSvc,
       ISystemOptionsService sysOptSvc,
       IPermissionsService permSvc,
       IPositionService positionService) : base(config, msgcenter, syncSvc, dicSvc, dnsSvc, logSvc, sysOptSvc)
      {
         mobjPermSvc = permSvc;
         mobjPositionService = positionService;
      }

      public ActionResult PositionConfig(string PositionCode)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
         {
            ViewBag.SitePath = "Mobile > Positions Config";
            ViewBag.PositionCode = PositionCode;
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult RetrievePositions([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
         {
            IEnumerable<PositionAssociation> objAllPositions = mobjPositionService.GetPositions();
            DataSourceResult data = objAllPositions.ToDataSourceResult(request, model => PositionViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult RetrievePosition(string positionCode)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileView, CurrentUser))
         {
            PositionViewModel model = new PositionViewModel();
            if (!string.IsNullOrWhiteSpace(positionCode))
            {
               PositionAssociation objPositionAssociation = mobjPositionService.GetPositionByPositionCode(positionCode);
               model = PositionViewModelBuilder.Build(objPositionAssociation);
            }
            return View("_PositionTabModal", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult GetBedPosition(string positionCode, [FromBody]IEnumerable<BedViewModel> bedList)
      {
         List<BedViewModel> model = BedViewModelBuilder.BuildList(mobjPositionService.GetAviableBedPosition(positionCode)).OrderBy(o => o.IdLocation).ThenBy(o => o.BedIndex).ToList();

         foreach (BedViewModel bed in bedList)
         {
            BedViewModel objBed = model.Where(a => a.BedId == bed.BedId && a.IdLocation == bed.IdLocation).FirstOrDefault();
            if (objBed != null)
               objBed.Selected = true;
         }

         ViewBag.PositionCode = positionCode;
         return Json(new { content = this.RenderViewAsync("_PositionBedLocation", model, true) });
      }

      [HttpPost]
      public JsonResult DeletePosition(string positionCode)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
            {
               if (!string.IsNullOrWhiteSpace(positionCode))
               {
                  mobjPositionService.DeletePosition(positionCode);
                  bolSuccess = true;
               }
               else
               {
                  messageError = "Missing Position Code";
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

      [RequestFormSizeLimit(valueCountLimit: 20000)]
      [HttpPost]
      public JsonResult SavePositionDetail(PositionViewModel model)
      {
         string messageError = string.Empty;
         PositionAssociation objPosition = null;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionMobileEdit, CurrentUser))
            {
               if (!string.IsNullOrWhiteSpace(model.PositionCode))
               {
                  if (string.IsNullOrWhiteSpace(model.SavedPositionCode))
                  { 
                     objPosition = mobjPositionService.CreatePosition(PositionEntityBuilder.Build(model));
                  }
                  else
                  {
                     objPosition = mobjPositionService.UpdatePosition(PositionEntityBuilder.Build(model));
                  }
               }
               else
               {
                  messageError = "Missing Position Code";
               }

               if (objPosition != null)
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
         catch (PositionException ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
   }
}