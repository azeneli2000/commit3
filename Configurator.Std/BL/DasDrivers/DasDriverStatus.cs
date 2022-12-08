using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Configurator.Std.BL.DasDrivers
{

   [XmlRoot(ElementName = "DriverDeviceStatus")]
   public class DriverDeviceStatus
   {
      [XmlAttribute(AttributeName = "DeviceStatus")]
      public int DeviceStatus { get; set; }
      [XmlAttribute(AttributeName = "DriverStatus")]
      public int DriverStatus { get; set; }
      [XmlAttribute(AttributeName = "AlarmIsSilenced")]
      public bool AlarmIsSilenced { get; set; }
      [XmlAttribute(AttributeName = "Notes")]
      public string Notes { get; set; }
      [XmlAttribute(AttributeName = "TriggerTime")]
      public string TriggerTime { get; set; }
   }

   [XmlRoot(ElementName = "DriverStatus")]
   public class DriverStatus
   {
      [XmlAttribute(AttributeName = "DeviceDriverId")]
      public int DeviceDriverId { get; set; }
      [XmlAttribute(AttributeName = "DriverName")]
      public string DriverName { get; set; }
      [XmlAttribute(AttributeName = "DriverVersion")]
      public string DriverVersion { get; set; }
      [XmlAttribute(AttributeName = "ProcessId")]
      public int ProcessId { get; set; }
      [XmlAttribute(AttributeName = "Address")]
      public string Address { get; set; }
      [XmlAttribute(AttributeName = "BedId")]
      public int BedId { get; set; }
      [XmlAttribute(AttributeName = "ProcessStatus")]
      public string ProcessStatus { get; set; }
      [XmlAttribute(AttributeName = "LastDataset")]
      public string LastDataset { get; set; }
      [XmlAttribute(AttributeName = "LastKeepAlive")]
      public string LastKeepAlive { get; set; }

      [XmlIgnore]
      public DateTime? LastDatasetReceived
      {
         get
         {
            return string.IsNullOrWhiteSpace(LastDataset) ? (DateTime?)null : DateTime.ParseExact(LastDataset, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
         }
      }

      [XmlIgnore]
      public DateTime? LastKeepAliveReceived
      {
         get
         {
            return string.IsNullOrWhiteSpace(LastKeepAlive) ? (DateTime?)null : DateTime.ParseExact(LastKeepAlive, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
         }
      }

      [XmlElement(ElementName = "DriverDeviceStatus")]
      public DriverDeviceStatus DriverDeviceStatus { get; set; }
   }

   [XmlRoot(ElementName = "DriverStatusList")]
   public class DriverStatusList
   {
      [XmlElement(ElementName = "DriverStatus")]
      public List<DriverStatus> DriverStatus { get; set; }
   }


   //[XmlRoot(ElementName = "DriverDeviceStatus")]
   //public class DriverDeviceStatus
   //{
   //   [XmlAttribute(AttributeName = "DeviceStatus")]
   //   public int DeviceStatus { get; set; }
   //   [XmlAttribute(AttributeName = "DriverStatus")]
   //   public int DriverStatus { get; set; }
   //   [XmlAttribute(AttributeName = "AlarmIsSilenced")]
   //   public string AlarmIsSilenced { get; set; }
   //}

   //[XmlRoot(ElementName = "DriverStatus")]
   //public class DriverStatus
   //{
   //   [XmlElement(ElementName = "DriverDeviceStatus")]
   //   public List<DriverDeviceStatus> DriverDeviceStatus { get; set; }
   //   [XmlAttribute(AttributeName = "DeviceDriverId")]
   //   public int DeviceDriverId { get; set; }
   //   [XmlAttribute(AttributeName = "DriverName")]
   //   public string DriverName { get; set; }
   //   [XmlAttribute(AttributeName = "DriverVersion")]
   //   public string DriverVersion { get; set; }
   //   [XmlAttribute(AttributeName = "ProcessId")]
   //   public int ProcessId { get; set; }
   //   [XmlAttribute(AttributeName = "Address")]
   //   public string Address { get; set; }
   //   [XmlAttribute(AttributeName = "BedId")]
   //   public int BedId { get; set; }
   //   [XmlAttribute(AttributeName = "ProcessStatus")]
   //   public string ProcessStatus { get; set; }
   //   [XmlAttribute(AttributeName = "LastDataset")]
   //   public string LastDataset { get; set; }
   //   [XmlAttribute(AttributeName = "LastKeepAlive")]
   //   public string LastKeepAlive { get; set; }

   //   [XmlIgnore]
   //   public DateTime? LastDatasetReceived
   //   {
   //      get
   //      {
   //         return string.IsNullOrWhiteSpace(LastDataset) ? (DateTime?)null : DateTime.ParseExact(LastDataset, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
   //      }
   //   }

   //   [XmlIgnore]
   //   public DateTime? LastKeepAliveReceived
   //   {
   //      get
   //      {
   //         return string.IsNullOrWhiteSpace(LastKeepAlive) ? (DateTime?)null : DateTime.ParseExact(LastKeepAlive, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
   //      }
   //   }

   //}

   //[XmlRoot(ElementName = "DriverStatusList")]
   //public class DriverStatusList
   //{
   //   [XmlElement(ElementName = "DriverStatus")]
   //   public List<DriverStatus> DriverStatus { get; set; }
   //}


   //[Serializable]
   ////[XmlRoot(Namespace = @"x-schema:UMSAuxPropertiesValue.xsd", ElementName = "UMSAuxPropertiesValue")]
   //[XmlRoot(ElementName = "DriverStatusList")]
   //public class DriverStatusList : List<DriverStatus>
   //{

   //}

   //[Serializable]
   //public class DriverStatus
   //{
   //   [XmlAttribute(AttributeName = "DeviceDriverId")]
   //   public int DeviceDriverId { get; set; }
   //   [XmlAttribute(AttributeName = "DriverName")]
   //   public string DriverName { get; set; }
   //   [XmlAttribute(AttributeName = "DriverVersion")]
   //   public string DriverVersion { get; set; }
   //   [XmlAttribute(AttributeName = "ProcessId")]
   //   public int ProcessId { get; set; }
   //   [XmlAttribute(AttributeName = "Address")]
   //   public string Address { get; set; }
   //   [XmlAttribute(AttributeName = "BedId")]
   //   public int BedId { get; set; }
   //   [XmlAttribute(AttributeName = "ProcessStatus")]
   //   public string ProcessStatus { get; set; }
   //   [XmlAttribute(AttributeName = "LastDataset")]
   //   public string LastDataset { get; set; }
   //   [XmlAttribute(AttributeName = "LastKeepAlive")]
   //   public string LastKeepAlive { get; set; }

   //   [XmlIgnore]
   //   public DateTime? LastDatasetReceived {
   //      get {
   //         return string.IsNullOrWhiteSpace(LastDataset) ? (DateTime?)null : DateTime.ParseExact(LastDataset, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
   //      }
   //   }

   //   [XmlIgnore]
   //   public DateTime? LastKeepAliveReceived
   //   {
   //      get
   //      {
   //         return string.IsNullOrWhiteSpace(LastKeepAlive) ? (DateTime?)null : DateTime.ParseExact(LastKeepAlive, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
   //      }
   //   }

   //   [XmlArrayItem("DriverDeviceStatus")]
   //   public List<DriverDeviceStatus> DriverDeviceStatus { get; set; }

   //}

   //[Serializable]
   //public class DriverDeviceStatus {
   //   [XmlAttribute(AttributeName = "DeviceStatus")]
   //   public int DeviceStatus { get; set; }
   //   [XmlAttribute(AttributeName = "DriverStatus")]
   //   public int DriverStatus { get; set; }
   //   [XmlAttribute(AttributeName = "AlarmIsSilenced")]
   //   public bool? AlarmIsSilenced { get; set; }
   //}

}
