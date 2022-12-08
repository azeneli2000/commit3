using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   public class AlarmAggregator
   {
      [NonSerialized]
      private System.Text.RegularExpressions.Regex mobjRegex = null;

      [ReadOnly(true), Browsable(false)]
      public System.Text.RegularExpressions.Regex CompiledRegex
      {
         get { return mobjRegex; }
      }

      public string MatchingRegex { get; set; }
      public string ResultString { get; set; }


      public AlarmAggregator()
      {

      }

      public override string ToString()
      {
         return ResultString;
      }
      public void CompileRegex()
      {
         mobjRegex = new System.Text.RegularExpressions.Regex(MatchingRegex, System.Text.RegularExpressions.RegexOptions.Compiled);
      }
   }

   public class AlarmAggregators
   {
      public List<AlarmAggregator> Aggregators { get; set; }

      public AlarmAggregators()
      {
         Aggregators = new List<AlarmAggregator>();
      }

      public static AlarmAggregators Deserialize(string xml)
      {
         AlarmAggregators objConfig = null;

         if (!string.IsNullOrEmpty(xml))
         {
            XmlSerializer objSerializer = new XmlSerializer(typeof(AlarmAggregators));
            System.IO.StringReader objReader = new System.IO.StringReader(xml);
            objConfig = (AlarmAggregators)objSerializer.Deserialize(objReader);
         }
         else
         {
            objConfig = new AlarmAggregators();
         }
         foreach (AlarmAggregator aggregator in objConfig.Aggregators)
         {
            aggregator.CompileRegex();
         }
         return objConfig;
      }

      public string Serialize()
      {
         string strResult = string.Empty;
         XmlSerializer objSerializer = new XmlSerializer(typeof(AlarmAggregators));
         System.IO.StringWriter objWriter = new System.IO.StringWriter();
         objSerializer.Serialize(objWriter, this);
         strResult = objWriter.ToString();
         objWriter.Dispose();
         return strResult;
      }

      public string AggregateString(string text)
      {
         string strResult = text;

         if (this.Aggregators.Count > 0)
         {
            foreach (AlarmAggregator aggregator in this.Aggregators)
            {
               if (aggregator.CompiledRegex.IsMatch(text))
               {
                  strResult = aggregator.ResultString;
                  break;
               }
            }
         }
         return strResult;
      }
   }

}
