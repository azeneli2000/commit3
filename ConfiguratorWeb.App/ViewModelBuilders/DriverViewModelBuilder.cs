using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.UMSLegacy;
using System.Text;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DriverViewModelBuilder
   {
      public static DriverViewModel Build(DriverRepository source, IDictionaryService dictionarySrv)
      {
         DriverViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DriverViewModel
               {
                  Id = source.Id,
                  AlarmSupport = (AlarmSupportTypes)source.AlarmSupport,
                  AlarmSupportDescription = getAlarmSupportType(source.AlarmSupport, dictionarySrv),
                  Capabilities = DriverCapabilityViewModelBuilder.BuildList(source.Capabilities),
                  ComToRegister = source.ComToRegister,
                  Current = source.Current,
                  DefaultCommConfiguration = source.DefaultCommConfiguration,
                  Device = source.Device,
                  DeviceType = source.DeviceType,
                  DeviceTypeDesc = string.IsNullOrEmpty(source.DeviceTypeDesc) ? GetDeviceTypeDesc(source.DeviceType) : source.DeviceTypeDesc,
                  DriverName = source.DriverName,
                  DriverVersion = source.DriverVersion,
                  DriverModel = source.Model,
                  DriverVersionBuild = source.DriverVersionBuild,
                  EntryExe = source.EntryExe,
                  EventCatalog = DriverEventCatalogViewModelBuilder.BuildList(source.EventsMapping),
                  FileCount = source.FileCount,
                  FormatStyle = source.FormatStyle,
                  HardwareRelease = source.HardwareRelease,
                  IsWrapper = source.IsWrapper,
                  LastStreamUpdate = source.LastStreamUpdate,
                  Manufacturer = source.Manufacturer,
                  Note = source.Note,
                  RemappedEvents = source.RemappedEvents,
                  SoftwareRelease = source.SoftwareRelease,
                  RunAsDLL = source.RunAsDLL,
                  //Stream = source.Stream,
                  //StreamSize = source.StreamSize,
                  UseDynamicParameters = source.UseDynamicParameters,
                  ValidToDate = source.ValidToDate,
                  Version = source.Version,
                  AlarmSystemType = source.AlarmSystemType.HasValue ? source.AlarmSystemType.Value : Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.AlarmSystemDefaultValue(),
                  AlarmSystemTypeDescription = getAlarmSystemType(source, dictionarySrv),
                  //DriverFilesStatusDescription = string.IsNullOrWhiteSpace(source.Id) ? "No files" : source.FileCount.ToString() + " files",
                  IsBinFile = source.IsBinFile,
                  HasContentInStream = source.Stream != null,

               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DriverViewModel> BuildList(IEnumerable<DriverRepository> source, IDictionaryService dictionarySrv)
      {
         try
         {
            return source.Select(x => Build(x, dictionarySrv));
         }
         catch (Exception)
         {

            throw;
         }
      }


      private static string GetDeviceTypeDesc(string devTypeInt)
      {
         StringBuilder objSb = new StringBuilder();
         if(!string.IsNullOrEmpty(devTypeInt))
         {
            List<string> objDevDescs = new List<string>();
            List<string> objDevTypes = devTypeInt.Split(',',StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach(string s in objDevTypes)
            {
               int intTmp = 0;
               if(Int32.TryParse(s, out intTmp))
               {
                  DeviceType objDevTypeEn = ((DeviceType)intTmp);
                  objDevDescs.Add(objDevTypeEn.ToString());
               }
            }
            objSb.Append(String.Join(", ", objDevDescs.ToArray()));
         }
         return objSb.ToString();
      }
      public static string getAlarmSystemType(DriverRepository source, IDictionaryService dictionarySrv)
      {
         var _val = source.AlarmSystemType.HasValue? source.AlarmSystemType.Value: Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.AlarmSystemDefaultValue();
         return dictionarySrv.XLate(UMSFrameworkParser.GetAlarmSystemTypeDescription(_val), StringParseMethod.Html);
      }
      public static string getAlarmSupportType(short source, IDictionaryService dictionarySrv)
      {
         return dictionarySrv.XLate(UMSFrameworkParser.GetAlarmSupportTypeDescription(source), StringParseMethod.Html);
      }
   }
}