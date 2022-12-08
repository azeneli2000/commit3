using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Threading.Tasks;

namespace Configurator.Std.BL
{
   public interface IMobileManager
   {
      Task<SystemStatus> RequestSystemStatus(string deviceID);
      Task<SourceRef> RequestSourceRef(string deviceID);
      Task<bool> SendCommunication(Communication communication);
      Task<MobileConfig> GetConfiguration(string deviceID);
      Task<bool> SetConfiguration(MobileConfig config);
      
      bool RequestLogs(string deviceID, DateTime? date);
      bool Disconnect(string deviceID);
   }

  
}
