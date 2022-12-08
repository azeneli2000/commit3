using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


using Digistat.FrameworkStd.Model.Integration.Telligence;

namespace Configurator.Std.BL
{
   public interface ITelligenceServerManager : Digistat.Dal.Interfaces.IDalManagerBase<TelligenceServer>
   {
      string Delete(int id);

      TelligenceServer Get(int id);
   }
}