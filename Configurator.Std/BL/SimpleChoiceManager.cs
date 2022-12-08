using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Configurator.Std.BL
{
   public class SimpleChoiceManager : DalManagerBase<SimpleChoiceManager>, ISimpleChoiceManager
   {

      #region Costructors      
      private readonly IMessageCenterManager mobjMsgCtrMgr;

      public SimpleChoiceManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      #endregion

      #region Data reading functions

      public Dictionary<string, int> GetAllGroup(bool noTracking = true)
      {
         //mobjLoggerService.Info("Executing Get for SimpleChoice with guid {0}");

         Dictionary<string,int> result = null;

         try
         {
            //TODO Trace
            //mobjLoggerService.Info("Read SystemOption with guid {0} from DB");
            if (noTracking)
            {


               result = mobjDbContext.Set<SimpleChoice>().AsNoTracking().GroupBy(g => g.Group).Select(s => new
               {

                  Group = s.Key,
                  NumOfChoises = s.Count()
               }).ToDictionary(arg => arg.Group, arg => arg.NumOfChoises);
            }
            else
            {
               result = mobjDbContext.Set<SimpleChoice>().GroupBy(g => g.Group).Select(s => new
               {

                  Group = s.Key,
                  NumOfChoises = s.Count()
               }).ToDictionary(arg => arg.Group, arg => arg.NumOfChoises);
            }
            //TODO Trace
            //mobjLoggerService.Info("SystemOption with guid {0} retrived from DB", );

         }
         catch (Exception e)
         {

            mobjLoggerService.ErrorException(e, "Error reading SimpleChoice from DB");
            throw new Exception(string.Format("Error reading Simple Choice from DB"), e);
         }

         return result;
      }

      public IEnumerable<SimpleChoice> GetAll(bool noTracking = true)
      {  IEnumerable<SimpleChoice> result = null;
         try
         {
            //TODO Trace
            //mobjLoggerService.Info("Read SystemOption with guid {0} from DB");
            if (noTracking)
            {


               result = mobjDbContext.Set<SimpleChoice>().AsNoTracking().ToList();
            }
            else
            {
               result = mobjDbContext.Set<SimpleChoice>().ToList();
            }
            //TODO Trace
            //mobjLoggerService.Info("SystemOption with guid {0} retrived from DB", );

         }
         catch (Exception e)
         {

            mobjLoggerService.ErrorException(e, "Error reading SimpleChoice from DB");
            throw new Exception(string.Format("Error reading Simple Choice from DB"), e);
         }

         return result;
      }

      public IEnumerable<SimpleChoice> GetGroupChoises(string group)
      {
         IEnumerable<SimpleChoice> result = null;

         try
         {
            //TODO Trace
            //mobjLoggerService.Info("Read SystemOption with guid {0} from DB");

            result = mobjDbContext.Set<SimpleChoice>().AsNoTracking().Where(w=>w.Group==group).OrderBy(o=>o.Index).AsEnumerable();
            
            //TODO Trace
            //mobjLoggerService.Info("SystemOption with guid {0} retrived from DB", );

         }
         catch (Exception e)
         {

            mobjLoggerService.ErrorException(e, "Error reading SimpleChoice from DB");
            throw new Exception(string.Format("Error reading Simple Choice from DB"), e);
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
      /// Update an existing SimpleChoice group 
      /// </summary>
      public bool UpdateGroup(List<SimpleChoice> choises)
      {
         if (choises == null)
         {
            throw new NullReferenceException("choises could not be null");
         }
         if (choises[0] == null)
         {
            throw new NullReferenceException("choises Group could not be null");
         }
         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var repository = mobjDbContext.Set<SimpleChoice>();
            var onDb = repository.AsNoTracking().Where(p => p.Group == choises[0].Group).ToList();
            int intCount = onDb.Count();
            
            if (intCount != 0)
            {
               foreach (SimpleChoice s in onDb)
               {
                  if (choises.FindIndex(f=>f.ID == s.ID)<0)
                  {
                     repository.Remove(s);
                  }
               }
               foreach (SimpleChoice s in choises)
               {
                  
                  repository.Update(s);
               }

               mobjDbContext.SaveChanges();
            }
            else
            {
               //throw new ConnectException(mobjDicSvc.XLate($"Driver {driverRepository.DriverName} with version {driverRepository.DriverVersion} already exists")); ;
            }

           
            if (executeClose) mobjDbContext.CommitTransaction();

            return true;

         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            //if (e is ConnectException)
            //{
            //   throw;
            //}
            mobjLoggerService.ErrorException(e, "Error updating Simple choice with {0}", choises[0].ID);
            string message = string.Format("Error updating Simple choice with {0}", choises[0].ID);
            throw new Exception(message, e);
         }
      }


      public SimpleChoice CreateGroup(SimpleChoice option)
      {
         //TODO Trace
         //mobjLoggerService.Info("Creating new {1} SystemOption  with value {2} for application {0}", option.Application, option.Name, option.Value);
         if (option == null)
         {
            throw new NullReferenceException("Simple choise could not be null");
         }
         if (option.Group == null)
         {
            throw new NullReferenceException("Simple choise Group could not be null");
         }
         try
         {

            mobjDbContext.BeginTransaction();

            var sysOptRepo = mobjDbContext.Set<SimpleChoice>();

            //Prevent duplications
            SimpleChoice loadedEntity = sysOptRepo.SingleOrDefault(x => x.Group == option.Group
               //&& x.Name == option.Name
               //&& x.HostName == option.HostName
               //&& x.UserAbbreviation == option.UserAbbreviation
            );
            if (loadedEntity != null)
            {
               //throw new Exception(string.Format("Unable to crate system option {0} for application {1}. System option with same attributes already exists.", option.Name, option.Application));
            }

            //Set current set as record
            //option.Guid = Guid.NewGuid().ToString();

            //Create new record
            sysOptRepo.Add(option);

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            //mobjMsgCtrMgr.SendSystemOptionEdited(option);

            mobjDbContext.CommitTransaction();

            //TODO Trace
            //mobjLoggerService.Info("System option {0} for application {1} succesfully created with id {2}", option.Name, option.Application, option.Guid);

            return option;

         }
         catch (Exception)
         {
            mobjDbContext.RollbackTransaction();

            //mobjLoggerService.ErrorException(e, "Error creating system option {0} for application {1}", option.Name, option.Application);

            //string message = string.Format("Error creating system option {0} for application {1} ", option.Name, option.Application);
            throw;
         }

      }
      /*
      public new SystemOption Update(SystemOption option)
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

            //TODO Trace
            mobjLoggerService.Info("System option with id {0} updated succesfully", option.Guid);

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
      */
      public bool DeleteGroup(string groupId)
      {
 
         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<SimpleChoice>();

            List<SimpleChoice> loadedEntity = repository.Where(x => x.Group == groupId).ToList();
            if (loadedEntity.Count==0)
            {
               throw new Exception(string.Format("Unable to delete Simple choices with group {0};", groupId));
            }

            repository.RemoveRange(loadedEntity);

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();


            //Send notification to Message Center
            //mobjMsgCtrMgr.SendSystemOptionEdited(loadedEntity);

            //TODO Trace
            mobjLoggerService.Info("Simple choices with group {0} removed succesfully", loadedEntity[0].Group);
            return true;
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing Simple choices with group {0}", groupId);
            string message = string.Format("Error removing Simple choices with group {0}", groupId);
            throw new Exception(message, e);
         }

      }

      #endregion

      //public SimpleChoiceManager Create(SimpleChoiceManager entity)
      //{
      //   return _simpleChoiceManagerImplementation.Create(entity);
      //}

      //public SimpleChoiceManager Update(SimpleChoiceManager entity)
      //{
      //   return _simpleChoiceManagerImplementation.Update(entity);
      //}

      //public IEnumerable<SimpleChoiceManager> ExecuteQuery(string sql, object[] parameters)
      //{
      //   return _simpleChoiceManagerImplementation.ExecuteQuery(sql, parameters);
      //}

      //public bool Exists(Expression<Func<SimpleChoiceManager, bool>> filterPredicate)
      //{
      //   return _simpleChoiceManagerImplementation.Exists(filterPredicate);
      //}

      //public IEnumerable<SimpleChoiceManager> Find(Expression<Func<SimpleChoiceManager, bool>> filterPredicate, IEnumerable<Expression<Func<SimpleChoiceManager, object>>> includePredicates = null, int pageNumber = 0,
      //   int pageSize = 0)
      //{
      //   return _simpleChoiceManagerImplementation.Find(filterPredicate, includePredicates, pageNumber, pageSize);
      //}

      //public IEnumerable<SimpleChoiceManager> GetAll(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<SimpleChoiceManager, object>>> includePredicates = null)
      //{
      //   return _simpleChoiceManagerImplementation.GetAll(pageNumber, pageSize, includePredicates);
      //}

      //public IQueryable<SimpleChoiceManager> GetQueryable(IEnumerable<Expression<Func<SimpleChoiceManager, object>>> includePredicates = null)
      //{
      //   return _simpleChoiceManagerImplementation.GetQueryable(includePredicates);
      //}


   }
}
