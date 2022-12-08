using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

using Configurator.Std.BL.Configurator;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.UMSLegacy;
using Configurator.Std.Helpers;

namespace Configurator.Std.BL.DasDrivers
{
   public class DriverManager : IDriverManager
   {
      //private string driverRepositoryIdentifier;

      #region constructors

      private readonly IConfiguratorWebConfiguration mobjDigConfig;
      private readonly IStandardParametersManager mobjStandardParametersManager;
      private readonly IStandardUnitsManager mobjStandardUnitsManager;
      private readonly IStandardDeviceTypesManager mobjStandardDeviceTypesManager;
      private readonly ILoggerService mobjLoggerService;

      public DriverManager(IConfiguratorWebConfiguration digConfig, ILoggerService loggerService,
         IStandardParametersManager standardParametersManager, IStandardUnitsManager standardUnitsManager,
         IStandardDeviceTypesManager standardDeviceTypesManager)//, string repositoryIdentifier)
      {
         mobjDigConfig = digConfig;
         mobjStandardParametersManager = standardParametersManager;
         mobjStandardUnitsManager = standardUnitsManager;
         mobjStandardDeviceTypesManager = standardDeviceTypesManager;
         mobjLoggerService = loggerService;
         //   this.driverRepositoryIdentifier = repositoryIdentifier;
      }

      #endregion

      public void CacheDriver(IEnumerable<CachedFile> driverFiles, string driverRepositoryIdentifier)
      {

         int receivedFilesNum = driverFiles.Count();


         //Index File
         if (receivedFilesNum == 1 && driverFiles.Any(x => Path.GetExtension(x.Name).Trim().ToLower() == ".bin"))
         {
            this.CacheDriverFromExported(driverFiles.First(), driverRepositoryIdentifier);
            return;
         }


         //Compressed File
         if (receivedFilesNum == 1 && driverFiles.Any(x => x.CheckIsCompressed()))
         {
            this.CacheDriverFromCompressed(driverFiles.First(), driverRepositoryIdentifier);
            return;
         }

         if (receivedFilesNum > 0)
         {
            this.CacheDriverFromFiles(driverFiles, driverRepositoryIdentifier);
         }
      }

      public void UpdateDriverInfoUsingCachedDriver(string driverRepositoryIdentifier, DriverRepository repository)
      {
         //TODO Trace
         mobjLoggerService.Info("Updating driver info for repository {0} using driver execution", repository.Id);

         try
         {
            string assemblyFileName = Directory.GetFiles(CachingHelper.GetDriverCachePath(driverRepositoryIdentifier, mobjDigConfig), "*.exe").Single();
            DASDriverTCPCommunicator objCommunicator = new DASDriverTCPCommunicator();
            DasDriverInfoExtended objInfo = objCommunicator.Reader(mobjDigConfig.ListenerTCPPort, assemblyFileName,"-SUITE 7.0");
            if(objInfo!=null)
            {

               objInfo.UpdateRepository(repository);



               if(repository.Capabilities!=null)
               {
                  foreach (var capability in repository.Capabilities)
                  {
                     try
                     {
                        //Load referenced entities
                        capability.StandardParameter = mobjStandardParametersManager.Get(capability.StandardParameterId);
                        capability.StandardUnit = mobjStandardUnitsManager.Get(capability.StandardUnitId);
                        capability.StandardDeviceType = mobjStandardDeviceTypesManager.Get(capability.StandardDeviceTypeId);
                     }
                     catch(Exception e)
                     {
                        throw new Exception($"Error while parsing capability with ID {capability.StandardParameterId}", e);
                     }
                     
                  }
               }
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error updating driver info for {0} ", repository.Id);
            throw;
         }
      }

      public void UpdateDriverInfoUsingIndex(string driverRepositoryIdentifier, DriverRepository repository, CachedFile indexArchiveFile)
      {
         //TODO Trace
         mobjLoggerService.Info("Updating driver info for repository {0} using index file", repository.Id);

         try
         {

            string tempFile = this.GetTempRootDirectory(driverRepositoryIdentifier);

            string subDir = CreateTempDirectory(tempFile, repository.Id);

            string tempFileFullName = Path.Combine(subDir, indexArchiveFile.Name);

            File.WriteAllBytes(tempFileFullName, indexArchiveFile.Content);

            Byte[] content = UMSFrameworkCompression.ExtractSingleUsingFramework(tempFileFullName);

            Directory.Delete(subDir, true);

            DasDriverComunicator.SetValuesFromHeader(repository, content);
            foreach (var capability in repository.Capabilities)
            {
               //Load referenced entities
               capability.StandardParameter = mobjStandardParametersManager.Get(capability.StandardParameterId);
               capability.StandardUnit = mobjStandardUnitsManager.Get(capability.StandardUnitId);
               capability.StandardDeviceType = mobjStandardDeviceTypesManager.Get(capability.StandardDeviceTypeId);
            }


         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error updating driver info for repository {0} using index file", repository.Id);
            throw;
         }


      }

      public CachedFile CreateExportArchive(DriverRepository repository)
      {
         //TODO Trace
         mobjLoggerService.Info("Caching driver for repository {0}", repository.Id);

         string repositoryTempDir = this.GetTempRootDirectory(repository.Id);

         try
         {

            string strHeader = DasDriverComunicator.BuildDriverHeader(repository);

            if (strHeader == null) return null;


            if (!Directory.Exists(repositoryTempDir))
            {
               Directory.CreateDirectory(repositoryTempDir);
            }

            string subdir = CreateTempDirectory(repositoryTempDir, repository.Id);

            string binName = UMSFrameworkCompression.CreateZippedArchive(repository, repositoryTempDir, strHeader, subdir);

            return new CachedFile(binName, File.ReadAllBytes(System.IO.Path.Combine(repositoryTempDir, binName)));
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error creating export file for {0}", repository.Id);
            throw;
         }
         finally
         {
            if (Directory.Exists(repositoryTempDir))
            {
               Directory.Delete(repositoryTempDir, true);
            }
         }
      }

      public byte[] PrepareFilesForDownload(DriverRepository repository, string driverRepositoryIdentifier)
      {
         //TODO Trace
         mobjLoggerService.Info("Updating driver info for repository {0} using index file", repository.Id);

         try
         {

            string tempDirectory = this.GetTempRootDirectory(driverRepositoryIdentifier);

            string subDir = CreateTempDirectory(tempDirectory, driverRepositoryIdentifier);

            string filename = Path.Combine(tempDirectory, Guid.NewGuid().ToString() + ".zip");

            UMSFrameworkCompression.ExtractAllUsingFramework(repository.Stream, subDir);

            ZipFile.CreateFromDirectory(subDir, filename, CompressionLevel.Fastest, false);

            var result = File.ReadAllBytes(filename);

            File.Delete(filename);

            Directory.Delete(tempDirectory, true);

            return result;

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error preparing driver zip file for repository {0}", repository.Id);
            throw;
         }
      }

      #region driver files physical caching

      private void CacheDriverFromExported(CachedFile exportedArchiveFile, string driverRepositoryIdentifier)
      {
         //TODO Trace
         mobjLoggerService.Info("Caching driver for repository {0}", driverRepositoryIdentifier);

         try
         {
            //Extract files in 
            UMSFrameworkCompression.ExtractAllUsingFramework(exportedArchiveFile.Content, CachingHelper.GetDriverCachePath(driverRepositoryIdentifier, mobjDigConfig));
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error caching extracted files from compressed archive");
            throw;
         }
      }

      private void CacheDriverFromCompressed(CachedFile compressedArchiveFile, string driverRepositoryIdentifier)
      {
         //TODO Trace
         mobjLoggerService.Info("Caching driver for repository {0}", driverRepositoryIdentifier);

         try
         {
            //Extract files in 
            UMSFrameworkCompression.ExtractAll(compressedArchiveFile.Content, CachingHelper.GetDriverCachePath(driverRepositoryIdentifier, mobjDigConfig));
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error caching extracted files from compressed archive");
            throw;
         }
      }

      private void CacheDriverFromFiles(IEnumerable<CachedFile> driverFiles, string driverRepositoryIdentifier)
      {
         //TODO Trace
         //mobjLoggerService.Info("Caching driver for repository {1}", driverRepositoryIdentifier);

         try
         {
            if (Directory.Exists(CachingHelper.GetDriverCachePath(driverRepositoryIdentifier, mobjDigConfig)))
            {
               var directory = new DirectoryInfo(CachingHelper.GetDriverCachePath(driverRepositoryIdentifier, mobjDigConfig));
               try
               {
                  directory.EnumerateFiles().ToList().ForEach(f => f.Delete());
               }
               catch (Exception)
               {
               }
               
            }
            foreach (CachedFile driverFile in driverFiles)
            {
               string destination = Path.Combine(CachingHelper.GetDriverCachePath(driverRepositoryIdentifier, mobjDigConfig), driverFile.Name);
               File.WriteAllBytes(destination, driverFile.Content);
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error caching extracted files from compressed archive");
            throw;
         }

      }

      #endregion

    

      #region Paths location

      private static string CreateTempDirectory(string repositoryTempDir, string driverRepositoryIdentifier)
      {
         //New temporary folder
         string subdir = Path.Combine(repositoryTempDir, Guid.NewGuid().ToString());

         if (!Directory.Exists(subdir))
         {
            Directory.CreateDirectory(subdir);
         }
         return subdir;
      }

      private string GetTempRootDirectory(string driverRepositoryIdentifier)
      {
         //Temporary path
         string tempDir = Path.Combine(mobjDigConfig.DasDriversCachePath, "Temporary");

         if (!Directory.Exists(tempDir))
         {
            Directory.CreateDirectory(tempDir);
         }

         //Subdirectory for selected driver repository
         return Path.Combine(tempDir, driverRepositoryIdentifier);

      }

      public bool RemoveCachedDriver(string BinariesCacheIdentifier)
      {
         bool bolRet = false;
         try
         {
            string tempDir = System.IO.Path.Combine(mobjDigConfig.DasDriversCachePath, BinariesCacheIdentifier);
            System.IO.Directory.Delete(tempDir, true);
            bolRet = true;
         }
         catch(Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Unable to remove cached driver files: {BinariesCacheIdentifier}");
         }
         return bolRet;
      }
      
      public byte[] GetFileStream(DriverRepository objDriverRepo,string BinariesCacheIdentifier)
      {
         byte[] objRet = null;
         string tempDir = System.IO.Path.Combine(mobjDigConfig.DasDriversCachePath, BinariesCacheIdentifier);
         if (!objDriverRepo.IsBinFile)
         {
            objRet = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompression.ZipAndGetStream(objDriverRepo, tempDir);
         }
         else
         {
            string fullFileName = System.IO.Directory.GetFiles(tempDir, "*.bin").FirstOrDefault();
            if (!string.IsNullOrEmpty(fullFileName))
            {
               objRet = File.ReadAllBytes(tempDir);
            }
            
         }
         return objRet;
      }
 
      #endregion
   }
}
