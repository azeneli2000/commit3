using ConfiguratorWeb.App.Models.Vitals;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class OCRDeviceViewModelBuilder
    {
        [XmlRoot("OCRDevices")]
        public sealed class OCRDevices : List<OCRDeviceViewModel>
        {
        }

        public static OCRDeviceViewModel[] BuildList(string source)
        {
            OCRDevices objDest = null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OCRDevices));
                using (StringReader objTextReader = new StringReader(source))
                {
                    objDest = (OCRDevices)xmlSerializer.Deserialize(objTextReader);
                }
            }

            return objDest?.ToArray() ?? new OCRDeviceViewModel[] { };
        }
    }
}