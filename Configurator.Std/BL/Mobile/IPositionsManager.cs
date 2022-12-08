using Digistat.FrameworkStd.Model.Mobile;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.Mobile
{
   public interface IPositionsManager //: Digistat.Dal.Interfaces.IDalManagerBase<PositionAssociation>
   {
      IList<PositionAssociation> GetPositions();
      PositionAssociation GetPositionByPositionCode(string positionCode);
      PositionAssociation CreatePosition(PositionAssociation objPositionAssociation);
      PositionAssociation UpdatePosition(PositionAssociation objPositionAssociation);
      IList<PositionBedLink> GetAssociatedBedWherePositionCodeIsNot(string positionCode);
      void DeletePosition(string positionCode);
   }
}
