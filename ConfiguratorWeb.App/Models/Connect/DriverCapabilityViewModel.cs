using ConfiguratorWeb.App.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class DriverCapabilityViewModel
    {
      public string DriverRepositoryId { get; set; }
      public int IdParameter { get; set; }
      public int IDUnit { get; set; }

      [DisplayName("Device ID")]
      public string DeviceID { get; set; }

      [DisplayName("Device Text")]
      public string DeviceText { get; set; }

      [DisplayName("Device Unit Text")]
      public string DeviceUnitText { get; set; }
      public string Name { get; set; }
      public string Unit { get; set; }
      public string Mnemonic { get; set; }

      public int Sporadic { get; set; }
      public string Type { get; set; }

      public string TypeShort { get; set; }

      public bool Enabled { get; set; }

      public bool MustBeSaved { get; set; }

      public int StandardDeviceTypeID { get; set; }

      public string StandardParameterDataType { get; set; }

      public string StandardParameterPrint { get; set; }

      public string StandardParameterIDAlias { get; set; }

      public bool StandardParameterIsMissing { get; set; }
      
      public bool StandardParameterIsWaveForm { get; set; }

      //[UIHint("SporadicDropDownEditor")]
      //public string SporadicName { get; set; }

      [UIHint("SporadicDropDownEditor")]
      public SporadicViewModel SporadicModel
      {
         get;
         set;
      }
   }

   public class SporadicViewModel
   {
      public int SporadicId { get; set; }
      public string SporadicName { get; set; }

   }


}