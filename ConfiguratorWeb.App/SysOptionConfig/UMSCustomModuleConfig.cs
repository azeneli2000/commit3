using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   /// <summary>
   /// UMSCustomModuleModule
   /// </summary>
   public class UMSCustomModuleModule
   {
      private string mstrModuleName = string.Empty;
      private string mstrModuleLongName = string.Empty;
      private bool mbolPatientUnrelated = true;

      /// <summary>
      /// Gets or sets the name of the module.
      /// </summary>
      /// <value>
      /// The name of the module.
      /// </value>
      public string ModuleName
      {
         get { return mstrModuleName; }
         set { mstrModuleName = value; }
      }

      /// <summary>
      /// Gets or sets the long name of the module.
      /// </summary>
      /// <value>
      /// The long name of the module.
      /// </value>
      public string ModuleLongName
      {
         get { return mstrModuleLongName; }
         set { mstrModuleLongName = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [patient unrelated].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [patient unrelated]; otherwise, <c>false</c>.
      /// </value>
      public bool PatientUnrelated
      {
         get { return mbolPatientUnrelated; }
         set { mbolPatientUnrelated = value; }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString()
      {
         return mstrModuleName;
      }
   }

   /// <summary>
   /// UMSCustomModuleMenuItem
   /// </summary>
   public class UMSCustomModuleMenuItem
   {
      private string mstrMenuPath = string.Empty;
      private string mstrCallback = string.Empty;
      private string mstrName = string.Empty;

      /// <summary>
      /// Gets or sets the menu path.
      /// </summary>
      /// <value>
      /// The menu path.
      /// </value>
      public string MenuPath
      {
         get { return mstrMenuPath; }
         set { mstrMenuPath = value; }
      }

      /// <summary>
      /// Gets or sets the callback.
      /// </summary>
      /// <value>
      /// The callback.
      /// </value>
      public string Callback
      {
         get { return mstrCallback; }
         set { mstrCallback = value; }
      }

      /// <summary>
      /// Gets or sets the name.
      /// </summary>
      /// <value>
      /// The name.
      /// </value>
      public string Name
      {
         get { return mstrName; }
         set { mstrName = value; }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString()
      {
         return mstrName;
      }
   }

   /// <summary>
   /// UMSCustomModuleSetting
   /// </summary>
   public class UMSCustomModuleSetting
   {
      private string mstrKey = string.Empty;
      private string mstrValue = string.Empty;

      /// <summary>
      /// Gets or sets the key.
      /// </summary>
      /// <value>
      /// The key.
      /// </value>
      public string Key
      {
         get { return mstrKey; }
         set { mstrKey = value; }
      }

      /// <summary>
      /// Gets or sets the value.
      /// </summary>
      /// <value>
      /// The value.
      /// </value>
      public string Value
      {
         get { return mstrValue; }
         set { mstrValue = value; }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString()
      {
         return mstrKey;
      }
   }

   /// <summary>
   /// CustomModule configuration
   /// </summary>
   public class UMSCustomModuleConfig
   {
      private List<UMSCustomModuleModule> mobjModules = new List<UMSCustomModuleModule>();
      private List<UMSCustomModuleMenuItem> mobjMenuItems = new List<UMSCustomModuleMenuItem>();
      private List<UMSCustomModuleSetting> mobjSettings = new List<UMSCustomModuleSetting>();

      /// <summary>
      /// Initializes a new instance of the <see cref="UMSCustomModuleConfig"/> class.
      /// </summary>
      public UMSCustomModuleConfig()
      {
      }

      public UMSCustomModuleMenuItem GetMenuItemByCallback(string callback)
      {
         UMSCustomModuleMenuItem objMenuItem = null;
         foreach (UMSCustomModuleMenuItem item in mobjMenuItems)
         {
            objMenuItem = item;
            break;
         }
         return objMenuItem;
      }

      #region Properties

      /// <summary>
      /// Gets or sets the modules.
      /// </summary>
      /// <value>
      /// The modules.
      /// </value>
      [CategoryAttribute("Modules"), DescriptionAttribute("Configure modules installed in the controlbar's application bar")]
      public List<UMSCustomModuleModule> Modules
      {
         get { return mobjModules; }
         set { mobjModules = value; }
      }

      /// <summary>
      /// Gets or sets the menu items.
      /// </summary>
      /// <value>
      /// The menu items.
      /// </value>
      [CategoryAttribute("Menu Handles"), DescriptionAttribute("Configure menu handles installed in the controlbar's menu")]
      public List<UMSCustomModuleMenuItem> MenuItems
      {
         get { return mobjMenuItems; }
         set { mobjMenuItems = value; }
      }

      /// <summary>
      /// Gets or sets the settings.
      /// </summary>
      /// <value>
      /// The settings.
      /// </value>
      [CategoryAttribute("Settings"), DescriptionAttribute("Settings are passed to the custom module Init method.")]
      public List<UMSCustomModuleSetting> Settings
      {
         get { return mobjSettings; }
         set { mobjSettings = value; }
      }
      #endregion

      /// <summary>
      /// Deserialize from xml
      /// </summary>
      /// <param name="config"></param>
      /// <returns></returns>
      public static UMSCustomModuleConfig Deserialize(string config)
      {
         UMSCustomModuleConfig objConfig = new UMSCustomModuleConfig();

         StringReader objStringReader = new StringReader(config);
         XmlTextReader objReader = new XmlTextReader(objStringReader);

         UMSCustomModuleMenuItem objMenuItem = null;
         UMSCustomModuleModule objModuleItem = null;
         UMSCustomModuleSetting objSetting = null;

         while (objReader.Read())
         {
            switch (objReader.NodeType)
            {
               case XmlNodeType.Element:

                  if (objReader.Name.CompareTo("Module") == 0)
                  {
                     objModuleItem = new UMSCustomModuleModule();
                     if (objReader.GetAttribute("Name") != null)
                     {
                        objModuleItem.ModuleName = objReader.GetAttribute("Name");
                     }
                     if (objReader.GetAttribute("LongName") != null)
                     {
                        objModuleItem.ModuleLongName = objReader.GetAttribute("LongName");
                     }
                     if (objReader.GetAttribute("PatientUnrelated") != null)
                     {
                        objModuleItem.PatientUnrelated = Convert.ToBoolean(objReader.GetAttribute("PatientUnrelated"));
                     }
                     objConfig.mobjModules.Add(objModuleItem);
                  }
                  if (objReader.Name.CompareTo("Menu") == 0)
                  {
                     objMenuItem = new UMSCustomModuleMenuItem();
                     if (objReader.GetAttribute("Name") != null)
                     {
                        objMenuItem.Name = objReader.GetAttribute("Name");
                     }
                     if (objReader.GetAttribute("Callback") != null)
                     {
                        objMenuItem.Callback = objReader.GetAttribute("Callback");
                     }
                     if (objReader.GetAttribute("Path") != null)
                     {
                        objMenuItem.MenuPath = objReader.GetAttribute("Path");
                     }
                     objConfig.mobjMenuItems.Add(objMenuItem);
                  }
                  if (objReader.Name.CompareTo("Setting") == 0)
                  {
                     objSetting = new UMSCustomModuleSetting();
                     if (objReader.GetAttribute("Key") != null)
                     {
                        objSetting.Key = objReader.GetAttribute("Key");
                     }
                     if (objReader.GetAttribute("Value") != null)
                     {
                        objSetting.Value = objReader.GetAttribute("Value");
                     }
                     objConfig.mobjSettings.Add(objSetting);
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

      /// <summary>
      /// Serialize to xml
      /// </summary>
      /// <returns></returns>
      public string Serialize()
      {
         StringBuilder objBuilder = new StringBuilder();

         objBuilder.AppendLine("<CustomModule>");

         objBuilder.Append("<Modules>");
         foreach (UMSCustomModuleModule item in this.Modules)
         {
            objBuilder.AppendLine(
               "<Module Name=\"" + item.ModuleName + "\" LongName=\"" + item.ModuleLongName + "\" PatientUnrelated=\"" + item.PatientUnrelated.ToString()
                  + "\" />");
         }
         objBuilder.Append("</Modules>");
         objBuilder.Append("<MenuItems>");
         foreach (UMSCustomModuleMenuItem item in this.MenuItems)
         {
            objBuilder.AppendLine(
               "<Menu Name=\"" + item.Name + "\" Callback=\"" + item.Callback + "\" Path=\"" + item.MenuPath + "\" />");
         }
         objBuilder.Append("</MenuItems>");
         objBuilder.Append("<Settings>");
         foreach (UMSCustomModuleSetting item in this.Settings)
         {
            objBuilder.AppendLine(
               "<Setting Key=\"" + item.Key + "\" Value=\"" + Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.XmlEscape(item.Value) + "\" />");
         }
         objBuilder.Append("</Settings>"); ;
         objBuilder.AppendLine("</CustomModule>");
         return objBuilder.ToString();
      }

      
   }
}
