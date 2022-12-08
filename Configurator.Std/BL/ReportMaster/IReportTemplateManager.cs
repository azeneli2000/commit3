using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Std.BL.ReportMaster
{
   public interface IReportTemplateManager : Digistat.Dal.Interfaces.IDalManagerBase<ReportTemplate>
   {
      ReportTemplate Get(string id);
      void Remove(string id);
      IQueryable<ReportTemplate> GetReportTemplates();
      Dictionary<string, string> GetParametersFromTemplateXml(string xml);
      IQueryable<ReportTemplate> GetReportTemplatesMonitoring();

   }
}
