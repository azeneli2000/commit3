using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Export;
using Digistat.FrameworkStd.Model.Online;
using Digistat.FrameworkStd.UMSLegacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
//using NPOI.SS.Formula.Functions;

namespace Configurator.Std.BL.ExportScheduler
{
   public class ExportJobsManager : DalManagerBase<ExportJobs>, IExportJobsManager
   {
      private readonly IDigistatConfiguration mobjDigConfig = null;
      protected readonly IMessageCenterManager mobjMsgCtrMgr;


      public ExportJobsManager(
         DigistatDBContext context
         , IDigistatConfiguration config
         , IMessageCenterManager msgCtrSvc
         , ILoggerService logSvc
         //,ISynchronizationService syncSvc
         )
      {
         mobjDbContext = context;
         mobjMsgCtrMgr = msgCtrSvc;
         mobjLoggerService = logSvc;
         mobjDigConfig = config;
      }

      public List<ExportJobs> GetAll(bool includeDeleted= false)
      {
         var listExportFile = new List<ExportJobs>();

         mobjLoggerService.Trace("Executing GetAll for ExportJobs");

         try
         {
            //Set detached
            if (includeDeleted)
               listExportFile = mobjDbContext.Set<ExportJobs>().ToList();
            else
               listExportFile = mobjDbContext.Set<ExportJobs>().Where(r=>r.exj_Status>=0).ToList();

            //TODO Trace
            mobjLoggerService.Trace("Reading ExportJobs List from DB");

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ExportJobs List from DB");
            throw new Exception("Error reading ExportJobs List from DB");
         }

         finally
         {
            //disposeContext();
         }


         return listExportFile;
      }

      public ExportJobs GetById(int id)
      {
         ExportJobs job = mobjDbContext.Set<ExportJobs>().FirstOrDefault(_=>_.exj_ID==id);;
         if (!String.IsNullOrWhiteSpace(job?.exj_FileSystemPassword))
         {
            job.exj_FileSystemPassword = UMSFrameworkParser.Decrypt(UMSFrameworkParser.Base64Decode(job.exj_FileSystemPassword), string.Empty)??"";
         }
         return job;
      }

      public List<ExportJobsHistory> GetHistoryById(int jobId)
      {
         List<ExportJobsHistory> job = mobjDbContext.Set<ExportJobsHistory>().Where(_=>_.exj_ID==jobId).OrderByDescending(p=>p.exjh_StartDateTime).Take(100).ToList();

         return job;
      }

      #region Data Write


     
      public bool Create(ExportJobs job,string usrAbbrev)
      {
         bool bolRet = false;
         try
         {
            if (!String.IsNullOrWhiteSpace(job.exj_FileSystemPassword))
            {
               job.exj_FileSystemPassword = UMSFrameworkParser.Base64Encode(UMSFrameworkParser.Encrypt(job.exj_FileSystemPassword,string.Empty));
            }
            mobjDbContext.BeginTransaction();
            var objExportJob = mobjDbContext.Set<ExportJobs>();
            

            objExportJob.Add(job);
            mobjDbContext.SaveChanges();

            //Send Message
            mobjMsgCtrMgr.SendExportJobCreated(job.exj_ID);

            mobjDbContext.CommitTransaction();

            

            string strSerialized = JsonConvert.SerializeObject(job);
            mobjLoggerService.WriteClinicalLog(100, $"Export Job CREATED. Value: {strSerialized}", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,
               usrAbbrev, 0, mobjDigConfig.ModuleName, string.Empty, "UMS");
            bolRet = true;
         }
         catch (Exception ex)
         {

            mobjDbContext.RollbackTransaction();
            string errMsg = "Error on ExportJobManager - Create";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }


      public bool Update(ExportJobs job,string usrAbbrev)
      {
         bool bolRet = false;
         try
         {
            if (!String.IsNullOrWhiteSpace(job.exj_FileSystemPassword))
            {
               job.exj_FileSystemPassword = UMSFrameworkParser.Base64Encode(UMSFrameworkParser.Encrypt(job.exj_FileSystemPassword,string.Empty));
            }
            mobjDbContext.BeginTransaction();
            var objExportJob = mobjDbContext.Set<ExportJobs>();
           

            objExportJob.Update(job);
            mobjDbContext.SaveChanges();

            //Send Message
            mobjMsgCtrMgr.SendExportJobEdited(job.exj_ID);

            mobjDbContext.CommitTransaction();

            
            string strSerialized = JsonConvert.SerializeObject(job);
            mobjLoggerService.WriteClinicalLog(100, $"Export Job {job.exj_ID} UPDATED. Value: {strSerialized}", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,
               usrAbbrev, 0, mobjDigConfig.ModuleName, string.Empty, "UMS");
            bolRet = true;

         }
         catch (Exception ex)
         {

            mobjDbContext.RollbackTransaction();
            string errMsg = $"Error on ExportJobManager for group {job.exj_ID} - Update";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }



      public bool Delete(int jobID,string usrAbbrev,string usrID)
      {
         bool bolRet = false;
         try
         {
            
            var objExportJob = mobjDbContext.Set<ExportJobs>();

            ExportJobs job =  objExportJob.Where(p => p.exj_ID == jobID).FirstOrDefault();
            if (job != null)
            {
               mobjDbContext.BeginTransaction();
               job.exj_Status = -1;
               job.exj_LastUpdate = DateTime.Now;
               job.exj_UserID = usrID;
               objExportJob.Update(job);
               //TODO: Send Message
               
               mobjDbContext.SaveChanges();

               //Send Message
               mobjMsgCtrMgr.SendExportJobDeleted(job.exj_ID);

               mobjDbContext.CommitTransaction();

               mobjLoggerService.WriteClinicalLog(100, $"Export Job {job.exj_ID} DELETED", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,
                  usrAbbrev, 0, mobjDigConfig.ModuleName, string.Empty, "UMS");
               bolRet = true;
            }
            else
            {
               throw new Exception($"No ExportJob found with ID {jobID}");
            }

         }
         catch (Exception ex)
         {
            try
            {
               mobjDbContext.RollbackTransaction();
            }
            catch (Exception) { }
            string errMsg = $"Error on ExportJobManager for group {jobID} - Delete";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }


      #endregion

      public string TestQuery(string query)
      {
         object test = null;
         DataTable table = new DataTable();
         var cnn = (SqlConnection) mobjDbContext.Database.GetDbConnection();
         try
         {
            if (cnn.State != ConnectionState.Open)
            {
               cnn.Open();
            }
            
            using (var cmd = new SqlCommand("set PARSEONLY  on " + query + ";set PARSEONLY  off;", cnn))
               cmd.ExecuteNonQuery();

         }
         catch (Exception e)
         {
            if (cnn.State == ConnectionState.Open)
            {
               cnn.Close();
            }
            throw;
         }
         try
         {
            if (cnn.State != ConnectionState.Open)
            {
               cnn.Open();
            }
            using (var cmd = new SqlCommand("BEGIN TRAN;" + query + ";ROLLBACK;", cnn))
            //using (var cmd = new SqlCommand(""query , cnn))
            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                  table.Load(rdr);

         }
         catch (Exception e)
         {
            if (cnn.State == ConnectionState.Open)
            {
               cnn.Close();
            }
            throw;
         }

         if (!table.Columns.Contains("PdfID"))
         {
            
            throw new Exception("The mandatory field 'PdfID' is not found");
         }

         return JsonConvert.SerializeObject(table);
      }

   }
}
