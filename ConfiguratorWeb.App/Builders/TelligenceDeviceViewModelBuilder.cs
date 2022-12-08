using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Enums;

namespace ConfiguratorWeb.App.Builders
{
   public static class TelligenceDeviceViewModelBuilder
   {
      public static TelligenceDeviceViewModel Build(TelligenceDevice source)
      {
         TelligenceDeviceViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new TelligenceDeviceViewModel
               {
                  ID = source.tl_ID,
                  TLDeviceID = source.tl_deviceID,
                  tl_IPAddress = source.tl_IPAddress,
                  TLLocationID = source.tl_locationID,
                  tl_MACAddress = source.tl_MACAddress,
                  NetworkID = source.tl_NetworkID,
                  tl_psv_ID = source.tl_psv_ID,
                  tl_ty_ = source.tl_ty_,
                  tl_ty_ID = source.tl_ty_ID,
                  HasNetwork = (source.tl_NetworkID.HasValue && source.tl_NetworkID != 0),
                  HasPortServer = (source.tl_psv_ID > 0),
                  TLLocationDescriptor = source.tl_locationDescriptor,
                  TLDeviceName = source.tl_DeviceName,
                  BedCount = source.BedCount 
               };
               if (source.tl_DeviceType != null)
               {
                  if (Enum.IsDefined(typeof(TelligenceXMLRPCClient.Entities.StaffStationTypes), source.tl_DeviceType))
                  {
                     objDest.tl_DeviceType = source.tl_DeviceType != null ? (TelligenceXMLRPCClient.Entities.StaffStationTypes)source.tl_DeviceType : TelligenceXMLRPCClient.Entities.StaffStationTypes.Unknown;
                     objDest.DeviceTypeDescription = source.tl_DeviceType != null ? ((TelligenceXMLRPCClient.Entities.StaffStationTypes)source.tl_DeviceType).GetDisplayAttribute() : (TelligenceXMLRPCClient.Entities.StaffStationTypes.Unknown).GetDisplayAttribute();
                  }
                  else
                  {
                     //Unrecognizable DeviceType, set as unknown
                     objDest.tl_DeviceType = TelligenceXMLRPCClient.Entities.StaffStationTypes.Unknown;
                     objDest.DeviceTypeDescription = (TelligenceXMLRPCClient.Entities.StaffStationTypes.Unknown).GetDisplayAttribute();
                  }

               }
            }
         }
         catch (Exception e)
         {
         }

         return objDest;
      }

      public static IEnumerable<TelligenceDeviceViewModel> BuildList(IEnumerable<TelligenceDevice> source)
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
