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
   public class DeviceDrivers3LogsManager : DalManagerBase<DeviceDriver3Log>, IDeviceDrivers3LogsManager
   {

      #region Costructors

      public DeviceDrivers3LogsManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      #endregion

      #region Data reading functions

      public IEnumerable<DeviceDriver3Log> GetByDeviceDriverId(int deviceDriverId)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get DeviceDriver3Log for device driver with id {1}", deviceDriverId);

         List<DeviceDriver3Log> result;

         try
         {
            IQueryable<DeviceDriver3Log> repository = mobjDbContext.Set<DeviceDriver3Log>();

            repository = repository.Where(x => x.DeviceDriverId == deviceDriverId);

            result = repository.ToList();

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3Log for device driver with id {0} retrived succesfully, {1} elements found", deviceDriverId, result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read DeviceDriver3Log for device driver with id {0} from DB", deviceDriverId);
            string message = string.Format("Unable to read logs for device driver with id {0} from DB", deviceDriverId);
            throw new Exception(message, e);
         }

         return result;

      }      

      #endregion
   }
}
