using System.Collections.Generic;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface ISimpleChoiceManager: Digistat.Dal.Interfaces.IDalManagerBase<SimpleChoiceManager>
   {
      Dictionary<string,int> GetAllGroup(bool noTracking=true);
      IEnumerable<SimpleChoice> GetAll(bool noTracking=true);
      IEnumerable<SimpleChoice> GetGroupChoises(string group);
      bool UpdateGroup(List<SimpleChoice> choises);
      bool DeleteGroup(string groupId);
      SimpleChoice CreateGroup(SimpleChoice choisesNew);
   }
}