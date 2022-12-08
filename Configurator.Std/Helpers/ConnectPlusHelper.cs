using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Configurator.Std.Helpers
{
   public class ConnectPlusHelper
   {
      public static string GetTableName(DriverRepository objDriver)
      {
         string strTableName = objDriver.DriverName + "_" + objDriver.DriverVersion;
         Regex rgx = new Regex("[^a-zA-Z0-9_]");
         strTableName = rgx.Replace(strTableName, "_");
         return "DAS_" + strTableName;
      }


      public static int GetParamType(string paramType)
      {
         int intRet = 0;
         switch (paramType.Trim().ToUpper())
         {
            case "NUMERIC":
            case "NM":
               intRet = 1;
               break;
            case "STRING":
               intRet = 2;
               break;
         }
         return intRet;
      }
   }
}
