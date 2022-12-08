using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Std.BL.CDSS;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.UMSLegacy;
   
using Configurator.Std.BL.Configurator;
using Digistat.FrameworkStd.MessageCenter;
using Microsoft.Extensions.Logging;

namespace Configurator.Std.BL.DasDrivers
{
   public class AsyncCDSSDispatcher : AsyncDispatcher<CDSSAnswer>
   {
      private readonly IConfiguratorWebConfiguration mobjDigConfig;
      private List<String> _token = new List<string>();

      private CDSSAnswer instances = new CDSSAnswer();

      public AsyncCDSSDispatcher(IMessageCenterService msgCtrSvc
         , IConfiguratorWebConfiguration digConfig
         , ILoggerService logSvc
         , int timeoutms = 5000) : base(msgCtrSvc,logSvc,null, timeoutms)
      {
         mobjDigConfig = digConfig;
      }

      public async Task<CDSSAnswer> GetDllList(string tokenId)
      {
         var response = new TaskCompletionSource<CDSSAnswer>();
         response.TrySetResult(instances);

         var messageType = UMSFrameworkParser.GetMessageCDSSGetDllFiles();
         CDSSAnswer ret;
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("ID", "TEST");
            objMessage.AddOption("TOKENID", tokenId);
            _token.Add(tokenId);
            //objMessage.AddOption("INPUTVALUES", testInputValues);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            //Console.WriteLine("************ AsyncCDSSDispatcher :{0}",DateTime.Now.ToString("HH:mm:ss.fff"));
            var s = DateTime.Now.ToString("HH:mm:ss.fff");
            Send(objMessage);
            
            Console.WriteLine("---------- {2}{0} sended - token[{1}]",messageType,tokenId,s);
            
            ret  = await WaitForResults();
         }
         catch
         {
            
            Console.WriteLine("---------- GetDllList :{0}",DateTime.Now.ToString("HH:mm:ss.fff"));
            //mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
         return ret;
      }
      public async Task<CDSSAnswer> GetDllSettingsList(string ruleMetodh, string dllName, string tokenId)
      {
         var response = new TaskCompletionSource<CDSSAnswer>();
         response.TrySetResult(instances);
         //CDSS_GET_DLL_RULE 
         var messageType = UMSFrameworkParser.GetMessageCDSSGetDllSettingsRequest();
         CDSSAnswer ret;
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            objMessage.AddOption("NAME", ruleMetodh);
            objMessage.AddOption("DLLFILENAME", dllName);
            objMessage.AddOption("TOKENID", tokenId);
            _token.Add(tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;
            Console.WriteLine("---------- {0} ",objMessage.ToString());
            Send(objMessage);
            Console.WriteLine("---------- {0} sended",messageType);
            ret  = await WaitForResults();
         }
         catch
         {

            //mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }
         return ret;
      }
      public async Task<CDSSAnswer> GetDllOutputParametersList(string strRuleName,string strDllFileName,string tokenId)
      {
         var response = new TaskCompletionSource<CDSSAnswer>();
         response.TrySetResult(instances);

         var messageType = UMSFrameworkParser.GetMessageCDSSGetRuleOutputParameters();
         CDSSAnswer ret;
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("NAME", strRuleName);
            objMessage.AddOption("DLLFILENAME", strDllFileName);
            objMessage.AddOption("TOKENID", tokenId);
            _token.Add(tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            Send(objMessage);
            Console.WriteLine("---------- {0} sended - token[{1}]",messageType,tokenId);
            ret  = await WaitForResults();
         }
         catch
         {
            //mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }

         
         return ret;
      }
      public async Task<CDSSAnswer> GetDllMethodList(string strDllFileName,string tokenId)
      {
         var response = new TaskCompletionSource<CDSSAnswer>();
         response.TrySetResult(instances);

         var messageType = UMSFrameworkParser.GetMessageCDSSGetDllMethodFiles();
         CDSSAnswer ret;
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("DLLFILENAME", strDllFileName);
            objMessage.AddOption("TOKENID", tokenId);
            _token.Add(tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            Send(objMessage);
            Console.WriteLine("---------- {0} sended - token[{1}]",messageType,tokenId);
            ret  = await WaitForResults();
         }
         catch
         {
            //mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            throw;
         }

         
         return ret;
      }
      
      public async Task<CDSSAnswer> Compile(int id,string tokenId)
      {
         var response = new TaskCompletionSource<CDSSAnswer>();
         response.TrySetResult(instances);

         var messageType = UMSFrameworkParser.GetMessageCDSSCompileTest();
         CDSSAnswer ret;
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("ID", id.ToString());
            objMessage.AddOption("TOKENID", tokenId);
            _token.Add(tokenId);
            //objMessage.AddOption("INPUTVALUES", testInputValues);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            Send(objMessage);
            Console.WriteLine("---------- {0} sended - token[{1}]",messageType,tokenId);
            ret  = await WaitForResults();
         }
         catch (Exception e)
         {
            //mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            Console.WriteLine("########{0}",e.Message);
            throw;
         }

         
         return ret;
      }
      
      public async Task<CDSSAnswer> CompileAndRun(int id, string inputValues,string tokenId)
      {
         var response = new TaskCompletionSource<CDSSAnswer>();
         response.TrySetResult(instances);

         var messageType = UMSFrameworkParser.GetMessageCDSSCompileAndRunTest();
         CDSSAnswer ret;
         try
         {
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("ID", id.ToString());
            objMessage.AddOption("INPUTVALUES", inputValues);
            
            objMessage.AddOption("TOKENID", tokenId);
            _token.Add(tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            Send(objMessage);
            Console.WriteLine("---------- {0} sended - token[{1}]",messageType,tokenId);
            ret  = await WaitForResults();
         }
         catch (Exception e)
         {
            //mobjLoggerService.ErrorException(e, "Error sending {0} message", messageType);
            Console.WriteLine("########{0}",e.Message);
            throw;
         }

         
         return ret;
      }
      protected override void OnUMSMessageReceived(MCMessage msg)
      {
         try
         {
            Console.WriteLine("---------- {2} {0} [{1}]",msg.Message.ToString(),msg.ToString(),DateTime.Now.ToString("HH:mm:ss.fff"));
         }
         catch (Exception) { }
         
         switch (msg.Message)
         {
            case UMSMessageExtendedCodes.messageTypeRemoteCommandCDSSResponse:
               var token = msg.GetSafeOptionValueAsString("TOKENID");
               if (!_token.Contains(token))
               {
                  Console.WriteLine("---------- token:{0}",token);
                  return;
               }
               else
               {
                  _token.Remove(token);
               }
               var data = msg.Options.Find((opt) => opt.Key.ToString().ToUpper().Equals("RESULTS"));
                
               if (data.Value != null) {                  
                  //Build object
                  CDSSAnswer objCdss = new CDSSAnswer();
                  objCdss.success = msg.GetSafeOptionValueAsBoolean("SUCCESS");
                  var rawDllList = msg.GetSafeOptionValueAsString("RESULTS");
                  
                  objCdss.messagges = new List<string>();
                  if (rawDllList.Length>0)
                  {
                     objCdss.messagges = rawDllList.Split(new string[]{"§"},StringSplitOptions.RemoveEmptyEntries).ToList();
                  }
                  var rawErrorList = msg.GetSafeOptionValueAsString("ERRORS");
                  
                  if (!objCdss.success && rawErrorList.Length>0)
                  {
                     objCdss.messagges = rawErrorList.Split(Convert.ToChar("§")).ToList();
                  }

                  Notify(objCdss);
               }
               break;
            //case "CDSS_ALIVE":
            //   break;
            default:
               Console.WriteLine("---------- message: {0}",msg.Message);
               break;

         }
      }

      
   }
}
