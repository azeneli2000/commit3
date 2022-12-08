using System.Xml.Serialization;

namespace ConfiguratorWeb.App.Models.Vitals
{
    [XmlType("OCRDevice")]
    public class OCRDeviceViewModel
    {
        [XmlElement(ElementName = "ID")]
        public int ID { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlArray("Parameters"), XmlArrayItem("Param")]
        public OCRDeviceParamViewModel[] OCRParameters { get; set; }
    }
}