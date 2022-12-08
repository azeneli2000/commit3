using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using System.Linq.Expressions;

namespace Configurator.Std.BL
{
   public class PersonnelRoleLinksManager : DalManagerBase<PersonnelRoleLink>, IPersonnelRoleLinksManager
   {

      #region Costructors

      public PersonnelRoleLinksManager(DigistatDBContext context, ILoggerService loggerService)
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
      public IEnumerable<PersonnelRoleLink> GetByPersonnel(string personnelGuid, bool loadRole = true, bool loadPersonnel = false)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing GetByPersonnel with id {0}", personnelGuid);

         IEnumerable<PersonnelRoleLink> result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<PersonnelRoleLink> repository = mobjDbContext.Set<PersonnelRoleLink>();

            //TODO Trace
            mobjLoggerService.Info("Reading PersonnelRoleLinks for personnel with id {0} from DB", personnelGuid);
            result = repository.Where(x => x.PersonnelGUID == personnelGuid).ToList();

            if (loadRole || loadPersonnel)
            {
               foreach (PersonnelRoleLink personnelRoleLink in result)
               {
                  if (loadRole)
                  {
                     personnelRoleLink.Role = mobjDbContext.Set<PersonnelRole>().Single(x => x.Guid == personnelRoleLink.RoleGUID && x.Current);
                  }

                  if (loadPersonnel)
                  {
                     personnelRoleLink.Personnel = mobjDbContext.Set<Personnel>().Single(x => x.Id == personnelRoleLink.PersonnelGUID && x.Current && x.Enabled);
                  }
               }
            }

            //TODO Trace
            mobjLoggerService.Info("PersonnelRoleLinks for personnel with id {0} retrived from DB", personnelGuid);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading PersonnelRoleLinks for personnel with guid {0} from DB", personnelGuid);
            throw new Exception(string.Format("Error reading PersonnelRoleLinks for personnel with guid {0} from DB", personnelGuid), e);
         }
         

         return result;

      }

      public IEnumerable<PersonnelRoleLink> GetByRole(string personnelRoleGuid, bool loadPersonnel = true, bool loadRole = false)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing GetByRole with id {0}", personnelRoleGuid);

         IEnumerable<PersonnelRoleLink> result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<PersonnelRoleLink> repository = mobjDbContext.Set<PersonnelRoleLink>();

            //TODO Trace
            mobjLoggerService.Info("Reading PersonnelRoleLinks for personnel role with id {0} from DB", personnelRoleGuid);
            result = repository.Where(x => x.RoleGUID == personnelRoleGuid).ToList();

            if (loadRole || loadPersonnel)
            {
               foreach (PersonnelRoleLink personnelRoleLink in result)
               {
                  if (loadRole)
                  {
                     personnelRoleLink.Role = mobjDbContext.Set<PersonnelRole>().Single(x => x.Guid == personnelRoleLink.RoleGUID && x.Current);
                  }

                  if (loadPersonnel)
                  {
                     personnelRoleLink.Personnel = mobjDbContext.Set<Personnel>().Single(x => x.Id == personnelRoleLink.PersonnelGUID && x.Current && x.Enabled);
                  }
               }
            }

            //TODO Trace
            mobjLoggerService.Info("PersonnelRoleLinks for personnel role with id {0} retrived from DB", personnelRoleGuid);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading PersonnelRoleLinks for personnel role with guid {0} from DB", personnelRoleGuid);
            throw new Exception(string.Format("Error reading PersonnelRoleLinks for personnel role with guid {0} from DB", personnelRoleGuid), e);
         }
         

         return result;

      }


      public new IEnumerable<PersonnelRoleLink> Find(Expression<Func<PersonnelRoleLink, bool>> filterPredicate, IEnumerable<Expression<Func<PersonnelRoleLink, object>>> includePredicates = null, int pageNumber = 0, int pageSize = 0)
      {
         throw new NotImplementedException("Find method with includepredicate is not available for Personnel role links, use loadPersonnel and loadRole");
      }

      //Using lambda expressions filters have some limitations when queries are complicated, but for simple projects are very usefull
      public IEnumerable<PersonnelRoleLink> Find(System.Linq.Expressions.Expression<Func<PersonnelRoleLink, bool>> filterPredicate,
                              bool loadPersonnel = true,
                              bool loadRole = false,
                              int pageNumber = 0,
                              int pageSize = 0)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing PersonnelRoleLink Find with page number {0} and page size {1} {2}", pageNumber, pageSize, filterPredicate == null ? "" : "using filter predicates");

         List<PersonnelRoleLink> result;

         try
         {
            IQueryable<PersonnelRoleLink> qResult = mobjDbContext.Set<PersonnelRoleLink>();

            qResult = qResult.Where(filterPredicate);

            if (pageNumber > 0)
            {
               qResult = qResult.Skip((pageNumber - 1) * pageSize);
            }

            if (pageSize > 0)
            {
               qResult = qResult.Take(pageSize);
            }

            result = qResult.ToList();


            if (loadRole || loadPersonnel)
            {
               foreach (PersonnelRoleLink personnelRoleLink in result)
               {
                  if (loadRole)
                  {
                     personnelRoleLink.Role = mobjDbContext.Set<PersonnelRole>().Single(x => x.Guid == personnelRoleLink.RoleGUID && x.Current);
                  }

                  if (loadPersonnel)
                  {
                     personnelRoleLink.Personnel = mobjDbContext.Set<Personnel>().Single(x => x.Id == personnelRoleLink.PersonnelGUID && x.Current && x.Enabled);
                  }
               }
            }


            //TODO Trace
            mobjLoggerService.Info("Page {0} ({1} items per page) of PersonnelRoleLinks find results list ({2}) retrived succesfully.", pageNumber, pageSize, filterPredicate == null ? "" : "using filter predicates");
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to search PersonnelRoleLinks");
            string message = string.Format("Unable to search Personnel role links");
            throw new Exception(message, e);
         }
         return result;
      }

      #endregion

      #region Data Writing functions

      //public PersonnelRoleLink Create(PersonnelRoleLink entity)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Creating new PersonnelRoleLink Role {0} Personnel {1}", entity.RoleGUID, entity.PersonnelGUID);

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var repository = mobjDbContext.Set<PersonnelRoleLink>();

      //      //Prevent duplications
      //      PersonnelRoleLink loadedEntity = repository.SingleOrDefault(x => x.RoleGUID == entity.RoleGUID && x.PersonnelGUID == entity.PersonnelGUID);
      //      if (loadedEntity != null)
      //      {
      //         throw new Exception(string.Format("Unable to crate personnel role links for role {0} and personnel {1}; relation already exists.", entity.RoleGUID, entity.PersonnelGUID));
      //      }

      //      //Create new record
      //      repository.Add(entity);

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      //TODO Trace
      //      mobjLoggerService.Info("PersonnelRoleLink with role {0} and personnel {1} succesfully created", entity.RoleGUID, entity.PersonnelGUID);

      //      return entity;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, "Error creating personnel role link  with role {0} and personnel {1}", entity.RoleGUID, entity.PersonnelGUID);
      //      string message = string.Format("Error creating personnel role link  with role {0} and personnel {1}", entity.RoleGUID, entity.PersonnelGUID);
      //      throw new Exception(message, e);
      //   }
         
      //}

      //public PersonnelRoleLink Update(PersonnelRoleLink entity)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Updating PersonnelRoleLink Role {0} Personnel {1}", entity.RoleGUID, entity.PersonnelGUID);

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var repository = mobjDbContext.Set<PersonnelRoleLink>();

      //      PersonnelRoleLink loadedEntity = repository.SingleOrDefault(x => x.RoleGUID == entity.RoleGUID && x.PersonnelGUID == entity.PersonnelGUID);
      //      if (loadedEntity == null)
      //      {
      //         throw new Exception(string.Format("Unable to update personnel role links for role {0} and personnel {1}; relation not found.", entity.RoleGUID, entity.PersonnelGUID));
      //      }


      //      loadedEntity.Primary = entity.Primary;
      //      loadedEntity.Index = entity.Index;

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      //TODO Trace
      //      mobjLoggerService.Info("PersonnelRoleLink with role {0} and personnel {1} succesfully updated", entity.RoleGUID, entity.PersonnelGUID);

      //      return loadedEntity;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, "Error updating personnel role link with role {0} and personnel {1}", entity.RoleGUID, entity.PersonnelGUID);
      //      string message = string.Format("Error updating personnel role link with role {0} and personnel {1}", entity.RoleGUID, entity.PersonnelGUID);
      //      throw new Exception(message, e);
      //   }
         
      //}

      /// <summary>
      /// Disable a personnel role. 
      /// </summary>
      public void Remove(PersonnelRoleLink entity)
      {
         //TODO Trace
         mobjLoggerService.Info("Removing PersonnelRoleLink Role {0} Personnel {1}", entity.RoleGUID, entity.PersonnelGUID);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var repository = mobjDbContext.Set<PersonnelRoleLink>();

            PersonnelRoleLink loadedEntity = repository.SingleOrDefault(x => x.RoleGUID == entity.RoleGUID && x.PersonnelGUID == entity.PersonnelGUID);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to remove personnel role links for role {0} and personnel {1}; relation not found.", entity.RoleGUID, entity.PersonnelGUID));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            if(executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("PersonnelRoleLink with role {0} and personnel {1} succesfully removed", entity.RoleGUID, entity.PersonnelGUID);
         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();

            mobjLoggerService.ErrorException(e, "Error removing personnel role link with role {0} and personnel {1}", entity.RoleGUID, entity.PersonnelGUID);
            string message = string.Format("Error removing personnel role link with role {0} and personnel {1}", entity.RoleGUID, entity.PersonnelGUID);
            throw new Exception(message, e);
         }
         
      }

      #endregion
      
   }
}
