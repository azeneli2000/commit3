using Configurator.Std.Enums;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Online;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.OnLine
{
   public interface IValidationGroupManager : Digistat.Dal.Interfaces.IDalManagerBase<ValidationGroup>
   {
      ValidationGroup Get(int id);
      IEnumerable<ValidationGroup> GetList();
      bool MoveValidationGroup(int valGroupID, MoveDirection direction);

      bool Create(ValidationGroup vg,string usrAbbrev);

      bool Update(ValidationGroup vg,string usrAbbrev);

      bool Delete(int vgID,string usrAbbrev,string usrID);

      IEnumerable<ValidationSection> GetSectionList();
      ValidationSection CreateSection(ValidationSection vSect);

      ValidationSection UpdateSection(ValidationSection vSect);

      bool DeleteSection(int sectId);

   }
}