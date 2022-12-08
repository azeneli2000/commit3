using Digistat.FrameworkStd.Model;
using System.Collections.Generic;

namespace Configurator.Std.BL
{
   public interface INetworkBedLinkManager : Digistat.Dal.Interfaces.IDalManagerBase<Bed>
   {
      bool UpdateNetworkBedLinkForLocation(List<Bed> objList, int idNetwork);
   }
}