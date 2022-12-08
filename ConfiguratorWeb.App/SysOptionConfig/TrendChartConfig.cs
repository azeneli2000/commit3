using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   //class ParameterEditor : UITypeEditor
   //{
   //   public static string ConnectionString = "";

   //   public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
   //   {
   //      return UITypeEditorEditStyle.Modal;
   //   }

   //   public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
   //   {
   //      var svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
   //      string text = value as string;
   //      if (svc != null)
   //      {
   //         using (frmParameterChooser form = new frmParameterChooser(ConnectionString, text))
   //         {
   //            if (svc.ShowDialog(form) == DialogResult.OK)
   //            {
   //               return form.ParamId;
   //            }
   //         }
   //         return value;
   //      }

   //      return value;
   //   }
   //}

   public class ActualDeviceConverter : StringConverter
   {
      public override bool GetStandardValuesSupported(
                              ITypeDescriptorContext context)
      {
         return true;
      }

      public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
      {
         return new StandardValuesCollection(GetActualDevice().ToArray());
      }

      public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
      {
         return false;
      }

      public List<string> GetActualDevice()
      {
         List<string> objNames = new List<string>();
         //UMS.Framework.Data.DBManager objManager = new UMS.Framework.Data.DBManager(
         //   UMS.Framework.Data.DBProvider.SqlServer, ParameterEditor.ConnectionString);
         //objManager.Open();

         //IDataReader objReader = objManager.ExecuteReader(CommandType.Text, @"select distinct ad_Name from ActualDevice order by ad_Name");

         //while (objReader.Read())
         //{
         //   if (objReader["ad_Name"] != DBNull.Value)
         //   {
         //      if (!string.IsNullOrEmpty(objReader["ad_Name"].ToString()))
         //      {
         //         objNames.Add(objReader["ad_Name"].ToString());
         //      }
         //   }
         //}
         //objManager.CloseReader();
         //objManager.Close();
         //objManager.Dispose();
         return objNames;
      }
   }

   public class BedConverter : StringConverter
   {
      public override bool GetStandardValuesSupported(
                              ITypeDescriptorContext context)
      {
         return true;
      }

      public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
      {
         return new StandardValuesCollection(GetBeds().ToArray());
      }

      public override bool GetStandardValuesExclusive(
                           ITypeDescriptorContext context)
      {
         return false;
      }

      public List<string> GetBeds()
      {
         List<string> objNames = new List<string>();
         //         UMS.Framework.Data.DBManager objManager = new UMS.Framework.Data.DBManager(
         //            UMS.Framework.Data.DBProvider.SqlServer, ParameterEditor.ConnectionString);
         //         objManager.Open();

         //         IDataReader objReader = objManager.ExecuteReader(CommandType.Text, @"select (b.LocationName + ' -- ' + a.BedName + ' (' + CAST(a.IDBed as nvarchar(max)) +')') as Name from Bed a inner join Location b on a.LocationRef = b.IDLocation
         //order by b.LocationIndex, a.BedIndex");

         //         while (objReader.Read())
         //         {
         //            if (objReader["Name"] != DBNull.Value)
         //            {
         //               if (!string.IsNullOrEmpty(objReader["Name"].ToString()))
         //               {
         //                  objNames.Add(objReader["Name"].ToString());
         //               }
         //            }
         //         }
         //         objManager.CloseReader();
         //         objManager.Close();
         //         objManager.Dispose();
         return objNames;
      }
   }
   public class TrendSerie
   {
      //private string mstrParameterId = string.Empty;

      [DescriptionAttribute("Name of the serie")]
      public string Name { get; set; }

      //[DescriptionAttribute("Parameter id associated to this serie."),
      //   //Editor(typeof(ParameterEditor), typeof(UITypeEditor)),
      //   TypeConverter(typeof(ExpandableObjectConverter))]
      //public string ParameterId
      //{
      //   get
      //   {
      //      return mstrParameterId;
      //   }
      //   set
      //   {
      //      mstrParameterId = value;
      //      ParameterName = GetParameterName(mstrParameterId);
      //   }
      //}

      //[DescriptionAttribute("Parameter name associated to this serie."), ReadOnly(true)]
      //public string ParameterName { get; set; }

      [DescriptionAttribute("Actual device name. If empty parameter can be taken from any device."),
         TypeConverter(typeof(ActualDeviceConverter))]
      public string ActualDeviceName { get; set; }
      public System.Drawing.Color ColorRGB
      {
         get
         {
            return System.Drawing.ColorTranslator.FromHtml(ColorRGBHTML);
         }
         set
         {
            ColorRGBHTML = System.Drawing.ColorTranslator.ToHtml(value);
         }
      }
      [ReadOnly(true), Browsable(false)]
      public string ColorRGBHTML { get; set; }
      //[DescriptionAttribute("Type of chart.")]
      //public System.Windows.Forms.DataVisualization.Charting.SeriesChartType ChartType { get; set; }

      //public System.Windows.Forms.DataVisualization.Charting.MarkerStyle MarkerStyleFilterOff { get; set; }

      //public System.Windows.Forms.DataVisualization.Charting.MarkerStyle MarkerStyleFilterOn { get; set; }

      [ReadOnly(true), Browsable(false)]
      public DataTable Data { get; set; }
      [ReadOnly(true), Browsable(false)]
      public double CalculatedMaximum { get; set; }
      [ReadOnly(true), Browsable(false)]
      public double CalculatedMinimum { get; set; }
      [ReadOnly(true), Browsable(false)]
      public string UnitId { get; set; }
      //[DescriptionAttribute("Data value type of the Y-axis.")]
      //public System.Windows.Forms.DataVisualization.Charting.ChartValueType YValueType { get; set; }

      //public string GetParameterName(string id)
      //{
      //   string strName = string.Empty;
      //   //         UMS.Framework.Data.DBManager objManager = new UMS.Framework.Data.DBManager(
      //   //            UMS.Framework.Data.DBProvider.SqlServer, ParameterEditor.ConnectionString);
      //   //         objManager.Open();
      //   //         objManager.CreateParameters(1);
      //   //         objManager.SetParameter(0, "@parid", id);
      //   //         object objValue = objManager.ExecuteScalar(CommandType.Text, @"select c.par_Print
      //   //from Standardparameter c where c.par_ID = @parid");
      //   //         if (objValue != null && objValue != DBNull.Value)
      //   //         {
      //   //            strName = objValue.ToString();
      //   //         }

      //   //         objManager.Close();
      //   //         objManager.Dispose();
      //   return strName;
      //}

      public override string ToString()
      {
         return Name;
      }
   }

   public class TrendChart
   {
      [DescriptionAttribute("Title of the chart")]
      public string Title { get; set; }
      [DescriptionAttribute("Max value for Y-axis. IF empty will be auto calculated.")]
      public float? YMax { get; set; }
      [DescriptionAttribute("Min value for Y-axis. If empty will be auto calculated")]
      public float? YMin { get; set; }
      [DescriptionAttribute("Format string for the Y-axis. Example: {0:000}")]
      public string YFormat { get; set; }
      [DescriptionAttribute("Height of the chart if SizeMOde is scrollable.")]
      public int ChartHeight { get; set; }
      [DescriptionAttribute("List of series displayed in this chart")]
      public List<TrendSerie> Series { get; set; }

      public TrendChart()
      {
         Series = new List<TrendSerie>();
      }

      public override string ToString()
      {
         return Title;
      }
      public double GetMaximum()
      {
         return Series.Max(m => m.CalculatedMaximum);
      }

      public double GetMinimum()
      {
         return Series.Min(m => m.CalculatedMinimum);
      }
   }

   public class TrendCharts
   {
      public enum TrendSizeMode
      {
         AutoFit = 0,
         Scrollable = 1
      }

      [DescriptionAttribute("If Autofit all charts will be displayed on the same page. If scrollable all charts will have a configured height.")]
      public TrendSizeMode SizeMode { get; set; }
      [DescriptionAttribute("List of configured charts.")]
      public List<TrendChart> Charts { get; set; }
      [DescriptionAttribute("Associated bed. If empty chart configuration is valid for all beds."),
    TypeConverter(typeof(BedConverter))]
      public string Bed { get; set; }

      public TrendCharts()
      {
         Charts = new List<TrendChart>();
      }

      public override string ToString()
      {
         return Bed;
      }
   }

   public class TrendChartConfiguration
   {
      public List<TrendCharts> Configuration { get; set; }

      public TrendChartConfiguration()
      {
         Configuration = new List<TrendCharts>();
      }

      public static TrendChartConfiguration Deserialize(string xml)
      {
         XmlSerializer objSerializer = new XmlSerializer(typeof(TrendChartConfiguration));
         System.IO.StringReader objReader = new System.IO.StringReader(xml);
         TrendChartConfiguration objConfig = (TrendChartConfiguration)objSerializer.Deserialize(objReader);
         return objConfig;
      }

      public string Serialize()
      {
         string strResult = string.Empty;
         XmlSerializer objSerializer = new XmlSerializer(typeof(TrendChartConfiguration));
         System.IO.StringWriter objWriter = new System.IO.StringWriter();
         objSerializer.Serialize(objWriter, this);
         strResult = objWriter.ToString();
         objWriter.Dispose();
         return strResult;
      }
   }
}
