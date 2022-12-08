using System.Collections.Generic;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IStandardParametersManager : Digistat.Dal.Interfaces.IDalManagerBase<StandardParameter>
   {
      void Delete(int entityId);
      StandardParameter Get(int id);
      List<StandardParameter> GetMulti(List<int> ids);
      List<StandardParameter> GetListByDeviceId(int deviceId);
      List<StandardParameter> GetListByDriverID(string drvID);
      List<StandardParameter> GetListByDriverIdOfWaveformType(string drvName);
      bool SendRefreshMessageToConnect();
   }
}