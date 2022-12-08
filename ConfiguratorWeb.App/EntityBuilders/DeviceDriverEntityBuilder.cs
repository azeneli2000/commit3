using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Extensions;
using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Serialization;


namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class DeviceDriverEntityBuilder
   {
      public static DeviceDriver3 Build(DeviceDriverViewModel source, DriverCommConfiguation defaultRepositoryConfiguration)
      {
         DeviceDriver3 objDest = null;
         try
         {
            if (source != null)
            {
               CommConfiguration config = new CommConfiguration {
                  ConnectionType = source.ConnectionType,
                  ComPort = source.SerialPort == null ? 0 : source.SerialPort.SerialPort,
                  DataBits = source.SerialPort == null ? 0 : Convert.ToInt32(source.SerialPort.DataBits),
                  Baud = source.SerialPort == null ? 0 : Convert.ToInt32(source.SerialPort.BitsPerSeconds),                  
                  HandShake = source.SerialPort == null ? 0 : (int)source.SerialPort.Handshake,
                  Parity = source.SerialPort == null ? 0 : (int)source.SerialPort.Parity,
                  StopBits = source.SerialPort == null ? "0" : ((int)source.SerialPort.StopBits).ToString(),
                  ReceivingDataMode = source.SerialPort == null ? 0 : Convert.ToInt32(source.SerialPort.DataModeId),
                  SmartCableId = source.SerialPort == null ? string.Empty : source.SerialPort.SmartCableID ?? string.Empty,
                  Hostname = source.Socket == null ? string.Empty : source.Socket.HostName ,
                  SocketPort = source.Socket == null ? 0 : source.Socket.Port,
                  TCPCommType = source.Socket == null ? 0 : Convert.ToInt32(source.Socket.SocketType),        
                  SupportedCommConnectionTypes = string.Empty,
                  SupportedDriverTypes = string.Empty,
                  CustomParameters = source.CustomParameters.Select(x => new CustomParam { Name = x.Name, Description = x.Description, Value = x.Value }).ToList(),
                  DtrEnabled = defaultRepositoryConfiguration.DtrEnabled,
                  RtsEnabled = defaultRepositoryConfiguration.RtsEnabled,
                  USBProducerId = defaultRepositoryConfiguration.USBProducerId,
                  USBSerialId = defaultRepositoryConfiguration.USBSerialId,
                  USBVendorId = defaultRepositoryConfiguration.USBVendorId
               };

               LogConfiguration logConfig = new LogConfiguration
               {
                  LogParameters = new LogParam
                  {
                     Destination = ConversionsHelper.BitArrayToBitMask(source.LogDestinations.ToDictionary(x => (int)x.LogDestination, x => x.Value)),
                     Level = ConversionsHelper.BitArrayToBitMask(source.LogLevels.ToDictionary(x => (int)x.LogLevel, x => x.Value)),
                  }
               };

               objDest = new DeviceDriver3
               {
                  Id = source.Id,
                  DriverType = source.DriverType,
                  ComputerName = source.ComputerName ?? string.Empty,
                  DeviceName = source.DeviceName ?? string.Empty,
                  AutoStartWatchDog = source.AutoStartWatchDog,
                  AutoStartDriver = source.AutoStartDriver,
                  LogEnabled = source.LogEnabled,
                  AlarmSystemType = source.AlarmSystemType,
                  DataRate = source.DataRate == 0 ? (int?)null : source.DataRate,
                  SendDataToMC = source.SendDataToMC,
                  SQLPatientResolve = source.SQLPatientResolve ?? string.Empty,
                  ForceSendDataWithoutPatient = source.ForceSendDataWithoutPatient,
                  PatientResolveNotCached = source.PatientResolveNotCached,
                  IdDriverRepository = source.IdDriverRepository,
                  CommConfiguration = SerializationExtensions.XmlSerialize(config),
                  LogConfig = SerializationExtensions.XmlSerialize(logConfig),                  

               };
               if (source.BedAssociation != null)
               {
                  var bedLinks = source.BedAssociation.Where(x => x.Enabled)
                     .Select(x => new DeviceDriver3BedLink
                     {                     
                        BedId = x.BedId,
                        DeviceDriverId = objDest.Id,
                        DeviceDriver3 = objDest,
                        DriverEnabled = x.Enabled,
                        WatchDogEnable = x.Watchdog,
                        WatchDogEnabled = x.Watchdog,
                        DriverSideBedName = x.DriverSideBedName ?? string.Empty
                     }).ToList();

                  objDest.BedLinks = bedLinks;   
               }
               

            }
         }
         catch
         {
            throw;
         }

         return objDest;
      }

      //public static IEnumerable<DeviceDriver3> BuildList(IEnumerable<DeviceDriverViewModel> source)
      //{
      //   try
      //   {
      //      return source.Select(Build);
      //   }
      //   catch (Exception)
      //   {

      //      throw;
      //   }
      //}
   }
}
