using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Digistat.FrameworkStd.Model.Integration.Telligence;

namespace Configurator.Std.BL
{
    public interface ITelligenceDeviceManager : Digistat.Dal.Interfaces.IDalManagerBase<TelligenceDevice>
    {
        //string Delete(int id);

        TelligenceDevice Get(int id);

        //TelligenceDevice Create(TelligenceDevice tlDevice, bool createNetwork);

        /// <summary>
        /// Creates a device.
        /// </summary>
        /// <param name="objTlDev">The <see cref="TelligenceDevice"/> to update.</param>
        /// <param name="objBeds">The <see cref="Bed"/> of the device.</param>
        /// <param name="objPortServerBeds">A dictionary containing the port/bed id of the device when it's a MDI device.</param>
        /// <param name="createNetwork">Indices if the network must be created (if necessary).</param>
        /// <param name="createPortServer">Indices if the port server must be created (if necessary).</param>
        /// <returns>The instance of the <see cref="TelligenceDevice"/> created.</returns>
        TelligenceDevice CreateDevice(TelligenceDevice objTlDev, List<Bed> objBeds, IDictionary<short, int> objPortServerBeds, bool createNetwork, bool createPortServer);

        /// <summary>
        /// Updates a device.
        /// </summary>
        /// <param name="objTlDev">The <see cref="TelligenceDevice"/> to update.</param>
        /// <param name="objBeds">The <see cref="Bed"/> of the device.</param>
        /// <param name="objPortServerBeds">A dictionary containing the port/bed id of the device when it's a MDI device.</param>
        /// <param name="createNetwork">Indices if the network must be created (if necessary).</param>
        /// <param name="createPortServer">Indices if the port server must be created (if necessary).</param>
        /// <returns>The instance of the <see cref="TelligenceDevice"/> updated.</returns>
        TelligenceDevice UpdateDevice(TelligenceDevice objTlDev, List<Bed> objBeds, IDictionary<short, int> objPortServerBeds, bool createNetwork, bool createPortServer);

        TelligenceDevice SearchForDevice(int TLDeviceID, int systemID);

        TelligenceDevice CreateNetworkForDevice(TelligenceDevice objTlDev);

        TelligenceDevice CreatePortServerForDevice(TelligenceDevice objTlDev);

        IEnumerable<TelligenceDevice> GetAllDevices();

        Dictionary<int, string> GetTLDeviceTypes();

        IEnumerable<TelligenceDevice> GetAllWithBeds(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<TelligenceDevice, object>>> includePredicates = null);

        bool UpdateNetworkBedLink(List<Bed> objList, int idNetwork);

        string Delete(int id);
    }
}