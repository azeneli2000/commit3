using Configurator.Std.BL.Configurator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Configurator.Std.Helpers
{
   public static class CachingHelper
   {


      public static string GetDigistatRepoPath(string cacheIdentifier,IConfiguratorWebConfiguration mobjDigConfig)
      {
         string strPathComplete = System.IO.Path.Combine(mobjDigConfig.DigistatRepositoryCachePath, cacheIdentifier);
         if (!Directory.Exists(strPathComplete))
         {
            Directory.CreateDirectory(strPathComplete);
            System.Threading.Thread.Sleep(200);
         }
         return strPathComplete;
      }

      /// <summary>
      /// Checks cache lifecicle timeout removing expired cache directories
      /// </summary>
      /// <returns></returns>
      public static string GetDriverCachePath(string driverRepositoryIdentifier, IConfiguratorWebConfiguration mobjDigConfig)
      {
         if (!Directory.Exists(mobjDigConfig.DasDriversCachePath))
         {
            if (!mobjDigConfig.DasDriversCachePathAutoCreate)
            {
               throw new Exception(string.Format("DAS temporary cache path '{0}' not found.", mobjDigConfig.DasDriversCachePath));
            }

            Directory.CreateDirectory(mobjDigConfig.DasDriversCachePath);
            System.Threading.Thread.Sleep(400);
         }
         else
         {
            if (mobjDigConfig.DasDriversCacheLifeCicleTimeout > 0)
            {
               //DateTime expirationDateTime = DateTime.Now.AddSeconds(-mobjDigConfig.DasDriversCacheLifeCicleTimeout);

               ////Remove expired cache items for repository
               //foreach (string subDirectoryPath in Directory.GetDirectories(mobjDigConfig.DasDriversCachePath))
               //{
               //   bool toRemove = true;

               //   foreach (string fileName in Directory.GetFiles(subDirectoryPath))
               //   {
               //      if (File.GetCreationTime(fileName).CompareTo(expirationDateTime) <= 0)
               //      {
               //         toRemove = false;
               //      }
               //   }

               //   if (toRemove)
               //   {
               //      Directory.Delete(subDirectoryPath, true);
               //   }
               //}
            }
         }

         string repositoryCachePath = Path.Combine(mobjDigConfig.DasDriversCachePath, driverRepositoryIdentifier);

         //If directory exists use delete to remove all contents than recreate directory

         if (!Directory.Exists(repositoryCachePath))
         {
            Directory.CreateDirectory(repositoryCachePath);
         }

         return repositoryCachePath;

         //List<string> cacheDirectories = Directory.GetDirectories(repositoryCachePath).ToList();

         //if (cacheDirectories.Count > 1)
         //{
         //   mobjLoggerService.Warn("Das Drivers cache corrupted, more than one directory for repository {0}", driverRepositoryIdentifier);
         //   throw new Exception("Das Drivers Cache corrupted");
         //}

         //if (cacheDirectories.Count == 0)
         //{
         //   Directory.Delete(repositoryCachePath, true);
         //}

         //if ((!Directory.Exists(repositoryCachePath)) || (cacheDirectories.Any(x => Path.GetDirectoryName(x) == driverRepositoryIdentifier)))
         //{
         //   return Path.Combine(repositoryCachePath, driverRepositoryIdentifier);
         //}

         //throw new Exception(string.Format("Das repository {0} already in use by an other user", driverRepositoryIdentifier));
      }


      public static string GetImageCachePath(string imageIdentifier, IConfiguratorWebConfiguration mobjDigConfig)
      {
         if (!Directory.Exists(mobjDigConfig.DeviceImageCachePath))
         {
            if (!mobjDigConfig.DeviceImageCachePathAutoCreate)
            {
               throw new Exception(string.Format("Image temporary cache path '{0}' not found.", mobjDigConfig.DeviceImageCachePath));
            }

            Directory.CreateDirectory(mobjDigConfig.DeviceImageCachePath);
            System.Threading.Thread.Sleep(400);
         }
         else
         {
            if (mobjDigConfig.DeviceImageCacheLifeCicleTimeout > 0)
            {
               //DateTime expirationDateTime = DateTime.Now.AddSeconds(-mobjDigConfig.DasDriversCacheLifeCicleTimeout);

               ////Remove expired cache items for repository
               //foreach (string subDirectoryPath in Directory.GetDirectories(mobjDigConfig.DasDriversCachePath))
               //{
               //   bool toRemove = true;

               //   foreach (string fileName in Directory.GetFiles(subDirectoryPath))
               //   {
               //      if (File.GetCreationTime(fileName).CompareTo(expirationDateTime) <= 0)
               //      {
               //         toRemove = false;
               //      }
               //   }

               //   if (toRemove)
               //   {
               //      Directory.Delete(subDirectoryPath, true);
               //   }
               //}
            }
         }

         string cachePath = Path.Combine(mobjDigConfig.DeviceImageCachePath, imageIdentifier);

         //If directory exists use delete to remove all contents than recreate directory

         if (!Directory.Exists(cachePath))
         {
            Directory.CreateDirectory(cachePath);
         }

         return cachePath;

         //List<string> cacheDirectories = Directory.GetDirectories(repositoryCachePath).ToList();

         //if (cacheDirectories.Count > 1)
         //{
         //   mobjLoggerService.Warn("Das Drivers cache corrupted, more than one directory for repository {0}", driverRepositoryIdentifier);
         //   throw new Exception("Das Drivers Cache corrupted");
         //}

         //if (cacheDirectories.Count == 0)
         //{
         //   Directory.Delete(repositoryCachePath, true);
         //}

         //if ((!Directory.Exists(repositoryCachePath)) || (cacheDirectories.Any(x => Path.GetDirectoryName(x) == driverRepositoryIdentifier)))
         //{
         //   return Path.Combine(repositoryCachePath, driverRepositoryIdentifier);
         //}

         //throw new Exception(string.Format("Das repository {0} already in use by an other user", driverRepositoryIdentifier));
      }



   }
}
