using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class ActualDeviceEntityBuilder
   {
      public static ActualDevice Build(ActualDeviceViewModel source)
      {
         ActualDevice objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ActualDevice
               {
                  Id = source.Id,
                  DeviceType = source.DeviceType,
                  Label = source.Label,
                  Mobile = source.Mobile,
                  Name = source.Name,
                  SerialNumber = source.SerialNumber
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<ActualDevice> BuildList(IEnumerable<ActualDeviceViewModel> source)
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
 