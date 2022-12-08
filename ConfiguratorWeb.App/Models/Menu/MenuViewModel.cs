using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class MenuViewModel
   {
      public int Id { get; set; }
      public int? ParentId { get; set; }
      public string Text { get; set; }
      public string Description { get; set; }
      public bool Enabled { get; set; }
      public ActionUrl Url { get; set; }
   }
}
