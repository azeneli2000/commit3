using Configurator.Std.Enums;
using Digistat.FrameworkStd.Model;
using System.Linq;

namespace Configurator.Std.BL
{
   public interface IBedsManager : Digistat.Dal.Interfaces.IDalManagerBase<Bed>
   {
      string Delete(int entityId);    
      Bed Get(int id);


      IQueryable<Bed> GetBedsWithFullData();
        /// <summary>
        /// move bet to new index and return new index
        /// </summary>
        /// <param name="BedID"></param>
        /// <param name="direction"></param>
        /// <returns>-1: if error in move</returns>
      int MoveBed(int BedID, MoveDirection direction);
      /// <summary>
      /// Reindex the beds and in case of duplicate value ordered it for Location-IdBed
      /// </summary>
      /// <returns></returns>
      int FixBedsIndex();
   }
}