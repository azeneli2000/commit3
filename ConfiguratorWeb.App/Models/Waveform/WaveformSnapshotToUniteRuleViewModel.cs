using System.ComponentModel.DataAnnotations;
using Digistat.FrameworkStd.Model;

namespace ConfiguratorWeb.App.Models.OnLine
{
   public class WaveformSnapshotToUniteRuleViewModel
   {
      public WaveformSnapshotToUniteRuleViewModel()
      {
         Priority = 99999;
         IdParam = -1;
      }
      
      public int Id { get; set; }
      /// <summary>
      /// Lower the value, higher the priority
      /// </summary>      
      public int Priority { get; set; }
      /// <summary>
      /// Driver id taken from DriverRepository (where current = 1)
      /// </summary>
      [Required]
      public string IdDriver { get; set; }
      /// <summary>
      /// Location.LocationRef, NULL means All
      /// </summary>
      public int? IdLocation { get; set; }
      /// <summary>
      /// DriverRepositoryEventCatalog.lnk_evn_Id, NULL means All
      /// </summary>
      public string IdLinkEvent { get; set; }
      /// <summary>
      /// Waveform parameter present in DriverRepositoryStandardParameterLink
      /// Or 0 (means all the variable content waveforms of the selected driver)
      /// A parameter is a waveform if StandardParameter.par_DataType = WAVEFORM
      /// A parameter is a variable content waveform if StandardParameter.par_IsVariableContentWaveform = 1
      /// </summary>
      [Required]
      public int IdParam { get; set; }
      /// <summary>
      /// Need to insert the label if the parameter is a variable content waveform or ParamId = 0
      /// </summary>
      public string Description { get; set; }
      public string DriverName { get; set; }
      public string EventName { get; set; }
      public virtual StandardParameter Parameter { get; set; }
      public virtual Location Location { get; set; }
   }
}
