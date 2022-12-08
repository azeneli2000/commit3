using Configurator.Std.BL.Vitals;
using ConfiguratorWeb.App.Extensions;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Vitals;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.Controllers
{
   public sealed class VitalsController : DigistatWebControllerBase
   {
      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private readonly IPermissionsService mobjPermSvc;
      private ILoggerService mobjLog;
      private IVitalsManager mobjVitMgr;

      public VitalsController(IDigistatConfiguration config, IMessageCenterService msgcenter,
       ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc,
       IDigistatEnvironmentService digEnvSvc, IVitalsManager vitMgr, IPermissionsService permSvc)
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjVitMgr = vitMgr;
         mobjPermSvc = permSvc;

         if (mobjSystemOptionsService != null)
         {
            mobjSystemOptionsService.CheckAndCreateSystemOption(
            applicationName: mobjDigistatConfig.ModuleName, userAbbrev: null, hospitalUnitShortName: null, hostName: null
            , optionName: "OCRDevices"
            , value: @"<OCRDevices>
                            <OCRDevice>
                            <ID>1</ID>
                            <Name>GEV100</Name>
                            <Parameters>
                                <Param>
                                    <ParamID>3009</ParamID>
                                    <Name>Systolic</Name>
                                    <Unit>mmHg</Unit>
                                </Param>
                                <Param>
                                    <ParamID>3011</ParamID>
                                    <Name>Diastolic</Name>
                                    <Unit>mmHg</Unit>
                                </Param>
                                <Param>
                                    <ParamID>7010</ParamID>
                                    <Name>SPO2</Name>
                                    <Unit>%</Unit>
                                </Param>
                                <Param>
                                    <ParamID>6024</ParamID>
                                    <Name>Temperature</Name>
                                    <Unit>°C</Unit>
                                </Param>
                                <Param>
                                    <ParamID>3001</ParamID>
                                    <Name>Pulse Rate</Name>
                                    <Unit>bpm</Unit>
                                </Param>
                                <Param>
                                    <ParamID>3017</ParamID>
                                    <Name>MAP/cuff</Name>
                                    <Unit>>mmHg</Unit>
                                </Param>
                            </Parameters>
                            </OCRDevice>
                            </OCRDevices>"
         , description: null, type: Digistat.FrameworkStd.Enums.OptionType.Text, upperLimit: 0, lowerLimit: 0, level: 0, isSystem: true);
         }
      }

      // GET: Actions
      public ActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            ViewBag.SitePath = "Modules>Vitals>Dataset";
            ViewBag.CanImport = mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalImport, CurrentUser);

            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult GetAllStandardDataset([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            IEnumerable<StandardDataset> objAllSystems = mobjVitMgr.GetAll(true);
            DataSourceResult data = objAllSystems.ToDataSourceResult(request, model => StandardDatasetViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetAllItemsForDS([DataSourceRequest] DataSourceRequest request, Guid dsID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            IEnumerable<StandardDatasetItem> objAllSystems = mobjVitMgr.GetItemsForDS(dsID);

            objAllSystems = objAllSystems.Where(p => p.si_Type != (int)Digistat.FrameworkStd.Enums.Vitals.ParamType.OcrImage);

            if (request.Filters.Count > 0)
            {
               request.RenameRequestFilterMember("Name", "si_Name");
               request.RenameRequestFilterMember("Label", "si_Label");
               request.RenameRequestFilterMember("Unit", "si_Unit");
               request.RenameRequestFilterMember("ItemType", "si_Type");
               request.RenameRequestFilterMember("Index", "si_Index");
               request.RenameRequestFilterMember("PlaceHolder", "si_PlaceHolder");
            }
            var sort = new Dictionary<string, string[]>()
                {
                   {"Name", new []{"si_Name"}},
                   {"Label", new []{"si_Label"}},
                   {"Unit", new []{"si_Unit"}},
                   {"ItemType", new []{"si_Type"}},
                   {"Index", new []{"si_Index"}},
                   {"PlaceHolder", new []{"si_PlaceHolder"}}
                };

            DataSourceResult data = objAllSystems.ToDataSourceResult(request.SortAttributesMapping(sort), model => SDItemViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetAllSubItemsForItem([DataSourceRequest] DataSourceRequest request, Guid itemID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            if (request.Filters?.Count > 0)
            {
               request.RenameRequestFilterMember("Code", "li_Code");
               request.RenameRequestFilterMember("SubItemIndex", "li_Index");
               request.RenameRequestFilterMember("ColorCode", "li_ColorCode");
               request.RenameRequestFilterMember("SubItemValue", "li_Value");
               request.RenameRequestFilterMember("Label", "li_Label");
            }
            var sort = new Dictionary<string, string[]>()
                {
                   {"Code", new []{"li_Code"}},
                   {"SubItemIndex", new []{"li_Index"}},
                   {"ColorCode", new []{"li_ColorCode"}},
                   {"SubItemValue", new []{"li_Value"}},
                   {"Label", new []{"li_Label"}}
                };

            IEnumerable<StandardDatasetSubItems> objAllSystems = mobjVitMgr.GetSubItemsForItem(itemID);
            DataSourceResult data = objAllSystems.ToDataSourceResult(request.SortAttributesMapping(sort), model => SDSubItemViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetScoreDescriptionsForDataset([DataSourceRequest] DataSourceRequest request, Guid sdID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            if (request.Filters.Count > 0)
            {
               request.RenameRequestFilterMember("MinValue", "dsr_MinValue");
               request.RenameRequestFilterMember("MaxValue", "dsr_MaxValue");
               request.RenameRequestFilterMember("Description", "dsr_Description");
               request.RenameRequestFilterMember("ColorCode", "dsr_ColorCode");
            }
            var sort = new Dictionary<string, string[]>()
                {
                   {"MinValue", new []{"dsr_MinValue"}},
                   {"MaxValue", new []{"dsr_MaxValue"}},
                   {"Description", new []{"dsr_Description"}},
                   {"ColorCode", new []{"dsr_ColorCode"}}
                };

            IEnumerable<StandardDatasetScoreDescription> objAllScoreDesc = mobjVitMgr.GetStdScoreDescriptions(sdID);
            DataSourceResult data = objAllScoreDesc.ToDataSourceResult(request.SortAttributesMapping(sort), model => SDScoreDescriptionViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public ActionResult GetStandardDatasetItem(Guid id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            SDItemViewModel model = new SDItemViewModel();
            if (id != Guid.Empty)
            {
               StandardDatasetItem objItem = mobjVitMgr.GetItem(id);
               model = SDItemViewModelBuilder.Build(objItem);
               if (objItem != null)
               {
                  ViewBag.IsPublished = objItem.si_sd_?.sd_Published == true;

                  if (objItem.si_score_ID.HasValue)
                  {
                     //get standard score dataset
                     StandardDataset sdScore = mobjVitMgr.Get(objItem.si_score_ID.Value);
                     if (sdScore != null)
                     {
                        model.DefaultValueDisplay = sdScore.sd_Name;
                     }
                  }

                  if (objItem.si_Conditional_li_ID.HasValue)
                  {
                     // get conditional item
                     model.ConditionalItemId = mobjVitMgr.GetSubItem(objItem.si_Conditional_li_ID.Value)?.li_si_ID;
                  }
               }
            }

            return PartialView("_SDItemDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult SaveSDItem(SDItemViewModel model)
      {
         StandardDatasetItem objRet = null;
         string strMessageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               objRet = mobjVitMgr.SetItem(ItemEntityBuilder.Build(model));
               if (objRet != null)
               {
                  bolSuccess = true;
               }
            }
            else
            {
               strMessageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = strMessageError, success = bolSuccess, DSItem = SDItemViewModelBuilder.Build(objRet) });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveDataset(StandardDatasetViewModel model)
      {
         string strMessageError = string.Empty;
         StandardDataset objDataset = null;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               objDataset = mobjVitMgr.SetDataset(StandardDatasetEntityBuilder.Build(model), model.UseOcrImage, CurrentUser.Abbrev);
               if (objDataset != null)
               {
                  bolSuccess = true;
               }
            }
            else
            {
               strMessageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = strMessageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveDatasetPublished(StandardDatasetViewModel model)
      {
         string strMessageError = string.Empty;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               bolSuccess = mobjVitMgr.SetDatasetPublished(StandardDatasetEntityBuilder.Build(model), CurrentUser.Abbrev) != null;
            }
            else
            {
               strMessageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = strMessageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveDatasetSubItem(SDSubItemViewModel model)
      {
         string strMessageError = string.Empty;
         StandardDatasetSubItems objSubItem = null;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               objSubItem = mobjVitMgr.SetSubItem(SubItemEntityBuilder.Build(model));
               if (objSubItem != null)
               {
                  bolSuccess = true;
               }
            }
            else
            {
               strMessageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = strMessageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      public JsonResult DeleteDatasetSubItem(SDSubItemViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               if (model != null && model.ID != Guid.Empty)
               {
                  mobjVitMgr.DeleteSubItem(SubItemEntityBuilder.Build(model), CurrentUser.Abbrev);
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = "No SubItem to remove", success = false });
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

      public JsonResult DeleteDatasetScoreDescription(StdScoreDescriptionViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               if (model != null && model.ID != Guid.Empty)
               {
                  mobjVitMgr.DeleteDatasetScoreDescription(StandardScoreDescriptionEntityBuilder.Build(model), CurrentUser.Abbrev);
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = "No Score Description to remove", success = false });
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

      public JsonResult DeleteDatasetItem(SDItemViewModel model)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               if (model != null && model.ID != Guid.Empty)
               {
                  mobjVitMgr.DeleteItem(ItemEntityBuilder.Build(model), CurrentUser.Abbrev);
                  return Json(new { errorMessage = string.Empty, success = true });
               }
               else
               {
                  return Json(new { errorMessage = "No Item to remove", success = false });
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
      public JsonResult DeleteDataset(Guid? id)
      {
         string messageError = string.Empty;
         bool bolSuccess = false;

         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               if (id != null && id != Guid.Empty)
               {
                  mobjVitMgr.DeleteStandardDataset(id.Value, CurrentUser.Abbrev);
                  bolSuccess = true;
               }
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }

            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      public ActionResult GetStandardSubItem(Guid subItemId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            SDSubItemViewModel model = new SDSubItemViewModel();
            if (subItemId != Guid.Empty)
            {
               StandardDatasetSubItems objItem = mobjVitMgr.GetSubItem(subItemId);
               model = SDSubItemViewModelBuilder.Build(objItem);
            }

            return PartialView("_SDSubItemDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public ActionResult GetStandardDataset(Guid id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            StandardDatasetViewModel model = new StandardDatasetViewModel();

            ViewBag.HasRecords = false;

            if (id != Guid.Empty)
            {
               StandardDataset tserver = mobjVitMgr.Get(id);
               model = StandardDatasetViewModelBuilder.Build(tserver);
               var lstItem = mobjVitMgr.GetItemsForDS(tserver.sd_ID);
               model.UseOcrImage = lstItem != null ? lstItem.Where(x => x.si_Type == (int)Digistat.FrameworkStd.Enums.Vitals.ParamType.OcrImage).Count() > 0 : false;

               ViewBag.HasRecords = mobjVitMgr.HasRecords(id);
            }

            return PartialView("_StandardDatasetTabStrip", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult GetAllTimings()
      {
         Dictionary<int, string> objItems = mobjVitMgr.GetDatasetTimings();
         IEnumerable<SelectListItem> objToRender = objItems.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() });
         return new JsonResult(objToRender);
      }

      public ActionResult GetDefaultValueDialog(ParamType type, Guid id)
      {
         if (id != Guid.Empty)
         {
            ViewBag.DatasetItem = mobjVitMgr.GetItem(id);
            ViewBag.StandardDatasetSubItems = mobjVitMgr.GetSubItemsForItem(id)
                .Select(SDSubItemViewModelBuilder.Build)
                .ToList();
         }

         return View("_DefaultValueDialog", type);
      }

      public JsonResult GetAllDatasetTypes()
      {
         Dictionary<int, string> objItems = mobjVitMgr.GetDatasetTypes();

         IEnumerable<SelectListItem> objToRender = objItems.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() });

         return new JsonResult(objToRender);
      }

      public JsonResult GetAllOcrDevices()
      {
         var objSysOption = mobjSysOptSvc.GetSystemOption(mobjDigistatConfig.ModuleName, null, null, null, "OCRDevices", true);
         var ocrDevices = OCRDeviceViewModelBuilder.BuildList(objSysOption.Value);
         var objToRender = ocrDevices.Select(c => new SelectListItem
         {
            Text = c.Name,
            Value = c.ID.ToString()
         });

         return new JsonResult(objToRender);
      }

      public JsonResult GetAllOcrParameters(int deviceId)
      {
         var objSysOption = mobjSysOptSvc.GetSystemOption(mobjDigistatConfig.ModuleName, null, null, null, "OCRDevices", true);
         var ocrDevices = OCRDeviceViewModelBuilder.BuildList(objSysOption.Value);
         var ocrDevice = ocrDevices.FirstOrDefault(x => x.ID == deviceId);

         var objToRender = ocrDevice?.OCRParameters?.Any() == true
             ? ocrDevice.OCRParameters.Where(c => c.Id > 0).Select(c => new SelectListItem { Text = $"{c.Name} - {c.Unit}", Value = c.Id.ToString() }).ToList()
             : new List<SelectListItem>();

         objToRender.Insert(0, new SelectListItem("", "0"));

         return new JsonResult(objToRender);
      }

      public JsonResult GetAllItemTypes()
      {
         Dictionary<int, string> objItems = mobjVitMgr.GetItemTypes();
         IEnumerable<SelectListItem> objToRender = objItems.Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() });
         return new JsonResult(objToRender);
      }

      public ActionResult GetScoreDescription(Guid scoreDescriptionId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsView, CurrentUser))
         {
            StdScoreDescriptionViewModel model = new StdScoreDescriptionViewModel();
            if (scoreDescriptionId != Guid.Empty)
            {
               StandardDatasetScoreDescription objScoreDescription = mobjVitMgr.GetStdScoreDescription(scoreDescriptionId);
               model = SDScoreDescriptionViewModelBuilder.Build(objScoreDescription);
            }

            return PartialView("_SDScoreDescriptionDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      [HttpPost]
      public JsonResult SaveScoreDescription(StdScoreDescriptionViewModel model)
      {
         string strMessageError = string.Empty;
         StandardDatasetScoreDescription objSubItem = null;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               objSubItem = mobjVitMgr.SetScoreDescription(StandardScoreDescriptionEntityBuilder.Build(model));
               if (objSubItem != null)
               {
                  bolSuccess = true;
               }
            }
            else
            {
               strMessageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = strMessageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      public ActionResult GetScoreIDListDialog(string onChangeEvent, string selectedValue)
      {
         ViewBag.ColumnDescriptionTitle = mobjDicSvc.XLate("Dataset Score Name");
         ViewBag.OnSelectEvent = onChangeEvent;
         ViewBag.HtmlAttributes = new { style = "height:460px;margin-top:10px;" };
         ViewBag.SelectedValue = selectedValue;

         IEnumerable<StandardDataset> objAllScores = mobjVitMgr.GetAllScoreDS(true);
         IEnumerable<SelectListItem> model = objAllScores.Select(a => new SelectListItem { Value = a.sd_ID.ToString(), Text = a.sd_Name });
         return View("_GenericPopupSelectList", model);
      }
      [ApiExplorerSettings(IgnoreApi = true)]
      [Route("Vitals/Export/export.bin")]
      public IActionResult Export(Guid? sdID, string name)
      {
         var strContent = mobjVitMgr.Export(sdID.Value);

         return strContent != null
             ? Content(strContent, "application/octet-stream")
             : NotFound();
      }

      public IActionResult Import(ICollection<IFormFile> files)
      {
         bool bolSuccess = true;
         string strMessage = string.Empty;

         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalImport, CurrentUser))
         {
            if (files?.Count > 0)
            {
               foreach (var objFile in files)
               {
                  try
                  {
                     var strJsonContent = objFile.ReadAsString();
                     mobjVitMgr.Import(strJsonContent, CurrentUser?.Abbrev);

                     strMessage += $"<div class='text-success'>{objFile.FileName}: {mobjDicSvc.XLate("Import successfully completed.")}</div>";
                  }
                  catch (InvalidOperationException exception)
                  {
                     strMessage += $"<div class='text-danger'>{objFile.FileName}: {mobjDicSvc.XLate(exception.Message)}</div>";
                     mobjLogSvc.Error($"Error importing {objFile.FileName}: {exception.Message}");
                  }
                  catch (Exception exception)
                  {
                     mobjLogSvc.ErrorException(exception, $"Error importing {objFile.FileName}");
                     strMessage += $"<div class='text-danger'>{objFile.FileName}: {mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR)}</div>";
                  }
               }
            }
         }
         else
         {
            strMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            bolSuccess = false;
         }

         return Json(new { message = strMessage, success = bolSuccess });
      }

      [HttpPost]
      public JsonResult Unpublish(Guid id)
      {
         return SetPublished(id, false);
      }

      [HttpPost]
      public JsonResult Publish(Guid id)
      {
         return SetPublished(id, true);
      }

      private JsonResult SetPublished(Guid sdID, bool state)
      {
         try
         {
            string strMessageError = string.Empty;
            bool bolSuccess;

            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionVitalSignsEdit, CurrentUser))
            {
               mobjVitMgr.SetPublished(sdID, state, CurrentUser.Abbrev);
               bolSuccess = true;
            }
            else
            {
               strMessageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
               bolSuccess = false;
            }
            return Json(new { errorMessage = strMessageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.InnerException is InvalidOperationException objInvalidOperationException ? objInvalidOperationException.Message : ex.Message, success = false });
         }
      }
   }
}