using Digistat.FrameworkStd.Model.Monitoring;

namespace Configurator.Std.BL.Monitoring
{
   public interface ICurrentSystemErrorStatusManager
   {
      CurrentSystemErrorStatus GetLastStatus();
      CurrentSystemErrorStatus GetLastValidStatus();
   }
}