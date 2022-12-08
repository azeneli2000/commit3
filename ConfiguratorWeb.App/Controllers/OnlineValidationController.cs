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
using Configurator.Std.BL.Online;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.EntityBuilders;
using Configurator.Std.BL.OnLine;
using Digistat.FrameworkStd.Model.Online;
using ConfiguratorWeb.App.Models.OnLine;
using Configurator.Std.Enums;
using Newtonsoft.Json;

namespace ConfiguratorWeb.App.Controllers
{
   [DigConfigFilterAttribute]
   [DigistatAuthFilterAttribute]
   public class OnlineValidationController : DigistatWebControllerBase
   {

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IValidationGroupManager mobjValMgr;
      private readonly IOnlineQueriesManager mobjOnlineQueriesMgr;
      private readonly IPermissionsService mobjPermSvc;

      public OnlineValidationController(IDigistatConfiguration config, IMessageCenterService msgcenter,
        ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
        ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc, IMessageCenterManager msgCtrMgr,
        IPermissionsService permSvc, IValidationGroupManager valGrouMgr,IOnlineQueriesManager onlineQueriesMgr)
     : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjPermSvc = permSvc;
         mobjValMgr = valGrouMgr;
         mobjOnlineQueriesMgr = onlineQueriesMgr;
      }


      public IActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > OnLine > Validation Groups";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult QueriesParameters()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > OnLine > Custom Queries";
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      public JsonResult ReadValidationGroups([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            IEnumerable<ValidationGroup> objAllValidationGroups = mobjValMgr.GetList();
            DataSourceResult data = objAllValidationGroups.ToDataSourceResult(request, model => ValidationGroupViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }

      public IActionResult GetValidationGroup(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            ValidationGroupViewModel model = new ValidationGroupViewModel();

            if (id > 0)
            {
               ValidationGroup objValGroup = mobjValMgr.Get(id);
               model = ValidationGroupViewModelBuilder.Build(objValGroup);
            }
            return View("_ValidationGroup", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public IActionResult GetValidationParameterDialog()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            ConfiguratorWeb.App.Models.OnLine.ValidationParameterViewModel m = new ValidationParameterViewModel();
            return View("_ValidationParameterDialog",m);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult MoveValidationGroup(int valGroupID, int direction)
      {
         string messageError = string.Empty;
         bool bolSuccess = true;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, CurrentUser))
            {
               if (valGroupID != 0)
               {
                  //Do validation group movement
                  mobjValMgr.MoveValidationGroup(valGroupID, (MoveDirection)direction);
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
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }

      #region Save


      [RequestFormSizeLimit(valueCountLimit: 20000)]
      [HttpPost]
      public JsonResult SaveValidationGroupDetail(ValidationGroupViewModel model)
      {
         string messageError = string.Empty;
         Network objNetwork = null;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, CurrentUser))
            {
               if (!string.IsNullOrWhiteSpace(model.ValidationParameterSerialized))
               {
                  model.Parameters = JsonConvert.DeserializeObject<ICollection<ValidationParameterViewModel>>(model.ValidationParameterSerialized);
               }
               ValidationGroup vgItem = ValidationGroupBuilder.Build(model);
               vgItem.LastUpdate = DateTime.Now;
               vgItem.UserID = CurrentUser!=null?CurrentUser.Id:string.Empty;
               if (vgItem.ID != 0)
               {
                  bolSuccess = mobjValMgr.Update(vgItem,CurrentUser.Abbrev);
               }
               else
               {
                  bolSuccess = mobjValMgr.Create(vgItem, CurrentUser.Abbrev);
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
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }


      public JsonResult DeleteValidationGroup(int vgID)
      {
         string messageError = string.Empty;
         Network objNetwork = null;
         bool bolSuccess = false;
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, CurrentUser))
            {
               if (vgID != 0)
               {
                  bolSuccess = mobjValMgr.Delete(vgID, CurrentUser != null ? CurrentUser.Abbrev: string.Empty, CurrentUser != null ? CurrentUser.Id : string.Empty);
               }
            }
            else
            {
               bolSuccess = false;
               messageError = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch(Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
      }
      #endregion

      #region Sections

      public IActionResult SectionEditingDialog()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            return View("_ValidationSections");
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }


      public JsonResult ReadValidationSections([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationView, CurrentUser))
         {
            IEnumerable<ValidationSection> objAllValidationGroups = mobjValMgr.GetSectionList();
            DataSourceResult data = objAllValidationGroups.ToDataSourceResult(request, model => ValidationSectionViewModelBuilder.Build(model));
            return new JsonResult(data);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }

      }


      

      [AcceptVerbs("Post")]
      public ActionResult Section_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationSectionViewModel> items)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, CurrentUser))
            {
               var results = new List<ValidationSectionViewModel>();
               if (items != null && ModelState.IsValid)
               {
                  foreach (var vsItem in items)
                  {
                     ValidationSection objSection = ValidationSectionBuilder.Build(vsItem);
                     mobjValMgr.CreateSection(objSection);
                  }
               }

               return Json(results.ToDataSourceResult(request, ModelState));
            }
            else
            {

               //throw (new Exception(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION)));
               return Unauthorized(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION));
            }
         }
         catch(Exception e)
         {
            
            mobjLogSvc.ErrorException(e,"Section_Create");
            return StatusCode(500,"An internal error occurred, see logs for details");
         }
         
      }

      [AcceptVerbs("Post")]
      public ActionResult Section_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationSectionViewModel> items)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, CurrentUser))
            {
               if (items != null && ModelState.IsValid)
               {
                  foreach (var vsItem in items)
                  {
                     ValidationSection objSection = ValidationSectionBuilder.Build(vsItem);
                     mobjValMgr.UpdateSection(objSection);
                  }
               }

               return Json(items.ToDataSourceResult(request, ModelState));
            }
            else
            {
               throw (new Exception(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION)));
            }
            
         }
         catch (Exception e)
         {
            
            mobjLogSvc.ErrorException(e,"Section_Update");
            return StatusCode(500,"An internal error occurred, see logs for details");
         }
      }

      [AcceptVerbs("Post")]
      public ActionResult Section_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationSectionViewModel> items)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionValidationEdit, CurrentUser))
            {
               if (items.Any())
               {
                  foreach (var vsItem in items)
                  {
                     mobjValMgr.DeleteSection(vsItem.ID);
                  }
               }

               return Json(items.ToDataSourceResult(request, ModelState));
            }
            else
            {
               throw (new Exception(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION)));
            }
            
         }
         catch(Exception e)
         {
            mobjLogSvc.ErrorException(e,"Section_Destroy");
            return StatusCode(500,"An internal error occurred, see logs for details");
         }
        
      }

      public IActionResult GetQuery(int id)
      {
          if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersView, CurrentUser))
          {

                try
                {
                    QueryParameterViewModel model = new QueryParameterViewModel();

                    if (id > 0)
                    {
                        OnlineQuery objQuery = mobjOnlineQueriesMgr.GetQueryable().FirstOrDefault(q => q.Id == id);
                        model = QueryParameterViewModelBuilder.Build(objQuery);
                    }
                    return View("_QueryParameter", model);
                }
                catch (Exception e)
                {
                    mobjLogSvc.ErrorException(e,$"Error reading OnlineQuery with id {id}");
                    return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.INTERNAL_SERVER_ERROR), success = false });
                }
          }
          else
          {
              return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
          }
      }

      //[AcceptVerbs("Get,Post")]
      public IActionResult GetAllQueries([DataSourceRequest] DataSourceRequest request)
      {
          if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersView, CurrentUser))
          {
              IEnumerable<OnlineQuery> objAllQueries = mobjOnlineQueriesMgr.GetAll();
              IEnumerable<QueryParameterViewModel> vm = QueryParameterViewModelBuilder.BuildList(objAllQueries);
              DataSourceResult data = vm.ToDataSourceResult(request, model => model);
              return new JsonResult(data);
          }
          else
          {
              return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
          }
          
      }

      public ActionResult DeleteQueryDetail(int queryId)
      {
          try
          {
              if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersEdit, CurrentUser))
              {
                  if (queryId >0)
                  {
                      try
                      {

                              mobjOnlineQueriesMgr.Delete(queryId,CurrentUser.Abbrev,CurrentUser.Id);
                          
                      }
                      catch (Exception e)
                      {
                          mobjLogSvc.ErrorException(e,"DeleteQueryDetail");
                          return new JsonResult(new {success=false, errorMessage=mobjDicSvc.XLate("An internal error occurred, see logs for details") });
                      }
                      
                  }

                  return new JsonResult(new {success=true});
              }
              else
              {
                  throw (new Exception(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION)));
              }
            
          }
          catch (Exception e)
          {
            
              mobjLogSvc.ErrorException(e,"DeleteQueryDetail");
              return StatusCode(500,mobjDicSvc.XLate("An internal error occurred, see logs for details"));
          }
      }
      public ActionResult SaveQueryDetail(QueryParameterViewModel query)
      {
          try
          {
              if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionQueryParametersEdit, CurrentUser))
              {
                  if (query != null && ModelState.IsValid)
                  {
                      try
                      {

                      
                          var dbQuery = new OnlineQuery
                          {
                              Id = query.QueryID,
                              ParameterLabel = query.ParameterLabel,
                              UnitLabel = query.UnitLabel,
                              Description = query.Description,
                              Query = query.SQLQuery,
                              LastSaveUserId = mobjSyncSvc.GetCurrentUser().Id,
                              LastSaveDatetime = DateTime.Now
                          };
                          if (query.QueryID >0)
                          {
                              mobjOnlineQueriesMgr.Update(dbQuery);
                              mobjLogSvc.Info($"The query [{JsonConvert.SerializeObject(query)}] was edited ");
                          }
                          else
                          {
                              var newQuery = mobjOnlineQueriesMgr.Create(dbQuery);
                              
                              mobjLogSvc.Info($"The query [{JsonConvert.SerializeObject(newQuery)}] was created ");
                          }
                      }
                      catch (Exception e)
                      {
                          mobjLogSvc.ErrorException(e,"SaveQueryDetail");
                          return new JsonResult(new {success=false, errorMessage=mobjDicSvc.XLate("An internal error occurred, see logs for details") });
                      }
                      
                  }

                  return new JsonResult(new {success=true});
              }
              else
              {
                  throw (new Exception(mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION)));
              }
            
          }
          catch (Exception e)
          {
            
              mobjLogSvc.ErrorException(e,"SaveQueryDetail");
              return StatusCode(500,mobjDicSvc.XLate("An internal error occurred, see logs for details"));
          }
      }
   }


   #endregion

}



