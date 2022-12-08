using System.Collections.Generic;
using Configurator.Std.Models.StockManagement;
using Configurator.Std4Stock.Models.StockManagement;

namespace Configurator.Std.BL.StockManagement
{
    public interface IStockManagementManager
    {
        List<RoleInPosition> GetPositionsByRole(int id);
        void SaveRolePositions(RoleUpdateRequest request);
        List<RoleOperatingBlock> GetOperatingBlocksByRole(int roleId);
        void SaveRoleOperatingBlocks(RoleUpdateRequest request);
         List<RoleOperatingRoom> GetOperatingRoomsByRole(int roleId);
        void SaveRoleOperatingRooms(RoleUpdateRequest request);
        List<ResourceCabinet> GetAllCabinets();
        List<ResourceCabinet> GetAllCabinetsWithResources(string resourceName);
        void InsertStockRoom(StockRoomDto request);
        void InsertCabinetGroup(CabinetGroupDto request);
        void UpdateStockRoom(StockRoomDto request);
        void UpdateCabinetGroup(CabinetGroupDto request);
        void InsertCabinet(CabinetDto request);
        void UpdateCabinet(CabinetDto request);
        void InsertLocation(LocationDto request);
        void UpdateLocation(LocationDto request);
        void InsertPosition(PositionDto request);
        void UpdatePosition(PositionDto request);
        void InsertResource(string srId, string cpId);
        List<ResourceDto> GetAllResources();
        List<ResourceTreeListTemplate> GetResourcesForCabinetPositions(string resourceName);
        List<StockRoomDto> GetStockRooms();
        List<CabinetGroupDto> GetCabinetGroups(string stockRoomId);
        List<CabinetDto> GetCabinets(string cabinetGroupId);
        List<LocationDto> GetCabinetLocations(string cabinetId);
        List<PositionDto> GetCabinetPositions(string locationId);
        string checkResourcesAnomalies(string srId, string spGuid, string newPositionGuid);
        void MoveResource(string spGuid, string newPositionId);
        string checkResourcesAnomaliesForRemove(string srId, string spGuid);
        void RemoveResource(string spGuid);
        void RemovePosition(string cpGuid);
        void RemoveLocation(string slGuid);
        void RemoveCabinet(string scGuid);
        void RemoveCabinetGroup(string sgGuid);
        void RemoveStockRoom(string ssGuid);
         List<ResourceCabinet> GetResourcesForPosition(string cpGuid,string searchKey);
        List<ResourceCabinet> GetAllPositions(string resourceName);
        List<OperatingBlockMasterDto> GetOperatingBlockMasterDto();
        List<OperatingBlockDetailDto> GetOperatingBlockDetailDto(int IDLocation);
        ResourceCabinet checkExistingStockRoomresource(string stockRoomId, string resourceId);
    }
}
