using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Web.Helpers;
//using System.Web.Mvc;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Vitals;
using Digistat.FrameworkWebExtensions.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Configurator.Std.BL.Vitals;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Extensions;
using ConfiguratorWeb.App.Models.CDSS;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.CDSS;
using Digistat.FrameworkStd.UMSLegacy;
using Kendo.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;


namespace ConfiguratorWeb.App.Controllers
{
    public class CDSSController : DigistatWebControllerBase
   {
      private readonly IDigistatConfiguration mobjDigistatConfig;
      private readonly ISystemOptionsService mobjSystemOptionsService;
      private readonly IDigistatEnvironmentService mobjDigEnvironmentService;
      private readonly IPermissionsService mobjPermSvc;
      private readonly IDriverRepositoriesManager mobjRepositoriesManager;
      private readonly IStandardDeviceTypesManager mobjStandardDeviceTypesManager;
      private ILoggerService mobjLog;
      private ICDSSManager mobjCDSSmgr;
      private string taskCancelled = ""; 
      private string CDSS_ID = "driver00-CDSS-read-only-000000000000";

      private readonly IStandardParametersManager mobjParMgr;
      private readonly IStandardUnitsManager mobjUnitMgr;

      public CDSSController(IDigistatConfiguration config, IMessageCenterService msgcenter,
       ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc
         , IPermissionsService permSvc, ICDSSManager cdssMgr,  IDriverRepositoriesManager repManager
         , IStandardParametersManager parMgr,IStandardUnitsManager unitMgr
         , IStandardDeviceTypesManager standardDeviceTypesManager
         )
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjDigistatConfig = config;
         mobjSystemOptionsService = sysOptSvc;
         mobjLog = logsvc;
         mobjDigEnvironmentService = digEnvSvc;
         mobjPermSvc = permSvc;
         mobjCDSSmgr = cdssMgr;
         taskCancelled = mobjDicSvc.XLate("Server CDSS doesn't respond in time");
         mobjRepositoriesManager = repManager;
         mobjParMgr = parMgr;
         mobjUnitMgr = unitMgr;
         mobjStandardDeviceTypesManager = standardDeviceTypesManager;
      }

      // GET: Actions
      public ActionResult Index()
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDSSView, CurrentUser))
         {
            ViewBag.SitePath = "Modules > CDSS > Rules";
            return View();
         }
          else
         {
            return View("NotAuthorized");
         }
      }


      public ActionResult GetRuleOptionEdit( string options,int? typeOptions)
      {
         CdssRuleOptionViewModel crovmOpt = JsonConvert.DeserializeObject<CdssRuleOptionViewModel>(options);
         ViewData["typeOptions"] = typeOptions; 
         return PartialView("_RuleOptionEdit",crovmOpt);
      }

      private void ModifyFilters(IEnumerable<IFilterDescriptor> filters)
      {
         if (filters.Any())
         {
            foreach (var filter in filters)
            {
               var descriptor = filter as FilterDescriptor;
               if (descriptor != null)
               {
                  switch (descriptor.Member)
                  {
                     case  "IsGenericDescr":
                        descriptor.Member = "IsGeneric";
                        descriptor.Value = descriptor.Value.ToString().ToUpper() == "GENERAL" ? true : false;
                        break;
                     case  "RuleTypeDescr":
                        descriptor.Member = "RuleType";
                        descriptor.Value = descriptor.Value.ToString().ToUpper() == "DLL" ? 0 : 1;
                        break;
                     case  "TriggerTypeDescr":
                        descriptor.Member = "TriggerType";
                        descriptor.Value = descriptor.Value.ToString().ToUpper() == "PERIODIC" ? 0 : 
                                           descriptor.Value.ToString().ToUpper() == "SCHEDULED"? 1 : 
                                           descriptor.Value.ToString().ToUpper() == "MULTI"? 3 : 2;
                        
                        break;
                  }
                  
               }
               else if (filter is CompositeFilterDescriptor)
               {
                  ModifyFilters(((CompositeFilterDescriptor)filter).FilterDescriptors);
               }
            }
         }
      }
      public JsonResult GetAllCDSSRules([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDSSView, CurrentUser))
            {
               IEnumerable<Digistat.FrameworkStd.Model.CDSS.CDSSRule> objAllSystems = mobjCDSSmgr.GetAll(true);
               var sort = new Dictionary<string, string[]>()
               {
                  {"RuleTypeDescr", new []{"RuleType"}},
                  {"TriggerTypeDescr", new []{"TriggerType"}},
                  {"IsGenericDescr", new []{"IsGeneric"}},
               //   {"Version", new []{"Repository.DriverVersion"}}
               };

               ModifyFilters(request.Filters);
               DataSourceResult data = objAllSystems.ToDataSourceResult(request.SortAttributesMapping(sort), model => CDSSRuleViewModelBuilder.Build(model));
               return new JsonResult(data);
            }
            else
            {
               return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
         }
         catch (Exception e)
         {
            return Json(new { errorMessage = mobjDicSvc.XLate("ERROR:")+e.Message, success = false });
         }
         
      }


      public ActionResult GETCDSSRule(int id)
      {
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionCDSSView, CurrentUser))
         {
            CDSSRuleViewModel model = new CDSSRuleViewModel();
            if (id > 0)
            {
               Digistat.FrameworkStd.Model.CDSS.CDSSRule objItem = mobjCDSSmgr.Get(id);
               model = CDSSRuleViewModelBuilder.Build(objItem);
            }
            else
            {
               model.RuleType = (short)(id == -1 ? 0 : 1);
            }

            return PartialView("_RuleDetail", model);
         }
         else
         {
            return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
         }
      }

      public JsonResult SaveCDSSRule(CDSSRuleViewModel model, bool clearOld = false)
      {
         string messageError = string.Empty;
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRule = null;
         bool bolSuccess = false;
         try
         {
            if (buildOutParameters(model, out var jsonResult)) return jsonResult;
            if (model.RuleType == 0)
            {
                  //Check CDSS method
                  var res = mobjCDSSmgr.GetDllSettingsByMessageSync(model.DllRuleName, model.DllFile,HttpContext.Session.Id);

                  bolSuccess = (res?.success??false);
                  if (!bolSuccess)
                  {
                     //mobjCDSSmgr.Delete(objRule);
                     messageError = String.Join(";", res?.messagges.ToArray()??new string[]{"The server did not respond in time. Please try again "});
                     return Json(new { errorMessage = messageError, success = false });
                  }
                  //return Json(new { errorMessage = messageError, success = false });

            }
            //According to Riccardo P. & Stefano S. more then one rule could have same model.DllFile - model.DllRuleName
            //if (model.RuleType==0)
            //{
            //   var ruleId = mobjCDSSmgr.CheckDllRuleMethod(model.DllFile, model.DllRuleName,model.ID);
            //   if (ruleId != 0)
            //   {
            //      return Json(new { errorMessage = mobjDicSvc.XLate("the rule method is already used-> rule {0}").FormatWith(ruleId), success = false });

            //   }
            //}
            if (!string.IsNullOrEmpty(model.Code))
            {
               var res = mobjCDSSmgr.GetAll().FirstOrDefault(f=>f.Code==model.Code && f.Id != model.ID);
               
               if (res !=null)
               {
                  messageError = String.Join(";", new string[]{$"The Code \"{model.Code}\" is in use "});
                  return Json(new { errorMessage = messageError, success = false });
               }
            }

            CDSSRule cdssRule = CDSSRuleEntityBuilder.Build(model);
            var ROutP = cdssRule.RuleOutputParameters;
            var st = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            
            if (model.ID <= 0) //create
            {
               cdssRule.Id = 0;

               objRule = mobjCDSSmgr.Create(cdssRule);
               mobjLogSvc.Write(100, $"CDSS Rule [{model.ID}] Created:{st}", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);
            }
            else //update
            {
               
               objRule = mobjCDSSmgr.UpdateCustom(cdssRule,clearOld);
               mobjLogSvc.Write(100, $"CDSS Rule [{model.ID}] Updated:{st}", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);

            }

            bolSuccess = (objRule != null);
            

            if (bolSuccess )
            {
               ICollection<DriverRepositoryStandardParameterLink> cdssCapabilities = mobjCDSSmgr.GetAllCapabilities();
               
               
               if (ROutP.Count>0)
               {
                  foreach (CDSSRuleOutputParameter cdssRuleOutputParameter in ROutP)
                  {
                     
                     int standardParameterId = 0;
                     if (!int.TryParse(cdssRuleOutputParameter.ParameterName,out standardParameterId ))
                     {
                        var resultString = Regex.Match(cdssRuleOutputParameter.ParameterName, @"\d+").Value;
                        standardParameterId = int.Parse(resultString) ;
                     }

                     string deviceText = "?";
                     StandardParameter sp = mobjParMgr.GetQueryable().Where(x => x.Id ==standardParameterId ).FirstOrDefault();
                     if (sp!=null)
                     {
                        //standard parameter : isnull(par_print, par_description) - se = ? allora è null
                        deviceText = sp.Print.Trim().Length==0?sp.Description.Trim():sp.Print.Trim();
                     }
                     var existingCapabilities = cdssCapabilities.FirstOrDefault(x=>x.StandardParameterId == standardParameterId);
                     int iStandardUnitId = int.TryParse(cdssRuleOutputParameter.UM,out int umId)?umId:99999;
                     if (existingCapabilities!=null)
                     {

                        existingCapabilities.StandardUnitId = iStandardUnitId;
                        existingCapabilities.StandardDeviceTypeId = 99;
                        existingCapabilities.DeviceText = deviceText != "?" ? deviceText : null;
                        existingCapabilities.DeviceUnitText = "99999";
                        existingCapabilities.Sporadic = 1;
                        existingCapabilities.IsEnabled = true;
                        //existingCapabilities.MustBeSaved = false;
                        if (existingCapabilities.StandardDeviceType == null)
                        {
                           existingCapabilities.StandardDeviceType = mobjStandardDeviceTypesManager.Get(99);
                        }
                     }
                     else
                     {
                        
                        cdssCapabilities.Add(new DriverRepositoryStandardParameterLink
                        {
                           DriverRepositoryId = CDSS_ID,
                           StandardParameterId = standardParameterId,
                           StandardUnitId = iStandardUnitId, //leggi da standard parameter
                           StandardDeviceTypeId = 99,//always OTHER
                           StandardParameterIdAlias = "", 
                           DeviceId = standardParameterId.ToString(),
                           DeviceText = deviceText!="?"?deviceText:null,   
                           DeviceUnitText = "99999",
                           Sporadic = 1,
                           IsEnabled = true,
                           MustBeSaved = false,
                           StandardDeviceType = mobjStandardDeviceTypesManager.Get(99)

                        });
                     }
                  }
                  //SAVING Capabilities
                  mobjCDSSmgr.SaveCapabilitiesAndSendMessage(cdssCapabilities);
               }
               

               //mobjRepositoriesManager.Update(objDriver,false,true,false);
            }
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }


      }

      private bool buildOutParameters(CDSSRuleViewModel model, out JsonResult jsonResult)
      {
         jsonResult = Json(new
         {
            errorMessage = "",
            success = false
         });
         if (!string.IsNullOrEmpty(model.OutputParameters))
         {
            string[] op = model.OutputParameters.Split(";", StringSplitOptions.RemoveEmptyEntries);
            IQueryable<StandardParameter>
               standardParameters = mobjParMgr.GetQueryable(); //.Where(x => op.Contains(x.Id.ToString()));
            model.OutputParameters = "";
            foreach (string o in op)
            {
               StandardParameter sp = standardParameters.FirstOrDefault(x => x.Id.ToString() == o.Trim());
               if (sp != null)
               {
                  string unitOfMeasureId = sp.UOMIds.Split(";")[0];
                  if (unitOfMeasureId == "" || unitOfMeasureId == "NA")
                  {
                     unitOfMeasureId = "99999";
                  }

                  string typeOfParam = sp.DataType;
                  if (typeOfParam == "" || typeOfParam == "STRING")
                  {
                     typeOfParam = "ST";
                  }
                  else
                  {
                     typeOfParam = "NM";
                  }

                  model.OutputParameters += "{0},{1},{2};".FormatWith(sp.Id, typeOfParam, unitOfMeasureId);
               }
               else
               {
                  {
                     jsonResult = Json(new
                     {
                        errorMessage = mobjDicSvc
                           .XLate("the output parameter {0} is not present in Standard Parameters")
                           .FormatWith(o.Trim()),
                        success = false
                     });
                     return true;
                  }
               }
            }
         }

         return false;
      }

      public JsonResult DeleteCDSSRule(CDSSRuleViewModel model)
      {
         string messageError = string.Empty;
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRule = null;
         bool bolSuccess = false;
         try
         {
            if (model.ID == 0) //create
            {
               //objRule = mobjCDSSmgr.Create(CDSSRuleEntityBuilder.Build(model));
            }
            else //update
            {
               var st = Newtonsoft.Json.JsonConvert.SerializeObject(model);
               objRule = mobjCDSSmgr.Delete(CDSSRuleEntityBuilder.Build(model));
               mobjLogSvc.Write(100, $"CDSS Rule {model.ID} deleted:{st}", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);
            }

            bolSuccess = (objRule != null);
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }

      }
      
      //public JsonResult CompileCDSSRuleByDll(/*CDSSRuleViewModel model,string script*/
      //   int id,string name, short dataType, short triggerType, int killTimeout, int interval, /*List<TimeSpan>*/ string times,
      //   int validityTimeout, bool isExecuteAtStartup, bool isAutoActivate, string messageType, string methodCode)
      //{
      //   string messageError = string.Empty;
      //   CDSSRuleViewModel model  = new CDSSRuleViewModel()
      //   {
      //      ID = id,
      //      Name = name,
      //      IsGeneric = (dataType==0),
      //      TriggerType = triggerType,
      //      KillTimeout = killTimeout,
      //      Interval = interval,
      //      Times = times,
      //      ValidityTimeout = validityTimeout,
      //      ExecuteAtStartup = isExecuteAtStartup,
      //      AutoActivate = isAutoActivate,
      //      MessageType = messageType,
      //      MethodCode = methodCode

      //   };
      //   bool bolSuccess = false;
      //   try
      //   {
            
      //      var res = mobjCDSSmgr.Compile(CDSSRuleEntityBuilder.Build(model));
      //      bolSuccess = res.Item1;
      //      messageError = String.Join("\r\n", res.Item2.ToArray());

      //      //bolSuccess = (objRule != null);
      //      return Json(new { errorMessage = messageError, success = bolSuccess });
      //   }
      //   catch (Exception ex)
      //   {
      //      return Json(new { errorMessage = ex.Message, success = false });
      //   }
      //}
      public async Task<JsonResult> CompileCDSSRuleByMessage(
         int id,string name, short dataType, short triggerType, int killTimeout, int interval, /*List<TimeSpan>*/ string times,
         int validityTimeout, bool isExecuteAtStartup, bool isAutoActivate, string messageType, string methodCode,
         string outputParameters, string options
         
         )
      {
         string messageError = string.Empty;
         CDSSRuleViewModel model  = new CDSSRuleViewModel()
         {
            ID = 0,
            Name = name,
            IsGeneric = (dataType==0),
            TriggerType = triggerType,
            KillTimeout = killTimeout,
            Interval = interval,
            Times = times,
            ValidityTimeout = validityTimeout,
            ExecuteAtStartup = isExecuteAtStartup,
            AutoActivate = isAutoActivate,
            MessageType = messageType,
            MethodCode = methodCode,
            Options = options,
            OutputParameters = outputParameters,
            IsTest = true

         };
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRule = null;
         bool bolSuccess = false;
         try
         {
            //Step 1: Save Temp
            try
            {
               if (buildOutParameters(model, out var jsonResult)) return jsonResult;

               objRule = mobjCDSSmgr.Create(CDSSRuleEntityBuilder.Build(model));
               //model.IsTest = false;
               bolSuccess = (objRule != null);
               //return Json(new { errorMessage = messageError, success = bolSuccess });
            }
            catch (Exception ex)
            {
               
               return Json(new {errorMessage = ex.Message, success = false});
            }

            if (bolSuccess)
            {
               id = objRule.Id;
            }
            else
            {
               return Json(new {errorMessage = "CAN'T SAVE TEMP RULE", success = false});
            }

            //Step 2: Call CDSS
            var res = mobjCDSSmgr.CompileByMessageSync(id,HttpContext.Session.Id);
            bolSuccess = res.success;
            messageError = String.Join("\r\n", res.messagges.ToArray());

            //bolSuccess = (objRule != null);
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (TaskCanceledException ex)
         {
               
            return Json(new {errorMessage = taskCancelled, success = false});
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         finally
         {
            if (objRule!=null)
            {
               mobjCDSSmgr.Delete(objRule);
            }
         }
      }
      public async Task<JsonResult> RunTestCDSSRuleByMessage(
         int id,string name, short dataType, short triggerType, int killTimeout, int interval, /*List<TimeSpan>*/ string times,
         int validityTimeout, bool isExecuteAtStartup, bool isAutoActivate, string messageType, string methodCode,
         string outputParameters, string options,
         string inputValues,
         int ruletype,
         string dllname,
         string rulename,
         string locations
         )
      {
         string messageError = string.Empty;
         int originalId = id;
         CDSSRuleViewModel model  = new CDSSRuleViewModel()
         {
            ID = 0,
            Name = name,
            IsGeneric = (dataType==0),
            TriggerType = triggerType,
            KillTimeout = killTimeout,
            Interval = interval,
            Times = times,
            ValidityTimeout = validityTimeout,
            ExecuteAtStartup = isExecuteAtStartup,
            AutoActivate = isAutoActivate,
            MessageType = messageType,
            MethodCode = methodCode,
            Options = options,
            OutputParameters = outputParameters,
            RuleType = (short)ruletype,
            IsTest = true,
            DllFile = dllname,
            DllRuleName = rulename,
            Locations = locations

         };
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRule = null;
         bool bolSuccess = false;
         try
         {
            //Step 1: Save Temp
            try
            {
               if (buildOutParameters(model, out var jsonResult)) return jsonResult;

               CDSSRule _cdssRule = CDSSRuleEntityBuilder.Build(model);
               objRule = mobjCDSSmgr.Create(_cdssRule);
               //model.IsTest = false;
               bolSuccess = (objRule != null);
               //return Json(new { errorMessage = messageError, success = bolSuccess });
            }
            catch (Exception ex)
            {
               
               return Json(new {errorMessage = ex.Message, success = false});
            }

            if (bolSuccess)
            {
               id = objRule.Id;
            }
            else
            {
               return Json(new {errorMessage = "CAN'T SAVE TEMP RULE", success = false});
            }

            //Step 2: Call CDSS
            var res = mobjCDSSmgr.RunningTestByMessageSync(id,inputValues,HttpContext.Session.Id);
            bolSuccess = res.success;
            messageError = String.Join("\r\n", res.messagges.ToArray());
            if (bolSuccess)
            {
               string idRule = (originalId == 0) ? objRule.Name : originalId.ToString();

               var st = Newtonsoft.Json.JsonConvert.SerializeObject(objRule);
               mobjLogSvc.Write(100, $"CDSS Rule [{idRule}] tested.\r\nRULE DETAIL:\r\n{st}\r\nPARAMS:\r\n[{inputValues}]\r\nRESPONSE OUTPUT:\r\n[{messageError}]", EventLogEntryType.Information, CurrentUser.Abbrev,0,LogType.CLN,mobjConfiguration.ModuleName);

            }
            //bolSuccess = (objRule != null);
            return Json(new { errorMessage = messageError, success = bolSuccess });
         }
         catch (TaskCanceledException ex)
         {
               
            return Json(new {errorMessage = taskCancelled, success = false});
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         finally
         {
            if (objRule!=null)
            {
               //Disabled for TEST
               mobjCDSSmgr.Delete(objRule);
            }
         }
      }
      public async Task<JsonResult> GetOutputParametersByMessage(
         int id,string name, short dataType, short triggerType, int killTimeout, int interval, /*List<TimeSpan>*/ string times,
         int validityTimeout, bool isExecuteAtStartup, bool isAutoActivate, string messageType, string methodCode,
         string outputParameters,string dllName, string ruleMetodh
      )
      {
         CDSSRuleViewModel model  = new CDSSRuleViewModel()
         {
            ID = 0,
            Name = name,
            IsTest = true,
            IsGeneric = (dataType==0),
            TriggerType = triggerType,
            KillTimeout = killTimeout,
            Interval = interval,
            Times = times,
            ValidityTimeout = validityTimeout,
            ExecuteAtStartup = isExecuteAtStartup,
            AutoActivate = isAutoActivate,
            MessageType = messageType,
            OutputParameters = outputParameters,
            DllFile = dllName, 
            DllRuleName= ruleMetodh
         };
         string messageError = string.Empty;
         //int id = 0;
         bool bolSuccess = false;
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRule = null;
         try
         {
            if (string.IsNullOrEmpty( model.Name))
            {
               model.Name = "temp_" + DateTime.Now.Ticks.ToString();
            }
            //Step 1: Save Temp
            try
            {
               objRule = mobjCDSSmgr.Create(CDSSRuleEntityBuilder.Build(model));
               //model.IsTest = false;
               bolSuccess = (objRule != null);
               //return Json(new { errorMessage = messageError, success = bolSuccess });
            }
            catch (Exception ex)
            {
               //return Json(new {errorMessage = ex.Message, success = false});
               return Json(new {errorMessage = mobjDicSvc.XLate("CAN'T SAVE TEMP RULE"), success = false});
            }

            if (bolSuccess)
            {
               id = objRule.Id;
            }
            else
            {
               return Json(new {errorMessage = "CAN'T SAVE TEMP RULE", success = false});
            }

            //Step 2: Call CDSS
            var res =  mobjCDSSmgr.GetDllOutputParamsByMessageSync(ruleMetodh, dllName,HttpContext.Session.Id);
            //bolSuccess = res.Item1;
            messageError = String.Join(";", res.messagges.ToArray());

            bolSuccess = (res.success);
            
            return Json(new {errorMessage = messageError, success = bolSuccess});
         }
         catch (TaskCanceledException ex)
         {
               
            return Json(new {errorMessage = taskCancelled, success = false});
         }
         catch (Exception ex)
         {
            return Json(new {errorMessage = ex.Message, success = false});
         }
         finally
         {
            if (objRule!=null)
            {
               mobjCDSSmgr.Delete(objRule);
            }
         }
      }
      //public async Task<JsonResult> GetDllListByMessage()
      //{
      //   string messageError = string.Empty;
         
      //   try
      //   {
      //      var objRet =  await mobjCDSSmgr.GetDllListByMessageSync(HttpContext.Session.Id);
            
      //      return Json(new { errorMessage = messageError, success = true , fullResponse=objRet });
      //   }
      //   catch (TaskCanceledException ex)
      //   {
      //      Console.WriteLine("************ CONTROLLER.TaskCanceledException :{0}",DateTime.Now.ToString("HH:mm:ss.fff"));
      //      return Json(new {errorMessage = taskCancelled, success = false, taskCancelled = true});
      //   }
      //   catch (Exception ex)
      //   {
            
      //      Console.WriteLine("************ CONTROLLER.GetDllListByMessage :{0}",DateTime.Now.ToString("HH:mm:ss.fff"));
      //      return Json(new { errorMessage = ex.Message, success = false });
      //   }
      //}
      
      public List<SelectListItem> GetDllListItemsByMessage()
      {
         string messageError = string.Empty;
         
         try
         {
            var objRet =  mobjCDSSmgr.GetDllListByMessageSync(HttpContext.Session.Id);
            //List<SelectListItem> objToRender = objRet.Result.messagges.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            List<SelectListItem> objToRender = objRet.messagges.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            
            return objToRender;
         }
         catch
         {
            throw;
            //return new List<SelectListItem>();
         }
      }
      public IActionResult GetDllList(string selectedDll, string idField,string idModel)
      {
         ViewBag.selectedDASBroker = selectedDll;
         ViewBag.IdToSet = idField;
         ViewBag.idModel = idModel;
         return View("_CDSSDllList");
      }
      public JsonResult ReadCDSSDllList([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            DataSourceResult data = GetDllListItemsByMessage().ToDataSourceResult(request);
            //return Json(new {errorMessage = taskCancelled, success = false, taskCancelled = true});
            
            return new JsonResult(data);
         }
         catch (TaskCanceledException ex)
         {
            var errors = new DataSourceResult();
            errors.Errors = new {errorMessage = taskCancelled, success = false, taskCancelled = true};
            Console.WriteLine("---------- {0} TaskCanceledException",DateTime.Now.ToString("HH:mm:ss.fff"));
            return new JsonResult(errors);
         }
         catch (Exception e)
         {
            var errors = new DataSourceResult();
               errors.Errors = new {errorMessage = e.Message, success = false, taskCancelled = false};
               Console.WriteLine("---------- {0} ReadCDSSDllList generic",DateTime.Now.ToString("HH:mm:ss.fff"));
            return new JsonResult(errors);
         }
         
      }

      public IActionResult GetDllMethodList(string selectedDll, string idField,string idModel)
      {
         ViewBag.selectedDll = selectedDll;
         ViewBag.IdToSet = idField;
         ViewBag.idModel = idModel;
         return View("_CDSSDllMethodList");
      }

      public IActionResult ReadCdssDllMethodList([DataSourceRequest] DataSourceRequest request,string strDllFileName)
      {
         try
         {
            DataSourceResult data = GetDllMethodListItemsByMessage(strDllFileName).ToDataSourceResult(request);
            return new JsonResult(data);
         }
         catch (TaskCanceledException ex)
         {
            var errors = new DataSourceResult();
            errors.Errors = new {errorMessage = taskCancelled, success = false, taskCancelled = true};
            Console.WriteLine("---------- {0} TaskCanceledException",DateTime.Now.ToString("HH:mm:ss.fff"));
            return new JsonResult(errors);
         }
         catch (Exception e)
         {
            var errors = new DataSourceResult();
            errors.Errors = new {errorMessage = e.Message, success = false, taskCancelled = false};
            Console.WriteLine("---------- {0} ReadCdssDllMethodList generic",DateTime.Now.ToString("HH:mm:ss.fff"));
            return new JsonResult(errors);
         }
      }

      private List<SelectListItem>  GetDllMethodListItemsByMessage(string strDllFileName)
      {
         string messageError = string.Empty;
         
         try
         {
            var objRet =  mobjCDSSmgr.GetDllMethodListByMessageSync(strDllFileName,HttpContext.Session.Id);
            List<SelectListItem> objToRender = objRet.messagges.Select(c => new SelectListItem { Text = c, Value = c }).ToList();
            //return Json(new { errorMessage = messageError, success = true , fullResponse=objRet });
            return objToRender;
         }
         catch (Exception ex)
         {
            throw;
            //return new List<SelectListItem>();
         }
      }
      
      public JsonResult GetStandardParameters([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            //var result = HttpContext.Current.Session["Products"] as IList<ProductViewModel>;
            IQueryable<StandardParameter> objAllparameters = mobjParMgr.GetQueryable();
            
            DataSourceResult data = objAllparameters.ToDataSourceResult(request);
            return Json(data);
         }
         catch (Exception e)
         {
            return new JsonResult(new { error = true });
         }
         //var products = Enumerable.Range(0, 500).Select(i => new StandardParameter
         //{
         //   Id = i,
         //   Description = "Product " + i
         //});

         //return Json(products.ToDataSourceResult(request));
      }

      public async Task<JsonResult>  GetAllSettingsByMessage(
      int id,string name, short dataType, short triggerType, int killTimeout, int interval, /*List<TimeSpan>*/ string times,
         int validityTimeout, bool isExecuteAtStartup, bool isAutoActivate, string messageType, string methodCode,
         string outputParameters,string dllName, string ruleMetodh
      )
      {
         CDSSRuleViewModel model  = new CDSSRuleViewModel()
         {
            ID = 0,
            Name = name,
            IsTest = true,
            IsGeneric = (dataType==0),
            TriggerType = triggerType,
            KillTimeout = killTimeout,
            Interval = interval,
            Times = times,
            ValidityTimeout = validityTimeout,
            ExecuteAtStartup = isExecuteAtStartup,
            AutoActivate = isAutoActivate,
            MessageType = messageType,
            OutputParameters = outputParameters,
            DllFile = dllName, 
            DllRuleName= ruleMetodh
         };
         string messageError = string.Empty;
         //int id = 0;
         bool bolSuccess = false;
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRule = null;
         try
         {
            if (string.IsNullOrEmpty( model.Name))
            {
               model.Name = "temp_" + DateTime.Now.Ticks.ToString();
            }
            //Step 1: Save Temp
            try
            {
               objRule = mobjCDSSmgr.Create(CDSSRuleEntityBuilder.Build(model));
               //model.IsTest = false;
               bolSuccess = (objRule != null);
               //return Json(new { errorMessage = messageError, success = bolSuccess });
            }
            catch (Exception ex)
            {
               //return Json(new {errorMessage = ex.Message, success = false});
               return Json(new {errorMessage = mobjDicSvc.XLate("CAN'T SAVE TEMP RULE"), success = false});
            }

            if (bolSuccess)
            {
               id = objRule.Id;
            }
            else
            {
               return Json(new {errorMessage = "CAN'T SAVE TEMP RULE", success = false});
            }

            //Step 2: Call CDSS
            var res =  mobjCDSSmgr.GetDllSettingsByMessageSync(ruleMetodh, dllName,HttpContext.Session.Id);

            if (res != null)
            {
               messageError = String.Join(";", res.messagges.ToArray());
            
               bolSuccess = (res.success); 
            }
            else
            {
               messageError = mobjDicSvc.XLate("No answer from CDSS");
               bolSuccess = false;
            }
            
            
            return Json(new {errorMessage = messageError, success = bolSuccess});
         }
         catch (TaskCanceledException ex)
         {
            //var msgTest = "{\"IsTimeout\":true,\"Name\":\"EarlyWarningScoreRule\",\"IsGeneric\":false,\"Interval\":5000,\"KillTimeout\":10,\"ValidityTimeout\":60,\"IsExecuteAtStartup\":false,\"IsAutoActivate\":false,\"TriggerType\":0,\"Times\":[\"08:29:34\",\"17:19:34\"],\"MessageType\":null,\"OutputParams\":[\"20000\",\"20001\"],\"Data\":null,\"Options\":[],\"LogicLayer\":{}}";
            //return Json(new {errorMessage = msgTest, success = true});
            return Json(new {errorMessage = taskCancelled, success = false});
         }
         catch (Exception ex)
         {
            return Json(new {errorMessage = ex.Message, success = false});
         }
         finally
         {
            if (objRule!=null)
            {
               mobjCDSSmgr.Delete(objRule);
            }
         }
      }
   }
    
}