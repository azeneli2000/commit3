using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model.DAS3Plus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class PortServerStatusViewModelBuilder
    {
        public static PortServerStatusViewModel Build(PortServerStatus source, IDictionary<short, BedViewModel> portServerBeds)
        {
            PortServerStatusViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new PortServerStatusViewModel
                    {
                        CableId = source.CableId,
                        SerialNumber = source.SerialNumber,
                        DeviceDescription = source.DeviceDescription,
                        DeviceHash = source.DeviceHash,
                        DeviceManufacturer = source.DeviceManufacturer,
                        DeviceModel = source.DeviceModel,
                        DeviceType = source.DeviceType,
                        DriverID = source.DriverID,
                        ID = source.ID,
                        LastConnection = source.LastConnection,
                        LastUpdate = source.LastUpdate,
                        Port = source.Port,
                        State = source.State,
                        Bed = portServerBeds.ContainsKey(source.Port) ? portServerBeds[source.Port] : null
                    };
                }
            }
            catch (Exception)
            {
            }

            return objDest;
        }

        public static IEnumerable<PortServerStatusViewModel> BuildList(IEnumerable<PortServerStatus> source, IDictionary<short, BedViewModel> portServerBeds)
        {
            try
            {
                return source.Select(pss => Build(pss, portServerBeds));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
