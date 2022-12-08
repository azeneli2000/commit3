using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Configurator.Std.BL.DasDrivers
{
   public static class SmartCentralTagHelper
   {

      public static Dictionary<string, string> GetContructTags()
      {
         Dictionary<string, string> objRet = new Dictionary<string, string>();
         objRet.Add("CONSTRUCTS", string.Empty);
         objRet.Add("if", "\\if(){}else{}");
         objRet.Add("translate", "{ }");
         objRet.Add("contraction", "\\?{extended text|short text}");
         return objRet;
      }

      public static Dictionary<string, string> GetTextFormatTags()
      {
         Dictionary<string, string> objRet = new Dictionary<string, string>();
         objRet.Add("TEXT FORMAT", string.Empty);
         objRet.Add("style 1", "\\s1");
         objRet.Add("style 2", "\\s2");
         objRet.Add("style 3", "\\s3");
         objRet.Add("color", "\\cXXXXXX");
         return objRet;
      }


      public static Dictionary<string, string> GetStartOfLineTags()
      {
         Dictionary<string, string> objRet = new Dictionary<string,string>();
         objRet.Add("START OF LINE", string.Empty);
         objRet.Add("required", "\\rX");
         objRet.Add("optional", "\\eX");
         return objRet;
      }

   }
}
