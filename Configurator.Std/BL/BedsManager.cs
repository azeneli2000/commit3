using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Configurator.Std.BL.Hubs;
using Configurator.Std.Exceptions;
using Configurator.Std.Enums;

namespace Configurator.Std.BL
{
   public class BedsManager : DalManagerBase<Bed>, IBedsManager
   {
      protected readonly IMessageCenterManager mobjMsgCtrMgr;

      #region Costructors

      public BedsManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrSvc)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrSvc;
      }

      #endregion

      #region Data reading functions

      public Bed Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for Bed with id {0}", id);

         Bed result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<Bed> repository = mobjDbContext.Set<Bed>();

            //if (loadDeviceDriver3Links)
            //{
            //   repository = repository.Include(x => x.DeviceDriver3Links);
            //}

            //TODO Trace
            mobjLoggerService.Info("Reading Bed with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).Include(x => x.Location).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("Bed with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Bed with id {0} from DB", id);
            throw new Exception(string.Format("Error reading Bed with id {0} from DB", id), e);
         }

         return result;

      }



      #endregion

      #region Data Writing functions


      public new Bed Create(Bed entity)
      {
         Bed objRet = null;
         try
         {
            int intCount = mobjDbContext.Set<Bed>().Where(p => p.BedCode==entity.BedCode && p.IdLocation== entity.IdLocation && p.Name==entity.Name).Count();

            if (intCount == 0)
            {
               mobjDbContext.BeginTransaction();
               entity.Location = null;
               //By default, bed index will be the maximum of the current indexes 
               entity.Index = (mobjDbContext.Set<Bed>().Max(p => p.Index)??0) + 1;
               mobjDbContext.Set<Bed>().Add(entity);
               mobjDbContext.SaveChanges();

               mobjMsgCtrMgr.SendBedConfig();
               mobjMsgCtrMgr.SendBedEdited(entity);

               mobjDbContext.CommitTransaction();
               objRet = entity;
            }
            else
            {
                    throw new BedException("Bed with same code and name already exists in this location");
            }
         }
            catch (BedException e)
            {
                mobjDbContext.RollbackTransaction();
                mobjLoggerService.ErrorException(e, e.Message);
                throw;
            }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            string errMsg = "Error on CreateBED";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      public int MoveBed(int BedID, MoveDirection direction)
      {
         int intRet = -1;
         if (BedID != 0)
         {
            try
            {
               mobjDbContext.BeginTransaction();

               Bed objToMove = mobjDbContext.Set<Bed>().Include(x=>x.Location).Where(p => p.Id == BedID).FirstOrDefault();
               if (objToMove != null)
               {
                  Bed objInstead = null;
                  if (direction == MoveDirection.Down)
                  {
                     objInstead = mobjDbContext.Set<Bed>().Where(p => p.Index > objToMove.Index && p.IdLocation==objToMove.IdLocation).OrderBy(p => p.Index).FirstOrDefault();
                     if(objInstead!=null)
                     {
                        int? tmp = objInstead.Index;
                        objInstead.Index = objToMove.Index;
                        objToMove.Index = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
                  else
                  {
                     objInstead = mobjDbContext.Set<Bed>().Where(p => p.Index < objToMove.Index && p.IdLocation == objToMove.IdLocation).OrderByDescending(p => p.Index).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int? tmp = objInstead.Index;
                        objInstead.Index = objToMove.Index;
                        objToMove.Index = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
                  intRet = objToMove.Index.HasValue?objToMove.Index.Value:-1;
               }

               
               mobjMsgCtrMgr.SendBedEdited(objToMove);
               mobjDbContext.CommitTransaction();
            }
            catch(Exception ex)
            {

               mobjDbContext.RollbackTransaction();
               string errMsg = "Error on MoveBed";
               mobjLoggerService.ErrorException(ex, errMsg);
               throw new Exception(errMsg, ex);
            }
            
         }
         return intRet;
      }

      public int FixBedsIndex()
      {
         int intRet = 0;
         try
         {
            var dbBed = mobjDbContext.Set<Bed>();
            var objBeds = dbBed.OrderBy(p => p.IdLocation).ThenBy(p => p.Index).ThenBy(p => p.Id).ToList();
            if (objBeds == null || objBeds.Count == 0)
            {
               return intRet;
            }
            int? lastLocation = -999999;
            int idx = 0;

            foreach (var bed in objBeds)
            {
               idx++;
               if (lastLocation.GetValueOrDefault(-99999) != bed.IdLocation.GetValueOrDefault(-99999))
               {
                  lastLocation = bed.IdLocation;
                  idx = 1;
               }
               bed.Index = idx;
            }
            dbBed.UpdateRange(objBeds);
            Task<int> task = mobjDbContext.SaveChangesAsync();
            intRet = task.Result;

         }
         catch (Exception e)
         {
            string errMsg = "Error on FixBedsIndex";
            mobjLoggerService.ErrorException(e, errMsg);
            throw;
         }
         return intRet;
      }
      public new Bed Update(Bed entity)
      {
         Bed objRet = null;
         try
         {
            if(entity!=null)
            {
               mobjDbContext.BeginTransaction();
               //Check if a bed with the same name already exists
               if(mobjDbContext.Set<Bed>().Include(x=>x.Location).Where(p=>p.Name==entity.Name && p.IdLocation == entity.IdLocation && p.Id != entity.Id).Count() > 0)
               {
                  throw new BedException("Unable to create bed : Bed with name {0} in location {1} already exists.");
                  
               }
               else
               {
                  //Save location name in a field, so I can clear location entity (I don't want to save it!!)
                  string strLocationName = entity.Location!=null? entity.Location.LocationName : null;
                  entity.Location = null;
                  //By default, bed index will be the maximum of the current indexes + 1

                  if (!entity.Index.HasValue)
                  {
                     int? objOldIndex = mobjDbContext.Set<Bed>().Where(p => p.IdLocation == entity.IdLocation).Max(p => p.Index);
                     if(objOldIndex.HasValue)
                     {
                        entity.Index = objOldIndex.Value+1;
                     }
                     else
                     {
                        entity.Index = 1;
                     }
                  }
                  
                  mobjDbContext.Set<Bed>().Update(entity);
                  mobjDbContext.SaveChanges();
                  mobjMsgCtrMgr.SendBedConfig();

                  //Reset location with locationname for message
                  entity.Location = new Location();
                  entity.Location.LocationName = strLocationName;

                  mobjMsgCtrMgr.SendBedEdited(entity);
               }
               
           
               mobjDbContext.CommitTransaction();
            }
            else
            {
               throw new BedException("Unable to create bed : Bed entity is null.");
            }
         }
         catch(BedException)
         {
            throw;
         }
         catch (Exception e)
         {
            
            mobjDbContext.RollbackTransaction();
            string errMsg = "Error on UpdateBed";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }


      public IQueryable<Bed> GetBedsWithFullData()
      {
         try
         {
            //List<Bed> objBedList = null;
            IQueryable<Bed> repository = mobjDbContext.Set<Bed>().Include(d => d.Location);
            return repository;
         }
         catch (Exception e)
         {
            string errMsg = "Error GetBedsWithFullData";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
      }

      public string Delete(int entityId)
      {

         string strRet = null;

         var executeClose = mobjDbContext.BeginTransaction();
         try
         {

            strRet = CheckIfBedCanBeDeleted(entityId);
            if (string.IsNullOrEmpty(strRet))
            {
               var repository = mobjDbContext.Set<Bed>();

               Bed loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
               if (loadedEntity == null)
               {
                  throw new Exception(string.Format("Unable to delete bed with id {0}; bed not found.", entityId));
               }
               repository.Remove(loadedEntity);

               //Delete from networkBedLink
               var repoNetBedLnk = mobjDbContext.Set<NetworkBedLink>();
               var objNetBedList =  repoNetBedLnk.Where(p => p.IdBed == entityId).ToList();
               repoNetBedLnk.RemoveRange(objNetBedList.ToList());


               mobjMsgCtrMgr.SendBedConfig();

               mobjDbContext.SaveChanges();

               if (executeClose) { mobjDbContext.CommitTransaction(); }
            }

         }
         catch (Exception e)
         {
            if (executeClose) { mobjDbContext.RollbackTransaction(); }
            mobjLoggerService.ErrorException(e, "Error removing bed with id {0}", entityId);
            string message = string.Format("Error removing bed with id {0}", entityId);
            throw new Exception(message, e);
         }
         return strRet;
      }

      #endregion


      private string CheckIfBedCanBeDeleted(int bedID)
      {
         string strTables = string.Empty;
         try
         {

            IDbConnection objDbConn = mobjDbContext.Database.GetDbConnection();
            using (var cmd = objDbConn.CreateCommand())
            {
               string strSQL = @"select 'Patient-Bed' as TableName where exists (select 1 from Bed where IDBed = @idBed and PatientRef != 0) 
				      UNION 
				      select 'Network' as TableName where exists (select 1 from Network where BedRef = @idBed)
				      UNION
				      select 'DriversOnComm' as TableName  where exists (select 1 from DriversOnComm where BedRef = cast(@idBed as nvarchar))
				      UNION
				      select 'OperatingRooms' as TableName  where exists (select 1 from OperatingRooms where or_IDBed = @idBed)
				      UNION 
				      select 'SerialPort' as TableName  where exists (select 1 from SerialPort where BedRef = @idBed)";
               cmd.CommandText = strSQL;
               cmd.Parameters.Add(new SqlParameter("@idBed", bedID));
               cmd.Transaction = mobjDbContext.Database.CurrentTransaction.GetDbTransaction();
               using (var reader = cmd.ExecuteReader())
               {
                
                  while (reader.Read())
                  {
                     strTables += reader.GetString(0) + ";";
                  }
                
               }
            }
            
         }
         catch(Exception exc)
         {
            string errMSg = $"Error on CheckIfBedCanBeDeleted for bedID {bedID} from DB";
            mobjLoggerService.ErrorException(exc, errMSg);
            throw new Exception(errMSg, exc);
         }
         return strTables;
               

      }

   
   }
}
