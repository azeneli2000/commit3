using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;

namespace Configurator.Std.BL
{
   public interface IDasOutputStateManager : Digistat.Dal.Interfaces.IDalManagerBase<DasOutputState>
   {
      void Delete(int locationId, int bedId, int patientId);
      DasOutputState Get(int locationId, int bedId, int patientId);
      IQueryable<DasOutputState> GetDasOutputStates();

   }
}