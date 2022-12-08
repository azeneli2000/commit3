using Digistat.Dal.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using System.Collections.Generic;

namespace Configurator.Std.BL.Monitoring
{
   public interface IMonitoringRawRequestManager : IDalManagerBase<MonitoringRawRequest>
   {
      MonitoringRawRequest[] GetLastRequests(MonitoringData.MonitoringType enuType);
      MonitoringRawRequest GetLastRequest(MonitoringData.MonitoringType enuType);
   }
}
