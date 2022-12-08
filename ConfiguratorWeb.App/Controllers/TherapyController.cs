using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkWebExtensions.Controllers;
using Microsoft.AspNetCore.Mvc;
//using ConfiguratorWeb.App.Models.Therapy;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Text.Json;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Model.Therapy;
using Microsoft.AspNetCore.Http;
using ConfiguratorWeb.App.Extensions;
using ConfiguratorWeb.App.Models.Therapy;
using ConfiguratorWeb.App.ViewModelBuilders;
using Digistat.FrameworkStd.Enums;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Digistat.FrameworkStd.Model;

namespace ConfiguratorWeb.App.Controllers
{
   public class TherapyController : DigistatWebControllerBase
   {
      public const string SANoDuplicateFields = "SANoDuplicateFields";
      public const string SADisableExternalFields = "SADisableExternalFields";
      public const string SADisableDuplicateFields = "SADisableDuplicateFields";

      public readonly ITherapyDataManager mobjTherapyDataManager;
      private readonly IDBSystemOptionsManager mobjSysOptManager;
      private readonly List<SystemOption> mobjTherapySysOptions;
      private readonly IPermissionsService mobjPermSvc;
      public TherapyController(IDigistatConfiguration config, IMessageCenterService msgcenter,
       ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc, ISystemOptionsService sysOptSvc, IDigistatEnvironmentService digEnvSvc,
       ITherapyDataManager therapyDataManager, IDBSystemOptionsManager sysOptManager, IPermissionsService permSvc)
       : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
      {
         mobjPermSvc = permSvc;
         mobjTherapyDataManager = therapyDataManager;
         mobjSysOptManager = sysOptManager;

         mobjTherapySysOptions = mobjSysOptManager.GetSystemOptions("THERAPY");
      }

      public IActionResult Index()
      {
         ViewBag.SitePath = "Therapy > Standard Actions";

         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionTherapyView, CurrentUser))
         {
            return View();
         } 
         else
         {
            return View("NotAuthorized");
         }
      }

      public IActionResult TherapyConfig()
      {
         ViewBag.SitePath = "Therapy > Standard Actions";
         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionTherapyView, CurrentUser))
         {
            return View("TherapyConfig");
         }
         else
         {
            return View("NotAuthorized");
         }
      }
      public IActionResult ProfilesConfig()
      {
         ViewBag.SitePath = "Therapy > Profiles";

         if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionTherapyView, CurrentUser))
         {
            return View("ProfilesConfig");
         }
         else
         {
            return View("NotAuthorized");
         }
      }
     
      [HttpPost]
      public IActionResult TherapyConfigEdit(TherapyItemModel updatedAction)
      {
         if (updatedAction != null)
         {

            updatedAction.QuantityDose.RebuildAllowedProperties();

            var objStandardAction = TherapyItemModelBuilder.ToStandardAction(updatedAction);
            List<StandardAssociationItem> componentList = null;

            if (updatedAction.ItemType != TherapyItemType.Action)
            {
               componentList = TherapyItemModelBuilder.ToMixtureComponentList(updatedAction.ComponentsFormVal).ToList();
            }
            if (updatedAction.Id >= 0)

            {

               if (!mobjTherapyDataManager.SaveStandardAction(objStandardAction, componentList, updatedAction.Link.Profiles, updatedAction.Link.Resources))
               {
                  return StatusCode(StatusCodes.Status500InternalServerError);
               }
            }
            //else
            //{
            //   bool exixstSamelocStd = mobjTherapyDataManager.CheckCopyLocation(objStandardAction.Name, objStandardAction.LocationRef);

            //   if (exixstSamelocStd)
            //   {
            //      if (!mobjTherapyDataManager.SaveStandardAction(objStandardAction, componentList, updatedAction.Link.Profiles, updatedAction.Link.Resources))
            //      {
            //         return StatusCode(StatusCodes.Status500InternalServerError, "There is already a Standard Action with same name and same Location");
            //      }
            //   }
            //   else
            //   {
            //      return StatusCode(StatusCodes.Status500InternalServerError);
            //   }
            //}
  

         }
 

         return PartialView("_StandardActionDetail", updatedAction);
      }

      [HttpPost]
      public IActionResult ProfileConfigEdit(Profiles updateProfile)
      {
         try
         {
             mobjTherapyDataManager.SaveProfileAssociations(updateProfile);
            return PartialView("ProfileDetails", updateProfile);
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();

      }

      public JsonResult DeleteAction(int ID)
      {
         try
         {
            //if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               mobjTherapyDataManager.Delete(ID);

               return Json(new { errorMessage = string.Empty, success = true });

            }
            //else
            //{
            //   return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            //}
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();
      }

     
      [HttpGet]
      public IActionResult CurrentStandardItem(int selectedItem)
      {
         //TODO: un minimo di check sulla presenza o meno dell'id nella lista
         //return PartialView("_StandardActionDetail", mobjTherapyDataManager.GetByID(selectedItem));
         try
         {

            SystemOption SANoDuplicateFields = mobjSysOptSvc.CheckAndCreateSystemOption("THERAPY", null, null, null, "SANoDuplicateFields", "ExternalKey", "List of standard action fields (with semicolon separator) to not duplicate on relative button pressure (ex. ExternalKey)", Digistat.FrameworkStd.Enums.OptionType.Text, 0, 0, 1, true);
            SystemOption SADisableExternalFields = mobjSysOptSvc.CheckAndCreateSystemOption("THERAPY", null, null, null, "SADisableExternalFields", "Name;Description", "List of standard action fields (with semicolon separator) to disable edit where ExternalKey is enhanced (ex. Name;Description;PlanningType)", Digistat.FrameworkStd.Enums.OptionType.Text, 0, 0, 1, true);
            SystemOption SADisableDuplicateFields = mobjSysOptSvc.CheckAndCreateSystemOption("THERAPY", null, null, null, "SADisableDuplicateFields", "ExternalKey", "List of standard action fields (with semicolon separator) that disable the duplicate button if enchanced (ex. ExternalKey).", Digistat.FrameworkStd.Enums.OptionType.Text, 0, 0, 1, true);


            ViewBag.SANoDuplicateFields = SANoDuplicateFields.Value;
            ViewBag.SADisableExternalFields = SADisableExternalFields.Value;
            ViewBag.SADisableDuplicateFields = SADisableDuplicateFields.Value;
            var objStandardAction = mobjTherapyDataManager.GetByID(selectedItem);
            TherapyItemModel objTherapyItem = ViewModelBuilders.TherapyItemModelBuilder.Build(objStandardAction);
            if (objTherapyItem.ItemType == TherapyItemType.Mixture || objTherapyItem.ItemType == TherapyItemType.Protocol)
            {
               //objTherapyItem.MixtureComponents = ViewModelBuilders.TherapyItemModelBuilder.BuildList(mobjTherapyDataManager.GetMixtureComponents(objTherapyItem.Id)).ToList();
               var mixtureComponents = ViewModelBuilders.TherapyItemModelBuilder.ToMixtureComponentListView(mobjTherapyDataManager.GetAssociationComponents(objTherapyItem.Id)).ToList();
               //var objSortedList = mixtureComponents.OrderBy(x => x.Index).ToList();
               objTherapyItem.ComponentsFormVal = mixtureComponents;
            }
            
            objTherapyItem.Link.Resources = mobjTherapyDataManager.GetAssociatedResources(objStandardAction.IDStandardAction);

            return PartialView("_StandardActionDetail", objTherapyItem);
         }
         catch (Exception e)
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      [HttpGet]
      public IActionResult CurrentProfile(int selectedItem)
      {

         try
         {
            var objStandardAction = mobjTherapyDataManager.GetProfileById(selectedItem);
            //if (objStandardAction.Location != null)
            //{
            //    objStandardAction.LocationName = objStandardAction.Location.LocationName;
            //}

            return PartialView("ProfileDetails", objStandardAction);
         }
         catch (Exception e)
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }


      [HttpGet]
      public IActionResult CreateStandardItem(int itemIdToDuplicate, string strType)
      {
         TherapyItemModel ret = new TherapyItemModel();

         if (Enum.TryParse(typeof(TherapyItemType), strType, out var enumValue))
         {
            ret.ItemType = (TherapyItemType)enumValue;
         }

         if (itemIdToDuplicate > 0)
         {
            var objClone = mobjTherapyDataManager.GetByID(itemIdToDuplicate);
            if (objClone != null)
            {
               ret.Id = 0;
               ret.ItemType = objClone.ActionType.HasValue? (TherapyItemType) objClone.ActionType : TherapyItemType.Invalid;
               ret.Name = objClone.Name;
               ret.Description = objClone.Description;
            }
            else
            {
               return StatusCode(StatusCodes.Status403Forbidden);
            }
         }
       

         return PartialView("_StandardActionDetail", ret);
      }


      public JsonResult ReadFilteredStandardActions([DataSourceRequest] DataSourceRequest request, int? LocationID)
      {
         try
         {
            //var queryFilters = new List<Kendo.Mvc.CompositeFilterDescriptor>();

            var resultset = mobjTherapyDataManager.GetAll(LocationID);
            if (request.Filters.Any())
            {

                foreach (var fdc in request.Filters.ToFilterDescriptor())
                {
                    if (fdc.Member == "ItemTypeString")
                    {
                        if (Enum.TryParse<TherapyItemType>(fdc.Value.ToString(), out TherapyItemType enuType) && enuType != TherapyItemType.Invalid)
                        {
                            fdc.Member = "ActionType";
                            fdc.Value = (int)enuType;
                            fdc.Operator = fdc.Operator;
                        }
                    }

                    else if (fdc.Member == "Classification.ClassCat")
                    {
                        fdc.Member = "ClassCat";
                    }
                    else if (fdc.Member == "Classification.Incomplete")
                    {
                        fdc.Member = "Incomplete";
                    }
                    else if (fdc.Member == "Classification.LocationName")
                    {
                        fdc.Member = "LocationName";
                    }
              
                }
              
            }

            resultset = (resultset.Where(request.Filters).Cast<StandardAction>()); //apply renamed filters.

            // if there are two or more standard actions with same name, action type and class cat, this code exclude the items with a different location
            // as the one provided with LocationID
            // this code is actually commented because the original Therapy configurator do not apply such kind of filtering
            //if (LocationID != -1)
            //{
            //   var distinctRes = new Dictionary<string, StandardAction>();

            //   foreach (var act in resultset)//filter the result to exclude clones of the same StandardAction without locationRef
            //   {
            //      string key = string.Format("{1}_{0}_{2}", act.Name.Replace(' ', '_'), act.ActionType.HasValue ? act.ActionType.Value : 0, act.ClassCat.HasValue ? act.ClassCat.Value : 0);
            //      if (!distinctRes.ContainsKey(key))
            //      {
            //         distinctRes.Add(key, act);
            //      }
            //      else
            //      {
            //         if (act.LocationRef == LocationID)
            //         {
            //            distinctRes[key] = act;
            //         }
            //      }
            //   }
            //   resultset = distinctRes.Values.AsQueryable<StandardAction>();
            //}

            if (request.Sorts.Any())
            {
               foreach (Kendo.Mvc.SortDescriptor sortDescriptor in request.Sorts)
               {
                  if (sortDescriptor.SortDirection == Kendo.Mvc.ListSortDirection.Ascending)
                  {
                     switch (sortDescriptor.Member)
                     {
                        case "Name":
                           resultset = resultset.OrderBy(sa => sa.Name);
                           break;
                        case "Description":
                           resultset = resultset.OrderBy(sa => sa.Description);
                           break;
                        case "ItemTypeString":
                           resultset = resultset.OrderBy(sa => sa.ActionType);
                           break;
                        case "Classification.ClassCat":
                           resultset = resultset.OrderBy(sa => sa.ClassCat);
                           break;
                        case "SearchLevel":
                           resultset = resultset.OrderBy(sa => sa.SearchLevel);
                           break;
                        case "Classification.Incomplete":
                           resultset = resultset.OrderBy(sa => sa.Incomplete);
                           break;
                        case "Classification.LocationName":
                           resultset = resultset.OrderBy(sa => sa.LocationName);
                           break;
                        case "ClassName":
                           resultset = resultset.OrderBy(sa => sa.ClassName);
                           break;
                     }
                  }
                  else
                  {
                     switch (sortDescriptor.Member)
                     {
                           case "Name":
                           resultset = resultset.OrderByDescending(sa => sa.Name);
                           break;
                        case "Description":
                           resultset = resultset.OrderByDescending(sa => sa.Description);
                           break;
                        case "ItemTypeString":
                           resultset = resultset.OrderByDescending(sa => sa.ActionType);
                           break;
                        case "Classification.ClassCat":
                           resultset = resultset.OrderByDescending(sa => sa.ClassCat);
                           break;
                        case "SearchLevel":
                           resultset = resultset.OrderByDescending(sa => sa.SearchLevel);
                           break;
                        case "Classification.Incomplete":
                           resultset = resultset.OrderByDescending(sa => sa.Incomplete);
                           break;
                        case "Classification.LocationName":
                           resultset = resultset.OrderByDescending(sa => sa.LocationName);
                           break;
                        case "ClassName":
                           resultset = resultset.OrderByDescending(sa => sa.ClassName);
                           break;
                     }
                  }
               }
            }

            decimal total = resultset.Count();

            if (total > request.PageSize)
            {
               int pages = (int)Math.Ceiling(total / (decimal)request.PageSize);
               if (request.Page > pages)
               {
                  request.Page = 1;
               }

               var start = (request.Page - 1) * request.PageSize;
               resultset = resultset.Skip(start).Take(request.PageSize);

            }
            

            List<TherapyItemModel> ret = TherapyItemModelBuilder.BuildList(resultset).ToList();

            var datares = new DataSourceResult() { Data = ret, Total = (int)total };

            return new JsonResult(datares);
         }
         catch (Exception exc)
         {
            return Json(new { errorMessage = exc.Message, success = false });
         }
      }


      public JsonResult ReadClassificationFilters([DataSourceRequest] DataSourceRequest request, int filterType)
      {
         try
         {
            Dictionary<int, string> objFilterOptions = null;
            string strFilter=string.Empty;
            var filters = new List<StringsPair>();
            int nameIntValuePosition = 0;
            if (filterType == 1)
            {
               strFilter = "C";
               objFilterOptions = GetOrderedSystemOptions("CustomFilter");
            }
            else if (filterType == 2)
            {
               strFilter = "D";
               objFilterOptions = GetOrderedSystemOptions("FastFilter");
            }
            else if (filterType == 3)
            {
               strFilter = "B";
               objFilterOptions = GetOrderedSystemOptions("ClassFilter");
               foreach (var sys in objFilterOptions)
               {
                  filters.Add(new StringsPair() { Name = sys.Key.ToString(), Description = sys.Value });
               }
               return Json(filters);
            }

            foreach (var sys in objFilterOptions)
            {
               filters.Add(new StringsPair() { Name = string.Format("{0}{1}", strFilter, sys.Key), Description = sys.Value });
            }
            DataSourceResult ret = filters.ToDataSourceResult(request);
            return new JsonResult(ret);
         }
         catch (Exception e)
         {
         }
         return Json(new { success = false });
      }

      private Dictionary<int, string> GetOrderedSystemOptions(string searchNamePattern)
      {
         var objFilterOptions = mobjTherapySysOptions.Where(p => p.Name.Contains(searchNamePattern)).ToList();
         int nameIntValuePosition = searchNamePattern.Length;
         Dictionary<int, string> systemOptionAssociated = new Dictionary<int, string>();
         foreach (var sys in objFilterOptions)
         {
            string value = sys.Value;
            int index = Int32.Parse(sys.Name.Substring(nameIntValuePosition));
            systemOptionAssociated.Add(index, value);
         }
         return systemOptionAssociated.OrderBy(x => x.Key).ToDictionary(x=>x.Key, x=> x.Value);
      }


      [HttpPost]
      public JsonResult VerifyComponentOfMixtureAndProtocol([DataSourceRequest] DataSourceRequest request,int ID, int? type)
      {
         try
         {
            List<AssociationStandardAction> ret = null;
            if (type == null)
            {
               var result = mobjTherapyDataManager.GetAssociationsByComponent(ID,null);
                ret = TherapyItemModelBuilder.ToMixtureComponentListView(result).ToList();

            }
            else
            {
               var result = mobjTherapyDataManager.GetAssociationsByComponent(ID, type.Value);
               ret = TherapyItemModelBuilder.ToMixtureComponentListView(result).ToList();
            }


            return new JsonResult(ret.ToDataSourceResult(request));

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();
      }


      [HttpPost]
      public JsonResult VerifyProfilesAssociationsFromLink([DataSourceRequest] DataSourceRequest request, int ID)
      {
         try
         {
            List<Profiles> ret = null;
            
            var result = mobjTherapyDataManager.GetProfileAssociationsByLink(ID);
            ret = result.ToList();
            


            return new JsonResult(ret.ToDataSourceResult(request));

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();
      }



      [HttpPost]
      public JsonResult VerifyProfilesAssociations([DataSourceRequest] DataSourceRequest request, int ID, int type)
      {
         try
         {
            List<AssociationStandardAction> ret = null;

            var result = mobjTherapyDataManager.GetProfileAssociationsById(ID, type);
            ret = TherapyItemModelBuilder.ToMixtureComponentListView(result).ToList();



            return new JsonResult(ret.ToDataSourceResult(request));

         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();
      }

      [HttpGet]
      public JsonResult LoadSchemaSystemOption(TherapyItemType ActionType)
      {
         try
         {
            //[TODO]:Remove this static string and use instead the related system option content
            //string strFakeSysOpt = @"<?xml version=""1.0"" encoding=""UTF-8""?><StdActionSchema><Schema name=""Schema1"" description=""INFUSIONE/BOLO 1 (Amount:g)"" mask=""2"" allowedMask=""2,9"" factor=""60""><schemaField name=""Volume"" enabled=""true"" unit=""mL""/><schemaField name=""Amount"" enabled=""true"" unit=""g""/><schemaField name=""Speed"" enabled=""true"" unit=""mL/h""/><schemaField name=""DrugSpeed"" enabled=""true"" unit=""g/Kg/min""/><schemaField name=""Duration"" enabled=""true"" unit=""h""/><schemaField name=""Concentration"" enabled=""false"" unit=""""/><schemaField name=""ProductConcentration"" enabled=""false"" unit=""""/></Schema><Schema name=""Schema2"" description=""INFUSIONE/BOLO 2 (Amount:g)"" mask=""2"" allowedMask=""2,9"" factor=""60""><schemaField name=""Volume"" enabled=""false"" unit=""mL""/><schemaField name=""Amount"" enabled=""false"" unit=""g""/><schemaField name=""Speed"" enabled=""false"" unit=""mL/h""/><schemaField name=""DrugSpeed"" enabled=""true"" unit=""g/Kg/min""/><schemaField name=""Duration"" enabled=""false"" unit=""h""/><schemaField name=""Concentration"" enabled=""false"" unit=""""/><schemaField name=""ProductConcentration"" enabled=""false"" unit=""""/></Schema><Schema name=""Schema3"" description=""INFUSIONE/BOLO 3 (Amount:g)"" mask=""2"" allowedMask=""2,9"" factor=""60""><schemaField name=""Volume"" enabled=""true"" unit=""mL""/><schemaField name=""Amount"" enabled=""false"" unit=""g""/><schemaField name=""Speed"" enabled=""true"" unit=""mL/h""/><schemaField name=""DrugSpeed"" enabled=""false"" unit=""g/Kg/min""/><schemaField name=""Duration"" enabled=""false"" unit=""h""/><schemaField name=""Concentration"" enabled=""false"" unit=""""/><schemaField name=""ProductConcentration"" enabled=""false"" unit=""""/></Schema></StdActionSchema>";

            if (!mobjSysOptSvc.CheckIfSystemOptionApplicationIsLoaded("THERAPY"))
            {
               mobjSysOptSvc.ReloadSystemOptions("THERAPY");
            }
            var schemaOpt = mobjSysOptSvc.GetSystemOption("THERAPY", null, null, null, "AvailablePrescriptionSchema", false);

            if (schemaOpt != null && !string.IsNullOrEmpty(schemaOpt.Value))
            {
               System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(StdActionSchema));
               StdActionSchema result;
               using (TextReader reader = new StringReader(schemaOpt.Value))
               {
                  result = (StdActionSchema)serializer.Deserialize(reader);
               }

               if (ActionType == TherapyItemType.Action)
               {
                  result.Schema = result.Schema.Where(a => a.Mask != -1 && a.Mask != -2 && a.Mask != -3).ToList();
               }
               else if (ActionType == TherapyItemType.Mixture)
               {
                  result.Schema = result.Schema.Where(a => a.Mask == -1 || a.Mask == -2 || a.Mask == -3).ToList();
               }

               return Json(new { data = result, success = true });
            }
         }
         catch (Exception e)
         {
         }
         return Json(new {success = false});
      }


      public JsonResult ReadProfiles([DataSourceRequest] DataSourceRequest request)
      {
         try
         {
            foreach (var fdc in request.Filters.ToFilterDescriptor())
            {
               if (fdc.Member == "LocationName")
               {
                  fdc.Member = "Location.LocationName";
               }
            }


            if (request.Sorts.Any())
            {
               foreach (Kendo.Mvc.SortDescriptor sortDescriptor in request.Sorts)
               {
                  if (sortDescriptor.Member.Equals("LocationName"))
                  {
                     sortDescriptor.Member = "Location.LocationName";
                  }
               }
            }

            var resultset = mobjTherapyDataManager.GetAllProfiles().AsQueryable();

            var datares = resultset.ToDataSourceResult(request);

            return new JsonResult(datares);
         }
         catch (Exception exc)
         {
            return Json(new { errorMessage = exc.Message, success = false });
         }
      }

      public JsonResult DeleteProfile(int ID)
      {
         try
         {
            //if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               mobjTherapyDataManager.DeleteProfile(ID);

               return Json(new { errorMessage = string.Empty, success = true });

            }
            //else
            //{
            //   return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            //}
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();
      }

      [HttpPost]
      public JsonResult CheckAsscoiatedLocations(string tempLoc, int[] associatedItems)
      {
         try
         {
            //if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionIntegrationTelligenceEdit, CurrentUser))
            {
               int? intLoc = 0;
               if(tempLoc=="")
               {
                  intLoc = null;
               }
               else if(tempLoc != null)
               {
                  intLoc = Int32.Parse(tempLoc);
               }
               else
               {
                  intLoc = null;
               }
               bool result = mobjTherapyDataManager.CheckAsscoiatedLocations(intLoc, associatedItems);

               return Json(new { errorMessage = string.Empty, success = true, data = result });

            }
            //else
            //{
            //   return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            //}
         }
         catch (Exception ex)
         {
            return Json(new { errorMessage = ex.Message, success = false });
         }
         throw new NotImplementedException();
      }

      [HttpGet]
      public IActionResult CreateProfile(int itemIdToDuplicate)
      {
         Profiles ret = new Profiles();
         ret.IdProfile = 0; 
         return PartialView("ProfileDetails", ret);
      }

      [HttpGet]
      public IActionResult ProfileReordering()
      {
         
         return PartialView("_ProfilesReordering");
      }

      
      [HttpPost]
      public IActionResult ReorderTherapyProfilesList(List<OrderedItem> reorderedItems)
      {
         try
         {
            if (reorderedItems.Count() > 0)
            {
               var data = new List<KeyValuePair<int, int>>();
               foreach (var item in reorderedItems)
               {
                  data.Add(new KeyValuePair<int, int>(item.ItemId, item.Order));
               }
               mobjTherapyDataManager.ReorderProfiles(data);
            }
            return PartialView("_ProfilesReordering");
         }
         catch (Exception e)
         {
            return StatusCode(StatusCodes.Status403Forbidden);
         }
      }

      public IActionResult ReadStandardResourcesSelection([DataSourceRequest] DataSourceRequest request)
      {
            try
            {
                var resources = mobjTherapyDataManager.GetResources();
                
                resources = resources.Where(request.Filters).Cast<StandardResource>();
                var datares = resources.ToDataSourceResult(request);

                return new JsonResult(datares);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
      }

        [HttpGet]
        public IActionResult StandardResourcesSelectionView()
        {
            try
            {
                return PartialView("_StandardResourcesSelection");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }
    }
}
