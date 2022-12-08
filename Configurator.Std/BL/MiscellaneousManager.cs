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
   public class MiscellaneousManager : DalManagerBase<Miscellanea>, IMiscellaneousManager
   {

      #region Costructors

      public MiscellaneousManager(DigistatDBContext context, ISynchronizationService syncService, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;

         //Before saving (a new) entity, verify that the entity doesn't already exist.
         this.BeforeSave += checkDataHandler;
         
         //Before updating an entity, verify that the entity exists!
         this.BeforeUpdate += checkThatEntityExists;
      }


      private void checkDataHandler(object sender, EventArgs e)
      {

         Miscellanea miscellanea = (Miscellanea)((SaveOrUpdateEventArgs)e).entity;

         var miscellaneousRepository = mobjDbContext.Set<Miscellanea>();

         //Prevent duplications
         Miscellanea loaded = miscellaneousRepository.SingleOrDefault(x => x.Key == miscellanea.Key);
         if (loaded != null)
         {
            if (loaded.Key == miscellanea.Key)
            {
               throw new Exception(string.Format("Unable to create Miscellanea {0}; Key already exists.", miscellanea.Key));
            }

         }
      }


      private void checkThatEntityExists(object sender, EventArgs e) {

         Miscellanea m = (Miscellanea)((SaveOrUpdateEventArgs)e).entity;
         var repo = mobjDbContext.Set<Miscellanea>();
         
         Miscellanea loaded = repo.SingleOrDefault(x => x.Key == m.Key);
         if (loaded == null || loaded.Key != m.Key)
            throw new Exception(string.Format("Unable to find Miscellanea with Key [{0}].", m.Key));
      
      }


      #endregion

      #region Data reading functions

      public String GetValue(string key)
      {

         var miscellanea = this.Get(key);
         if (miscellanea == null) return null;

         return miscellanea.Value;
      }

      public Miscellanea Get(string key)
      {
        
         Miscellanea result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Miscellanea> repository = mobjDbContext.Set<Miscellanea>();

            
            //I don't use SingleOrDefault because sometimes values are duplicated, probably because of some bugs in any application (Configurator???)
            //So I need to use FirstOrDefault
            result = repository.Where(x => x.Key == key).FirstOrDefault();

            
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Miscellanea with key {0} from DB", key);
            throw new Exception(string.Format("Error reading Miscellanea with key {0} from DB", key), e);
         }

         return result;

      }


      public Miscellanea GetFromID(int id)
      {
         Miscellanea result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Miscellanea> repository = mobjDbContext.Set<Miscellanea>();


            //I don't use SingleOrDefault because sometimes values are duplicated, probably because of some bugs in any application (Configurator???)
            //So I need to use FirstOrDefault
            result = repository.Where(x => x.Id == id).FirstOrDefault();


         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Miscellanea with ID {0} from DB", id);
            throw new Exception(string.Format("Error reading Miscellanea with ID {0} from DB", id), e);
         }

         return result;

      }


      #endregion

      #region Data Writing functions


      public new Miscellanea Update(Miscellanea m)
      {
         try
         {
            var repo = mobjDbContext.Set<Miscellanea>();
            //Check if an entry already exists
            
            repo.Update(m);
            mobjDbContext.SaveChanges();
            
            
         }
         catch(Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error updating Miscellanea with ID {0} from DB", m.Id);
            throw new Exception(string.Format("Error updating Miscellanea with ID {0} from DB", m.Id), e);
         }
         return m;
      }


      public new Miscellanea Create(Miscellanea m)
      {
         try
         {
            var repo = mobjDbContext.Set<Miscellanea>();
            if (repo.Where(p => p.Key == m.Key).Count() == 0)
            {
               repo.Add(m);
               mobjDbContext.SaveChanges();
            }
            else
            {
               throw new Exception(string.Format("Unable to create Miscellanea {0}; Key already exists.", m.Key));
            }
            
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error inserting new  Miscellaneous element", m.Id);
            throw new Exception(string.Format("Error inserting new  Miscellaneous element", m.Id), e);
         }
         return m;
      }


      public void Delete(int id)
      {
         try
         {

            var repo = mobjDbContext.Set<Miscellanea>();
            var tmpObj = repo.Where(p => p.Id == id).FirstOrDefault();
            if (tmpObj != null)
            {
               repo.Remove(tmpObj);
               mobjDbContext.SaveChanges();
            }

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error deleting   Miscellaneous element", id);
            throw new Exception(string.Format("Error deleting   Miscellaneous element", id), e);
         }

      }

      public void Remove(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Deleting Miscellanea with id {0} and version {1}", id);


         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var userRepository = mobjDbContext.Set<Miscellanea>();

            Miscellanea loaded = userRepository.SingleOrDefault(x => x.Id == id);
            if (loaded == null)
            {
               throw new Exception(string.Format("Unable to update Miscellanea with id {0}; Miscellanea not found.", id));
            }


            userRepository.Remove(loaded);

            //mobjDbContext.SaveChanges();
            if(executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Miscellanea with id {0} and key {1} succesfully removed", loaded.Id, loaded.Key);

         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error disablig miscellaneous with id {0}", id);
            string message = string.Format("Error disablig miscellaneous with id {0}", id);
            throw new Exception(message, e);
         }
      }

      #endregion
   }
}
