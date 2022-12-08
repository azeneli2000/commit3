using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Digistat.FrameworkStd.UMSLegacy;

namespace Configurator.Std.BL.ReportMaster
{
   public class ReportTemplateManager : DalManagerBase<ReportTemplate>, IReportTemplateManager
   {
      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;

      public ReportTemplateManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }


      #endregion


      #region Data reading functions


      public ReportTemplate Get(string id)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for ReportTemplate with id {0}", id);

         ReportTemplate result = null;

         try
         {

            //TODO Trace
            mobjLoggerService.Info("Reading ReportTemplate with id {0} from DB", id);
            result = mobjDbContext.Set<ReportTemplate>().Where(x => x.ID == id && x.Current == true).SingleOrDefault();
          

            //TODO Trace                                                                                                                                                  
            mobjLoggerService.Info("ReportTemplate with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ReportTemplate with guid {0} from DB", id);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            throw new Exception(string.Format("Error reading ReportTemplate with guid {0} from DB", id), e);
         }


         return result;
      }
      public IQueryable<ReportTemplate> GetReportTemplates()
      {
         var objReportTemplate = mobjDbContext.Set<ReportTemplate>();
        
         return objReportTemplate.OrderBy(o => o.CreationDate);
      }
      public IQueryable<ReportTemplate> GetReportTemplatesMonitoring()
      {
         IQueryable<ReportTemplate> objReportTemplate = mobjDbContext.Set<ReportTemplate>().Where(x=>x.Application== "SUPERVISION" && x.Module== "SUPERVISION" && x.Current==true && (x.ValidToDate==null|| x.ValidToDate > DateTime.Now));
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
         return objReportTemplate;
      }
      /// <summary>
      /// GetParametersFromTemplateXml
      /// </summary>
      /// <param name="xml"></param>
      /// <returns></returns>
      public Dictionary<string, string> GetParametersFromTemplateXml(string xml)
      {
         Dictionary<string, string> objParameters = new Dictionary<string, string>();

         StringReader objStringReader = new StringReader(xml);
         XmlTextReader objReader = new XmlTextReader(objStringReader);

         while (objReader.Read())
         {
            switch (objReader.NodeType)
            {
               case XmlNodeType.Element:
                  if (objReader.Name.CompareTo("Parameter") == 0)
                  {
                     objParameters.Add(objReader.GetAttribute("Name"), objReader.GetAttribute("DataType")); 
                  }
                  break;
               case XmlNodeType.EndElement:
                  if (objReader.Name.CompareTo("Dictionary") == 0)
                  {
                     break;
                  }
                  break;
               default:
                  break;
            }
         }
         objReader.Close();
         objStringReader.Dispose();

         return objParameters;
      }
      #endregion

      #region Data Writing functions

      public new ReportTemplate Create(ReportTemplate reportTemplate)
      {

         //TODO Trace
         mobjLoggerService.Info("Creating new ReportTemplate {0} ({1})", reportTemplate.Name , reportTemplate.Filename);
         
         try
         {

            mobjDbContext.BeginTransaction();

            var repTemplateRepository = mobjDbContext.Set<ReportTemplate>();

            
            //Prevent duplications
            ReportTemplate loadedRepTemplate = repTemplateRepository.Where(x => x.Filename == reportTemplate.Filename || x.Name == reportTemplate.Name).FirstOrDefault();
            
            if (loadedRepTemplate != null)
            {
               if (loadedRepTemplate.Name == reportTemplate.Name)
               {
                  throw new Exception(string.Format("Unable to create ReportTemplate {0}; ReportTemplate name already exists.", reportTemplate.Name));
               }

               if (loadedRepTemplate.Filename == reportTemplate.Filename)
               {
                  throw new Exception(string.Format("Unable to crate ReportTemplate {0}; ReportTemplate name {1} already exists.", reportTemplate.Name, reportTemplate.Filename));
               }
            }

            //Set current set as record
            reportTemplate.ID = Guid.NewGuid().ToString();
            reportTemplate.Version = 1;
            reportTemplate.CreationDate=DateTime.Now;
            reportTemplate.Current = true;

            //Create new record
            repTemplateRepository.Add(reportTemplate);

            mobjDbContext.SaveChanges();

            //Send notification to Message Center

         //   mobjMsgCtrMgr.SendUserEdited(reportTemplate.Id);

            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Report Template with {0} succesfully created with id {1}", reportTemplate.Name, reportTemplate.ID);

            return reportTemplate;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error creating report template {0}", reportTemplate.Name);
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }

      }

      public new ReportTemplate Update(ReportTemplate repTemplate)
      {

         //TODO Trace
         mobjLoggerService.Info("Updating report template with id {0} and name {1}", repTemplate.ID, repTemplate.Name);

         try
         {
            var executeClose = mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<ReportTemplate>(); 

            ReportTemplate loadedRepTemplate = repository.FirstOrDefault(x => x.ID == repTemplate.ID && x.Current == true);

            if (loadedRepTemplate == null)
            {
               throw new Exception(string.Format("Unable to update report template with id {0}; user not found.", repTemplate.ID));
            }
            if (repTemplate.Version != loadedRepTemplate.Version)
            {
               throw new Exception(string.Format("Unable to update report template with id {0}; version ({1}) is different from expected current version ({2}).", repTemplate.ID, loadedRepTemplate.Version, repTemplate.Version));
            }

            //Create new record for updated entity
            ReportTemplate newReportTemplate = repTemplate.CreateUpdatedClone();
            repository.Add(newReportTemplate);

            loadedRepTemplate.Current = false;
            loadedRepTemplate.ValidToDate = DateTime.Now; 

           // repository.Update(repTemplate);
            mobjDbContext.SaveChanges();

            mobjMsgCtrMgr.SendReportTemplateEdited(newReportTemplate);
           
            if (executeClose) { mobjDbContext.CommitTransaction(); }

            //TODO Trace
            mobjLoggerService.Info("User with id {0} updated succesfully", repTemplate.ID);
            return repTemplate;
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error updating report template with id {0}", repTemplate.ID);
            string message = string.Format("Error updating report template with id {0}", repTemplate.ID);
            throw new Exception(message, e);
         }

      }

      /// <summary>
      /// Delete a ReportTemplate. 
      /// </summary>
      public void Remove(string id)
      {
         //TODO Trace
         mobjLoggerService.Info("Disabling Report Template with id {0}", id);

         try
         {

            mobjDbContext.BeginTransaction();

            var reportRepository = mobjDbContext.Set<ReportTemplate>();

            ReportTemplate report = reportRepository.SingleOrDefault(x => x.ID == id && x.Current);
            if (report == null)
            {
               throw new Exception(string.Format("Unable to disable report template with id {0}; report template not found.", id));
            }

            //Create new record for updated entity
            //ReportTemplate newReport = report.CreateUpdatedClone();
            report.Current = false;
            report.ValidToDate = DateTime.Now;
            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendReportTemplateEdited(report);
            mobjDbContext.CommitTransaction();
            //TODO Trace
            mobjLoggerService.Info("Report template with {0} disabled succesfully", id);
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error disabling report template with id {0}", id);
            string message = string.Format("Error disabling report template with id {0}", id);
            throw new Exception(message, e);
         }
      }

   


      #endregion



   }
}
