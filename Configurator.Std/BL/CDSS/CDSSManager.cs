using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Vitals;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using Configurator.Std.BL.CDSS;
using Configurator.Std.BL.Configurator;
using Configurator.Std.BL.DasDrivers;
using Digistat.FrameworkStd.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using Configurator.Std.BL.Hubs;
using Configurator.Std.BL.Mobile;
using Digistat.FrameworkStd.Model.CDSS;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics;
using Digistat.FrameworkStd.MessageCenter;
using Digistat.FrameworkStd.UMSLegacy;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
using EventLogEntryType = Digistat.FrameworkStd.Enums.EventLogEntryType;

namespace Configurator.Std.BL.Vitals
{
  

   public class CDSSManager : DalManagerBase<CDSSRule>, ICDSSManager,IDisposable
   {

      #region Costructors

      //private readonly ILoggerService mobjLoggerService;
      private readonly IDigistatEnvironmentService mobjEnvService;
      protected readonly IMessageCenterManager mobjMsgCtrMgr;
      //private readonly DigistatDBContext mobjDbContext;
      private readonly IConfiguratorWebConfiguration mobjDigConfig;
      private readonly IMessageCenterService mobjMsgCtrSvc;
      public  string CDSS_ID = "driver00-CDSS-read-only-000000000000";
      private readonly string mstrCDSS_HARDWARE_ID ="ASCOM CDSS SERVER";
      
      private List<SyncToken> _tokens = new List<SyncToken>();
      private string cdssListSeparator = "__";
      private static ILogger _logger;

      public CDSSManager(DigistatDBContext context, ILoggerService loggerService,IDigistatEnvironmentService envSvc
         , IMessageCenterManager msgCtrMgr
         , IMessageCenterService msgCtrSvc
      , IConfiguratorWebConfiguration digConfig
         , ILogger<CDSSManager> logger)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjEnvService = envSvc;
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDigConfig = digConfig;
         mobjMsgCtrSvc = msgCtrSvc;
         _logger = logger;
      }

      #endregion

      
      /// <summary>
      /// Returns the list of all rules
      /// </summary>
      /// <returns></returns>
      public List<CDSSRule> GetAll(bool validOnly = false)
      {
         List<CDSSRule> objRet = null;
         try
         {
            
            IQueryable<CDSSRule> repository = mobjDbContext.Set<CDSSRule>();


            objRet = repository.Where(x => (validOnly? x.IsTest == false : x.IsTest ==false || x.IsTest== true)).ToList();
            //objRet = repository.Take(1).ToList();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading CDSS Rules from DB");
            throw new Exception("Error reading CDSS Rules ", e);
         }
         return objRet;
      }


      /// <summary>
      /// Return a rule given its ID
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public Digistat.FrameworkStd.Model.CDSS.CDSSRule Get(int id)
      {
         Digistat.FrameworkStd.Model.CDSS.CDSSRule objRet = null;
         try
         {

            IQueryable<Digistat.FrameworkStd.Model.CDSS.CDSSRule> repository = mobjDbContext.Set<Digistat.FrameworkStd.Model.CDSS.CDSSRule>();
            objRet = repository.AsNoTracking()
               .Include(x => x.RuleLocationLinks)
               .Include(x => x.RuleOptions)
               .Include(x => x.RuleOutputParameters)
               .FirstOrDefault(p => p.Id == id );
            //if (objRet != null && objRet.RuleOptions.Count > 0 &&
            //    objRet.RuleOptions.Count(o => o.PatientID != 0) > 0)
            //{
            //   var x = objRet.RuleOptions.Where(o => o.PatientID != 0).ToList();
            //   foreach (var ro in x)
            //   {
            //      objRet.RuleOptions.Remove(ro);
            //   }
            //}
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading CDSS Rule {id} from DB");
            throw new Exception($"Error reading CDSS Rule {id}", e);
         }
         return objRet;
      }


      public new Digistat.FrameworkStd.Model.CDSS.CDSSRule Create( Digistat.FrameworkStd.Model.CDSS.CDSSRule entity)
      {

         try
         {
            mobjDbContext.BeginTransaction();

            var ruleRepo = mobjDbContext.Set<CDSSRule>();

            if (entity.IsTest && String.IsNullOrWhiteSpace(entity.Name))
               entity.Name = "test";

            ruleRepo.Add(entity);

            mobjDbContext.SaveChanges();
            if (!entity.IsTest)
            {
               mobjMsgCtrMgr.SendCDSSCreated(entity);
            }
            

            mobjDbContext.CommitTransaction();


            return entity;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error creating CDSS RULE {entity.Id}");
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }
      }

      public CDSSRule Delete(CDSSRule entity)
      {
         try
         {
            mobjDbContext.BeginTransaction();

            var ruleRepo = mobjDbContext.Set<CDSSRule>();

            var objOldEntity = ruleRepo.Where(p => p.Id == entity.Id).FirstOrDefault();
            if (objOldEntity != null)
            {
               var ruleOptions=mobjDbContext.Set<CDSSRuleOption>();
               var objOptions = ruleOptions.Where(ro=>ro.RuleID ==entity.Id);
               var ruleOutputParameter=mobjDbContext.Set<CDSSRuleOutputParameter>();//.Where(ro=>ro.RuleID ==entity.Id);
               var objOutputParameter = ruleOutputParameter.Where(ro=>ro.RuleID ==entity.Id);
               
               mobjDbContext.RemoveRange(objOptions);   
               mobjDbContext.RemoveRange(objOutputParameter);

               mobjDbContext.Entry(objOldEntity).State = EntityState.Detached;

               mobjDbContext.Entry(entity).State = EntityState.Deleted;

               mobjDbContext.SaveChanges();

               if (!entity.IsTest)
               {
                  //Send notification to Message Center
                  mobjMsgCtrMgr.SendCDSSDeleted(entity);
               }
               

               mobjDbContext.CommitTransaction();
            }

            

            return entity;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error deleting CDSS RULE {entity.Id}");
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }
      }
      [Obsolete]
      public Tuple<bool, IEnumerable<string> > Compile(CDSSRule rule)
      {
         var retT = new Tuple<bool,IEnumerable<string> >(false,null);

         try
         {
            retT = Digistat.FrameworkStd.Helpers.CDSSHelper.TestCompile(rule);
         }
         catch (Exception e)
         {
            retT = new Tuple<bool,IEnumerable<string> >(false,new List<string>(){e.Message});
         }
         
         return retT;
      }



      public ICollection<DriverRepositoryStandardParameterLink> GetAllCapabilities()
      {
         ICollection<DriverRepositoryStandardParameterLink> objRet = null;
         try
         {
            string strCdssNotExists = "-no-";
            string idCdss = mobjDbContext.Set<DriverRepository>().FirstOrDefault(x => x.Current && x.HardwareRelease == mstrCDSS_HARDWARE_ID)?.Id??"-no-";
            if (idCdss==strCdssNotExists)
            {
               mobjLoggerService.Error("CDSS Driver not installed");
               throw new Exception("CDSS Driver not installed");
            }
            IQueryable<DriverRepositoryStandardParameterLink> repository = mobjDbContext.Set<DriverRepositoryStandardParameterLink>();
            objRet = repository.AsNoTracking().Where(p => p.DriverRepositoryId == idCdss).ToList();
         }
         catch (Exception e)
         {
            //mobjLoggerService.ErrorException(e, $"Error reading CDSS Rule {id} from DB");
            if (e.Message=="CDSS Driver not installed")
            {
               throw e;
            }
            throw new Exception($"Error reading Capabilities", e);
         }
         return objRet;
      }

      public bool SaveCapabilitiesAndSendMessage(ICollection<DriverRepositoryStandardParameterLink> capabilities)
      {
         try
         {
            mobjDbContext.BeginTransaction();
            //mobjDbContext.Set<DriverRepositoryStandardParameterLink>().AddRange(capabilities);
            string strCdssNotExists = "-no-";
            string idCdss = mobjDbContext.Set<DriverRepository>().FirstOrDefault(x => x.Current && x.HardwareRelease == mstrCDSS_HARDWARE_ID)?.Id??strCdssNotExists;
            if (idCdss==strCdssNotExists)
            {
               mobjLoggerService.Error("CDSS Driver not installed");
               throw new Exception("CDSS Driver not installed");
            }
            IQueryable<DriverRepositoryStandardParameterLink> repository = mobjDbContext.Set<DriverRepositoryStandardParameterLink>();//.AddRange(capabilities);
            var db = repository.Where(p => p.DriverRepositoryId == idCdss).ToList();
            mobjDbContext.RemoveRange(db);
            mobjDbContext.SaveChanges();
            //throw new Exception("r");

            db.Clear();
            foreach (DriverRepositoryStandardParameterLink cap in capabilities)
            {
               cap.DriverRepositoryId = idCdss;
            }
            db.AddRange(capabilities);

            mobjDbContext.AddRange(db);
            mobjDbContext.SaveChanges();
            //throw new Exception("r");
            mobjDbContext.CommitTransaction();
            
            mobjMsgCtrMgr.SendDriverEdited("CDSS Driver", idCdss, false, false, true, false);
            return true;
         }
         catch
         {
            mobjDbContext.RollbackTransaction();
            return false;
         }
         
      }

    

      public int CheckDllRuleMethod(string fileName,string ruleMethod, int iD)
      {
         int iRet = 0;
         
         try
         {
            var dbRule=  mobjDbContext.Set<CDSSRule>().FirstOrDefault(x => x.DllFile==fileName.Trim() && x.DllRuleName==ruleMethod.Trim() && x.Id!= iD);
            iRet = dbRule?.Id ?? 0;
         }
         //Pain and Fear!!!!
         catch
         {
            
         }
         return iRet;
      }
      public Digistat.FrameworkStd.Model.CDSS.CDSSRule UpdateCustom(CDSSRule entity, bool clearAll=false)
      {

         try
         {

            mobjDbContext.BeginTransaction();
            
            var ruleRepo = mobjDbContext.Set<CDSSRule>();

            var objOldEntity = ruleRepo.Where(p => p.Id == entity.Id)
               .Include(x =>x.RuleOutputParameters)
               .Include(x =>x.RuleOptions)
               .Include(x =>x.RuleLocationLinks)
               .FirstOrDefault();
               //
               
            if (objOldEntity != null)
            {

               
               mobjDbContext.Entry(objOldEntity).Entity.RuleOutputParameters.Clear();

               foreach (CDSSRuleOutputParameter op in entity.RuleOutputParameters)
               {
                  var e1= objOldEntity.RuleOutputParameters.FirstOrDefault(a => a.ParameterName == op.ParameterName);
                  if (e1 == null)
                  {
                     objOldEntity.RuleOutputParameters.Add(op);
                  }
                  
               }
               mobjDbContext.Entry(objOldEntity).Entity.RuleLocationLinks.Clear();
               foreach (CDSSRuleLocationLink rll in entity.RuleLocationLinks)
               {
                  var e1= objOldEntity.RuleLocationLinks.FirstOrDefault(a => a.IDLocation == rll.IDLocation);
                  if (e1 == null)
                  {
                     objOldEntity.RuleLocationLinks.Add(rll);
                  }
                  
               }

               if (clearAll)
               {
                  mobjDbContext.Entry(objOldEntity).Entity.RuleOptions.Clear();
               }
               else
               {
                  var patientRuleOptions =
                     mobjDbContext.Entry(objOldEntity).Entity.RuleOptions.Where(r => r.PatientID != 0).ToList();
                  mobjDbContext.Entry(objOldEntity).Entity.RuleOptions.Clear();
                  foreach (CDSSRuleOption patientRuleOption in patientRuleOptions)
                  {
                     entity.RuleOptions.Add(patientRuleOption);
                     //mobjDbContext.Entry(objOldEntity).Entity.RuleOptions.Remove(patientRuleOption);
                  }
               }
               

               foreach (CDSSRuleOption op in entity.RuleOptions)
               {
                  var e1= objOldEntity.RuleOptions.FirstOrDefault(a => a.Name == op.Name);
                  if (e1 == null)
                  {
                     objOldEntity.RuleOptions.Add(op);
                  }
                  
               }
               mobjDbContext.Entry(objOldEntity).CurrentValues.SetValues(entity);

               ruleRepo.Update(objOldEntity);

               mobjDbContext.SaveChanges();

               //Send notification to Message Center
               mobjMsgCtrMgr.SendCDSSEdited(entity);

               mobjDbContext.CommitTransaction();
            }


            return entity;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error updating CDSS RULE {entity.Id}");
            string message = string.Format(e.Message);
            throw new Exception(message, e);
         }
      }
      /// <summary>
      /// Returns the list of all rules
      /// </summary>
      /// <returns></returns>
      public List<CDSSRuleOutputParameter> GetAllOutputParamters(bool validOnly = true)
      {
         List<CDSSRuleOutputParameter> objRet = null;
         try
         {
            IQueryable<CDSSRule> rule = mobjDbContext.Set<CDSSRule>();
            var rules = rule.Where(x=>(validOnly ? x.IsTest == false : x.IsTest == false || x.IsTest == true)).Select(x => x.Id);
            IQueryable<CDSSRuleOutputParameter> repository = mobjDbContext.Set<CDSSRuleOutputParameter>();
            objRet = repository.Where(x =>  rules.Contains(x.RuleID) ).OrderBy(x=>x.ParameterName ).ToList();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading CDSS Rules from DB");
            throw new Exception("Error reading CDSS Rules ", e);
         }
         return objRet;
      }
      [Obsolete("deprecated by CancelToken problem. Use GetDllListByMessageSync")]
      public async Task<CDSSAnswer> GetDllListByMessage(string tokenId)
      {
         int timeoutms = 30000;
         using (var mgr = new AsyncCDSSDispatcher(mobjMsgCtrSvc ,mobjDigConfig, mobjLoggerService,timeoutms))
         {
            
            try
            {
               var data = await mgr.GetDllList(tokenId);
               return data;
            }
            catch (Exception e)
            {
               _logger.LogDebug("************ CDSS MANAGER :{0}\r\n\t{1}",DateTime.Now.ToString("HH:mm:ss.fff"),e);
               //_logger.LogError(e,e.Message);
               throw;
            }
         }
      }
      public CDSSAnswer GetDllListByMessageSync(string tokenId)
      {
         int timeoutMs = 30000;
         
         mobjMsgCtrSvc.OnMessageReceived += OnUMSMessageReceived;

         //}
         CDSSAnswer data = null;
         try
         {
            var messageType = UMSFrameworkParser.GetMessageCDSSGetDllFiles();
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
               
               

               objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
               objMessage.SourceApp = mobjDigConfig.ModuleName;
               objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

               objMessage.Message = messageType;
               
               _tokens.Add(new SyncToken(){Token = tokenId});
               var s = DateTime.Now.ToString("HH:mm:ss.fff");
               mobjMsgCtrSvc.SendMessage(objMessage);
               _logger.LogDebug("--->>>----- {2}{0} sent - token[{1}]", messageType, tokenId, s);

               data = WaitCdssAnswer(tokenId, timeoutMs);
            }
            catch (Exception e)
            {
            
               _logger.LogError(e,"--ERROR-------- GetDllList :{0}",DateTime.Now.ToString("HH:mm:ss.fff"));
               throw;
            }
            
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
            throw;
         }

         return data;
      }

      
      [Obsolete("deprecated by CancelToken problem. Use GetDllMethodListByMessageSync")]
      public async Task<CDSSAnswer> GetDllMethodListByMessage(string strDllFileName,string tokenId)
      {
         int timeoutms = 30000;
         using (var mgr = new AsyncCDSSDispatcher(mobjMsgCtrSvc ,mobjDigConfig, mobjLoggerService,timeoutms))
         {
            
            try
            {
               var data = await mgr.GetDllMethodList( strDllFileName,tokenId);
               return data;
            }
            catch (Exception e)
            {
               _logger.LogError(e,e.Message);
               throw;
            }
         }
      }


      public CDSSAnswer GetDllMethodListByMessageSync(string strDllFileName, string tokenId)
      {
         int timeoutMs = 30000;
         
         mobjMsgCtrSvc.OnMessageReceived += OnUMSMessageReceived;

         //}
         CDSSAnswer data = null;
         try
         {
            var messageType = UMSFrameworkParser.GetMessageCDSSGetDllMethodFiles();
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("DLLFILENAME", strDllFileName);
            objMessage.AddOption("TOKENID", tokenId);
            _tokens.Add(new SyncToken(){Token = tokenId});
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            var s = DateTime.Now.ToString("HH:mm:ss.fff");

            mobjMsgCtrSvc.SendMessage(objMessage);
            _logger.LogDebug("--->>>------- {2}{0} sent - token[{1}]", messageType, tokenId, s);

            data = WaitCdssAnswer(tokenId, timeoutMs);
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
         }
         return data;
      }

      [Obsolete("deprecated by CancelToken problem. Use GetDllOutputParamsByMessageSync")]
      public async Task<CDSSAnswer> GetDllOutputParamsByMessage(string strRuleName,string strDllFileName,string tokenId)
      {
         int timeoutms = 15000;
         using (var mgr = new AsyncCDSSDispatcher(mobjMsgCtrSvc ,mobjDigConfig, mobjLoggerService,timeoutms))
         {
            
            try
            {
               var data = await mgr.GetDllOutputParametersList( strRuleName, strDllFileName,tokenId);
               return data;
            }
            catch (Exception e)
            {
               _logger.LogError(e,e.Message);
               throw;
            }
         }
      }
      
      public CDSSAnswer GetDllOutputParamsByMessageSync(string strRuleName,string strDllFileName, string tokenId)
      {
         int timeoutMs = 15000;
         
         mobjMsgCtrSvc.OnMessageReceived += OnUMSMessageReceived;

         //}
         CDSSAnswer data = null;
         try
         {
            var messageType = UMSFrameworkParser.GetMessageCDSSGetRuleOutputParameters();
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("NAME", strRuleName);
            objMessage.AddOption("DLLFILENAME", strDllFileName);
            objMessage.AddOption("TOKENID", tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;
            
            _tokens.Add(new SyncToken(){Token = tokenId});
            _logger.LogDebug("--->>>----- {2}{0} sending - token[{1}]", messageType, tokenId, DateTime.Now.ToString("HH:mm:ss.fff"));
            mobjMsgCtrSvc.SendMessage(objMessage);
            data = WaitCdssAnswer(tokenId, timeoutMs);
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
         }
         return data;
      }
      [Obsolete("deprecated by CancelToken problem. Use GetDllListByMessageSync")]
      public async Task<CDSSAnswer> CompileByMessage(int id,string tokenId)
      {
         
         int timeoutms = 30000;
         using (var mgr = new AsyncCDSSDispatcher(mobjMsgCtrSvc ,mobjDigConfig, mobjLoggerService,timeoutms))
         {
            
            try
            {
               var data = await mgr.Compile(id,tokenId);
               return data;
            }
            catch (Exception e)
            {
               _logger.LogError(e,e.Message);
               throw;
            }
         }


      }
      
      public CDSSAnswer CompileByMessageSync(int id, string tokenId)
      {
         int timeoutMs = 30000;
         
         mobjMsgCtrSvc.OnMessageReceived += OnUMSMessageReceived;

         //}
         CDSSAnswer data = null;
         try
         {
            var messageType = UMSFrameworkParser.GetMessageCDSSCompileTest();
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("ID", id.ToString());
            objMessage.AddOption("TOKENID", tokenId);
            //objMessage.AddOption("INPUTVALUES", testInputValues);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;
            
            _tokens.Add(new SyncToken(){Token = tokenId});
            _logger.LogDebug("--->>>----- {2}{0} sending - token[{1}]", messageType, tokenId, DateTime.Now.ToString("HH:mm:ss.fff"));
            mobjMsgCtrSvc.SendMessage(objMessage);
            data = WaitCdssAnswer(tokenId, timeoutMs);
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
         }
         return data;
      }
      [Obsolete("deprecated by CancelToken problem. Use GetDllListByMessageSync")]
      public async Task<CDSSAnswer> RunningTestByMessage(int id, string inputValues,string tokenId)
      {
         int timeoutms = 60000;
         using (var mgr = new AsyncCDSSDispatcher(mobjMsgCtrSvc ,mobjDigConfig, mobjLoggerService,timeoutms))
         {
            
            try
            {
               var data = await mgr.CompileAndRun(id,inputValues,tokenId);
               return data;
            }
            catch (Exception e)
            {
               _logger.LogError(e,e.Message);
               throw;
            }
         }
      }
      
      public CDSSAnswer RunningTestByMessageSync(int id, string inputValues, string tokenId)
      {
         int timeoutMs = 60000;
         
         mobjMsgCtrSvc.OnMessageReceived += OnUMSMessageReceived;

         //}
         CDSSAnswer data = null;
         try
         {
            var messageType = UMSFrameworkParser.GetMessageCDSSCompileAndRunTest();
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };


            objMessage.AddOption("ID", id.ToString());
            objMessage.AddOption("INPUTVALUES", inputValues);
            
            objMessage.AddOption("TOKENID", tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;
            
            _tokens.Add(new SyncToken(){Token = tokenId});
            _logger.LogDebug("--->>>----- {2}{0} sending - token[{1}]", messageType, tokenId, DateTime.Now.ToString("HH:mm:ss.fff"));
            mobjMsgCtrSvc.SendMessage(objMessage);
            data = WaitCdssAnswer(tokenId, timeoutMs);
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
         }
         return data;
      }
      [Obsolete("deprecated by CancelToken problem. Use GetDllSettingsByMessageSync")]
      public async Task<CDSSAnswer> GetDllSettingsByMessage(string ruleMetodh, string dllName, string tokenId)
      {
         int timeoutms = 15000;
         using (var mgr = new AsyncCDSSDispatcher(mobjMsgCtrSvc ,mobjDigConfig, mobjLoggerService,timeoutms))
         {
            
            try
            {
               var data = await mgr.GetDllSettingsList( ruleMetodh, dllName, tokenId);
               return data;
            }
            catch (Exception e)
            {
               _logger.LogError(e,e.Message);
               throw;
            }
         }
      }
      
      public CDSSAnswer GetDllSettingsByMessageSync(string ruleMetodh, string dllName, string tokenId)
      {
         int timeoutMs = 15000;
         
         mobjMsgCtrSvc.OnMessageReceived += OnUMSMessageReceived;

         //}
         CDSSAnswer data = null;
         try
         {
            var messageType = UMSFrameworkParser.GetMessageCDSSGetDllSettingsRequest();
            MCMessage objMessage = new MCMessage
            {
               // construct the message
               DestinationHost = DestinationHostCodes.All.GetDisplayAttribute(),
               DestinationApp = ApplicationCodes.All.GetDisplayAttribute()
            };

            objMessage.AddOption("NAME", ruleMetodh);
            objMessage.AddOption("DLLFILENAME", dllName);
            objMessage.AddOption("TOKENID", tokenId);
            objMessage.SourceHost = UMSFrameworkParser.GetWorkstationName().ToUpper();
            objMessage.SourceApp = mobjDigConfig.ModuleName;
            objMessage.PacketType = UMSFrameworkParser.GetPacketTypeRemoteCommand();

            objMessage.Message = messageType;

            _tokens.Add(new SyncToken(){Token = tokenId});
            _logger.LogDebug("--->>>----- {2}{0} sending - token[{1}]", messageType, tokenId, DateTime.Now.ToString("HH:mm:ss.fff"));
            mobjMsgCtrSvc.SendMessage(objMessage);
            data = WaitCdssAnswer(tokenId, timeoutMs);
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
         }
         return data;
      }
      private CDSSAnswer WaitCdssAnswer(string tokenId, int timeoutMs)
      {
         CDSSAnswer data=null;
         bool exitCondition = false;
         bool exit4timeout = false;
         Stopwatch sw = new Stopwatch();
         SyncToken token = null;
         sw.Start();

         while (!exitCondition)
         {
            System.Threading.Thread.Sleep(200);
            token = _tokens.Find(f => f.Token == tokenId);
            if (token != null)
            {
               if (token.Completed)
               {
                  data = token.Answer;
                  exitCondition = true;
               }
               else
               {
                  //we wait
               }
            }
            else
            {
               exitCondition = true;
            }

            if (!exitCondition)
            {
               exitCondition = sw.ElapsedMilliseconds >= timeoutMs;
               exit4timeout = exitCondition;
            }
         }

         sw.Stop();

         _logger.LogDebug("---------- token [{1}]:{0}", sw.Elapsed,tokenId);
         if (token != null && !token.Completed && !exit4timeout)
         {
            System.Threading.Thread.Sleep(1000);
         }

         if (token != null && !token.Completed)
         {
            throw new TaskCanceledException("The server did not respond in time. Please try again");
         }
         else if (token == null)
         {
            throw new TaskCanceledException("The server did not respond. Please try again");
         }
         else
         {
            try
            {
               _tokens.Remove(token);
            }
            catch (Exception e)
            {
               _logger.LogError(e,e.Message);
            }
         }

         return data;
      }
      protected  void OnUMSMessageReceived(object sender,MCMessage msg)
      {
      
         try
         {
            _logger.LogDebug("---<<<----- {2} {0} [{1}]",msg.Message.ToString(),msg.ToString(),DateTime.Now.ToString("HH:mm:ss.fff"));
         }
         catch (Exception) { }
         
         switch (msg.Message)
         {
            case UMSMessageExtendedCodes.messageTypeRemoteCommandCDSSResponse:
               var token = msg.GetSafeOptionValueAsString("TOKENID");
               SyncToken uToken = _tokens.Find(f=>f.Token == token);;
               if (uToken == null)
               {
                  //Console.WriteLine();
                  _logger.LogDebug("---------- token:{0}",token);
                  return;
               }
               //else
               //{
               //   //_token.Remove(token);

               //   //this.mobjCancel.Dispose();
               //}
               var data = msg.Options.Find((opt) => opt.Key.ToString().ToUpper().Equals("RESULTS"));
                
               if (data.Value != null) {                  
                  //Build object
                  CDSSAnswer objCdss = new CDSSAnswer();
                  objCdss.success = msg.GetSafeOptionValueAsBoolean("SUCCESS");
                  var rawDllList = msg.GetSafeOptionValueAsString("RESULTS");
                  
                  objCdss.messagges = new List<string>();
                  if (!objCdss.success)
                  {
                     
                  }
                  if (rawDllList.Length>0)
                  {
                     objCdss.messagges = rawDllList.Split(new string[] {cdssListSeparator},StringSplitOptions.RemoveEmptyEntries).ToList();
                  }
                  var rawErrorList = msg.GetSafeOptionValueAsString("ERRORS");
                  
                  if (!objCdss.success && rawErrorList.Length>0)
                  {
                     objCdss.messagges = rawErrorList.Split(new string[] {cdssListSeparator},StringSplitOptions.RemoveEmptyEntries).ToList();
                  }

                  uToken.Answer = objCdss;

                  uToken.Completed = true;
                  _logger.LogDebug("--removed- token:{0}", token);
               }
               break;
            //case "CDSS_ALIVE":
            //   break;
            default:
               _logger.LogDebug("---------- message: {0}",msg.Message);
               break;

         }
      }

      public void Dispose()
      {
         try
         {
            mobjMsgCtrSvc.OnMessageReceived -= OnUMSMessageReceived;
         }
         catch (Exception e)
         {
            _logger.LogError(e,e.Message);
         }
         

      }
   }
}
