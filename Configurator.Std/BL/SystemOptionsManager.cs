using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL
{
   public class SystemOptionsManager : DalManagerBase<SystemOption>, ISystemOptionsManager
   {

      #region Costructors      
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly ISystemOptionsService mobjSysOptSvc;
      private readonly IDigistatConfiguration mobjDigCfg;

      public SystemOptionsManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService
         ,ISystemOptionsService sysOptSvc,IDigistatConfiguration digCfg)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjSysOptSvc = sysOptSvc;
         mobjDigCfg = digCfg;
      }

      #endregion

      #region Data reading functions

      //public SystemOption Get(string guid, IEnumerable<System.Linq.Expressions.Expression<Func<SystemOption, object>>> includePredicates = null) {
      public SystemOption Get(string guid)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for SystemOption with guid {0}", guid);

         SystemOption result = null;

         try
         {
            //TODO Trace
            mobjLoggerService.Info("Read SystemOption with guid {0} from DB", guid);

            result = GetSystemOptions().Where(x=> x.Guid == guid).SingleOrDefault();
            
            //TODO Trace
            mobjLoggerService.Info("SystemOption with guid {0} retrived from DB", guid);

         }
         catch (Exception e)
         {

            mobjLoggerService.ErrorException(e, "Error reading SystemOption with guid {0} from DB", guid);
            throw new Exception(string.Format("Error reading System Option with guid {0} from DB", guid), e);
         }

         return result;
      }

      public List<SystemOption> GetByApplicationName(string applicationName)
      {

         //TODO Trace
         mobjLoggerService.Info("Getting SystemOptions for application {0}", applicationName);

         List<SystemOption> result;

         //Set the results as detached from mobjDbContext
         //mobjDbContext.Configuration.ProxyCreationEnabled = false;

         try
         {

            //If application is not set on DB the value is null
            if (string.IsNullOrWhiteSpace(applicationName)) applicationName = null;

            result = mobjDbContext.Set<SystemOption>().Where(x => x.Application == applicationName).ToList();

            //TODO Trace
            mobjLoggerService.Info("{0} SystemOptions found for application {1}", result.Count, applicationName);

         }
         catch (Exception e)
         {

            mobjLoggerService.ErrorException(e, "Unable to retrieve SystemOptions for application {0}", applicationName);

            string message = string.Format("Unable to retrieve System Options for application {0}", applicationName);
            throw new Exception(message, e);
         }
         return result;
      }

      #endregion

      #region Data Writing functions

      /// <summary>
      /// Update an existing systemoption value
      /// </summary>
      public void UpdateValue(string guid, string value)
      {

         //TODO Trace
         mobjLoggerService.Info("Updating SystemOption {0} value, setting: {1}", guid, value);

         try
         {

            mobjDbContext.BeginTransaction();

            SystemOption systemOption = mobjDbContext.Set<SystemOption>().SingleOrDefault(x => x.Guid == guid);
            if (systemOption == null)
            {
               throw new Exception(string.Format("Unable to update value for system option {0}; system option not found.", guid));
            }

            //Update 
            systemOption.Value = value;

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendSystemOptionEdited(systemOption);

            mobjDbContext.CommitTransaction();
            //TODO Trace
            mobjLoggerService.Info("{0} System Option updated succesfully", guid);

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();

            mobjLoggerService.ErrorException(e, "Error updating SystemOption {0} value", guid);
            string message = string.Format("Error updating system option {0} value", guid);
            throw new Exception(message, e);
         }

      }

      public SystemOption Create(SystemOption option, string usrAbbrev, string strHostname)
      {
         //TODO Trace
         mobjLoggerService.Info("Creating new {1} SystemOption  with value {2} for application {0}", option.Application, option.Name, option.Value);

         try
         {

            mobjDbContext.BeginTransaction();

            var sysOptRepo = mobjDbContext.Set<SystemOption>();

            //Prevent duplications
            SystemOption loadedEntity = sysOptRepo.SingleOrDefault(x => x.Application == option.Application
               && x.Name == option.Name
               && x.HostName == option.HostName
               && x.UserAbbreviation == option.UserAbbreviation
            );
            if (loadedEntity != null)
            {
               throw new Exception(string.Format("Unable to crate system option {0} for application {1}. System option with same attributes already exists.", option.Name, option.Application));
            }

            //Set current set as record
            option.Guid = Guid.NewGuid().ToString();

            //Create new record
            sysOptRepo.Add(option);

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendSystemOptionEdited(option);

            mobjDbContext.CommitTransaction();

            //Add an entry as Clinical Log
            mobjLoggerService.Write(100, $"System option with [id] {option.Guid} created with [Value] {option.Value}", EventLogEntryType.Information, usrAbbrev, 0, LogType.CLN, mobjDigCfg.ModuleName,strHostname,"UMS");

            return option;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();

            mobjLoggerService.ErrorException(e, "Error creating system option {0} for application {1}", option.Name, option.Application);

            string message = string.Format("Error creating system option {0} for application {1} ", option.Name, option.Application);
            throw;
         }

      }

      public SystemOption Update(SystemOption option,string usrAbbrev,string strHostname)
      {
         //TODO Trace
         mobjLoggerService.Info("Updating System option with guid {0}", option.Guid);

         try
         {
            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<SystemOption>();

            if (option.Value == null)
               option.Value = string.Empty;

            repository.Update(option);

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendSystemOptionEdited(option);

            mobjDbContext.CommitTransaction();

            //Add an entry as Clinical Log
            mobjLoggerService.Write(100, $"System option with [id] {option.Guid} updated with [Value] {option.Value}", EventLogEntryType.Information, usrAbbrev, 0, LogType.CLN, mobjDigCfg.ModuleName, strHostname, "UMS");

            if (string.IsNullOrEmpty(option.Application) || option.Application==mobjDigCfg.ModuleName || mobjSysOptSvc.CheckIfSystemOptionApplicationIsLoaded(option.Application))
            {
               //this application too should be coinvolved in a change for a general systemoption. Reload all systemoptions.
               mobjSysOptSvc.ReloadSystemOptions(option.Application!=null?option.Application:string.Empty);
            }

            

            return option;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error updating system option with id {0}", option.Guid);
            string message = string.Format("Error updating system option with id {0}", option.Guid);
            throw new Exception(message, e);
         }

      }

      public void Delete(string entityId)
      {
 
      try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<SystemOption>();

            SystemOption loadedEntity = repository.SingleOrDefault(x => x.Guid == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to delete SystemOption with id {0}; network not found.", entityId));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();


            //Send notification to Message Center
            mobjMsgCtrMgr.SendSystemOptionEdited(loadedEntity);

            //TODO Trace
            mobjLoggerService.Info("SystemOption with id {0} removed succesfully", loadedEntity.Guid);

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing SystemOption with id {0}", entityId);
            string message = string.Format("Error removing SystemOption with id {0}", entityId);
            throw new Exception(message, e);
         }

      }

      
      public IQueryable<SystemOption> GetSystemOptions(bool convertAppNameEmptyToNull = false)
      {
         var objSO = mobjDbContext.Set<SystemOption>();
         var objH = mobjDbContext.Set<HospitalUnit>();
         var objU = mobjDbContext.Set<User>();
         var objQ = from l in objSO.AsQueryable()
                    join h in objH.AsQueryable() on l.HospitalUnitGUID equals h.GUID into res
                        from r in res.DefaultIfEmpty()
                        where (r == null || r.Current == true)
                    join u in objU.AsQueryable() on l.UserAbbreviation equals u.Abbrev into resUsr
                    from ru in resUsr.DefaultIfEmpty()
                    where (ru == null || ru.Current == true)
                    select new SystemOption
                    {
                       Application =(convertAppNameEmptyToNull? (string.IsNullOrWhiteSpace(l.Application) ? null : l.Application): l.Application),
                       Description = l.Description,
                       Guid = l.Guid,
                       HospitalUnit = r,
                       HospitalUnitGUID = l.HospitalUnitGUID,
                       HostName = l.HostName,
                       IsSystem = l.IsSystem,
                       Level = l.Level,
                       LowerLimit = l.LowerLimit,
                       Name = l.Name,
                       Type = l.Type,
                       UpperLimit = l.UpperLimit,
                       UserAbbreviation = l.UserAbbreviation,
                       Value = l.Value,
                       User = ru
                    };

         return objQ.OrderBy(o => o.Application).ThenBy(n => n.Name);
      }

      #endregion

   }
}
