using Configurator.Std.BL.Configurator;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.Monitoring
{
   public class MonitoringRawRequestManager : DalManagerBase<MonitoringRawRequest>, IMonitoringRawRequestManager
   {
      public MonitoringRawRequestManager(DigistatDBContext context, ILoggerService logSvc)
      {
         mobjDbContext = context;
         mobjLoggerService = logSvc;
      }

      public MonitoringRawRequest[] GetLastRequests(MonitoringData.MonitoringType enuType)
      {
         try
         {
            mobjLoggerService.Info("Executing GetLastRequests");

            var ret = mobjDbContext.Set<MonitoringRawRequest>().Where(a=>a.mrq_Type == (int)enuType).OrderByDescending(a => a.mrq_id).Take(2).ToArray();

            return ret;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error on GetLastRequests");
         }

         return null;
      }
      public MonitoringRawRequest GetLastRequest(MonitoringData.MonitoringType enuType)
      {
         try
         {
            mobjLoggerService.Info("Executing GetLastRequests");

            var ret = mobjDbContext.Set<MonitoringRawRequest>().OrderByDescending(a => a.mrq_id).FirstOrDefault(a=>a.mrq_Type == (int)enuType);

            return ret;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error on GetLastRequests");
         }

         return null;
      }
   }
}
