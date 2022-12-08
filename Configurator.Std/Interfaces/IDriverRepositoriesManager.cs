using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Configurator.Std.BL.DasDrivers;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IDriverRepositoriesManager : Digistat.Dal.Interfaces.IDalManagerBase<DriverRepository>
   {
      CachedFile DownloadDriver(string driverRepositoryId);
      CachedFile ExportDriver(string driverRepositoryId);
      //DriverRepository UploadDriver(string driverRepositoryId, IEnumerable<CachedFile> driverFiles);      
      DriverRepository Get(string id, bool loadCapabilities = false, bool loadEventsMapping = false, bool noTracking=false);
      DriverCommConfiguation GetConfiguration(string id);
      DriverRepository UploadDriver(string driverRepositoryId, DriverRepository entity, IEnumerable<CachedFile> driverFiles, bool keepCapabilities, bool keepFormatString,bool keepRemappedEvents);
      void Remove(string driverRepositoryId);
      IEnumerable<DriverRepository> GetAll(int pageNumber = 0, int pageSize = 0, bool loadCapabilities = false, bool loadEventsMapping = false);
      IEnumerable<DriverRepository> Find(Expression<Func<DriverRepository, bool>> filterPredicate, bool loadCapabilities = false, bool loadEventsMapping = false, int pageNumber = 0, int pageSize = 0);
      DriverRepository Update(DriverRepository driverRepository, bool FileHasChanged =false, bool updateCapabilities = true, bool updateEventsMapping = true);
      bool Create(DriverRepository driverRepository, bool updateCapabilities = true, bool updateEventsMappings = true);

      IEnumerable<T> GetActives<T>(Expression<Func<DriverRepository, T>> selectPredicate = null);

      IEnumerable<DriverRepository> GetActives();
      /// <summary>
      /// Get Id,Version and Driver name for current driver ordered
      /// </summary>
      /// <returns></returns>
      List<(string Id,int Version, string DriverName)> GetActivesLight();
      bool CheckCdssAlreadyExists();
      string GetCdssGuid();
      IEnumerable<DriverRepositoryEventCatalog> GetEventsByDriverId(string id);
   }
}