using System.Collections.Generic;
using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums;
using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
   public class SystemOptionViewModel
   {

      [TranslatedDisplayAttribute("Application")]
      public string Application { get; set; }
      [DataType(DataType.MultilineText)]
      [TranslatedDisplayAttribute("Description")]
      public string Description { get; set; }
      public string GUID { get; set; }
      public string HospitalUnitGUID { get; set; }
      public string HostName { get; set; }
      public bool? IsSystem { get; set; }
      public int? Level { get; set; }
      public double? LowerLimit { get; set; }
      public string Name { get; set; }
      protected SystemOptionsRowViewModel Row { get; set; }
      public OptionType Type { get; set; }
      public double? UpperLimit { get; set; }
      [TranslatedDisplayAttribute("User Name")]
      public string UserName { get; set; }
      public string Value { get; set; }
      public double ValueAsDouble { get; set; }
      public int ValueAsInteger { get; set; }

      public string ValueDisplayBinary { get; set; }

      [TranslatedDisplayAttribute("Hospital Unit")]
      public HospitalUnitViewModel HospitalUnit { get; set; }

      public UserViewModel User { get; set; }

      [TranslatedDisplayAttribute("Hospital Unit")]
      public string HospitalUnitName { get { return (HospitalUnit != null ? HospitalUnit.ShortName : string.Empty); } }

      public string UserFullName { get { return (User != null ? "[" + User.Abbrev + "] " + User.FirstName + " " + User.LastName : string.Empty); } }
   }



   public static class SystemOptionViewModelExtensions
   {
      // source taken on :
      //http://tymoteuszkestowicz.com/2013/11/en-kendo-grid-does-not-always-work-well/
      private static readonly IDictionary<string, string[]> sortMappings;
      private static readonly IDictionary<string, string[]> groupMappings;
      private static readonly IDictionary<string, string[]> filterMappings;

      static SystemOptionViewModelExtensions()
      {

         sortMappings = new Dictionary<string, string[]>()
         {
            {"UserName", new []{"UserAbbreviation"}},
            {"HospitalUnitName", new []{"HospitalUnit.ShortName"}},
            {"UserFullName", new []{"User.Abbrev", "User.FirstName","User.LastName"}},

         };

         groupMappings = new Dictionary<string, string[]>()
         {
            {"UserName", new []{"UserAbbreviation"}},
            {"HospitalUnitName", new []{"HospitalUnit.ShortName"}},
            {"UserFullName", new []{"User.Abbrev", "User.FirstName","User.LastName"}},
         };

         //{"UserName", "UserAbbreviation"},
         //{"HospitalUnitName", "HospitalUnit.ShortName"}
         filterMappings = new Dictionary<string, string[]>()
         {

            {"UserName", new []{"UserAbbreviation"}},
            {"HospitalUnitName", new []{"HospitalUnit.ShortName"}},
            {"UserFullName", new []{"User.Abbrev", "User.FirstName","User.LastName"}},
         };


       
      }

      public static IDictionary<string, string[]> SortMappings { get { return sortMappings; } }

      public static IDictionary<string, string[]> GroupMappings { get { return groupMappings; } }

      public static IDictionary<string, string[]> FilterMappings { get { return filterMappings; } }
   }
}
