using ConfiguratorWeb.App.Enums;
using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Attributes;

namespace ConfiguratorWeb.App.Models.OnLine
{
   public class QueryParameterViewModel
   {
      
      public int QueryID { get; set; }



      public string ParameterLabel { get; set; }
      public string UnitLabel { get; set; }
      public string Description { get; set; }
      [Required]
      [TranslatedDisplay("Sql Statement")]
      public string SQLQuery { get; set; }
      public string LastSaveUserID { get; set; }
      public DateTime? LastSaveDatetime { get; set; }

      public virtual string LastSaveUserName { get; set; }
   }
}
