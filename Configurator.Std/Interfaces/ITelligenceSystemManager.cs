using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


using Digistat.FrameworkStd.Model.Integration.Telligence;

namespace Configurator.Std.BL
{
   public interface ITelligenceSystemManager : Digistat.Dal.Interfaces.IDalManagerBase<TelligenceSystem>
   {
      string Delete(int id);

      TelligenceSystem Get(int id);
      TelligenceSystem GetByHostID(int hostid);

      IEnumerable<TelligenceSystem> GetAllSystems();
   }
}