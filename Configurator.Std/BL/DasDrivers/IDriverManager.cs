using System.Collections.Generic;

using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL.DasDrivers
{
   public interface IDriverManager
   {
      void CacheDriver(IEnumerable<CachedFile> driverFiles, string driverRepositoryIdentifier);
      CachedFile CreateExportArchive(DriverRepository repository);
      byte[] PrepareFilesForDownload(DriverRepository repository, string driverRepositoryIdentifier);
      void UpdateDriverInfoUsingCachedDriver(string driverRepositoryIdentifier, DriverRepository repository);
      void UpdateDriverInfoUsingIndex(string driverRepositoryIdentifier, DriverRepository repository, CachedFile indexArchiveFile);
      byte[] GetFileStream(DriverRepository objDriverRepo, string BinariesCacheIdentifier);

      bool RemoveCachedDriver(string BinariesCacheIdentifier);
   }
}