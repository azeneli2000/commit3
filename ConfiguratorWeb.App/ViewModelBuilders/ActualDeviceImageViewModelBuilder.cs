using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digistat.FrameworkStd.UMSLegacy;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class ActualDeviceImageViewModelBuilder
   {
      public static ActualDeviceImageViewModel Build(ActualDeviceImage source)
      {
         ActualDeviceImageViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ActualDeviceImageViewModel
               {
                  IsNewRecord = false,
                  DeviceType = source.DeviceType,
                  DeviceName = source.ActualDeviceName,
                  DeviceSerialNumber = source.ActualDeviceSerial,
                  DeviceTypeDescription = UMSFrameworkParser.GetDeviceTypeDescription(source.DeviceType),
                  Extension = source.Extension.Trim('.'),
                  //Image = source.Image == null ? null : System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(source.Image)),
                  //Thumbnail = source.Thumbnail == null ? null : System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(source.Thumbnail)),
                  Image = source.Image == null ? "" : Convert.ToBase64String(source.Image),
                  Thumbnail = source.Thumbnail == null ? "" : Convert.ToBase64String(source.Thumbnail),
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<ActualDeviceImageViewModel> BuildList(IEnumerable<ActualDeviceImage> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
