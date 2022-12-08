using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.Models.SystemOptions
{

   [XmlRoot(ElementName="Module")]
   public class Module {
      [XmlAttribute(AttributeName="Name")]
      public string Name { get; set; }
      [XmlAttribute(AttributeName="Callback")]
      public string Callback { get; set; }
      [XmlAttribute(AttributeName="Address")]
      public string Address { get; set; }
      [XmlAttribute(AttributeName="IncludeLogin")]
      public string IncludeLogin { get; set; }
      [XmlAttribute(AttributeName="IncludePatient")]
      public string IncludePatient { get; set; }
      [XmlAttribute(AttributeName="IncludeNetwork")]
      public string IncludeNetwork { get; set; }
      [XmlAttribute(AttributeName="ReceiveMessages")]
      public string ReceiveMessages { get; set; }
      [XmlAttribute(AttributeName="HideToolbar")]
      public string HideToolbar { get; set; }
      [XmlAttribute(AttributeName="PatientRelated")]
      public string PatientRelated { get; set; }
      [XmlAttribute(AttributeName="SuppressErrors")]
      public string SuppressErrors { get; set; }
      [XmlAttribute(AttributeName = "LeaveConfirmation")]
      public string LeaveConfirmation { get; set; }

       [XmlAttribute(AttributeName = "MandatoryUser")]
       public string MandatoryUser { get; set; }
    }

   [XmlRoot(ElementName="BrowserModules")]
   public class BrowserModulesViewModels {
      [XmlElement(ElementName="Module")]
      public List<Module> Module { get; set; }
   }

}
