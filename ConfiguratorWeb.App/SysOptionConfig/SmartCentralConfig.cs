using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   [XmlRoot(ElementName="SmartCentralConfig")]
   public class SmartCentralConfig
   {
      public enum EventsDisplayMode
      {
         Opened = 0,
         Closed = 1,
         LastSeen = 2
      }

      public enum DeviceType
      {
         InfusionPump = 1,
         PatientMonitor = 2,
         PulmonaryVentilator = 3,
         AnesthesiaDeliveryUnit = 4,
         BloodFiltration = 5,
         HeartLungMachine = 6,
         BloodGasAnalyzer = 7,
         LaboratoryInformationSystem = 8,
         Incubator = 9,
         OtherSimple = 98,
         OtherComplex = 99,
      }

      private int mintAlarmImagesCacheDays = 0;
      private int mintInoperativityTimeout = 0;
      private int mintMyPatientsBedCount = 3;
      private int mintCentralRowCount = 3;
      private int mintCentralColumnCount = 3;
      private int mintCentralTotalCount = 9;
      private List<DeviceType> menuDeviceTypeOrder = new List<DeviceType>();
      private int mintLastEventHours = 24;
      private int mintFirstDisplayedPage = 1;
      private Dictionary<string, StringCollection> mobjCentralBedFilter = new Dictionary<string, StringCollection>();
      
      private string mstrS1Font = "DaxCompact-Medium,16,Regular";
      private string mstrS2Font = "DaxCompact-ExtraBold,16,Regular";
      private string mstrS3Font = "DaxCompact-Medium,11,Regular";
      private string mstrEventsGridFont = "Arial,10,Regular";
      private EventsDisplayMode menuEventsDisplayMode = EventsDisplayMode.Closed;

      private string mstrFontModel = "Tahoma,10,Regular";
      private string mstrModelFormatString = string.Empty;
      private string mstrFontAlarm = "DaxCompact-Medium,11,Regular";
      private string mstrFontAlarmValues = "DaxCompact-Medium,11,Regular";
      private string mstrFontMoreDevices = "Tahoma,10,Regular";

      private int mintColumnSizeAlarm = 300;
      private int mintColumnSizeValue = 300;
      private int mintMaximumAlarmLineLimit = 0;
      private int mintDisplayLastWarningAlarmSec = 60;

      public SmartCentralConfig()
      {
         menuDeviceTypeOrder.Add(DeviceType.AnesthesiaDeliveryUnit);
         menuDeviceTypeOrder.Add(DeviceType.BloodFiltration);
         menuDeviceTypeOrder.Add(DeviceType.BloodGasAnalyzer);
         menuDeviceTypeOrder.Add(DeviceType.HeartLungMachine);
         menuDeviceTypeOrder.Add(DeviceType.Incubator);
         menuDeviceTypeOrder.Add(DeviceType.InfusionPump);
         menuDeviceTypeOrder.Add(DeviceType.LaboratoryInformationSystem);
         menuDeviceTypeOrder.Add(DeviceType.OtherComplex);
         menuDeviceTypeOrder.Add(DeviceType.OtherSimple);
         menuDeviceTypeOrder.Add(DeviceType.PatientMonitor);
         menuDeviceTypeOrder.Add(DeviceType.PulmonaryVentilator);
      }

      internal string SerializeDeviceTypeOrder(List<DeviceType> order)
      {
         StringBuilder objBuilder = new StringBuilder();
         foreach(DeviceType type in order)
         {
            objBuilder.Append((int)type); //.ToString());
            objBuilder.Append(';');
         }
         return objBuilder.ToString();
      }

      internal string SerializeCentralBedFilter(Dictionary<string, StringCollection> filter)
      {
         StringBuilder objBuilder = new StringBuilder();
         foreach (string location in filter.Keys)
         {
            objBuilder.Append(location + "=");
            foreach (string bed in filter[location])
            {
               objBuilder.Append(bed);
               objBuilder.Append(',');
            }
            objBuilder.Append(';');
         }
         return objBuilder.ToString();
      }

      internal string Serialize()
      {
         StringBuilder objBuilder = new StringBuilder();
         objBuilder.Append("<SmartCentral><SmartCentralConfig ");
         objBuilder.Append("MyPatientBedCount=\"" + mintMyPatientsBedCount.ToString() + "\" ");
         objBuilder.Append("CentralColumnCount=\"" + mintCentralColumnCount.ToString() + "\" ");
         objBuilder.Append("CentralRowCount=\"" + mintCentralRowCount.ToString() + "\" ");
         objBuilder.Append("DeviceTypeOrder=\"" + SerializeDeviceTypeOrder(menuDeviceTypeOrder) + "\" ");
         objBuilder.Append("CentralBedFilter=\"" + SerializeCentralBedFilter(mobjCentralBedFilter) + "\" ");
         objBuilder.Append("LastEventHours=\"" + mintLastEventHours.ToString() + "\" ");
         objBuilder.Append("FirstDisplayedPage=\"" + mintFirstDisplayedPage.ToString() + "\" ");
         objBuilder.Append("S1Font=\"" + mstrS1Font + "\" ");
         objBuilder.Append("S2Font=\"" + mstrS2Font + "\" ");
         objBuilder.Append("S3Font=\"" + mstrS3Font + "\" ");
         objBuilder.Append("EventsGridFont=\"" + mstrEventsGridFont + "\" ");
         objBuilder.Append("EventsDisplayMode=\"" + ((int)menuEventsDisplayMode).ToString() + "\" ");

         objBuilder.Append("FontModel=\"" + mstrFontModel + "\" ");
         objBuilder.Append("ModelFormatString=\"" + mstrModelFormatString + "\" ");
         objBuilder.Append("FontAlarm=\"" + mstrFontAlarm + "\" ");
         objBuilder.Append("FontAlarmValues=\"" + mstrFontAlarmValues + "\" ");
         objBuilder.Append("FontMoreDevices=\"" + mstrFontMoreDevices + "\" ");
         objBuilder.Append("ColumnSizeAlarm=\"" + mintColumnSizeAlarm + "\" ");
         objBuilder.Append("ColumnSizeValue=\"" + mintColumnSizeValue + "\" ");
         objBuilder.Append("MaximumAlarmLineLimit=\"" + mintMaximumAlarmLineLimit + "\" ");
         objBuilder.Append("DisplayLastWarningAlarmSec=\"" + mintDisplayLastWarningAlarmSec + "\" ");
         objBuilder.Append("InoperativityTimeout=\"" + mintInoperativityTimeout + "\" ");
         objBuilder.Append("AlarmImagesCacheDays=\"" + mintAlarmImagesCacheDays + "\" ");

         objBuilder.Append("/></SmartCentral>");
         return objBuilder.ToString();
      }

      public static SmartCentralConfig Deserialize(string xml)
      {
         SmartCentralConfig objConfig = new SmartCentralConfig();

         StringReader objStringReader = new StringReader(xml);
         XmlTextReader objReader = new XmlTextReader(objStringReader);
         while (objReader.Read())
         {
            switch (objReader.NodeType)
            {
               case XmlNodeType.Element:
                  if (objReader.Name.CompareTo("SmartCentralConfig") == 0)
                  {
                     objConfig.MyPatientsBedCount = Int32.Parse(objReader.GetAttribute("MyPatientBedCount"));
                     objConfig.CentralColumnCount = Int32.Parse(objReader.GetAttribute("CentralColumnCount"));
                     objConfig.CentralRowCount = Int32.Parse(objReader.GetAttribute("CentralRowCount"));
                     objConfig.CentralTotalCount = objConfig.CentralRowCount * objConfig.CentralColumnCount;
                     
                     if (objReader.GetAttribute("LastEventHours") != null)
                     {
                        objConfig.LastEventHours = Int32.Parse(objReader.GetAttribute("LastEventHours"));
                     }
                     if (objReader.GetAttribute("FirstDisplayedPage") != null)
                     {
                        if (objReader.GetAttribute("FirstDisplayedPage").Length > 0)
                        {
                           objConfig.FirstDisplayedPage = Int32.Parse(objReader.GetAttribute("FirstDisplayedPage"));
                           if (objConfig.FirstDisplayedPage < 1)
                           {
                              objConfig.FirstDisplayedPage = 1;
                           }
                        }
                     }
                     if (objReader.GetAttribute("S1Font") != null)
                     {
                        if (objReader.GetAttribute("S1Font").Length > 0)
                        {
                           objConfig.S1Font = objReader.GetAttribute("S1Font");
                        }
                     }
                     if (objReader.GetAttribute("S2Font") != null)
                     {
                        if (objReader.GetAttribute("S2Font").Length > 0)
                        {
                           objConfig.S2Font = objReader.GetAttribute("S2Font");
                        }
                     }
                     if (objReader.GetAttribute("S3Font") != null)
                     {
                        if (objReader.GetAttribute("S3Font").Length > 0)
                        {
                           objConfig.S3Font = objReader.GetAttribute("S3Font");
                        }
                     }
                     if (objReader.GetAttribute("EventsGridFont") != null)
                     {
                        if (objReader.GetAttribute("EventsGridFont").Length > 0)
                        {
                           objConfig.EventsGridFont = objReader.GetAttribute("EventsGridFont");
                        }
                     }
                     if (objReader.GetAttribute("EventsDisplayMode") != null)
                     {
                        if (objReader.GetAttribute("EventsDisplayMode").Length > 0)
                        {
                           int intValue = 0;
                           Int32.TryParse(objReader.GetAttribute("EventsDisplayMode"), out intValue);
                           objConfig.EventsDisplayModeSelected = (EventsDisplayMode)intValue;
                        }
                     }
                     if (!string.IsNullOrEmpty(objReader.GetAttribute("CentralBedFilter")))
                     {
                        if (objReader.GetAttribute("CentralBedFilter").Length > 0)
                        {
                           string[] astrValues = objReader.GetAttribute("CentralBedFilter").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                           foreach (string value in astrValues)
                           {
                              string[] astrLocationValues = value.Split('=');
                              if (astrLocationValues.Length == 2)
                              {
                                 if (!objConfig.mobjCentralBedFilter.ContainsKey(astrLocationValues[0]))
                                 {
                                    StringCollection objValues = new StringCollection();
                                    string[] astrBeds = astrLocationValues[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string bedId in astrBeds)
                                    {
                                       objValues.Add(bedId);
                                    }
                                    objConfig.mobjCentralBedFilter.Add(astrLocationValues[0], objValues);
                                 }
                              }
                           }
                        }
                     }
                     if (objReader.GetAttribute("DeviceTypeOrder") != null)
                     {
                        string strOrder = objReader.GetAttribute("DeviceTypeOrder");
                        if (!string.IsNullOrEmpty(strOrder))
                        {
                           string[] astrValues = strOrder.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                           objConfig.DeviceTypeOrder.Clear();

                           foreach (string value in astrValues)
                           {
                              objConfig.DeviceTypeOrder.Add((DeviceType)Int32.Parse(value));
                           }
                        }
                     }
                     if (objReader.GetAttribute("FontModel") != null)
                     {
                        if (objReader.GetAttribute("FontModel").Length > 0)
                        {
                           objConfig.FontModel = objReader.GetAttribute("FontModel");
                        }
                     }
                     if (objReader.GetAttribute("ModelFormatString") != null)
                     {
                        if (objReader.GetAttribute("ModelFormatString").Length > 0)
                        {
                           objConfig.ModelFormatString = objReader.GetAttribute("ModelFormatString");
                        }
                     }
                     if (objReader.GetAttribute("FontAlarm") != null)
                     {
                        if (objReader.GetAttribute("FontAlarm").Length > 0)
                        {
                           objConfig.FontAlarm = objReader.GetAttribute("FontAlarm");
                        }
                     }
                     if (objReader.GetAttribute("FontAlarmValues") != null)
                     {
                        if (objReader.GetAttribute("FontAlarmValues").Length > 0)
                        {
                           objConfig.FontAlarmValues = objReader.GetAttribute("FontAlarmValues");
                        }
                     }
                     if (objReader.GetAttribute("FontMoreDevices") != null)
                     {
                        if (objReader.GetAttribute("FontMoreDevices").Length > 0)
                        {
                           objConfig.FontMoreDevices = objReader.GetAttribute("FontMoreDevices");
                        }
                     }
                     if (objReader.GetAttribute("ColumnSizeAlarm") != null)
                     {
                        if (objReader.GetAttribute("ColumnSizeAlarm").Length > 0)
                        {
                           objConfig.ColumnSizeAlarm = Convert.ToInt32(objReader.GetAttribute("ColumnSizeAlarm"));
                        }
                     }
                     if (objReader.GetAttribute("ColumnSizeValue") != null)
                     {
                        if (objReader.GetAttribute("ColumnSizeValue").Length > 0)
                        {
                           objConfig.ColumnSizeValue = Convert.ToInt32(objReader.GetAttribute("ColumnSizeValue"));
                        }
                     }
                     if (objReader.GetAttribute("MaximumAlarmLineLimit") != null)
                     {
                        if (objReader.GetAttribute("MaximumAlarmLineLimit").Length > 0)
                        {
                           objConfig.MaximumAlarmLineLimit = Convert.ToInt32(objReader.GetAttribute("MaximumAlarmLineLimit"));
                        }
                     }
                     if (objReader.GetAttribute("DisplayLastWarningAlarmSec") != null)
                     {
                        if (objReader.GetAttribute("DisplayLastWarningAlarmSec").Length > 0)
                        {
                           objConfig.DisplayLastWarningAlarmSec = Convert.ToInt32(objReader.GetAttribute("DisplayLastWarningAlarmSec"));
                        }
                     }
                     if (objReader.GetAttribute("InoperativityTimeout") != null)
                     {
                        if (objReader.GetAttribute("InoperativityTimeout").Length > 0)
                        {
                           objConfig.InoperativityTimeout = Convert.ToInt32(objReader.GetAttribute("InoperativityTimeout"));
                        }
                     }
                     if (objReader.GetAttribute("AlarmImagesCacheDays") != null)
                     {
                        if (objReader.GetAttribute("AlarmImagesCacheDays").Length > 0)
                        {
                           objConfig.AlarmImagesCacheDays = Convert.ToInt32(objReader.GetAttribute("AlarmImagesCacheDays"));
                        }
                     }
                  }
                  break;
               default:
                  break;
            }
         }
         objReader.Close();
         objStringReader.Close();
         objStringReader.Dispose();

         return objConfig;
      }

      #region Properties

      [XmlAttribute(AttributeName="FontMoreDevices")]
      public string FontMoreDevices
      {
         get { return mstrFontMoreDevices;  }
         set { mstrFontMoreDevices = value; }
      }
      
      [XmlAttribute(AttributeName="FontModel")]
      public string FontModel
      {
         get { return mstrFontModel; }
         set { mstrFontModel = value; }
      }

      [XmlAttribute(AttributeName="ModelFormatString")]
      public string ModelFormatString
      {
         get { return mstrModelFormatString; }
         set { mstrModelFormatString = value; }
      }

      [XmlAttribute(AttributeName="FontAlarm")]
      public string FontAlarm
      {
         get { return mstrFontAlarm; }
         set { mstrFontAlarm = value; }
      }

      [XmlAttribute(AttributeName="FontAlarmValues")]
      public string FontAlarmValues
      {
         get { return mstrFontAlarmValues; }
         set { mstrFontAlarmValues = value; }
      }

      [XmlAttribute(AttributeName="ColumnSizeAlarm")]
      public int ColumnSizeAlarm
      {
         get { return mintColumnSizeAlarm; }
         set { mintColumnSizeAlarm = value; }
      }

      [XmlAttribute(AttributeName="ColumnSizeValue")]
      public int ColumnSizeValue
      {
         get { return mintColumnSizeValue; }
         set { mintColumnSizeValue = value; }
      }

      [XmlAttribute(AttributeName="MaximumAlarmLineLimit")]
      public int MaximumAlarmLineLimit
      {
         get { return mintMaximumAlarmLineLimit; }
         set { mintMaximumAlarmLineLimit = value; }
      }

      [XmlAttribute(AttributeName="DisplayLastWarningAlarmSec")]
      public int DisplayLastWarningAlarmSec
      {
         get { return mintDisplayLastWarningAlarmSec; }
         set { mintDisplayLastWarningAlarmSec = value; }
      }


      [XmlAttribute(AttributeName="EventsDisplayMode")]
      public EventsDisplayMode EventsDisplayModeSelected
      {
         get { return menuEventsDisplayMode; }
         set
         {
            menuEventsDisplayMode = value;
         }
      }

      [XmlAttribute(AttributeName="EventsGridFont")]
      public string EventsGridFont
      {
         get { return mstrEventsGridFont; }
         set { mstrEventsGridFont = value; }
      }

      [XmlAttribute(AttributeName="S1Font")]
      public string S1Font
      {
         get { return mstrS1Font; }
         set { mstrS1Font = value; }
      }

      [XmlAttribute(AttributeName="S2Font")]
      public string S2Font
      {
         get { return mstrS2Font; }
         set { mstrS2Font = value; }
      }

      [XmlAttribute(AttributeName="S3Font")]
      public string S3Font
      {
         get { return mstrS3Font; }
         set { mstrS3Font = value; }
      }

      [XmlAttribute(AttributeName="CentralBedFilter")]
      public Dictionary<string, StringCollection> CentralBedFilter
      {
         get { return mobjCentralBedFilter; }
         set
         {
            mobjCentralBedFilter = value;
         }
      }

      [XmlAttribute(AttributeName="FirstDisplayedPage")]
      public int FirstDisplayedPage
      {
         get { return mintFirstDisplayedPage; }
         set { mintFirstDisplayedPage = value; }
      }

      [XmlAttribute(AttributeName="LastEventHours")]
      public int LastEventHours
      {
         get { return mintLastEventHours; }
         set { mintLastEventHours = value; }
      }
      
      [XmlAttribute(AttributeName="DeviceTypeOrder")]
      public List<DeviceType> DeviceTypeOrder
      {
         get { return menuDeviceTypeOrder; }
      }

      public int CentralTotalCount
      {
         get { return mintCentralTotalCount; }
         set { mintCentralTotalCount = value; }
      }

      [XmlAttribute(AttributeName="MyPatientBedCount")]
      public int MyPatientsBedCount
      {
         get { return mintMyPatientsBedCount; }
         set { mintMyPatientsBedCount = value; }
      }

      [XmlAttribute(AttributeName="CentralRowCount")]
      public int CentralRowCount
      {
         get { return mintCentralRowCount; }
         set { mintCentralRowCount = value; }
      }

      [XmlAttribute(AttributeName="CentralColumnCount")]
      public int CentralColumnCount
      {
         get { return mintCentralColumnCount; }
         set { mintCentralColumnCount = value; }
      }

      [XmlAttribute(AttributeName="InoperativityTimeout")]
      public int InoperativityTimeout
      {
         get { return mintInoperativityTimeout; }
         set { mintInoperativityTimeout = value; }
      }

      [XmlAttribute(AttributeName="AlarmImagesCacheDays")]
      public int AlarmImagesCacheDays
      {
         get { return mintAlarmImagesCacheDays; }
         set { mintAlarmImagesCacheDays = value; }
      }
      #endregion
   }

   [XmlRoot(ElementName="SmartCentral")]
   public class SmartCentral {
      [XmlElement(ElementName="SmartCentralConfig")]
      public SmartCentralConfig SmartCentralConfig { get; set; }
   }
}
