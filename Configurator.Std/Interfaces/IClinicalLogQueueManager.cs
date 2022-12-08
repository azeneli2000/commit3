using System.Collections.Generic;
using System.Linq;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IClinicalLogQueueManager : Digistat.Dal.Interfaces.IDalManagerBase<ClinicalLogQueueManager>
   {
      bool TableExists();
      IQueryable<Digistat.FrameworkStd.Model.ClinicalLogQueue> GetClinicalLogs();
      IEnumerable<string> GetDistinctClinicalLogsPriority();
      IEnumerable<string> GetDistinctClinicalLogsTask();
   }
}