using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Microsoft.EntityFrameworkCore;

namespace Configurator.Std.BL
{
   public class ClinicalLogQueueManager : DalManagerBase<ClinicalLogQueueManager>, IClinicalLogQueueManager
   {
      public ClinicalLogQueueManager(DigistatDBContext context, ILoggerService loggerService)
      {
         //mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      public bool TableExists()
      {
         try
         {
            var result = mobjDbContext.Set<Digistat.FrameworkStd.Model.ClinicalLogQueue>().Take(1);
         }
         catch
         {
            //Console.WriteLine(e);
            return false;
         }

         
         return true;
      }

      public IQueryable<Digistat.FrameworkStd.Model.ClinicalLogQueue> GetClinicalLogs()
      {
         IQueryable<Digistat.FrameworkStd.Model.ClinicalLogQueue> result = null;
         try
         {
            result = mobjDbContext.Set<Digistat.FrameworkStd.Model.ClinicalLogQueue>();
         }
         //Pain and Fear
         catch
         {
         }

         return result;
      }

      public IEnumerable<string> GetDistinctClinicalLogsPriority()
      {
         IEnumerable<String> result = null;
         try
         {
            result = mobjDbContext.Set<ClinicalLogQueue>().AsNoTracking().GroupBy(f=>f.Priority).Select(x=> x.Key ).ToList();
         }
         //Pain and Fear
         catch
         {
         }

         return result;
      }

      public IEnumerable<string> GetDistinctClinicalLogsTask()
      {
         IEnumerable<String> result = null;
         try
         {
            result = mobjDbContext.Set<ClinicalLogQueue>().AsNoTracking().GroupBy(f=>f.Task).Select(x=> x.Key ).ToList();
         }
         //Pain and Fear
         catch
         {
         }

         return result;
      }
   }
}
