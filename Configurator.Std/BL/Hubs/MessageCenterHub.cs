using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

using UMS.Framework.ContextManager;

using Configurator.Std.Model;
using Configurator.Std.Logging;
using Configurator.Std.BL.Configurator;



namespace Configurator.Std.BL.Hubs {

   /// <summary>
   /// Manage socket connection with http clients
   /// </summary>
   public class MessageCenterHub : Hub {

      private static UMS.Framework.ContextManager.UMSMessageCenterClient mobjClient = null;
      private static Dictionary<string, List<string>> ConnectedClients = new Dictionary<string, List<string>>();
      private static Dictionary<string, string> MappingConnectionIdToHostname = new Dictionary<string, string>();
      private static Dictionary<string, bool> MappingHostNameToDigistatModule = new Dictionary<string, bool>();
      private static Dictionary<string, DateTime> TemporaryRemovedClients = new Dictionary<string, DateTime>();
      private static Dictionary<string, UMSMessage> ExternalLoginRequests = new Dictionary<string, UMSMessage>();
      private static System.Threading.Thread mobjConnectionThread = null;
      private static System.Threading.Thread mobjDeleteDeadUsersThread = null;
      private static System.Threading.Thread mobjCloseSessionsThread = null;
      private static bool IsConnectedToMC = false;
      public static int ClientCounter = 0;
      public static int DigistatWebClientMax = 0;

      public static string LastCdssDetails { get; set; }
      public static DateTime LastCdssDetailsTime { get; set; }


      /// <summary>
      /// Initializes a new instance of the <see cref="MessageCenterHub"/> class.
      /// </summary>
      public MessageCenterHub()
         : base() {
      }

      static MessageCenterHub() {
         mobjClient = new UMS.Framework.ContextManager.UMSMessageCenterClient(DigistatConfig.ApplicationName, DigistatConfig.MessageCenter, Int32.Parse(DigistatConfig.MessageCenterInstance));
         mobjClient.Disconnected += mobjClient_Disconnected;
         mobjConnectionThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckConnection));
         mobjConnectionThread.IsBackground = true;
         mobjConnectionThread.Start();
      }

      #region Manage connection context

      public static void SetMyClientAsAlive(string hostname) {
         lock (ConnectedClients) {
            if (TemporaryRemovedClients.ContainsKey(hostname)) {
               TemporaryRemovedClients.Remove(hostname);
            }
         }
      }

      /// <summary>
      /// Add a connection
      /// </summary>
      /// <param name="hostname"></param>
      /// <param name="connectionId"></param>
      private void AddContext(string hostname, string connectionId) {
         lock (ConnectedClients) {
            if (!ConnectedClients.ContainsKey(hostname)) {
               ConnectedClients.Add(hostname, new List<string>());
            }
            ConnectedClients[hostname].Add(connectionId);
            bool bolIsDigModule = Context.QueryString["DIG"] == "1";
            if (MappingHostNameToDigistatModule.ContainsKey(hostname)) {
               if (MappingHostNameToDigistatModule[hostname] != bolIsDigModule) {
                  UMSMessage objMessage = new UMSMessage();
                  objMessage.DestinationApp = DigistatConfig.ApplicationName;
                  objMessage.DestinationHost = hostname;
                  objMessage.PacketType = "RCMD";
                  objMessage.SourceHost = UMS.Framework.UMSUtility.GetWorkstationName().ToUpper();
                  objMessage.SourceApp = DigistatConfig.ApplicationName;
                  objMessage.Message = "WEB_CONCURRENT_SESSION_OPEN";
                  var objHub = GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
                  objHub.Clients.Client(connectionId).sendMessage(objMessage);
                  //throw new Exception();                  
               }
            } else {
               MappingHostNameToDigistatModule.Add(hostname, bolIsDigModule);
            }


            if (!MappingConnectionIdToHostname.ContainsKey(connectionId)) {
               MappingConnectionIdToHostname.Add(connectionId, hostname);
            } else {
               MappingConnectionIdToHostname[connectionId] = hostname;
            }

            if (TemporaryRemovedClients.ContainsKey(hostname)) {
               TemporaryRemovedClients.Remove(hostname);
            }

         }
      }

      /// <summary>
      /// Remove a connection
      /// </summary>
      /// <param name="connectionId"></param>
      private void RemoveContext(string connectionId) {
         lock (ConnectedClients) {
            string strHostname = string.Empty;
            if (MappingConnectionIdToHostname.ContainsKey(connectionId)) {
               strHostname = MappingConnectionIdToHostname[connectionId];
               MappingConnectionIdToHostname.Remove(connectionId);
               if (!MappingConnectionIdToHostname.ContainsValue(strHostname)) {
                  MappingHostNameToDigistatModule.Remove(strHostname);
               }

            }
            if (!string.IsNullOrEmpty(strHostname)) {
               if (ConnectedClients[strHostname].Contains(connectionId)) {
                  ConnectedClients[strHostname].Remove(connectionId);
               }
            } else {
               // Fallback, it should never execute this code
               foreach (string hostname in ConnectedClients.Keys) {
                  if (ConnectedClients[hostname].Contains(connectionId)) {
                     ConnectedClients[hostname].Remove(connectionId);
                     break;
                  }
               }
            }

            if (!TemporaryRemovedClients.ContainsKey(strHostname)) {
               TemporaryRemovedClients.Add(strHostname, DateTime.UtcNow);
            } else {
               TemporaryRemovedClients[strHostname] = DateTime.UtcNow;
            }

         }
      }

      #endregion

      #region Threads

    

      
      /// <summary>
      /// Check MC connection
      /// </summary>
      private static void CheckConnection() {
         try {
            while (true) {
               if (!mobjClient.MessageCenterIsConnected()) {
                  mobjClient.ConnectToMessageCenter();
                  if (mobjClient.MessageCenterIsConnected()) {
                     DigistatWebClientMax = mobjClient.AuthData.DigistatWebCount;
                     //Set connection string in static class DigistatConfig owner of the centralized configurations
                     DigistatConfig.ConnString = mobjClient.AuthData.ConnectionString;
                     //Updates logger connection string to enable db logging
                     UmsLogger.Instance.UpdateConnectionString();
                     mobjClient.MessageReceived += mobjClient_MessageReceived;
                     if (mobjCloseSessionsThread != null) {
                        if (!mobjCloseSessionsThread.IsAlive) {
                           mobjCloseSessionsThread.Start();
                        }
                     }


                     IsConnectedToMC = true;
                     LastCdssDetailsTime = DateTime.UtcNow;
                     var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
                     UMS.Framework.ContextManager.UMSMessage message = new UMS.Framework.ContextManager.UMSMessage();
                     message.PacketType = "RCMD";
                     message.Message = "CONNECTED_TO_MC";
                     hub.Clients.All.sendMessage(message);
                     break;
                  }
               } else {
                  IsConnectedToMC = false;
                  System.Threading.Thread.Sleep(10000);
               }
            }
         } catch (Exception) {
            //TODO : Do something!!!
         }
      }

      #endregion

      #region Manage local message dispatching

      /// <summary>
      /// Get local clients list
      /// </summary>
      /// <param name="hostname"></param>
      /// <returns></returns>
      public List<string> GetLocalDestinations(string hostname) {
         List<string> objDestinations = null;

         lock (ConnectedClients) {
            if (ConnectedClients.ContainsKey(hostname)) {
               objDestinations = ConnectedClients[hostname];
            }
         }
         return objDestinations;
      }

      /// <summary>
      /// Get a client list using a filter
      /// </summary>
      /// <param name="message"></param>
      /// <param name="destinationIds"></param>
      public List<string> GetLocalDestinationsByFilter(string hostname, Predicate<string> filter) {
         List<string> objDestinations = null;

         lock (ConnectedClients) {
            if (ConnectedClients.ContainsKey(hostname)) {
               objDestinations = ConnectedClients[hostname].FindAll(filter);
            }
         }
         return objDestinations;
      }

     
      /// <summary>
      /// Send a message to all client connected to DigistatWeb
      /// </summary>
      /// <param name="message"></param>
      /// <param name="destinationIds"></param>
      public static void SendMessageToAllClients(UMS.Framework.ContextManager.UMSMessage message) {
         var objHub = GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
         objHub.Clients.All.sendMessage(UMSMessageEx.ConvertMessageToMessageEx(message));
      }


      //#region Local Messages

   

      ///// <summary>
      ///// Send a message to all client connected to DigistatWeb
      ///// </summary>
      ///// <param name="message"></param>
      ///// <param name="destinationIds"></param>
      //public static void SendLocalMessage(UMSNetwork objNetwork, UMSMessageEx message) {
      //   lock (ConnectedClients) {
      //      string strHostName = objNetwork.SyncAddress;
      //      //if(DigistatConfig.ClientAuthMode != ClientEnableMode.DigistatStyle)
      //      //{
      //      //   strHostName = objNetwork.IpAddress;
      //      //}
      //      if (ConnectedClients.ContainsKey(strHostName)) {
      //         SendLocalMessage(message, ConnectedClients[strHostName]);
      //      }
      //   }
      //}

      ///// <summary>
      ///// Send a message to all client connected to DigistatWeb
      ///// </summary>
      ///// <param name="message"></param>
      ///// <param name="destinationIds"></param>
      //public static void SendLocalMessage(string hostname, UMSMessageEx message) {
      //   lock (ConnectedClients) {
      //      if (ConnectedClients.ContainsKey(hostname)) {
      //         SendLocalMessage(message, ConnectedClients[hostname]);
      //      }
      //   }
      //}

      ///// <summary>
      ///// Send a message to all client connected to DigistatWeb
      ///// </summary>
      ///// <param name="message"></param>
      ///// <param name="destinationIds"></param>
      //public static void SendLocalMessage(UMS.Framework.ContextManager.UMSMessage message, IList<string> destinationIds) {
      //   lock (ConnectedClients) {
      //      var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
      //      hub.Clients.Clients(destinationIds).sendMessage(message);
      //   }
      //}

      ///// <summary>
      ///// Send a message to all client connected to DigistatWeb
      ///// </summary>
      ///// <param name="message"></param>
      ///// <param name="destinationIds"></param>
      //public static void SendLocalMessage(UMSMessageEx message, IList<string> destinationIds) {
      //   if (destinationIds != null && destinationIds.Count > 0) {
      //      lock (ConnectedClients) {
      //         var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
      //         hub.Clients.Clients(destinationIds).sendMessage(message);
      //      }
      //   }
      //}

      //#endregion

      /// <summary>
      /// Send a message to MC
      /// </summary>
      /// <param name="message"></param>
      public static void SendMessageToMC(UMS.Framework.ContextManager.UMSMessage message) {
         lock (mobjClient) {
            mobjClient.MessageBuilder.Connection.SendMessage(message);
         }
      }

      #endregion

      #region Manage external logins

      public static void ResetExternalLoginRequest(string requestId) {
         lock (ExternalLoginRequests) {
            if (ExternalLoginRequests.ContainsKey(requestId)) {
               ExternalLoginRequests.Remove(requestId);
            }
         }
      }

      public static UMSMessage CheckExternalLoginRequest(string requestId) {
         UMSMessage objMessage = null;
         lock (ExternalLoginRequests) {
            if (ExternalLoginRequests.ContainsKey(requestId)) {
               objMessage = ExternalLoginRequests[requestId];
            }
         }
         return objMessage;
      }

      #endregion

     

      #region MessageCenter events

      /// <summary>
      /// Determines whether connection to messagecenter is established
      /// </summary>
      /// <returns></returns>
      public static bool IsMessageCenterConnected() {
         return IsConnectedToMC;
      }

      /// <summary>
      /// Manage messages received from the messagecenter
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="UMS.Framework.ContextManager.MessageSentOrReceivedReceivedEventArgs"/> instance containing the event data.</param>
      static void mobjClient_MessageReceived(object sender, UMS.Framework.ContextManager.MessageSentOrReceivedReceivedEventArgs e) {
         
            var objHub = GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");

            UMSMessageEx objMessageToDispatch = UMSMessageEx.ConvertMessageToMessageEx(e.Message);

            throw new NotImplementedException();
      }

      /// <summary>
      /// Manage the disconnection from the messagecenter
      /// </summary>
      /// <param name="sender">The sender.</param>
      static void mobjClient_Disconnected(object sender) {
         IsConnectedToMC = false;
         mobjClient.MessageReceived -= mobjClient_MessageReceived;
         var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
         UMS.Framework.ContextManager.UMSMessage message = new UMS.Framework.ContextManager.UMSMessage();
         message.PacketType = "RCMD";
         message.Message = "DISCONNECTED_TO_MC";

         hub.Clients.All.sendMessage(message);

         mobjConnectionThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckConnection));
         mobjConnectionThread.Start();
      }

      #endregion

      #region SignalR events

      /// <summary>
      /// Handle connection event
      /// </summary>
      /// <returns></returns>
      public override System.Threading.Tasks.Task OnConnected() {
         //ClientCounter++;
         //string strHostname = GetHostname();
         //if (DigistatConfig.ClientAuthMode != ClientEnableMode.DigistatStyle) {
         //   strHostname = strHostname + "|" + Context.QueryString["uid"];
         //   AddContext(strHostname, Context.ConnectionId);
         //} else {
         //   AddContext(strHostname, Context.ConnectionId);
         //   if (!IsConnectedToMC) {
         //      var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext("MessageCenterHub");
         //      UMS.Framework.ContextManager.UMSMessage message = new UMS.Framework.ContextManager.UMSMessage();
         //      message.PacketType = "RCMD";
         //      message.Message = "DISCONNECTED_TO_MC";
         //      hub.Clients.All.sendMessage(message);
         //   }
         //}


         return base.OnConnected();
      }


      //public static void RemoveClientCounter()
      //{
      //   ClientCounter--;
      //}

      /// <summary>
      /// Handle disconnection event
      /// </summary>
      /// <param name="stopCalled"></param>
      /// <returns></returns>
      public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled) {
         try {
            //ClientCounter--;

            //RemoveContext(Context.ConnectionId);
         } catch (Exception) {

         }
         return base.OnDisconnected(stopCalled);
      }

      /// <summary>
      /// Handle reconnection on poor connections
      /// </summary>
      /// <returns></returns>
      public override System.Threading.Tasks.Task OnReconnected() {
         //ClientCounter++;


         //string strHostname = GetHostname();
         //if (DigistatConfig.ClientAuthMode != ClientEnableMode.DigistatStyle) {
         //   strHostname = strHostname + "|" + Context.QueryString["uid"];
         //}
         //AddContext(strHostname, Context.ConnectionId);

         return base.OnReconnected();
      }

      #endregion

     
   }


}
