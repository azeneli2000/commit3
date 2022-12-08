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
   public class DeviceDrivers3BedLinksManager : DalManagerBase<DeviceDriver3BedLink>, IDeviceDrivers3BedLinksManager
   {

      #region Costructors

      public DeviceDrivers3BedLinksManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      #endregion

      #region Data reading functions

      public DeviceDriver3BedLink Get(int idDeviceDriver, int idBed, bool loadDeviceDriver = false, bool loadBed = false)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for DeviceDriver3BedLink for device driver id {0} and bed id", idDeviceDriver, idBed);

         DeviceDriver3BedLink result = null;

         try
         {
            //Set detached loading
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<DeviceDriver3BedLink> repository = mobjDbContext.Set<DeviceDriver3BedLink>();

            if (loadDeviceDriver)
            {
               repository = repository.Include(x => x.DeviceDriver3);
            }

            if (loadBed)
            {
               repository = repository.Include(x => x.Bed);
            }

            //TODO Trace
            mobjLoggerService.Info("Reading DeviceDriver3BedLink for device driver id {0} and bed id {1} from DB", idDeviceDriver, idBed);

            result = repository.Where(x => x.DeviceDriverId == idDeviceDriver && x.BedId == idBed).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3BedLink for device driver id {0} and bed id {1} retrived from DB", idDeviceDriver, idBed);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DeviceDriver3BedLink for device driver id {0} and bed id {1} from DB", idDeviceDriver, idBed);
            throw new Exception(string.Format("Error reading relation between Device Driver with id {0} and Bed {1} from DB", idDeviceDriver, idBed), e);
         }

         return result;

      }

      public IEnumerable<DeviceDriver3BedLink> GetByDeviceDriverId(int deviceDriverId, bool loadBed = false)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get DeviceDriver3BedLinks for device driver with id {1}", deviceDriverId);

         List<DeviceDriver3BedLink> result;

         try
         {
            IQueryable<DeviceDriver3BedLink> repository = mobjDbContext.Set<DeviceDriver3BedLink>();

            repository = repository.Where(x => x.DeviceDriverId == deviceDriverId);

            if (loadBed)
            {
               repository = repository.Include(x => x.Bed);
            }

            result = repository.ToList();

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3BedLinks for device driver with id {0} retrived succesfully, {1} elements found", deviceDriverId, result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read DeviceDriver3BedLinks for device driver with id {0} from DB", deviceDriverId);
            string message = string.Format("Unable to read relations between device driver with id {0} and beds from DB", deviceDriverId);
            throw new Exception(message, e);
         }

         return result;

      }

      public IEnumerable<DeviceDriver3BedLink> GetByDeviceDriverIds(IEnumerable<int> deviceDriverIds, bool loadBed = false)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get DeviceDriver3BedLinks for device driver with ids list");

         List<DeviceDriver3BedLink> result = new List<DeviceDriver3BedLink>();
         try
         {
            IQueryable<DeviceDriver3BedLink> repository = mobjDbContext.Set<DeviceDriver3BedLink>();

            //This method load bedlinks using db "IN" caluse. 
            //"IN" clause in SQl Server as a limit of 32767 element passed in 
            //SQL Server limits the number of identifiers and constants that can be contained in a single expression of a query.
            //This cause a limit of of 32767 element passed to "IN" clause
            //To prevent this issue in case of huge number of devicedriverids request, this method splits the request in different queries respecting the given limit.

            int sqlInClauselimit = 32767;
            decimal cicles = Math.Ceiling(Decimal.Divide(deviceDriverIds.Count(), sqlInClauselimit));

            for (int i = 0; i < cicles; i++)
            {
               IEnumerable<int> splitteddeviceDriverId = deviceDriverIds.Take(sqlInClauselimit).Skip(sqlInClauselimit * i);
               repository = repository.Where(x => splitteddeviceDriverId.Contains(x.DeviceDriverId));

               if (loadBed)
               {
                  repository = repository.Include(x => x.Bed);
               }

               result.AddRange(repository.ToList());
            }

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3BedLinks search for {0} device driver ids finished, retrived succesfully {1} elements found", deviceDriverIds.Count(), result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read DeviceDriver3BedLinks from DB for the {0} device driver ids required", deviceDriverIds.Count());
            string message = "Unable to read relations from DB between device drivers with id int the required range and beds";
            throw new Exception(message, e);
         }

         return result;


      }

      public IEnumerable<DeviceDriver3BedLink> GetByBedId(int bedId, bool loadDeviceDriver = false)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get DeviceDriver3BedLinks for bed with id {1}", bedId);

         List<DeviceDriver3BedLink> result;

         try
         {
            IQueryable<DeviceDriver3BedLink> repository = mobjDbContext.Set<DeviceDriver3BedLink>();

            repository = repository.Where(x => x.BedId == bedId);

            if (loadDeviceDriver)
            {
               repository = repository.Include(x => x.DeviceDriver3);
            }

            result = repository.ToList();

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3BedLinks for bed with id {0} retrived succesfully, {1} elements found", bedId, result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read DeviceDriver3BedLinks for bed with id {0} from DB", bedId);
            string message = string.Format("Unable to read relations between bed with id {0} and device drivers from DB", bedId);
            throw new Exception(message, e);
         }

         return result;

      }
      #endregion

      #region Data Writing functions

      /// <summary>
      /// Delete a DeviceDriver3BedLink. 
      /// </summary>
      public void Delete(int idDeviceDriver)
      {
         //TODO Trace
         mobjLoggerService.Info("Removing all DeviceDriver3BedLink for device driver with id {0}", idDeviceDriver);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            var repository = mobjDbContext.Set<DeviceDriver3BedLink>();

            IEnumerable<DeviceDriver3BedLink> entities = repository.Where(x => x.DeviceDriverId == idDeviceDriver).ToList();
            if (!entities.Any())
            {
               //throw new Exception(string.Format("Unable to remove DeviceDriver3BedLink for device driver with id {0}; no DeviceDriver3BedLink found.", idDeviceDriver));
               return;
            }

            //Create new record for updated entity
            repository.RemoveRange(entities);

            mobjDbContext.SaveChanges();

            if (executeClose) { mobjDbContext.CommitTransaction(); }

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3BedLink for device driver with id {0} removed succesfully", idDeviceDriver);
         }
         catch (Exception e)
         {
            if (executeClose) { mobjDbContext.RollbackTransaction(); }
            mobjLoggerService.ErrorException(e, "Error removing DeviceDriver3BedLink for device driver with id {0}", idDeviceDriver);
            string message = string.Format("Error removing relation between device driver with id {0}", idDeviceDriver);
            throw new Exception(message, e);
         }
      }

      #endregion

   }
}
