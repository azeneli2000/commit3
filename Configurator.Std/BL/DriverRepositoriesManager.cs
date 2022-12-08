using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Configurator.Std.BL.Hubs;
using Configurator.Std.BL.DasDrivers;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using Configurator.Std.Exceptions;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Exceptions;

namespace Configurator.Std.BL
{
   public class DriverRepositoriesManager : DalManagerBase<DriverRepository>, IDriverRepositoriesManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDriverManager mobjDriverManager;
      private readonly IDigistatConfiguration mobjDigCfg;
      private readonly IDictionaryService mobjDicSvc;
      private readonly string mstrCDSS_HARDWARE_ID = "ASCOM CDSS SERVER";
      public DriverRepositoriesManager(DigistatDBContext context,
         IMessageCenterManager msgCtrMgr, IDriverManager driverManager, ILoggerService loggerService, IDigistatConfiguration digCfg, IDictionaryService dicSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjDriverManager = driverManager;
         mobjLoggerService = loggerService;
         mobjDigCfg = digCfg;
         mobjDicSvc = dicSvc;
         //this.BeforeSave += BeforeSaveHandler;
         //this.BeforeUpdate += BeforeUpdateHandler;
         //this.AfterSave += AfterSaveHandler;
      }


      private void validateData(DriverRepository driverRepository, bool checkStream)
      {

         if (String.IsNullOrWhiteSpace(driverRepository.DriverName))
         {
            throw new ConnectException(mobjDicSvc.XLate("Unable to save a driver without Driver Name"));
         }

         if (String.IsNullOrWhiteSpace(driverRepository.DriverVersion))
         {
            throw new ConnectException(mobjDicSvc.XLate("Unable to save a driver without Driver Version"));
         }

         if (checkStream && (driverRepository.Stream == null || driverRepository.Stream.Length == 0))
         {
            throw new ConnectException(mobjDicSvc.XLate("Unable to save a driver without file"));
         }


         var repository = mobjDbContext.Set<DriverRepository>();

         //Prevent duplications
         IEnumerable<DriverRepository> loadedEntities = repository.Where(x => x.DriverName == driverRepository.DriverName && x.DriverVersion == driverRepository.DriverVersion && x.Id != driverRepository.Id).ToList();
         if (loadedEntities.Any())
         {
            //throw new Exception(string.Format("Unable to crate driver repository {0} {1}; driver repository with same name and version already exists.", driverRepository.DriverName, driverRepository.DriverVersion));
            //throw new ConnectException(mobjDicSvc.XLate($"Driver {driverRepository.DriverName} with version {driverRepository.DriverVersion} already exists"));
            string msg = $"Driver {driverRepository.DriverName} with version {driverRepository.DriverVersion} already exists";

            if (loadedEntities.All(x => !x.Current))
            {
               msg += ". It's no more active!!";
            }

            throw new ConnectException(mobjDicSvc.XLate(msg));
         }

      }

      //private void BeforeSaveHandler(object sender, EventArgs e)
      //{

      //   DriverRepository driverRepository = (DriverRepository)((SaveOrUpdateEventArgs)e).entity;

      //   validateData(driverRepository);

      //   //Set current set as record
      //   driverRepository.Id = Guid.NewGuid().ToString();
      //   driverRepository.Version = 1;
      //   driverRepository.Current = true;
      //}

      //private void BeforeUpdateHandler(object sender, EventArgs e)
      //{

      //   DriverRepository driverRepository = (DriverRepository)((SaveOrUpdateEventArgs)e).entity;

      //   validateData(driverRepository);
      //}

      //void AfterSaveHandler(object sender, EventArgs e)
      //{
      //   DriverRepository driverRepository = (DriverRepository)((SaveOrUpdateEventArgs)e).entity;

      //   //Send notification to Message Center
      //   mobjMsgCtrMgr.SendDriverEdited(driverRepository.DriverName, driverRepository.Id, true, !string.IsNullOrWhiteSpace(driverRepository.FormatStyle), driverRepository.Capabilities.Count > 0);
      //}

      #endregion

      #region Data reading functions


      public DriverCommConfiguation GetConfiguration(string id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for DriverRepository with id {0}", id);

         DriverCommConfiguation result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            //TODO Trace
            mobjLoggerService.Info("Reading DriverRepository with id {0} from DB", id);
            var config = repository.Where(x => x.Id == id && x.Current == true).Select(x => x.DefaultCommConfiguration).SingleOrDefault();

            result = new DriverCommConfiguation(config);

            //TODO Trace
            mobjLoggerService.Info("DriverRepository with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DriverRepository with guid {0} from DB", id);
            throw new Exception(string.Format("Error reading DriverRepository with guid {0} from DB", id), e);
         }

         return result;

      }

      public DriverRepository Get(string id, bool loadCapabilities = false, bool loadEventsMapping = false, bool noTracking = false)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for DriverRepository with id {0}", id);

         DriverRepository result = null;

         try
         {

            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();
            if (noTracking)
               result = repository.Where(x => x.Id == id && x.Current == true).AsNoTracking().SingleOrDefault();
            else
               result = repository.Where(x => x.Id == id && x.Current == true).SingleOrDefault();
            //            result = repository.Where(x => x.Id == id && x.Current == true).SingleOrDefault();

            if (loadCapabilities)
            {
               //result.Capabilities =
               // mobjDbContext.Set<DriverRepositoryStandardParameterLink>()

               // .Include(x => x.StandardParameter)

               // .Include(x => x.StandardUnit)
               // .Include(x => x.StandardDeviceType)
               // .Where(x => x.DriverRepositoryId == id)
               // .OrderBy(x => x.StandardParameter.Id)
               // .ToList();

               IQueryable<DriverRepositoryStandardParameterLink> objSParLink = mobjDbContext.Set<DriverRepositoryStandardParameterLink>();
               IQueryable<StandardParameter> objSPar = mobjDbContext.Set<StandardParameter>();
               IQueryable<StandardUnit> objSUni = mobjDbContext.Set<StandardUnit>();
               IQueryable<StandardDeviceType> objSDev = mobjDbContext.Set<StandardDeviceType>();


               //var objQxxx = objSParLink
               //   .GroupJoin(objSPar, l => l.StandardParameterId, h => h.Id, (l, res) => new { l, res })
               //   .GroupJoin(objSDev, @t => @t.l.StandardDeviceTypeId, d => d.Id, (@t, devs) => new { @t, devs })
               //   .GroupJoin(objSUni, @t => @t.@t.l.StandardUnitId, u => u.Id, (@t, units) => new { @t, units })
               //   .Where(@t => (@t.@t.@t.l.DriverRepositoryId == id))
               //   .Select(@t => new DriverRepositoryStandardParameterLink
               //   {
               //       DriverRepositoryId = @t.@t.@t.l.DriverRepositoryId,
               //       DeviceId = @t.@t.@t.l.DeviceId,
               //       DeviceText = @t.@t.@t.l.DeviceText,
               //       DeviceUnitText = @t.@t.@t.l.DeviceUnitText,
               //       IsEnabled = @t.@t.@t.l.IsEnabled,
               //       //MustBeSaved = @t.@t.@t.l.MustBeSaved,
               //       StandardParameterId = @t.@t.@t.l.StandardParameterId,
               //       StandardParameterIdAlias = @t.@t.@t.l.StandardParameterIdAlias,
               //       StandardParameter = @t.@t.@t.res.FirstOrDefault(),
               //       StandardDeviceTypeId = @t.@t.@t.l.StandardDeviceTypeId,
               //       StandardDeviceType = @t.@t.devs.FirstOrDefault(),
               //       StandardUnitId = @t.@t.@t.l.StandardUnitId,
               //       StandardUnit = @t.units.FirstOrDefault(),
               //       Sporadic = @t.@t.@t.l.Sporadic
               //   });

               var objQ = from l in objSParLink
                          where (l.DriverRepositoryId == id)
                          select new DriverRepositoryStandardParameterLink
                          {
                             DriverRepositoryId = l.DriverRepositoryId,
                             DeviceId = l.DeviceId,
                             DeviceText = l.DeviceText,
                             DeviceUnitText = l.DeviceUnitText,
                             IsEnabled = l.IsEnabled,
                             //MustBeSaved = l.MustBeSaved,
                             StandardParameterId = l.StandardParameterId,
                             StandardParameterIdAlias = l.StandardParameterIdAlias,
                             StandardParameter = objSPar.Where(h => h.Id == l.StandardParameterId).FirstOrDefault(),
                             StandardDeviceTypeId = l.StandardDeviceTypeId,
                             StandardDeviceType = objSDev.Where(d => d.Id == l.StandardDeviceTypeId).FirstOrDefault(),
                             StandardUnitId = l.StandardUnitId,
                             StandardUnit = objSUni.Where(u => u.Id == l.StandardUnitId).FirstOrDefault(),
                             Sporadic = l.Sporadic
                          };

               result.Capabilities = objQ.OrderBy(o => o.StandardParameterId).ToList();
               //Load Parameters to be saved from DASAcquisitionTableManager
               DAS3Plus.DASAcquisitionTableManager objAcqTblMgr = new DAS3Plus.DASAcquisitionTableManager(mobjDbContext, mobjDigCfg, mobjLoggerService);
               var objParameters = objAcqTblMgr.GetAcquisitionParameter(result.Id);

               foreach ((int, string) i in objParameters)
               {
                  var par = result.Capabilities.FirstOrDefault(p => p.StandardParameterId == i.Item1 &&
                                                                    (string.IsNullOrEmpty(p.StandardParameterIdAlias) ? "" : p.StandardParameterIdAlias) == i.Item2);
                  if (par != null)
                  {
                     par.MustBeSaved = true;
                  }
               }
            }

            if (loadEventsMapping)
            {
               result.EventsMapping = mobjDbContext.Set<DriverRepositoryEventCatalog>().Where(x => x.DriverRepositoryId == id).ToList();

               //Load remapping from xml
               if (!string.IsNullOrEmpty(result.RemappedEvents))
               {
                  List<DriverRepositoryEventCatalog> objRemapped = UMSFrameworkParser.DeserializeUMSRemappedEvents(result.RemappedEvents);
                  if (objRemapped != null)
                  {
                     foreach (DriverRepositoryEventCatalog objEc in objRemapped)
                     {
                        DriverRepositoryEventCatalog objFound = result.EventsMapping.Where(p => p.Id == objEc.Id).FirstOrDefault();
                        if (objFound != null)
                        {
                           objFound.NewClass = objEc.NewClass;
                           objFound.NewLevel = objEc.NewLevel;
                           objFound.TextENG = objEc.TextENG;
                           objFound.TextENGShort = objEc.TextENGShort;
                           objFound.TextUser = objEc.TextUser;
                           objFound.TextUserShort = objEc.TextUserShort;
                        }
                     }
                  }
               }

            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DriverRepository with guid {0} from DB", id);
            throw new Exception(string.Format("Error reading DriverRepository with guid {0} from DB", id), e);
         }
         return result;
      }


      public IEnumerable<DriverRepositoryEventCatalog> GetEventsByDriverId(string driverId)
      {
         var retEvents = new List<DriverRepositoryEventCatalog>();
         try
         {
            retEvents = mobjDbContext.Set<DriverRepositoryEventCatalog>().Where(x => x.DriverRepositoryId == driverId).ToList();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DriverRepositoryEventCatalog with driver id {0} from DB", driverId);
            throw new Exception(string.Format("Error reading DriverRepositoryEventCatalog with driver id {0} from DB", driverId), e);
         }
         return retEvents;
      }


      public IEnumerable<DriverRepository> GetActives()
      {
         List<DriverRepository> result;

         try
         {
            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            result = repository.Where(x => x.Current).OrderBy(x => x.DriverName).ThenBy(x => x.DriverVersion).ToList();

            //TODO Trace
            mobjLoggerService.Info("Active DriverRepository list retrived succesfully, {0} elements found", result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read active DriverRepositories from DB");
            string message = string.Format("Unable to read active driver repositories from DB");
            throw new Exception(message, e);
         }

         return result;

      }

      
      public List<(string Id, int Version, string DriverName)> GetActivesLight()
      {
         List<(string Id, int Version, string DriverName)> result = new List<(string Id, int DriverVersion, string DriverName)>();

         try
         {
            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            var X = repository.Where(x => x.Current)
               .OrderBy(x => x.DriverName).ThenBy(x => x.DriverVersion)
               .Select(_ =>
                  new {
                     _.Id
                     , _.Version, _.DriverName
                  })
               .ToList();
            for (int i = 0; i < X.Count; i++)
            {
               result.Add( (X[i].Id,X[i].Version,X[i].DriverName));
            }
            //TODO Trace
            mobjLoggerService.Info("Active DriverRepository list retrived succesfully, {0} elements found", result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read active DriverRepositories from DB");
            string message = string.Format("Unable to read active driver repositories from DB");
            throw new Exception(message, e);
         }

         return result;

      }

      public bool CheckCdssAlreadyExists()
      {
         try
         {
            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            var result = repository.FirstOrDefault(x => x.Current && x.HardwareRelease == mstrCDSS_HARDWARE_ID) != null;

            return result;
         }
         catch
         {
            return false;
         }
      }

      public string GetCdssGuid()
      {
         try
         {
            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            var result = repository.FirstOrDefault(x => x.Current && x.HardwareRelease == mstrCDSS_HARDWARE_ID).Id ?? Guid.Empty.ToString();

            return result;
         }
         catch
         {
            return Guid.Empty.ToString();
         }
      }

      public IEnumerable<T> GetActives<T>(Expression<Func<DriverRepository, T>> selectPredicate = null)
      {
         List<T> result;

         try
         {
            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            result = repository.Where(x => x.Current).OrderBy(x => x.DriverName).ThenBy(x => x.DriverVersion).Select(selectPredicate).ToList();

            //TODO Trace
            mobjLoggerService.Info("Active DriverRepository list retrived succesfully, {0} elements found", result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read active DriverRepositories from DB");
            string message = string.Format("Unable to read active driver repositories from DB");
            throw new Exception(message, e);
         }

         return result;

      }

      //Disable Base methods not working for this entity
      public new IEnumerable<DriverRepository> GetAll(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<DriverRepository, object>>> includePredicates = null)
      {
         throw new NotImplementedException("GetAll method with include predicate is not available for Driver Repository, use  loadCapabilities and loadEventsMapping");
      }

      public IEnumerable<DriverRepository> GetAll(int pageNumber = 0, int pageSize = 0, bool loadCapabilities = false, bool loadEventsMapping = false)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing GetAll DriverRepository with page number {0} and page size {1}", pageNumber, pageSize);

         List<DriverRepository> result;

         try
         {
            IQueryable<DriverRepository> repository = mobjDbContext.Set<DriverRepository>();

            if (pageNumber > 0)
            {
               repository = repository.Skip((pageNumber - 1) * pageSize);
            }

            if (pageSize > 0)
            {
               repository = repository.Take(pageSize);
            }

            result = repository.ToList();


            if (loadCapabilities || loadEventsMapping)
            {
               foreach (var item in result)
               {
                  if (loadCapabilities)
                  {
                     item.Capabilities = mobjDbContext.Set<DriverRepositoryStandardParameterLink>()
                                          .Include(x => x.StandardParameter)
                                          .Include(x => x.StandardUnit)
                                          .Include(x => x.StandardDeviceType)
                                          .Where(x => x.DriverRepositoryId == item.Id)
                                          .ToList();
                  }

                  if (loadEventsMapping)
                  {
                     item.EventsMapping = mobjDbContext.Set<DriverRepositoryEventCatalog>().Where(x => x.DriverRepositoryId == item.Id).ToList();
                  }
               }
            }

            //TODO Trace
            mobjLoggerService.Info("Page {0} ({1} items per page) of all DriverRepository list retrived succesfully, {2} elements found", pageNumber, pageSize, result.Count);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read all DriverRepositories from DB");
            string message = string.Format("Unable to read all driver repositories from DB");
            throw new Exception(message, e);
         }

         return result;

      }


      //Disable Base methods not working for this entity
      public new IEnumerable<DriverRepository> Find(Expression<Func<DriverRepository, bool>> filterPredicate, IEnumerable<Expression<Func<DriverRepository, object>>> includePredicates = null, int pageNumber = 0, int pageSize = 0)
      {
         throw new NotImplementedException("Find method with includepredicate is not available for Dryvre Repository, use  loadCapabilities and loadEventsMapping");
      }

      //Using lambda expressions filters have some limitations when queries are complicated, but for simple projects are very usefull
      public IEnumerable<DriverRepository> Find(System.Linq.Expressions.Expression<Func<DriverRepository, bool>> filterPredicate,
                              bool loadCapabilities = false,
                              bool loadEventsMapping = false,
                              int pageNumber = 0,
                              int pageSize = 0)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing DriverRepository Find with page number {0} and page size {1} {2}", pageNumber, pageSize, filterPredicate == null ? "" : "using filter predicates");

         List<DriverRepository> result;

         try
         {
            IQueryable<DriverRepository> qResult = mobjDbContext.Set<DriverRepository>();

            qResult = qResult.Where(filterPredicate);

            if (pageNumber > 0)
            {
               qResult = qResult.Skip((pageNumber - 1) * pageSize);
            }

            if (pageSize > 0)
            {
               qResult = qResult.Take(pageSize);
            }

            result = qResult.ToList();

            if (loadCapabilities || loadEventsMapping)
            {
               foreach (var item in result)
               {
                  if (loadCapabilities)
                  {
                     item.Capabilities = mobjDbContext.Set<DriverRepositoryStandardParameterLink>().Where(x => x.DriverRepositoryId == item.Id).ToList();
                  }

                  if (loadEventsMapping)
                  {
                     item.EventsMapping = mobjDbContext.Set<DriverRepositoryEventCatalog>().Where(x => x.DriverRepositoryId == item.Id).ToList();
                  }
               }
            }

            //TODO Trace
            mobjLoggerService.Info("Page {0} ({1} items per page) of DriverRepository find results list ({2}) retrived succesfully.", pageNumber, pageSize, filterPredicate == null ? "" : "using filter predicates");
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to search DriverRepository");
            string message = string.Format("Unable to search DriverRepository");
            throw new Exception(message, e);
         }
         return result;
      }

      #endregion

      #region Data Writing functions

      //Disable Base methods not working for this entity
      public new DriverRepository Update(DriverRepository entity)
      {
         throw new NotImplementedException("Update method without parameters is not available for Driver Repository, use updateDriverInformations, updateCapabilities and updateEventsMapping");
      }

      public bool Create(DriverRepository driverRepository, bool updateCapabilities = true, bool updateEventsMappings = true)
      {
         bool bolRet = false;

         //if (String.IsNullOrWhiteSpace(driverRepository.DriverName))
         //{
         //    throw new ConnectException(mobjDicSvc.XLate("Unable to save a driver without Driver Name"));
         //}


         //if (driverRepository.Stream == null || driverRepository.Stream.Length == 0)
         //{
         //    throw new ConnectException(mobjDicSvc.XLate("Unable to save a driver without file"));
         //}

         validateData(driverRepository, true);

         //Open transaction
         mobjDbContext.BeginTransaction();

         try
         {

            var objDriverRepo = mobjDbContext.Set<DriverRepository>();
            var objCapabilitiesRepo = mobjDbContext.Set<DriverRepositoryStandardParameterLink>();
            var objEventMappingRepo = mobjDbContext.Set<DriverRepositoryEventCatalog>();
            var objStdDeviceTyperepo = mobjDbContext.Set<StandardDeviceType>();
            List<DriverRepositoryStandardParameterLink> objListCapabilities = null;
            ////#4466
            //var driver = objDriverRepo.Where(p =>
            //   p.DriverName == driverRepository.DriverName && p.DriverVersion == driverRepository.DriverVersion).ToList();
            ////Check if a driver with the same name/version already exists
            //int intCount = driver.Count();
            //if (intCount == 0)
            //{
            //Create driver ID and version
            string strGuid = System.Guid.NewGuid().ToString();
            driverRepository.Id = strGuid;
            driverRepository.Version = 1;
            driverRepository.Current = true;


            //Serialize Remapped Events
            if (driverRepository.EventsMapping != null)
            {
               //Remove not remapped events from collection to mantain compatibility with old Configurator
               List<DriverRepositoryEventCatalog> objClearedEvents = new List<DriverRepositoryEventCatalog>();
               foreach (DriverRepositoryEventCatalog ec in driverRepository.EventsMapping)
               {
                  if (CheckEventIsRemapped(ec))
                  {
                     objClearedEvents.Add(ec);
                  }
                  driverRepository.RemappedEvents = UMSFrameworkParser.SerializeAsUMSRemappedEvents(objClearedEvents);
               }
            }

            objDriverRepo.Add(driverRepository);

            //Clone the capabilities. I'm going to use them later
            objListCapabilities = new List<DriverRepositoryStandardParameterLink>();

            //Set correct driverID to capabilities
            foreach (DriverRepositoryStandardParameterLink objSpl in driverRepository.Capabilities)
            {
               DriverRepositoryStandardParameterLink objNewSp = new DriverRepositoryStandardParameterLink();
               objNewSp.DeviceId = objSpl.DeviceId;
               objNewSp.DeviceText = objSpl.DeviceText;
               objNewSp.DeviceUnitText = objSpl.DeviceUnitText;
               objNewSp.DriverRepositoryId = objSpl.DriverRepositoryId;
               objNewSp.IsEnabled = objSpl.IsEnabled;
               objNewSp.MustBeSaved = objSpl.MustBeSaved;
               objNewSp.Sporadic = objSpl.Sporadic;
               objNewSp.StandardDeviceTypeId = objSpl.StandardDeviceTypeId;
               objNewSp.StandardParameterId = objSpl.StandardParameterId;
               objNewSp.StandardUnitId = objSpl.StandardUnitId;
               objNewSp.DriverRepositoryId = strGuid;
               objNewSp.StandardParameterIdAlias = objSpl.StandardParameterIdAlias;
               objListCapabilities.Add(objNewSp);

               objSpl.DriverRepositoryId = strGuid;
            }

            //Set correct driverID to events
            foreach (DriverRepositoryEventCatalog objEvent in driverRepository.EventsMapping)
            {
               objEvent.DriverRepositoryId = strGuid;
            }

            //Save capabilities and event mappings
            objCapabilitiesRepo.AddRange(objListCapabilities);
            objEventMappingRepo.AddRange(driverRepository.EventsMapping);

            mobjDbContext.SaveChanges();

            //Connect Plus actions
            DAS3Plus.DASAcquisitionTableManager objDrvRepoMgr = new DAS3Plus.DASAcquisitionTableManager(mobjDbContext, mobjDigCfg, mobjLoggerService);

            objDrvRepoMgr.CreateNewDASAcquisitionTable(driverRepository);

            //Commit transaction

            mobjDbContext.CommitTransaction();

            //Send UMS Message
            mobjMsgCtrMgr.SendDriverEdited(driverRepository.DriverName, driverRepository.Id, true, true, true, true);

            bolRet = true;
            //}
            //else
            //{
            //    string msg = $"Driver {driverRepository.DriverName} with version {driverRepository.DriverVersion} already exists";

            //    if (driver.FindIndex(x => x.Current == true) == -1)
            //    {
            //        msg += ". It's no more active!!";
            //    }

            //    throw new ConnectException(mobjDicSvc.XLate(msg)); ;
            //}
         }
         catch (Exception e)
         {
            if (e is ConnectException)
            {
               throw;
            }
            string errMsg = "Error creating driver repository ";
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return bolRet;
      }

      private bool CheckEventIsRemapped(DriverRepositoryEventCatalog evt)
      {
         bool ret = false;
         if (evt.NewClass != -1 && evt.NewLevel != -1 || !string.IsNullOrEmpty(evt.TextENG) || !string.IsNullOrEmpty(evt.TextENGShort)
            || !string.IsNullOrEmpty(evt.TextUser) || !string.IsNullOrEmpty(evt.TextUserShort))
         {
            ret = true;
         }
         return ret;
      }

      public DriverRepository Update(DriverRepository driverRepository, bool FileHasChanged, bool updateCapabilities = true, bool updateEventsMapping = true)
      {
         bool capabilitiesToBeSavedChanged = false;
         //TODO Trace
         mobjLoggerService.Info("Updating DriverRepository with id {0} and version {1}", driverRepository.Id, driverRepository.Version);

         validateData(driverRepository, false);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            DriverRepository newEntity = null;
            var repository = mobjDbContext.Set<DriverRepository>();

            DriverRepository loadedEntity = repository.SingleOrDefault(x => x.Id == driverRepository.Id && x.Current == true);

            var bNameOrVersionChanged = false;
            if (loadedEntity.DriverName != driverRepository.DriverName || loadedEntity.DriverVersion != driverRepository.DriverVersion)
            {
               bNameOrVersionChanged = true;
            }
            //mobjDbContext.Entry(loadedEntity).Load(x => x.Capabilities);
            loadedEntity.Capabilities = mobjDbContext.Set<DriverRepositoryStandardParameterLink>().Where(p => p.DriverRepositoryId == driverRepository.Id).ToList();

            //DriverRepository loadedEntity = repository.Include(e => e.Capabilities).Include(e => e.EventsMapping).SingleOrDefault(x => x.Id == driverRepository.Id && x.Current == true);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update driver repository with id {0}; driver repository not found.", driverRepository.Id));
            }

            if (driverRepository.Version != loadedEntity.Version)
            {
               throw new Exception(string.Format("Unable to update driver repository with id {0}; driver repository version ({1}) is different from expected ({2}).", driverRepository.Id, loadedEntity.Version, driverRepository.Version));
            }

            //Check if something relevant is changed
            bool genericChanged = driverRepository.DriverName != loadedEntity.DriverName ||
               driverRepository.DriverVersion != loadedEntity.DriverVersion ||
               driverRepository.EntryExe != loadedEntity.EntryExe ||
               driverRepository.ComToRegister != loadedEntity.ComToRegister ||
               driverRepository.IsWrapper != loadedEntity.IsWrapper ||
               driverRepository.Manufacturer != loadedEntity.Manufacturer ||
               driverRepository.Device != loadedEntity.Device ||
               driverRepository.RunAsDLL != loadedEntity.RunAsDLL ||
               driverRepository.Model != loadedEntity.Model;


            if (FileHasChanged)
            {
               genericChanged = true;
            }

            //Check if format string is changed
            bool formatChanged = driverRepository.FormatStyle != loadedEntity.FormatStyle;

            bool capabilitiesChanged = false;

            if (updateCapabilities)
            {



               //If number of capabilities is changed something is changed
               capabilitiesChanged = (driverRepository.Capabilities.Count() != loadedEntity.Capabilities.Count());
               //Otherwise....
               if (!capabilitiesChanged)
               {



                  foreach (DriverRepositoryStandardParameterLink capability in driverRepository.Capabilities)
                  {
                     //If one capability does not match with an existing one something is changed

                     if (!loadedEntity.Capabilities.Any(x =>
                        x.DeviceId == capability.DeviceId &&
                        x.DeviceText == capability.DeviceText &&
                        x.DeviceUnitText == capability.DeviceUnitText &&
                        //x.DriverRepositoryId == capability.DriverRepositoryId &&
                        x.IsEnabled == capability.IsEnabled &&
                        x.Sporadic == capability.Sporadic &&
                        x.StandardDeviceTypeId == capability.StandardDeviceTypeId &&
                        x.StandardParameterId == capability.StandardParameterId &&
                        x.StandardUnitId == capability.StandardUnitId &&
                        x.MustBeSaved == capability.MustBeSaved
                        ))
                     {
                        capabilitiesChanged = true;
                        break;
                     }
                  }
               }
            }



            //Create new record for updated entity

            newEntity = driverRepository.CreateUpdatedClone();
            //Driver file content has not been posted, so we need to retrieve the same as before
            if (driverRepository.Stream == null)
            {
               newEntity.Stream = loadedEntity.Stream;
               newEntity.LastStreamUpdate = loadedEntity.LastStreamUpdate;
            }



            //Serialize Remapped Events
            if (newEntity.EventsMapping != null)
            {
               //Remove not remapped events from collection to mantain compatibility with old Configurator
               List<DriverRepositoryEventCatalog> objClearedEvents = new List<DriverRepositoryEventCatalog>();
               foreach (DriverRepositoryEventCatalog ec in newEntity.EventsMapping)
               {
                  if (CheckEventIsRemapped(ec))
                  {
                     objClearedEvents.Add(ec);
                  }
               }
               newEntity.RemappedEvents = UMSFrameworkParser.SerializeAsUMSRemappedEvents(objClearedEvents);
            }

            repository.Add(newEntity);

            //Set current record as updated
            loadedEntity.Current = false;
            loadedEntity.ValidToDate = DateTime.Now;

            //mobjDbContext.SaveChanges();

            //Remove old Capabilities and eventmapping
            var parLinkRepo = mobjDbContext.Set<DriverRepositoryStandardParameterLink>();
            var objListLinks = parLinkRepo.Where(p => p.DriverRepositoryId == driverRepository.Id).ToList();

            //var objSPar = mobjDbContext.Set<StandardParameter>();
            ////var oldStdParLink = parLinkRepo.Include(p => p.StandardParameter).Where(p => p.DriverRepositoryId == driverRepository.Id).ToList();
            //var oldStdParLink = parLinkRepo
            //   .GroupJoin(objSPar.AsQueryable(), l => l.StandardParameterId, h => h.Id, (l, h) => new { l, h })
            //   //.Include(p => p.StandardParameter)
            //   .Where(p => p.l.DriverRepositoryId == driverRepository.Id)
            //   .Select(@t => new DriverRepositoryStandardParameterLink
            //   {
            //      DriverRepositoryId = @t.l.DriverRepositoryId,
            //      DeviceId = @t.l.DeviceId,
            //      DeviceText = @t.l.DeviceText,
            //      DeviceUnitText = @t.l.DeviceUnitText,
            //      IsEnabled = @t.l.IsEnabled,
            //      MustBeSaved = @t.l.MustBeSaved,
            //      StandardParameterId = @t.l.StandardParameterId,
            //      StandardParameterIdAlias = @t.l.StandardParameterIdAlias,
            //      StandardParameter = null, //@t.h.FirstOrDefault(),
            //      StandardUnit = null,
            //      StandardDeviceType = null

            //   }).ToList();
            //.Where(p => p.DriverRepositoryId == driverRepository.Id)
            //.ToList();

            foreach (DriverRepositoryStandardParameterLink drspl in objListLinks)
            {
               //var st = drspl.CalculateHash();
               try
               {
                  parLinkRepo.Remove(drspl);
               }
               //Pain and Fear
               catch
               {
                  //throw;
               }

            }
            //parLinkRepo.RemoveRange(objListLinks);
	    var evtCatRepo = mobjDbContext.Set<DriverRepositoryEventCatalog>();
            var oldEvtCat = evtCatRepo.Where(p => p.DriverRepositoryId == driverRepository.Id).ToList();
            evtCatRepo.RemoveRange(oldEvtCat);

            List<DriverRepositoryStandardParameterLink> editedStParLink = new List<DriverRepositoryStandardParameterLink>();
            foreach (DriverRepositoryStandardParameterLink d in driverRepository.Capabilities.ToList())
            {
               var driverRepositoryStandardParameterLink = (DriverRepositoryStandardParameterLink)d.Clone();
               if (driverRepositoryStandardParameterLink.StandardParameter.DataType == null
                   || !driverRepositoryStandardParameterLink.IsEnabled
                   )
               {
                  driverRepositoryStandardParameterLink.MustBeSaved = false;
               }
               editedStParLink.Add(driverRepositoryStandardParameterLink);
            }
            //editedStParLink =driverRepository.Capabilities.ToList();

            if (updateCapabilities)
            {
               //Make sure repository Id is correctly set and set to null related objects
               foreach (DriverRepositoryStandardParameterLink capability in driverRepository.Capabilities)
               {
                  capability.DriverRepositoryId = driverRepository.Id;
                  capability.StandardDeviceType = null;
                  capability.StandardParameter = null;

                  capability.StandardUnit = null;
               }

               objListLinks = driverRepository.Capabilities.OrderBy(o => o.StandardParameterId).ToList();
            }

            if (updateEventsMapping)
            {
               //Make sure repository Id is correctly set
               foreach (DriverRepositoryEventCatalog eventMapping in driverRepository.EventsMapping)
               {
                  eventMapping.DriverRepositoryId = driverRepository.Id;

               }
               oldEvtCat = driverRepository.EventsMapping.ToList();
            }


            foreach (DriverRepositoryStandardParameterLink drspl in objListLinks)
            {
               var st = drspl.CalculateHash();
               try
               {
                  parLinkRepo.Add(drspl);
               }
               catch
               {
                  throw;
               }

            }

            //parLinkRepo.AddRange(oldStdParLink);
            evtCatRepo.AddRange(oldEvtCat);


            mobjDbContext.SaveChanges();


            //ConnectPlus Actions
            DAS3Plus.DASAcquisitionTableManager objDrvRepoMgr = new DAS3Plus.DASAcquisitionTableManager(mobjDbContext, mobjDigCfg, mobjLoggerService);

            var objParamCurrentlySaved = objDrvRepoMgr.GetAcquisitionParameter(driverRepository.Id);

            //newEntity.Capabilities = oldStdParLink;
            newEntity.Capabilities = editedStParLink;

            //Check if capabilities to be saved are changed
            var objListCapToBeSaved = editedStParLink.Where(p => p.MustBeSaved).Select(p => new { StandardParameterId = p.StandardParameterId, StandardParameterIdAlias = p.StandardParameterIdAlias }).ToList();
            capabilitiesToBeSavedChanged = !objListCapToBeSaved.OrderBy(x => x.StandardParameterId).ThenBy(x => x.StandardParameterIdAlias)
               .SequenceEqual(objParamCurrentlySaved.ToList().Select(p => new { StandardParameterId = p.Item1, StandardParameterIdAlias = p.Item2 })
                                             .OrderBy(x => x.StandardParameterId).ThenBy(x => x.StandardParameterIdAlias));


            if (genericChanged || capabilitiesToBeSavedChanged)
            {
               objDrvRepoMgr.CreateNewDASAcquisitionTable(newEntity);
            }

            //Send UMS Message
            mobjMsgCtrMgr.SendDriverEdited(driverRepository.DriverName, driverRepository.Id, (genericChanged || updateEventsMapping), formatChanged, capabilitiesChanged, capabilitiesToBeSavedChanged || bNameOrVersionChanged);
            if (executeClose) mobjDbContext.CommitTransaction();


            return newEntity;

         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            if (e is ConnectException)
            {
               throw;
            }
            mobjLoggerService.ErrorException(e, "Error updating driver repository with id {0}", driverRepository.Id);
            string message = string.Format("Error updating driver repository with id {0}", driverRepository.Id);
            throw new Exception(message, e);
         }
      }

      //2020-11-17 Questo Metodo non è utilizzato mai, l'ho scritto cercando di dare un senso al metodo "Update" che fa un groviglio di query rileggendo, aggiungendo e rimuovendo sempre gli stessi dati.
      //Nella speranza che un giorno ci sentiremo abbastanza forti e coraggiosi da guardare l'oscurità negli occhi e ripulire quel casino, lascio la mia eredità, nel caso il mio lavoro possa essere utile a qualcuno.
      //IL METODO NON E' STATO TESTATO
      public DriverRepository CleanedUpdate(DriverRepository driverRepository, bool FileHasChanged, bool updateCapabilities = true, bool updateEventsMapping = true)
      {
         //TODO Trace
         mobjLoggerService.Info("Updating DriverRepository with id {0} and version {1}", driverRepository.Id, driverRepository.Version);

         validateData(driverRepository, false);

         bool executeClose = mobjDbContext.BeginTransaction();

         try
         {
            var repository = mobjDbContext.Set<DriverRepository>();
            var parLinkRepo = mobjDbContext.Set<DriverRepositoryStandardParameterLink>();
            var evtCatRepo = mobjDbContext.Set<DriverRepositoryEventCatalog>();

            //Load stored Driver Repository
            DriverRepository loadedEntity = repository.SingleOrDefault(x => x.Id == driverRepository.Id && x.Current == true);

            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update driver repository with id {0}; driver repository not found.", driverRepository.Id));
            }

            if (driverRepository.Version != loadedEntity.Version)
            {
               throw new Exception(string.Format("Unable to update driver repository with id {0}; driver repository version ({1}) is different from expected ({2}).", driverRepository.Id, loadedEntity.Version, driverRepository.Version));
            }

            //Load stored DriverRepository Event catalog            
            IEnumerable<DriverRepositoryEventCatalog> loadedEvtCat = evtCatRepo.Where(p => p.DriverRepositoryId == driverRepository.Id).ToList();
            //Load stored DriverRepository Capabilities            
            IEnumerable<DriverRepositoryStandardParameterLink> loadedCapabilities = parLinkRepo.Where(p => p.DriverRepositoryId == driverRepository.Id).ToList();

            //Check if drivername or driverversion is changed
            bool bNameOrVersionChanged = (loadedEntity.DriverName != driverRepository.DriverName || loadedEntity.DriverVersion != driverRepository.DriverVersion);

            //Check if something relevant is changed
            bool genericChanged =
               driverRepository.DriverName != loadedEntity.DriverName ||
               driverRepository.DriverVersion != loadedEntity.DriverVersion ||
               driverRepository.EntryExe != loadedEntity.EntryExe ||
               driverRepository.ComToRegister != loadedEntity.ComToRegister ||
               driverRepository.IsWrapper != loadedEntity.IsWrapper ||
               driverRepository.Manufacturer != loadedEntity.Manufacturer ||
               driverRepository.Device != loadedEntity.Device ||
               driverRepository.RunAsDLL != loadedEntity.RunAsDLL ||
               driverRepository.Model != loadedEntity.Model ||
               FileHasChanged;

            //Check if format string is changed
            bool formatChanged = driverRepository.FormatStyle != loadedEntity.FormatStyle;

            bool capabilitiesChanged = false;
            if (updateCapabilities)
            {
               //If number of capabilities is changed something is changed
               capabilitiesChanged = (driverRepository.Capabilities.Count() != loadedCapabilities.Count());
               //Otherwise....
               if (!capabilitiesChanged)
               {
                  foreach (DriverRepositoryStandardParameterLink capability in driverRepository.Capabilities)
                  {
                     //If one capability does not match with an existing one something is changed
                     if (!loadedCapabilities.Any(x =>
                           x.DeviceId == capability.DeviceId &&
                           x.DeviceText == capability.DeviceText &&
                           x.DeviceUnitText == capability.DeviceUnitText &&
                           //x.DriverRepositoryId == capability.DriverRepositoryId &&
                           x.IsEnabled == capability.IsEnabled &&
                           x.Sporadic == capability.Sporadic &&
                           x.StandardDeviceTypeId == capability.StandardDeviceTypeId &&
                           x.StandardParameterId == capability.StandardParameterId &&
                           x.StandardUnitId == capability.StandardUnitId &&
                           x.MustBeSaved == capability.MustBeSaved)
                        )
                     {
                        capabilitiesChanged = true;
                        break;
                     }
                  }
               }
            }

            //Create new record for updated entity
            DriverRepository newEntity = driverRepository.CreateUpdatedClone();

            //Driver file content has not been posted, so we need to retrieve the same as before
            if (driverRepository.Stream == null)
            {
               newEntity.Stream = loadedEntity.Stream;
               newEntity.LastStreamUpdate = loadedEntity.LastStreamUpdate;
            }

            //Serialize Remapped Events
            if (newEntity.EventsMapping != null)
            {
               //Remove not remapped events from collection to mantain compatibility with old Configurator
               List<DriverRepositoryEventCatalog> objClearedEvents = new List<DriverRepositoryEventCatalog>();
               foreach (DriverRepositoryEventCatalog ec in newEntity.EventsMapping)
               {
                  if (CheckEventIsRemapped(ec))
                  {
                     objClearedEvents.Add(ec);
                  }
               }
               newEntity.RemappedEvents = UMSFrameworkParser.SerializeAsUMSRemappedEvents(objClearedEvents);
            }

            repository.Add(newEntity);

            //Set current record as updated
            loadedEntity.Current = false;
            loadedEntity.ValidToDate = DateTime.Now;

            if (updateCapabilities)
            {
               //Remove existing
               parLinkRepo.RemoveRange(loadedCapabilities);
               //Make sure repository Id is correctly set and set to null related objects
               foreach (DriverRepositoryStandardParameterLink capability in driverRepository.Capabilities)
               {
                  capability.DriverRepositoryId = driverRepository.Id;
                  capability.StandardDeviceType = null;
                  capability.StandardParameter = null;

                  capability.StandardUnit = null;

                  //Not mapped in EF used only for next statements CreateNewDASAcquisitionTable
                  capability.MustBeSaved = capability.StandardParameter.DataType != null && capability.IsEnabled;
               }
               //Add received in entity configuration
               loadedCapabilities = driverRepository.Capabilities.OrderBy(o => o.StandardParameterId);
               parLinkRepo.AddRange(loadedCapabilities);
            }

            if (updateEventsMapping)
            {
               evtCatRepo.RemoveRange(loadedEvtCat);
               //Make sure repository Id is correctly set
               foreach (DriverRepositoryEventCatalog eventMapping in driverRepository.EventsMapping)
               {
                  eventMapping.DriverRepositoryId = driverRepository.Id;

               }
               evtCatRepo.AddRange(driverRepository.EventsMapping);
            }

            mobjDbContext.SaveChanges();

            //ConnectPlus Actions

            // † In Loving Memory of Dependency Injection †
            DAS3Plus.DASAcquisitionTableManager objDrvRepoMgr = new DAS3Plus.DASAcquisitionTableManager(mobjDbContext, mobjDigCfg, mobjLoggerService);

            var objParamCurrentlySaved = objDrvRepoMgr.GetAcquisitionParameter(driverRepository.Id);

            //Check if capabilities to be saved are changed
            var objListCapToBeSaved = driverRepository.Capabilities.Where(p => p.MustBeSaved).Select(p => new { StandardParameterId = p.StandardParameterId, StandardParameterIdAlias = p.StandardParameterIdAlias }).ToList();

            bool capabilitiesToBeSavedChanged = !objListCapToBeSaved.OrderBy(x => x.StandardParameterId).ThenBy(x => x.StandardParameterIdAlias)
                   .SequenceEqual(objParamCurrentlySaved.ToList().Select(p => new { StandardParameterId = p.Item1, StandardParameterIdAlias = p.Item2 })
                                                 .OrderBy(x => x.StandardParameterId).ThenBy(x => x.StandardParameterIdAlias));

            if (genericChanged || capabilitiesToBeSavedChanged)
            {
               newEntity.Capabilities = loadedCapabilities.ToList();
               objDrvRepoMgr.CreateNewDASAcquisitionTable(newEntity);
            }
            //End ConnectPlus Actions

            //Send UMS Message
            mobjMsgCtrMgr.SendDriverEdited(driverRepository.DriverName, driverRepository.Id, (genericChanged || updateEventsMapping), formatChanged, capabilitiesChanged, capabilitiesToBeSavedChanged || bNameOrVersionChanged);

            if (executeClose) mobjDbContext.CommitTransaction();

            return newEntity;
         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            if (e is ConnectException)
            {
               throw;
            }
            mobjLoggerService.ErrorException(e, "Error updating driver repository with id {0}", driverRepository.Id);
            string message = string.Format("Error updating driver repository with id {0}", driverRepository.Id);
            throw new Exception(message, e);
         }
      }

      /// <summary>
      /// Disable a driver repository. 
      /// </summary>
      public void Remove(string driverRepositoryId)
      {

         //TODO Trace
         mobjLoggerService.Info("Disabling DriverRepository with id {0}", driverRepositoryId);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {
            var repository = mobjDbContext.Set<DriverRepository>();

            DriverRepository driverRepository = repository.SingleOrDefault(x => x.Id == driverRepositoryId && x.Current);
            if (driverRepository == null)
            {
               throw new Exception(string.Format("Unable to disable personnel with id {0}; personnel not found.", driverRepositoryId));
            }

            //Set current record as disabled
            driverRepository.Current = false;
            driverRepository.ValidToDate = DateTime.Now;

            //Remove all linked drivers
            var driversRepository = mobjDbContext.Set<DeviceDriver3>();
            var driverBedLinksRepository = mobjDbContext.Set<DeviceDriver3BedLink>();
            List<DeviceDriver3> objDevDrvList = driversRepository.Where(x => x.IdDriverRepository == driverRepositoryId).ToList();
            if (objDevDrvList != null && objDevDrvList.Count > 0)
            {
               foreach (DeviceDriver3 objDev in objDevDrvList)
               {
                  List<DeviceDriver3BedLink> objDevDrvBedLink = driverBedLinksRepository.Where(x => x.DeviceDriverId == objDev.Id).ToList();
                  if (objDevDrvBedLink != null && objDevDrvBedLink.Count > 0)
                  {
                     driverBedLinksRepository.RemoveRange(objDevDrvBedLink);
                  }
               }
               driversRepository.RemoveRange(objDevDrvList);

            }

            mobjDbContext.SaveChanges();

            //Send notification to Message Center
            mobjMsgCtrMgr.SendDriverDeleted(driverRepository.DriverName, driverRepository.Id);

            if (executeClose) mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("DriverRepository with id {0} disabled succesfully", driverRepositoryId);
         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error disabling DriverRepository with id {0}", driverRepositoryId);
            string message = string.Format("Error disabling DriverRepository with id {0}", driverRepositoryId);
            throw new Exception(message, e);
         }
      }


      private void CreateStandardParameterLinks(string driverRepositoryId, IEnumerable<DriverRepositoryStandardParameterLink> parameterLinks, DigistatDBContext mobjDbContext)
      {
         //Make sure repository Id is correctly set
         foreach (DriverRepositoryStandardParameterLink parameterLink in parameterLinks)
         {
            parameterLink.DriverRepositoryId = driverRepositoryId;
         }

         mobjDbContext.Set<DriverRepositoryStandardParameterLink>().AddRange(parameterLinks);
      }

      private void CreateEventCatalogs(string driverRepositoryId, IEnumerable<DriverRepositoryEventCatalog> eventCatalogs, DigistatDBContext mobjDbContext)
      {
         //Make sure repository Id is correctly set
         foreach (DriverRepositoryEventCatalog eventCatalog in eventCatalogs)
         {
            eventCatalog.DriverRepositoryId = driverRepositoryId;
         }

         mobjDbContext.Set<DriverRepositoryEventCatalog>().AddRange(eventCatalogs);
      }

      #endregion

      #region Das Driver Update



      #endregion

      #region Files management

      public DasDrivers.CachedFile DownloadDriver(string driverRepositoryId)
      {

         //TODO Trace
         mobjLoggerService.Info("Reading driver of DriverRepository with id {0} for download", driverRepositoryId);

         try
         {

            DriverRepository loadedEntity = this.Get(driverRepositoryId);

            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to download driver with repository id {0}; driver repository not found.", driverRepositoryId));
            }

            string fileNamePattern = "Driver {0} v{1} - {2} {3}.zip";

            //var driverManager = new DasDrivers.DriverManager(currentUserIdentifier, driverRepositoryId);
            byte[] compressedDriverFolder = mobjDriverManager.PrepareFilesForDownload(loadedEntity, driverRepositoryId);

            return new DasDrivers.CachedFile(string.Format(fileNamePattern, loadedEntity.DriverName, loadedEntity.DriverVersion, loadedEntity.Manufacturer, loadedEntity.Model), compressedDriverFolder);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error downloading driver for repository with id {0}", driverRepositoryId);
            string message = string.Format("Error downloading driver for repository with id {0}", driverRepositoryId);
            throw new Exception(message, e);
         }

      }

      //public DriverRepository UploadDriver(string driverRepositoryId, IEnumerable<CachedFile> driverFiles)
      //{
      //   if (driverFiles.Count() == 0)
      //   {
      //      throw new Exception(string.Format("Unable to cache driver files for repository with id {0}; No files received.", driverRepositoryId));
      //   }

      //   //TODO Trace
      //   mobjLoggerService.Info("Caching driver for DriverRepository with id {0}", driverRepositoryId);

      //   try
      //   {
      //      //Save driverfiles in cache
      //      mobjDriverManager.CacheDriver(driverFiles, driverRepositoryId);

      //      DriverRepository loadedEntity = this.Get(driverRepositoryId);

      //      if (loadedEntity == null)
      //      {
      //         throw new Exception(string.Format("Unable to cache driver files for repository with id {0}; driver repository not found.", driverRepositoryId));
      //      }

      //      //
      //      //var driverManager = new DasDrivers.DriverManager(currentUserIdentifier, driverRepositoryId);



      //      //Exported file
      //      if (driverFiles.Count() == 1 && System.IO.Path.GetExtension(driverFiles.First().Name).ToLower() == ".bin")
      //      {
      //         mobjDriverManager.UpdateDriverInfoUsingIndex(loadedEntity, driverFiles.First());
      //      }
      //      else
      //      {
      //         mobjDriverManager.UpdateDriverInfoUsingCachedDriver(loadedEntity);
      //      }

      //      return loadedEntity;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Error caching driver files for repository with id {0}", driverRepositoryId);
      //      string message = string.Format("Error caching driver files for repository with id {0}", driverRepositoryId);
      //      throw new Exception(message, e);
      //   }
      //}





      public DriverRepository UploadDriver(string driverRepositoryId, DriverRepository entity, IEnumerable<CachedFile> driverFiles, bool keepCapabilities, bool keepFormatString, bool keepRemappedEvents)
      {
         string message = string.Empty;
         if (driverFiles.Count() == 0)
         {
            throw new Exception(string.Format("Unable to cache driver files for repository with id {0}; No files received.", driverRepositoryId));
         }

         //TODO Trace
         mobjLoggerService.Info("Caching driver for DriverRepository with id {0}", driverRepositoryId);

         try
         {
            //Save driverfiles in cache
            mobjDriverManager.CacheDriver(driverFiles, driverRepositoryId);
            ICollection<DriverRepositoryStandardParameterLink> objOldCapabilities = entity.Capabilities;
            ICollection<DriverRepositoryEventCatalog> objOldEvents = entity.EventsMapping;
            string oldFormatStyle = entity.FormatStyle;


            if (entity == null)
            {
               throw new Exception(string.Format("Unable to cache driver files for repository with id {0}; driver repository not found.", driverRepositoryId));
            }

            if (driverFiles.Count() == 1 && System.IO.Path.GetExtension(driverFiles.First().Name).ToLower() == ".bin")
            {
               //BIN file
               entity.IsBinFile = true;
               mobjDriverManager.UpdateDriverInfoUsingIndex(driverRepositoryId, entity, driverFiles.First());
            }
            else
            {
               //Other files
               entity.IsBinFile = false;
               mobjDriverManager.UpdateDriverInfoUsingCachedDriver(driverRepositoryId, entity);

               //Clear collection from duplicates
               if (entity.Capabilities != null)
               {
                  entity.Capabilities = entity.Capabilities.GroupBy(p => new { p.DriverRepositoryId, p.StandardParameterId, p.StandardUnitId, p.DeviceId })
                     .Select(grp => grp.First()).ToList();
               }


               if (keepCapabilities)
               {
                  if (entity.Id == GetCdssGuid())
                  {
                     entity.Capabilities = objOldCapabilities;
                  }
                  foreach (DriverRepositoryStandardParameterLink objCapability in entity.Capabilities)
                  {
                     var objOldCapFound = objOldCapabilities.Where(p => p.StandardParameterId == objCapability.StandardParameterId
                        && p.StandardUnitId == objCapability.StandardUnitId).FirstOrDefault();
                     if (objOldCapFound != null)
                     {
                        objCapability.IsEnabled = objOldCapFound.IsEnabled;
                        objCapability.MustBeSaved = objOldCapFound.MustBeSaved;
                     }
                  }

               }


               if (keepFormatString)
               {
                  entity.FormatStyle = oldFormatStyle;
               }

               if (keepRemappedEvents)
               {
                  foreach (DriverRepositoryEventCatalog objEvtCatalog in entity.EventsMapping)
                  {
                     var objOldEvtFound = objOldEvents.Where(p => p.Id == objEvtCatalog.Id).FirstOrDefault();
                     if (objOldEvtFound != null)
                     {
                        objEvtCatalog.NewClass = objOldEvtFound.NewClass;
                        objEvtCatalog.NewLevel = objOldEvtFound.NewLevel;
                        objEvtCatalog.TextENG = objOldEvtFound.TextENG;
                        objEvtCatalog.TextENGShort = objOldEvtFound.TextENGShort;
                        objEvtCatalog.TextUser = objOldEvtFound.TextUser;
                        objEvtCatalog.TextUserShort = objOldEvtFound.TextUserShort;
                     }
                  }
               }


            }
            entity.FileCount = driverFiles.Count();

            return entity;

         }
         catch (ConnectDriverLoadException ex)
         {
            string errMsg = string.Empty;
            switch (ex.ErrorType)
            {
               case DriverLoadErrorType.CommunicationTimeout:
                  errMsg = "A timeout occurred while waiting for driver data. This may be due to network issues";
                  break;
               case DriverLoadErrorType.EmptyMessageReceived:
                  errMsg = "No data received from driver. This may be due to network issues";
                  break;
               case DriverLoadErrorType.NoPortAvailable:
                  errMsg = "No port available for TCP Listener. " + ex.Message;
                  break;
               case DriverLoadErrorType.IncorrectMessageReceived:
                  errMsg = "Incorrect message received from Driver. Cannot parse content.";
                  break;
            }
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error caching driver files for repository with id {0}", driverRepositoryId);
            message = string.Format("Error caching driver files for repository with id {0}", driverRepositoryId);
            throw new Exception(message, e);
         }
      }

      public CachedFile ExportDriver(string driverRepositoryId)
      {

         //TODO Trace
         mobjLoggerService.Info("Reading driver of DriverRepository with id {0} for export ", driverRepositoryId);

         try
         {

            DriverRepository loadedEntity = this.Get(driverRepositoryId, true, true);

            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to export driver with repository id {0}; driver repository not found.", driverRepositoryId));
            }

            //Build driver Header
            //var driverManager = new DasDrivers.DriverManager(currentUserIdentifier, driverRepositoryId);

            return mobjDriverManager.CreateExportArchive(loadedEntity);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error exporting driver for repository with id {0}", driverRepositoryId);
            string message = string.Format("Error exporting driver for repository with id {0}", driverRepositoryId);
            throw new Exception(message, e);
         }

      }

      #endregion

   }
}
