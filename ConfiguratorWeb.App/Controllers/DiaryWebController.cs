using Configurator.Std.BL;
using ConfiguratorWeb.App.Models.DiaryWeb;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Model.DiaryWeb;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Controllers
{
   public class DiaryWebController : DigistatWebControllerBase
   {

      private const string SMARTSV = "SMARTSUPERVISOR";
      internal const int DEFAULT_PERMISSION_LEVEL = 98;
      internal const string DEFAULT_PERMISSION_MODIFIER = "Z";
      private IDiaryWebManager mobjDiaryMgr;
      private IPermissionsService mobjPermSvc;
      private IDBPermissionManager mobjPermMgr = null;
      private static readonly string APP = "CONFIGURATORWEB";
      private static readonly string DESTINATION = "DIARYWEBAPI";
      private static readonly string MSG_UPDATE = "DIARY_CONFIG_UPDATE";
      private static readonly string MSG_NEW = "DIARY_CONFIG_NEW";
      private readonly IMessageCenterService mobjMsgCtrSvc = null;

      public DiaryWebController(IDigistatConfiguration config, IMessageCenterService msgcenter, ISystemOptionsService sysOptSvc, ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
      IDiaryWebManager diaryWebManager, IPermissionsService permSvc, IDBPermissionManager permMgr)
      : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDiaryMgr = diaryWebManager;
         mobjPermSvc = permSvc;
         mobjPermMgr = permMgr;
         mobjMsgCtrSvc = msgcenter;

      }

      private void SendUMSMessageDiary(string msg)
      {
         var objMessage = new MCMessage();
         objMessage.Encrypt = true;
         objMessage.SourceApp = APP;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.DestinationApp = DESTINATION;
         objMessage.DestinationHost = DestinationHostCodes.All.GetDisplayAttribute();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         objMessage.Message = msg;
         mobjMsgCtrSvc.SendMessage(objMessage);
      }

      private void SendDiaryConfigNew()
      {
         SendUMSMessageDiary(MSG_NEW);
      }

      private void SendDiaryConfigUpdate()
      {
         SendUMSMessageDiary(MSG_UPDATE);
      }

      public IActionResult Index()
      {
         ViewBag.SitePath = "Modules > Diary > Configure";
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, CurrentUser))
         {
            return View();
         }
         else
         {
            return View("NotAuthorized");
         }
      }

      #region TAG
      [HttpPost]
      public async Task<JsonResult> GetTags([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, CurrentUser))
         {
            try
            {
               var ret = await mobjDiaryMgr.GetTags();
               var tags = ret.OrderBy(x => x.DtgIndex);
               DataSourceResult data = DiaryWebViewModelBuilder.BuildTagsModelList(tags).ToDataSourceResult(request);
               return new JsonResult(data);
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "GetTags Error");
               return Json(new { errorMessage = "Error Retrieving Tags", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }

      [HttpGet]
      public IActionResult GetTag(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, CurrentUser))
         {
            try
            {
               Tag pbjTag = null;
               if (id > 0)
               {
                  var t = mobjDiaryMgr.GetTag(id);
                  t.Wait();
                  if (t.Result != null)
                     pbjTag = t.Result.ToModel();
                  if (pbjTag.ColorTag.Contains("\r\n"))
                  {
                     pbjTag.ColorTag = pbjTag.ColorTag.Replace("\r\n", "");
                  }
               }
               else
               {
                  pbjTag = new Tag(); //this is for adding a new Tag
                  pbjTag.IsActiveTag = true;
               }

               return PartialView("_DiaryTagItemDetail", pbjTag);
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "GetTag Error");
            }
         }
         return StatusCode(StatusCodes.Status403Forbidden);
      }

      [HttpPost]
      public async Task<JsonResult> SaveDiaryTag(Tag tag)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {

               if (tag != null)
               {

                  var dtag = new DiaryTag()
                  {
                     DtgId = tag.IDtag,
                     DtgColor = tag.ColorTag,
                     DtgName = tag.TextTag,
                     DtgIndex = tag.IndexTag,
                     DtgIsActive = tag.IsActiveTag,
                     DtgIsSystem = tag.IsSystemTag
                  };
                  var result = await mobjDiaryMgr.EditTag(dtag);
                  if (result == null)
                  {
                     throw new Exception("Failed to save Tag");
                  }

                  SendDiaryConfigUpdate();
                  return Json(new { tagID = result, success = true });
               }
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "SaveDiaryTag Error");
            }
            return Json(new { errorMessage = "Error Saving Tag", success = false });
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }


      [HttpPost]
      public async Task<IActionResult> DeleteDiaryTag(int ID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = await mobjDiaryMgr.RemoveTag(ID);
               if (result == 0)
               {
                  throw new Exception("Failed to remove Subject");
               }

               return Json(new { tagID = result, success = true });
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "DeleteDiaryTag Error");
               return Json(new { errorMessage = "Error Removing Tag", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }
      #endregion

      #region Category

      [HttpPost]
      public async Task<JsonResult> GetCategories([DataSourceRequest] DataSourceRequest request)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, CurrentUser))
         {
            try
            {
               var ret = await mobjDiaryMgr.GetCategories();
               var orderedCat1 = ret.OrderBy(x => x.DcaName);
               var orderedCat2 = orderedCat1.OrderBy(x => x.DcaIndex);
               DataSourceResult data = DiaryWebViewModelBuilder.BuildCategoriesModelList(orderedCat2).ToDataSourceResult(request);
               return new JsonResult(data);
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "GetCategories Error");
               return Json(new { errorMessage = "Error Retrieving Categories", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }
      [HttpGet]
      public IActionResult CategoryReordering()
      {
         return PartialView("_CategoryReordered");


      }
      [HttpGet]
      public IActionResult TagReordering()
      {
         return PartialView("_TagReordered");


      }
      [HttpGet]
      public IActionResult GetCategory(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, CurrentUser))
         {
            try
            {
               Category objCat = null;
               if (id > 0)
               {
                  var t = mobjDiaryMgr.GetCategory(id);
                  t.Wait();
                  if (t.Result != null)
                  {
                     var newObject = mobjDiaryMgr.CheckEditableSubjects(t.Result);
                     objCat = newObject.ToModel();

                  }
                  objCat.Subjects = objCat.Subjects.OrderBy(x => x.Index);
                  objCat.Phrases = objCat.Phrases.OrderBy(x => x.Index);


               }
               else
               {
                  objCat = new Category();
                  objCat.IsActive = true;

               }



               return PartialView("_CategoryItemDetail", objCat);
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "GetCategory Error");
            }
         }
         return StatusCode(StatusCodes.Status403Forbidden);
      }

      [HttpPost]
      public async Task<JsonResult> SaveCategory(Category c, bool isNameUpdated, string oldName)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               if (c != null)
               {
                  string trimNameCat = c.Text.TrimEnd();
                  if (c.ID == 0 || isNameUpdated)
                  {

                     string strRlpText = trimNameCat.Replace(' ', '_');
                     string strCatPermission = "DIARY.NEW." + strRlpText.ToUpper();

                     if (c.ID != 0 && isNameUpdated)
                     {
                        string strRplTextOld = oldName.Replace(' ', '_').ToUpper();
                        string strCatPermissionOld = "DIARY.NEW." + strRplTextOld;
                        bool resultP = await mobjDiaryMgr.UpdatePermissionCategory(strCatPermissionOld, strCatPermission);
                        if (!resultP)
                        {
                           throw new Exception("Failed to Update Category Permission or RolePermission Operations");
                        }

                     }
                     else
                     {
                        Digistat.FrameworkStd.Model.Permission objPerm = new Digistat.FrameworkStd.Model.Permission();
                        objPerm.FunctionName = strCatPermission;
                        objPerm.PriorityLevel = DEFAULT_PERMISSION_LEVEL;
                        objPerm.PermissionCode = DEFAULT_PERMISSION_MODIFIER;
                        objPerm.Description = "Diary Category specific permission for Category";
                        mobjPermMgr.CreatePermission(objPerm, mobjPermSvc.GetAdminRoleID(), true);
                     }
                  }
                  DiaryCategory cat = c.ToEntity();
                  int result = await mobjDiaryMgr.SaveCategory(cat);
                  if (result == 2)
                  {
                     throw new Exception("Failed to save Category");
                  }
                  else
                  {
                     if (result == 1)
                     {
                        SendDiaryConfigUpdate();
                     }
                     else
                     {
                        SendDiaryConfigNew();
                     }
                  }
                  return Json(new { success = true });
               }
               else
               {
                  return Json(new { errorMessage = "Category not found", success = false });
               }
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "SaveCategory Error");
               return Json(new { errorMessage = "Error Saving Category", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }

      [HttpPost]
      public async Task<JsonResult> ReorderDiaryCategory(Dictionary<int, int> categories)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {

               if (categories.Count > 0)
               {
                  bool result = await mobjDiaryMgr.ReorderCagtegory(categories);
                  if (!result)
                     throw new Exception("Failed to save Category");
                  else
                     SendDiaryConfigUpdate();
                  return Json(new { success = true });

               }
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "Reorder category Error");
               return Json(new { errorMessage = "Error Reorder category", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }
      [HttpPost]
      public async Task<JsonResult> ReorderDiaryTag(Dictionary<int, int> tags)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {

               if (tags.Count > 0)
               {
                  bool result = await mobjDiaryMgr.ReorderTag(tags);
                  if (!result)
                     throw new Exception("Failed to save Tag");
                  else
                     SendDiaryConfigUpdate();
                  return Json(new { success = true });

               }
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "Reorder tags Error");
               return Json(new { errorMessage = "Error Reorder tags", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }

      [HttpPost]
      public async Task<IActionResult> DeleteCategory(int ID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = await mobjDiaryMgr.RemoveCategory(ID);
               var resultR = await mobjDiaryMgr.ResetCategoryIndex();
               if (result == 0)
               {
                  return Json(new { tagID = result, success = false });

               }
               if (!resultR)
               {
                  return Json(new { tagID = result, success = false });

               }
               if (result == 2)
               {
                  return Json(new { tagID = result, success = true, resCode = 1 });
               }
               return Json(new { tagID = result, success = true, resCode = 0 });
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "DeleteCategory Error");
               return Json(new { errorMessage = "Error Removing Category" + ex.Message, success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }



      [HttpPost]
      public async Task<IActionResult> DeactiveCategory(int ID)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = await mobjDiaryMgr.DeactiveCategory(ID);


               if (result == 0)
               {
                  return Json(new { tagID = result, success = false });

               }
               if (result == 1)
               {
                  return Json(new { tagID = result, success = true, statusCode = 0 });

               }
               if (result == 2)
               {
                  return Json(new { tagID = result, success = true, statusCode = 1 });
               }

            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "DeactiveCategory Error");
               return Json(new { errorMessage = "Error Deactivating Category" + ex.Message, success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }
      #endregion

      #region SUBJECT
      [HttpPost]
      public async Task<IActionResult> EditSubject(int categoryId, int subjectID, string subject)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = 0;
               if (subjectID > 0)//update
               {
                  result = await mobjDiaryMgr.UpdateSubject(subjectID, subject);
                  if (result == 0)
                  {
                     throw new Exception("Failed to update Subject");
                  }
               }
               else
               {
                  result = await mobjDiaryMgr.AddSubject(categoryId, subject);
                  if (result == 0)
                  {
                     throw new Exception("Failed to add Subject");
                  }
               }
               return Json(new { subjectID = result, success = true });
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "AddSubject Error");
               return Json(new { errorMessage = "Error Saving Subject", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }

      [HttpPost]
      public async Task<IActionResult> RemoveSubject(int subjectId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = await mobjDiaryMgr.RemoveSubject(subjectId);
               if (result == 0)
               {
                  throw new Exception("Failed to add Subject");
               }
               return Json(new { subjectID = result, success = true });
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "AddSubject Error");
               return Json(new { errorMessage = "Error Saving Subject", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }

      [HttpPost]
      public IActionResult GetSubject(Subject s)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebView, CurrentUser))
         {
            try
            {
               Subject objSub = null;
               if (s.Text != "")
               {

                    objSub = s;
                    if(objSub != null)
                    {
                        if(objSub.SubjectsPhrases != null)
                        {
                            var orderderSubPh = s.SubjectsPhrases.OrderBy(x => x.Index);
                            objSub.SubjectsPhrases = orderderSubPh;
                        }
                    }
               }
               else
               {
                  objSub = new Subject();
                  objSub.ID = 0;
                  objSub.IsActive = true;

               }



               return PartialView("_SubjectPhraseDialog", objSub);
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "GetSubject Error");
            }
         }
         return StatusCode(StatusCodes.Status403Forbidden);
      }

#endregion
#region CATEGORY PHRASE

      [HttpPost]
      public async Task<IActionResult> EditCategoryPhrase(int categoryId, int phraseID, string phrase)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = 0;
               if (phraseID > 0)//update
               {
                  result = await mobjDiaryMgr.UpdateCategoryPhrases(phraseID, phrase);
                  if (result == 0)
                  {
                     throw new Exception("Failed to update Category Phrase");
                  }
               }
               else //add
               {
                  result = await mobjDiaryMgr.AddCategoryPhrases(categoryId, phrase);
                  if (result == 0)
                  {
                     throw new Exception("Failed to add Category Phrase");
                  }
               }
               return Json(new { phraseID = result, success = true });
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "AddSubject Error");
               return Json(new { errorMessage = "Error Saving Subject", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }

      [HttpPost]
      public async Task<IActionResult> RemoveCategoryPhrase(int phraseId)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionDiaryWebEdit, CurrentUser))
         {
            try
            {
               int result = await mobjDiaryMgr.RemoveCategoryPhrases(phraseId);
               if (result == 0)
               {
                  throw new Exception("Failed to add Subject");
               }
               return Json(new { subjectID = result, success = true });
            }
            catch (Exception ex)
            {
               mobjLogSvc.ErrorException(ex, "AddSubject Error");
               return Json(new { errorMessage = "Error Saving Subject", success = false });
            }
         }
         return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
      }
      #endregion
   }
}
