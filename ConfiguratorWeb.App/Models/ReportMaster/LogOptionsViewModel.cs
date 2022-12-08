using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.Models.ReportMaster
{
   
   [XmlRoot(ElementName="LogOptions")]
   public class LogOptionsViewModel {
      [XmlElement(ElementName="LogToFile")]
      public string LogToFile { get; set; }
      [XmlElement(ElementName="LogToDB")]
      public string LogToDB { get; set; }
      [XmlElement(ElementName="TelnetPort")]
      public string TelnetPort { get; set; }
      [XmlElement(ElementName="LogLevel")]
      public string LogLevel { get; set; }
      [XmlElement(ElementName="MailServer")]
      public MailServer MailServer { get; set; }
      [XmlElement(ElementName="MailSubject")]
      public string MailSubject { get; set; }
      [XmlElement(ElementName="MailFromAddress")]
      public string MailFromAddress { get; set; }
      [XmlElement(ElementName="MailFromName")]
      public string MailFromName { get; set; }
      [XmlElement(ElementName="CodeToLog")]
      public CodeToLog CodeToLog { get; set; }
      [XmlElement(ElementName="EMailDestinations")]
      public EMailDestinations EMailDestinations { get; set; }
   }
   [XmlRoot(ElementName="MailServer")]
   public class MailServer {
      [XmlElement(ElementName="SMTPHost")]
      public string SMTPHost { get; set; }
      [XmlElement(ElementName="SMTPPort")]
      public string SMTPPort { get; set; }
      [XmlElement(ElementName="SMTPUseSSL")]
      public string SMTPUseSSL { get; set; }
      [XmlElement(ElementName="SMTPUsername")]
      public string SMTPUsername { get; set; }
      [XmlElement(ElementName="SMTPPassword")]
      public string SMTPPassword { get; set; }
   }

   [XmlRoot(ElementName="CodeToLog")]
   public class CodeToLog {
      [XmlElement(ElementName="code")]
      public List<string> Code { get; set; }
   }

   [XmlRoot(ElementName="EMailDestinations")]
   public class EMailDestinations {
      [XmlElement(ElementName="destination")]
      public List<string> Destination { get; set; }
   }

}
