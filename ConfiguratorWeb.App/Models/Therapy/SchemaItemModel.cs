using Digistat.FrameworkStd.Model.Therapy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Digistat.FrameworkStd.Enums;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.Models.Therapy{

   // using System.Xml.Serialization;
   // XmlSerializer serializer = new XmlSerializer(typeof(StdActionSchema));
   // using (StringReader reader = new StringReader(xml))
   // {
   //    var test = (StdActionSchema)serializer.Deserialize(reader);
   // }

   [XmlRoot(ElementName = "schemaField")]
   public class SchemaField
   {

      [XmlAttribute(AttributeName = "name")]
      public string Name { get; set; }

      [XmlAttribute(AttributeName = "enabled")]
      public bool Enabled { get; set; }

      [XmlAttribute(AttributeName = "unit")]
      public string Unit { get; set; }
   }

   [XmlRoot(ElementName = "Schema")]
   public class Schema
   {

      [XmlElement(ElementName = "schemaField")]
      public List<SchemaField> SchemaField { get; set; }

      [XmlAttribute(AttributeName = "name")]
      public string Name { get; set; }

      [XmlAttribute(AttributeName = "description")]
      public string Description { get; set; }

      [XmlAttribute(AttributeName = "mask")]
      public int Mask { get; set; }

      [XmlAttribute(AttributeName = "allowedMask")]
      public string AllowedMask { get; set; }

      [XmlAttribute(AttributeName = "factor")]
      public double Factor { get; set; }
   }

   [XmlRoot(ElementName = "StdActionSchema")]
   public class StdActionSchema
   {

      [XmlElement(ElementName = "Schema")]
      public List<Schema> Schema { get; set; }
   }
}