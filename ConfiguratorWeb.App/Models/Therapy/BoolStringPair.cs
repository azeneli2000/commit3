using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{

   public class BoolStringPair
   {
      public BoolStringPair()
      {

      }
      public string Name { get; set; }
      public bool Enabled { get; set; }

      public BoolStringPair(string name, bool enabled = false)
      {
         Name = name;
         Enabled = enabled;
      }

      public static List<BoolStringPair> Build(string strAllowedValues, string strSelectedValues)
      {
         var ret = new List<BoolStringPair>();

         if (!string.IsNullOrWhiteSpace(strAllowedValues))
         {
            var aAllowedValues = strAllowedValues.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string val in aAllowedValues)
            {
               var item = new BoolStringPair(val);

               if ( !string.IsNullOrWhiteSpace(strSelectedValues) && val == strSelectedValues)
               {
                  item.Enabled = true;
               }

               ret.Add(item);
            }
         }

         return ret;
      }

      public static void GetAllowed(List<BoolStringPair> objPairs, out string strAllowed, out string strSelected)
      {
         strAllowed = string.Empty;
         strSelected = string.Empty;

         if (objPairs != null)
         {
            var strBuilder = new StringBuilder();
            foreach (var pair in objPairs)
            {
               
               strBuilder.Append(pair.Name+',');
               if (pair.Enabled)
               {
                  strSelected = pair.Name;
               }
            }
            strAllowed = strBuilder.ToString();
            if (strAllowed != null && strAllowed.Length > 0 && strAllowed[strAllowed.Length - 1] == ',')
            {
               strAllowed = strAllowed.Remove(strAllowed.Length - 1);
            }
         }
      }
   }
}
