using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.DiaryWeb
{
   public class Subject
   {
      public int ID { get; set; }
      public int Category { get; set; }
      public string Text { get; set; }

      public int Index { get; set; }

      public bool IsActive { get; set; }

     public IEnumerable<SubjectPhrase> SubjectsPhrases { get; set; } = null;


        public bool isEditable { get; set; }
   }
}
