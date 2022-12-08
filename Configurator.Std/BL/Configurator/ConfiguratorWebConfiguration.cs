using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Digistat.FrameworkStd.Configuration;

namespace Configurator.Std.BL.Configurator
{
   public class ConfiguratorWebConfiguration : DigistatConfigurationBase, IConfiguratorWebConfiguration
   {
      public ConfiguratorWebConfiguration(IConfiguration config)
      {
         _config = config;
      }


      //public static void InitDigistatConfig(string messageCenter, string messageCenterInstance, bool autoLogin, int clientAuthMode,
      //   int loggerLevelType, bool loggerConsoleLoggingEnabled, bool loggerEventLogLoggingEnabled, bool loggerDatabaseLoggingEnabled,
      //   string loggerFileLoggingDestiantionPath, string loggerSocketLoggingPort, string dasDriversCachePath, 
      //   int dasDriversCacheLifeCicleTimeout, string dasDriverBaseClassName)
      //{

      //   //DigistatConfig.MessageCenter = messageCenter;
      //   //DigistatConfig.MessageCenterInstance = messageCenterInstance;
      //   DigistatConfig.AutoLogin = autoLogin;

      //   try
      //   {
      //      DigistatConfig.ClientAuthMode = (ClientEnableMode)clientAuthMode;
      //   }
      //   catch
      //   {
      //      //UmsLogger.Instance.Error(string.Empty, LogCode.Generic, LevelType.Always, "ClientAuthMode setting is not a valid ClientEnableMode value");
      //      DigistatConfig.ClientAuthMode = ClientEnableMode.DigistatStyle;
      //   }

      //   try
      //   {
      //      DigistatConfig.LoggerLevelType = (LevelType)loggerLevelType;
      //   }
      //   catch
      //   {
      //      //UmsLogger.Instance.Error(string.Empty, LogCode.Generic, LevelType.Always, "LoggerLevelType setting is not a valid LevelType value");
      //      DigistatConfig.LoggerLevelType = LevelType.Standard;
      //   }

      //   DigistatConfig.LoggerConsoleLoggingEnabled = loggerConsoleLoggingEnabled;
      //   DigistatConfig.LoggerEventLogLoggingEnabled = loggerEventLogLoggingEnabled;
      //   DigistatConfig.LoggerDatabaseLoggingEnabled = loggerDatabaseLoggingEnabled;
      //   DigistatConfig.LoggerFileLoggingDestiantionPath = loggerFileLoggingDestiantionPath;

      //   if (string.IsNullOrWhiteSpace(loggerSocketLoggingPort))
      //   {
      //      DigistatConfig.LoggerSocketLoggingPort = null;
      //   }
      //   else
      //   {
      //      try
      //      {
      //         DigistatConfig.LoggerSocketLoggingPort = Convert.ToInt32(loggerSocketLoggingPort);
      //      }
      //      catch
      //      {
      //         //UmsLogger.Instance.Error(string.Empty, LogCode.Generic, LevelType.Always, "LoggerSocketLoggingPort setting is not a valid port number");
      //         DigistatConfig.LoggerSocketLoggingPort = null;
      //      }

      //   }


      //   DigistatConfig.DasDriversCachePath = dasDriversCachePath;

      //   DigistatConfig.DasDriversCacheLifeCicleTimeout = dasDriversCacheLifeCicleTimeout;

      //   DigistatConfig.DasDriverBaseClassName = dasDriverBaseClassName;

      //}



      //public static bool PatientPhotoEnabled { get; set; }

      //public static bool GetNetworkFromRequest  { get; set; }

      //public static bool DnsEnabled { get; set; }

      //public static bool ContentCacheEnabled { get; set; }

      //public static bool DnsCacheEnabled { get; set; }

      //public static string DefaultLanguage { get; set; }

      //public static bool DictionaryLearnMode { get; set; }

      //public static string DebugPatient { get; set; }

      //public static string DebugUserUsername { get; set; }

      //public static string DebugUserPassword { get; set; }

      //public static bool ClientSyncDisable { get; set; }

      //public static string OranJPlanSvcAddress { get; set; }


      //Configurator

      //public static string ConnString { get; set; }

      //public static string ApplicationName
      //{
      //   get
      //   {
      //      return "CONFIGURATOR";
      //   }
      //}

      public bool AutoLogin
      {
         get
         {
            return _config.GetValue<bool>("ConfiguratorWeb:AutoLogin");
         }
      }


      public string UserName
      {
         get
         {
            return _config.GetValue<string>("ConfiguratorWeb:Username");
         }
      }

      public string Password
      {
         get
         {
            return _config.GetValue<string>("ConfiguratorWeb:Password");
         }
      }

      //DAS
      public string DasDriversCachePath
      {
         get
         {
            return System.IO.Path.Combine(CacheRootPath, "Drivers_cache");
         }
      }

      public bool DasDriversCachePathAutoCreate
      {
         get
         {
            return _config.GetValue<bool>("ConfiguratorWeb:DasDrivers:AutoCreateCachePath");
         }
      }

      public int DasDriversCacheLifeCicleTimeout
      {
         get
         {
            return _config.GetValue<Int32>("ConfiguratorWeb:DasDrivers:CacheLifeCicleTimeout");
         }
      }

      public string DasDriverBaseClassName
      {
         get
         {
            return _config.GetValue<string>("ConfiguratorWeb:DasDrivers:BaseClassName");
         }
      }


      public int ListenerTCPPort
      {
         get
         {
            return _config.GetValue<Int32>("ConfiguratorWeb:DasDrivers:ListenerTCPPort");
         }
      }

      public string CacheRootPath
      {
         get
         {
            return _config.GetValue<string>("ConfiguratorWeb:CacheRootPath");
         }
      }


      public string DeviceImageCachePath
      {
         get
         {
            return System.IO.Path.Combine(CacheRootPath,"Images_cache");
         }
      }


      public string DigistatRepositoryCachePath
      {
         get
         {
            return System.IO.Path.Combine(CacheRootPath, "DigistatRepository_cache");
         }
      }

      public bool DeviceImageCachePathAutoCreate
      {
         get
         {
            return _config.GetValue<bool>("ConfiguratorWeb:Images:AutoCreateCachePath");
         }
      }

      public int DeviceImageCacheLifeCicleTimeout
      {
         get
         {
            return _config.GetValue<Int32>("ConfiguratorWeb:Images:CacheLifeCicleTimeout");
         }
      }

      public int DasInstancesRequestTimeout
      {
         get
         {
            var value = _config.GetValue<Int32>("ConfiguratorWeb:DasDrivers:DasInstancesRequestTimeout");

            return value == 0 ? 5000 : value;
         }
      }

      public int DasDriversStatusRequestTimeout
      {
         get
         {
            var value = _config.GetValue<Int32>("ConfiguratorWeb:DasDrivers:DasDriversStatusRequestTimeout");

            return value == 0 ? 5000 : value;
         }
      }

      public int DasDriversStatusRefreshInterval
      {
         get
         {
            return _config.GetValue<Int32>("ConfiguratorWeb:DasDrivers:DasDriversStatusRefreshInterval");
         }
      }

      public string ApkCache
      {
         get
         {
            return System.IO.Path.Combine(CacheRootPath, "APK_cache");
         }
      }

      
   }
}
