using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class NetworkViewModel
   {
      public NetworkViewModel()
      {
         DefaultLocation = new LocationViewModel();
         Location = new LocationViewModel();
      }
      public int Id { get; set; }
      public string HostName { get; set; }
      public NetworkTypeEnum Type { get; set; }

      public string TypeDescription { get; set; }

      public short TypeShort { get; set; }

      public int? LocationID { get; set; }
      public int? BedID { get; set; }
      public bool LockBed { get; set; }
      public string Modules { get; set; }
      public bool AllowEdit { get; set; }
      [DisplayName("Workstation Label")]
      public string WorkstationLabel { get; set; }
      public string CurrentContactRef { get; set; }
      public string UserId { get; set; }
      public int? UserVersion { get; set; }
      public DateTime? LastConnection { get; set; }
      public bool? IsEnabled { get; set; }
      public string CultureInfo { get; set; }
      public string MacAddress { get; set; }
      public string IpAddress { get; set; }
      public string WebModules { get; set; }
      public string WebMenu { get; set; }
      public string ControlbarCurrentVersion { get; set; }
      public string CurrentProductName { get; set; }
      public string CurrentProductVersion { get; set; }
      public string DeployProductName { get; set; }
      public string DeployProductVersion { get; set; }

      public LocationViewModel Location { get; }
      public IEnumerable<BedViewModel> BedList { get; set; }

      public LocationViewModel DefaultLocation { get; set; }

   }
}
