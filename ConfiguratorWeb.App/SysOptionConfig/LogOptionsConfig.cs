using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   public class LogOptionsConfig
   {
      public string Serialize()
      {
         return new Digistat.FrameworkStd.UMSLegacy.UMSFrameworkLogOption().Serialize();
      }

      /// <summary>
      /// Deserialize from a string a LogOptions object
      /// </summary>
      /// <param name="options">string containing the LogOptions object</param>
      /// <returns>The LogOptions deserialized object.</returns>
      public static Tuple<bool,string> Deserialize(string options)
      {
         return new Digistat.FrameworkStd.UMSLegacy.UMSFrameworkLogOption().TestDeserializeObj(options);
      }
   }
}
