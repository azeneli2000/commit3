using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.DictionaryTerms;
using Configurator.Std.BL.DasDrivers;
using System.Globalization;
using Configurator.Std.BL;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DasStatusViewModelBuilder
   {

      public static DasStatusViewModel Build(KeyValuePair<string, DriverStatus> source, IBedsManager bedManager, IDictionaryService dictionarySrv)
      {
         DasStatusViewModel objDest = null;
         try
         {
            var brokerInstance = source.Key;
            var dasStatus = source.Value;

            if (dasStatus != null)
            {
               objDest = new DasStatusViewModel
               {
                  DasBroker = brokerInstance,
                  DeviceDriverId = dasStatus.DeviceDriverId,
                  ProcessId = dasStatus.ProcessId,
                  Name = dasStatus.DriverName,
                  Version = dasStatus.DriverVersion,
                  ProcessStatus = dasStatus.ProcessStatus,
                  ProcessStatusClass = getProcesStatusClass(dasStatus.ProcessStatus),
                  LastDatasetElaspedTime = getStatusLastDateDescription(dasStatus.LastDatasetReceived, dictionarySrv),
                  DriverStatus = dasStatus.DriverDeviceStatus.DriverStatus,
                  DriverStatusClass = getDriverStatusClass(dasStatus.DriverDeviceStatus.DriverStatus),
                  DriverStatusDescription = UMSFrameworkParser.GetDriverStatusDescription(dasStatus.DriverDeviceStatus.DriverStatus),
                  DeviceStatus = dasStatus.DriverDeviceStatus.DeviceStatus,
                  DeviceStatusClass = getDeviceStatusClass(dasStatus.DriverDeviceStatus.DeviceStatus),
                  DeviceStatusDescription = UMSFrameworkParser.GetDeviceStatusDescription(dasStatus.DriverDeviceStatus.DeviceStatus),
                  //Unable to get the information, desktop configurator does not retrieve the information, seems to be always empty
                  //DeviceMessage = 
                  Address = dasStatus.Address,
                  BedName = getBedName(dasStatus.BedId, bedManager),                 
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }


      private static string getProcesStatusClass(string processStatus)
      {
         //CA1304 e CA1308 consigliano k.ToUpperInvariant() e modificare i case non lo faccio ora per non ritestare.... 
         // ....alla prima occasione sarebbe meglio farlo
         switch (processStatus.ToLower().Trim())
         {
            case "started":
               return "NormalStatus";
            default:
               return "AnomalyStatus";
         }
      }

      private static string getDriverStatusClass(int driverStatus)
      {
         if (UMSFrameworkParser.IsDriverStatusNone(driverStatus)) {
            return "AlarmStatus";
         }
         if (UMSFrameworkParser.IsDriverStatusHalted(driverStatus))
         {
            return "AlarmStatus";
         }

         return "NormalStatus";
      }

      private static string getDeviceStatusClass(int deviceStatus)
      {
         if (UMSFrameworkParser.IsDeviceStatusNone(deviceStatus))
         {
            return "AlarmStatus";
         }
         if (UMSFrameworkParser.IsDeviceStatusStandBy(deviceStatus))
         {
            return "WarningStatus";
         }

         //Reflects UMS IndicationV2Level.HighPriority (3)
         if (UMSFrameworkParser.IsDeviceStatusHighPriorityAlarm(deviceStatus))
         {
            return "HighPriority";
         }

         //Reflects UMS IndicationV2Level.MediumPriority (2)
         if (UMSFrameworkParser.IsDeviceStatusMediumPriorityAlarm(deviceStatus))
         {
            return "MediumPriority";
         }

         //Reflects UMS IndicationV2Level.LowPriority (1)
         if (UMSFrameworkParser.IsDeviceStatusLowPriorityAlarm(deviceStatus))
         {
            return "LowPriority";
         }

         //Reflects UMS IndicationV2Level.Info (0)
         if (UMSFrameworkParser.IsDeviceStatusInformationAlarm(deviceStatus))
         {
            return "InfoAlarm";
         }

         if (UMSFrameworkParser.IsDeviceStatusInitializing(deviceStatus))
         {
            return "NormalStatus";
         }

         //Running
         return "NormalStatus";
      }

      private static string getStatusLastDateDescription(DateTime? lastDate, IDictionaryService dictionarySrv)
      {

         if (!lastDate.HasValue)
         {
            return string.Empty;
         }

         if (DateTime.UtcNow.Date != lastDate.Value.Date)
         {
            return lastDate.Value.ToLocalTime().ToString("dd/MM HH:mm:ss", CultureInfo.InvariantCulture);
         }

         int intSec = (int)DateTime.UtcNow.Subtract(lastDate.Value).TotalSeconds;
         if (intSec < 60)
         {
            //#4484
            if (intSec<0)
               intSec = 0;

            return string.Format("{0} {1} {2}", intSec, dictionarySrv.XLate("sec."), dictionarySrv.XLate("ago"));
         }

         return lastDate.Value.ToLocalTime().ToString("HH:mm:ss", CultureInfo.InvariantCulture);

      }

      private static string getBedName(int bedId, IBedsManager bedManager)
      {

         var bed = bedManager.Get(bedId);

         if (bed == null)
         {
            return string.Empty;
         }


         return bed.Name;
      }


      public static IEnumerable<DasStatusViewModel> BuildList(string broker, List<DriverStatus> source, IBedsManager bedManager, IDictionaryService dictionarySrv)
      {
         try
         {
            return source.Select(x => Build(new KeyValuePair<string, DriverStatus>(broker, x), bedManager, dictionarySrv));
         }
         catch (Exception)
         {
            throw;
         }
      }


      public static IEnumerable<DasStatusViewModel> BuildList(Dictionary<string, List<DriverStatus>> source, IBedsManager bedManager, IDictionaryService dictionarySrv)
      {
         try
         {
            return source.Select(x => BuildList(x.Key, x.Value, bedManager, dictionarySrv)).SelectMany(x => x);
         }
         catch (Exception)
         {
            throw;
         }
      }


   }
}
