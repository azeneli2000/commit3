using Configurator.Std.BL.DasDrivers;
using Digistat.FrameworkStd.Model;
using System.Collections.Generic;

namespace Configurator.Std.BL
{
   public interface IActualDeviceImagesManager : Digistat.Dal.Interfaces.IDalManagerBase<ActualDeviceImage>
   {
      void Delete(int deviceType, string deviceName, string deviceSerial);
      ActualDeviceImage Get(int deviceType, string deviceName, string deviceSerial, bool loadImage = false, bool loadThumb = false);
      ActualDeviceImage GetForDevice(ActualDevice device);
      DeviceImageInfo GetImageForDevice(ActualDevice device);
      Dictionary<int, DeviceImageInfo> GetThumbnailForDevices(IEnumerable<ActualDevice> devices);
      DeviceImageInfo GetThumbnailForDevice(ActualDevice device);
      CachedFile DownloadImage(int deviceType, string deviceName, string deviceSerial);
      void UploadImage(string imageCacheId, string filename, byte[] image);
   }
}