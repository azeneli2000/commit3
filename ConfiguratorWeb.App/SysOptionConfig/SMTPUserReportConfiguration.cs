using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   [Serializable]
   public class SMTPUserReportConfiguration
   {
      [XmlElement("Group")]
      public Group[] Groups { get; set; }

      [Serializable]
      public class Group
      {
         [XmlAttribute]
         public string LocationID { get; set; }

         [XmlElement]
         public string EmailSubject { get; set; }

         [XmlElement]
         public string EmailBody { get; set; }

         [XmlElement]
         public EmailSender EmailSender { get; set; }

         [XmlArray]
         public EmailRecipient[] EmailRecipients { get; set; }
      }

      [Serializable]
      public class EmailSender
      {
         [XmlAttribute]
         public string Name { get; set; }
         [XmlAttribute]
         public string Email { get; set; }
      }

      [Serializable]
      [XmlType(TypeName = "Recipient")]
      public class EmailRecipient
      {
         [XmlAttribute]
         public string Name { get; set; }
         [XmlAttribute]
         public string Email { get; set; }
      }


      public static SMTPUserReportConfiguration Deserialize(string xml)
      {
         SMTPUserReportConfiguration objConfig = null;

         if (!string.IsNullOrEmpty(xml))
         {
            XmlSerializer objSerializer = new XmlSerializer(typeof(SMTPUserReportConfiguration));
            System.IO.StringReader objReader = new System.IO.StringReader(xml);
            objConfig = (SMTPUserReportConfiguration)objSerializer.Deserialize(objReader);

         }
         else
         {
            objConfig = new SMTPUserReportConfiguration();
         }
         return objConfig;
      }


      public static string Serialize(SMTPUserReportConfiguration config)
      {
         string strRet = null;
         XmlWriterSettings settings = new XmlWriterSettings();
         settings.OmitXmlDeclaration = true;
         XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

         XmlSerializer xsSubmit = new XmlSerializer(typeof(SMTPUserReportConfiguration));
         using (System.IO.StringWriter objWriter = new System.IO.StringWriter())
         {
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(objWriter, settings))
            {

               var xmlns = new XmlSerializerNamespaces();
               xmlns.Add(string.Empty, string.Empty);

               xsSubmit.Serialize(writer, config, emptyNamespaces);
               strRet = objWriter.ToString(); // Your XML
            }
         }
         return strRet;
      }
   }

}
