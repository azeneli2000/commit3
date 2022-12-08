
using Digistat.FrameworkStd.Model.Mobile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configurator.Std.BL
{
   public interface IMobileServiceManager
   {
      Task<List<MobileDevice>> GetDevices();
      Task<bool> AddApk(ApkMetadata metadata, string filePath);
      Task<bool> RemoveApk(DigistatMobileAPK apk);
      Task<ServerStatus> GetServerStatus();
   }
}
