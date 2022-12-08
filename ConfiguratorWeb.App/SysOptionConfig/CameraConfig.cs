using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   //class BedEditor : UITypeEditor
   //{
   //   public static string ConnectionString = "";

   //   public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
   //   {
   //      return UITypeEditorEditStyle.Modal;
   //   }

   //   public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
   //   {
   //      var svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
   //      int intValue = (int)value;
   //      if (svc != null)
   //      {
   //         using (frmBedChooser form = new frmBedChooser(ConnectionString, intValue))
   //         {
   //            if (svc.ShowDialog(form) == DialogResult.OK)
   //            {
   //               return form.BedId;
   //            }
   //         }
   //         return value;
   //      }

   //      return value;
   //   }
   //}

   public class VlcCommandLine : List<VlcCommand>
   {
      public VlcCommandLine()
      {

      }
   }

   public class VlcCommand
   {
      public string CommandText { get; set; }

      public VlcCommand()
      {

      }

      public VlcCommand(string value)
      {
         CommandText = value;
      }
   }
   public class CameraConfig
   {
      private int mintBedId = 0;

   //   [DescriptionAttribute("Bed id associated to this serie."),
   //  Editor(typeof(BedEditor), typeof(UITypeEditor)),
   //TypeConverter(typeof(ExpandableObjectConverter))]
      public int BedId
      {
         get
         {
            return mintBedId;
         }
         set
         {
            mintBedId = value;
            BedName = GetBedIdName(mintBedId);
         }
      }
      [ReadOnly(true)]
      public string BedName { get; set; }

      public string HomeURL { get; set; }
      public string DetailURL { get; set; }
      public string MobileURL { get; set; }
      public string User { get; set; }
      public string Password { get; set; }
      public int CameraMaxYPercInMixMode { get; set; }
      public int MobileProxyProtocol { get; set; }
      public VlcCommandLine VlcCommandLineHome { get; set; }
      public VlcCommandLine VlcCommandLineDetail { get; set; }

      public CameraConfig()
      {
         VlcCommandLineHome = new VlcCommandLine();
         VlcCommandLineDetail = new VlcCommandLine();
         MobileProxyProtocol = 0;
         CameraMaxYPercInMixMode = 30;
      }

      public string GetBedIdName(int id)
      {
         string strName = string.Empty;
         //         UMS.Framework.Data.DBManager objManager = new UMS.Framework.Data.DBManager(
         //            UMS.Framework.Data.DBProvider.SqlServer, BedEditor.ConnectionString);
         //         objManager.Open();
         //         objManager.CreateParameters(1);
         //         objManager.SetParameter(0, "@bedid", id);
         //         object objValue = objManager.ExecuteScalar(CommandType.Text, @"select b.LocationName + ' - ' + a.BedName 
         //from Bed a Inner join Location b on a.LocationRef=b.IDLocation where a.IDBed = @bedid");
         //         if (objValue != null && objValue != DBNull.Value)
         //         {
         //            strName = objValue.ToString();
         //         }

         //         objManager.Close();
         //         objManager.Dispose();
         return strName;
      }

      public void IncludeUserCredentialInCommandLine()
      {
         if (!string.IsNullOrEmpty(User))
         {
            bool bolIsPresent = false;
            foreach (VlcCommand value in VlcCommandLineHome)
            {
               if (value.CommandText.Contains("rtsp-user"))
               {
                  bolIsPresent = true;
                  break;
               }
            }
            if (!bolIsPresent)
            {
               VlcCommandLineHome.Add(new VlcCommand(":rtsp-user=" + User));
               VlcCommandLineHome.Add(new VlcCommand(":rtsp-pwd=" + Password));
            }
            bolIsPresent = false;
            foreach (VlcCommand value in VlcCommandLineDetail)
            {
               if (value.CommandText.Contains("rtsp-user"))
               {
                  bolIsPresent = true;
                  break;
               }
            }
            if (!bolIsPresent)
            {
               VlcCommandLineDetail.Add(new VlcCommand(":rtsp-user=" + User));
               VlcCommandLineDetail.Add(new VlcCommand(":rtsp-pwd=" + Password));
            }
         }
      }
   }

   public class CameraConfigs
   {
      public List<CameraConfig> Cameras { get; set; }

      public string VlcDirectory { get; set; }

      public CameraConfigs()
      {
         Cameras = new List<CameraConfig>();
      }


      public static CameraConfigs Deserialize(string xml)
      {
         CameraConfigs objConfig = null;

         if (!string.IsNullOrEmpty(xml))
         {
            XmlSerializer objSerializer = new XmlSerializer(typeof(CameraConfigs));
            System.IO.StringReader objReader = new System.IO.StringReader(xml);
            objConfig = (CameraConfigs)objSerializer.Deserialize(objReader);

            foreach (CameraConfig config in objConfig.Cameras)
            {
               config.IncludeUserCredentialInCommandLine();
            }
         }
         else
         {
            objConfig = new CameraConfigs();
         }
         return objConfig;
      }

      public string Serialize()
      {
         string strResult = string.Empty;
         XmlSerializer objSerializer = new XmlSerializer(typeof(CameraConfigs));
         System.IO.StringWriter objWriter = new System.IO.StringWriter();
         objSerializer.Serialize(objWriter, this);
         strResult = objWriter.ToString();
         objWriter.Dispose();
         return strResult;
      }
      public string Default()
      {
         string strResult = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <CameraConfigs xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
               <Cameras>
		            <CameraConfig>
			            <BedId>1</BedId>
			            <BedName>ICU - 1</BedName>
			            <HomeURL>rtsp://192.168.0.10/patientbed.mp4</HomeURL>
			            <DetailURL>rtsp://192.168.0.10/patientbed.mp4</DetailURL>
			            <User>admin</User>
			            <Password>password</Password>
			            <CameraMaxYPercInMixMode>30</CameraMaxYPercInMixMode>
			            <MobileProxyProtocol>0</MobileProxyProtocol>
			            <VlcCommandLineHome>
				            <VlcCommand>
					            <CommandText>:rtsp-user=admin</CommandText>
				            </VlcCommand>
				            <VlcCommand>
					            <CommandText>:rtsp-pwd=password</CommandText>
				            </VlcCommand>
			            </VlcCommandLineHome>
			            <VlcCommandLineDetail>
				            <VlcCommand>
					            <CommandText>:rtsp-user=admin</CommandText>
				            </VlcCommand>
				            <VlcCommand>
					            <CommandText>:rtsp-pwd=password</CommandText>
				            </VlcCommand>
			            </VlcCommandLineDetail>
		            </CameraConfig>
		            <CameraConfig>
			            <BedId>2</BedId>
			            <BedName>ICU - 2</BedName>
			            <MobileURL>rtsp://192.168.0.10:1234</MobileURL>
			            <CameraMaxYPercInMixMode>40</CameraMaxYPercInMixMode>
			            <MobileProxyProtocol>0</MobileProxyProtocol>
			            <VlcCommandLineHome />
			            <VlcCommandLineDetail />
		            </CameraConfig>
	            </Cameras>
            </CameraConfigs>";
         return strResult;
      }
   }
}
