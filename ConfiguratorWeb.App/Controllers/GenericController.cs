using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Enums;

using Configurator.Std.BL;
using Configurator.Std.BL.DasDrivers;
using Configurator.Std.Helpers;

using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.UMSLegacy;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.EntityFrameworkCore;

namespace ConfiguratorWeb.App.Controllers
{
   public class GenericController : Controller
   {
      private readonly IDictionaryService mobjDictionaryService;
      private readonly IDnsCacherService mobjDNSSvc;

      private readonly IDriverRepositoriesManager mobjDrvRepManager;
      private readonly IActualDevicesManager mobjActualDeviceManager;

      private readonly IStandardParametersManager mobjParMgr;
      private readonly IStandardUnitsManager mobjUnitMgr;
      private readonly IStandardDeviceTypesManager mobjStandardDeviceTypesManager;

      public GenericController(IDictionaryService dictionaryService,
         IDriverRepositoriesManager driverRepositoryManager,
         IActualDevicesManager actualDeviceManager, IDnsCacherService dnsSvc
         , IStandardParametersManager parMgr
         , IStandardUnitsManager unitMgr
         , IStandardDeviceTypesManager standardDeviceTypesManager)
      {
         mobjDictionaryService = dictionaryService;
         mobjDNSSvc = dnsSvc;

         mobjDrvRepManager = driverRepositoryManager;
         mobjActualDeviceManager = actualDeviceManager;  
         
         mobjParMgr = parMgr;
         mobjUnitMgr = unitMgr;
         mobjStandardDeviceTypesManager = standardDeviceTypesManager;
      }

      public IActionResult GetNetworkTypeList()
      {
         List<SelectListItem> objNetworkTypes = UtilityHelper.EnumToListSelectItem<NetworkTypeEnum>();
         return Json(objNetworkTypes);
      }

      public IActionResult GetCollectTypeList()
      {
         List<SelectListItem> objCollectTypes = UMSFrameworkParser.GetOutputTypeList().Select(c => new SelectListItem { Text = c.Value, Value = c.Key.ToString() }).ToList();
         return Json(objCollectTypes);
      }

      public ActionResult GetSporadicList()
      {
         List<SelectListItem> objSporadic = new List<SelectListItem>();
         objSporadic = UtilityHelper.EnumToStringList<Enums.DriverSporadic>(false, Enums.DriverSporadic.ONLINE, true);
         return Json(objSporadic.Select(a=> new SporadicViewModel { SporadicName = a.Text, SporadicId = Convert.ToInt32(a.Value)}));
      }


      

      public ActionResult GetDriverEventClassList()
      {
         //List<SelectListItem> objClass = new List<SelectListItem>();

         //objClass = UtilityHelper.EnumToStringList<Digistat.FrameworkStd.Enums.AlarmClass>(false, Digistat.FrameworkStd.Enums.AlarmClass.Other, true);

         //List<DriverEventClassViewModel> objClassList = objClass.Select(a => new DriverEventClassViewModel { ClassName = a.Text, ClassId = Convert.ToInt32(a.Value) }).ToList();
         //objClassList.Insert(0, new DriverEventClassViewModel()
         //{
         //   ClassId = -1,
         //   ClassName = string.Empty
         //});


         //return Json(objClass.Select(a => new DriverEventClassViewModel { ClassName = a.Text, ClassId = Convert.ToInt32(a.Value) }));


         var objClassList = UMSFrameworkParser.GetAlarmClassList();
         objClassList.Add(-1, string.Empty);

         return Json(objClassList.OrderBy(x => x.Key).Select(a => new DriverEventClassViewModel { ClassName = a.Value, ClassId = Convert.ToInt32(a.Key) }));
      }

      public ActionResult GetDriverEventLevelList()
      {
         //List<SelectListItem> objLevels = new List<SelectListItem>();

         //objLevels = UtilityHelper.EnumToStringList<Digistat.FrameworkStd.Enums.EventType > (false, Digistat.FrameworkStd.Enums.EventType.None,true);

         //List<DriverEventLevelViewModel> objEventList = objLevels.Select(a => new DriverEventLevelViewModel { LevelName = a.Text, LevelId = Convert.ToInt32(a.Value) }).ToList();
         //objEventList.Insert(0, new DriverEventLevelViewModel()
         //{
         //   LevelId = -1,
         //   LevelName = string.Empty
         //});

         //return Json(objEventList);


         var objLevels = UMSFrameworkParser.GetEventTypeList();
         objLevels.Add(-1, string.Empty);

         return Json(objLevels.OrderBy(x => x.Key).Select(a => new DriverEventLevelViewModel { LevelName = a.Value, LevelId = Convert.ToInt32(a.Key) }));


      }

      public ActionResult GetDeviceDriverNameList()
      {
         var repositories = mobjDrvRepManager.GetActives(x => new Models.General.DropdownModel
         {
            Value = x.Id,
            Text = x.DriverName + " " + x.DriverVersion
         });
         return Json(repositories);
      }

      public JsonResult GetHospitalUnitsTypes()
      {
         List<SelectListItem> enuList = UtilityHelper.EnumToStringList<HospitalUnitType>(false, HospitalUnitType.Both, true);
         IEnumerable<SelectListItem> objToRender = enuList.Select(c => new SelectListItem { Text = c.Text, Value = c.Value.ToString() });
         return new JsonResult(objToRender);
      }

      public ActionResult GetCDSSRuleDefaultValues()
      {
         object result=null;
         try
         {
            result = new
            {
               SupportedRuleTypes = DropdownValuesHelper.GetCdssRuleTypeList(mobjDictionaryService),
               errorMessage ="",
               success= true
            };
         }
         catch (Exception e)
         {
            result = new {errorMessage = e.Message,success= false};
         }
         return Json(result);
      }

      public ActionResult GetCDSSDataTypeDefaultValues()
      {
         object result=null;
         try
         {
            result = new
            {
               SupportedRuleDataTypes = DropdownValuesHelper.GetCdssDataTypeList(mobjDictionaryService),
               errorMessage ="",
               success= true
            };

         }
         catch (Exception e)
         {
            result = new {errorMessage = e.Message,success= false};
         }
         return Json(result);
      }

      public ActionResult GetDriverDefaultValues(string idRepository) {
         try
         {
            DriverRepository repository = mobjDrvRepManager.Get(idRepository);
            var configuration = repository.DefaultCommConfigurationObject;

            object result = new
            {
               SupportedDriverTypes =  DropdownValuesHelper.GetDriverTypeList(mobjDictionaryService, configuration.SupportedDriverTypes),
               SupportedConnectionTypes = DropdownValuesHelper.GetConnectionTypeList(mobjDictionaryService, configuration.SupportedCommConnectionTypes, configuration.ConnectionType),
               SupportedAlarmSystemType = DropdownValuesHelper.GeAlarmSystemTypeNameList(mobjDictionaryService, repository.AlarmSystemType),
               DefaultConnectionType = configuration.ConnectionType,
               DefaultHostname = configuration.Hostname,
               ReceivingDataMode = configuration.ReceivingDataMode,
               DefaultSocketPort = configuration.SocketPort,
               DefaultComPort = configuration.ComPort,
               DefaultBaud = configuration.Baud,
               DefaultDataBits = configuration.DataBits,
               DefaultParity = configuration.Parity,
               DefaultHandShake = configuration.HandShake,
               DefaultTCPCommType = configuration.TCPCommType,
               DefaultStopBits = configuration.StopBits,
               DefaultSmartCableId = configuration.SmartCableId,
               DefaultCustomParameters = configuration.CustomParam.Select(x => new CustomParametersViewModel {
                  ID = 0,
                  Name = x.Key,
                  Description = x.Value.Value,                  
                  Value= x.Value.Key,
               })
               //DefaultUSBProducerId = configuration.USBProducerId,
               //DefaultUSBVendorId = configuration.USBVendorId,
               //DefaultUSBSerialId = configuration.USBSerialId,               
               //DefaultRtsEnabled = configuration.RtsEnabled,
               //DefaultDtrEnabled = configuration.DtrEnabled,
            };

            return Json(result);
         }
         catch 
         {

            throw;
         }
 
      }

      public ActionResult GetBitsPerSecondList()
      {

         List<SelectListItem> objList = Enum.GetValues(typeof(BitPerSecond)).Cast<int>()
            .Select(x => new SelectListItem
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(((BitPerSecond)x).GetDisplayAttribute(), StringParseMethod.Html)
            })
            .ToList();
         
         return Json(objList);
      }

      public ActionResult GetDataBitsList()
      {

         List<SelectListItem> objList = ValuesDescriptionsHelper.DataBitsValues
            .Select(x => new SelectListItem
            {
               Value = x.ToString(),
               Text = x.ToString()
            })
            .ToList();

         return Json(objList);
      }


      public ActionResult GetHandshakeList()
      {
         List<SelectListItem> objList = Enum.GetValues(typeof(System.IO.Ports.Handshake)).Cast<int>()
            .Select(x => new SelectListItem
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(ValuesDescriptionsHelper.GetHandshakeDescription((System.IO.Ports.Handshake)x), StringParseMethod.Html)
            })
            .ToList();

         return Json(objList);
      }

      public ActionResult GetParityList()
      {
         List<SelectListItem> objList = Enum.GetValues(typeof(System.IO.Ports.Parity)).Cast<int>()
             .Select(x => new SelectListItem
             {
                Value = x.ToString(),
                Text = mobjDictionaryService.XLate(ValuesDescriptionsHelper.GetParityDescription((System.IO.Ports.Parity)x), StringParseMethod.Html)
             })
             .ToList();

         return Json(objList);
      }

      public ActionResult GetStopBitsList()
      {
         List<SelectListItem> objList = Enum.GetValues(typeof(System.IO.Ports.StopBits)).Cast<int>()
            .Where(w=>w>0)
            .Select(x => new SelectListItem {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(ValuesDescriptionsHelper.GetStopBitsDescription((System.IO.Ports.StopBits)x), StringParseMethod.Html)
            })
            .ToList();

         return Json(objList);
      }


      public ActionResult GetDataModelList()
      {

         //List<SelectListItem> objList = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDataModes()
         //.Select(x => new SelectListItem
         //{
         //   Value = x.ToString(),
         //   Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDataModeDescription(x))
         //})
         //.ToList();

         //return Json(objList);

         return Json(DropdownValuesHelper.GetDataModelList(mobjDictionaryService));
      }

      public ActionResult GetSocketTypeList()
      {
         //List<SelectListItem> objList = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetSocketTypesValues()
         //   .Select(x => new SelectListItem
         //   {
         //      Value = x.ToString(),
         //      Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetSocketTypeDescription(x))
         //   })
         //   .ToList();

         //return Json(objList);

         return Json(DropdownValuesHelper.GetSocketTypeList(mobjDictionaryService));
      }

      public ActionResult GetDeviceTypeList()
      {
         return Json(DropdownValuesHelper.GetDeviceTypeList(mobjDictionaryService));
      }

      public ActionResult GetUsersLanguageList()
      {
         List<SelectListItem> objList = mobjDictionaryService.GetLanguages()
            .Select(a => new SelectListItem { Text = a, Value = a })
            .ToList();

         objList.Insert(0, new SelectListItem { Text = "", Value = "" });
         return Json(objList);
      }

      //public ActionResult Grouping_GetRoles(string text)
      //{
      //   var result = UtilityHelper.GetUserRoles().ToList(); //.Where(c => c.RoleName.Contains(text))
      //   return Json(result);
      //}

      public ActionResult GetCurrentWorkstationName()
      {
         string currentHost = mobjDNSSvc.ResolveIp(HttpContext.Connection.RemoteIpAddress.ToString());
         return Json(currentHost);

      }

      public ActionResult GetActualDeviceNamesListByType(int deviceType)
      {
         var model = mobjActualDeviceManager.GetByDeviceType(deviceType).Select(x => new { deviceType = deviceType, deviceName = x.Name }).Distinct().OrderBy(a => a.deviceName);//.GroupBy(x => new {x.deviceType ,x.deviceName}).OrderBy();
         return Json(model);
      }

      public ActionResult GetActualDeviceSerialsListByTypeAndName(int deviceType, string name)
      {
         var model = mobjActualDeviceManager.GetByDeviceTypeDeviceName(deviceType, name).Select(x => new { deviceType = deviceType, deviceName = name, deviceSerial = x.SerialNumber }).Distinct().OrderBy(a => a.deviceSerial);
         return Json(model);
      }


      public JsonResult GetStandardUnit([DataSourceRequest] DataSourceRequest request,string id)
      {
         List<StandardUnit> model = new List<StandardUnit>();
         if (!string.IsNullOrEmpty(id))
         {
            string[] vals = id.Split(";");
            model.AddRange( mobjUnitMgr.GetAll().Where(s=> vals.Contains(s.Id.ToString()) ).OrderBy(f=>f.Id).ToList()); 
            model.AddRange( mobjUnitMgr.GetAll(request.Page,request.PageSize).Where(s=> !vals.Contains(s.Id.ToString()) ).OrderBy(f=>f.Id).ToList());
         }
         else
         {
            model.AddRange( mobjUnitMgr.GetAll(request.Page,request.PageSize).OrderBy(f=>f.Id).ToList());
         }

         
         if (id.Contains(";"))
         {
            foreach (string element in id.Split(";"))
            {
               if (model.AsQueryable().FirstOrDefault(e=>e.Id.ToString() ==element || e.UCUMCaseSensitive == element)==null)
               {
                  StandardUnit NAelement = new StandardUnit();
                  NAelement.Id = int.TryParse(element,out int i)?i:-9999;
                  NAelement.Print = element;
                  NAelement.UCUMCaseSensitive = element;
                  model.Add(NAelement);
               }
            }
         }
         var lista = StandarUnitViewModelBuilder.BuildList(model.OrderBy(f=>f.Id)).ToDataSourceResult(request);
         
         return Json(lista);
      }
      
      public ActionResult EncryptString(string text)
      {
         var str = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.EncryptString(text,null);
         return Json(str);
      }
      public ActionResult EncryptStringWithKey(string text,string key)
      {
         var str = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.EncryptString(text,key);
         return Json(str);
      }

   }
}