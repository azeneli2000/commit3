using System;
using System.Linq;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.UMSLegacy;
using Configurator.Std.BL.Hubs;
using System.Text;
using Digistat.FrameworkStd.Extensions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace Configurator.Std.BL
{
   public class HospitalUnitsManager : DalManagerBase<HospitalUnit>, IHospitalUnitsManager
   {
      private readonly IMessageCenterManager mobjMsgCtrMgr;

      public HospitalUnitsManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrMgr)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrMgr;
      }

      public IEnumerable<HospitalUnit> GetList()
      {
         IEnumerable<HospitalUnit> objResult = null;

         try
         {
            objResult = mobjDbContext.Set<HospitalUnit>().Where(x => x.Current).ToList();
         }
         catch(System.Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading HospitalUnits");
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }

         return objResult;
      }

      public HospitalUnit Get(string guid)
      {
         HospitalUnit objResult = null;

         try
         {
            IQueryable<HospitalUnit> repository = mobjDbContext.Set<HospitalUnit>();

            objResult = repository.Where(x => x.GUID == guid && x.Current).FirstOrDefault();
         }
         catch
         {
            //TODO log and throw?
         }

         return objResult;
      }

      #region Data Writing functions

      public new HospitalUnit Create(HospitalUnit hu)
      {
         mobjLoggerService.Info($"Creating new HospitalUnit with name {hu.Name} and shortname {hu.ShortName}");

         try
         {
            mobjDbContext.BeginTransaction();

            //Prevent duplications
            var huRepository = mobjDbContext.Set<HospitalUnit>();

            HospitalUnit objHu = huRepository.Where(x => (x.Name == hu.Name || x.ShortName == hu.ShortName) && x.Current).FirstOrDefault();
            if (objHu != null)
            {
               if (objHu.Name == hu.Name)
               {
                  throw new Exception($"Unable to create HospitalUnit with name : {hu.Name}; HospitalUnit already exists.");
               }

               if (objHu.ShortName == hu.ShortName)
               {
                  throw new Exception($"Unable to create HospitalUnit with short name : {hu.ShortName} ; HospitalUnit already exists.");
               }
            }

            //Set current set as record
            hu.GUID = Guid.NewGuid().ToString();
            hu.Version = 1;
            hu.Current = true;

            //Create new record
            huRepository.Add(hu);

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            // mobjMsgCtrMgr.

            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info($"HospitalUnit with name {hu.Name} and short name {hu.ShortName} succesfully created with guid {hu.GUID}");

            return hu;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error creating HospitalUnit {hu.Name}");
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }

      }

      public new HospitalUnit Update(HospitalUnit hu)
      {

         //TODO Trace
         mobjLoggerService.Info($"Updating new HospitalUnit with name {hu.Name} and shortname {hu.ShortName}");

         try
         {

            mobjDbContext.BeginTransaction();

            var huRepository = mobjDbContext.Set<HospitalUnit>();

            HospitalUnit objHu = huRepository.Where(x => x.GUID == hu.GUID && x.Current == true).FirstOrDefault();
            if (objHu == null)
            {
               throw new Exception($"Unable to update HospitalUnit with guid {hu.GUID}; HospitalUnit not found.");
            }
            if (hu.Version != objHu.Version)
            {
               throw new Exception($"Unable to update HospitalUnit with guid {hu.GUID}; HospitalUnit version ({hu.Version}) is different from expected current version ({objHu.Version}).");
            }

            //Create new record for updated entity
            HospitalUnit newHospitalUnit = hu.CreateUpdatedClone();

            huRepository.Add(newHospitalUnit);

            //Set current record as updated
            objHu.Current = false;
            objHu.ValidToDate = DateTime.Now.DatetimeUMSDBToString();
            mobjDbContext.SaveChanges();

            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info($"HospitalUnit with guid {newHospitalUnit.GUID} updated succesfully");

            return newHospitalUnit;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error updating HospitalUnit with guid {hu.GUID}");
            throw new Exception($"Error updating HospitalUnit with guid {hu.GUID}", e);
         }

      }

      /// <summary>
      /// Disable a user. 
      /// </summary>
      public bool Delete(string GUID, out string errorMessage)
      {
         bool bolDeleted = false;
         errorMessage = "";
         //TODO Trace
         mobjLoggerService.Info($"Deleting HospitalUnit with guid {GUID}");

         try
         {
            var executeClose = mobjDbContext.BeginTransaction();

            string strError = CheckIfHospitalUnitCanBeDeleted(GUID);
            if (string.IsNullOrEmpty(strError))
            {

               var huRepository = mobjDbContext.Set<HospitalUnit>();

               HospitalUnit hu = huRepository.SingleOrDefault(x => x.GUID == GUID && x.Current);
               if (hu == null)
               {
                  throw new Exception($"Unable to delete HospitalUnit with guid {GUID}; user not found.");
               }

               //Set current record as updated
               hu.Current = false;
               hu.ValidToDate = DateTime.Now.DatetimeUMSDBToString();

               mobjDbContext.SaveChanges();
               if (executeClose) { mobjDbContext.CommitTransaction(); }
               bolDeleted = true;
               mobjLoggerService.Info($"HospitalUnit with guid {GUID} has been disabled succesfully");
            }
            else
            {
               mobjLoggerService.Info($"Deleting HospitalUnit with guid {GUID}. Error: {strError}");
               errorMessage = strError;
            }
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error disabling HospitalUnit with guid {GUID}");
            throw new Exception($"Error disabling HospitalUnit with guid {GUID}", e);
         }
         return bolDeleted;
      }

      private string CheckIfHospitalUnitCanBeDeleted(string GUID)
      {
         string strTables = string.Empty;

         IDbConnection objDbConn = mobjDbContext.Database.GetDbConnection();
         using (var cmd = objDbConn.CreateCommand())
         {
            StringBuilder strSql = new StringBuilder($"select 'Location' as TableName Where exists (select 1 from Location Where hu_GUID = '{GUID}')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'CostCenter' as TableName Where exists(select 1 from hu_CC_link Where ln_hu_GUID = '{GUID}')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'User' as TableName Where exists(select 1 from us_hu_link Where ln_hu_GUID = '{GUID}')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'Personnel' as TableName Where exists(select 1 from Personnel Where pe_hu_GUIDs LIKE '%{GUID}%')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'CalendarExceptions' as TableName Where exists(select 1 from CalendarExceptions Where ce_hu_ID = '{GUID}')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'ActualSlot' as TableName Where exists(select 1 from ActualSlot Where as_hu_ID = '{GUID}' and as_Date > '{DateTime.Today.ToDateInternalFullFormat()}')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'ActualOperation' as TableName Where exists(select 1 from ActualOperation Where (ao_hu_ID_Requesting = '{GUID}' OR ao_hu_ID_Hospitalization = '{GUID}'))");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'SlotTemplate' as TableName Where exists(select 1 from SlotTemplate Where st_hu_ID = '{GUID}' and st_ActiveTo > '{DateTime.Today.ToDateInternalFullFormat()}')");
            strSql.AppendLine("UNION");
            strSql.AppendLine($"select 'HospitalUnit' as TableName Where exists(select 1 from HospitalUnit Where hu_ParentID = '{GUID}')");

            cmd.CommandText = strSql.ToString();
            cmd.Transaction = mobjDbContext.Database.CurrentTransaction.GetDbTransaction();
            using (var reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  strTables += reader.GetString(0) + ", ";
               }
               if (!string.IsNullOrEmpty(strTables))
               {
                  strTables = strTables.Substring(0, strTables.Length - 2);
               }
            }
         }

         return strTables;
      }

      #endregion
   }
}
