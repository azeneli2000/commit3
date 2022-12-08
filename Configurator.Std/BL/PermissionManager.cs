using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;

using Digistat.FrameworkStd.Model;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;



namespace Configurator.Std.BL
{
   public class PermissionsManager : DalManagerBase<Permission>, IPermissionsManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;

      public PermissionsManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;

         this.AfterSave += sendPermissionEditedHandler;
         this.AfterUpdate += sendPermissionEditedHandler;

      }
     
      void sendPermissionEditedHandler (object sender, EventArgs e) {

         Permission entity = (Permission)((SaveOrUpdateEventArgs)e).entity;

         //Send notification to Message Center
         mobjMsgCtrMgr.SendPermissionEdited(entity);

      }

      #endregion

      #region Data reading functions

      //public Permission Get(string guid, IEnumerable<System.Linq.Expressions.Expression<Func<Permission, object>>> includePredicates = null) {
      public Permission Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for Permission with id {0}", id);

         Permission result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Permission> repository = mobjDbContext.Set<Permission>();

            //if (includePredicates != null && includePredicates.Count() > 0) {
            //   includePredicates.ToList().ForEach(x => repository = repository.Include(x));
            //}

            //TODO Trace
            mobjLoggerService.Info("Reading Permission with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("Permission with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Permission with id {0} from DB", id);
            throw new Exception(string.Format("Error reading Permission with id {0} from DB", id), e);
         }


         return result;

      }

      //public IEnumerable<Permission> GetAll(int pageNumber = 0, int pageSize = 0)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Executing GetAll Permissions with page number {0} and page size {1}", pageNumber, pageSize);

      //   List<Permission> result;

      //   try
      //   {
      //      IQueryable<Permission> repository = mobjDbContext.Set<Permission>();

      //      if (pageNumber > 0)
      //      {
      //         repository = repository.Skip((pageNumber - 1) * pageSize);
      //      }

      //      if (pageSize > 0)
      //      {
      //         repository = repository.Take(pageSize);
      //      }

      //      result = repository.ToList();


      //      //TODO Trace
      //      mobjLoggerService.Info("Page {0} ({1} items per page) of all Permissions list retrived succesfully, {2} elements found", pageNumber, pageSize, result.Count);

      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to read all Permissions from DB");
      //      string message = string.Format("Unable to read all permissions from DB");
      //      throw new Exception(message, e);
      //   }


      //   return result;

      //}

      ////Using lambda expressions filters have some limitations when queries are complicated, but for simple projects are very usefull
      //public IEnumerable<Permission> Find(System.Linq.Expressions.Expression<Func<Permission, bool>> filterPredicate,
      //                        IEnumerable<System.Linq.Expressions.Expression<Func<Permission, object>>> includePredicates = null,
      //                        int pageNumber = 0,
      //                        int pageSize = 0)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Executing Permissions Find with page number {0} and page size {1} {2}", pageNumber, pageSize, filterPredicate == null ? "" : "using filter predicates");

      //   List<Permission> result;

      //   try
      //   {
      //      IQueryable<Permission> qResult = mobjDbContext.Set<Permission>();

      //      if (includePredicates != null && includePredicates.Count() > 0)
      //      {
      //         includePredicates.ToList().ForEach(x => qResult = qResult.Include(x));
      //      }

      //      qResult = qResult.Where(filterPredicate);

      //      if (pageNumber > 0)
      //      {
      //         qResult = qResult.Skip((pageNumber - 1) * pageSize);
      //      }

      //      if (pageSize > 0)
      //      {
      //         qResult = qResult.Take(pageSize);
      //      }

      //      result = qResult.ToList();

      //      //TODO Trace
      //      mobjLoggerService.Info("Page {0} ({1} items per page) of Permissions find results list ({2}) retrived succesfully.", pageNumber, pageSize, filterPredicate == null ? "" : "using filter predicates");
      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to search Permissions");
      //      string message = string.Format("Unable to search Permissions");
      //      throw new Exception(message, e);
      //   }
      //   return result;
      //}

      //public bool Exists(System.Linq.Expressions.Expression<Func<Permission, bool>> filterPredicate)
      //{
      //   //TODO Trace
      //   mobjLoggerService.Info("Executing Exists for Permissions {0}", filterPredicate == null ? "" : "using filter predicates");

      //   bool result;

      //   try
      //   {
      //      result = mobjDbContext.Set<Permission>().Any(filterPredicate);
      //      //TODO Trace
      //      mobjLoggerService.Info("Exists for Permission {0} compleated with result {1}", filterPredicate == null ? "" : "using filter predicates", result ? "true" : "false");
      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to execute Permissions exists");
      //      string message = string.Format("Unable to execute Permissions exists");
      //      throw new Exception(message, e);
      //   }
      //   return result;
      //}

      //public IQueryable<Permission> GetQueriable(IEnumerable<System.Linq.Expressions.Expression<Func<Permission, object>>> includePredicates = null)
      //{
      //   //TODO Trace
      //   mobjLoggerService.Info("Getting queriable for Permissions");

      //   IQueryable<Permission> result;

      //   try
      //   {
      //      result = mobjDbContext.Set<Permission>();

      //      if (includePredicates != null && includePredicates.Count() > 0)
      //      {
      //         includePredicates.ToList().ForEach(x => result = result.Include(x));
      //      }

      //      //TODO Trace
      //      mobjLoggerService.Info("Queriable for Permissions retrieved");
      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to retrieve queriable for Permissions entity");
      //      string message = string.Format("Unable to retrieve queriable for Permissions entity");
      //      throw new Exception(message, e);
      //   }
      //   return result;

      //}

      #endregion

      #region Data Writing functions

      //public Permission Create(Permission entity)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Creating new Permission");

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var userRepository = mobjDbContext.Set<Permission>();

      //      //Create new record
      //      userRepository.Add(entity);

      //      mobjDbContext.SaveChanges();

      //      //Send notification to Message Center
      //      mobjMsgCtrMgr.SendPermissionEdited(entity);

      //      mobjDbContext.CommitTransaction();

      //      //TODO Trace
      //      mobjLoggerService.Info("Permission succesfully created with id {1}", entity.Id);

      //      return entity;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, "Error creating new permission");
      //      string message = string.Format("Error creating new permission");
      //      throw new Exception(message, e);
      //   }

      //}

      //public Permission Update(Permission entity)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Updating Permission with id {0}", entity.Id);

      //   var executeClose = mobjDbContext.BeginTransaction();

      //   try
      //   {
      //      var repository = mobjDbContext.Set<Permission>();

      //      //Permission loadedEntity = repository.SingleOrDefault(x => x.Id == entity.Id);
      //      //if (loadedEntity == null)
      //      //{
      //      //   throw new Exception(string.Format("Unable to update permission with id {0}; permission not found.", entity.Id));
      //      //}

      //      repository.Attach(entity);

      //      mobjDbContext.SaveChanges();

      //      //Send notification to Message Center
      //      mobjMsgCtrMgr.SendPermissionEdited(entity);

      //      if (executeClose) { mobjDbContext.CommitTransaction(); }


      //      //TODO Trace
      //      mobjLoggerService.Info("Permission with id {0} updated succesfully", entity.Id);

      //      return entity;

      //   }
      //   catch (Exception e)
      //   {
      //      if (executeClose) { mobjDbContext.RollbackTransaction(); }
      //      mobjLoggerService.ErrorException(e, "Error updating permission with id {0}", entity.Id);
      //      string message = string.Format("Error updating permission with id {0}", entity.Id);
      //      throw new Exception(message, e);
      //   }

      //}

      public void Delete(int entityId)
      {
         //TODO Trace
         mobjLoggerService.Info("Deleting Permission with id {0}", entityId);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            var repository = mobjDbContext.Set<Permission>();

            Permission loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update permission with id {0}; permission not found.", entityId));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendPermissionEdited(loadedEntity);

            if (executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Permission with id {0} removed succesfully", loadedEntity.Id);

         }
         catch (Exception e)
         {
            if(executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing permission with id {0}", entityId);
            string message = string.Format("Error removing permission with id {0}", entityId);
            throw new Exception(message, e);
         }

      }

      #endregion

   }
}
