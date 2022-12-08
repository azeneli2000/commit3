using Configurator.Std.BL.Hubs;
using Configurator.Std.Enums;
using Configurator.Std.Exceptions;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Extensions;
//using NPOI.POIFS.Storage;
using System.Data.SqlClient;
using Configurator.Std.BL.Configurator;
using System.Threading.Tasks;

namespace Configurator.Std.BL
{
   public class LocationManager : DalManagerBase<Location>, ILocationManager
   {
      protected readonly IMessageCenterManager mobjMsgCtrMgr;
      protected readonly IConfiguratorWebConfiguration mobjConfig;


      public LocationManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrMgr
         , IConfiguratorWebConfiguration config)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrMgr;
         mobjConfig = config;
      }


      public new Location Create(Location entity)
      {
         Location objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();

            if (entity.Id == 0 && mobjDbContext.Set<Location>().Where(p => p.LocationName == entity.LocationName).Count() > 0)
            {
               throw new BedException("Unable to create Location : Location with name {0} already exists.");

            }
            else
            {
               //By default, location index will be the maximum of the current indexes + 1

               if (!entity.LocationIndex.HasValue)
               {
                  entity.LocationIndex = mobjDbContext.Set<Location>().Max(p => p.LocationIndex) + 1;
               }
               var locRepository = mobjDbContext.Set<Location>();
               locRepository.Add(entity);
               mobjDbContext.SaveChanges();
               mobjMsgCtrMgr.SendBedConfig();
               mobjDbContext.CommitTransaction();
               objRet = entity;
            }
         
         }
         catch (BedException)
         {
            throw;
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            string message = string.Format("Error creating Location ", entity.Id);
            mobjLoggerService.ErrorException(e, message);
            throw new Exception(message, e);
         }


         return objRet;
      }

      public new Location Update(Location entity)
      {
         Location objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();

            if (entity.Id != 0 && mobjDbContext.Set<Location>().Where(p => p.LocationName == entity.LocationName && p.Id != entity.Id).Count() > 0)
            {
               throw new BedException("Unable to create Location : Location with name {0} already exists.");

            }
            else
            {
               //By default, location index will be the maximum of the current indexes + 1
             
               if (!entity.LocationIndex.HasValue)
               {
                  entity.LocationIndex = mobjDbContext.Set<Location>().Where(p => p.Id == entity.Id).Max(p => p.LocationIndex) + 1;
               }
               var locRepository = mobjDbContext.Set<Location>();
               locRepository.Update(entity);
               mobjDbContext.SaveChanges();
               mobjMsgCtrMgr.SendBedConfig();
               mobjDbContext.CommitTransaction();
               objRet = entity;
            }

           
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            string message = string.Format("Error updating Location with id {0}",entity.Id);
            mobjLoggerService.ErrorException(e, message);
            throw new Exception(message, e);
         }
         return objRet;
      }

      public void Delete(int entityId)
      {
         //TODO Trace
         mobjLoggerService.Info("Deleting Location with id {0}", entityId.ToString(CultureInfo.InvariantCulture));

         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<Location>();

            var loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to delete Location with id {0}; network not found.", entityId.ToString(CultureInfo.InvariantCulture)));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();

            mobjMsgCtrMgr.SendBedConfig();

            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Location with id {0} removed succesfully", loadedEntity.Id.ToString(CultureInfo.InvariantCulture));

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();            
            string message = string.Format("Error removing Location with id {0}", entityId.ToString(CultureInfo.InvariantCulture));
            mobjLoggerService.ErrorException(e, message);
            throw new Exception(message, e);
         }
      }

      public bool MoveLocation(int LocationID, MoveDirection direction)
      {
         bool bolRet = false;
         //throw new NotImplementedException();
         if (LocationID != 0)
         {
            try
            {
               mobjDbContext.BeginTransaction();

               Location objToMove = mobjDbContext.Set<Location>().Where(p => p.Id == LocationID).FirstOrDefault();
               if (objToMove != null)
               {
                  Location objInstead = null;
                  if (direction == MoveDirection.Down)
                  {
                     objInstead = mobjDbContext.Set<Location>().Where(p => p.LocationIndex > objToMove.LocationIndex).OrderBy(p => p.LocationIndex).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int? tmp = objInstead.LocationIndex;
                        objInstead.LocationIndex = objToMove.LocationIndex;
                        objToMove.LocationIndex = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
                  else
                  {
                     objInstead = mobjDbContext.Set<Location>().Where(p => p.LocationIndex < objToMove.LocationIndex).OrderByDescending(p => p.LocationIndex).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int? tmp = objInstead.LocationIndex;
                        objInstead.LocationIndex = objToMove.LocationIndex;
                        objToMove.LocationIndex = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
               }
               mobjMsgCtrMgr.SendBedConfig();
               mobjDbContext.CommitTransaction();
               bolRet = true;
            }
            catch (Exception ex)
            {

               mobjDbContext.RollbackTransaction();
               string errMsg = "Error on MoveLocation";
               mobjLoggerService.ErrorException(ex, errMsg);
               throw new Exception(errMsg, ex);
            }

         }
         return bolRet;
      }

      public Location Get(int id)
      {
         return GetLocations().Where(x => x.Id == id).SingleOrDefault();
      }

      public List<Location> GetAllWithBedCounts()
      {
         List<Location> objLocations = new List<Location>();
         try
         {
            const string strQuery = @"select LocationName, LocationIndex, LocationCode, IDLocation, count(b.IDBed) as BedCount from Location as l
                                       inner join Bed as b   
                                       on b.LocationRef = l.IDLocation
                                       group by IDLocation, LocationName, LocationIndex, LocationCode";

            using (SqlConnection objConn = new SqlConnection(mobjConfig.ConnectionString))
                                              {
               objConn.Open();
               using (SqlCommand objComm = new SqlCommand(strQuery, objConn))
               {
                  using (SqlDataReader objReader = objComm.ExecuteReader())
                  {
                     while (objReader.Read())
                     {
                        objLocations.Add(new Location()
                        {
                           Id = Int32.Parse(objReader["IDLocation"].ToString()),
                           LocationName = objReader["LocationName"].ToString(),
                           LocationIndex = objReader["LocationIndex"] as int? ?? null,
                           LocationCode = objReader["LocationCode"].ToString(),
                           BedCount = Int32.Parse(objReader["BedCount"].ToString())
                        });
         }
                  }
               }
            }

            //  mobjDbContext.Database.E(strCommand);
            //var objLocRepo = mobjDbContext.Set<Location>();
            //var objBedRepo = mobjDbContext.Set<Bed>();
            //IQueryable<Location> repository = from l in objLocRepo.AsQueryable()
            //                                  join b in objBedRepo.AsQueryable() on l.Id equals b.IdLocation into res
            //                                  select new Location
            //                                  {
            //                                     BedCount = res.AsQueryable().Sum(s => 1),
            //                                     Id = l.Id,
            //                                     LocationCode = l.LocationCode,
            //                                     LocationIndex = l.LocationIndex,
            //                                     LocationName = l.LocationName
            //                                  };
            //return repository.ToList();
         }
         catch(Exception ex)
         {
            mobjLoggerService.ErrorException(ex, "Unable to read Locations GetAllWithBedCounts");
            string message = string.Format("Unable to read all GetAllWithBeds records from DB");
            throw new Exception(message, ex);
         }
         
         return objLocations;
      }

      //public List<Location> GetByBedid(string bedId) {

      //   var repository = mobjDbContext.Set<Bed>();

      //   repository.Where(x => x.)

      //}

      public IQueryable<Location> GetLocations()
      {
         var objLoc = mobjDbContext.Set<Location>();
         var objH = mobjDbContext.Set<HospitalUnit>();

         var objQ = from l in objLoc.AsQueryable()
                    join h in objH.AsQueryable() on l.HospitalUnitGuid equals h.GUID into res
                    from r in res.DefaultIfEmpty()
                    where (r == null || r.Current == true)
                    select new Location
                    {
                       HospitalUnitGuid = l.HospitalUnitGuid,
                       HospitalUnitName = r != null ? r.Name : string.Empty,
                       Id = l.Id,
                       LocationCode = l.LocationCode,
                       LocationIndex = l.LocationIndex,
                       LocationName = l.LocationName,
                       UniteCode = l.UniteCode
                    };

         return objQ.OrderBy(o => o.LocationIndex);
      }

      public bool LocationCanBeDeleted(int id)
      {         
         var objB = mobjDbContext.Set<Bed>();
         var objN = mobjDbContext.Set<Network>();
         var objOB = mobjDbContext.Set<OperatingBlocks>();

         return objB.Where(x => x.IdLocation == id).Count() == 0 
            && objN.Where(x => x.LocationRef == id).Count() == 0 
            && objOB.Where(x => x.ob_IDLocation == id).Count() == 0;         
      }

      public int FixLocationsIndex()
      {
         int intRet = 0;
         try
         {
            var dbLocation = mobjDbContext.Set<Location>();
            var lstLocations = dbLocation.OrderBy(p => p.LocationIndex).ThenBy(p => p.Id).ToList();
            if (lstLocations == null || lstLocations.Count == 0)
            {
               return intRet;
            }
            int? lastLocation = -999999;
            int idx = 0;

            foreach (var loc in lstLocations)
            {
               idx++;
               loc.LocationIndex = idx;
            }
            dbLocation.UpdateRange(lstLocations);
            Task<int> task = mobjDbContext.SaveChangesAsync();
            intRet = task.Result;

         }
         catch (Exception e)
         {
            string errMsg = "Error on FixLocationsIndex";
            mobjLoggerService.ErrorException(e, errMsg);
            throw;
         }
         return intRet;
      }
 
   }
}
