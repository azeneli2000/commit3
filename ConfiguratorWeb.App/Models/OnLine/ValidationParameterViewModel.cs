using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.OnLine
{
   public class ValidationParameterViewModel
   {
      private string driverID = "";
      private int deviceID=0;

      public ValidationParameterViewModel()
      {


      }

      public int GroupID { get; set; }

      public int ParameterID { get; set; }

      public string DriverID { get => driverID ?? ""; set => driverID = value; }
      public int DeviceID { get => deviceID; set => deviceID = value; }

      public string ChannelID { get; set; }

      public bool IsMandatory { get; set; }

      public bool IsAlwaysVisible { get; set; }

      public bool IsEditable { get; set; }

      public double? RangeMin { get; set; }

      public double? RangeMax { get; set; }

      public string SQLQuery { get; set; }

      public bool IsAutoExecuteQuery { get; set; }

      public bool IsManuallyAlarmable { get; set; }

      public int? Index { get; set; }

      public double? PlausibilityMin { get; set; }

      public double? PlausibilityMax { get; set; }

      public string SimpleChoiceGroup { get; set; }

      public int UnitOfMeasureID { get; set; }

      public short? Decimal { get; set; }

      public bool? IsSimpleChoiceOnly { get; set; }

      public int? SectionID { get; set; }


      //StdParameter Data
      public string ParameterDescription { get; set; }
      public string ParameterPrint { get; set; }
      public string ParameterMnemonic { get; set; }
      public string ParameterDataType { get; set; }

      //StdUnit Data
      public string UnitDescription { get; set; }
      public string UnitPrint { get; set; }

      //Section
      public string SectionDescription { get; set; }
      public int SectionIndex { get; set; }
      public string DriverName { get; set; }
   }
}
