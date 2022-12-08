using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Model.Export;

namespace Configurator.Std.BL.ExportScheduler
{
   public interface IExportJobsManager
   {
      bool Create(ExportJobs job, string usrAbbrev);
      bool Delete(int jobID, string usrAbbrev, string usrID);

      List<ExportJobs> GetAll(bool includeDeleted = false);
      ExportJobs GetById(int id);
      List<ExportJobsHistory> GetHistoryById(int jobId);
      string TestQuery(string query);
      bool Update(ExportJobs job, string usrAbbrev);

   }
}
