using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.Models.ReportMaster
{
	[XmlRoot(ElementName="Menu")]
	public class Menu {
		[XmlAttribute(AttributeName="Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName="Callback")]
		public string Callback { get; set; }
		[XmlAttribute(AttributeName="Path")]
		public string Path { get; set; }
		[XmlAttribute(AttributeName="Template")]
		public string Template { get; set; }
		[XmlAttribute(AttributeName="Preview")]
		public string Preview { get; set; }
		[XmlAttribute(AttributeName="Print")]
		public string Print { get; set; }
		[XmlAttribute(AttributeName="UseWindowsPrintDialog")]
		public string UseWindowsPrintDialog { get; set; }
		[XmlAttribute(AttributeName="WatermarkEnabled")]
		public string WatermarkEnabled { get; set; }
		[XmlAttribute(AttributeName="OutlineEnabled")]
		public string OutlineEnabled { get; set; }
		[XmlAttribute(AttributeName="PrintButtonEnabled")]
		public string PrintButtonEnabled { get; set; }
		[XmlAttribute(AttributeName="ExportFormats")]
		public string ExportFormats { get; set; }
		[XmlElement(ElementName="Parameters")]
		public Parameters Parameters { get; set; }
	}

	[XmlRoot(ElementName="Parameter")]
	public class Parameter {
		[XmlAttribute(AttributeName="Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName="Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName="Parameters")]
	public class Parameters {
		[XmlElement(ElementName="Parameter")]
		public List<Parameter> Parameter { get; set; }
	}
   [XmlRoot(ElementName="PrinterSettings")]
   public class PrinterSettings {
      [XmlAttribute(AttributeName="PrinterName")]
      public string PrinterName { get; set; }
      [XmlAttribute(AttributeName="Copies")]
      public string Copies { get; set; }
      [XmlAttribute(AttributeName="Duplex")]
      public string Duplex { get; set; }
      [XmlAttribute(AttributeName="Collate")]
      public string Collate { get; set; }
      [XmlAttribute(AttributeName="ShowDialog")]
      public string ShowDialog { get; set; }
   }

	[XmlRoot(ElementName="StorageSettings")]
	public class StorageSettings {
		[XmlAttribute(AttributeName="Path")]
		public string Path { get; set; }
		[XmlAttribute(AttributeName="Server")]
		public string Server { get; set; }
		[XmlAttribute(AttributeName="ShareName")]
		public string ShareName { get; set; }
		[XmlAttribute(AttributeName="User")]
		public string User { get; set; }
		[XmlAttribute(AttributeName="Password")]
		public string Password { get; set; }
	}

	[XmlRoot(ElementName="MailSettings")]
	public class MailSettings {
		[XmlAttribute(AttributeName="FromAddress")]
		public string FromAddress { get; set; }
		[XmlAttribute(AttributeName="FromName")]
		public string FromName { get; set; }
		[XmlAttribute(AttributeName="Body")]
		public string Body { get; set; }
		[XmlAttribute(AttributeName="Subject")]
		public string Subject { get; set; }
		[XmlAttribute(AttributeName="Format")]
		public string Format { get; set; }
		[XmlAttribute(AttributeName="Destinations")]
		public string Destinations { get; set; }
	}

	[XmlRoot(ElementName="UISettings")]
	public class UISettings {
		[XmlAttribute(AttributeName="ShowProgress")]
		public string ShowProgress { get; set; }
		[XmlAttribute(AttributeName="UsePrintDialog")]
		public string UsePrintDialog { get; set; }
		[XmlAttribute(AttributeName="PrintButton")]
		public string PrintButton { get; set; }
		[XmlAttribute(AttributeName="Outline")]
		public string Outline { get; set; }
		[XmlAttribute(AttributeName="WatermarkEnabled")]
		public string WatermarkEnabled { get; set; }
		[XmlAttribute(AttributeName="Watermark")]
		public string Watermark { get; set; }
		[XmlAttribute(AttributeName="ExportParameters")]
		public string ExportParameters { get; set; }
	}

	[XmlRoot(ElementName="ReportMaster")]
	public class ReportMasterViewModel {
		[XmlElement(ElementName="Menu")]
		public List<Menu> Menu { get; set; }
      [XmlElement(ElementName="PrinterSettings")]
      public PrinterSettings PrinterSettings { get; set; }
		[XmlElement(ElementName="StorageSettings")]
		public StorageSettings StorageSettings { get; set; }
		[XmlElement(ElementName="MailSettings")]
		public MailSettings MailSettings { get; set; }
		[XmlElement(ElementName="UISettings")]
		public UISettings UISettings { get; set; }
		[XmlElement(ElementName="ExportSettings")]
		public string ExportSettings { get; set; }
	}


}
