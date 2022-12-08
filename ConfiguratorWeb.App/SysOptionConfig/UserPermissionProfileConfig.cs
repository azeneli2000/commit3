using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   /// <summary>
   /// UserPermissionProfile
   /// </summary>
   public class UserPermissionProfile
   {
      private string mstrProfileName = "ProfileName";
      private int mintPermissionLevel = 0;
      private string mstrPermissionModifier = "Z";

      /// <summary>
      /// Gets or sets the name of the profile.
      /// </summary>
      /// <value>
      /// The name of the profile.
      /// </value>
      public string Name
      {
         get { return mstrProfileName; }
         set { mstrProfileName = value; }
      }

      /// <summary>
      /// Gets or sets the permission level.
      /// </summary>
      /// <value>
      /// The permission level.
      /// </value>
      public int PermissionLevel
      {
         get { return mintPermissionLevel; }
         set 
         {
            mintPermissionLevel = value;

            if (mintPermissionLevel < 0)
            {
               mintPermissionLevel = 0;
            }
            else
            {
               if (mintPermissionLevel > 99)
               {
                  mintPermissionLevel = 99;
               }
            }
         }
      }

      /// <summary>
      /// Gets or sets the permission modifier.
      /// </summary>
      /// <value>
      /// The permission modifier.
      /// </value>
      public string PermissionModifier
      {
         get { return mstrPermissionModifier; }
         set { mstrPermissionModifier = value; }
      }
   }

   public class UserPermissionProfileConfig
   {
      #region Members
      /// <summary>
      /// The permission profiles
      /// </summary>
      private List<UserPermissionProfile> mobjUserPermissionProfiles = new List<UserPermissionProfile>();
      #endregion

      #region Properties
      /// <summary>
      /// Gets or sets the modules.
      /// </summary>
      /// <value>
      /// The modules.
      /// </value>
      [CategoryAttribute("PermissionProfiles"), DescriptionAttribute("Configure permission profiles.")]
      public List<UserPermissionProfile> PermissionProfiles
      {
         get { return mobjUserPermissionProfiles; }
         set { mobjUserPermissionProfiles = value; }
      }
      #endregion

      /// <summary>
      /// Deserialize from xml
      /// </summary>
      /// <param name="config"></param>
      /// <returns></returns>
      public static UserPermissionProfileConfig Deserialize(string config)
      {
         UserPermissionProfileConfig objConfig = new UserPermissionProfileConfig();

         try
         {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<UserPermissionProfile>));
            StringReader objTextReader = new StringReader(config);
            objConfig.PermissionProfiles = (List<UserPermissionProfile>)xmlSerializer.Deserialize(objTextReader);
         }
         catch
         { 
            //Intentionally left emty
         }
         return objConfig;
      }

      /// <summary>
      /// Serialize to xml
      /// </summary>
      /// <returns></returns>
      public string Serialize()
      {
         string strResult = string.Empty;
         XmlSerializer objSrialize = new XmlSerializer(this.PermissionProfiles.GetType());

         using (StringWriter objTextWriter = new StringWriter())
         {
            objSrialize.Serialize(objTextWriter, this.PermissionProfiles);
            strResult = objTextWriter.ToString();
         }
         return strResult;
      }
   }
}
