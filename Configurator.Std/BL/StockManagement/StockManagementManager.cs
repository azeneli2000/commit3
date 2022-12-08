using Configurator.Std.BL.Hubs;
using Configurator.Std.Enums;
using Configurator.Std.Exceptions;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Extensions;
//using NPOI.POIFS.Storage;
using System.Data.SqlClient;
using Configurator.Std.BL.Configurator;
using System.Threading.Tasks;
using Configurator.Std.Models.StockManagement;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Configurator.Std4Stock.Models.StockManagement;
using NPOI.SS.Formula.Functions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using NPOI.POIFS.Properties;
using Fizzler;
using UMS.Framework.Data.Types;

namespace Configurator.Std.BL.StockManagement
{
    public class StockManagementManager : IStockManagementManager
    {
        protected readonly IMessageCenterManager mobjMsgCtrMgr;
        protected readonly IConfiguratorWebConfiguration mobjConfig;
        protected readonly ILoggerService mobjLoggerService;
        protected string msConnectionString;

        public StockManagementManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrMgr
           , IConfiguratorWebConfiguration config)
        {
            msConnectionString = context.Database.GetConnectionString();
            mobjLoggerService = loggerService;
            mobjMsgCtrMgr = msgCtrMgr;
            mobjConfig = config;
        }

        public List<RoleInPosition> GetPositionsByRole(int roleId)
        {
            List<RoleInPosition> objPositions = new List<RoleInPosition>();
            try
            {
                const string strQuery = @"SELECT  ss_ShortName StockRoomName,
                                            sg_ShortName CabinetGroupName,
                                            sc_ShortName CabinetName,
                                            sc_GUID ScGuid,											
                                            CAST(case when (kln_rol_ID is null) then 0 else 1 end as BIT)  Allow,
		                                    CAST(ISNULL(s.sc_IsTrolley,0) as BIT) IsTrolley, 
											CAST(ISNULL(s.sc_IsBasket,0) as BIT) IsBasket, 
											CAST(ISNULL(s.sc_IsGenericKit,0) as BIT) IsGenericKit
                                          FROM 
                                            StandardStockrooms ss 
                                            INNER JOIN StandardCabinetGroups g on ss.ss_GUID = g.sg_ss_GUID
                                            INNER JOIN StandardCabinets s on g.sg_GUID = s.sc_sg_GUID
                                            LEFT JOIN RoleStandardCabinets R ON R.kln_sc_GUID = s.sc_GUID and  kln_rol_ID = @P_Id";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@P_Id", roleId);
                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objPositions.Add(new RoleInPosition()
                                {
                                    ScGuid = objReader["ScGuid"].ToString(),
                                    StockRoomName = objReader["StockRoomName"].ToString(),
                                    CabinetGroupName = objReader["CabinetGroupName"].ToString(),
                                    CabinetName = objReader["CabinetName"].ToString(),
                                    Allow = (bool)objReader["Allow"],
                                    IsTrolley = (bool)objReader["IsTrolley"],
                                    IsBasket = (bool)objReader["IsBasket"],
                                    IsGenericKit = (bool)objReader["IsGenericKit"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read RoleInPosition - GetPositionsByRole");
                string message = string.Format("Unable to read all PositionsByRole records from DB");
                throw new Exception(message, ex);
            }
            return objPositions;
        }
        public List<RoleOperatingBlock> GetOperatingBlocksByRole(int roleId)
        {
            List<RoleOperatingBlock> objOperatingBlocks = new List<RoleOperatingBlock>();
            try
            {
                const string strQuery = @"SELECT ob_Name OperatingBlockName,ob_Id OperatingBlockGuid,ob_description OperatingBlockDescription,										
                                            CAST(case when (kln_rol_ID is null) then 0 else 1 end as BIT)  Allow
										  FROM 
                                            OperatingBlocks O 											
                                            LEFT JOIN RoleOperatingBlocks R ON R.kln_ob_ID = O.ob_ID and  kln_rol_ID = @P_Id";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@P_Id", roleId);
                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objOperatingBlocks.Add(new RoleOperatingBlock()
                                {
                                    OperatingBlockGuid = objReader["OperatingBlockGuid"].ToString(),
                                    OperatingBlockName = objReader["OperatingBlockName"].ToString(),
                                    OperatingBlockDescription = objReader["OperatingBlockDescription"].ToString(),

                                    Allow = (bool)objReader["Allow"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read OperatingBlocks - GetOperatingBlocksByRole");
                string message = string.Format("Unable to read all OperatingBlocks records from DB");
                throw new Exception(message, ex);
            }
            return objOperatingBlocks;
        }
        public List<RoleOperatingRoom> GetOperatingRoomsByRole(int roleId)
        {
            List<RoleOperatingRoom> objOperatingRooms = new List<RoleOperatingRoom>();
            try
            {
                const string strQuery = @"SELECT or_Name OperatingRoomName,or_Id OperatingRoomGuid,OB.ob_Name OperatingBlockName,or_description OperatingRoomDescription,										
                                            CAST(case when (kln_rol_ID is null) then 0 else 1 end as BIT)  Allow
										  FROM 
                                            OperatingRooms O 	
											inner join OperatingBlocks OB on O.or_ob_ID = OB.ob_ID
                                            LEFT JOIN RoleOperatingRooms R ON R.kln_or_ID = O.or_ID and  kln_rol_ID =  @P_Id";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@P_Id", roleId);
                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objOperatingRooms.Add(new RoleOperatingRoom()
                                {
                                    OperatingRoomGuid = objReader["OperatingRoomGuid"].ToString(),
                                    OperatingRoomName = objReader["OperatingRoomName"].ToString(),
                                    OperatingRoomDescription = objReader["OperatingRoomDescription"].ToString(),
                                    OperatingBlockName = objReader["OperatingBlockName"].ToString(),
                                    Allow = (bool)objReader["Allow"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read OperatingRooms - GetOperatingRoomsByRole");
                string message = string.Format("Unable to read all OperatingRooms records from DB");
                throw new Exception(message, ex);
            }
            return objOperatingRooms;
        }
        public void SaveRolePositions(RoleUpdateRequest request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    var tran = objConn.BeginTransaction();
                    foreach (string item in request.insert)
                    {
                        SqlCommand insert = new SqlCommand("INSERT INTO dbo.RoleStandardCabinets (kln_rol_ID,kln_sc_GUID) VALUES (@RoleId,@ScGuid)", objConn);
                        insert.Parameters.AddWithValue("@RoleId", request.roleId);
                        insert.Parameters.AddWithValue("@ScGuid", item);
                        insert.Transaction = tran;
                        insert.ExecuteNonQuery();
                    }
                    foreach (string item in request.delete)
                    {
                        SqlCommand delete = new SqlCommand("DELETE FROM dbo.RoleStandardCabinets where kln_sc_GUID = @ScGuid AND kln_rol_ID = @RoleId", objConn);
                        delete.Parameters.AddWithValue("@RoleId", request.roleId);
                        delete.Parameters.AddWithValue("@ScGuid", item);
                        delete.Transaction = tran;
                        delete.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to update RoleInPosition - SaveRolePositions");
                string message = string.Format("Unable to delete/insert PositionsRole records from DB");
                throw new Exception(message, ex);
            }
        }
        public void SaveRoleOperatingBlocks(RoleUpdateRequest request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    var tran = objConn.BeginTransaction();
                    foreach (string item in request.insert)
                    {
                        SqlCommand insert = new SqlCommand("INSERT INTO dbo.RoleOperatingBlocks (kln_rol_ID,kln_ob_ID) VALUES (@RoleId,@ObGuid)", objConn);
                        insert.Parameters.AddWithValue("@RoleId", request.roleId);
                        insert.Parameters.AddWithValue("@ObGuid", item);
                        insert.Transaction = tran;
                        insert.ExecuteNonQuery();
                    }
                    foreach (string item in request.delete)
                    {
                        SqlCommand delete = new SqlCommand("DELETE FROM dbo.RoleOperatingBlocks where kln_ob_ID = @ObGuid AND kln_rol_ID = @RoleId", objConn);
                        delete.Parameters.AddWithValue("@RoleId", request.roleId);
                        delete.Parameters.AddWithValue("@ObGuid", item);
                        delete.Transaction = tran;
                        delete.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to update OperatingBlocks - SaveRoleOperatingBlocks");
                string message = string.Format("Unable to delete/insert OperatingBlocks records from DB");
                throw new Exception(message, ex);
            }
        }
        public void SaveRoleOperatingRooms(RoleUpdateRequest request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    var tran = objConn.BeginTransaction();
                    foreach (string item in request.insert)
                    {
                        SqlCommand insert = new SqlCommand("INSERT INTO dbo.RoleOperatingRooms (kln_rol_ID,kln_or_ID) VALUES (@RoleId,@OrGuid)", objConn);
                        insert.Parameters.AddWithValue("@RoleId", request.roleId);
                        insert.Parameters.AddWithValue("@OrGuid", item);
                        insert.Transaction = tran;
                        insert.ExecuteNonQuery();
                    }
                    foreach (string item in request.delete)
                    {
                        SqlCommand delete = new SqlCommand("DELETE FROM dbo.RoleOperatingRooms WHERE kln_or_ID = @OrGuid AND kln_rol_ID = @RoleId", objConn);
                        delete.Parameters.AddWithValue("@RoleId", request.roleId);
                        delete.Parameters.AddWithValue("@OrGuid", item);
                        delete.Transaction = tran;
                        delete.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to update OperatingRooms - SaveRoleOperatingRooms");
                string message = string.Format("Unable to delete/insert OperatingRooms records from DB");
                throw new Exception(message, ex);
            }
        }

        //Resources cabinets relations 
        public List<ResourceCabinet> GetAllPositions(string resourceName)
        {
            List<ResourceCabinet> objPositions = new List<ResourceCabinet>();
            try
            {
                const string strQuery = @"SELECT 
                                            ss_ShortName StockRoomShortName,
                                            ss_Name StockRoomName,
                                            ss_Description StockRoomDescription ,
                                            ss_Index StockRoomIndex,
                                            ss_IsForUnknown StockRoomIsForUnknown,
                                            ss_GUID StockRoomId,

                                            sg_ShortName CabinetGroupShortName,
                                            sg_Name CabinetGroupName,
                                            sg_GUID CabinetGroupId,
                                            sg_Description CabinetGroupDescription,
                                            sg_Index CabinetGroupIndex,

                                            sc_ShortName CabinetShortName,   
                                            sc_Name CabinetName,                    
                                            sc_Description CabinetDescription,  
                                            sc_Index CabinetIndex,
                                            sc_IsBasket CabinetIsBasket,
                                            sc_IsTrolley CabinetIsTrolley,
                                            sc_IsGenericKit CabinetIsGenericKit,
                                            sc_IsForNewPosition CabinetIsForNewPosition,
                                            sc_GUID CabinetId,
                                           
										    sl_ShortName LocationShortName,   
                                            sl_Name LocationName,                    
                                            sl_Description LocationDescription,  
										    sl_Index LocationIndex,
											sl_NumPositions LocationPositionNumber,
											sl_Guid LocationId,

											cp_ShortName PositionShortName,   
                                            cp_Name PositionName,                    
                                            cp_Description PositionDescription,  
										    cp_Index PositionIndex,
											cp_Guid PositionId,
											max(d.tot) RowNr

                                            FROM 
                                            StandardStockrooms ss 
                                            INNER JOIN StandardCabinetGroups g on ss.ss_GUID = g.sg_ss_GUID
                                            INNER JOIN StandardCabinets s on g.sg_GUID = s.sc_sg_GUID
                                            INNER JOIN StandardCabinetLocations l on l.sl_sc_GUID = s.sc_GUID   
											INNER JOIN StandardCabinetPosition p on p.cp_sl_GUID = l.sl_GUID   
                                            OUTER APPLY (SELECT DISTINCT COUNT(*) tot FROM  StandardPositionContent spc 
														  LEFT JOIN StandardResource s on spc.sp_sr_ID = s.sr_ID  
                                                          WHERE p.cp_GUID = spc.sp_cp_GUID 
                                                          AND  (@ResourceName is null OR sr_Name LIKE  '%' + @ResourceName + '%')) d

                                            -- LEFT JOIN StandardPositionContent spc on p.cp_GUID = spc.sp_cp_GUID
											--LEFT JOIN StandardResource sr on sr_ID = spc.sp_sr_ID
											--WHERe @ResourceName is null or sr_Name LIKE  '%' + @ResourceName + '%'	             
                                                         GROUP BY ss_Name,
                                                         ss_ShortName,
                                                         ss_GUID, 
                                                         sg_ShortName,
                                                         sg_GUID,
                                                         sc_ShortName,
                                                         sc_GUID,
                                                         ss_Description,
                                                         ss_Index,
                                                         ss_IsForUnknown,
                                                         sg_Description,
                                                         sg_Name,
                                                         sg_Index,
                                                         sc_Name,
                                                         sc_Description,
                                                         sc_Index,
                                                         sc_IsBasket,
                                                         sc_IsTrolley,
                                                         sc_IsGenericKit,
                                                         sc_IsForNewPosition,
														 sl_ShortName,
														 sl_Name,
														 sl_Description,
														 sl_Index,
														 sl_NumPositions,
														 sl_Guid,
														 cp_ShortName,
														 cp_Name,                    
														 cp_Description,  
														 cp_Index,
														 cp_Guid
               
	                                                     ";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@ResourceName", string.IsNullOrEmpty(resourceName) ? (object)DBNull.Value : resourceName);
                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objPositions.Add(new ResourceCabinet()
                                {
                                    StockRoomShortName = objReader["StockRoomShortName"].ToString(),
                                    StockRoomName = objReader["StockRoomName"].ToString(),
                                    StockRoomDescription = objReader["StockRoomDescription"].ToString(),
                                    StockRoomIndex = (int)objReader["StockRoomIndex"],
                                    StockRoomIsForUnknown =(bool)objReader["StockRoomIsForUnknown"],
                                    StockRoomId = objReader["StockRoomId"].ToString(),

                                    CabinetGroupShortName = objReader["CabinetGroupShortName"].ToString(),
                                    CabinetGroupId = objReader["CabinetGroupId"].ToString(),
                                    CabinetGroupName = objReader["CabinetGroupName"].ToString(),
                                    CabinetGroupDescription = objReader["CabinetGroupDescription"].ToString(),
                                    CabinetGroupIndex = Convert.ToInt32(objReader["CabinetGroupIndex"].ToString()),

                                    CabinetShortName = objReader["CabinetShortName"].ToString(),
                                    CabinetId = objReader["CabinetId"].ToString(),
                                    CabinetName = objReader["CabinetName"].ToString(),
                                    CabinetDescription = objReader["CabinetDescription"].ToString(),
                                    CabinetIndex = Convert.ToInt32(objReader["CabinetIndex"].ToString()),
                                    CabinetIsBasket =   objReader["CabinetIsBasket"] == DBNull.Value ? false : (bool)objReader["CabinetIsBasket"],
                                    CabinetIsTrolley = objReader["CabinetIsTrolley"] == DBNull.Value ? false : (bool)objReader["CabinetIsTrolley"],
                                    CabinetIsGenericKit = objReader["CabinetIsGenericKit"] == DBNull.Value ? false : (bool)objReader["CabinetIsGenericKit"],
                                    CabinetIsForNewPosition = objReader["CabinetIsForNewPosition"] == DBNull.Value ? false : (bool)objReader["CabinetIsForNewPosition"],

                                    LocationShortName = objReader["LocationShortName"].ToString(),
                                    LocationName = objReader["LocationName"].ToString(),
                                    LocationDescription = objReader["LocationDescription"].ToString(),
                                    LocationIndex = (int)objReader["LocationIndex"],
                                    LocationId = objReader["LocationId"].ToString(),
                                    LocationPositionNumber =  objReader["LocationPositionNumber"] == DBNull.Value ? null : (int)objReader["LocationPositionNumber"],


                                    PositionShortName = objReader["PositionShortName"].ToString(),
                                    PositionName = objReader["PositionName"].ToString(),
                                    PositionDescription = objReader["PositionDescription"].ToString(),
                                    PositionIndex = (int)objReader["PositionIndex"],
                                    PositionId = objReader["PositionId"].ToString(),
                                    RowNr = objReader["RowNr"] != DBNull.Value ? Convert.ToInt32(objReader["RowNr"].ToString()) : 0,

                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read ResourceCabinets - GetAllCabinets");
                string message = string.Format("Unable to read all ResourceCabinets records from DB");
                throw new Exception(message, ex);
            }
            return objPositions;
        }
        public List<ResourceCabinet> GetResourcesForPosition(string cpGuid,string searchKey)
        {
            List<ResourceCabinet> objPositions = new List<ResourceCabinet>();
            try
            {
                const string strQuery = @"SELECT  DISTINCT
                                        sr.sr_ID ResourceId,
                                        sr.sr_Code ResourceCode,
                                        sr.sr_ShortName ResourceShortName,
                                        sr.sr_Name ResourceName,
                                        sr.sr_Description ResourceDescription,
                                        sr.sr_MainStockroomName MainStore,
                                        spc.sp_IdealQuantity IdealQuantity,
                                        spc.sp_AlarmQuantity AlarmQuantity,
                                        spc.sp_ReorderLevel MinQuantity,
                                        spc.sp_GUID SpGuid
                                        FROM
                                        StandardCabinetPosition  p 
                                        LEFT JOIN StandardPositionContent spc on p.cp_GUID = spc.sp_cp_GUID
                                        LEFT JOIN StandardResource sr on sr_ID = spc.sp_sr_ID and sr_Current = 1
                                        WHERE (p.cp_GUID = @PositionId) and (@ResourceName is null or sr_Name LIKE  '%' + @ResourceName + '%')
	                                                ";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@PositionId", cpGuid);
                        objComm.Parameters.AddWithValue("@ResourceName", string.IsNullOrEmpty(searchKey) ? (object)DBNull.Value : searchKey);

                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objPositions.Add(new ResourceCabinet()
                                {
                                    ResourceId = objReader["ResourceId"].ToString(),
                                    ResourceCode = objReader["ResourceCode"].ToString(),
                                    ResourceShortName = objReader["ResourceShortName"].ToString(),
                                    ResourceName = objReader["ResourceCode"].ToString() + " " + objReader["ResourceName"].ToString(),
                                    ResourceDescription = objReader["ResourceDescription"].ToString(),
                                    MainStore = objReader["MainStore"].ToString(),
                                    IdealQuantity = objReader["IdealQuantity"] == DBNull.Value ? null : (double)objReader["IdealQuantity"],
                                    MinQuantity = objReader["MinQuantity"] == DBNull.Value ? null : (double)objReader["MinQuantity"],
                                    AlarmQuantity =objReader["AlarmQuantity"] == DBNull.Value ? null : (double)objReader["AlarmQuantity"],
                                    SpGuid = objReader["SpGuid"].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read ResourceCabinets - GetAllCabinets");
                string message = string.Format("Unable to read all ResourceCabinets records from DB");
                throw new Exception(message, ex);
            }
            return objPositions;
        }
        public List<ResourceCabinet> GetAllCabinets()
        {
            List<ResourceCabinet> objPositions = new List<ResourceCabinet>();
            try
            {
                const string strQuery = @"SELECT 
                                            ss_ShortName StockRoomShortName,
                                            ss_Name StockRoomName,
                                            ss_Description StockRoomDescription ,
                                            ss_Index StockRoomIndex,
                                            ss_IsForUnknown StockRoomIsForUnknown,
                                            ss_GUID StockRoomId,

                                            sg_ShortName CabinetGroupShortName,
                                            sg_Name CabinetGroupName,
                                            sg_GUID CabinetGroupId,
                                            sg_Description CabinetGroupDescription,
                                            sg_Index CabinetGroupIndex,

                                            sc_ShortName CabinetShortName,   
                                            sc_Name CabinetName,                    
                                            sc_Description CabinetDescription,  
                                            sc_Index CabinetIndex,
                                            sc_IsBasket CabinetIsBasket,
                                            sc_IsTrolley CabinetIsTrolley,
                                            sc_IsGenericKit CabinetIsGenericKit,
                                            sc_IsForNewPosition CabinetIsForNewPosition,
                                            sc_GUID CabinetId,
                                           
										    sl_ShortName LocationShortName,   
                                            sl_Name LocationName,                    
                                            sl_Description LocationDescription,  
										    sl_Index LocationIndex,
											sl_NumPositions LocationPositionNumber,
											sl_Guid LocationId,

											cp_ShortName PositionShortName,   
                                            cp_Name PositionName,                    
                                            cp_Description PositionDescription,  
										    cp_Index PositionIndex,
											cp_Guid PositionId,

                                            sr.sr_ID ResourceId,
                                            sr.sr_Code ResourceCode,
											sr.sr_ShortName ResourceShortName,
									        sr.sr_Name ResourceName,
											sr.sr_Description ResourceDescription,
											sr.sr_MainStockroomName MainStore,
											spc.sp_IdealQuantity IdealQuantity,
											spc.sp_AlarmQuantity AlarmQuantity,
											spc.sp_ReorderLevel MinQuantity,
                                            spc.sp_GUID SpGuid
                                            FROM 
                                            StandardStockrooms ss 
                                            INNER JOIN StandardCabinetGroups g on ss.ss_GUID = g.sg_ss_GUID
                                            INNER JOIN StandardCabinets s on g.sg_GUID = s.sc_sg_GUID
                                            INNER JOIN StandardCabinetLocations l on l.sl_sc_GUID = s.sc_GUID   
											INNER JOIN StandardCabinetPosition p on p.cp_sl_GUID = l.sl_GUID   
	                                        LEFT JOIN StandardPositionContent spc on p.cp_GUID = spc.sp_cp_GUID
											LEFT JOIN StandardResource sr on sr_ID = spc.sp_sr_ID

                                                         GROUP BY ss_Name,
                                                         ss_ShortName,
                                                         ss_GUID, 
                                                         sg_ShortName,
                                                         sg_GUID,
                                                         sc_ShortName,
                                                         sc_GUID,
                                                         ss_Description,
                                                         ss_Index,
                                                         ss_IsForUnknown,
                                                         sg_Description,
                                                         sg_Name,
                                                         sg_Index,
                                                         sc_Name,
                                                         sc_Description,
                                                         sc_Index,
                                                         sc_IsBasket,
                                                         sc_IsTrolley,
                                                         sc_IsGenericKit,
                                                         sc_IsForNewPosition,
														 sl_ShortName,
														 sl_Name,
														 sl_Description,
														 sl_Index,
														 sl_NumPositions,
														 sl_Guid,
														 cp_ShortName,
														 cp_Name,                    
														 cp_Description,  
														 cp_Index,
														 cp_Guid,
                                                         sr.sr_ID,
                                                         sr.sr_Code,
														 sr.sr_ShortName,
														 sr.sr_Name,
														 sr.sr_Description,
														 sr.sr_MainStockroomName,
														 spc.sp_IdealQuantity,
														 spc.sp_AlarmQuantity,
														 spc.sp_ReorderLevel,
                                                         spc.sp_GUID
	                                                     ";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {

                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objPositions.Add(new ResourceCabinet()
                                {
                                    StockRoomShortName = objReader["StockRoomShortName"].ToString(),
                                    StockRoomName = objReader["StockRoomName"].ToString(),
                                    StockRoomDescription = objReader["StockRoomDescription"].ToString(),
                                    StockRoomIndex = (int)objReader["StockRoomIndex"],
                                    StockRoomIsForUnknown =(bool)objReader["StockRoomIsForUnknown"],
                                    StockRoomId = objReader["StockRoomId"].ToString(),

                                    CabinetGroupShortName = objReader["CabinetGroupShortName"].ToString(),
                                    CabinetGroupId = objReader["CabinetGroupId"].ToString(),
                                    CabinetGroupName = objReader["CabinetGroupName"].ToString(),
                                    CabinetGroupDescription = objReader["CabinetGroupDescription"].ToString(),
                                    CabinetGroupIndex = Convert.ToInt32(objReader["CabinetGroupIndex"].ToString()),

                                    CabinetShortName = objReader["CabinetShortName"].ToString(),
                                    CabinetId = objReader["CabinetId"].ToString(),
                                    CabinetName = objReader["CabinetName"].ToString(),
                                    CabinetDescription = objReader["CabinetDescription"].ToString(),
                                    CabinetIndex = Convert.ToInt32(objReader["CabinetIndex"].ToString()),
                                    CabinetIsBasket =   objReader["CabinetIsBasket"] == DBNull.Value ? false : (bool)objReader["CabinetIsBasket"],
                                    CabinetIsTrolley = objReader["CabinetIsTrolley"] == DBNull.Value ? false : (bool)objReader["CabinetIsTrolley"],
                                    CabinetIsGenericKit = objReader["CabinetIsGenericKit"] == DBNull.Value ? false : (bool)objReader["CabinetIsGenericKit"],
                                    CabinetIsForNewPosition = objReader["CabinetIsForNewPosition"] == DBNull.Value ? false : (bool)objReader["CabinetIsForNewPosition"],

                                    LocationShortName = objReader["LocationShortName"].ToString(),
                                    LocationName = objReader["LocationName"].ToString(),
                                    LocationDescription = objReader["LocationDescription"].ToString(),
                                    LocationIndex = (int)objReader["LocationIndex"],
                                    LocationId = objReader["LocationId"].ToString(),
                                    LocationPositionNumber =  objReader["LocationPositionNumber"] == DBNull.Value ? null : (int)objReader["LocationPositionNumber"],


                                    PositionShortName = objReader["PositionShortName"].ToString(),
                                    PositionName = objReader["PositionName"].ToString(),
                                    PositionDescription = objReader["PositionDescription"].ToString(),
                                    PositionIndex = (int)objReader["PositionIndex"],
                                    PositionId = objReader["PositionId"].ToString(),

                                    ResourceId = objReader["ResourceId"].ToString(),
                                    ResourceCode = objReader["ResourceCode"].ToString(),
                                    ResourceShortName = objReader["ResourceShortName"].ToString(),
                                    ResourceName = objReader["ResourceName"].ToString(),
                                    ResourceDescription = objReader["ResourceDescription"].ToString(),
                                    MainStore = objReader["MainStore"].ToString(),
                                    IdealQuantity = objReader["IdealQuantity"] == DBNull.Value ? null : (double)objReader["IdealQuantity"],
                                    MinQuantity = objReader["MinQuantity"] == DBNull.Value ? null : (double)objReader["MinQuantity"],
                                    AlarmQuantity =objReader["AlarmQuantity"] == DBNull.Value ? null : (double)objReader["AlarmQuantity"],
                                    SpGuid = objReader["SpGuid"].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read ResourceCabinets - GetAllCabinets");
                string message = string.Format("Unable to read all ResourceCabinets records from DB");
                throw new Exception(message, ex);
            }
            return objPositions;
        }
        public List<ResourceCabinet> GetAllCabinetsWithResources(string resourceName)
        {
            List<ResourceCabinet> objPositions = new List<ResourceCabinet>();
            try
            {
                const string strQuery = @"SELECT 
                                            ss_ShortName StockRoomShortName,
                                            ss_Name StockRoomName,
                                            ss_Description StockRoomDescription ,
                                            ss_Index StockRoomIndex,
                                            ss_IsForUnknown StockRoomIsForUnknown,
                                            ss_GUID StockRoomId,

                                            sg_ShortName CabinetGroupShortName,
                                            sg_Name CabinetGroupName,
                                            sg_GUID CabinetGroupId,
                                            sg_Description CabinetGroupDescription,

                                            sc_ShortName CabinetShortName,   
                                            sc_Name CabinetName,                    
                                            sc_Description CabinetDescription,  
                                            sc_Index CabinetIndex,
                                            sc_IsBasket CabinetIsBasket,
                                            sc_IsTrolley CabinetIsTrolley,
                                            sc_IsGenericKit CabinetIsGenericKit,
                                            sc_IsForNewPosition CabinetIsForNewPosition,
                                            sc_GUID CabinetId,
                                           
										    sl_ShortName LocationShortName,   
                                            sl_Name LocationName,                    
                                            sl_Description LocationDescription,  
										    sl_Index LocationIndex,
											sl_NumPositions LocationPositionNumber,
											sl_Guid LocationId,

											cp_ShortName PositionShortName,   
                                            cp_Name PositionName,                    
                                            cp_Description PositionDescription,  
										    cp_Index PositionIndex,
											cp_Guid PositionId,

                                            sr.sr_ID ResourceId,
                                            sr.sr_Code ResourceCode,
											sr.sr_ShortName ResourceShortName,
									        sr.sr_Name ResourceName,
											sr.sr_Description ResourceDescription,
											sr.sr_MainStockroomName MainStore,
											spc.sp_IdealQuantity IdealQuantity,
											spc.sp_AlarmQuantity AlarmQuantity,
											spc.sp_ReorderLevel MinQuantity
                                            FROM 
                                            StandardStockrooms ss 
                                            INNER JOIN StandardCabinetGroups g on ss.ss_GUID = g.sg_ss_GUID
                                            INNER JOIN StandardCabinets s on g.sg_GUID = s.sc_sg_GUID
                                            INNER JOIN StandardCabinetLocations l on l.sl_sc_GUID = s.sc_GUID   
											INNER JOIN StandardCabinetPosition p on p.cp_sl_GUID = l.sl_GUID   
	                                        LEFT JOIN StandardPositionContent spc on p.cp_GUID = spc.sp_cp_GUID
											LEFT JOIN StandardResource sr on sr_ID = spc.sp_sr_ID
											WHERe sr_Name LIKE  '%' + @ResourceName + '%'		
                                                         GROUP BY ss_Name,
                                                         ss_ShortName,
                                                         ss_GUID, 
                                                         sg_ShortName,
                                                         sg_GUID,
                                                         sc_ShortName,
                                                         sc_GUID,
                                                         ss_Description,
                                                         ss_Index,
                                                         ss_IsForUnknown,
                                                         sg_Description,
                                                         sg_Name,
                                                         sc_Name,
                                                         sc_Description,
                                                         sc_Index,
                                                         sc_IsBasket,
                                                         sc_IsTrolley,
                                                         sc_IsGenericKit,
                                                         sc_IsForNewPosition,
														 sl_ShortName,
														 sl_Name,
														 sl_Description,
														 sl_Index,
														 sl_NumPositions,
														 sl_Guid,
														 cp_ShortName,
														 cp_Name,                    
														 cp_Description,  
														 cp_Index,
														 cp_Guid,
                                                         sr.sr_ID,
                                                         sr.sr_Code,
														 sr.sr_ShortName,
														 sr.sr_Name,
														 sr.sr_Description,
														 sr.sr_MainStockroomName,
														 spc.sp_IdealQuantity,
														 spc.sp_AlarmQuantity,
														 spc.sp_ReorderLevel
	                                            ";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@ResourceName", resourceName);

                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objPositions.Add(new ResourceCabinet()
                                {
                                    StockRoomShortName = objReader["StockRoomShortName"].ToString(),
                                    StockRoomName = objReader["StockRoomName"].ToString(),
                                    StockRoomDescription = objReader["StockRoomDescription"].ToString(),
                                    StockRoomIndex = (int)objReader["StockRoomIndex"],
                                    StockRoomIsForUnknown =(bool)objReader["StockRoomIsForUnknown"],
                                    StockRoomId = objReader["StockRoomId"].ToString(),

                                    CabinetGroupShortName = objReader["CabinetGroupShortName"].ToString(),
                                    CabinetGroupId = objReader["CabinetGroupId"].ToString(),
                                    CabinetGroupName = objReader["CabinetGroupName"].ToString(),
                                    CabinetGroupDescription = objReader["CabinetGroupDescription"].ToString(),

                                    CabinetShortName = objReader["CabinetShortName"].ToString(),
                                    CabinetId = objReader["CabinetId"].ToString(),
                                    CabinetName = objReader["CabinetName"].ToString(),
                                    CabinetDescription = objReader["CabinetDescription"].ToString(),
                                    CabinetIndex = Convert.ToInt32(objReader["CabinetIndex"].ToString()),
                                    CabinetIsBasket =   objReader["CabinetIsBasket"] == DBNull.Value ? false : (bool)objReader["CabinetIsBasket"],
                                    CabinetIsTrolley = objReader["CabinetIsTrolley"] == DBNull.Value ? false : (bool)objReader["CabinetIsTrolley"],
                                    CabinetIsGenericKit = objReader["CabinetIsGenericKit"] == DBNull.Value ? false : (bool)objReader["CabinetIsGenericKit"],
                                    CabinetIsForNewPosition = objReader["CabinetIsForNewPosition"] == DBNull.Value ? false : (bool)objReader["CabinetIsForNewPosition"],

                                    LocationShortName = objReader["LocationShortName"].ToString(),
                                    LocationName = objReader["LocationName"].ToString(),
                                    LocationDescription = objReader["LocationDescription"].ToString(),
                                    LocationIndex = (int)objReader["LocationIndex"],
                                    LocationId = objReader["LocationId"].ToString(),
                                    LocationPositionNumber =  objReader["LocationPositionNumber"] == DBNull.Value ? null : (int)objReader["LocationPositionNumber"],


                                    PositionShortName = objReader["PositionShortName"].ToString(),
                                    PositionName = objReader["PositionName"].ToString(),
                                    PositionDescription = objReader["PositionDescription"].ToString(),
                                    PositionIndex = (int)objReader["PositionIndex"],
                                    PositionId = objReader["PositionId"].ToString(),

                                    ResourceId = objReader["ResourceId"].ToString(),
                                    ResourceCode = objReader["ResourceCode"].ToString(),
                                    ResourceShortName = objReader["ResourceShortName"].ToString(),
                                    ResourceName = objReader["ResourceName"].ToString(),
                                    ResourceDescription = objReader["ResourceDescription"].ToString(),
                                    MainStore = objReader["MainStore"].ToString(),
                                    IdealQuantity = objReader["IdealQuantity"] == DBNull.Value ? null : (double)objReader["IdealQuantity"],
                                    MinQuantity = objReader["MinQuantity"] == DBNull.Value ? null : (double)objReader["MinQuantity"],
                                    AlarmQuantity =objReader["AlarmQuantity"] == DBNull.Value ? null : (double)objReader["AlarmQuantity"],
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read ResourceCabinets - GetAllCabinets");
                string message = string.Format("Unable to read all ResourceCabinets records from DB");
                throw new Exception(message, ex);
            }
            return objPositions;
        }
        public List<ResourceTreeListTemplate> GetResourcesForCabinetPositions(string resourceName)
        {
            List<ResourceTreeListTemplate> objPositionsResources = new List<ResourceTreeListTemplate>();
            try
            {
                const string strQuery = @"SELECT  
                                            r.sr_Name ResourceName, 
                                            r.sr_ID ResourceId,
                                            r.sr_Code ResourceCode,
                                            r.sr_ShortName ResourceShortName,
                                            r.sr_Description ResourceDescription,
                                            r.sr_MainStockroomName MainStore,
                                            s.sp_IdealQuantity IdealQuantity,
                                            s.sp_AlarmQuantity AlarmQuantity,
                                            s.sp_ReorderLevel MinQuantity,
                                            c.cp_GUID  PositionId
                                            FROM 
                                            StandardCabinetPosition c
                                            INNER JOIN StandardPositionContent s on s.sp_cp_GUID = c.cp_GUID
                                            INNER JOIN StandardResource r on r.sr_ID = s.sp_sr_ID
											WHERe sr_Name LIKE  '%' + @ResourceName + '%'";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {
                        objComm.Parameters.AddWithValue("@ResourceName", resourceName);
                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objPositionsResources.Add(new ResourceTreeListTemplate()
                                {
                                    ResourceId = objReader["ResourceId"].ToString(),
                                    ResourceCode = objReader["ResourceCode"].ToString(),
                                    ResourceShortName= objReader["ResourceShortName"].ToString(),
                                    ResourceName= objReader["ResourceName"].ToString(),
                                    ResourceDescription= objReader["ResourceDescription"].ToString(),
                                    MainStore = objReader["MainStore"].ToString(),
                                    IdealQuantity = objReader["IdealQuantity"] == DBNull.Value ? null : (double)objReader["IdealQuantity"],
                                    MinQuantity = objReader["MinQuantity"] == DBNull.Value ? null : (double)objReader["MinQuantity"],
                                    AlarmQuantity =objReader["AlarmQuantity"] == DBNull.Value ? null : (double)objReader["AlarmQuantity"],
                                    ParentId = objReader["PositionId"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read ResourceCabinetPositions - GetResourcesForCabinetPositions");
                string message = string.Format("Unable to read all ResourceCabinetPositions records from DB");
                throw new Exception(message, ex);
            }
            return objPositionsResources;
        }
        public void InsertStockRoom(StockRoomDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"CREATE TABLE #InsertedStockRoomId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)
                                                            CREATE TABLE #InsertedCabinetGroupId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)
                                                            CREATE TABLE #InsertedCabinetId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)
                                                            CREATE TABLE #InsertedLocationId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)

                                                            INSERT INTO StandardStockrooms
                                                            (ss_GUID,ss_Code,ss_Name,ss_ShortName,ss_Description,ss_Index,ss_IsForUnknown) 
                                                            OUTPUT INSERTED.ss_GUID INTO #InsertedStockRoomId
                                                            VALUES
                                                            (NEWID(),@StockRoomShortName,@StockRoomName,@StockRoomShortName,@StockRoomDescription,@StockRoomIndex,@StockRoomIsForUnknown)

                                                            INSERT INTO StandardCabinetGroups
                                                            (sG_GUID,sg_Code,sg_Name,sg_ShortName,sg_Description,sg_Index,sg_ss_GUID) 
                                                            OUTPUT INSERTED.sg_GUID INTO #InsertedCabinetGroupId
                                                            VALUES
                                                            (NEWID(),'CABINETGROUP11','CABINETGROUP11','CABINETGROUP11',NULL,0, (select ID from #InsertedStockRoomId))

                                                            INSERT INTO StandardCabinets
                                                            (sc_GUID,sc_Code,sc_Name,sc_ShortName,sc_Description,sc_NumLocations, sc_Index,sc_sg_GUID,sc_Picture,sc_IsBasket,sc_IsTrolley,sc_CostCenter,sc_IsGenericKit,sc_IsForNewPosition) 
                                                            OUTPUT INSERTED.sc_GUID INTO #InsertedCabinetId
                                                            VALUES
                                                            (NEWID(),'CABINET11','CABINET11','CABINET11',NULL,NULL,0,(select ID from #InsertedCabinetGroupId),NULL,0,0,NULL,0,0)

                                                            INSERT INTO StandardCabinetLocations
                                                            (sl_GUID,sl_Code,sl_Name,sl_ShortName,sl_Description,sl_NumPositions, sl_Index,sl_sc_GUID,sl_Left,sl_Top,sl_Width,sl_Height) 
                                                            OUTPUT INSERTED.sl_GUID INTO #InsertedLocationId
                                                            VALUES
                                                            (NEWID(),'LOCATION11','LOCATION11','LOCATION11',NULL,1,0,(select ID from #InsertedCabinetId),NULL,NULL,NULL,NULL)

                                                            INSERT INTO StandardCabinetPosition
                                                            (cp_GUID,cp_Code,cp_Name,cp_ShortName,cp_Description, cp_Index,cp_sl_GUID) 
                                                            VALUES
                                                            (NEWID(),'a11','a11','a11',NULL,0,(select ID from #InsertedLocationId))

                                                            DROP TABLE #InsertedStockRoomId,#InsertedCabinetGroupId,#InsertedCabinetId,#InsertedLocationId

", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@StockRoomName", request.StockRoomName);
                        objCommand.Parameters.AddWithValue("@StockRoomShortName", request.StockRoomShortName);
                        objCommand.Parameters.AddWithValue("@StockRoomDescription", request.StockRoomDescription);
                        objCommand.Parameters.AddWithValue("@StockRoomIndex", request.StockRoomIndex);
                        objCommand.Parameters.AddWithValue("@StockRoomIsForUnknown", request.StockRoomIsForUnknown);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Stockroom - InsertStockRoom");
                string message = string.Format("Unable to insert Stockroom record in DB");
                throw new Exception(message, ex);
            }
        }
        public void UpdateStockRoom(StockRoomDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"update StandardStockrooms set 
                                                            ss_Code = @StockRoomShortName,
                                                            ss_Name = @StockRoomName,
                                                            ss_ShortName = @StockRoomShortName,
                                                            ss_Description = @StockRoomDescription,
                                                            ss_index = @StockRoomIndex,
                                                            ss_IsForUnknown = @StockRoomIsForUnknown
                                                            where ss_GUID = @StockRoomId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@StockRoomName", request.StockRoomName);
                        objCommand.Parameters.AddWithValue("@StockRoomShortName", request.StockRoomShortName);
                        objCommand.Parameters.AddWithValue("@StockRoomDescription", request.StockRoomDescription);
                        objCommand.Parameters.AddWithValue("@StockRoomIndex", request.StockRoomIndex);
                        objCommand.Parameters.AddWithValue("@StockRoomIsForUnknown", request.StockRoomIsForUnknown);
                        objCommand.Parameters.AddWithValue("@StockRoomId", request.StockRoomId);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Stockroom - InsertStockRoom");
                string message = string.Format("Unable to insert Stockroom record in DB");
                throw new Exception(message, ex);
            }
        }
        public void InsertCabinetGroup(CabinetGroupDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                            CREATE TABLE #InsertedCabinetGroupId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)
                                                            CREATE TABLE #InsertedCabinetId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)
                                                            CREATE TABLE #InsertedLocationId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)                                                           

                                                            INSERT INTO StandardCabinetGroups
                                                            (sG_GUID,sg_Code,sg_Name,sg_ShortName,sg_Description,sg_Index,sg_ss_GUID) 
                                                            OUTPUT INSERTED.sg_GUID INTO #InsertedCabinetGroupId
                                                            VALUES
                                                            (NEWID(),@CabinetGroupShortName,@CabinetGroupName,@CabinetGroupShortName,@CabinetGroupDescription,@CabinetGroupIndex,@StockRoomId )

                                                            INSERT INTO StandardCabinets
                                                            (sc_GUID,sc_Code,sc_Name,sc_ShortName,sc_Description,sc_NumLocations, sc_Index,sc_sg_GUID,sc_Picture,sc_IsBasket,sc_IsTrolley,sc_CostCenter,sc_IsGenericKit,sc_IsForNewPosition) 
                                                            OUTPUT INSERTED.sc_GUID INTO #InsertedCabinetId
                                                            VALUES
                                                            (NEWID(),'CABINET11','CABINET11','CABINET11',NULL,NULL,0,(select ID from #InsertedCabinetGroupId),NULL,0,0,NULL,0,0)

                                                            INSERT INTO StandardCabinetLocations
                                                            (sl_GUID,sl_Code,sl_Name,sl_ShortName,sl_Description,sl_NumPositions, sl_Index,sl_sc_GUID,sl_Left,sl_Top,sl_Width,sl_Height) 
                                                            OUTPUT INSERTED.sl_GUID INTO #InsertedLocationId
                                                            VALUES
                                                            (NEWID(),'LOCATION11','LOCATION11','LOCATION11',NULL,1,0,(select ID from #InsertedCabinetId),NULL,NULL,NULL,NULL)

                                                            INSERT INTO StandardCabinetPosition
                                                            (cp_GUID,cp_Code,cp_Name,cp_ShortName,cp_Description, cp_Index,cp_sl_GUID) 
                                                            VALUES
                                                            (NEWID(),'a11','a11','a11',NULL,0,(select ID from #InsertedLocationId))

                                                            DROP TABLE #InsertedCabinetGroupId,#InsertedCabinetId,#InsertedLocationId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@CabinetGroupName", request.CabinetGroupName);
                        objCommand.Parameters.AddWithValue("@CabinetGroupShortName", request.CabinetGroupShortName);
                        objCommand.Parameters.AddWithValue("@CabinetGroupDescription", request.CabinetGroupDescription);
                        objCommand.Parameters.AddWithValue("@CabinetGroupIndex", request.CabinetGroupIndex);
                        objCommand.Parameters.AddWithValue("@StockRoomId", request.StockRoomId);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Stockroom - InsertStockRoom");
                string message = string.Format("Unable to insert Stockroom record in DB");
                throw new Exception(message, ex);
            }
        }
        public void UpdateCabinetGroup(CabinetGroupDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"UPDATE StandardCabinetGroups SET 
                                                            sg_Code = @CabinetGroupShortName,
                                                            sg_Name = @CabinetGroupName,
                                                            sg_ShortName = @CabinetGroupShortName,
                                                            sg_Description = @CabinetGroupDescription,
                                                            sg_index = @CabinetGroupIndex
                                                            where sg_GUID = @CabinetGroupId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@CabinetGroupName", request.CabinetGroupName);
                        objCommand.Parameters.AddWithValue("@CabinetGroupShortName", request.CabinetGroupShortName);
                        objCommand.Parameters.AddWithValue("@CabinetGroupDescription", request.CabinetGroupDescription);
                        objCommand.Parameters.AddWithValue("@CabinetGroupIndex", request.CabinetGroupIndex);
                        objCommand.Parameters.AddWithValue("@CabinetGroupId", request.CabinetGroupId);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Stockroom - InsertStockRoom");
                string message = string.Format("Unable to insert Stockroom record in DB");
                throw new Exception(message, ex);
            }
        }
        public void InsertCabinet(CabinetDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                            CREATE TABLE #InsertedCabinetId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)
                                                            CREATE TABLE #InsertedLocationId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)                                                           

                                                           
                                                            INSERT INTO StandardCabinets
                                                            (sc_GUID,sc_Code,sc_Name,sc_ShortName,sc_Description,sc_NumLocations, sc_Index,sc_sg_GUID,sc_Picture,sc_IsBasket,sc_IsTrolley,sc_CostCenter,sc_IsGenericKit,sc_IsForNewPosition) 
                                                            OUTPUT INSERTED.sc_GUID INTO #InsertedCabinetId
                                                            VALUES
                                                            (NEWID(),@CabinetShortName,@CabinetName,@CabinetShortName,@CabinetDescription,NULL,@CabinetIndex,@CabinetGroupId,NULL,@CabinetIsBasket,@CabinetIsTrolley,NULL,@CabinetIsGenericKit,@CabinetIsForNewPosition)

                                                            INSERT INTO StandardCabinetLocations
                                                            (sl_GUID,sl_Code,sl_Name,sl_ShortName,sl_Description,sl_NumPositions, sl_Index,sl_sc_GUID,sl_Left,sl_Top,sl_Width,sl_Height) 
                                                            OUTPUT INSERTED.sl_GUID INTO #InsertedLocationId
                                                            VALUES
                                                            (NEWID(),'LOCATION11','LOCATION11','LOCATION11',NULL,1,0,(select ID from #InsertedCabinetId),NULL,NULL,NULL,NULL)

                                                            INSERT INTO StandardCabinetPosition
                                                            (cp_GUID,cp_Code,cp_Name,cp_ShortName,cp_Description, cp_Index,cp_sl_GUID) 
                                                            VALUES
                                                            (NEWID(),'a11','a11','a11',NULL,0,(select ID from #InsertedLocationId))

                                                            DROP TABLE #InsertedCabinetId,#InsertedLocationId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@CabinetName", request.CabinetName);
                        objCommand.Parameters.AddWithValue("@CabinetShortName", request.CabinetShortName);
                        objCommand.Parameters.AddWithValue("@CabinetDescription", request.CabinetDescription);
                        objCommand.Parameters.AddWithValue("@CabinetIndex", request.CabinetIndex);
                        objCommand.Parameters.AddWithValue("@CabinetGroupId", request.CabinetGroupId);
                        objCommand.Parameters.AddWithValue("@CabinetIsBasket", request.CabinetIsBasket);
                        objCommand.Parameters.AddWithValue("@CabinetIsTrolley", request.CabinetIsTrolley);
                        objCommand.Parameters.AddWithValue("@CabinetIsGenericKit", request.CabinetIsGenericKit);
                        objCommand.Parameters.AddWithValue("@CabinetIsForNewPosition", request.CabinetIsForNewPosition);

                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Cabinet - InsertCabinet");
                string message = string.Format("Unable to insert Cabinet record in DB");
                throw new Exception(message, ex);
            }
        }
        public void UpdateCabinet(CabinetDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"UPDATE StandardCabinets SET 
                                                            sc_Code = @CabinetShortName,
                                                            sc_Name = @CabinetName,
                                                            sc_ShortName = @CabinetShortName,
                                                            sc_Description = @CabinetDescription,
                                                            sc_Index = @CabinetIndex,
                                                            sc_IsBasket = @CabinetIsBasket,
                                                            sc_IsTrolley = @CabinetIsTrolley,
                                                            sc_IsGenericKit = @CabinetIsGenericKit,
                                                            sc_IsForNewPosition =@CabinetIsForNewPosition

                                                            where sc_GUID = @CabinetId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@CabinetName", request.CabinetName);
                        objCommand.Parameters.AddWithValue("@CabinetShortName", request.CabinetShortName);
                        objCommand.Parameters.AddWithValue("@CabinetDescription", request.CabinetDescription);
                        objCommand.Parameters.AddWithValue("@CabinetIndex", request.CabinetIndex);
                        objCommand.Parameters.AddWithValue("@CabinetId", request.CabinetId);
                        objCommand.Parameters.AddWithValue("@CabinetIsBasket", request.CabinetIsBasket);
                        objCommand.Parameters.AddWithValue("@CabinetIsTrolley", request.CabinetIsTrolley);
                        objCommand.Parameters.AddWithValue("@CabinetIsGenericKit", request.CabinetIsGenericKit);
                        objCommand.Parameters.AddWithValue("@CabinetIsForNewPosition", request.CabinetIsForNewPosition);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to update Cabinet - InsertStockRoom");
                string message = string.Format("Unable to update Cabinet record in DB");
                throw new Exception(message, ex);
            }
        }
        public void InsertLocation(LocationDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                            CREATE TABLE #InsertedLocationId (ID char(36) COLLATE SQL_Latin1_General_CP1_CI_AS)                                                           
                                                            DECLARE @index INT =  0 
															declare @numpos INT  = @LocationPositions
                                                            DECLARE @sequenceChar INT  = 97 
                                                                                                                       
                                                            INSERT INTO StandardCabinetLocations
                                                            (sl_GUID,sl_Code,sl_Name,sl_ShortName,sl_Description,sl_NumPositions, sl_Index,sl_sc_GUID,sl_Left,sl_Top,sl_Width,sl_Height) 
                                                            OUTPUT INSERTED.sl_GUID INTO #InsertedLocationId
                                                            VALUES
                                                            (NEWID(),@LocationShortName,@LocationName,@LocationShortName,@LocationDescription,@LocationPositions,@LocationIndex,@CabinetId,NULL,NULL,NULL,NULL)
                                                            
                                                            WHILE @index<@numpos
                                                            BEGIN
                                                            INSERT INTO StandardCabinetPosition
                                                            (cp_GUID,cp_Code,cp_Name,cp_ShortName,cp_Description, cp_Index,cp_sl_GUID) 
                                                            VALUES
                                                            (NEWID(),char(@sequenceChar),char(@sequenceChar),char(@sequenceChar),NULL,0,(select ID from #InsertedLocationId))
                                                            SET @sequenceChar = @sequenceChar + 1
                                                            SET @index = @index + 1                                                            
                                                            end
                                                            DROP TABLE #InsertedLocationId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@LocationName", request.LocationName);
                        objCommand.Parameters.AddWithValue("@LocationShortName", request.LocationShortName);
                        objCommand.Parameters.AddWithValue("@LocationDescription", request.LocationDescription);
                        objCommand.Parameters.AddWithValue("@LocationIndex", request.LocationIndex);
                        objCommand.Parameters.AddWithValue("@CabinetId", request.CabinetId);
                        objCommand.Parameters.AddWithValue("@LocationPositions", request.LocationPositionNumber);

                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Cabinet - InsertCabinet");
                string message = string.Format("Unable to insert Cabinet record in DB");
                throw new Exception(message, ex);
            }
        }
        public void UpdateLocation(LocationDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                           UPDATE StandardCabinetLocations SET                                                            
                                                            sl_Name = @LocationName,
                                                            sl_ShortName = @LocationShortName,
                                                            sl_Description = @LocationDescription,
                                                            sl_Index =@LocationIndex,
                                                            sl_NumPositions = @LocationPositions                                                           

                                                            where sl_GUID = @LocationId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@LocationName", request.LocationName);
                        objCommand.Parameters.AddWithValue("@LocationShortName", request.LocationShortName);
                        objCommand.Parameters.AddWithValue("@LocationDescription", request.LocationDescription);
                        objCommand.Parameters.AddWithValue("@LocationIndex", request.LocationIndex);
                        objCommand.Parameters.AddWithValue("@LocationId", request.LocationId);
                        objCommand.Parameters.AddWithValue("@LocationPositions", request.LocationPositionNumber);

                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Cabinet - InsertCabinet");
                string message = string.Format("Unable to insert Cabinet record in DB");
                throw new Exception(message, ex);
            }
        }
        public void InsertPosition(PositionDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"                                                           
                                                            INSERT INTO StandardCabinetPosition
                                                            (cp_GUID,cp_Code,cp_Name,cp_ShortName,cp_Description,cp_Index,cp_sl_GUID) 
                                                            VALUES
                                                            (NEWID(),@PositionShortName,@PositionName,@PositionShortName,@PositionDescription,@PositionIndex,@LocationId)                                                         
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@PositionName", request.PositionName);
                        objCommand.Parameters.AddWithValue("@PositionShortName", request.PositionShortName);
                        objCommand.Parameters.AddWithValue("@PositionDescription", request.PositionDescription);
                        objCommand.Parameters.AddWithValue("@PositionIndex", request.PositionIndex);
                        objCommand.Parameters.AddWithValue("@LocationId", request.LocationId);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Position - InsertPosition");
                string message = string.Format("Unable to insert Position record in DB");
                throw new Exception(message, ex);
            }
        }
        public void UpdatePosition(PositionDto request)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                           UPDATE StandardCabinetPosition SET                                                            
                                                            cp_Name = @PositionName,
                                                            cp_ShortName = @PositionShortName,
                                                            cp_Description = @PositionDescription,
                                                            cp_Index =@PositionIndex                                                                                                                     
                                                            where cp_GUID = @PositionId
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@PositionName", request.PositionName);
                        objCommand.Parameters.AddWithValue("@PositionShortName", request.PositionShortName);
                        objCommand.Parameters.AddWithValue("@PositionDescription", request.PositionDescription);
                        objCommand.Parameters.AddWithValue("@PositionIndex", request.PositionIndex);
                        objCommand.Parameters.AddWithValue("@PositionId", request.PositionId);

                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to update Position - InsertPosition");
                string message = string.Format("Unable to update Position record in DB");
                throw new Exception(message, ex);
            }
        }
        public void InsertResource(string srId, string cpId)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"INSERT INTO                                                           
                                                            StandardPositionContent 
                                                            (sp_GUID, sp_sr_ID, sp_IdealQuantity, sp_ReorderLevel, sp_AlarmQuantity, sp_cp_GUID, sp_LatestAlarm) 
                                                            VALUES     
                                                            (NEWID(),@SrId,0,0,0,@CpId,0)    
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@SrId", srId);
                        objCommand.Parameters.AddWithValue("@CpId", cpId);

                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to insert Resource - InsertResource");
                string message = string.Format("Unable to insert Resource record in DB");
                throw new Exception(message, ex);
            }
        }
        public List<ResourceDto> GetAllResources()
        {
            List<ResourceDto> objResources = new List<ResourceDto>();
            try
            {
                const string strQuery = @"SELECT DISTINCT sr_ID SrId,sr_Name SrName,Sr_code SrCode
                                            FROM 
                                            StandardPositionContent C 
                                            INNER JOIN StandardResource R on c.sp_sr_ID = R.sr_ID
	                                      ";
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                    {

                        using (SqlDataReader objReader = objComm.ExecuteReader())
                        {
                            while (objReader.Read())
                            {
                                objResources.Add(new ResourceDto()
                                {
                                    SrName = objReader["SrName"].ToString(),
                                    SrCode = objReader["SrCode"].ToString(),
                                    SrId = objReader["SrId"].ToString()

                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to read Resources - GeatAllResources");
                string message = string.Format("Unable to read all Resources records from DB");
                throw new Exception(message, ex);
            }
            return objResources;
        }
        //stocks
        public List<StockRoomDto> GetStockRooms()
        {
            List<StockRoomDto> objStockRooms = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT ss_ShortName StockRoomShortName,ss_GUID StockRoomId
                                    FROM StandardStockRooms";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {

                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            objStockRooms.Add(new StockRoomDto()
                            {
                                StockRoomShortName = objReader["StockRoomShortName"].ToString(),
                                StockRoomId  =  objReader["StockRoomId"].ToString()
                            });
                        }


                    }
                }
            }
            return objStockRooms;
        }
        public List<CabinetGroupDto> GetCabinetGroups(string stockRoomId)
        {
            List<CabinetGroupDto> objCabinetGroups = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT sg_ShortName CabinetGroupShortName, sg_GUID CabinetGroupId
                                    FROM StandardCabinetGroups
                                    where sg_ss_GUID  = @StockRoomId";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@StockRoomId", stockRoomId);
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            objCabinetGroups.Add(new CabinetGroupDto()
                            {
                                CabinetGroupShortName = objReader["CabinetGroupShortName"].ToString(),
                                CabinetGroupId  =  objReader["CabinetGroupId"].ToString()
                            });
                        }


                    }
                }
            }
            return objCabinetGroups;
        }
        public List<CabinetDto> GetCabinets(string cabinetGroupId)
        {
            List<CabinetDto> objCabinets = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT sc_ShortName CabinetShortName,sc_GUID CabinetId
                                    FROM StandardCabinets
									where sc_sg_GUID  = @CabinetGroupId";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@CabinetGroupId", cabinetGroupId);
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            objCabinets.Add(new CabinetDto()
                            {
                                CabinetShortName = objReader["CabinetShortName"].ToString(),
                                CabinetId  =  objReader["CabinetId"].ToString()
                            });
                        }
                    }
                }
            }
            return objCabinets;
        }
        public List<LocationDto> GetCabinetLocations(string cabinetId)
        {
            List<LocationDto> objLocations = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT sl_ShortName LocationShortName,sl_GUID LocationId
                                    FROM StandardCabinetLocations
									where sl_sc_GUID  = @CabinetId";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@CabinetId", cabinetId);
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            objLocations.Add(new LocationDto()
                            {
                                LocationShortName  = objReader["LocationShortName"].ToString(),
                                LocationId  =  objReader["LocationId"].ToString()
                            });
                        }
                    }
                }
            }
            return objLocations;
        }
        public List<PositionDto> GetCabinetPositions(string locationId)
        {
            List<PositionDto> objPositions = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT cp_ShortName PositionShortName,cp_GUID PositionId
                                    FROM StandardCabinetPosition
									where cp_sl_GUID  = @LocationId";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@LocationId", locationId);
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            objPositions.Add(new PositionDto()
                            {
                                PositionShortName  = objReader["PositionShortName"].ToString(),
                                PositionId  =  objReader["PositionId"].ToString()
                            });
                        }
                    }
                }
            }
            return objPositions;
        }
        public string checkResourcesAnomalies(string srId, string spGuid, string newPositionGuid)
        {
            string anomalies = "";
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"select dbo.UMSAllocateResourceCheck(@srId,@spGuid,@newPositionGuid) Anomalies";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@srId", srId);
                    objComm.Parameters.AddWithValue("@spGuid", spGuid);
                    objComm.Parameters.AddWithValue("@newPositionGuid", newPositionGuid);
                    anomalies = objComm.ExecuteScalar().ToString();

                }
            }
            return anomalies;
        }
        public string checkResourcesAnomaliesForRemove(string srId, string spGuid)
        {
            string anomalies = "";
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"select dbo.UMSRemoveResourceCheck(@srId,@spGuid) Anomalies";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@srId", srId);
                    objComm.Parameters.AddWithValue("@spGuid", spGuid);
                    anomalies = objComm.ExecuteScalar().ToString();

                }
            }
            return anomalies;
        }
        public ResourceCabinet checkExistingStockRoomresource(string stockRoomId, string resourceId)
        {
            ResourceCabinet resource = null;
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @" SELECT  TOP 1
										   ss_ShortName StockRoomShortName,
										   sg_ShortName CabinetGroupShortName,
										   sc_ShortName CabinetShortName,
										   sl_ShortName LocationShortName,
										   cp_ShortName PositionShortName,
										   sr_Name ResourceName,
										   sr_Code ResourceCode
											
											FROM 
										    StandardStockrooms ss 
                                            INNER JOIN StandardCabinetGroups g on ss.ss_GUID = g.sg_ss_GUID
                                            INNER JOIN StandardCabinets s on g.sg_GUID = s.sc_sg_GUID
                                            INNER JOIN StandardCabinetLocations l on l.sl_sc_GUID = s.sc_GUID   
											INNER JOIN StandardCabinetPosition p on p.cp_sl_GUID = l.sl_GUID   	         
                                            Inner JOIN StandardPositionContent spc on p.cp_GUID = spc.sp_cp_GUID
											inner join StandardResource sr on sr_ID = spc.sp_sr_ID
											WHERE ss.ss_GUID = @ssGuid and sr_ID = @srId  and sr_Current = 1";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    objComm.Parameters.AddWithValue("@srId", resourceId);
                    objComm.Parameters.AddWithValue("@ssGuid", stockRoomId);
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                          resource =   new ResourceCabinet()
                            {
                                StockRoomShortName = objReader["StockRoomShortName"].ToString(),
                                CabinetGroupShortName = objReader["CabinetGroupShortName"].ToString(),
                                CabinetShortName = objReader["CabinetShortName"].ToString(),
                                LocationShortName = objReader["LocationShortName"].ToString(),
                                PositionShortName = objReader["PositionShortName"].ToString(),
                                ResourceName = objReader["ResourceCode"].ToString() + " " + objReader["ResourceName"].ToString()
                            };
                        }
                    }
                }
            }
            return resource;

        }
        public void MoveResource(string spGuid, string newPositionId)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          UPDATE StandardPositionContent SET sp_cp_GUID = @newPositionId
                                                          WHERE sp_GUID =@spGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@spGuid", spGuid);
                        objCommand.Parameters.AddWithValue("@newPositionId",newPositionId);                       
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to move resource - MoveReource");
                string message = string.Format("Unable to update StandardPosition record in DB");
                throw new Exception(message, ex);
            }
        }
        public void RemoveResource(string spGuid)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          DELETE FROM StandardPositionContent 
                                                          WHERE sp_GUID =@spGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@spGuid", spGuid);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to delete resource - RemoveReource");
                string message = string.Format("Unable to delete StandardPosition record in DB");
                throw new Exception(message, ex);
            }
        }
        public void RemovePosition(string cpGuid)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          DELETE FROM StandardCabinetposition 
                                                          WHERE cp_GUID =@cpGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@cpGuid", cpGuid);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to delete position - RemoveReource");
                string message = string.Format("Unable to delete StandardCabinetPosition record in DB");
                throw new Exception(message, ex);
            }
        }
        public void RemoveLocation(string slGuid)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          DELETE FROM StandardCabinetLocations 
                                                          WHERE sl_GUID =@slGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@slGuid", slGuid);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to delete location - RemoveReource");
                string message = string.Format("Unable to delete StandardCabinetLocations record in DB");
                throw new Exception(message, ex);
            }
        }
        public void RemoveCabinet(string scGuid)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          DELETE FROM StandardCabinets 
                                                          WHERE sc_GUID =@scGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@scGuid", scGuid);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to delete cabinet - RemoveCabinet");
                string message = string.Format("Unable to delete StandardCabinets record in DB");
                throw new Exception(message, ex);
            }
        }
        public void RemoveCabinetGroup(string sgGuid)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          DELETE FROM StandardCabinetGroups 
                                                          WHERE sg_GUID =@sgGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@sgGuid", sgGuid);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to delete cabinetGroup - RemoveCabinet");
                string message = string.Format("Unable to delete StandardCabinetsGroups record in DB");
                throw new Exception(message, ex);
            }
        }
        public void RemoveStockRoom(string ssGuid)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                {
                    objConn.Open();
                    using (var objCommand = new SqlCommand(@"
                                                          DELETE FROM StandardStockrooms 
                                                          WHERE ss_GUID =@ssGuid
                                                            ", objConn))
                    {
                        objCommand.Parameters.AddWithValue("@ssGuid", ssGuid);
                        objCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                mobjLoggerService.ErrorException(ex, "Unable to delete stockRoom - RemoveStockRoom");
                string message = string.Format("Unable to delete StandardStockrooms record in DB");
                throw new Exception(message, ex);
            }
        }


        public List<OperatingBlockMasterDto> GetOperatingBlockMasterDto()
        {
            List<OperatingBlockMasterDto> retval = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT [IDLocation],[LocationName],[LocationIndex],[hu_GUID],[LocationCode],[UniteCode] FROM [Location] ORDER BY [LocationName]";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {                    
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            retval.Add(new OperatingBlockMasterDto()
                            {
                               Code = objReader["LocationCode"].ToString(),
                               Name = objReader["LocationName"].ToString(),
                               HuGuid = objReader["hu_GUID"].ToString(),
                               IDLocation = (int)(objReader["IDLocation"]),
                               Index = (int)(objReader["LocationIndex"]),
                               UniteCode = objReader["UniteCode"].ToString()                               
                            });
                        }
                    }
                }
            }
            return retval;
        }

        public List<OperatingBlockDetailDto> GetOperatingBlockDetailDto(int IDLocation)
        {
            List<OperatingBlockDetailDto> retval = new();
            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
            {
                string strQuery = @"SELECT dbo.OperatingBlocks.ob_ID, dbo.OperatingBlocks.ob_Name, dbo.OperatingBlocks.ob_Description, 
                dbo.OperatingBlocks.ob_ShortName, dbo.OperatingBlocks.ob_IDLocation, dbo.OperatingBlocks.ob_Index,
                dbo.OperatingBlocks.ob_ParentID, dbo.OperatingBlocks.ob_ValidFrom, dbo.OperatingBlocks.ob_ValidTo,
                dbo.OperatingBlocks.ob_ReasonForClosing, dbo.OperatingBlocks.ob_SpecialRequests, dbo.Location.LocationName
                FROM dbo.OperatingBlocks INNER JOIN dbo.Location ON dbo.OperatingBlocks.ob_IDLocation = dbo.Location.IDLocation 
                WHERE ob_IDLocation = " + IDLocation + " ORDER BY [LocationName], dbo.OperatingBlocks.ob_ShortName ";

                objConn.Open();
                using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
                {
                    using (SqlDataReader objReader = objComm.ExecuteReader())
                    {
                        while (objReader.Read())
                        {
                            retval.Add(new OperatingBlockDetailDto()
                            {
                                Id = objReader["ob_ID"].ToString(),
                                IDLocation = (int)objReader["ob_IDLocation"],
                                Index = (int)objReader["ob_Index"],
                                Description = objReader["ob_Description"].ToString(),
                                LocationName = objReader["LocationName"].ToString(),
                                ParentBlock = "",  //TODO
                                Name = objReader["ob_Name"].ToString(),
                                ShortName = objReader["ob_ShortName"].ToString(),
                                ParentID = objReader["ob_ParentID"].ToString(),
                                ReasonForClosing = objReader["ob_ReasonForClosing"].ToString(),
                                SpecialRequests = objReader["ob_SpecialRequests"].ToString(),
                                ValidFrom = (DateTime)objReader["ob_ValidFrom"],
                                ValidTo = objReader.IsDBNull("ob_ValidTo") ? (DateTime?)null : (DateTime?)objReader.GetDateTime("ob_ValidTo")
                            });
                        }
                    }
                }
            }
            return retval;
        }
    }
}
