using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using Microsoft.EntityFrameworkCore;

namespace Configurator.Std.BL.Monitoring
{
   public class CurrentSystemErrorStatusManager : /*DalManagerBase<CurrentSystemErrorStatus>,*/ ICurrentSystemErrorStatusManager
   {
      protected DigistatDBContext mobjDbContext;
      protected ILoggerService mobjLoggerService;
      public CurrentSystemErrorStatusManager(DigistatDBContext context, ILoggerService logSvc)
      {
         mobjDbContext = context;
         mobjLoggerService = logSvc;
      }
      /// <summary>
      /// Return last valid (cse_UpdateDateTime is in last 60 second) status.
      /// </summary>
      /// <returns></returns>
      public CurrentSystemErrorStatus GetLastValidStatus()
      {
         try
         {
            mobjLoggerService.Info("Executing GetLastStatus");

            var ret = mobjDbContext.Set<CurrentSystemErrorStatus>().FromSqlRaw(
               $"SELECT TOP 1 [cse_SystemStatus], [cse_UpdateDateTimeUTC] FROM [CurrentSystemErrorStatus] where [cse_UpdateDateTimeUTC] >  DATEADD(MINUTE,-1 ,GETUTCDATE()) ORDER BY cse_UpdateDateTimeUTC DESC"
               ).FirstOrDefault();

            return ret;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error on GetLastStatus");
            throw;
         }


         return null;
      }
      /// <summary>
      /// Return last status if present
      /// </summary>
      /// <returns></returns>
      public CurrentSystemErrorStatus GetLastStatus()
      {
         try
         {
            mobjLoggerService.Info("Executing GetLastStatus");

            var ret = mobjDbContext.Set<CurrentSystemErrorStatus>().OrderByDescending(o=> o.UpdateDateTime).FirstOrDefault();

            return ret;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error on GetLastStatus");
         }

         return null;
      }
   }
}
