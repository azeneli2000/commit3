using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.DiaryWeb
{
   public class Tag
   {
      public int IDtag { get; set; }
      public string TextTag { get; set; }
      public string ColorTag { get; set; }
      public int IndexTag { get; set; }
      public bool IsActiveTag { get; set; }
      public bool IsSystemTag { get; set; }
   }
}
