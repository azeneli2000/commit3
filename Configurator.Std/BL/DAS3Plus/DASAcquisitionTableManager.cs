using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Configurator.Std.Exceptions;
using Configurator.Std.Helpers;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace Configurator.Std.BL.DAS3Plus
{
   public class DASAcquisitionTableManager : Digistat.Dal.Data.DalManagerBase<DASAcquisitionTableManager>
   {


      //Digistat.Dal.Data.DigistatDBContext mobjContext;
      protected readonly IDigistatConfiguration mobjConfig;

      public DASAcquisitionTableManager(DigistatDBContext context, IDigistatConfiguration digConfig, ILoggerService logSvc)
      {
         mobjConfig = digConfig;
         mobjDbContext = context;
         mobjLoggerService = logSvc;
      }


      //public List<string> GetParamToBeSaved(string drv_ID)
      //{
      //   List<string> objRet = new List<string>();
      //   string strSql = @"select dp.par_ID from DASAcqTable_ParametersLink dp
      //      inner join DASAcqTable dt on dp.acq_ID = dt.ID and dp.acq_Version = dt.Version
      //      where drv_id = '@driverID' and dt.IsCurrent = 1";
      //   IDbConnection objDbConn = mobjDbContext.Database.GetDbConnection();
      //   var objCommand = objDbConn.CreateCommand();
      //   objCommand.Transaction = mobjDbContext.Database.CurrentTransaction.GetDbTransaction();
      //   SqlParameter objPar = new SqlParameter("@driverID", drv_ID);
      //   objCommand.Parameters.Add(objPar);
      //   objCommand.CommandText = strSql;
      //   using (IDataReader objReader = objCommand.ExecuteReader())
      //   {
      //      while(objReader.Read())
      //      {
      //         objRet.Add(objReader[0].ToString());
      //      }
      //   }
      //   objCommand.Dispose();
      //   return objRet;
      //}

      /// <summary>
      /// Create an acquisition table if it doesn't exists. Otherwise an exception will be thrown
      /// </summary>
      /// <param name="strTblName">Acquisition table name</param>
      /// <param name="tblSuffix">Acquisition table suffix (RAW or SPR)</param>
      /// <param name="strTblVersion">Acquisition table version</param>
      /// <param name="capabilities">DriverRepositoryStandardParameterLink collection</param>
      /// <returns></returns>
      private bool CheckAndCreateTable(string strTblName, string tblSuffix, string strTblVersion, List<DriverRepositoryStandardParameterLink> capabilities)
      {
         bool bolRet = false;
         string strSqlCheckTbl = $@"SELECT count(*) AS TBLCOUNTER 
                        FROM information_schema.tables
                        WHERE table_schema = 'dbo' 
                        AND table_name = '{strTblName + tblSuffix + strTblVersion}'";
         IDbConnection objDbConn = mobjDbContext.Database.GetDbConnection();
         var objCommand = objDbConn.CreateCommand();
         objCommand.Transaction = mobjDbContext.Database.CurrentTransaction.GetDbTransaction();
         objCommand.CommandText = strSqlCheckTbl;
         int intRet = (int)objCommand.ExecuteScalar();
         objCommand.Dispose();
         if (intRet == 0)
         {
            string strSqlCreateTble = GetTableCreationScript(strTblName, tblSuffix, strTblVersion, capabilities);
            var objCommandCreate = objDbConn.CreateCommand();
            objCommandCreate.Transaction = mobjDbContext.Database.CurrentTransaction.GetDbTransaction();
            objCommandCreate.CommandText = strSqlCreateTble;
            objCommandCreate.ExecuteNonQuery();
            objCommandCreate.Dispose();
         }
         else
         {
            
            throw new ConnectPlusException($"Table {strTblName + tblSuffix + strTblVersion} already exists");
         }
         return bolRet;
      }

      /// <summary>
      /// Update DASAcqTable_ParametersLink
      /// </summary>
      /// <param name="acqTableID">Acquisition table ID</param>
      /// <param name="acqTableVersion">Acquisition table version</param>
      /// <param name="objList">DriverRepositoryStandardParameterLink collection</param>
      /// <returns></returns>
      private bool UpdateParameterLink(string acqTableID, int acqTableVersion, List<DriverRepositoryStandardParameterLink> objList)
      {
         bool bolRet = false;
         StringBuilder objBuilder = new StringBuilder();
         if (objList.Count > 0)
         {
            foreach (DriverRepositoryStandardParameterLink objSplink in objList)
            {
               //If there is an alias I write two rows : one with alias = '' , other with alias not empty
               string strAlias = objSplink.StandardParameterIdAlias == null ? string.Empty : objSplink.StandardParameterIdAlias;
               if (string.IsNullOrEmpty(strAlias))
               {
                  objBuilder.Append("IF NOT EXISTS(Select acq_ID from [DASAcqTable_ParametersLink] WHERE acq_ID = '" +
                                    acqTableID + "' and acq_Version =" + acqTableVersion.ToString() +
                                    " and par_ID = '" + objSplink.StandardParameterId.ToString() + "' AND par_ID_Alias = '') ");
                  objBuilder.Append(
                     @"INSERT INTO [dbo].[DASAcqTable_ParametersLink] ([acq_ID],[acq_Version],[par_ID],[par_ID_Alias]) VALUES ('" +
                     acqTableID + "'," + acqTableVersion.ToString() + ",'" + objSplink.StandardParameterId.ToString() + "','');");
               }
               else
               //if (!string.IsNullOrEmpty(strAlias))
               {
                  objBuilder.Append("IF NOT EXISTS(Select acq_ID from [DASAcqTable_ParametersLink] WHERE acq_ID = '" + acqTableID + "' and acq_Version =" + acqTableVersion.ToString() + " and par_ID = '" + objSplink.StandardParameterId.ToString() + "' AND par_ID_Alias = '" + strAlias + "') ");
                  objBuilder.Append(@"INSERT INTO [dbo].[DASAcqTable_ParametersLink] ([acq_ID],[acq_Version],[par_ID],[par_ID_Alias]) VALUES ('" + acqTableID + "'," + acqTableVersion.ToString() + ",'" + objSplink.StandardParameterId.ToString() + "','" + strAlias + "');");
               }
               
            }
            IDbConnection objDbConn = mobjDbContext.Database.GetDbConnection();
            var objCommand = objDbConn.CreateCommand();
            objCommand.Transaction = mobjDbContext.Database.CurrentTransaction.GetDbTransaction();
            objCommand.CommandText = objBuilder.ToString();
            objCommand.ExecuteNonQuery();
            objCommand.Dispose();
         }

         return bolRet;
      }


      /// <summary>
      /// Create SQL script for creating a new Table
      /// </summary>
      /// <param name="strTblName">Acquisition Table name</param>
      /// <param name="tblSuffix">Suffix (SPR or RAW)</param>
      /// <param name="strTblVersion">Acquisition table version</param>
      /// <param name="objList">DriverRepositoryStandardParameterLink collection</param>
      /// <returns></returns>
      private string GetTableCreationScript(string strTblName, string tblSuffix, string strTblVersion, List<DriverRepositoryStandardParameterLink> objList)
      {
         StringBuilder objBuilder = new StringBuilder();
         string strRet = string.Empty;
         objBuilder.Append($@"CREATE TABLE [dbo].[{strTblName + tblSuffix + strTblVersion}] (
            [ClinicalTimeUTC][datetime] NOT NULL,
            [ActualDevice] [int] NOT NULL,
            [BedId] [int] NOT NULL,
            [PatientId] [int] NOT NULL,
            [Channel] [int] NULL, ");
         foreach (DriverRepositoryStandardParameterLink objDrvParLink in objList)
         {
            if (objDrvParLink.StandardParameter == null || objDrvParLink.StandardParameter.DataType==null)
            {
               continue;
            }
            int intParamType = ConnectPlusHelper.GetParamType(objDrvParLink.StandardParameter.DataType);
            string strColName = objDrvParLink.StandardParameterId.ToString();
            if(!string.IsNullOrEmpty(objDrvParLink.StandardParameterIdAlias))
            {
               strColName = objDrvParLink.StandardParameterIdAlias;
            }
            if (intParamType > 0)
            {
               if (intParamType == 1)
               {
                  objBuilder.Append("[" + strColName + "] decimal(19,6) NULL,");
               }
               else
               {
                  objBuilder.Append("[" + strColName + "] int NULL,");
               }
               objBuilder.Append("[" + strColName + "_U] [char](5) NULL, [" + strColName + "_S] [smallint] NULL,");
            }
            //Else not needed. No other parameter types must be handled
         }
         objBuilder.Append($@"CONSTRAINT[PK_{strTblName}_{tblSuffix}_{strTblVersion}] PRIMARY KEY CLUSTERED
         (
           [PatientId] ASC,
           [ClinicalTimeUTC] ASC,
           [BedId] ASC,  
           [ActualDevice] ASC
         )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
         ) ON[PRIMARY]");
         return objBuilder.ToString();

      }


      public List<(int, string)>  GetAcquisitionParameter(string driverID)
      {
         List<DASAcqTable_ParametersLink> objRet = new List<DASAcqTable_ParametersLink>();
         var objRepo = mobjDbContext.Set<DASAcquisitionTable>();
         var objRepoLink = mobjDbContext.Set<DASAcqTable_ParametersLink>();

         var objQ = objRepo
            .Join(objRepoLink, a => new {p1 = a.Id, p2 = a.Version},
               p => new {p1 = p.AcqTableID, p2 = p.AcqTableVersion}, (a, p) => new {a, p})
            .Where(@t => (@t.a.DriverID == driverID && @t.a.Current))
            .Select(@t => new {ParameterID=@t.p.ParameterID,ParameterAlias= @t.p.ParameterAlias});
         List<(int, string)>  ret= new List<(int, string)> ();
         foreach (var item in objQ)
         {
            ret.Add((item.ParameterID,item.ParameterAlias));
         }
         return ret;

      }


      /// <summary>
      /// Check if a new Acquisition tabel must be created. This method scans for changes in capabilities list.
      /// </summary>
      /// <param name="drv"></param>
      /// <returns>Check result.</returns>
      public bool CheckNewTableMustBeCreated(DriverRepository drv)
      {
         bool bolRet = false;
         var objRepo = mobjDbContext.Set<DASAcquisitionTable>();
         var objRepoLink = mobjDbContext.Set<DASAcqTable_ParametersLink>();
         //Get current DASAcquisitionTable record
         DASAcquisitionTable objDasAcq = objRepo.Where(p => p.DriverID == drv.Id && p.Current).FirstOrDefault();
         if (objDasAcq != null)
         {
            List<DriverRepositoryStandardParameterLink> objParamList = drv.Capabilities.Where(p => p.MustBeSaved).ToList();
            List<DASAcqTable_ParametersLink> objParamListExisting = objRepoLink.Where(p => p.AcqTableID == objDasAcq.Id && p.AcqTableVersion == objDasAcq.Version).ToList();

            foreach (DriverRepositoryStandardParameterLink objPar in objParamList)
            {
               if (objParamListExisting.Where(p => p.ParameterID == objPar.StandardParameterId).Count() == 0)
               {
                  bolRet = true;
                  break;
               }
            }
          

         }
         else
         {
            bolRet = true;
         }



         return bolRet;
      }


      /// <summary>
      /// Create a new entry in DASAcqTable; create SPR and RAW tables; update DASAcqTable_ParametersLink
      /// </summary>
      /// <param name="drv"></param>
      public bool CreateNewDASAcquisitionTable(DriverRepository drv)
      {
         bool bolret = false;
         bool bolIsolatedTransaction = false;
         try
         {
            
            if (mobjDbContext.Database.CurrentTransaction == null)
            {
               bolIsolatedTransaction = true;
               mobjDbContext.Database.BeginTransaction();
            }
            
            int intCurrVersion = 0;
            int intNextVersion = 1;
            var objRepo = mobjDbContext.Set<DASAcquisitionTable>();
            //Get current records , one for each type (currently RAW and SPR)
            DASAcquisitionTable objDasAcqRAW = objRepo.Where(p => p.DriverID == drv.Id && p.Current && p.Name.EndsWith("_RAW")).FirstOrDefault();
            DASAcquisitionTable objDasAcqSPR = objRepo.Where(p => p.DriverID == drv.Id && p.Current && p.Name.EndsWith("_SPR")).FirstOrDefault();
            if (objDasAcqRAW != null)
            {
               intCurrVersion = objDasAcqRAW.Version;
            }


            //Calculate version
            intNextVersion = intCurrVersion + 1;
            string strTblVersion = "#" + intNextVersion.ToString();

            //Get Table Name
            string strTableName = ConnectPlusHelper.GetTableName(drv);

            string strTableGUIDSPR = System.Guid.NewGuid().ToString();
            string strTableGUIDRAW = System.Guid.NewGuid().ToString();
            if (objDasAcqRAW != null)
            {
               strTableGUIDSPR = objDasAcqSPR.Id;
               strTableGUIDRAW = objDasAcqRAW.Id;
            }

            //Retrieve parameters to be saved
            List<DriverRepositoryStandardParameterLink> objListCapabilities = drv.Capabilities.Where(p => p.MustBeSaved).ToList();

            //Update parameter link and create RAW and SPR tables
            UpdateParameterLink(strTableGUIDRAW, intNextVersion, objListCapabilities.Where(p => p.Sporadic == 0).ToList());
            UpdateParameterLink(strTableGUIDSPR, intNextVersion, objListCapabilities.Where(p => p.Sporadic != 0).ToList());

            CheckAndCreateTable(strTableName, "_RAW", strTblVersion, objListCapabilities.Where(p => p.Sporadic == 0).ToList());
            CheckAndCreateTable(strTableName, "_SPR", strTblVersion, objListCapabilities.Where(p => p.Sporadic != 0).ToList());
            mobjDbContext.SaveChanges();

            //Create new record in DASAcquisitionTable for _RAW
            DASAcquisitionTable objNewAcqRAW = new DASAcquisitionTable();
            objNewAcqRAW.Id = strTableGUIDRAW;
            objNewAcqRAW.Name = strTableName + "_RAW";
            objNewAcqRAW.Version = intNextVersion;
            objNewAcqRAW.ValidFromDate = DateTime.UtcNow;
            objNewAcqRAW.Current = true;
            objNewAcqRAW.DriverID = drv.Id;
            //Update old record (if exists)
            if (objDasAcqRAW != null)
            {
               objDasAcqRAW.Current = false;
               objDasAcqRAW.ValidToDate = DateTime.UtcNow;
            }
            objRepo.Add(objNewAcqRAW);
            //mobjDbContext.SaveChanges();

            //Create new record in DASAcquisitionTable for _SPR
            DASAcquisitionTable objNewAcqSPR = new DASAcquisitionTable();
            objNewAcqSPR.Id = strTableGUIDSPR;
            objNewAcqSPR.Name = strTableName + "_SPR";
            objNewAcqSPR.Version = intNextVersion;
            objNewAcqSPR.ValidFromDate = DateTime.UtcNow;
            objNewAcqSPR.Current = true;
            objNewAcqSPR.DriverID = drv.Id;
            //Update old record (if exists)
            if (objDasAcqSPR != null)
            {
               objDasAcqSPR.Current = false;
               objDasAcqSPR.ValidToDate = DateTime.UtcNow;
            }
            objRepo.Add(objNewAcqSPR);
            mobjDbContext.SaveChanges();




            //Commit all changes
            if (bolIsolatedTransaction)
            {
               mobjDbContext.Database.CommitTransaction();
            }
            
            string infoMsg = $"DAS3+: Table {strTableName} Version {strTblVersion} created for driver id {drv.Id} version {drv.Version}";
            mobjLoggerService.Info(infoMsg);
         }
         catch (Exception ex)
         {
            string errMsg = $"Error on CreateNewDASAcquisitionTable for driver id {drv.Id} version {drv.Version}";
            if (mobjDbContext.Database.CurrentTransaction != null && bolIsolatedTransaction)
            {
               mobjDbContext.Database.RollbackTransaction();
            }
            
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolret;
      }
   }
}