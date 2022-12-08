using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Interfaces;

namespace ConfiguratorWeb.App.Filters
{
   public class DigConfigFilter : IActionFilter
   {

      private readonly ISynchronizationService mobjSyncSvc;
      private readonly ISystemOptionsService mobjSysOptSvc;
      private readonly IDnsCacherService mobjDnsSvc;
      private readonly IHttpContextAccessor mobjHttpContext;

      public DigConfigFilter(ISynchronizationService syncSvc, ISystemOptionsService sysOptSvc, IDnsCacherService dnsSvc, IHttpContextAccessor contextAccesssor)
      {
         mobjSyncSvc = syncSvc;
         mobjSysOptSvc = sysOptSvc;
         mobjDnsSvc = dnsSvc;
         mobjHttpContext = contextAccesssor;
      }

      public void OnActionExecuted(ActionExecutedContext context)
      {
         //throw new NotImplementedException();
      }

      public void OnActionExecuting(ActionExecutingContext context)
      {
         Network objCurrentNetwork = mobjSyncSvc.GetCurrentNetwork();
         if(objCurrentNetwork==null)
         {
            objCurrentNetwork = new Network();
            objCurrentNetwork.HostName = mobjDnsSvc.ResolveIp(mobjHttpContext.HttpContext.Connection.RemoteIpAddress.ToString());
            mobjSyncSvc.SetNetwork(objCurrentNetwork);
         }
      }
   }
}
