using System;
using System.Collections.Generic;
using System.Text;

using Digistat.FrameworkStd.Interfaces;

namespace Configurator.Std.BL.Configurator
{
   public interface IConfiguratorWebConfiguration : IDigistatConfiguration
   {
      bool AutoLogin { get; }
      
      //DAS
      string DasDriversCachePath { get; }

      bool DasDriversCachePathAutoCreate { get; }

      int DasDriversCacheLifeCicleTimeout { get; }

      string DasDriverBaseClassName { get; }

      string CacheRootPath { get;  }

      string DeviceImageCachePath { get; }

      bool DeviceImageCachePathAutoCreate { get; }

      int DeviceImageCacheLifeCicleTimeout { get; }

      int DasInstancesRequestTimeout { get; }

      int DasDriversStatusRequestTimeout { get; }

      int DasDriversStatusRefreshInterval { get; }

      string DigistatRepositoryCachePath { get; }

      string UserName { get; }

      string Password { get; }

      int ListenerTCPPort { get; }

      string ApkCache { get; }
   }
}
