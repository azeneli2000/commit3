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
   public class DigistatLogManager : DalManagerBase<DigistatLogManager>, IDigistatLogManager
   {
      //private IDigistatLogManager _digistatLogManagerImplementation;

      public DigistatLogManager(DigistatDBContext context, ILoggerService loggerService)
      {
         //mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      public bool TableExists()
      {
         try
         {
            var result = mobjDbContext.Set<Digistat.FrameworkStd.Model.Log>().Take(1);
         }
         catch
         {
            //Console.WriteLine(e);
            return false;
         }

         
         return true;
      }

      public IQueryable<Digistat.FrameworkStd.Model.Log> GetDigistatLogs()
      {
         IQueryable<Digistat.FrameworkStd.Model.Log> result = null;
         try
         {
            result = mobjDbContext.Set<Digistat.FrameworkStd.Model.Log>();
         }
         //Pain and Fear
         catch
         {
         }

         return result;
      }

      public IEnumerable<string> GetDistinctDigistatLogsPriority()
      {
         IEnumerable<String> result = null;
         try
         {
            result = mobjDbContext.Set<Log>().AsNoTracking().GroupBy(f=>f.Priority).Select(x=> x.Key ).ToList();
         }
         //Pain and Fear
         catch
         {
         }

         return result;
      }

      public IEnumerable<string> GetDistinctDigistatLogsTask()
      {
         IEnumerable<String> result = null;
         try
         {
            result = mobjDbContext.Set<Log>().AsNoTracking().GroupBy(f=>f.Task).Select(x=> x.Key ).ToList();
         }
         //Pain and Fear
         catch
         {
         }

         return result;
      }
   }
}
