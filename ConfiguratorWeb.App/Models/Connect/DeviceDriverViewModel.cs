using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ConfiguratorWeb.App.Models
{
   public class DeviceDriverViewModel
   {

      //Used by Razor
      public DeviceDriverViewModel(){
         CustomParameters = new List<CustomParametersViewModel>();
      }

         public DeviceDriverViewModel(List<Bed> beds, bool useNoneValue = false)
      {
         AutoStartDriver = true;
         SendDataToMC = true;

         SerialPort = new DeviceDriverSerialPortViewModel();
         Socket = new DeviceDriverSocketViewModel();
         CustomParameters = new List<CustomParametersViewModel>();

         LogDestinations = Enum.GetValues(typeof(LogDestination)).OfType<LogDestination>().Where(x => useNoneValue || x != LogDestination.None).Select(x => new LogDestinationViewModel
         {
            Label = x.GetDisplayAttribute(),
            LogDestination = x,
            Value = x == LogDestination.File //Default: only File selected
         })
         .ToList();


         LogLevels = Enum.GetValues(typeof(DASLogLevel)).OfType<DASLogLevel>().Where(x => useNoneValue || x != DASLogLevel.None).Select(x => new LogLevelViewModel
         {
            Label = x.GetDisplayAttribute(),
            LogLevel = x,
            Value = x != DASLogLevel.Raw //Default: all but raw are selected
         })
         .ToList();
         //Added check for IdLocation!=null because is a foreignKey
         BedAssociation = beds.Where(w=>w.IdLocation!=null).Select(x => new BedAssociationViewModel
         {
            BedId = x.Id,
            BedIndex = x.Index??0,
            LocationId = x.IdLocation.Value,
            Bedcode = x.BedCode,
            BedName = x.Name,
            Location = x.Location.LocationName,
            DriverSideBedName = string.Empty,
            Watchdog = false,
            Enabled = false,
         }).OrderBy(x => x.Location).ThenBy(x => x.BedIndex);
      }

      public int Id { get; set; }

      //[UIHint("DriverTypeEditor")]
      //public DriverType DriverType { get; set; }

      public short DriverType { get; set; }

      public string ComputerName { get; set; }

      public string DeviceName { get; set; }

      public string SQLPatientResolve { get; set; }

      //public string CommConfiguration { get; set; }

      public bool AutoStartWatchDog { get; set; }

      //[Display(Name = "Log Enabled")]
      public bool LogEnabled { get; set; }

      public string IdDriverRepository { get; set; }

      public bool AutoStartDriver { get; set; }

      public int DataRate { get; set; }

      //public string LogConfig { get; set; }

      public bool SendDataToMC { get; set; }

      public bool ForceSendDataWithoutPatient { get; set; }

      public bool PatientResolveNotCached { get; set; }

      public short AlarmSystemType { get; set; }

      //[UIHint("DeviceDriverListEditor")]
      //public string Name { get; set; }
      //public string Version { get; set; }

      //[UIHint("ConnectionTypeListEditor")]
      public int ConnectionType { get; set; }

      public DeviceDriverSerialPortViewModel SerialPort { get; set; }

      public DeviceDriverSocketViewModel Socket { get; set; }

      public IEnumerable<CustomParametersViewModel> CustomParameters { get; set; }
      public string CustomParametersJson { get; set; }

      public IEnumerable<BedAssociationViewModel> BedAssociation { get; set; }

      public List<LogDestinationViewModel> LogDestinations { get; set; }
      public List<LogLevelViewModel> LogLevels { get; set; }

      public string BedAssociationChanged { get; set; }

      public string BedLinkAssociationSerialize
      {
         get;
         set;
         //{
         //   return JsonConvert.SerializeObject(this.BedLinkAssociation.Select(x => new DeviceDriver3BedLink
         //      {                     
         //         BedId = x.BedId,
         //         DeviceDriverId = x.DeviceDriverId,
         //         DriverEnabled = x.DriverEnabled,
         //         WatchDogEnable = x.WatchDogEnable,
         //         WatchDogEnabled = x.WatchDogEnabled,
         //         DriverSideBedName = x.DriverSideBedName 
         //      })
         //      //,new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All 
         //      //   ,PreserveReferencesHandling = PreserveReferencesHandling.Objects
         //      //   ,Formatting = Formatting.Indented
         //      //}
         //      );
         //}

      }
      //public virtual IEnumerable<DeviceDriverBedLinkViewModel> BedLinks { get; set; }

      //public string Address { get; set; }

      //public string BedLink { get; set; }
   }
}