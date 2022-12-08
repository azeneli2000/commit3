using System;
using System.Linq;
using Configurator.Std.BL.Hubs;
using Configurator.Std.Enums;
using Configurator.Std.Interfaces;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Ips;
using Microsoft.EntityFrameworkCore;

namespace Configurator.Std.BL.Waveform
{
   public class WaveformManager : DalManagerBase<WaveformSnapshotToUniteRule>, IWaveformManager
   {
      protected readonly IMessageCenterManager mobjMessageCenterService;

      public WaveformManager(DigistatDBContext context, ILoggerService loggerService,IMessageCenterManager msgCtrMgr)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMessageCenterService = msgCtrMgr;
         AfterSave += WaveformManager_AfterSave;
         AfterUpdate += WaveformManager_AfterSave;
      }

      public IQueryable<WaveformSnapshotToUniteRule> GetAllInclude()
      {
         IQueryable<WaveformSnapshotToUniteRule> retObj ;
         try
         {            
            var objWR = mobjDbContext.Set<WaveformSnapshotToUniteRule>().AsNoTracking();
            var objDR = mobjDbContext.Set<DriverRepository>().AsNoTracking();
            var objSP = mobjDbContext.Set<StandardParameter>().AsNoTracking();
            var objL = mobjDbContext.Set<Location>().AsNoTracking();
            var objEC = mobjDbContext.Set<DriverRepositoryEventCatalog>().AsNoTracking();


            var objDRSub = from driverRep in objDR
                           where driverRep.Current == true
                           select new DriverRepository
                           {
                              Id = driverRep.Id,
                              Version = driverRep.Version,
                              Current = driverRep.Current,
                              ValidToDate = driverRep.ValidToDate,
                              AlarmSupport = driverRep.AlarmSupport,
                              ComToRegister = driverRep.ComToRegister,
                              DefaultCommConfiguration = driverRep.DefaultCommConfiguration,
                              Device = driverRep.Device,
                              DeviceType = driverRep.DeviceType,
                              DriverName = driverRep.DriverName,
                              DriverVersion = driverRep.DriverVersion,
                              DriverVersionBuild = driverRep.DriverVersionBuild,                              
                              UseDynamicParameters = driverRep.UseDynamicParameters,
                              Capabilities = driverRep.Capabilities,
                              EventsMapping = driverRep.EventsMapping,
                              AlarmSystemType = driverRep.AlarmSystemType,
                              RunAsDLL = driverRep.RunAsDLL,
                              //XmlRemappedEvents = driverRep.XmlRemappedEvents,
                              RemappedEvents = driverRep.RemappedEvents
                           };

            var objQ = from d in objWR
                join dr in objDRSub on d.IdDriver equals dr.Id into res
                from r in res.DefaultIfEmpty()
                join sp in objSP on d.IdParam equals sp.Id into pes
               from p in pes.DefaultIfEmpty()
               join loc in objL on d.IdLocation equals loc.Id into les
               from l in les.DefaultIfEmpty()
               join evnt in objEC on new { a = d.IdLinkEvent, b = d.IdDriver }  equals new { a = evnt.Id ,b = evnt.DriverRepositoryId} into ees
               from e in ees.DefaultIfEmpty()
               select new WaveformSnapshotToUniteRule
               {
                  Id = d.Id,
                  Priority = d.Priority,
                  IdDriver = d.IdDriver,
                  IdLocation = d.IdLocation,
                  IdLinkEvent = d.IdLinkEvent,
                  IdParam = d.IdParam,
                  Description = d.Description,
                  Repository = r,
                  Parameter = p,
                  Location = l,
                  Event = e
               };

            retObj = objQ;
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            throw;
         }
                      
         return retObj;
      }

      public int GetLowerPriority(int id)
      {
         return GetAllInclude().Max(wf => (int?) wf.Priority).GetValueOrDefault(0) + 1;
      }

      /// <summary>
      ///  Check if id priority is already assigned
      /// </summary>
      /// <param name="priority"></param>
      /// <param name="id"></param>
      /// <returns></returns>
      public bool IsPriorityAlreadyAssigned(int priority, int id = 0)
      {
         if (id == 0)
         {
            return GetAllInclude().Any(wf => wf.Priority == priority);
         }
         else
         {
            return GetAllInclude().Any(wf => wf.Priority == priority && wf.Id != id);
         }
      }

      public int MoveWaveformRule(int waveformRuleId, MoveDirection direction)
      {
         var newAssignedPriority = -1;

         if (waveformRuleId != 0)
         {
            try
            {
               mobjDbContext.BeginTransaction();

               var objWfRuleToMove = mobjDbContext.Set<WaveformSnapshotToUniteRule>().FirstOrDefault(p => p.Id == waveformRuleId);
               if (objWfRuleToMove != null)
               {
                  WaveformSnapshotToUniteRule objInstead = null;
                  if (direction == MoveDirection.Down)
                  {
                     objInstead = mobjDbContext.Set<WaveformSnapshotToUniteRule>()
                        .Where(wfRule => wfRule.Priority > objWfRuleToMove.Priority)
                        .OrderBy(p => p.Priority)
                        .ThenBy(p => p.Id)
                        .FirstOrDefault();
                  }
                  if (direction == MoveDirection.Up)
                  {
                     objInstead = mobjDbContext.Set<WaveformSnapshotToUniteRule>()
                        .Where(wfRule => wfRule.Priority < objWfRuleToMove.Priority)
                        .OrderByDescending(p => p.Priority)
                        .ThenBy(p => p.Id)
                        .FirstOrDefault();
                  }

                  if (objInstead != null)
                  {
                     var oldInsteadPriority = objInstead.Priority;
                     var oldObjToMovePriority = objWfRuleToMove.Priority;
                     objInstead.Priority = -1;
                     mobjDbContext.SaveChanges();
                     objWfRuleToMove.Priority = oldInsteadPriority;
                     mobjDbContext.SaveChanges();
                     objInstead.Priority = oldObjToMovePriority;
                     mobjDbContext.SaveChanges();
                     newAssignedPriority = objWfRuleToMove.Priority;
                  }
               }

               mobjDbContext.CommitTransaction();
               mobjMessageCenterService.SendWaveformRuleEdited(objWfRuleToMove);
            }
            catch (Exception ex)
            {
               mobjDbContext.RollbackTransaction();
               var errMsg = "Error on MoveWaveformRule";
               mobjLoggerService.ErrorException(ex, errMsg);
               throw new Exception(errMsg, ex);
            }
         }

         return newAssignedPriority;
      }

      public int Delete(int id)
      {
         var result = 0;
         try
         {
            var objWR = mobjDbContext.Set<WaveformSnapshotToUniteRule>();
            var objWfRuleToRemove = objWR.SingleOrDefault(wf => wf.Id == id);
            if (objWfRuleToRemove != null)
            {
               objWR.Remove(objWfRuleToRemove);
               mobjLoggerService.Info($"Waveform: Removed Item {objWfRuleToRemove.Id}");
               result = mobjDbContext.SaveChanges();

               mobjMessageCenterService.SendWaveformRuleEdited(objWfRuleToRemove);
            }
         }
         catch (Exception ex)
         {
            result = -1;
            mobjLoggerService.ErrorException(ex, $"Error removing WaveformSnapshotToUniteRule id {id} from DB");
            throw new Exception($"Error removing WaveformSnapshotToUniteRule id {id} from DB", ex);
         }
         return result;
      }

      private void WaveformManager_AfterSave(object sender, EventArgs e)
      {
         var entity = (WaveformSnapshotToUniteRule)((SaveOrUpdateEventArgs) e).entity;
         mobjMessageCenterService.SendWaveformRuleEdited(entity);
      }

   }
}
