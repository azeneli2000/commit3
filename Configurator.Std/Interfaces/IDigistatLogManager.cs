using System.Collections.Generic;
using System.Linq;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IDigistatLogManager : Digistat.Dal.Interfaces.IDalManagerBase<DigistatLogManager>
   {
      bool TableExists();
      IQueryable<Digistat.FrameworkStd.Model.Log> GetDigistatLogs();
      IEnumerable<string> GetDistinctDigistatLogsPriority();
      IEnumerable<string> GetDistinctDigistatLogsTask();
   }
}