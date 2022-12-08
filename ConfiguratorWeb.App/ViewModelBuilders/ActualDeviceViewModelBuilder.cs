using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Interfaces;
//using ConfiguratorWeb.Core.Model;
//using System.Threading.Tasks;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.UMSLegacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class ActualDeviceViewModelBuilder
   {
      public static ActualDeviceViewModel Build(ActualDevice source, IDictionaryService dictionarySrv, Configurator.Std.BL.DeviceImageInfo thumb = null)
      {
         ActualDeviceViewModel objDest = null;
         try
         {
            if (source != null)
            {
               //var thumb = imageManager == null ? null : imageManager.GetThumbnailForDevice(source);

               objDest = new ActualDeviceViewModel
               {
                  Id = source.Id,
                  DeviceType = source.DeviceType,
                  DeviceTypeDescription = dictionarySrv.XLate(UMSFrameworkParser.GetDeviceTypeDescription(source.DeviceType), Digistat.FrameworkStd.Enums.StringParseMethod.Html),
                  Label = source.Label,
                  Mobile = source.Mobile,
                  Name = source.Name,
                  SerialNumber = source.SerialNumber,
                  Thumbnail = thumb == null ? "" : Convert.ToBase64String(thumb.Content),
                  Extension = thumb == null ? "" : thumb.Extension,

               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<ActualDeviceViewModel> BuildList(IEnumerable<ActualDevice> source, IDictionaryService dictionarySrv, Dictionary<int, Configurator.Std.BL.DeviceImageInfo> thumbs = null)
      {
         try
         {
            return source.Select(x => Build(x, dictionarySrv, thumbs.ContainsKey(x.Id) ? thumbs[x.Id] : null));
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
