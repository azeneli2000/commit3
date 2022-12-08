using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.UMSLegacy;
using Configurator.Std.BL.Hubs;

namespace Configurator.Std.BL
{
   public class DeviceDrivers3Manager : DalManagerBase<DeviceDriver3>, IDeviceDrivers3Manager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDeviceDrivers3BedLinksManager mobjBedLinksMgr;

      public DeviceDrivers3Manager(DigistatDBContext context, IDeviceDrivers3BedLinksManager bedLinksMgr, IMessageCenterManager msgCtrMgr, ILoggerService loggerService)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjBedLinksMgr = bedLinksMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;

         this.BeforeSave += BeforeSaveHandler;
         this.BeforeUpdate += BeforeUpdateHandler;

         this.AfterSave += AfterSaveHandler;
         this.AfterUpdate += AfterUpdateHandler;

      }

      #endregion

      #region private functions

      //  private bool CheckDriverOnSamePort()
      //  {
      //     bool bolReturn = false;
      //     if (!string.IsNullOrEmpty(txtSmartCableId.Text))
      //     {
      //        UMS.Framework.Data.DBManager objManager = new UMS.Framework.Data.DBManager(
      //          UMS.Framework.Data.DBProvider.SqlServer, DASConfigurator.LogicLayer.ConnectionString);
      //        objManager.Open();
      //        objManager.CreateParameters(1);
      //        if (menuEditState == EditState.AddNew)
      //        {
      //           objManager.SetParameter(0, "@deviceDriverId", DBNull.Value);
      //        }
      //        else
      //        {
      //           objManager.SetParameter(0, "@deviceDriverId", mobjDriverItem.ID);
      //        }

      //        object objReturn = objManager.ExecuteScalar(CommandType.Text, "select 1 from DeviceDriverNew where " +
      //           "(dvd_ID <> @deviceDriverId OR @deviceDriverId is null) and dvd_CommConfiguration like '%ConnectionType=\"1\"%' and " +
      //"dvd_CommConfiguration like '%ComPort=\"" + Int32.Parse(UMS.Framework.UMSUtility.ExtractNumberFromString((string)cboSerialPort.SelectedItem)).ToString() + "\"%' and " +
      //"dvd_ComputerName = '" + txtHostname.Text + "' and dvd_AutoStartDriver = 1");

      //        objManager.Close();
      //        objManager.Dispose();

      //        if (objReturn != null && objReturn != DBNull.Value)
      //        {
      //           bolReturn = Convert.ToBoolean(objReturn);
      //        }
      //     }
      //     return bolReturn;
      //  }

      private void validateData(DeviceDriver3 deviceDriver)
      {


         if (string.IsNullOrWhiteSpace(deviceDriver.ComputerName) && string.IsNullOrWhiteSpace(deviceDriver.CommConfigurationObject.SmartCableId))
         {
            throw new ArgumentException("Unable to save device driver with empty Hostname (and SmartCableId)", "Hostname");
         }

         if (!UMSFrameworkParser.CheckConnectionTypeValue(deviceDriver.CommConfigurationObject.ConnectionType))
         {
            throw new ArgumentException("Unable to save device driver with unrecognized ConnectionType", "ConnectionType");
         }

         //DriverType SingleBed and more than one bed selected
         //2019/05/17: like required is possible to save without bed selected
         //if (deviceDriver.IsSinglebedDriverType() && deviceDriver.BedLinks.Count(x => x.DriverEnabled) != 1)
         if (deviceDriver.IsSinglebedDriverType() && deviceDriver.BedLinks.Count(x => x.DriverEnabled) > 1)
         {
            throw new ArgumentException("SingleBed device driver requires one (and only one) BedLink enabled", "");
         }

         //2019/05/17: like required is possible to save without bed selected
         //if (deviceDriver.IsMultibedDriverType() && deviceDriver.BedLinks.Count(x => x.DriverEnabled) == 0)
         //{
         //   throw new ArgumentException("MultiBed device driver requires one or more BedLink enabled", "BedLinks");
         //}


         var repository = mobjDbContext.Set<DeviceDriver3>();

         if (deviceDriver.IsRs232ConnectionType() && deviceDriver.AutoStartDriver)
         {

            //Prevent duplications
            FormattableString sql = $"select dvd_ID from DeviceDriverNew where (dvd_ID <> {deviceDriver.Id}) and dvd_CommConfiguration like '%ConnectionType=\"{deviceDriver.CommConfigurationObject.ConnectionType}\"%' and dvd_CommConfiguration like '%ComPort=\"{deviceDriver.CommConfigurationObject.ComPort}\"%' and dvd_ComputerName = '{deviceDriver.CommConfigurationObject.Hostname}' and dvd_AutoStartDriver = 1";
            int? existingId = repository.FromSqlRaw(sql.ToString()).Select(x => x.Id).SingleOrDefault();
            if (existingId.HasValue && existingId != 0)
            {
               throw new ArgumentException(string.Format("Unable to crate device driver using serial port {0}; device driver using the same port already exists.", deviceDriver.CommConfigurationObject.ComPort, "ComPort"));
            }
         }

         if (deviceDriver.IsRs232ConnectionType() && !string.IsNullOrWhiteSpace(deviceDriver.CommConfigurationObject.SmartCableId))
         {
            //Prevent duplications
            FormattableString sql = $"select dvd_ID from DeviceDriverNew where (dvd_ID <> {deviceDriver.Id}) and dvd_CommConfiguration like '%ConnectionType=\"{deviceDriver.CommConfigurationObject.ConnectionType}\"%' and dvd_CommConfiguration like '%SmartCableId=\"{deviceDriver.CommConfigurationObject.SmartCableId}\"%'";
            int? existingId = repository.FromSqlRaw(sql.ToString()).Select(x => x.Id).SingleOrDefault();
            if (existingId.HasValue && existingId != 0)
            {
               throw new ArgumentException(string.Format("Unable to crate driver using SmartCable Id {0}; device driver using the same SmartCable Id already exists.", deviceDriver.CommConfigurationObject.SmartCableId), "SmartCableId");
            }
         }
      }

      private bool bedConfigChanged(DeviceDriver3 original, DeviceDriver3 modified)
      {
         //If number of capabilities is changed something is changed
         bool bedsChanged = (original.BedLinks.Count() != modified.BedLinks.Count());
         //Otherwise....
         if (!bedsChanged)
         {
            foreach (DeviceDriver3BedLink b in original.BedLinks)
            {
               //If one capability does not match with an existing one something is changed

               if (!modified.BedLinks.Any(x =>
                  x.BedId == b.BedId
                  ))
               {
                  bedsChanged = true;
                  break;
               }
            }
         }
         return bedsChanged;
      }

      private bool genericConfigChanged(DeviceDriver3 original, DeviceDriver3 modified)
      {
         if (original.ComputerName != modified.ComputerName ||
            original.DeviceName != modified.DeviceName ||
            original.SQLPatientResolve != modified.SQLPatientResolve ||
            original.DriverType != modified.DriverType ||
            original.AlarmSystemType != modified.AlarmSystemType ||
            original.CommConfiguration != modified.CommConfiguration
            //original.CommConfigurationObject.Hostname != modified.CommConfigurationObject.Hostname ||
            //original.CommConfigurationObject.SocketPort != modified.CommConfigurationObject.SocketPort ||
            //original.CommConfigurationObject.USBProducerId != modified.CommConfigurationObject.USBProducerId ||
            //original.CommConfigurationObject.USBSerialId != modified.CommConfigurationObject.USBSerialId ||
            //original.CommConfigurationObject.USBVendorId != modified.CommConfigurationObject.USBVendorId ||
            //original.CommConfigurationObject.USBVendorId != modified.CommConfigurationObject.USBVendorId ||
            //original.CommConfigurationObject.ComPort != modified.CommConfigurationObject.ComPort ||
            //original.CommConfigurationObject.Baud != modified.CommConfigurationObject.Baud ||
            //original.CommConfigurationObject.Parity != modified.CommConfigurationObject.Parity ||
            //original.CommConfigurationObject.DataBits != modified.CommConfigurationObject.DataBits ||
            //original.CommConfigurationObject.StopBits != modified.CommConfigurationObject.StopBits ||
            //original.CommConfigurationObject.HandShake != modified.CommConfigurationObject.HandShake ||
            //original.CommConfigurationObject.ReceivingDataMode != modified.CommConfigurationObject.ReceivingDataMode ||
            //original.CommConfigurationObject.ConnectionType != modified.CommConfigurationObject.ConnectionType ||
            //original.CommConfigurationObject.TCPCommType != modified.CommConfigurationObject.TCPCommType ||
            //original.CommConfigurationObject.SmartCableId != modified.CommConfigurationObject.SmartCableId
            )
         {

            return true;
         }

         return false;
      }

      private bool logConfigChanged(DeviceDriver3 original, DeviceDriver3 modified)
      {
         if (
            original.LogEnabled != modified.LogEnabled ||
            original.LogConfig != modified.LogConfig)
         {
            return true;
         }

         return false;
      }

      #endregion

      #region Handlers

      void BeforeSaveHandler(object sender, EventArgs e)
      {

         DeviceDriver3 deviceDriver = (DeviceDriver3)((SaveOrUpdateEventArgs)e).entity;

         validateData(deviceDriver);
      }

      void BeforeUpdateHandler(object sender, EventArgs e)
      {

         DeviceDriver3 deviceDriver = (DeviceDriver3)((SaveOrUpdateEventArgs)e).entity;

         validateData(deviceDriver);

         bool logOnly = false;
         if (deviceDriver.Id != 0)
         {
            var existing = mobjDbContext.Set<DeviceDriver3>().AsNoTracking().Where(x => x.Id == deviceDriver.Id).Single();

            existing.BedLinks = mobjDbContext.Set<DeviceDriver3BedLink>().Where(x => x.DeviceDriverId == existing.Id).ToList();

            logOnly = !genericConfigChanged(existing, deviceDriver) && logConfigChanged(existing, deviceDriver) && !bedConfigChanged(existing, deviceDriver);
         }

         ((SaveOrUpdateEventArgs)e).parameters.Add("logOnly", logOnly);

      }

      void AfterSaveHandler(object sender, EventArgs e)
      {

         DeviceDriver3 entity = (DeviceDriver3)((SaveOrUpdateEventArgs)e).entity;

         mobjBedLinksMgr.Delete(entity.Id);
         foreach (var bedLink in entity.BedLinks)
         {
            mobjBedLinksMgr.Create(bedLink);
         }

         //Send notification to Message Center
         mobjMsgCtrMgr.SendDeviceDriverAdded(entity.Id);

      }

      void AfterUpdateHandler(object sender, EventArgs e)
      {

         DeviceDriver3 entity = (DeviceDriver3)((SaveOrUpdateEventArgs)e).entity;
         Dictionary<string, object> parameters = ((SaveOrUpdateEventArgs)e).parameters;

         if (!parameters.ContainsKey("logOnly"))
         {
            throw new Exception("Unable to set logOnly parameter in device driver editetd message");
         }

         bool logOnly = (bool)parameters["logOnly"];

         mobjBedLinksMgr.Delete(entity.Id);
         foreach (var bedLink in entity.BedLinks)
         {
            mobjBedLinksMgr.Create(bedLink);
         }

         //Send notification to Message Center
         mobjMsgCtrMgr.SendDeviceDriverEdited(entity.Id, logOnly);

      }

      #endregion

      #region Data reading functions



      public bool CheckCableIDAlreadyExists(DeviceDriver3 objDevice, int id)
      {
         bool bolret = false;
         try
         {
            if (objDevice.CommConfigurationObject != null && !string.IsNullOrEmpty(objDevice.CommConfigurationObject.SmartCableId))
            {
               IQueryable<DeviceDriver3> repository = mobjDbContext.Set<DeviceDriver3>();
               //bolret = repository.Where(p => p.CommConfigurationObject.SmartCableId == objDevice.CommConfigurationObject.SmartCableId
               //                               && p.CommConfigurationObject.ConnectionType == 1
               //                               && p.Id != id).Count() > 0;

               string scValue = $"%smartcableid=\"{objDevice.CommConfigurationObject.SmartCableId}\"%";
               string ctValue = "%connectiontype=\"1\"%";
               bolret = repository.Where(p =>
                   EF.Functions.Like(p.CommConfiguration.Replace(" ", "").ToLower(), scValue) &&
                   EF.Functions.Like(p.CommConfiguration.Replace(" ", "").ToLower(), ctValue) &&
                   p.Id != id).Count() > 0;
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error checking for CableID existing");
            throw new Exception(string.Format("Error checking for CableID existing"), e);
         }
         return bolret;
      }

      public DeviceDriver3 Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for DeviceDriver3 with id {0}", id);

         DeviceDriver3 result = null;

         try
         {
            //Set detached loading
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<DeviceDriver3> repository = mobjDbContext.Set<DeviceDriver3>();

            //TODO Trace
            mobjLoggerService.Info("Reading DeviceDriver3 with id {0} from DB", id);

            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //result.Repository = mobjDbContext.Set<DriverRepository>().Where(x => x.Id == result.IdDriverRepository && x.Current).SingleOrDefault();

            if (result == null)
            {
               mobjLoggerService.Info("DeviceDriver3 with id {0} not found in DB", id);
               return result;
            }

            result.BedLinks = mobjDbContext.Set<DeviceDriver3BedLink>().Where(x => x.DeviceDriverId == result.Id)
               .Include(x => x.Bed).ThenInclude(y => y.Location).ToList();

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3 with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DeviceDriver3 with id {0} from DB", id);
            throw new Exception(string.Format("Error reading Device Driver with id {0} from DB", id), e);
         }

         return result;

      }

      public IQueryable<DeviceDriver3> GetDeviceDrivers()
      {
         try
         {


            var objDD = mobjDbContext.Set<DeviceDriver3>();
            var objDR = mobjDbContext.Set<DriverRepository>();
            var objBl = mobjDbContext.Set<DeviceDriver3BedLink>();
            var objBed = mobjDbContext.Set<Bed>();

            //var objQ = from d in objDD
            //           join dr in objDR on d.IdDriverRepository equals dr.Id into res
            //           from r in res.DefaultIfEmpty()
            //           where (r == null || r.Current == true)
            //           select new DeviceDriver3
            //           {
            //              AutoStartDriver = d.AutoStartDriver,
            //              AutoStartWatchDog = d.AutoStartWatchDog,
            //              CommConfiguration = d.CommConfiguration,
            //              ComputerName = d.ComputerName,
            //              DataRate = d.DataRate,
            //              DeviceName = d.DeviceName,
            //              DriverType = d.DriverType,
            //              ForceSendDataWithoutPatient = d.ForceSendDataWithoutPatient,
            //              Id = d.Id,
            //              IdDriverRepository = d.IdDriverRepository,
            //              LogConfig = d.LogConfig,
            //              LogEnabled = d.LogEnabled,
            //              PatientResolveNotCached = d.PatientResolveNotCached,
            //              SendDataToMC = d.SendDataToMC,
            //              SQLPatientResolve = d.SQLPatientResolve,
            //              AlarmSystemType = d.AlarmSystemType,
            //              Repository = r,
            //              BedLinks = objBl.Where(x => x.DeviceDriverId == d.Id)
            //                   .Select(y =>
            //                       new DeviceDriver3BedLink
            //                       {
            //                           BedId = y.BedId,
            //                           DeviceDriverId = y.DeviceDriverId,
            //                           DriverEnabled = y.DriverEnabled,
            //                           DriverSideBedName = y.DriverSideBedName,
            //                           WatchDogEnable = y.WatchDogEnable,
            //                           WatchDogEnabled = y.WatchDogEnabled,
            //                           Bed = objBed.Where(z => z.Id == y.BedId).FirstOrDefault(),
            //                           DeviceDriver3 = d
            //                       })
            //                   .ToList()
            //           };


            /////WARNING
            //Entity Framework 3.1 does not parse linq JoinGroup method, and to permit DB side filtering / pageing and grouping, bedlinks must be loaded with separated query.
            //Maybe one day Microsoft will try to transform EF to a real ORM, waiting for that day I leave the code above written to load bedlinks contextually with Device drivers
            //FYI: https://github.com/dotnet/efcore/issues/19930, https://entityframeworkcore.com/knowledge-base/60588364/group-join-in-ef-core-3-1

            var objQ = from d in objDD
                       join dr in objDR on d.IdDriverRepository equals dr.Id into res
                       from r in res.DefaultIfEmpty()
                       where (r == null || r.Current == true)
                       select new DeviceDriver3
                       {
                          AutoStartDriver = d.AutoStartDriver,
                          AutoStartWatchDog = d.AutoStartWatchDog,
                          CommConfiguration = d.CommConfiguration,
                          ComputerName = d.ComputerName,
                          DataRate = d.DataRate,
                          DeviceName = d.DeviceName,
                          DriverType = d.DriverType,
                          ForceSendDataWithoutPatient = d.ForceSendDataWithoutPatient,
                          Id = d.Id,
                          IdDriverRepository = d.IdDriverRepository,
                          LogConfig = d.LogConfig,
                          LogEnabled = d.LogEnabled,
                          PatientResolveNotCached = d.PatientResolveNotCached,
                          SendDataToMC = d.SendDataToMC,
                          SQLPatientResolve = d.SQLPatientResolve,
                          AlarmSystemType = d.AlarmSystemType,
                          Repository = r,
                       };

            return objQ;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading DeviceDriver3 list from DB");
            throw new Exception(string.Format("Error reading Device Driver list from DB"), e);
         }
      }

      #endregion

      #region Data Writing functions

      /// <summary>
      /// Delete a DeviceDriver3. 
      /// </summary>
      public void Delete(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Removing DeviceDriver3 with id {0}", id);

         var executeClose = mobjDbContext.BeginTransaction();

         try
         {

            var repository = mobjDbContext.Set<DeviceDriver3>();

            DeviceDriver3 entity = repository.SingleOrDefault(x => x.Id == id);
            if (repository == null)
            {
               throw new Exception(string.Format("Unable to remove DeviceDriver3 with id {0}; DeviceDriver3 not found.", id));
            }

            //Create new record for updated entity
            repository.Remove(entity);

            mobjDbContext.SaveChanges();

            if (executeClose) mobjDbContext.CommitTransaction();

            mobjMsgCtrMgr.SendDeviceDriverDeleted(id);

            //TODO Trace
            mobjLoggerService.Info("DeviceDriver3 with {0} removed succesfully", id);
         }
         catch (Exception e)
         {
            if (executeClose) mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing DeviceDriver3 with id {0}", id);
            string message = string.Format("Error removing device driver with id {0}", id);
            throw new Exception(message, e);
         }
      }

      #endregion
   }
}
