using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Std.BL
{
   public interface IServiceStatusManager : Digistat.Dal.Interfaces.IDalManagerBase<ServiceStatus>
   {
      List<ServiceStatus> GetServiceStatuses();
      void SetActive(string Application, string Host);
   }
}
