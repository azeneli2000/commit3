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
   public class PersonnelManager : DalManagerBase<Personnel>, IPersonnelManager
   {

      #region Costructors

      public PersonnelManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;

         this.BeforeSave += checkDataHandler;

      }

      private void checkDataHandler(object sender, EventArgs e)
      {

         Personnel personnel = (Personnel)((SaveOrUpdateEventArgs)e).entity;
         if(personnel.Version == 0) 
            personnel.Version = 1;

         var personnelRepository = mobjDbContext.Set<Personnel>();

         //Prevent duplications
         Personnel loadedPersonnel = personnelRepository.SingleOrDefault(x => x.Name == personnel.Name || x.Code == personnel.Code);
         if (loadedPersonnel != null)
         {
            if (loadedPersonnel.Name == personnel.Name)
            {
               throw new Exception(string.Format("Unable to create peciaorsonnel {0}; personnel name already exists.", personnel.Name));
            }

            if (loadedPersonnel.Code == personnel.Code)
            {
               throw new Exception(string.Format("Unable to crate personnel {0}; personnel abbreviation {1} already exists.", personnel.Name, personnel.Code));
            }

            if (personnel.Version != loadedPersonnel.Version)
            {
               throw new Exception(string.Format("Unable to update personnel with id {0}; personnel version ({1}) is different from expected ({2}).", personnel.Id, loadedPersonnel.Version, personnel.Version));
            }
         }

         

         //Set current set as record
         personnel.Current = true;
      }

      #endregion

      #region Data reading functions

      //public SystemOption Get(string guid, IEnumerable<System.Linq.Expressions.Expression<Func<Personnel, object>>> includePredicates = null) {
      public Personnel Get(string id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for Personnel with id {0}", id);

         Personnel result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Personnel> repository = mobjDbContext.Set<Personnel>();

            //if (includePredicates != null && includePredicates.Count() > 0) {
            //   includePredicates.ToList().ForEach(x => repository = repository.Include(x));
            //}

            //TODO Trace
            mobjLoggerService.Info("Reading Personnel with id {0} from DB", id);
            result = repository.Where(x => x.Id == id && x.Current == true).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("Personnel with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Personnel with guid {0} from DB", id);
            throw new Exception(string.Format("Error reading Personnel with guid {0} from DB", id), e);
         }
         

         return result;

      }
        
      #endregion

      #region Data Writing functions

      //public new Personnel Create(Personnel personnel)
      //{

      //   //TODO Trace
      //   mobjLoggerService.Info("Creating new Personnel {0} ({1})", personnel.Name, personnel.Code);

      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var personnelRepository = mobjDbContext.Set<Personnel>();

      //      //Prevent duplications
      //      Personnel loadedPersonnel = personnelRepository.SingleOrDefault(x => x.Name == personnel.Name || x.Code == personnel.Code);
      //      if (loadedPersonnel != null)
      //      {
      //         if (loadedPersonnel.Name == personnel.Name)
      //         {
      //            throw new Exception(string.Format("Unable to create personnel {0}; personnel name already exists.", personnel.Name));
      //         }

      //         if (loadedPersonnel.Code == personnel.Code)
      //         {
      //            throw new Exception(string.Format("Unable to crate personnel {0}; personnel abbreviation {1} already exists.", personnel.Name, personnel.Code));
      //         }
      //      }

      //      if (personnel.Version != loadedPersonnel.Version)
      //      {
      //         throw new Exception(string.Format("Unable to update personnel with id {0}; personnel version ({1}) is different from expected ({2}).", personnel.Id, loadedPersonnel.Version, personnel.Version));
      //      }

      //      //Set current set as record
      //      personnel.Version = 1;
      //      personnel.Current = true;

      //      //Create new record
      //      personnelRepository.Add(personnel);

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      //TODO Trace
      //      mobjLoggerService.Info("Personnel with {0} succesfully created with id {1}", personnel.Name, personnel.Id);

      //      return personnel;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, "Error creating personnel {0}", personnel.Name);
      //      string message = string.Format("Error creating personnel {0}", personnel.Name);
      //      throw new Exception(message, e);
      //   }
         
      //}

      public new Personnel Update(Personnel personnel)
      {

         //TODO Trace
         mobjLoggerService.Info("Updating Personnel with id {0} and version {1}", personnel.Id, personnel.Version);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var personnelRepository = mobjDbContext.Set<Personnel>();

            Personnel loadedPersonnel = personnelRepository.SingleOrDefault(x => x.Id == personnel.Id && x.Current == true);
            if (loadedPersonnel == null)
            {
               throw new Exception(string.Format("Unable to update personnel with id {0}; personnel not found.", personnel.Id));
            }
            if (personnel.Version != loadedPersonnel.Version)
            {
               throw new Exception(string.Format("Unable to update personnel with id {0}; personnel version ({1}) is different from expected ({2}).", personnel.Id, loadedPersonnel.Version, personnel.Version));
            }

            //Create new record for updated entity
            Personnel newPersonnel = personnel.CreateUpdatedClone();
            personnelRepository.Add(newPersonnel);

            //Set current record as updated
            personnel.Current = false;

            mobjDbContext.SaveChanges();
            if (executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Personnel with id {0} updated succesfully", personnel.Id);

            return newPersonnel;

         }
         catch (Exception e)
         {
            if(executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error updating personnel with id {0}", personnel.Id);
            string message = string.Format("Error updating personnel with id {0}", personnel.Id);
            throw new Exception(message, e);
         }
         
      }

      /// <summary>
      /// Disable a personnel. 
      /// </summary>
      public void Remove(string personnelId)
      {

         //TODO Trace
         mobjLoggerService.Info("Disabling Personnel with id {0}", personnelId);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {            
            var personnelRepository = mobjDbContext.Set<Personnel>();

            Personnel personnel = personnelRepository.SingleOrDefault(x => x.Id == personnelId && x.Current);
            if (personnel == null)
            {
               throw new Exception(string.Format("Unable to disable personnel with id {0}; personnel not found.", personnelId));
            }

            //Create new record for updated entity
            Personnel newPersonnel = personnel.CreateUpdatedClone();
            newPersonnel.Enabled = false;
            newPersonnel.ValidToDate = null;
            personnelRepository.Add(newPersonnel);

            //Set current record as updated
            personnel.Current = false;
            personnel.ValidToDate = DateTime.Now;

            mobjDbContext.SaveChanges();
            if (executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Personnel with id {0} disabled succesfully", personnelId);
         }
         catch (Exception e)
         {
            if(executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error disabling personnel with id {0}", personnelId);
            string message = string.Format("Error disabling personnel with id {0}", personnelId);
            throw new Exception(message, e);
         }
         
      }

      #endregion

   }
}
