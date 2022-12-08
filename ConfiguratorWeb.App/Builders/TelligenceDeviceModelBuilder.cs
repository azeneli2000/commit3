using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Builders
{
   public static class TelligenceDeviceModelBuilder
   {

      public static TelligenceDevice Build(TelligenceDeviceViewModel source)
      {
         TelligenceDevice objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceDevice
               {
                  tl_ID = source.ID,
                  tl_deviceID = source.TLDeviceID,
                  tl_IPAddress = source.tl_IPAddress,
                  tl_locationID = source.TLLocationID,
                  tl_MACAddress = source.tl_MACAddress,
                  tl_NetworkID = source.NetworkID,
                  tl_psv_ID = source.tl_psv_ID,
                  tl_ty_ = source.tl_ty_,
                  tl_ty_ID = source.tl_ty_ID,
                  tl_DeviceType = (int)source.tl_DeviceType,
                  tl_locationDescriptor = source.TLLocationDescriptor,
                  tl_DeviceName = source.TLDeviceName
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceDevice> BuildList(IEnumerable<TelligenceDeviceViewModel> source)
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
