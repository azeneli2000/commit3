using Digistat.Dal.Interfaces;
using Digistat.FrameworkStd.Model;
using System.Collections.Generic;

namespace Configurator.Std.BL
{
   public interface IHospitalUnitsManager : IDalManagerBase<HospitalUnit>
   {
      HospitalUnit Get(string guid);
      bool Delete(string GUID, out string errorMessage);

      IEnumerable<HospitalUnit> GetList();

   }
}
