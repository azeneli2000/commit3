using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Mobile;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.Mobile
{
   public interface IPositionService
   {
      IEnumerable<PositionAssociation> GetPositions();
      PositionAssociation GetPositionByPositionCode(string positionCode);
      PositionAssociation CreatePosition(PositionAssociation objPositionAssociation);
      PositionAssociation UpdatePosition(PositionAssociation objPositionAssociation);
      IQueryable<Bed> GetAviableBedPosition(string positionCode);
      void DeletePosition(string positionCode);
   }
}
