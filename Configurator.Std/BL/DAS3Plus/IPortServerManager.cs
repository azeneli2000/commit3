using System.Collections.Generic;
using System.Linq;
using Digistat.FrameworkStd.Model.DAS3Plus;

namespace Configurator.Std.BL.DasDrivers
{
    public interface IPortServerManager : Digistat.Dal.Interfaces.IDalManagerBase<PortServer>
    {
        List<PortServer> GetTelligencePortServer();

        PortServer Get(int ID);

        PortServer GetWithBeds(int ID);

        Dictionary<int, string> GetPortServerTypes();

        bool Delete(int ID);

        IQueryable<PortServerStatus> GetPortServerStatus(int psID);
    }
}