using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Digistat.FrameworkStd.Interfaces;

using Configurator.Std.BL.Configurator;
using Configurator.Std.BL.Hubs;

namespace Configurator.Std.BL.DasDrivers
{
   public class DriverMonitorService : IDriverMonitorService
   {

      #region Costructors

      private readonly IMessageCenterService mobjMsgCtrSvc;
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly ILoggerService mobjLoggerService;
      private readonly IConfiguratorWebConfiguration mobjConfig;

      public DriverMonitorService(IConfiguratorWebConfiguration config, IMessageCenterService msgCtrSvc, IMessageCenterManager msgCtrMgr, ILoggerService loggerService)
      {
         mobjMsgCtrSvc = msgCtrSvc;
         mobjMsgCtrMgr = msgCtrMgr;
         mobjLoggerService = loggerService;
         mobjConfig = config;
      }

      #endregion


      public async Task<List<DasInstance>> GetDasInstances()
      {
         using (var mgr = new AsyncDasDispatcher(mobjMsgCtrSvc, mobjConfig, mobjLoggerService))
         {
            try
            {
               var data = await mgr.RequestDasInstances();

               return data;
            }
            catch (TaskCanceledException)
            {
               if (mgr.HasInstancesResult())
               {
                  return await mgr.GetDasInstances();
               }
               else
               {
                  throw;
               }
            }
            catch (Exception)
            {

               throw;
            }
         }
      }

      public async Task<Dictionary<string, List<DriverStatus>>> GetDasInstanceStatus(string comupterName)
      {

         using (var mgr = new AsyncDasInstanceDispatcher(mobjMsgCtrSvc, mobjConfig, mobjLoggerService))
         {
            try
            {
               var data = await mgr.RequestDriversStatus(comupterName);

               return data;
            }
            catch (TaskCanceledException)
            {
               if (mgr.HasBrokersResult())
               {
                  return await mgr.GetDriversStatus();
               }
               else
               {
                  throw;
               }
            }
            catch (Exception)
            {

               throw;
            }
            //return await mgr.RequestDriversStatus(comupterName);
         }
      }

      public void RestartDriver(int deviceDriverId, int processId, string dasBroker)
      {
         try
         {
            mobjMsgCtrMgr.SendRestartDriver(deviceDriverId, processId, dasBroker, false);
         }
         catch (Exception)
         {

            throw;
         }
      }

      public void KillProcess(int deviceDriverId, int processId, string dasBroker)
      {
         try
         {
            mobjMsgCtrMgr.SendRestartDriver(deviceDriverId, processId, dasBroker, true);
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}

