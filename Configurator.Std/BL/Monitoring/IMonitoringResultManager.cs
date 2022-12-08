using Digistat.Dal.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Configurator.Std.BL.Monitoring
{
   public interface IMonitoringResultManager : IDalManagerBase<MonitoringResult>
   {
      IQueryable<MonitoringResult> GetLastMonitoringResults(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<MonitoringResult, object>>> includePredicates = null);

      IQueryable<MonitoringResult> GetLastMonitoringResultsCheckFilter(bool value);
   }
}
