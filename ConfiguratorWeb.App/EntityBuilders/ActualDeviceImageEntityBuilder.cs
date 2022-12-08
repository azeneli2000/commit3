using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class ActualDeviceImageEntityBuilder
   {
      public static ActualDeviceImage Build(ActualDeviceImageViewModel source)
      {
         ActualDeviceImage objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ActualDeviceImage
               {
                  DeviceType = source.DeviceType,
                  ActualDeviceName = source.DeviceName ?? string.Empty,
                  ActualDeviceSerial = source.DeviceSerialNumber ?? string.Empty,
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<ActualDeviceImage> BuildList(IEnumerable<ActualDeviceImageViewModel> source)
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
 