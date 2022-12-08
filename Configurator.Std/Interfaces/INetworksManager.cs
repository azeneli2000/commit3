using Digistat.FrameworkStd.Model;
using System.Collections.Generic;

namespace Configurator.Std.BL
{
   public interface INetworksManager : Digistat.Dal.Interfaces.IDalManagerBase<Network>
   {
      void Delete(int entityId);
      Network Get(int id);

      List<NetworkBedLink> UpdateNetworkBedLinkForLocation(IEnumerable<int?> locationIDS, int networkID, IEnumerable<NetworkBedLink> objLink);
      Network GetWithBeds(int id);

      Network CreateNetwork(Network objNetwork);

      Network UpdateNetwork(Network objNetwork);
      IEnumerable<Network> GetList();
   }
}