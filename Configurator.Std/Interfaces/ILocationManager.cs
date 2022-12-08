using Configurator.Std.Enums;
using Digistat.FrameworkStd.Model;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL
{
   public interface ILocationManager : Digistat.Dal.Interfaces.IDalManagerBase<Location>
   {
      void Delete(int entityId);
      Location Get(int id);
      IQueryable<Location> GetLocations();
      bool LocationCanBeDeleted(int id);
      List<Location> GetAllWithBedCounts();

      bool MoveLocation(int LocationID, MoveDirection direction);
      int FixLocationsIndex();
   }
}
