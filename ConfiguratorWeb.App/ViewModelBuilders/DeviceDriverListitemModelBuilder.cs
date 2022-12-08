using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Enums;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DeviceDriverListitemModelBuilder
   {

      public static DeviceDriverListitemModel Build(DeviceDriver3 source, IEnumerable<DeviceDriver3BedLink> bedLnks, IDictionaryService dictionarySrv)
      {
         DeviceDriverListitemModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DeviceDriverListitemModel
               {
                  Id = source.Id,
                  Address = getAddress(source),
                  AutoStartDriver = source.AutoStartDriver,
                  AutoStartWatchDog = source.AutoStartWatchDog,
                  ComputerName = source.ComputerName,
                  //CommConfiguration = source.CommConfiguration,
                  DeviceName = source.DeviceName,
                  DriverType = getDriverType(source, dictionarySrv),
                  IdDriverRepository = source.IdDriverRepository,
                  LogEnabled = source.LogEnabled,
                  Description = source.DeviceName,
                  Name = source.Repository.DriverName,
                  Version = source.Repository.DriverVersion,
                  BedLink = getBedLink(source, bedLnks), //dictionarySrv),
                  AlarmSystemType = getAlarmSystemType(source, dictionarySrv),
               };
            }
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }


      //private string getStringValueFromDriverAlarmSystemType(int driverType, IDictionaryService dictionarySrv)
      //{
      //   string result = string.Empty;
      //   switch (driverType)
      //   {
      //      case 0:
      //         result = dictionarySrv.XLate(DasDictionaryTerms.driverAlarmSystemTypeDIS, StringParseMethod.Html);
      //         break;
      //      case 1:
      //         result = dictionarySrv.XLate(DasDictionaryTerms.driverAlarmSystemTypeDAS, StringParseMethod.Html);
      //         break;
      //      case 2:
      //         result = dictionarySrv.XLate(DasDictionaryTerms.driverAlarmSystemTypeCDAS, StringParseMethod.Html);
      //         break;
      //      default:
      //         break;
      //   }
      //   return result;
      //}

      //private static string getDriverType(DeviceDriver3 source, IDictionaryService dictionarySrv)
      //{
      //   string result = string.Empty;

      //   if (source.IsMultibedDriverType())
      //   {
      //      result = dictionarySrv.XLate(DasDictionaryTerms.driverTypeMultiBed, StringParseMethod.Html);
      //   }

      //   if (source.IsSinglebedDriverType())
      //   {
      //      result = dictionarySrv.XLate(DasDictionaryTerms.driverTypeSingleBed, StringParseMethod.Html);
      //   }

      //   if (source.IsCentralDriverType())
      //   {
      //      result = dictionarySrv.XLate(DasDictionaryTerms.driverTypeCentral, StringParseMethod.Html);
      //   }

      //   return result;
      //}

      public static string getDriverType(DeviceDriver3 source, IDictionaryService dictionarySrv)
      {
         return dictionarySrv.XLate(source.GetDriverTypeDescription(), StringParseMethod.Html);
      }

      public static string getAlarmSystemType(DeviceDriver3 source, IDictionaryService dictionarySrv)
      {
         return dictionarySrv.XLate(source.GetAlarmSystemTypeDescription(), StringParseMethod.Html);
      }

      private static string getBedLink(DeviceDriver3 source, IEnumerable<DeviceDriver3BedLink> bedLinks)
      {

         if (bedLinks == null) { bedLinks = new List<DeviceDriver3BedLink>(); }

         if (!source.IsMultibedDriverType())
         {
            return bedLinks.Count() == 1 ? bedLinks.First().Bed?.Name : string.Empty;
         }

         string sTmp = "";

         foreach (DeviceDriver3BedLink bedl in bedLinks)
         {
            if (!string.IsNullOrEmpty(sTmp)) { sTmp += ","; }
            sTmp += bedl.Bed.Name ?? "[" + bedl.Bed.BedCode + "]";
            
            if (sTmp.Length > 10)
            {
               break;
            }
         }

         if (sTmp.Length > 10)
         {
            sTmp = sTmp.Substring(0, 10) + " ..."; ;
         }

         return sTmp;

      }

      private static string getAddress(DeviceDriver3 source)
      {

         string result = string.Empty;

         if (source.IsRs232ConnectionType())
         {
            result = "COM" + source.CommConfigurationObject.ComPort.ToString();
         }

         if (source.IsSocketConnectionType())
         {
            result = source.CommConfigurationObject.Hostname;
         }

         return result;
      }


      public static IEnumerable<DeviceDriverListitemModel> BuildList(IEnumerable<DeviceDriver3> source, IEnumerable<DeviceDriver3BedLink> bedLnks, IDictionaryService dictionarySrv)
      {
         try
         {
            return source.Select(x => Build(x, bedLnks.Where(y => y.DeviceDriverId == x.Id), dictionarySrv));
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
