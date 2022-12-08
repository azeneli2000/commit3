using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class DriverEntityBuilder
   {
      public static DriverRepository Build(DriverViewModel source)
      {
         DriverRepository objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DriverRepository
               {
                  Id = source.Id,
                  Version = source.Version,
                  Current = source.Current,
                  ValidToDate = source.ValidToDate,
                  DriverName = source.DriverName,
                  DriverVersion = source.DriverVersion,
                  IsWrapper = source.IsWrapper,
                  //Stream = source.Stream,
                  //StreamSize = source.StreamSize,
                  FileCount = source.FileCount,
                  EntryExe = source.EntryExe,
                  LastStreamUpdate = source.LastStreamUpdate.HasValue?source.LastStreamUpdate.Value:new DateTime(1900,1,1),
                  Note = source.Note,
                  ComToRegister = source.ComToRegister,
                  DefaultCommConfiguration = source.DefaultCommConfiguration,
                  Manufacturer = source.Manufacturer,
                  Device = source.Device,
                  Model=source.DriverModel,
                  DeviceType = source.DeviceType,
                  DriverVersionBuild = source.DriverVersionBuild,
                  HardwareRelease = source.HardwareRelease,
                  SoftwareRelease = source.SoftwareRelease,
                  FormatStyle = source.FormatStyle,
                  RemappedEvents = source.RemappedEvents,
                  RunAsDLL = source.RunAsDLL,
                  AlarmSupport=(short)source.AlarmSupport,
                  UseDynamicParameters=source.UseDynamicParameters,
                  Capabilities = DriverCapabilityEntityBuilder.BuildList(source.Capabilities).ToList(),
                  EventsMapping =   DriverEventCatalogEntityBuilder.BuildList(source.EventCatalog).ToList(),
                  AlarmSystemType = source.AlarmSystemType,
                  IsBinFile = source.IsBinFile,
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DriverRepository> BuildList(IEnumerable<DriverViewModel> source)
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
