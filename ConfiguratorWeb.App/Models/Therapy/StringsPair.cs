using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
   public class StringsPair
   {
      public StringsPair()
      {

      }

      public string Name { get; set; }
      public string Description { get; set; }

      public StringsPair(string name, string description )
      {
         Name = name;
         Description = description;
      }
   }
}
