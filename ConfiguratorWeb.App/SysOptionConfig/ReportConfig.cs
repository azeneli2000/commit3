using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using ConfiguratorWeb.App.Extensions.Helpers;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   /// <summary>
   /// ReportMaster configuration
   /// </summary>
   public class ReportMasterConfig
   {
      private List<ReportMasterMenuItem> mobjMenuItems = new List<ReportMasterMenuItem>();
      private ReportPrinterSettings mobjReportPrinterSettings = null;
      private StorageSettings mobjStorageSettings = null;
      private MailSettings mobjMailSettings = null;
      private string mstrExportSettings = string.Empty;
      private UISettings mobjUISettings = null;

      /// <summary>
      /// Initializes a new instance of the <see cref="ReportMasterConfig"/> class.
      /// </summary>
      public ReportMasterConfig()
      {
      }

      #region Properties

      /// <summary>
      /// List of Menu Items.
      /// </summary>
      /// <value>List of Menu Items.</value>
      public List<ReportMasterMenuItem> MenuItems
      {
         get { return mobjMenuItems; }
      }

      /// <summary>
      /// Export Settings
      /// </summary>
      public string ExportSettings
      {
         get { return mstrExportSettings; }
         set { mstrExportSettings = value; }
      }

      /// <summary>
      /// Printer Settings
      /// </summary>
      public ReportPrinterSettings PrinterSettings
      {
         get { return mobjReportPrinterSettings; }
         set { mobjReportPrinterSettings = value; }
      }

      /// <summary>
      /// Storage Settings
      /// </summary>
      public StorageSettings StorageSettings
      {
         get { return mobjStorageSettings; }
         set { mobjStorageSettings = value; }
      }

      /// <summary>
      /// Mail Settings
      /// </summary>
      public MailSettings MailSettings
      {
         get { return mobjMailSettings; }
         set { mobjMailSettings = value; }
      }

      /// <summary>
      /// UI Settings
      /// </summary>
      public UISettings UISettings
      {
         get { return mobjUISettings; }
         set { mobjUISettings = value; }
      }
      #endregion

      /// <summary>
      /// GetMenuItemByCallback
      /// </summary>
      /// <param name="callback"></param>
      /// <returns></returns>
      public ReportMasterMenuItem GetMenuItemByCallback(string callback)
      {
         
         ReportMasterMenuItem objItem = null;
         foreach (ReportMasterMenuItem item in mobjMenuItems)
         {
            if (string.Compare(item.Callback, callback, StringComparison.OrdinalIgnoreCase) == 0)
            {
               objItem = item;
               break;
            }
         }
         return objItem;
      }

      /// <summary>
      /// Deserialize
      /// </summary>
      /// <param name="config"></param>
      /// <returns></returns>
      public static ReportMasterConfig Deserialize(string config)
      {
         ReportMasterConfig objConfig = new ReportMasterConfig();

         StringReader objStringReader = new StringReader(config);
         XmlTextReader objReader = new XmlTextReader(objStringReader);

         ReportMasterMenuItem objItem = null;

         while (objReader.Read())
         {
            switch (objReader.NodeType)
            {
               case XmlNodeType.Element:
                  
                  if (objReader.Name.CompareTo("Menu") == 0)
                  {
                     objItem = new ReportMasterMenuItem();
                     if (objReader.GetAttribute("Name") != null)
                     {
                        objItem.Name = objReader.GetAttribute("Name");
                     }
                     if (objReader.GetAttribute("Callback") != null)
                     {
                        objItem.Callback = objReader.GetAttribute("Callback");
                     }
                     if (objReader.GetAttribute("Path") != null)
                     {
                        objItem.Path = objReader.GetAttribute("Path");
                     }
                     if (objReader.GetAttribute("Template") != null)
                     {
                        objItem.Template = objReader.GetAttribute("Template");
                     }
                     if (objReader.GetAttribute("Preview") != null)
                     {
                        objItem.Preview = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.ConvertToBoolean(objReader.GetAttribute("Preview"));
                     }
                     if (objReader.GetAttribute("Print") != null)
                     {
                        objItem.Print = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.ConvertToBoolean(objReader.GetAttribute("Print"));
                     }
                     if (objReader.GetAttribute("PrintButtonEnabled") != null)
                     {
                        objItem.PrintButtonEnabled = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.ConvertToBoolean(objReader.GetAttribute("PrintButtonEnabled"));
                     }
                     if (objReader.GetAttribute("OutlineEnabled") != null)
                     {
                        objItem.OutlineEnabled = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.ConvertToBoolean(objReader.GetAttribute("OutlineEnabled"));
                     }
                     if (objReader.GetAttribute("WatermarkEnabled") != null)
                     {
                        objItem.WatermarkEnabled = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.ConvertToBoolean(objReader.GetAttribute("WatermarkEnabled"));
                     }
                     if (objReader.GetAttribute("UseWindowsPrintDialog") != null)
                     {
                        objItem.UseWindowsPrintDialog = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.ConvertToBoolean(objReader.GetAttribute("UseWindowsPrintDialog"));
                     }
                     if (objReader.GetAttribute("ExportFormats") != null)
                     {
                        string[] astrExportFormats = objReader.GetAttribute("ExportFormats").Split(';');
                        foreach (string value in astrExportFormats)
                        {
                           objItem.ExportFormats.Add(value);
                        }
                     }
                     objConfig.MenuItems.Add(objItem);
                  }
                  else
                  {
                     if (objReader.Name.CompareTo("Parameter") == 0)
                     {
                        objItem.Parameters.Add(objReader.GetAttribute("Name"), objReader.GetAttribute("Value"));
                     }
                     else
                     {
                        if (objReader.Name.CompareTo("PrinterSettings") == 0)
                        {
                           objConfig.PrinterSettings = new ReportPrinterSettings();
                           if (objReader.GetAttribute("PrinterName") != null)
                           {
                              objConfig.PrinterSettings.PrinterName = objReader.GetAttribute("PrinterName");
                           }
                           if (objReader.GetAttribute("Copies") != null)
                           {
                              objConfig.PrinterSettings.Copies = Convert.ToInt32(objReader.GetAttribute("Copies"));
                           }
                           if (objReader.GetAttribute("Duplex") != null)
                           {
                              objConfig.PrinterSettings.Duplex = objReader.GetAttribute("Duplex");
                           }
                           if (objReader.GetAttribute("Collate") != null)
                           {
                              objConfig.PrinterSettings.Collate = Convert.ToBoolean(objReader.GetAttribute("Collate"));
                           }
                           if (objReader.GetAttribute("ShowDialog") != null)
                           {
                              objConfig.PrinterSettings.ShowDialog = Convert.ToBoolean(objReader.GetAttribute("ShowDialog"));
                           }
                        }
                        else
                        {
                           if (objReader.Name.CompareTo("StorageSettings") == 0)
                           {
                              objConfig.StorageSettings = new StorageSettings();
                              if (objReader.GetAttribute("Path") != null)
                              {
                                 objConfig.StorageSettings.StoragePath = objReader.GetAttribute("Path");
                              }
                              if (objReader.GetAttribute("ShareName") != null)
                              {
                                 objConfig.StorageSettings.ShareName = objReader.GetAttribute("ShareName");
                              }
                              if (objReader.GetAttribute("Server") != null)
                              {
                                 objConfig.StorageSettings.Server = objReader.GetAttribute("Server");
                              }
                              if (objReader.GetAttribute("User") != null)
                              {
                                 objConfig.StorageSettings.CredentialUser = objReader.GetAttribute("User");
                              }
                              if (objReader.GetAttribute("Password") != null)
                              {
                                 objConfig.StorageSettings.CredentialPassword = objReader.GetAttribute("Password");
                              }
                           }
                           else
                           {
                              if (objReader.Name.CompareTo("MailSettings") == 0)
                              {
                                 objConfig.MailSettings = new MailSettings();
                                 if (objReader.GetAttribute("Body") != null)
                                 {
                                    objConfig.MailSettings.Body = objReader.GetAttribute("Body");
                                 }
                                 if (objReader.GetAttribute("Destinations") != null)
                                 {
                                    objConfig.MailSettings.Destinations =
                                       Digistat.FrameworkStd.UMSLegacy.UMSFrameworkStringCollectionSerializer.Parse(objReader.GetAttribute("Destinations"));
                                 }
                                 if (objReader.GetAttribute("FromAddress") != null)
                                 {
                                    objConfig.MailSettings.FromAddress = objReader.GetAttribute("FromAddress");
                                 }
                                 if (objReader.GetAttribute("FromName") != null)
                                 {
                                    objConfig.MailSettings.FromName = objReader.GetAttribute("FromName");
                                 }
                                 if (objReader.GetAttribute("Subject") != null)
                                 {
                                    objConfig.MailSettings.Subject = objReader.GetAttribute("Subject");
                                 }
                                 if (objReader.GetAttribute("Format") != null)
                                 {
                                    objConfig.MailSettings.Format = objReader.GetAttribute("Format");
                                 }
                              }
                              else
                              {
                                 if (objReader.Name.CompareTo("ExportSettings") == 0)
                                 {
                                    objReader.Read();
                                    objConfig.ExportSettings = objReader.Value;
                                 }
                                 else
                                 {
                                    if (objReader.Name.CompareTo("UISettings") == 0)
                                    {
                                       objConfig.UISettings = new UISettings();
                                       if (objReader.GetAttribute("ShowProgress") != null)
                                       {
                                          objConfig.UISettings.ShowProgress = Convert.ToBoolean(objReader.GetAttribute("ShowProgress"));
                                       }
                                       if (objReader.GetAttribute("UsePrintDialog") != null)
                                       {
                                          objConfig.UISettings.UsePrintDialog = Convert.ToBoolean(objReader.GetAttribute("UsePrintDialog"));
                                       }
                                       if (objReader.GetAttribute("PrintButton") != null)
                                       {
                                          objConfig.UISettings.PrintButton = Convert.ToBoolean(objReader.GetAttribute("PrintButton"));
                                       }
                                       if (objReader.GetAttribute("Outline") != null)
                                       {
                                          objConfig.UISettings.Outline = Convert.ToBoolean(objReader.GetAttribute("Outline"));
                                       }
                                       if (objReader.GetAttribute("WatermarkEnabled") != null)
                                       {
                                          objConfig.UISettings.WatermarkEnabled = Convert.ToBoolean(objReader.GetAttribute("WatermarkEnabled"));
                                       }
                                       if (objReader.GetAttribute("Watermark") != null)
                                       {
                                          objConfig.UISettings.Watermark = objReader.GetAttribute("Watermark");
                                       }
                                       if (objReader.GetAttribute("ExportParameters") != null)
                                       {
                                          objConfig.UISettings.ExportParameters =
                                             Digistat.FrameworkStd.UMSLegacy.UMSFrameworkStringCollectionSerializer.Parse(objReader.GetAttribute("ExportParameters"));
                                       }
                                    }
                                 }
                              }
                           }
                        }
                        }
                     }
                  break;
               default:
                  break;
            }
         }
         objReader.Close();
         objStringReader.Dispose();
         return objConfig;
      }

      public string Serialize()
      {
         StringBuilder objBuilder = new StringBuilder();

         objBuilder.AppendLine("<ReportMaster>");

         foreach (ReportMasterMenuItem item in this.MenuItems)
         {
            objBuilder.AppendLine(
               "<Menu Name=\"" + item.Name + "\" Callback=\"" + item.Callback + "\" Path=\"" + item.Path
               + "\" Template=\"" + item.Template + "\" Preview=\"" + item.Preview + "\" Print=\"" + item.Print + "\" UseWindowsPrintDialog=\"" + item.UseWindowsPrintDialog
               + "\" WatermarkEnabled=\"" + item.WatermarkEnabled + "\" OutlineEnabled=\"" + item.OutlineEnabled + "\" PrintButtonEnabled=\"" + item.PrintButtonEnabled
               + "\" ExportFormats=\"" + Digistat.FrameworkStd.UMSLegacy.UMSFrameworkStringCollectionSerializer.ToString(item.ExportFormats) + "\">");
            objBuilder.AppendLine("<Parameters>");
            foreach (string strValue in item.Parameters.Keys)
            {
               objBuilder.AppendLine("<Parameter Name=\"" + strValue + "\" Value=\"" + item.Parameters[strValue] + "\"/>");
            }
            objBuilder.AppendLine("</Parameters>");
            objBuilder.AppendLine("</Menu>");
         }
         if (mobjReportPrinterSettings != null)
         {
            objBuilder.AppendLine("<PrinterSettings PrinterName=\"" + mobjReportPrinterSettings.PrinterName + "\" Copies=\"" +
               mobjReportPrinterSettings.Copies.ToString() + "\" Duplex=\"" + mobjReportPrinterSettings.Duplex + "\" Collate=\"" + 
               mobjReportPrinterSettings.Collate.ToString() +"\" ShowDialog=\"" + mobjReportPrinterSettings.ShowDialog.ToString() + "\"/>");
         }
         if (mobjStorageSettings != null)
         {
            objBuilder.AppendLine("<StorageSettings Path=\"" + mobjStorageSettings.StoragePath + "\" Server=\"" + mobjStorageSettings.Server 
            + "\" ShareName=\"" + mobjStorageSettings.ShareName + "\" User=\"" + mobjStorageSettings.CredentialUser
            + "\" Password=\"" + mobjStorageSettings.CredentialPassword + "\"/>");
         }
         if (mobjMailSettings != null)
         {
            objBuilder.AppendLine("<MailSettings FromAddress=\"" + mobjMailSettings.FromAddress 
            + "\" FromName=\"" + mobjMailSettings.FromName + "\" Body=\"" + mobjMailSettings.Body
            + "\" Subject=\"" + mobjMailSettings.Subject + "\" Format=\"" + mobjMailSettings.Format
            + "\" Destinations=\"" +
            Digistat.FrameworkStd.UMSLegacy.UMSFrameworkStringCollectionSerializer.ToString(mobjMailSettings.Destinations) + "\"/>");
         }
         if (mobjUISettings != null)
         {
            objBuilder.AppendLine("<UISettings ShowProgress=\"" + mobjUISettings.ShowProgress.ToString() +"\" UsePrintDialog=\"" +
               mobjUISettings.UsePrintDialog.ToString() + "\" PrintButton=\"" + mobjUISettings.PrintButton.ToString() + "\" Outline=\"" +
               mobjUISettings.Outline.ToString() + "\" WatermarkEnabled=\"" + mobjUISettings.WatermarkEnabled.ToString() + "\" Watermark=\"" + mobjUISettings.Watermark + "\" ExportParameters=\"" +
               Digistat.FrameworkStd.UMSLegacy.UMSFrameworkStringCollectionSerializer.ToString(mobjUISettings.ExportParameters) + "\"/>");
         }
         objBuilder.Append("<ExportSettings>");
         objBuilder.Append(mstrExportSettings.Trim().Replace(System.Environment.NewLine,""));
         objBuilder.AppendLine("</ExportSettings>");
         objBuilder.AppendLine("</ReportMaster>");
         return objBuilder.ToString();
      }


   }

   /// <summary>
   /// Report Master Menu Item
   /// </summary>
   public class ReportMasterMenuItem
   {
      private string mstrName = string.Empty;
      private string mstrCallback = string.Empty;
      private string mstrPath = string.Empty;
      private string mstrTemplate = string.Empty;
      private bool mbolPreview = false;
      private bool mbolPrint = false;
      private bool mbolOutlineEnabled = false;
      private bool mbolWatermarkEnabled = false;
      private bool mbolUseWindowsPrintDialog = false;
      private bool mbolPrintButtonEnabled = false;

      private StringCollection mobjExportFormats = new System.Collections.Specialized.StringCollection();
      private Dictionary<string, string> mobjParameters = new Dictionary<string, string>();

      public ReportMasterMenuItem()
      {
      }

      public override string ToString()
      {
         return mstrName;
      }
      #region Properties
      /// <summary>
      /// Name
      /// </summary>
      public string Name
      {
         get { return mstrName; }
         set { mstrName = value; }
      }

      /// <summary>
      /// Callback
      /// </summary>
      public string Callback
      {
         get { return mstrCallback; }
         set { mstrCallback = value; }
      }

      /// <summary>
      /// Path
      /// </summary>
      public string Path
      {
         get { return mstrPath; }
         set { mstrPath = value; }
      }

      /// <summary>
      /// Template
      /// </summary>
      public string Template
      {
         get { return mstrTemplate; }
         set { mstrTemplate = value; }
      }

      /// <summary>
      /// Show Preview
      /// </summary>
      public bool Preview
      {
         get { return mbolPreview; }
         set { mbolPreview = value; }
      }

      /// <summary>
      /// Print Report
      /// </summary>
      public bool Print
      {
         get { return mbolPrint; }
         set { mbolPrint = value; }
      }

      /// <summary>
      /// Print Button Enabled
      /// </summary>
      public bool PrintButtonEnabled
      {
         get { return mbolPrintButtonEnabled; }
         set { mbolPrintButtonEnabled = value; }
      }

      /// <summary>
      /// UseWindowsPrintDialog
      /// </summary>
      public bool UseWindowsPrintDialog
      {
         get { return mbolUseWindowsPrintDialog; }
         set { mbolUseWindowsPrintDialog = value; }
      }

      /// <summary>
      /// Watermark Enabled
      /// </summary>
      public bool WatermarkEnabled
      {
         get { return mbolWatermarkEnabled; }
         set { mbolWatermarkEnabled = value; }
      }

      /// <summary>
      /// Outline Enabled
      /// </summary>
      public bool OutlineEnabled
      {
         get { return mbolOutlineEnabled; }
         set { mbolOutlineEnabled = value; }
      }

      /// <summary>
      /// Export formats
      /// </summary>
      public StringCollection ExportFormats
      {
         get { return mobjExportFormats; }
      }

      /// <summary>
      /// Parameters
      /// </summary>
      public Dictionary<string, string> Parameters
      {
         get { return mobjParameters; }
         set { mobjParameters = value; }
      }
      #endregion
   }

   /// <summary>
   /// ReportPrinterSettings
   /// </summary>
   public class ReportPrinterSettings
   {
      private string mstrPrinterName = string.Empty;
      private int mintCopies = 0;
      private string mstrDuplex = string.Empty;
      private bool mbolCollate = false;
      private bool mbolShowDialog = false;

      /// <summary>
      /// Constructor
      /// </summary>
      public ReportPrinterSettings()
      {
      }

      #region Properties

      /// <summary>
      /// Printer name
      /// </summary>
      public string PrinterName
      {
         get { return mstrPrinterName; }
         set { mstrPrinterName = value; }
      }

      /// <summary>
      /// Copies
      /// </summary>
      public int Copies
      {
         get { return mintCopies; }
         set { mintCopies = value; }
      }

      /// <summary>
      /// Duplex
      /// </summary>
      public string Duplex
      {
         get { return mstrDuplex; }
         set { mstrDuplex = value; }
      }

      /// <summary>
      /// Collate
      /// </summary>
      public bool Collate
      {
         get { return mbolCollate; }
         set { mbolCollate = value; }
      }

      /// <summary>
      /// ShowDialog
      /// </summary>
      public bool ShowDialog
      {
         get { return mbolShowDialog; }
         set { mbolShowDialog = value; }
      }
      #endregion
   }

   /// <summary>
   /// Storage Settings
   /// </summary>
   public class StorageSettings
   {
      private string mstrStoragePath = string.Empty;
      private string mstrCredentialUser = string.Empty;
      private string mstrCredentialPassword = string.Empty;
      private string mstrShareName = string.Empty;
      private string mstrServer = string.Empty;

      /// <summary>
      /// Constructor
      /// </summary>
      public StorageSettings()
      {
      }

      #region Properties

      /// <summary>
      /// Storage Path
      /// </summary>
      public string StoragePath
      {
         get { return mstrStoragePath; }
         set { mstrStoragePath = value; }
      }

      /// <summary>
      /// Credential User
      /// </summary>
      public string CredentialUser
      {
         get { return mstrCredentialUser; }
         set { mstrCredentialUser = value; }
      }

      /// <summary>
      /// Credential Password
      /// </summary>
      public string CredentialPassword
      {
         get { return mstrCredentialPassword; }
         set { mstrCredentialPassword = value; }
      }

      /// <summary>
      /// Share Name
      /// </summary>
      public string ShareName
      {
         get { return mstrShareName; }
         set { mstrShareName = value; }
      }

      /// <summary>
      /// Server
      /// </summary>
      public string Server
      {
         get { return mstrServer; }
         set { mstrServer = value; }
      }
      #endregion
   }

   /// <summary>
   /// Mail Settings
   /// </summary>
   public class MailSettings
   {
      private bool mbolSendMail = false;
      private string mstrSubject = string.Empty;
      private string mstrBody = string.Empty;
      private StringCollection mobjDestinations = new StringCollection();
      private string mstrFromAddress = string.Empty;
      private string mstrFromName = string.Empty;
      private string mstrFormat = string.Empty;

      /// <summary>
      /// Constructor
      /// </summary>
      public MailSettings()
      {
      }

      #region Properties
      /// <summary>
      /// Send Mail
      /// </summary>
      public bool SendMail
      {
         get { return mbolSendMail; }
         set { mbolSendMail = value; }
      }

      /// <summary>
      /// Format
      /// </summary>
      public string Format
      {
         get { return mstrFormat; }
         set { mstrFormat = value; }
      }

      /// <summary>
      /// Subject
      /// </summary>
      public string Subject
      {
         get { return mstrSubject; }
         set { mstrSubject = value; }
      }

      /// <summary>
      /// Body
      /// </summary>
      public string Body
      {
         get { return mstrBody; }
         set { mstrBody = value; }
      }

      /// <summary>
      /// FromAddress
      /// </summary>
      public string FromAddress
      {
         get { return mstrFromAddress; }
         set { mstrFromAddress = value; }
      }

      /// <summary>
      /// FromName
      /// </summary>
      public string FromName
      {
         get { return mstrFromName; }
         set { mstrFromName = value; }
      }

      /// <summary>
      /// Destinations
      /// </summary>
      public StringCollection Destinations
      {
         get { return mobjDestinations; }
         set { mobjDestinations = value; }
      }
      #endregion
   }

   /// <summary>
   /// UISettings
   /// </summary>
   public class UISettings
   {
      private bool mbolShowProgress = true;
      private bool mbolOutline = false;
      private string mstrWatermark = string.Empty;
      private bool mbolPrintButton = true;
      private bool mbolUsePrintDialog = false;
      private StringCollection mobjExportParameters = new StringCollection();
      private bool mbolWatermarkEnabled = true;


      /// <summary>
      /// Initializes a new instance of the <see cref="UISettings"/> class.
      /// </summary>
      public UISettings()
      {
         mobjExportParameters.Add("PDF");
         mobjExportParameters.Add("XLS");
      }

      #region Properties

      /// <summary>
      /// Gets or sets a value indicating whether [watermark enabled].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [watermark enabled]; otherwise, <c>false</c>.
      /// </value>
      public bool WatermarkEnabled
      {
         get { return mbolWatermarkEnabled; }
         set { mbolWatermarkEnabled = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [show progress].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [show progress]; otherwise, <c>false</c>.
      /// </value>
      public bool ShowProgress
      {
         get { return mbolShowProgress; }
         set { mbolShowProgress = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="UISettings"/> is outline.
      /// </summary>
      /// <value>
      ///   <c>true</c> if outline; otherwise, <c>false</c>.
      /// </value>
      public bool Outline
      {
         get { return mbolOutline; }
         set { mbolOutline = value; }
      }

      /// <summary>
      /// Gets or sets the watermark.
      /// </summary>
      /// <value>
      /// The watermark.
      /// </value>
      public string Watermark
      {
         get { return mstrWatermark; }
         set { mstrWatermark = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [print button].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [print button]; otherwise, <c>false</c>.
      /// </value>
      public bool PrintButton
      {
         get { return mbolPrintButton; }
         set { mbolPrintButton = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [use print dialog].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [use print dialog]; otherwise, <c>false</c>.
      /// </value>
      public bool UsePrintDialog
      {
         get { return mbolUsePrintDialog; }
         set { mbolUsePrintDialog = value; }
      }

      /// <summary>
      /// Gets or sets the export parameters.
      /// </summary>
      /// <value>
      /// The export parameters.
      /// </value>
      public StringCollection ExportParameters
      {
         get { return mobjExportParameters; }
         set { mobjExportParameters = value; }
      }
      #endregion
   }
}
