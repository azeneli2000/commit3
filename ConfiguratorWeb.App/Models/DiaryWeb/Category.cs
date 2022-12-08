using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.DiaryWeb
{
   public class Category
   {
      public int ID { get; set; }
      public string Text { get; set; }
      public string Color { get; set; }
      public int LocationID { get; set; }

      public string LocationName { get; set; }

      public int Index { get; set; }

      public bool IsActive { get; set; }
      public bool IsSystem { get; set; }
      public IEnumerable<Subject> Subjects { get; set; } = null;
      public IEnumerable<CategoryPhrases> Phrases { get; set; } = null;
   }
}
