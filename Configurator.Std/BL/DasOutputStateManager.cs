using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Digistat.FrameworkStd.Interfaces;
using Digistat.Dal.Data;

using Configurator.Std.BL.Configurator;
using Configurator.Std.BL.Hubs;


namespace Configurator.Std.BL
{
   public class DasOutputStateManager : DalManagerBase<DasOutputState>, IDasOutputStateManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IConfiguratorWebConfiguration mobjConfig;
      private readonly ISynchronizationService mobjSyncSvc;

      public DasOutputStateManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, IConfiguratorWebConfiguration digConfig, ISynchronizationService syncSvc, ILoggerService loggerService)
      {

         mobjConfig = digConfig;
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjSyncSvc  = syncSvc;
         mobjLoggerService = loggerService;

         this.BeforeSave += BeforeSaveHandler;
         this.BeforeUpdate += BeforeUpdateHandler;

         this.AfterSave += AfterSaveHandler;
         this.AfterUpdate += AfterUpdateHandler;

      }

      #endregion

      #region private functions

      private void validateData(DasOutputState oState, bool isNewRecord)
      {

         if (!oState.IsSystem)
         {
            throw new ArgumentException("Output state must be marked as system", "IsSystem");
         }

         var repository = mobjDbContext.Set<DasOutputState>();


         //Prevent duplications
         FormattableString sql = $"select dos_IsSystem from DasOutputState where (BedRef = {oState.BedId}) and PatientRef = {oState.PatientId} and LocationRef = {oState.LocationId} and dos_IsSystem = 1";
         bool? existingValue = repository.FromSqlRaw(sql.ToString()).Select(x => x.IsSystem).SingleOrDefault();

         if (isNewRecord)
         {
            if (existingValue.HasValue && existingValue.Value)
            {
               throw new ArgumentException(string.Format("Unable to create DAS output state on location {0}, bed {1} and patient {2}; output state with the same configuration already exists.", oState.LocationId, oState.BedId, oState.PatientId));
            }
         }
         else
         {
            if (!existingValue.HasValue || !existingValue.Value)
            {
               throw new ArgumentException(string.Format("Unable to update DAS output state on location {0}, bed {1} and patient {2}; output state does not exists.", oState.LocationId, oState.BedId, oState.PatientId));
            }
         }
      }

      private string getCurrentUserIdentifier() {
         User currentUser = mobjSyncSvc.GetCurrentUser(mobjSyncSvc.GetSyncAddress());
         return currentUser.Abbrev;
      }

      #endregion

      #region Handlers

      void BeforeSaveHandler(object sender, EventArgs e)
      {

         DasOutputState entity = (DasOutputState)((SaveOrUpdateEventArgs)e).entity;         

         validateData(entity, true);

         entity.LastUpdateDateUtc = DateTime.UtcNow;         
         entity.LastUpdateSource = string.Format("{0}#{1}", mobjConfig.ModuleName, getCurrentUserIdentifier());
      }

      void BeforeUpdateHandler(object sender, EventArgs e)
      {

         DasOutputState entity = (DasOutputState)((SaveOrUpdateEventArgs)e).entity;

         validateData(entity, false);

         entity.LastUpdateDateUtc = DateTime.UtcNow;
         entity.LastUpdateSource = string.Format("{0}#{1}", mobjConfig.ModuleName, getCurrentUserIdentifier());
      }

      void AfterSaveHandler(object sender, EventArgs e)
      {

         DasOutputState entity = (DasOutputState)((SaveOrUpdateEventArgs)e).entity;

         ////Send notification to Message Center
         mobjMsgCtrMgr.SendOutputStateNotification(entity.LocationId, entity.BedId, entity.PatientId, entity.IsSystem);

         ////Send notification to Message Center
         //Task<bool> McNotifiationResult  = Task.Run(async () =>
         //{
         //   bool result = await mobjMsgCtrMgr.GetDasInstanceStatus(entity);
         //   return result;
         //});

         //if (!McNotifiationResult.Result) {
         //   throw new ConnectException("DAS has encountered a problem managing Output State modifications message");
         //}

      }

      void AfterUpdateHandler(object sender, EventArgs e)
      {

         DasOutputState entity = (DasOutputState)((SaveOrUpdateEventArgs)e).entity;

         //Send notification to Message Center
         mobjMsgCtrMgr.SendOutputStateNotification(entity.LocationId, entity.BedId, entity.PatientId, entity.IsSystem);

         ////Send notification to Message Center
         //Task<bool> McNotifiationResult = Task.Run(async () =>
         //{
         //   bool result = await mobjMsgCtrMgr.GetDasInstanceStatus(entity);
         //   return result;
         //});

         //if (!McNotifiationResult.Result)
         //{
         //   throw new ConnectException("DAS has encountered a problem managing Output State modifications message");
         //}
      }

      #endregion

      #region Data reading functions

      public DasOutputState Get(int locationId, int bedId, int patientId)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for DasOutputState with location id {0}, bed id {1} and patient id {2}", locationId, bedId, patientId);

         DasOutputState result = null;

         try
         {
            IQueryable<DasOutputState> repository = mobjDbContext.Set<DasOutputState>();

            //TODO Trace
            mobjLoggerService.Info("Reading DasOutputState with location id {0}, bed id {1} and patient id {2} from DB", locationId, bedId, patientId);

            result = repository.Where(x => x.LocationId == locationId && x.BedId == bedId && x.PatientId == patientId).SingleOrDefault();

            if (result != null)
            {
               result.Location = result.LocationId == 0 ?  null : mobjDbContext.Set<Location>().Single(x => x.Id == result.LocationId);
               result.Bed = result.BedId == 0 ? null : mobjDbContext.Set<Bed>().Single(x => x.Id == result.BedId);
               result.Patient = result.PatientId == 0 ? null : mobjDbContext.Set<Patient>().Single(x => x.Id == result.PatientId);
            }

            //TODO Trace
            mobjLoggerService.Info("DasOutputState with location id {0}, bed id {1} and patient id {2} retrived from DB", locationId, bedId, patientId);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DasOutputState with location id {0}, bed id {1} and patient id {2} from DB", locationId, bedId, patientId);
            throw new Exception(string.Format("Error reading DAS Output State with location id {0}, bed id {1} and patient id {2} from DB", locationId, bedId, patientId), e);
         }

         return result;

      }

      public IQueryable<DasOutputState> GetDasOutputStates()
      {
         var objOS = mobjDbContext.Set<DasOutputState>();
         var objLocation = mobjDbContext.Set<Location>();
         //var objPatient = mobjDbContext.Set<Patient>();
         var objBed = mobjDbContext.Set<Bed>();

         var objQ = from os in objOS.AsQueryable()
                    join loc in objLocation.AsQueryable() on os.LocationId equals loc.Id into locations
                     from l in locations.DefaultIfEmpty()
                    join bed in objBed.AsQueryable() on os.BedId equals bed.Id into beds
                     from b in beds.DefaultIfEmpty()
                    //join pat in objPatient.AsQueryable() on os.PatientId equals pat.Id into patients
                     //from p in patients.DefaultIfEmpty()
                    where os.PatientId == 0 
                    select new DasOutputState
                    {
                       BedId = os.BedId,
                       IsSystem = os.IsSystem,
                       LastUpdateDateUtc = os.LastUpdateDateUtc,
                       LastUpdateSource = os.LastUpdateSource,
                       LocationId = os.LocationId,
                       PatientId = os.PatientId,
                       SamplingSeconds = os.SamplingSeconds,
                       StartDateUtc = os.StartDateUtc,
                       StopDateUtc = os.StopDateUtc,
                       Type = os.Type,
                       Location = l,
                       Bed = b,
                       //Patient = p,
                    };

         return objQ;
      }

      #endregion

      #region Data Writing functions

      /// <summary>
      /// Delete a DasOutputState. 
      /// </summary>
      public void Delete(int locationId, int bedId, int patientId)
      {

         //TODO Trace
         mobjLoggerService.Info("Removing DasOutputState with location id {0}, bed id {1} and patient id {2}", locationId, bedId, patientId);

         if (locationId == 0 && bedId == 0 )
         {
            throw new Exception(string.Format("Default general Output State can not be removed (location id = 0, bed id = 0)."));
         }

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var repository = mobjDbContext.Set<DasOutputState>();

            DasOutputState entity = repository.Where(x => x.LocationId == locationId && x.BedId == bedId && x.PatientId == patientId && x.IsSystem).SingleOrDefault();

            if (entity == null)
            {
               throw new Exception(string.Format("Unable to remove DasOutputState with location id {0}, bed id {1} and patient id {2}; System output state not found.", locationId, bedId, patientId));
            }

            //Create new record for updated entity
            repository.Remove(entity);

            mobjDbContext.SaveChanges();

            if (executeClose) mobjDbContext.CommitTransaction();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendOutputStateNotification(entity.LocationId, entity.BedId, entity.PatientId, entity.IsSystem);

            //TODO Trace
            mobjLoggerService.Info("DasOutputState with location id {0}, bed id {1} and patient id {2} removed succesfully", locationId, bedId, patientId);
         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing DasOutputState with location id {0}, bed id {1} and patient id {2}", locationId, bedId, patientId);
            string message = string.Format("Error removing Das Output State with location id {0}, bed id {1} and patient id {2}", locationId, bedId, patientId);
            throw new Exception(message, e);
         }
      }

      #endregion

   }
}
