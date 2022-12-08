using System;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.Models.Vitals
{
    [Serializable]
    public class OCRDeviceParamViewModel
    {
        [XmlElement(ElementName = "ParamID")]
        public int Id { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Unit")]
        public string Unit { get; set; }
    }
}