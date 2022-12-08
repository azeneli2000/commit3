using Configurator.Std.BL.Mobile;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configurator.Std.BL
{
   public class MobileServiceManager : IMobileServiceManager
   {
      private readonly IMessageCenterService mobjMessageCenter;
      private readonly IDigistatConfiguration mobjConfig;
      private readonly ILoggerService Log;

      public MobileServiceManager(
         IMessageCenterService msgcenter,
         IDigistatConfiguration config,
         ILoggerService logSvc
      )
      {
         mobjMessageCenter = msgcenter;
         mobjConfig = config;
         Log = logSvc;
      }

      public async Task<List<MobileDevice>> GetDevices()
      {
         using (var mgr = new AsyncMobileDeviceDispatcher(mobjMessageCenter, mobjConfig, Log))
         {
            var offline = await mgr.FetchDeviceList();
            var data = await mgr.RequestDevices();
            foreach (var device in offline)
            {
               if (data.Find(target => target.DeviceID.Equals(device.DeviceID)) == null)
               {
                  data.Add(device);
               }
            }
            return data;
         }
      }

      public async Task<bool> AddApk(ApkMetadata metadata, string filePath)
      {
         var apkManager = new ApkRepository(mobjConfig);
         DigistatMobileAPK item = null;
         try
         {
            item = new DigistatMobileAPK
            {
               Version = metadata.Version,
               Name = metadata.Filename,
               File = string.Empty,
               Blob = System.IO.File.ReadAllBytes(filePath)
            };

            await apkManager.Add(item);

            using (var mgr = new AsyncAPKUploader(mobjMessageCenter, Log, metadata.Session))
            {
               bool result = await mgr.NewAPK(metadata.Filename, metadata.Version);
               if (!result)
               {
                  apkManager.Remove(item);
               }
               return result;
            }
         }
         catch (Exception)
         {
            if (item != null)
            {
               apkManager.Remove(item);
            }
            return false;
         }
      }

      public async Task<bool> RemoveApk(DigistatMobileAPK apk)
      {
         using (var mgr = new AsyncAPKUploader(mobjMessageCenter, Log, Guid.NewGuid().ToString()))
         {
            return await mgr.RemoveAPK(apk);
         }
      }

      public async Task<ServerStatus> GetServerStatus()
      {
         using (var mgr = new AsyncServerStatusDispatcher(mobjMessageCenter, Log))
         {
            return await mgr.RequestServerStatus();
         }
      }
   }
}
