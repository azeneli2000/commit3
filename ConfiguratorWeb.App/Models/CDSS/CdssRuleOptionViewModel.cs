using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.CDSS
{
   public class CdssRuleOptionViewModel:Digistat.FrameworkStd.Model.CDSS.CDSSRuleOption   
   {
      public string uid { get; set; }
      public int typeOptions { get; set; }
   }
}
