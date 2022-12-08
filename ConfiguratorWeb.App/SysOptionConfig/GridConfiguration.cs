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
   public class GridParameter
   {
      private string mstrParameterId = string.Empty;

      //[DescriptionAttribute("Parameter id associated to this serie."),
      //Editor(typeof(ParameterEditor), typeof(UITypeEditor)),
      //TypeConverter(typeof(ExpandableObjectConverter))]
      public string ParameterId
      {
         get
         {
            return mstrParameterId;
         }
         set
         {
            mstrParameterId = value;
            ParameterName = string.Empty; //GetParameterName(mstrParameterId);
         }
      }
      [ReadOnly(true)]
      public string ParameterName { get; set; }
      public string ParameterNameCustom { get; set; }

//      public string GetParameterName(string id)
//      {
//         string strName = string.Empty;
////         UMS.Framework.Data.DBManager objManager = new UMS.Framework.Data.DBManager(
////            UMS.Framework.Data.DBProvider.SqlServer, ParameterEditor.ConnectionString);
////         objManager.Open();
////         objManager.CreateParameters(1);
////         objManager.SetParameter(0, "@parid", id);
////         object objValue = objManager.ExecuteScalar(CommandType.Text, @"select c.par_Print
////from Standardparameter c where c.par_ID = @parid");
////         if (objValue != null && objValue != DBNull.Value)
////         {
////            strName = objValue.ToString();
////         }

//         //objManager.Close();
//         //objManager.Dispose();
//         return strName;
//      }

      public override string ToString()
      {
         if (!string.IsNullOrEmpty(ParameterNameCustom))
         {
            return ParameterNameCustom;
         }
         else
         {
            return ParameterName;
         }
      }
   }

   public class GridConfiguration
   {
      public List<GridParameter> Parameters { get; set; }

      public GridConfiguration()
      {
         Parameters = new List<GridParameter>();
      }

      public List<string> GetParameters()
      {
         List<string> objItems = new List<string>();
         foreach (GridParameter param in Parameters)
         {
            objItems.Add(param.ParameterId);
         }
         return objItems;
      }

      public static GridConfiguration Deserialize(string xml)
      {
         GridConfiguration objConfig = null;

         if (!string.IsNullOrEmpty(xml))
         {
            XmlSerializer objSerializer = new XmlSerializer(typeof(GridConfiguration));
            System.IO.StringReader objReader = new System.IO.StringReader(xml);
            objConfig = (GridConfiguration)objSerializer.Deserialize(objReader);
         }
         else
         {
            objConfig = new GridConfiguration();
         }
         return objConfig;
      }

      public string Serialize()
      {
         string strResult = string.Empty;
         XmlSerializer objSerializer = new XmlSerializer(typeof(GridConfiguration));
         System.IO.StringWriter objWriter = new System.IO.StringWriter();
         objSerializer.Serialize(objWriter, this);
         strResult = objWriter.ToString();
         objWriter.Dispose();
         return strResult;
      }
   }
}
