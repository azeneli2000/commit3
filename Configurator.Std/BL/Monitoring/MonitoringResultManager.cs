using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Configurator.Std.BL.Monitoring
{
   public class MonitoringResultManager : DalManagerBase<MonitoringResult>, IMonitoringResultManager
   {
      public MonitoringResultManager(DigistatDBContext context, ILoggerService logSvc)
      {
         mobjDbContext = context;
         mobjLoggerService = logSvc;
      }


      public IQueryable<MonitoringResult> GetLastMonitoringResults(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<MonitoringResult, object>>> includePredicates = null)
      {
         try
         {
            mobjLoggerService.Info("Executing GetLastMonitoringResults");
            DateTime filter = DateTime.UtcNow.AddHours(-1);
            //var ret = mobjDbContext.Set<MonitoringResult>().OrderByDescending(a => a.ID);
            var ret = mobjDbContext.Set<MonitoringResult>().OrderByDescending(a => a.ID);
            IQueryable<MonitoringResult> result = from l in ret.AsQueryable()
                                                  select new MonitoringResult
                                                  {
                                                     ID = l.ID,
                                                     mre_ResultTimeUTC = l.mre_ResultTimeUTC,
                                                     mre_Value = l.mre_Value
                                                  };
            return result;

         }
         catch (Exception e)
         {
            string message = string.Format("Error on retrieve data from MonitoringResult");
            mobjLoggerService.ErrorException(e, message);
         }

         return null;
      }

      public IQueryable<MonitoringResult> GetLastMonitoringResultsCheckFilter(bool value)
      {
         try
         {
            mobjLoggerService.Info("Executing GetLastMonitoringResults");
            //DateTime filter = DateTime.UtcNow.AddHours(-1);
            IQueryable<MonitoringResult> ret = null ; 
            if (value)
            {
             
                ret = mobjDbContext.Set<MonitoringResult>().OrderByDescending(a => a.ID).Where(x=> x.mre_Value !="");
   
            }
            else
            {
                ret = mobjDbContext.Set<MonitoringResult>().OrderByDescending(a => a.ID);

            }
            //IQueryable<MonitoringResult> result = from l in ret.AsQueryable()
            //                                      select new MonitoringResult
            //                                      {
            //                                         ID = l.ID,
            //                                         mre_ResultTimeUTC = l.mre_ResultTimeUTC,
            //                                         mre_Value = l.mre_Value
            //                                      };
            return ret;

         }
         catch (Exception e)
         {
            string message = string.Format("Error on retrieve data from MonitoringResult");
            mobjLoggerService.ErrorException(e, message);
         }

         return null;
      }
   }
}
