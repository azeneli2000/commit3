using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMS.Framework.ContextManager;

namespace Configurator.Std.BL.Hubs
{
   /// <summary>
   /// Wrap UMSMessages in a serializable class
   /// </summary>
   public class UMSMessageEx
   {
      public string PacketType { get; set; }
      public string Message { get; set; }
      public string PatientID { get; set; }
      public string DestinationHost { get; set; }
      public string DestinationApp { get; set; }
      public string SourceApp { get; set; }
      public string SourceHost { get; set; }
      public List<System.Collections.DictionaryEntry> Options { get; set; }
      public bool DestinationExcludeMe { get; set; }
      public bool DestinationExcludeMyWorkstation { get; set; }

      public static UMSMessageEx ConvertMessageToMessageEx(UMSMessage message)
      {
         UMSMessageEx objMessageToDispatch = new UMSMessageEx();
         objMessageToDispatch.PacketType = message.PacketType;
         objMessageToDispatch.Message = message.Message;
         objMessageToDispatch.DestinationApp = message.DestinationApp;
         objMessageToDispatch.PatientID = message.PatientID.ToString();
         objMessageToDispatch.SourceApp = message.SourceApp;
         objMessageToDispatch.SourceHost = message.SourceHost;
         objMessageToDispatch.Options = new List<System.Collections.DictionaryEntry>();
         for (int i = 0; i < message.GetOptionsCount(); i++)
         {
            objMessageToDispatch.Options.Add(message.GetOption(i));
         }
         return objMessageToDispatch;
      }

      public static UMSMessage ConvertMessageExToMessage(UMSMessageEx message)
      {
         UMSMessage objMessageToDispatch = new UMSMessage();
         objMessageToDispatch.PacketType = message.PacketType;
         objMessageToDispatch.Message = message.Message;
         objMessageToDispatch.SourceApp = message.SourceApp;
         objMessageToDispatch.SourceHost = message.SourceHost;
         if (!string.IsNullOrEmpty(message.DestinationApp))
         {
            objMessageToDispatch.DestinationApp = message.DestinationApp;
         }
         else
         {
            objMessageToDispatch.DestinationApp = "ALL";
         }
         if (!string.IsNullOrEmpty(message.DestinationHost))
         {
            objMessageToDispatch.DestinationHost = message.DestinationHost;
         }
         else
         {
            objMessageToDispatch.DestinationHost = "ALL";
         }
         if (!string.IsNullOrEmpty(message.PatientID.ToString()))
         {
            objMessageToDispatch.PatientID = Int32.Parse(message.PatientID.ToString());
         }
         foreach (System.Collections.DictionaryEntry entry in message.Options)
         {
            objMessageToDispatch.AddOption(entry.Key.ToString(), entry.Value.ToString());
         }
         return objMessageToDispatch;
      }
   }
}
