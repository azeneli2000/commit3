using System.Linq;
using Configurator.Std.Enums;
using Digistat.FrameworkStd.Model.Ips;

namespace Configurator.Std.Interfaces
{
   public interface IWaveformManager: Digistat.Dal.Interfaces.IDalManagerBase<WaveformSnapshotToUniteRule>
   {
      public IQueryable<WaveformSnapshotToUniteRule> GetAllInclude();
      public int Delete(int id);
      public int GetLowerPriority(int id);
      public bool IsPriorityAlreadyAssigned(int priority, int id = 0);
      int MoveWaveformRule(int waveformId, MoveDirection direction);
   }
}