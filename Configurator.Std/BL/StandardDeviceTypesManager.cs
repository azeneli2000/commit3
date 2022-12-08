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
   public class StandardDeviceTypesManager : DalManagerBase<StandardDeviceType>, IStandardDeviceTypesManager
   {

      #region Costructors

      public StandardDeviceTypesManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;

      }

      #endregion

      #region Data reading functions

      public StandardDeviceType Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for StandardDeviceTypesManager with id {0}", id);

         StandardDeviceType result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDeviceType> repository = mobjDbContext.Set<StandardDeviceType>();

            //TODO Trace
            mobjLoggerService.Info("Reading StandardDeviceType with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("StandardDeviceType with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardDeviceType with id {0} from DB", id);
            throw new Exception(string.Format("Error reading StandardDeviceType with id {0} from DB", id), e);
         }
         

         return result;

      }

      #endregion

      #region Data Writing functions

      public void Delete(int entityId)
      {

         //TODO Trace
         mobjLoggerService.Info("Deleting StandardDeviceType with id {0}", entityId);


         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var repository = mobjDbContext.Set<StandardDeviceType>();

            StandardDeviceType loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update update standard with id {0}; update standard not found.", entityId));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("StandardDeviceType with id {0} removed succesfully", loadedEntity.Id);

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing update standard device type with id {0}", entityId);
            string message = string.Format("Error removing update standard device type with id {0}", entityId);
            throw new Exception(message, e);
         }
         
      }

      #endregion
   }
}
