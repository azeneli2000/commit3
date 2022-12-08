using ConfiguratorWeb.App.Attributes;
using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class DriverEventCatalogViewModel
   {
      public DriverEventCatalogViewModel()
      {
         NewLevel = 0;
         NewClass = 0;
         TextENG = string.Empty;
         TextENGShort = string.Empty;
         TextUser = string.Empty;
         TextUserShort = string.Empty;
         DriverEventLevel = new DriverEventLevelViewModel();
         DriverEventClass = new DriverEventClassViewModel();

      }

      [TranslatedDisplayAttribute("Code")]
      public string Code { get; set; }

      [TranslatedDisplayAttribute("Level")]
      public short Level { get; set; }
      public string LevelDescription { get; set; }

      [TranslatedDisplayAttribute("Class")]
      public short Class { get; set; }
      public string ClassDescription { get; set; }

      [TranslatedDisplayAttribute("Short Text")]
      public string ShortText { get; set; }

      [TranslatedDisplayAttribute("Long Text")]
      public string LongText { get; set; }

      [TranslatedDisplayAttribute("New Level")]
      public int NewLevel { get; set; }
      public string NewLevelDescription { get; set; }

      [TranslatedDisplayAttribute("New Class")]
      public int NewClass { get; set; }
      public string NewClassDescription { get; set; }

      [TranslatedDisplayAttribute("Text ENG")]
      public string TextENG { get; set; }

      [TranslatedDisplayAttribute("Text ENG Short")]
      public string TextENGShort { get; set; }

      [TranslatedDisplayAttribute("Text User")]
      public string TextUser { get; set; }

      [TranslatedDisplayAttribute("Text User Short")]
      public string TextUserShort { get; set; }

      [UIHint("DriverEventLevelDropDownEditor")]
      public DriverEventLevelViewModel DriverEventLevel { get; set; }
      [UIHint("DriverEventClassDropDownEditor")]
      public DriverEventClassViewModel DriverEventClass { get; set; }

   }

   public class DriverEventLevelViewModel
   {
      public int LevelId { get; set; }
      public string LevelName { get; set; }

      public  DriverEventLevelViewModel()
      {
         LevelId = -2;
      }
   }

   public class DriverEventClassViewModel
   {
      public int ClassId { get; set; }
      public string ClassName { get; set; }

      public DriverEventClassViewModel()
      {
         ClassId = -2;
      }
   }
}