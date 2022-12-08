using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Model.CDSS;
using Digistat.FrameworkStd.Model.Integration.Telligence;

namespace ConfiguratorWeb.App.Models
{
   public class CDSSRuleViewModel
   {
      

      public CDSSRuleViewModel()
      {
         KillTimeout = 30;
         ValidityTimeout = 3600;
         Interval = 5000;
         IsActive = false;
         TriggerType = 3; //bug #5461: now is always multi
      }
      [TranslatedDisplayAttribute("ID")]
      public int ID { get; set; }

      [Required]
      [TranslatedDisplayAttribute("Name")]
      public string Name { get; set; }

      [TranslatedDisplayAttribute("Rule Type Id")]
      public short RuleType { get; set; }

      [TranslatedDisplayAttribute("Rule Type")]
      public string RuleTypeDescr { get; set; }

      [TranslatedDisplayAttribute("Trigger Type Id")]
      public short TriggerType { get; set; }

      [TranslatedDisplayAttribute("Trigger Type")]
      public string TriggerTypeDescr { get; set; }

      [Range(500, 86400000)]
      [TranslatedDisplayAttribute("Interval")]
      public int? Interval { get; set; }

      [TranslatedDisplayAttribute("Times")]
      public string Times { get; set; }
      
      [Range(1, 86400)]
      [TranslatedDisplayAttribute("Validity Timeout")]
      public int? ValidityTimeout { get; set; }

      [TranslatedDisplayAttribute("Data Type Rule")]
      public bool IsGeneric { get; set; }

      [TranslatedDisplayAttribute("Data Type ")]
      public String IsGenericDescr { get; set; }

      [TranslatedDisplayAttribute("Auto Activate")]
      public bool AutoActivate { get; set; }

      [TranslatedDisplayAttribute("DLL File name")]
      public string DllFile { get; set; }      
      [TranslatedDisplayAttribute("DLL Rule name")]
      public string DllRuleName { get; set; }

      [TranslatedDisplayAttribute("Method Code")]
      public string MethodCode { get; set; }

      [DefaultValue(30)]
      [Range(1, 30)]
      [TranslatedDisplayAttribute("Kill timeout")]
      public int KillTimeout { get; set; }

      [TranslatedDisplayAttribute("Execute at startup")]
      public bool? ExecuteAtStartup { get; set; }

      /// TODO: Check if necessary!!!
      public int PatientID { get; set; }

      
      [TranslatedDisplayAttribute("Message Type")]
      public string MessageType { get; set; }

      [TranslatedDisplayAttribute("Is Test Rule")]
      public bool IsTest { get; set; }

      [TranslatedDisplayAttribute("Active")]
      public bool IsActive { get; set; }
      [TranslatedDisplayAttribute("Description")]
      public string Description { get; set; } // nvarchar(max)
      [TranslatedDisplayAttribute("Code")]
      public string Code { get; set; } // nvarchar(5)
      [TranslatedDisplayAttribute("Available on clients")]
      public bool ClientVisible { get; set; } // bit
      [TranslatedDisplayAttribute("Editable on clients")]
      public bool ClientEditable { get; set; } // bit
      [TranslatedDisplayAttribute("Configurable on clients")]
      public bool ClientConfigurable { get; set; } // bit
      [TranslatedDisplayAttribute("Uri")]
      public string Uri { get; set; } // nvarchar(500)
      [TranslatedDisplayAttribute("Copyable on clients")]
      public bool? ClientCopyable { get; set; } // bit

      public string Options { get; set; }
      public string OutputParameters { get;  set; }
      
      public virtual string Locations { get; set; }
   }
}
