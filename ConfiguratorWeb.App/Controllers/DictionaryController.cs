using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Interfaces;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Digistat.FrameworkStd.Exceptions;
using Configurator.Std.BL.Dictionary;
using ConfiguratorWeb.App.Builders;
using ConfiguratorWeb.App.Extensions;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Services;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc;
using NPOI.XWPF.UserModel;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.UMSLegacy;

namespace ConfiguratorWeb.App.Controllers
{
   
   /* 
   Terms used in this document:
    - Key := (DictionaryKey, Module)
    - Translation := ((Key, Language), Value)
    - Key-Group := set of all Translations with a given Key.
   
   Note:
   (Key, Language) === (DictionaryKey, Module, Language) is the 
   Model.Key of Digistat.FrameworkStd.Model.Dictionary .
    
   Features:
    - Default: Show all Translations as a grid, all languages on a row.
    - Add a new Key: this is done by adding a new Translation for a Key that DOESN'T EXIST YET. 
    - Delete a Key-Group: delete all Translations with the given Key.
    - Update a Key-Group.
   */
   
   public class DictionaryController : DigistatWebControllerBase
   {
      
      #region Constructor

      //This is the access to the business-logic
      private readonly Configurator.Std.BL.Dictionary.IDictionaryManager dm;
      
      public DictionaryController(IDigistatConfiguration config, IMessageCenterService msgcenter,
         ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc,
         Configurator.Std.BL.Dictionary.IDictionaryManager DictionaryManager
         ) 
         : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         dm = DictionaryManager;
         
      }
      
      #endregion

      #region Actions: Retrieve data and show them on a Grid view
      
      
      /* Default: show all data in a grid. */
      public IActionResult Index()
      { 
         User currUsr = mobjSyncSvc.GetCurrentUser();
         ViewBag.SitePath = "General > System Configuration > Dictionary";
         ViewBag.UserCanEdit = dm.GetEditPermission(currUsr);
         ViewBag.UserCanAdd = dm.GetAddKeyPermission(currUsr);
         ViewBag.UserCanDel = dm.GetDeleteKeyPermission(currUsr);
         ViewBag.UserCanEditSys = dm.GetSystemEditPermission(currUsr);
                
         try {
            DataTable T = dm.GetAllTraslationsListedForKey();
            return View(T);
         } 
         catch (UserAuthorizationException) {
            return BadRequest("User is not authorized.");
         }
         catch {
            return BadRequest("Could not load data.");
         }
      }


      /* Used for Ajax call from IndexView. */
      public IActionResult ReadDictionaryGrid([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
             DataTable T = dm.GetAllTraslationsListedForKey();
            
            if (request.Filters.Count>0  )
            {
                foreach (FilterDescriptor filterDescriptor in request.Filters.ToFilterDescriptor())
                {
                    if (filterDescriptor.Member == "IsSystem")
                    {
                        filterDescriptor.MemberType = typeof(string);
                        filterDescriptor.Value = filterDescriptor.Value.ToString();
                    }
                }    
            }

            DataSourceResult data = T.ToDataSourceResult(request);
            return new JsonResult(data);
         }
         catch (UserAuthorizationException)
         {
            return StatusCode(StatusCodes.Status401Unauthorized);
         }
         catch (Exception e)
         { 
            mobjLogSvc.ErrorException(e, "Error on ReadDictionaryGrid");
            return  Json(new
            {
               Status = "error",
               Stack = e.StackTrace,
               Message = e.Message,
               errors = new List<string>() { e.Message },
               errorMessage = "Error on ReadDictionaryGrid", success = false
            }
            );
            
         }
      }

      #endregion

      #region Actions: Get UI to create a new Key
      public IActionResult GetViewForNewKey([DataSourceRequest] DataSourceRequest request) 
      {
         
         return PartialView("_NewDictionaryKey", 
            new DictionaryTranslateViewModel()
            {  DictionaryKey = "", 
               Module="",  
               Description = "", //"Insert here some optional info.",
               Value = "", 
               IsSystem = false,
               Language = string.Join(",", dm.Languages().ToArray()) // "ENGLISH,ITALIAN,DANISH,..."
            }
         );
      }

      #endregion

      #region Actions: Create, Update, Delete

      /* Create a new Key (a single Translation for a non existing Key). */
      public JsonResult AddKeyGroup(Digistat.FrameworkStd.Model.Dictionary item) 
      {
         bool bolSuccess = false;
         string strMessageError = "";
         try
         {


            //Force null Module to string.null.
            if (item.Module == null) item.Module = string.Empty;
            if (item.IsSystem == null) item.IsSystem = false;

            Digistat.FrameworkStd.Model.Dictionary newitem =  dm.AddKey(item);
            SendDictionarySync("ADD", item);
            bolSuccess = true;
         }
         catch (UserAuthorizationException)
         {
            strMessageError = mobjDicSvc.XLate("User have not the permission to insert new key");
         }
         catch (ArgumentException ae)
         {
            strMessageError = ae.Message;
         }
         catch (Exception ex)
         {
            bolSuccess = false;
            strMessageError = ex.Message;
         }

         return Json(new { errorMessage = strMessageError, success = bolSuccess });
      }


      /* Delete a Key-Group: delete all Translations with the given Key. */
      public JsonResult DeleteKeyGroup([DataSourceRequest] DataSourceRequest request, 
         Digistat.FrameworkStd.Model.Dictionary translation) 
      {
         bool bolSuccess = false;
         string strMessageError = "";
         
         
         try
         {
            if (translation == null) {
               return Json(new { errorMessage = mobjDicSvc.XLate("Item not defined"), success = bolSuccess });
            }
            dm.DeleteKey(translation.DictionaryKey??"", translation.Module??"");
            SendDictionarySync("DELETE", translation);
            bolSuccess = true;
         }
         catch (UserAuthorizationException)
         {
            strMessageError = mobjDicSvc.XLate("User have not the permission to insert new key");
         }
         catch (ArgumentException ae)
         {
            strMessageError = ae.Message;
         }
         catch (Exception ex)
         {
            bolSuccess = false;
            strMessageError = ex.Message;
         }

         return Json(new { errorMessage = strMessageError, success = bolSuccess });
      }


      /* Update a Key-Group */
      public IActionResult UpdateKeyGroup([DataSourceRequest] DataSourceRequest request, 
         System.Collections.Generic.Dictionary<string, string> items) 
      {
         try {
            Configurator.Std.BL.Dictionary.TranslationsForKeyAndModule t = 
               DictionaryRowGridBuilder.GetTranslationsForKeyAndModule(items);
            dm.UpdateAllTraslationsListedForKey(t);
            Digistat.FrameworkStd.Model.Dictionary objTmp = new Dictionary()
            {
               Module = t.Module,
               DictionaryKey = t.DictionaryKey
            };
            SendDictionarySync("UPDATE", objTmp);
            return StatusCode(StatusCodes.Status200OK);
         } 
         catch (UserAuthorizationException) { return StatusCode(StatusCodes.Status401Unauthorized); }
         catch (ArgumentOutOfRangeException) { return StatusCode(StatusCodes.Status400BadRequest); }
         catch (Exception) { throw; }
      }



      #endregion

      #region SendUMSMessages
      private MCMessage InitUMSMessage()
      {
         MCMessage objMessage = new MCMessage
         {
            // construct the message
            DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
            DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
         };
         objMessage.SourceApp = mobjConfiguration.ModuleName;
         objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
         objMessage.PacketType = UMSFrameworkParser.GetPacketTypeSync();
         return objMessage;
      }

      private void SendDictionarySync(string action, Digistat.FrameworkStd.Model.Dictionary objDict)
      {
         MCMessage objMessage = InitUMSMessage();
         switch (action.Trim().ToUpperInvariant())
         {
            case "ADD":
               objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncTranslationDictionaryKeyAdded();
               break;
            case "UPDATE":
               objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncTranslationDictionaryKeyUpdated();
               break;
            case "DELETE":
               objMessage.Message = UMSFrameworkParser.GetMessageTypeSyncTranslationDictionaryKeyDeleted();
               break;
         }

         objMessage.AddOption("KEY", objDict.DictionaryKey);
         objMessage.AddOption("MODULE", objDict.Module);
         mobjMessageCenterSvc.SendMessage(objMessage);
      }

      #endregion

   }

   [ApiController]
   [Route("api/[controller]/[action]")]
   public class DictionaryApiController : Controller
   {
      private ILoggerService mobjLogSvc;

      public DictionaryApiController(IDigistatConfiguration config, ISynchronizationService syncSvc,
         IDictionaryService dicSvc, ILoggerService logSvc)
      {
         mobjLogSvc = logSvc;
      }
      

   
   }
}
