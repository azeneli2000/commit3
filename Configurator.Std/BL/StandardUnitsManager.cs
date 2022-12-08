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
   public class StandardUnitsManager : DalManagerBase<StandardUnit>, IStandardUnitsManager
   {

      #region Costructors

      public StandardUnitsManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      #endregion

      #region Data reading functions

      public StandardUnit Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for StandardUnitsManager with id {0}", id);

         StandardUnit result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardUnit> repository = mobjDbContext.Set<StandardUnit>();

            //TODO Trace
            mobjLoggerService.Info("Reading StandardUnit with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("StandardUnit with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardUnit with id {0} from DB", id);
            throw new Exception(string.Format("Error reading StandardUnit with id {0} from DB", id), e);
         }


         return result;

      }

      public List<StandardUnit> GetMulti(List<int> ids)
      {

         List<StandardUnit> result = null;
         string strIds = string.Empty;
         try
         {

            if (ids != null)
            {
               strIds = string.Join(",", ids.ToArray());
               IQueryable<StandardUnit> repository = mobjDbContext.Set<StandardUnit>();
               result = repository.Where(x => ids.Contains(x.Id)).ToList();
            }
            else
            {
               return null;
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardUnit with GetMulti {0} from DB", strIds);
            throw new Exception(string.Format("Error reading StandardUnit with GetMulti {0} from DB", strIds), e);
         }
         return result;

      }

      #endregion

      #region Data Writing functions

      public void Delete(int entityId)
      {

         //TODO Trace
         mobjLoggerService.Info("Deleting StandardUnit with id {0}", entityId);

         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<StandardUnit>();

            StandardUnit loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update update standard unit with id {0}; update standard not found.", entityId));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("StandardUnit with id {0} removed succesfully", loadedEntity.Id);

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing update standard unit with id {0}", entityId);
            string message = string.Format("Error removing update standard unit with id {0}", entityId);
            throw new Exception(message, e);
         }

      }

      #endregion

   }
}
