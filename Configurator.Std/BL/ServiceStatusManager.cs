using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.Dal.Interfaces;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Configurator.Std.BL
{
   public class ServiceStatusManager : DalManagerBase<ServiceStatus>, IServiceStatusManager
   {
      private IMessageCenterManager mobjMsgCtrMgr;

      public ServiceStatusManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrMgr)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrMgr;
      }

      public List<ServiceStatus> GetServiceStatuses()
      {
         return mobjDbContext.Set<ServiceStatus>()
            .OrderBy(a => a.Application)
            .ThenBy(s => s.Status)
            .ThenBy(h => h.Hostname)
            .ToList();
      }

      public void SetActive(string Application, string Host)
      {
         mobjDbContext.BeginTransaction();
         try
         {
            var all = mobjDbContext.Set<ServiceStatus>().Where(x => x.Application == Application).ToList();
            foreach (var item in all)
            {
               if (item.Hostname.ToUpperInvariant() == Host.ToUpperInvariant())
               {
                  item.Status = "HANDOVER";
               }
               else
               {
                  item.Status = "IDLE";
               }
            }

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();
         }
         catch
         {
            //TODO log
            mobjDbContext.RollbackTransaction();            
         }
      }
   }
}
