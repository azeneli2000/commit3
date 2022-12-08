using System;
using System.Collections.Generic;
using System.Text;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Interfaces;
using System.Linq;
using System.IO;
using Configurator.Std.Helpers;
using Configurator.Std.BL.Configurator;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.UMSLegacy;

namespace Configurator.Std.BL
{

   public enum DeviceImageType
   {
      Image,
      Thumbnail,
   }

   public class DeviceImageInfo
   {

      public string DeviceName { get; set; }

      public string Extension { get; set; }

      public DeviceImageType Type { get; set; }

      public byte[] Content { get; set; }
   }

   public class ActualDeviceImagesManager : DalManagerBase<ActualDeviceImage>, IActualDeviceImagesManager
   {

      #region Costructors

      private readonly IConfiguratorWebConfiguration mobjDigConfig;

      public ActualDeviceImagesManager(DigistatDBContext context, ILoggerService loggerService, IConfiguratorWebConfiguration digConfig)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjDigConfig = digConfig;

         this.BeforeSave += BeforeSaveHandler;
         this.BeforeUpdate += BeforeUpdateHandler;
      }

      private void BeforeSaveHandler(object sender, EventArgs e)
      {

         ActualDeviceImage deviceDriver = (ActualDeviceImage)((SaveOrUpdateEventArgs)e).entity;

         validateData(deviceDriver);

         //Prevent duplications
         var repository = mobjDbContext.Set<ActualDeviceImage>();
         ActualDeviceImage loadedEntity = repository.SingleOrDefault(x => x.DeviceType == deviceDriver.DeviceType && x.ActualDeviceName == deviceDriver.ActualDeviceName && x.ActualDeviceSerial == deviceDriver.ActualDeviceSerial);
         if (loadedEntity != null)
         {
            throw new Exception(string.Format("Unable to crate actual device image for {0} with name {1} and serial {2}; image with same identifiers already exists.", UMSFrameworkParser.GetDeviceTypeDescription(deviceDriver.DeviceType), deviceDriver.ActualDeviceName, deviceDriver.ActualDeviceSerial));
         }

      }

      private void BeforeUpdateHandler(object sender, EventArgs e)
      {

         ActualDeviceImage deviceDriver = (ActualDeviceImage)((SaveOrUpdateEventArgs)e).entity;

         validateData(deviceDriver);

         int lastVersion = mobjDbContext.Set<ActualDeviceImage>()
            .Where(x => x.DeviceType == deviceDriver.DeviceType && x.ActualDeviceName == deviceDriver.ActualDeviceName && x.ActualDeviceSerial == deviceDriver.ActualDeviceSerial && x.Image != deviceDriver.Image)            
            .Select(x => x.ImageVersion)
            .SingleOrDefault();

         if (lastVersion == 0)
         {
            throw new Exception(string.Format("Unable to update actual device image for {0} with name {1} and serial {2}; image does not exists.", UMSFrameworkParser.GetDeviceTypeDescription(deviceDriver.DeviceType), deviceDriver.ActualDeviceName, deviceDriver.ActualDeviceSerial));
         }

         deviceDriver.ImageVersion = lastVersion + 1;
      }


      private void validateData(ActualDeviceImage image)
      {

         if (image.DeviceType == 0)
         {
            throw new ArgumentException("Unable to save actual device images without Device Type", "DeviceType");
         }


         if (image.Image == null || image.Image.Length == 0)
         {
            throw new ArgumentException("Unable to saveactual device images without image stream", "Image");
         }
        
      }

      #endregion

      public void Delete(int deviceType, string deviceName, string deviceSerial)
      {
         //TODO Trace
         mobjLoggerService.Info("Deleting ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2}", deviceType, deviceName, deviceSerial);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            // for retrocompatibility , null value is saved as empty string
            if (string.IsNullOrWhiteSpace(deviceSerial))
            {
               deviceSerial = "";
            }
            // for retrocompatibility , null value is saved as empty string
            if (string.IsNullOrWhiteSpace(deviceName))
            {
               deviceName = "";
            }
            var repository = mobjDbContext.Set<ActualDeviceImage>();

            ActualDeviceImage loadedEntity = repository.SingleOrDefault(x => x.DeviceType == deviceType && x.ActualDeviceName == deviceName && x.ActualDeviceSerial == deviceSerial);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2}; entity not found.", deviceType, deviceName, deviceSerial));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            if (executeClose) { mobjDbContext.CommitTransaction(); }

            //TODO Trace
            mobjLoggerService.Info("ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2} removed succesfully", loadedEntity.DeviceType, loadedEntity.ActualDeviceName, loadedEntity.ActualDeviceSerial);

         }
         catch (Exception e)
         {
            if (executeClose) { mobjDbContext.RollbackTransaction(); }
            mobjLoggerService.ErrorException(e, "Error removing ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2}; entity not found.", deviceType, deviceName, deviceSerial);
            string message = string.Format("Error removing ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2}; entity not found.", deviceType, deviceName, deviceSerial);
            throw new Exception(message, e);
         }
      }

      public ActualDeviceImage Get(int deviceType, string deviceName, string deviceSerial, bool loadImage = false, bool loadThumb = false)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for Actual Device Image for Device Type {0}, Device Name {1} and  Device Serial {2}", deviceType, deviceName, deviceSerial);

         ActualDeviceImage result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<ActualDeviceImage> repository = mobjDbContext.Set<ActualDeviceImage>();

            //TODO Trace
            mobjLoggerService.Info("Reading ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2} from DB", deviceType, deviceName, deviceSerial);

            result = repository
               .Select(x => new ActualDeviceImage {
                  ActualDeviceName = x.ActualDeviceName,
                  ActualDeviceSerial = x.ActualDeviceSerial,
                  DeviceType = x.DeviceType,
                  ImageVersion = x.ImageVersion,
                  Extension = x.Extension,
                  Image = loadImage ? x.Image : null,
                  Thumbnail = loadThumb ? x.Thumbnail : null,
               })
               .SingleOrDefault(x => x.DeviceType == deviceType && x.ActualDeviceName == deviceName && x.ActualDeviceSerial == deviceSerial);

            //TODO Trace
            mobjLoggerService.Info("ActualDeviceImage with id {0} retrived from DB", deviceType, deviceName, deviceSerial);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2} from DB", deviceType, deviceName, deviceSerial);
            throw new Exception(string.Format("Error reading ActualDeviceImage with Device Type {0}, Device Name {1} and  Device Serial {2} from DB", deviceType, deviceName, deviceSerial), e);
         }

         return result;
      }

      public ActualDeviceImage GetForDevice(ActualDevice device)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for Actual Device Image for Device with Type {0}, Name {1} and Serial {2}", device.DeviceType, device.Name, device.SerialNumber);

         ActualDeviceImage result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<ActualDeviceImage> repository = mobjDbContext.Set<ActualDeviceImage>();

            //TODO Trace
            mobjLoggerService.Info("Reading from DB ActualDeviceImage with Device Type {0}", device.DeviceType);

            //Get all images for the given deivce type
            List<ActualDeviceImage> images = repository.Where(x => x.DeviceType == device.DeviceType)
               .Select(x => new ActualDeviceImage
               {
                  DeviceType = x.DeviceType,
                  ActualDeviceName = x.ActualDeviceName,
                  ActualDeviceSerial = x.ActualDeviceSerial,
                  Extension = x.Extension,
                  ImageVersion = x.ImageVersion
               }).ToList();

            //No images found
            if (!images.Any())
            {
               return null;
            }

            result = findDeviceImage(device, images);

            //TODO Trace
            mobjLoggerService.Info("ActualDeviceImage with  Type {0}, Name {1} and Serial {2} retrived from DB", device.DeviceType, device.Name, device.SerialNumber);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading from DB Actual Device Image with Device Type {0}", device.DeviceType);
            throw new Exception(string.Format("Error reading from DB Actual Device Image with Device Type {0", device.DeviceType), e);
         }

         return result;
      }

      private ActualDeviceImage findDeviceImage(ActualDevice device, List<ActualDeviceImage> images)
      {
         int occurrenciesCount = images.Count(x => x.ActualDeviceName == device.Name && x.ActualDeviceSerial == device.SerialNumber);
         if (occurrenciesCount > 1)
         {
            mobjLoggerService.Error("More than one image found for actual device with Type {0}, Name {1} and Serial {2}", device.DeviceType, device.Name, device.SerialNumber);
            return images.Where(x => x.ActualDeviceName == device.Name && x.ActualDeviceSerial == device.SerialNumber).OrderByDescending(x => x.ImageVersion).First();
         }

         if (occurrenciesCount == 1)
         {
            return images.Single(x => x.ActualDeviceName == device.Name && x.ActualDeviceSerial == device.SerialNumber);
         }

         if (occurrenciesCount == 0)
         {
            mobjLoggerService.Info("No images found for Actual Device with Type {0}, Name {1} and Serial {2}", device.DeviceType, device.Name, device.SerialNumber);

            occurrenciesCount = images.Count(x => x.ActualDeviceName == device.Name && string.IsNullOrWhiteSpace(x.ActualDeviceSerial));
            if (occurrenciesCount > 1)
            {
               mobjLoggerService.Error("More than one image found for actual device with Type {0}, Name {1} and no Serial", device.DeviceType, device.Name);
               return images.Where(x => x.ActualDeviceName == device.Name && string.IsNullOrWhiteSpace(x.ActualDeviceSerial)).OrderByDescending(x => x.ImageVersion).First();
            }

            if (occurrenciesCount == 1)
            {
               return images.Single(x => x.ActualDeviceName == device.Name && string.IsNullOrWhiteSpace(x.ActualDeviceSerial));
            }

            if (occurrenciesCount == 0)
            {
               mobjLoggerService.Info("No images found for Actual Device with Type {0}, Name {1} and no Serial", device.DeviceType, device.Name);

               occurrenciesCount = images.Count(x => string.IsNullOrWhiteSpace(x.ActualDeviceName) && string.IsNullOrWhiteSpace(x.ActualDeviceSerial));
               if (occurrenciesCount > 1)
               {
                  mobjLoggerService.Error("More than one image found for actual device with Type {0}, no Name and no Serial", device.DeviceType);
                  return images.Where(x => string.IsNullOrWhiteSpace(x.ActualDeviceName) && string.IsNullOrWhiteSpace(x.ActualDeviceSerial)).OrderByDescending(x => x.ImageVersion).First();
               }
               if (occurrenciesCount == 1)
               {
                  return images.Single(x => string.IsNullOrWhiteSpace(x.ActualDeviceName) && string.IsNullOrWhiteSpace(x.ActualDeviceSerial));
               }
               if (occurrenciesCount == 0)
               {
                  mobjLoggerService.Info("No images found for Actual Device with Type {0}, Name {1} and Serial {2}", device.DeviceType, device.Name, device.SerialNumber);
                  return null;
               }
            }
         }

         return null;
      }

      public DeviceImageInfo GetImageForDevice(ActualDevice device)
      {

         ActualDeviceImage image = this.GetForDevice(device);

         if (image == null)
         {
            return null;
         }

         //Reload image 
         return mobjDbContext.Set<ActualDeviceImage>()
            .Where(x => x.DeviceType == image.DeviceType && x.ActualDeviceName == image.ActualDeviceName && x.ActualDeviceSerial == image.ActualDeviceSerial)
            .Select(x => new DeviceImageInfo
            {
               DeviceName = device.Name + "_" + device.SerialNumber + "_v" + x.ImageVersion,
               Content = x.Image,
               Extension = x.Extension,
               Type = DeviceImageType.Image

            })
            .SingleOrDefault();

      }

      public Dictionary<int, DeviceImageInfo> GetThumbnailForDevices(IEnumerable<ActualDevice> devices) 
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for Actual Device Image for Devices");

         var result = new Dictionary<int, DeviceImageInfo>();

         try
         {
            IQueryable<ActualDeviceImage> repository = mobjDbContext.Set<ActualDeviceImage>();

            //TODO Trace
            mobjLoggerService.Info("Reading from DB ActualDeviceImage for Devices Types");

            //Get all images for the given deivce type
            List<ActualDeviceImage> images = repository.Where(x => devices.Select(y => y.DeviceType).Any(y => y ==  x.DeviceType))
               .Select(x => new ActualDeviceImage
               {
                  DeviceType = x.DeviceType,
                  ActualDeviceName = x.ActualDeviceName,
                  ActualDeviceSerial = x.ActualDeviceSerial,
                  Extension = x.Extension,
                  ImageVersion = x.ImageVersion
               }).ToList();

            //No images found
            if (!images.Any())
            {
               return null;
            }

            foreach (var device in devices)
            {
               DeviceImageInfo thumb = null;
               var image = findDeviceImage(device, images);
               if (image != null) 
               {
                  //Read Image thumb
                  thumb =  mobjDbContext.Set<ActualDeviceImage>()
                     .Where(x => x.DeviceType == image.DeviceType && x.ActualDeviceName == image.ActualDeviceName && x.ActualDeviceSerial == image.ActualDeviceSerial)
                     .Select(x => new DeviceImageInfo
                     {
                        DeviceName = device.Name + "_" + device.SerialNumber + "_v" + x.ImageVersion,
                        Content = x.Thumbnail,
                        Extension = x.Extension,
                        Type = DeviceImageType.Thumbnail
                     })
                     .SingleOrDefault();                  
               }
               result.Add (device.Id, thumb);
            }

            //TODO Trace
            mobjLoggerService.Info("ActualDevice thumbnails retrived from DB");

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading from DB Actual Device thumbnails");
            throw new Exception("Error reading from DB Actual Device thumbnails", e);
         }

         return result;

      }

      public DeviceImageInfo GetThumbnailForDevice(ActualDevice device)
      {

         ActualDeviceImage image = this.GetForDevice(device);

         if (image == null)
         {
            return null;
         }

         //Reload image 
         return mobjDbContext.Set<ActualDeviceImage>()
            .Where(x => x.DeviceType == image.DeviceType && x.ActualDeviceName == image.ActualDeviceName && x.ActualDeviceSerial == image.ActualDeviceSerial)
            .Select(x => new DeviceImageInfo
            {
               DeviceName = device.Name + "_" + device.SerialNumber + "_v" + x.ImageVersion,
               Content = x.Thumbnail,
               Extension = x.Extension,
               Type = DeviceImageType.Thumbnail

            })
            .SingleOrDefault();

      }

      #region Image Management
      public DasDrivers.CachedFile DownloadImage(int deviceType, string deviceName, string deviceSerial)
      {
         string devType = null;
         try
         {
            devType = UMSFrameworkParser.GetDeviceTypeDescription(deviceType);

            //TODO Trace
            mobjLoggerService.Info("Reading image of actual device with Type {0}, Name {1} and Serial {2} for download", devType, deviceName ?? "-", deviceSerial ?? "-");

            ActualDeviceImage loadedEntity  = this.Get(deviceType, deviceName ?? "", deviceSerial ?? "", true);

            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to download image of actual device with Type {0}, Name {1} and Serial {2}; image not found.", devType, deviceName ?? "-", deviceSerial ?? "-"));
            }

            string fileNamePattern = "Device {0} -{1}_{2}.{3}";

            return new DasDrivers.CachedFile(string.Format(fileNamePattern, devType, deviceName, deviceSerial, loadedEntity.Extension), loadedEntity.Image);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error downloading image of actual device with Type {0}, Name {1} and Serial {2}", devType ?? deviceType.ToString(), deviceName ?? "-", deviceSerial ?? "-");
            string message = string.Format("Error downloading image of actual device with Type {0}, Name {1} and Serial {2}", devType ?? deviceType.ToString(), deviceName ?? "-", deviceSerial ?? "-");
            throw new Exception(message, e);
         }

      }


      public void UploadImage(string imageCacheId, string filename, byte[] image)
      {
         if (image == null)
         {
            throw new Exception(string.Format("Unable to cache image file for iActual device image with id {0}; File not received.", imageCacheId));
         }

         //TODO Trace
         mobjLoggerService.Info("Caching image for Actual Device Image with id {0}", imageCacheId);

         try
         {
            //Save image in cache
            string directory = CachingHelper.GetImageCachePath(imageCacheId, mobjDigConfig);
            string destinationImage = Path.Combine(directory, filename);
            File.WriteAllBytes(destinationImage, image);
            //Save thumbnail in cache
            string destinationThumbnail = Path.Combine(directory, "thumbnail_" + filename);
            var thumbnail = ConversionsHelper.ImageToThumbnail(image);
            File.WriteAllBytes(destinationImage, thumbnail);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error caching image for actual device image with id {0}", imageCacheId);
            string message = string.Format("Error caching image for actual device image with id {0}", imageCacheId);
            throw new Exception(message, e);
         }
      }
      #endregion
   }
}
