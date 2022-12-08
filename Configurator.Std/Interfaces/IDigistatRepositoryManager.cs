using Configurator.Std.BL.DasDrivers;
using Configurator.Std.Enums;
using Digistat.FrameworkStd.Model;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL
{
   public interface IDigistatRepositoryManager : Digistat.Dal.Interfaces.IDalManagerBase<DigistatRepository>
   {
      
      DigistatRepository Get(string id);

      DigistatRepository Create(string filename, byte[] image, bool isArchive);

      IQueryable<DigistatRepository> GetQueryable();



      byte[] CreateArchiveForDigistatRepository(List<CachedFile> objCacheFiles, string archiveName);

      byte[] UncompressArchive(string archiveName,byte[] archiveContent);

      void Delete(string id);
   
   }
}