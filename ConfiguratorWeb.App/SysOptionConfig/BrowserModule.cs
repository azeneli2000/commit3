using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConfiguratorWeb.App.SysOptionConfig
{
   /// <summary>
   /// Browser Module
   /// </summary>
   public class BrowserModule
   {
      private string mstrModuleName = string.Empty;
      private string mstrCallback = string.Empty;
      private string mstrAddress = string.Empty;
      private string mstrAuthenticationAddress = string.Empty;
      private bool mbolIncludeLogin = false;
      private bool mbolIncludePatient = false;
      private bool mbolIncludeNetwork = false;
      private bool mbolReceiveMessages = false;
      private bool mbolHideToolBar = false;
      private bool mbolPatientRelated = false;
      private bool mbolSuppressErrors = false;
      private bool mbolLeaveConfirmation = false;

      public BrowserModule()
      {
         mstrModuleName = "YourModuleName";
         mstrCallback = "YourCallBack";
         mstrAddress = "http://YourUriAddress/YourModule/YourEndpoint?param1=x&amp;param2=0";
         mstrAuthenticationAddress = "";
         mbolIncludeLogin = true;
         mbolIncludePatient = false;
         mbolIncludeNetwork = true;
         mbolReceiveMessages = false;
         mbolHideToolBar = true;
         mbolPatientRelated = false;
         mbolSuppressErrors = true;
         mbolLeaveConfirmation = false;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BrowserModule" /> class.
      /// </summary>
      /// <param name="moduleName">Name of the module.</param>
      /// <param name="callback">The callback.</param>
      /// <param name="address">The address.</param>
      /// <param name="authenticationAddress">The address to call for login authorization.</param>
      /// <param name="includeLogin">if set to <c>true</c> [include login].</param>
      /// <param name="includePatient">if set to <c>true</c> [include patient].</param>
      /// <param name="includeNetwork">if set to <c>true</c> [include network].</param>
      /// <param name="receiveMessages">if set to <c>true</c> [receive messages].</param>
      /// <param name="hideToolBar">if set to <c>true</c> [hide tool bar].</param>
      /// <param name="patientRelated">if set to <c>true</c> [patient related].</param>
      /// <param name="suppressErrors"></param>
      /// <param name="leaveConfirmation"></param>
      public BrowserModule(string moduleName, string callback, string address, string authenticationAddress, bool includeLogin,
         bool includePatient, bool includeNetwork, bool receiveMessages, bool hideToolBar, bool patientRelated, bool suppressErrors,
         bool leaveConfirmation)
      {
         mstrModuleName = moduleName;
         mstrCallback = callback;
         mstrAddress = address;
         mstrAuthenticationAddress = authenticationAddress;
         mbolIncludeLogin = includeLogin;
         mbolIncludePatient = includePatient;
         mbolIncludeNetwork = includeNetwork;
         mbolReceiveMessages = receiveMessages;
         mbolHideToolBar = hideToolBar;
         mbolPatientRelated = patientRelated;
         mbolSuppressErrors = suppressErrors;
         mbolLeaveConfirmation = leaveConfirmation;
      }

      #region Properties

      /// <summary>
      /// Gets or sets a value indicating whether [suppress errors].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [suppress errors]; otherwise, <c>false</c>.
      /// </value>
      public bool SuppressErrors
      {
         get { return mbolSuppressErrors; }
         set { mbolSuppressErrors = value; }
      }

      /// <summary>
      /// Gets the name of the module.
      /// </summary>
      /// <value>The name of the module.</value>
      public string ModuleName
      {
         get { return mstrModuleName; }
      }

      /// <summary>
      /// Gets the callback.
      /// </summary>
      /// <value>The callback.</value>
      public string Callback
      {
         get { return mstrCallback; }
      }

      /// <summary>
      /// Gets the address.
      /// </summary>
      /// <value>The address.</value>
      public string Address
      {
         get { return mstrAddress; }
      }

      /// <summary>
      /// Gets the authentication address.
      /// </summary>
      /// <value>The address.</value>
      public string AuthenticationAddress
      {
         get { return mstrAuthenticationAddress; }
      }

      /// <summary>
      /// Gets a value indicating whether [include login].
      /// </summary>
      /// <value><c>true</c> if [include login]; otherwise, <c>false</c>.</value>
      public bool IncludeLogin
      {
         get { return mbolIncludeLogin; }
      }

      /// <summary>
      /// Gets a value indicating whether [include patient].
      /// </summary>
      /// <value><c>true</c> if [include patient]; otherwise, <c>false</c>.</value>
      public bool IncludePatient
      {
         get { return mbolIncludePatient; }
      }

      /// <summary>
      /// Gets a value indicating whether [include network].
      /// </summary>
      /// <value><c>true</c> if [include network]; otherwise, <c>false</c>.</value>
      public bool IncludeNetwork
      {
         get { return mbolIncludeNetwork; }
      }

      /// <summary>
      /// Gets a value indicating whether [receive messages].
      /// </summary>
      /// <value><c>true</c> if [receive messages]; otherwise, <c>false</c>.</value>
      public bool ReceiveMessages
      {
         get { return mbolReceiveMessages; }
      }

      /// <summary>
      /// Gets a value indicating whether [hide toolbar].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [hide toolbar]; otherwise, <c>false</c>.
      /// </value>
      public bool HideToolbar
      {
         get { return mbolHideToolBar; }
      }

      /// <summary>
      /// Gets a value indicating whether [patient related].
      /// </summary>
      /// <value>
      ///   <c>true</c> if [patient related]; otherwise, <c>false</c>.
      /// </value>
      public bool PatientRelated
      {
         get { return mbolPatientRelated; }
      }

      /// <summary>
      /// Gets a value indicating whether [LeaveConfirmation].
      /// </summary>
      /// <value><c>true</c> if [LeaveConfirmation]; otherwise, <c>false</c>.</value>
      public bool LeaveConfirmation
      {
         get { return mbolLeaveConfirmation; }
      }
      #endregion
   }
   /// <summary>
   /// A collection of BrowserModules
   /// </summary>
   public class BrowserModules : List<BrowserModule>
   {
      /// <summary>
      /// Deserializes the specified modules.
      /// </summary>
      /// <param name="modules">The modules.</param>
      /// <returns></returns>
      public static BrowserModules Deserialize(string modules)
      {
         BrowserModules objModules = new BrowserModules();

         StringReader objStringReader = new StringReader(modules);
         XmlTextReader objReader = new XmlTextReader(objStringReader);

         while (objReader.Read())
         {
            switch (objReader.NodeType)
            {
               case XmlNodeType.Element:
                  if (objReader.Name.CompareTo("Module") == 0)
                  {
                     bool bolHideToolbar = false;
                     bool bolPatientRelated = false;
                     bool bolSuppressErrors = false;
                     if (!string.IsNullOrEmpty(objReader.GetAttribute("HideToolbar")))
                     {
                        bolHideToolbar = Convert.ToBoolean(objReader.GetAttribute("HideToolbar"));
                     }
                     if (!string.IsNullOrEmpty(objReader.GetAttribute("PatientRelated")))
                     {
                        bolPatientRelated = Convert.ToBoolean(objReader.GetAttribute("PatientRelated"));
                     }
                     if (!string.IsNullOrEmpty(objReader.GetAttribute("SuppressErrors")))
                     {
                        bolSuppressErrors = Convert.ToBoolean(objReader.GetAttribute("SuppressErrors"));
                     }
                     BrowserModule objModule = new BrowserModule(objReader.GetAttribute("Name"),
                        objReader.GetAttribute("Callback"), objReader.GetAttribute("Address"), objReader.GetAttribute("AuthenticationAddress"), Convert.ToBoolean(objReader.GetAttribute("IncludeLogin")),
                        Convert.ToBoolean(objReader.GetAttribute("IncludePatient")), Convert.ToBoolean(objReader.GetAttribute("IncludeNetwork")),
                        Convert.ToBoolean(objReader.GetAttribute("ReceiveMessages")), bolHideToolbar, bolPatientRelated, bolSuppressErrors, Convert.ToBoolean(objReader.GetAttribute("LeaveConfirmation")));
                     objModules.Add(objModule);
                  }
                  break;
               default:
                  break;
            }
         }
         objReader.Close();
         objStringReader.Dispose();
         return objModules;
      }

      /// <summary>
      /// Serializes to string.
      /// </summary>
      /// <returns></returns>
      public string Serialize()
      {
         StringBuilder objBuilder = new StringBuilder();

         objBuilder.AppendLine("<BrowserModules>");

         foreach (BrowserModule module in this)
         {
            
            objBuilder.AppendLine(
               "<Module Name=\"" + module.ModuleName + "\" Callback=\"" + module.Callback + "\" Address=\"" + module.Address +"\" AuthenticationAddress=\"" + module.AuthenticationAddress +
               "\" IncludeLogin=\"" + module.IncludeLogin.ToString() + "\" IncludePatient=\"" + module.IncludePatient + "\" IncludeNetwork=\"" +
               module.IncludeNetwork.ToString() + "\" ReceiveMessages=\"" + module.ReceiveMessages.ToString() +
               "\" HideToolbar=\"" + module.HideToolbar.ToString() + "\" PatientRelated=\"" + module.PatientRelated.ToString() + "\" LeaveConfirmation=\"" + module.LeaveConfirmation.ToString() + "\"/>"
               );
         }
         objBuilder.AppendLine("</BrowserModules>");
         return objBuilder.ToString();
      }

      
   }
}
