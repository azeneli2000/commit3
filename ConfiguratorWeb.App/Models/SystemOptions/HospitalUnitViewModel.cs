using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
   public class HospitalUnitViewModel
   {
      public string GUID { get; set; }
      public int Version { get; set; }
      public bool Current { get; set; }
      public string ValidToDate { get; set; }
      public string rc_ID { get; set; }
      public int? rc_Version { get; set; }
      [TranslatedDisplayAttribute("Beeper")]
      public string Beeper { get; set; }
      [TranslatedDisplayAttribute("Cell Phone")]
      public string CellPhone { get; set; }
      [TranslatedDisplayAttribute("Code")]
      public string Code { get; set; }
      protected string CompareValue { get; }
      public string CostUnit { get; set; }
      [Required]
      public string Description { get; set; }
      [TranslatedDisplayAttribute("External Key")]
      public string ExternalKey { get; set; }
      [TranslatedDisplayAttribute("Inherits Slots")]
      public bool InheritsSlots { get; set; }
      [TranslatedDisplayAttribute("Mail")]
      public string Mail { get; set; }
      [TranslatedDisplayAttribute("Hospital Unit Name")]
      [Required]
      public string Name { get; set; }
      public HospitalUnitViewModel Parent { get; set; }
      public string ParentGUID { get; set; }
      [TranslatedDisplayAttribute("Phone")]
      public string Phone { get; set; }
      protected HospitalUnitRowViewModel Row { get; set; }
      [TranslatedDisplayAttribute("Short Name")]
      [Required]
      public string ShortName { get; set; }
      [TranslatedDisplayAttribute("Type")]
      public HospitalUnitType Type { get; set; }
      [TranslatedDisplayAttribute("Parent Unit")]
      public string ParentUnit { get; set; }
   }
   
}
