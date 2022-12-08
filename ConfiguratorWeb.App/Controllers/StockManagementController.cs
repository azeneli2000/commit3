using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Configurator.Std.BL.ExportScheduler;
using Configurator.Std.BL.Hubs;
using Configurator.Std.BL.ReportMaster;
using Configurator.Std.BL.StockManagement;
using Configurator.Std.BL.Vitals;
using Configurator.Std.Models.StockManagement;
using Configurator.Std4Stock.Models.StockManagement;
using ConfiguratorWeb.App.EntityBuilders;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Export;
using Digistat.FrameworkWebExtensions.Controllers;
using FastReport;
using FastReport.Export.OoXML;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using static Kendo.Mvc.UI.UIPrimitives;

namespace ConfiguratorWeb.App.Controllers
{
    public class StockManagementController : DigistatWebControllerBase
    {
        private readonly IMessageCenterManager mobjMsgCtrMgr;

        private readonly IPermissionsService mobjPermSvc;
        private readonly IStockManagementManager mobjStockManagemetMgr;

        public StockManagementController(IDigistatConfiguration config, IMessageCenterService msgcenter,
           ISynchronizationService syncSvc, IDictionaryService dicSvc, IDnsCacherService dnssvc, ILoggerService logsvc,
           ISystemOptionsService sysOptSvc, IMessageCenterManager msgCtrMgr,
           IPermissionsService permSvc, IStockManagementManager stockExampleManager
        )
           : base(config, msgcenter, syncSvc, dicSvc, dnssvc, logsvc, sysOptSvc)
        {
            mobjMsgCtrMgr = msgCtrMgr;
            mobjPermSvc = permSvc;
            mobjStockManagemetMgr = stockExampleManager;
        }
        //TODO: check for appropriate permissions in every method
        public JsonResult ReadPositionsRole([DataSourceRequest] DataSourceRequest request, int id)
        {
            IEnumerable<RoleInPosition> objAllValidationGroups = mobjStockManagemetMgr.GetPositionsByRole(id);
            DataSourceResult data = objAllValidationGroups.ToDataSourceResult(request, model => model);
            return new JsonResult(data);
        }
        public JsonResult ReadOperatingBlocksRole([DataSourceRequest] DataSourceRequest request, int id)
        {
            IEnumerable<RoleOperatingBlock> objAllValidationGroups = mobjStockManagemetMgr.GetOperatingBlocksByRole(id);
            DataSourceResult data = objAllValidationGroups.ToDataSourceResult(request, model => model);
            return new JsonResult(data);
        }
        public JsonResult ReadOperatingRoomsRole([DataSourceRequest] DataSourceRequest request, int id)
        {
            IEnumerable<RoleOperatingRoom> objAllValidationGroups = mobjStockManagemetMgr.GetOperatingRoomsByRole(id);
            DataSourceResult data = objAllValidationGroups.ToDataSourceResult(request, model => model);
            return new JsonResult(data);
        }
        public JsonResult SaveCabinetRoles(RoleUpdateRequest model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.SaveRolePositions(model);
                    bolSuccess = true;
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
        public JsonResult SaveOperatingBlockRoles(RoleUpdateRequest model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.SaveRoleOperatingBlocks(model);
                    bolSuccess = true;
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
        public JsonResult SaveOperatingRoomRoles(RoleUpdateRequest model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.SaveRoleOperatingRooms(model);
                    bolSuccess = true;
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




        public IActionResult Details()
        {
            ViewBag.SitePath = "Stock Management > Details";
            //ViewBag.treeData = ReadResourceCabinets();
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStockManagementDetailView,
                   CurrentUser))
            {
                return View("Details");
            }
            else
            {
                return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
        }

        public static List<ResourceTreeListTemplate> ResourceTreeListData = new();
        public JsonResult ReadResources(string position)
        {
            var result = ResourceTreeListData.Where(x => x.ParentId == position).ToList();
            return new JsonResult(result);
        }
        public JsonResult ReadResourcesForCabinetPositions(string srName)
        {
            var result = mobjStockManagemetMgr.GetResourcesForCabinetPositions(srName);
            var groupedResult = result.GroupBy(x => x.ParentId);
            List<CabinetPositionResourcesDto> resourcesInPositions = new List<CabinetPositionResourcesDto>();
            foreach (var position in groupedResult)
            {
                string positionId = position.Key;
                List<ResourceTreeListTemplate> resourceList = new List<ResourceTreeListTemplate>();
                foreach (var resource in position)
                {
                    resourceList.Add(resource);
                }
                resourcesInPositions.Add(
                    new CabinetPositionResourcesDto()
                    {
                        PositionId = positionId,
                        ResourceTreeListTemplates = resourceList
                    });
            }
            return new JsonResult(result);
        }
        // public JsonResult ReadResourceCabinets([DataSourceRequest] DataSourceRequest request)
        //public JsonResult ReadResourceCabinets(string id)
        public JsonResult ReadResourceCabinets([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<ResourceCabinet> objAllValidationGroups = mobjStockManagemetMgr.GetAllCabinets();
            List<TreeListTemplate> treeListData = new List<TreeListTemplate>();
            ResourceTreeListData.Clear();
            treeListData.Add(
                   new TreeListTemplate
                   {
                       Id = "root",
                       Name = "Block",
                       ParentId = null,
                       Type = "Root",
                       // HasChildren = true,

                   });
            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                ssName = x.StockRoomName,
                ssShortName = x.StockRoomShortName,
                ssId = x.StockRoomId,
                ssDescription = x.StockRoomDescription,
                ssIndex = x.StockRoomIndex,
                ssIsFromUnknown = x.StockRoomIsForUnknown
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.ssId,
                        Name = item.ssShortName,
                        StockRoomShortName = item.ssShortName,
                        StockRoomName = item.ssName,
                        StockRoomDescription = item.ssDescription,
                        StockRoomIndex = item.ssIndex,
                        StockRoomIsForUnknown = item.ssIsFromUnknown,
                        StockRoomId = item.ssId,
                        ParentId = "root",
                        Type = "StockRoom",
                        //HasChildren = true,

                    });
            }
            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                ssId = x.StockRoomId,
                cgShortName = x.CabinetGroupShortName,
                cgIndex = x.CabinetGroupIndex,
                cgName = x.CabinetGroupName,
                cgId = x.CabinetGroupId,
                cgDescription = x.CabinetGroupDescription
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.cgId,
                        Name = item.cgShortName,
                        CabinetGroupId = item.cgId,
                        CabinetGroupName = item.cgName,
                        CabinetGroupDescription = item.cgDescription,
                        CabinetGroupShortName = item.cgShortName,
                        CabinetGroupIndex = item.cgIndex,
                        ParentId = item.ssId,
                        Type = "CabinetGroup",
                        // HasChildren = true,

                    });
            }

            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                cgId = x.CabinetGroupId,
                cShortName = x.CabinetShortName,
                cName = x.CabinetName,
                cDescription = x.CabinetDescription,
                cIndex = x.CabinetIndex,
                cIsBaseket = x.CabinetIsBasket,
                cIsTrolley = x.CabinetIsTrolley,
                cIsGenericKit = x.CabinetIsGenericKit,
                cIsForNewPosition = x.CabinetIsForNewPosition,
                cId = x.CabinetId
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.cId,
                        Name = item.cShortName,
                        ParentId = item.cgId,
                        CabinetId = item.cId,
                        CabinetShortName = item.cShortName,
                        CabinetName = item.cName,
                        CabinetDescription = item.cDescription,
                        CabinetIndex = item.cIndex,
                        CabinetIsForNewPosition = item.cIsForNewPosition,
                        CabinetIsGenericKit = item.cIsGenericKit,
                        CabinetIsTrolley = item.cIsTrolley,
                        CabinetIsBasket = item.cIsBaseket,
                        Type = "Cabinet",
                        // HasChildren = true,

                    });
            }

            foreach (var item in objAllValidationGroups.Select(x =>
           new
           {
               cId = x.CabinetId,
               lId = x.LocationId,
               lShortName = x.LocationShortName,
               lName = x.LocationName,
               lDescription = x.LocationDescription,
               lNumPositions = x.LocationPositionNumber
           }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.lId,
                        Name = item.lShortName,
                        LocationId = item.lId,
                        LocationName = item.lName,
                        LocationDescription = item.lDescription,
                        LocationShortName = item.lShortName,
                        LocationPositionNumber = item.lNumPositions,
                        ParentId = item.cId,
                        Type = "Location",
                        // HasChildren = true,

                    });
            }


            foreach (var item in objAllValidationGroups.Select(x =>
         new
         {
             lId = x.LocationId,
             pId = x.PositionId,
             pShortName = x.PositionShortName,
             pName = x.PositionName,
             pDescription = x.PositionDescription
         }).Distinct().OrderBy(x => x.pShortName))
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.pId,
                        Name = item.pShortName,
                        PositionId = item.pId,
                        PositionName = item.pName,
                        PositionDescription = item.pDescription,
                        PositionShortName = item.pShortName,
                        ParentId = item.lId,
                        Type = "Position",
                        // HasChildren = true,
                    });
            }


            foreach (var item in objAllValidationGroups.Select(x =>
         new
         {
             pId = x.PositionId,
             rId = x.ResourceId,
             rCode = x.ResourceCode,
             rShortName = x.ResourceShortName,
             rName = x.ResourceName,
             rDescription = x.ResourceDescription,
             rMinQty = x.MinQuantity,
             rAlarmQty = x.AlarmQuantity,
             rIdealQty = x.IdealQuantity,
             rSpGuid = x.SpGuid
         }).Distinct())
            {
                ResourceTreeListData.Add(
                    new ResourceTreeListTemplate
                    {
                        Id = item.rId,
                        Name = item.rCode + item.rName,
                        ResourceId = item.rId,
                        ResourceName = item.rName,
                        ResourceDescription = item.rDescription,
                        ResourceShortName = item.rShortName,
                        ResourceCode = item.rCode,
                        MinQuantity = item.rMinQty,
                        AlarmQuantity = item.rAlarmQty,
                        IdealQuantity = item.rIdealQty,
                        ParentId = item.pId,
                        SpGuid = item.rSpGuid,
                        Type = "Resource",
                    });
            }
            //var data = treeListData.ToDataSourceResult(request, item => new TreeListTemplate()
            //{
            //    Id = item.Id,
            //    Name = item.Name,
            //    ParentId =  item?.ParentId,
            //});

            var data = treeListData.ToTreeDataSourceResult(request, e => e.Id, e => e.ParentId, e => e);

            //return new JsonResult(treeListData.Where(x => string.IsNullOrEmpty(id) ? x.ParentId == id : x.ParentId == null).Select(item => new {
            //    id = item.Id,
            //    Name = item.Name,
            //    expanded = true,               
            //    hasChildren = item.HasChildren
            //}));
            return new JsonResult(data);
        }
        public JsonResult ReadResourceCabinetsWithResources([DataSourceRequest] DataSourceRequest request, string resourceName = "ref")
        {
            var timer = new Stopwatch();
            timer.Start();
            IEnumerable<ResourceCabinet> objAllValidationGroups = mobjStockManagemetMgr.GetAllCabinetsWithResources(resourceName);
            List<TreeListTemplate> treeListData = new List<TreeListTemplate>();
            ResourceTreeListData.Clear();
            treeListData.Add(
                   new TreeListTemplate
                   {
                       Id = "root",
                       Name = "Block",
                       ParentId = null,
                       Type = "Root"
                   });
            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                ssName = x.StockRoomName,
                ssShortName = x.StockRoomShortName,
                ssId = x.StockRoomId,
                ssDescription = x.StockRoomDescription,
                ssIndex = x.StockRoomIndex,
                ssIsFromUnknown = x.StockRoomIsForUnknown
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.ssId,
                        Name = item.ssShortName,
                        StockRoomShortName = item.ssShortName,
                        StockRoomName = item.ssName,
                        StockRoomDescription = item.ssDescription,
                        StockRoomIndex = item.ssIndex,
                        StockRoomIsForUnknown = item.ssIsFromUnknown,
                        StockRoomId = item.ssId,
                        ParentId = "root",
                        Type = "StockRoom"
                    });
            }
            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                ssId = x.StockRoomId,
                cgShortName = x.CabinetGroupShortName,
                cgName = x.CabinetGroupName,
                cgId = x.CabinetGroupId,
                cgDescription = x.CabinetGroupDescription
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.cgId,
                        Name = item.cgShortName,
                        CabinetGroupId = item.cgId,
                        CabinetGroupName = item.cgName,
                        CabinetGroupDescription = item.cgDescription,
                        CabinetGroupShortName = item.cgShortName,
                        ParentId = item.ssId,
                        Type = "CabinetGroup"
                    });
            }

            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                cgId = x.CabinetGroupId,
                cShortName = x.CabinetShortName,
                cName = x.CabinetName,
                cDescription = x.CabinetDescription,
                cIndex = x.CabinetIndex,
                cIsBaseket = x.CabinetIsBasket,
                cIsTrolley = x.CabinetIsTrolley,
                cIsGenericKit = x.CabinetIsGenericKit,
                cIsForNewPosition = x.CabinetIsForNewPosition,
                cId = x.CabinetId
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.cId,
                        Name = item.cShortName,
                        ParentId = item.cgId,
                        CabinetId = item.cId,
                        CabinetShortName = item.cShortName,
                        CabinetName = item.cName,
                        CabinetDescription = item.cDescription,
                        CabinetIndex = item.cIndex,
                        CabinetIsForNewPosition = item.cIsForNewPosition,
                        CabinetIsGenericKit = item.cIsGenericKit,
                        CabinetIsTrolley = item.cIsTrolley,
                        CabinetIsBasket = item.cIsBaseket,
                        Type = "Cabinet"
                    });
            }

            foreach (var item in objAllValidationGroups.Select(x =>
           new
           {
               cId = x.CabinetId,
               lId = x.LocationId,
               lShortName = x.LocationShortName,
               lName = x.LocationName,
               lDescription = x.LocationDescription,
               lNumPositions = x.LocationPositionNumber
           }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.lId,
                        Name = item.lShortName,
                        LocationId = item.lId,
                        LocationName = item.lName,
                        LocationDescription = item.lDescription,
                        LocationShortName = item.lShortName,
                        LocationPositionNumber = item.lNumPositions,
                        ParentId = item.cId,
                        Type = "Location",
                    });
            }


            foreach (var item in objAllValidationGroups.Select(x =>
         new
         {
             lId = x.LocationId,
             pId = x.PositionId,
             pShortName = x.PositionShortName,
             pName = x.PositionName,
             pDescription = x.PositionDescription
         }).Distinct().OrderBy(x => x.pShortName))
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.pId,
                        Name = item.pShortName,
                        PositionId = item.pId,
                        PositionName = item.pName,
                        PositionDescription = item.pDescription,
                        PositionShortName = item.pShortName,
                        ParentId = item.lId,
                        Type = "Position",
                    });
            }


            foreach (var item in objAllValidationGroups.Select(x =>
         new
         {
             pId = x.PositionId,
             rId = x.ResourceId,
             rCode = x.ResourceCode,
             rShortName = x.ResourceShortName,
             rName = x.ResourceName,
             rDescription = x.ResourceDescription,
             rMinQty = x.MinQuantity,
             rAlarmQty = x.AlarmQuantity,
             rIdealQty = x.IdealQuantity
         }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id = item.rId,
                        Name = item.rCode + item.rName,
                        ResourceId = item.rId,
                        ResourceName = item.rName,
                        ResourceDescription = item.rDescription,
                        ResourceShortName = item.rShortName,
                        ResourceCode = item.rCode,
                        MinQuantity = item.rMinQty,
                        AlarmQuantity = item.rAlarmQty,
                        IdealQuantity = item.rIdealQty,
                        ParentId = item.pId,
                        Type = "Resource",
                    });
            }
            var data1 = treeListData;
            var data = treeListData.ToTreeDataSourceResult(request, e => e.Id, e => e.ParentId, e => e);
            timer.Stop();
            var s = timer.Elapsed;
            var r = new JsonResult(objAllValidationGroups);
            string json = JsonSerializer.Serialize(r);
            System.IO.File.WriteAllText(@"C:\test\path.json", json);
            //await using FileStream createStream = File.Create(@"D:\path.json");
            //await JsonSerializer.SerializeAsync(createStream, r);

            return r;
        }

        public static string searchKey = null;
        public void setSearchKey(string key)
        {
            searchKey = key;
        }
        public JsonResult ReadAllPositions([DataSourceRequest] DataSourceRequest request, string resourceName = null)
        {
            searchKey = resourceName;
            IEnumerable<ResourceCabinet> objAllValidationGroups = mobjStockManagemetMgr.GetAllPositions(resourceName)
                .OrderBy(c => c.StockRoomShortName)
                .ThenBy(n => n.CabinetGroupShortName)
                .ThenBy(x => x.CabinetShortName)
                .ThenBy(x => x.LocationShortName)
                .ThenBy(x => x.PositionShortName);
            return new JsonResult(objAllValidationGroups.ToDataSourceResult(request));
        }
        public JsonResult ReadResourcesForPosition(string cpGuid, [DataSourceRequest] DataSourceRequest request)
        {
            string s = searchKey;
            IEnumerable<ResourceCabinet> objAllValidationGroups = mobjStockManagemetMgr.GetResourcesForPosition(cpGuid, s).OrderBy(x => x.ResourceName);
            return new JsonResult(objAllValidationGroups.ToDataSourceResult(request));
        }

        public JsonResult ReadResourceCabinetsWithResourcesForGrid([DataSourceRequest] DataSourceRequest request, string resourceName = "ref")
        {
            var timer = new Stopwatch();
            timer.Start();
            IEnumerable<ResourceCabinet> objAllValidationGroups = mobjStockManagemetMgr.GetAllCabinetsWithResources(resourceName);
            List<TreeListTemplate> treeListData = new List<TreeListTemplate>();
            ResourceTreeListData.Clear();
            treeListData.Add(
                   new TreeListTemplate
                   {
                       Id="root",
                       Name= "Block",
                       ParentId = null,
                       Type = "Root"
                   });
            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                ssName = x.StockRoomName,
                ssShortName = x.StockRoomShortName,
                ssId = x.StockRoomId,
                ssDescription = x.StockRoomDescription,
                ssIndex = x.StockRoomIndex,
                ssIsFromUnknown = x.StockRoomIsForUnknown
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id=item.ssId,
                        Name= item.ssShortName,
                        StockRoomShortName = item.ssShortName,
                        StockRoomName = item.ssName,
                        StockRoomDescription = item.ssDescription,
                        StockRoomIndex = item.ssIndex,
                        StockRoomIsForUnknown = item.ssIsFromUnknown,
                        StockRoomId = item.ssId,
                        ParentId = "root",
                        Type = "StockRoom"
                    });
            }
            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                ssId = x.StockRoomId,
                cgShortName = x.CabinetGroupShortName,
                cgName = x.CabinetGroupName,
                cgId = x.CabinetGroupId,
                cgDescription = x.CabinetGroupDescription
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id=item.cgId,
                        Name= item.cgShortName,
                        CabinetGroupId = item.cgId,
                        CabinetGroupName = item.cgName,
                        CabinetGroupDescription = item.cgDescription,
                        CabinetGroupShortName = item.cgShortName,
                        ParentId = item.ssId,
                        Type = "CabinetGroup"
                    });
            }

            foreach (var item in objAllValidationGroups.Select(x =>
            new
            {
                cgId = x.CabinetGroupId,
                cShortName = x.CabinetShortName,
                cName = x.CabinetName,
                cDescription = x.CabinetDescription,
                cIndex = x.CabinetIndex,
                cIsBaseket = x.CabinetIsBasket,
                cIsTrolley = x.CabinetIsTrolley,
                cIsGenericKit = x.CabinetIsGenericKit,
                cIsForNewPosition = x.CabinetIsForNewPosition,
                cId = x.CabinetId
            }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id=item.cId,
                        Name= item.cShortName,
                        ParentId = item.cgId,
                        CabinetId = item.cId,
                        CabinetShortName = item.cShortName,
                        CabinetName = item.cName,
                        CabinetDescription = item.cDescription,
                        CabinetIndex = item.cIndex,
                        CabinetIsForNewPosition= item.cIsForNewPosition,
                        CabinetIsGenericKit = item.cIsGenericKit,
                        CabinetIsTrolley= item.cIsTrolley,
                        CabinetIsBasket = item.cIsBaseket,
                        Type = "Cabinet"
                    });
            }

            foreach (var item in objAllValidationGroups.Select(x =>
           new
           {
               cId = x.CabinetId,
               lId = x.LocationId,
               lShortName = x.LocationShortName,
               lName = x.LocationName,
               lDescription = x.LocationDescription,
               lNumPositions = x.LocationPositionNumber
           }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id=item.lId,
                        Name= item.lShortName,
                        LocationId = item.lId,
                        LocationName = item.lName,
                        LocationDescription = item.lDescription,
                        LocationShortName = item.lShortName,
                        LocationPositionNumber = item.lNumPositions,
                        ParentId = item.cId,
                        Type = "Location",
                    });
            }


            foreach (var item in objAllValidationGroups.Select(x =>
         new
         {
             lId = x.LocationId,
             pId = x.PositionId,
             pShortName = x.PositionShortName,
             pName = x.PositionName,
             pDescription = x.PositionDescription
         }).Distinct().OrderBy(x => x.pShortName))
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id=item.pId,
                        Name= item.pShortName,
                        PositionId = item.pId,
                        PositionName = item.pName,
                        PositionDescription = item.pDescription,
                        PositionShortName = item.pShortName,
                        ParentId = item.lId,
                        Type = "Position",
                    });
            }


            foreach (var item in objAllValidationGroups.Select(x =>
         new
         {
             pId = x.PositionId,
             rId = x.ResourceId,
             rCode = x.ResourceCode,
             rShortName = x.ResourceShortName,
             rName = x.ResourceName,
             rDescription = x.ResourceDescription,
             rMinQty = x.MinQuantity,
             rAlarmQty = x.AlarmQuantity,
             rIdealQty = x.IdealQuantity
         }).Distinct())
            {
                treeListData.Add(
                    new TreeListTemplate
                    {
                        Id=item.rId,
                        Name= item.rCode + item.rName,
                        ResourceId = item.rId,
                        ResourceName = item.rName,
                        ResourceDescription = item.rDescription,
                        ResourceShortName = item.rShortName,
                        ResourceCode = item.rCode,
                        MinQuantity = item.rMinQty,
                        AlarmQuantity = item.rAlarmQty,
                        IdealQuantity= item.rIdealQty,
                        ParentId = item.pId,
                        Type = "Resource",
                    });
            }
            var data1 = treeListData;
            var data = treeListData.ToTreeDataSourceResult(request, e => e.Id, e => e.ParentId, e => e);
            timer.Stop();
            var s = timer.Elapsed;
            var r = new JsonResult(objAllValidationGroups);
            string json = JsonSerializer.Serialize(r);
            System.IO.File.WriteAllText(@"C:\test\path.json", json);
            //await using FileStream createStream = File.Create(@"D:\path.json");
            //await JsonSerializer.SerializeAsync(createStream, r);

            return r;
        }




        public JsonResult InsertNewStockRoom(StockRoomDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.InsertStockRoom(model);
                    bolSuccess = true;
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
        public JsonResult UpdateStockRoom(StockRoomDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.UpdateStockRoom(model);
                    bolSuccess = true;
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
        public JsonResult InsertNewCabinetGroup(CabinetGroupDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.InsertCabinetGroup(model);
                    bolSuccess = true;
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
        public JsonResult UpdateCabinetGroup(CabinetGroupDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.UpdateCabinetGroup(model);
                    bolSuccess = true;
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
        public JsonResult InsertNewCabinet(CabinetDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.InsertCabinet(model);
                    bolSuccess = true;
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
        public JsonResult UpdateCabinet(CabinetDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.UpdateCabinet(model);
                    bolSuccess = true;
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
        public JsonResult InsertNewLocation(LocationDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.InsertLocation(model);
                    bolSuccess = true;
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
        public JsonResult UpdateLocation(LocationDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.UpdateLocation(model);
                    bolSuccess = true;
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
        public JsonResult InsertNewPosition(PositionDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.InsertPosition(model);
                    bolSuccess = true;
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
        public JsonResult UpdatePosition(PositionDto model)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.UpdatePosition(model);
                    bolSuccess = true;
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
        public JsonResult GetAllResources(DataSourceRequest request)
        {
            IEnumerable<ResourceDto> objAllResources = mobjStockManagemetMgr.GetAllResources();
            DataSourceResult data = objAllResources.ToDataSourceResult(request, model => model);
            return new JsonResult(data);
        }
        public PartialViewResult ResourceView()
        {
            return PartialView("Resources");
        }
        public PartialViewResult StockView()
        {
            return PartialView("Stock");
        }
        public JsonResult AddResource(string srId, string cpId)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.InsertResource(srId, cpId);
                    bolSuccess = true;
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
        //stocks
        public JsonResult GetStockRooms()
        {
            var result = mobjStockManagemetMgr.GetStockRooms();
            return Json(result);
        }
        public JsonResult GetCabinetGroups(string stockRoomId)
        {
            var result = mobjStockManagemetMgr.GetCabinetGroups(stockRoomId);
            return Json(result);
        }
        public JsonResult GetCabinets(string cabinetGroupId)
        {
            var result = mobjStockManagemetMgr.GetCabinets(cabinetGroupId);
            return Json(result);
        }
        public JsonResult GetLocations(string cabinetId)
        {
            var result = mobjStockManagemetMgr.GetCabinetLocations(cabinetId);
            return Json(result);
        }
        public JsonResult GetPositions(string locationId)
        {
            var result = mobjStockManagemetMgr.GetCabinetPositions(locationId);
            return Json(result);
        }
        public string getResourceAnomalies(string srId, string spGuid, string newPositionGuid)
        {
            string result = mobjStockManagemetMgr.checkResourcesAnomalies(srId, spGuid, newPositionGuid);
            int i = 0;
            string temp;
            do
            {
                temp = result.Substring(i, 1);
                if (temp == "0")
                {
                    i++;
                    continue;
                }
                else
                    break;
            }
            while (i <= 3);
            //TODO : translate err msg
            switch (i)
            {
                case 0:
                    return "ONE OR MORE GENERIC KIT HAS BEEN FOUND.";
                case 1:
                    return "THE CURRENT CABINET POSITION AND THE NEW ARE THE SAME";
                case 2:
                    return "THE RESOURCE HAS BEEN USED IN ONE OR MORE VALID OPERATION KIT.";
                case 3:
                    return (temp == "1") ? "THE RESOURCE HAS BEEN USED IN ONE OR MORE OPERATION KIT IN VALIDATION OR PREPARATION STATE." : "";
                default: return "";

            }
        }
        public string getResourceAnomaliesForRemove(string srId, string spGuid)
        {
            string result = mobjStockManagemetMgr.checkResourcesAnomaliesForRemove(srId, spGuid);
            int i = 0;
            string temp;
            do
            {
                temp = result.Substring(i, 1);
                if (temp == "0")
                {
                    i++;
                    continue;
                }
                else
                    break;
            }
            while (i < 3);
            //TODO : translate err msg
            switch (i)
            {
                case 0:
                    return "ONE OR MORE GENERIC KIT HAS BEEN FOUND.";
                case 1:
                    return "THE RESOURCE HAS BEEN USED IN ONE OR MORE VALID OPERATION KIT.";
                case 2:
                    return (temp == "1") ? "THE RESOURCE HAS BEEN USED IN ONE OR MORE OPERATION KIT IN VALIDATION OR PREPARATION STATE." : "";
                default: return "";

            }
        }
        public JsonResult checkExistingResourceInStockroom(string stockRoomId, string resourceId)
        {
            bool boolExists = false;
            string messageError = string.Empty;
            try
            {
                ResourceCabinet result = mobjStockManagemetMgr.checkExistingStockRoomresource(stockRoomId, resourceId);
                if (result != null) boolExists  = true;
                return Json(new { errorMessage = messageError, success = boolExists });
            }
            catch (Exception ex)
            {
                return Json(new { errorMessage = ex.Message, success = false });
            }
        }
        public JsonResult MoveResource(string spGuid, string newPositionId)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.MoveResource(spGuid, newPositionId);
                    bolSuccess = true;
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
        public JsonResult RemoveResource(string spGuid)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.RemoveResource(spGuid);
                    bolSuccess = true;
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
        public JsonResult RemovePosition(string cpGuid)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.RemovePosition(cpGuid);
                    bolSuccess = true;
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
        public JsonResult RemoveLocation(string slGuid)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.RemoveLocation(slGuid);
                    bolSuccess = true;
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
        public JsonResult RemoveCabinet(string scGuid)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.RemoveCabinet(scGuid);
                    bolSuccess = true;
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
        public JsonResult RemoveCabinetGroup(string sgGuid)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.RemoveCabinetGroup(sgGuid);
                    bolSuccess = true;
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
        public JsonResult RemoveStockRoom(string ssGuid)
        {
            bool bolSuccess = false;
            string messageError = string.Empty;
            try
            {
                if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionRolesEdit, CurrentUser))
                {
                    mobjPermSvc.ClearCache();
                    mobjStockManagemetMgr.RemoveStockRoom(ssGuid);
                    bolSuccess = true;
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

        public IActionResult OperatingBlocks()
        {
            ViewBag.SitePath = "Stock Management > Operating Blocks";
            //ViewBag.treeData = ReadResourceCabinets();
            if (mobjPermSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionStockManagementOperatingBlocksView,
                   CurrentUser))
            {
                return View("OperatingBlocks");
            }
            else
            {
                return Json(new { errorMessage = mobjDicSvc.XLate(CommonStrings.NO_VALID_PERMISSION), success = false });
            }
        }

        public ActionResult GetOperatingBlockMaster([DataSourceRequest] DataSourceRequest request)
        {
            List<OperatingBlockMasterDto> list = mobjStockManagemetMgr.GetOperatingBlockMasterDto();
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult GetOperatingBlockDetail(int IDLocation, [DataSourceRequest] DataSourceRequest request)
        {
            List<OperatingBlockDetailDto> list = mobjStockManagemetMgr.GetOperatingBlockDetailDto(IDLocation);
            return Json(list.ToDataSourceResult(request));
        }
    }
}

