using ConfiguratorWeb.App.Models;
//using ConfiguratorWeb.Core.Model;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Enums;
using Configurator.Std.Helpers;
using Digistat.FrameworkStd.DictionaryTerms;
using Newtonsoft.Json;


namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class DeviceDriverViewModelBuilder
   {

      public static DeviceDriverViewModel Build(DeviceDriver3 source, List<Bed> beds, IDictionaryService dictionarySrv)
      {
         DeviceDriverViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DeviceDriverViewModel(beds)
               {
                  Id = source.Id,
                  AutoStartDriver = source.AutoStartDriver,
                  AutoStartWatchDog = source.AutoStartWatchDog,
                  //CommConfiguration = source.CommConfiguration,
                  ComputerName = source.ComputerName,
                  //ConnectionType = dictionarySrv.XLate(source.GetConnectionTypeDescription(), StringParseMethod.Html),
                  ConnectionType = source.CommConfigurationObject.ConnectionType,
                  DataRate = source.DataRate ?? 0,
                  DeviceName = source.DeviceName,
                  DriverType = source.DriverType,
                  ForceSendDataWithoutPatient = source.ForceSendDataWithoutPatient,
                  IdDriverRepository = source.IdDriverRepository,
                  PatientResolveNotCached = source.PatientResolveNotCached,
                  SendDataToMC = source.SendDataToMC,
                  SQLPatientResolve = source.SQLPatientResolve,
                  AlarmSystemType = source.AlarmSystemType.HasValue ? source.AlarmSystemType.Value : UMSFrameworkParser.AlarmSystemDefaultValue(),
                  Socket = new DeviceDriverSocketViewModel
                  {
                     //SocketType = dictionarySrv.XLate(source.GetSocketTypeDescription(), StringParseMethod.Html),
                     SocketType = source.CommConfigurationObject.TCPCommType.ToString(),
                     HostName = source.CommConfigurationObject.Hostname,
                     Port = source.CommConfigurationObject.SocketPort
                  },
                  SerialPort = new DeviceDriverSerialPortViewModel
                  {
                     SerialPort = source.CommConfigurationObject.ComPort,
                     BitsPerSeconds = source.CommConfigurationObject.Baud.ToString(),
                     DataBits = source.CommConfigurationObject.DataBits.ToString(),
                     DataMode = source.GetDataModeDescription(),
                     DataModeId=source.CommConfigurationObject.ReceivingDataMode,
                     Handshake = (System.IO.Ports.Handshake)source.CommConfigurationObject.HandShake,
                     Parity = (System.IO.Ports.Parity)source.CommConfigurationObject.Parity,
                     StopBits = (System.IO.Ports.StopBits)source.CommConfigurationObject.StopBits,
                     SmartCableID = source.CommConfigurationObject.SmartCableId,
                  },
                  //LogConfig = source.LogConfig,
                  LogEnabled = source.LogEnabled,
                  //LogLevels = BuildLogLevelModel(source.LogLevels),
                  //LogDestinations = BuildDestinationLevelModel(source.LogDestinations),
                  CustomParameters = source.CommConfigurationObject.CustomParam.Select(x => new CustomParametersViewModel()
                  {
                     ID = 0,
                     Name = x.Key,
                     Value = x.Value.Key,
                     Description = x.Value.Value,
                  }),
                  //BedAssociation = source.BedLinks.Select(x => new BedAssociationViewModel {
                  //   Bedcode = x.Bed.BedCode,
                  //   BedName = x.Bed.Name,
                  //   DriverSideBedName = x.DriverSideBedName,
                  //   Watchdog = x.WatchDogEnable,
                  //   Location = x.Bed.Location.LocationName,
                  //   Enabled = x.DriverEnabled,
                  //}).ToList()
               };
            }

            UpdateLogLevel(objDest, source.LogConfigurationObject.LogLevels);
            UpdateLogDestination(objDest, source.LogConfigurationObject.LogDestinations);
            objDest.BedAssociation = UpdateBedsAssociations(objDest.BedAssociation, source.BedLinks);

            //IEnumerable<DeviceDriver3BedLink> ieLista= 
            objDest.BedAssociationChanged = string.Empty;
            objDest.BedLinkAssociationSerialize = 
               JsonConvert.SerializeObject(source.BedLinks.Select(x => new DeviceDriver3BedLink
                  {                     
                     BedId = x.BedId,
                     DeviceDriverId = x.DeviceDriverId,
                     //DeviceDriver3 = objDest,
                     DriverEnabled = x.DriverEnabled,
                     WatchDogEnable = x.WatchDogEnable,
                     WatchDogEnabled = x.WatchDogEnabled,
                     DriverSideBedName = x.DriverSideBedName ,
                     Bed = x.Bed
                  })
               );
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }


      public static List<BedAssociationViewModel> UpdateBedsAssociations(IEnumerable<BedAssociationViewModel> model, IEnumerable<DeviceDriver3BedLink> associations)
      {

         //foreach (BedAssociationViewModel bedAssociation in model.BedAssociation)
         //{

         //   DeviceDriver3BedLink association = associations.SingleOrDefault(x => x.BedId == bedAssociation.BedId && x.Bed.IdLocation == bedAssociation.LocationId);

         //   if (association != null)
         //   {
         //      bedAssociation.DriverSideBedName = association.DriverSideBedName;
         //      bedAssociation.Watchdog = association.WatchDogEnable;
         //      bedAssociation.Enabled = association.DriverEnabled;
         //   }
         //}

         List<BedAssociationViewModel> result = model.ToList();

         foreach (DeviceDriver3BedLink association in associations)
         {

            BedAssociationViewModel current = result.SingleOrDefault(x => x.BedId == association.BedId && x.LocationId == association.Bed.IdLocation);

            if (current != null)
            {
               current.DriverSideBedName = association.DriverSideBedName;
               current.Watchdog = association.WatchDogEnable;
               current.Enabled = association.DriverEnabled;
            }
         }

         return result;

      }

      public static void UpdateLogLevel(DeviceDriverViewModel model, Dictionary<int, bool> logLevels)
      {
         foreach (var level in model.LogLevels)
         {
            if (logLevels.ContainsKey((int)level.LogLevel))
            {
               level.Value = logLevels[(int)level.LogLevel];
            }
            else
            {
               level.Value = false;
            }
         }
      }


      public static void UpdateLogDestination(DeviceDriverViewModel model, Dictionary<int, bool> logDestinations)
      {
         foreach (var destination in model.LogDestinations)
         {
            if (logDestinations.ContainsKey((int)destination.LogDestination))
            {
               destination.Value = logDestinations[(int)destination.LogDestination];
            }
            else {
               destination.Value = false;
            }
         }
     
      }

      public static IEnumerable<DeviceDriverViewModel> BuildList(IEnumerable<DeviceDriver3> source, List<Bed> beds, IDictionaryService dictionarySrv)
{
   try
   {
      return source.Select(x => Build(x, beds, dictionarySrv));
   }
   catch (Exception)
   {

      throw;
   }
}
   }
}
