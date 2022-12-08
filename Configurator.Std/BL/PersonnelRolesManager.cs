using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;

namespace Configurator.Std.BL
{
   public class PersonnelRolesManager : DalManagerBase<PersonnelRole>, IPersonnelRolesManager
   {

      #region Costructors

      public PersonnelRolesManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;

         this.BeforeSave += checkDataHandler;
      }

      private void checkDataHandler(object sender, EventArgs e)
      {

         PersonnelRoleLink entity = (PersonnelRoleLink)((SaveOrUpdateEventArgs)e).entity;

         var repository = mobjDbContext.Set<PersonnelRoleLink>();

         //Prevent duplications
         PersonnelRoleLink loadedEntity = repository.SingleOrDefault(x => x.RoleGUID == entity.RoleGUID && x.PersonnelGUID == entity.PersonnelGUID);
         if (loadedEntity != null)
         {
            throw new Exception(string.Format("Unable to crate personnel role links for role {0} and personnel {1}; relation already exists.", entity.RoleGUID, entity.PersonnelGUID));
         }
      }

      #endregion

      #region Data reading functions

      //public PersonnelRole Get(string guid, IEnumerable<System.Linq.Expressions.Expression<Func<PersonnelRole, object>>> includePredicates = null) {
      public PersonnelRole Get(string guid)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for PersonnelRole with id {0}", guid);

         PersonnelRole result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<PersonnelRole> repository = mobjDbContext.Set<PersonnelRole>();

            //if (includePredicates != null && includePredicates.Count() > 0) {
            //   includePredicates.ToList().ForEach(x => repository = repository.Include(x));
            //}

            //TODO Trace
            mobjLoggerService.Info("Reading PersonnelRole with id {0} from DB", guid);
            result = repository.Where(x => x.Guid == guid && x.Current == true).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("PersonnelRole with id {0} retrived from DB", guid);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading PersonnelRole with guid {0} from DB", guid);
            throw new Exception(string.Format("Error reading Personnel role with guid {0} from DB", guid), e);
         }
         

         return result;

      }
      

      #endregion

      #region Data Writing functions

      public new PersonnelRole Create(PersonnelRole entity)
      {

         //TODO Trace
         mobjLoggerService.Info("Creating new PersonnelRole {0} ({1})", entity.Name, entity.Code);

         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<PersonnelRole>();

            //Prevent duplications
            PersonnelRole loadedEntity = repository.SingleOrDefault(x => x.Name == entity.Name || x.Code == entity.Code);
            if (loadedEntity != null)
            {
               if (loadedEntity.Name == entity.Name)
               {
                  throw new Exception(string.Format("Unable to create personnel role {0}; personnel role name already exists.", entity.Name));
               }

               if (loadedEntity.Code == entity.Code)
               {
                  throw new Exception(string.Format("Unable to crate personnel role {0}; personnel role code {1} already exists.", entity.Name, entity.Code));
               }
               if (entity.Version != loadedEntity.Version)
               {
                  throw new Exception(string.Format("Unable to update personnel role with id {0}; personnel role version ({1}) is different from expected ({2}).", entity.Guid, loadedEntity.Version, entity.Version));
               }
            }

            

            //Set current set as record
            entity.Current = true;
            if(entity.Version<=0) entity.Version = 1;

            //Create new record
            repository.Add(entity);

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("PersonnelRole with {0} succesfully created with id {1}", entity.Name, entity.Guid);

            return entity;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error creating personnel role {0}", entity.Name);
            string message = string.Format("Error creating personnel role {0}", entity.Name);
            throw new Exception(message, e);
         }
         
      }

      public new PersonnelRole Update(PersonnelRole entity)
      {

         //TODO Trace
         mobjLoggerService.Info("Updating PersonnelRole with id {0} and version {1}", entity.Guid, entity.Version);

         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<PersonnelRole>();

            PersonnelRole loadedEntity = repository.SingleOrDefault(x => x.Guid == entity.Guid && x.Current == true);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update personnel with id {0}; personnel not found.", entity.Guid));
            }
            if (entity.Version != loadedEntity.Version)
            {
               throw new Exception(string.Format("Unable to update personnel with id {0}; personnel version ({1}) is different from expected ({2}).", entity.Guid, loadedEntity.Version, entity.Version));
            }

            //Create new record for updated entity
            PersonnelRole newEntity = entity.CreateUpdatedClone();
            repository.Add(newEntity);

            //Set current record as updated
            entity.Current = false;

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Personnel with id {0} updated succesfully", entity.Guid);

            return newEntity;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error updating personnel with id {0}", entity.Guid);
            string message = string.Format("Error updating personnel with id {0}", entity.Guid);
            throw new Exception(message, e);
         }
         
      }

      ///// <summary>
      ///// Disable a personnel role. 
      ///// </summary>
      //public void Remove(string guid)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Disabling PersonnelRole with id {0}", guid);

      //   var mobjDbContext = new DigistatDBContext(mobjDigConfig.ConnectionString);

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var repository = mobjDbContext.Set<PersonnelRole>();

      //      PersonnelRole personnel = repository.SingleOrDefault(x => x.Guid == guid && x.Current);
      //      if (personnel == null)
      //      {
      //         throw new Exception(string.Format("Unable to disable personnel role with id {0}; personnel not found.", guid));
      //      }

      //      //Create new record for updated entity
      //      PersonnelRole newEntity = personnel.CreateUpdatedClone();
      //      newEntity.Enabled = false;
      //      newEntity.ValidToDate = null;
      //      repository.Add(newEntity);

      //      //Set current record as updated
      //      personnel.Current = false;
      //      personnel.ValidToDate = DateTime.Now.DatetimeUMSDBToString();

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();
      //      //TODO Trace
      //      mobjLoggerService.Info("PersonnelRole with id {0} disabled succesfully", guid);
      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException( e, "Error disabling personnel role with id {0}", guid);
      //      string message = string.Format("Error disabling personnel role with id {0}", guid);
      //      throw new Exception(message, e);
      //   }
      //   finally
      //   {
      //      disposeContext();
      //   }
      //}

      #endregion
   }
}
