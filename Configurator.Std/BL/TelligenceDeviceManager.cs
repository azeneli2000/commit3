using System;
using System.Collections.Generic;
using System.Linq;
using Configurator.Std.BL.Hubs;
using Microsoft.EntityFrameworkCore;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;
using System.Data;
using Configurator.Std.BL.DasDrivers;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Digistat.FrameworkStd.Enums;
using Configurator.Std.Exceptions;
using Configurator.Std.Helpers;

namespace Configurator.Std.BL
{
    public class TelligenceDeviceManager : DalManagerBase<TelligenceDevice>, ITelligenceDeviceManager
    {

        private readonly IMessageCenterManager mobjMsgCtrMgr;
        private readonly IDictionaryService mobjDicSvc;
        private readonly IPortServerManager mobjPSMgr;

        #region Costructors

        public TelligenceDeviceManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService,
           IDictionaryService dicSvc, IPortServerManager psMgr)
        {
            mobjMsgCtrMgr = msgCtrMgr;
            mobjDbContext = context;
            mobjLoggerService = loggerService;
            mobjDicSvc = dicSvc;
            mobjPSMgr = psMgr;
        }

        #endregion




        private Network CreateNetworkEntry(TelligenceDevice objTlDevice)
        {
            Network objNetwork = null;
            string strNetworkHostname = string.Format("TELLIGENCE_{0}_{1}", objTlDevice.tl_MACAddress, objTlDevice.tl_locationID);
            var networkRepo = mobjDbContext.Set<Network>();
            Network objNetworkExisting = networkRepo.Where(p => p.HostName == strNetworkHostname).FirstOrDefault();
            if (objNetworkExisting == null)
            {
                //Create new network entity
                objNetwork = new Network();
                objNetwork.HostName = strNetworkHostname;
                objNetwork.IpAddress = objTlDevice.tl_IPAddress;
                objNetwork.Modules = "ALL";
                objNetwork.IsEnabled = true;
                objNetwork.Type = (short)NetworkTypeEnum.TelligenceSS;
                objNetwork.LocationRef = 0;
                objNetwork.BedRef = 0;
                networkRepo.Add(objNetwork);
                mobjDbContext.SaveChanges();
            }
            else
            {
                objNetwork = objNetworkExisting;
            }
            return objNetwork;
        }


        private PortServer CreatePortServerEntry(TelligenceDevice objTLDevice)
        {
            PortServer objPs = new PortServer();
            objPs.Address = objTLDevice.tl_IPAddress;
            objPs.Type = (short)PortServerType.StaffStation;
            objPs.UpdateDate = DateTime.Now;
            objPs.AuthInfo = objTLDevice.tl_MACAddress;
            TelligenceSystem tlSys = mobjDbContext.Set<TelligenceSystem>().FirstOrDefault(w => w.ty_ID == objTLDevice.tl_ty_ID);
            if (tlSys != null)
            {
                objPs.EncryptionKey = tlSys.ty_MDIEncKey;
                objPs.FirstPort = tlSys.ty_MDIPort;
            }
            PortServer objPsFound = null;
            if (objTLDevice.tl_psv_ID != 0)
            {
                objPsFound = mobjPSMgr.Find(p => p.ID == objTLDevice.tl_psv_ID).FirstOrDefault();
            }
            else
            {
                objPsFound = mobjPSMgr.Find(p => p.Address == objPs.Address).FirstOrDefault();
            }
            if (objPsFound == null)
            {
                objPsFound = mobjPSMgr.Create(objPs);
                objTLDevice.tl_psv_ID = objPsFound.ID;
            }
            else
            {
                if (((PortServerType)objPsFound.Type) != PortServerType.StaffStation)
                {
                    throw new PortServerCreationException();
                }

            }
            return objPsFound;
        }


        public IEnumerable<TelligenceDevice> GetAllWithBeds(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<TelligenceDevice, object>>> includePredicates = null)
        {
            List<TelligenceDevice> objRet = null;
            try
            {

                var objDevRepo = mobjDbContext.Set<TelligenceDevice>();
                var objBedLinkRepo = mobjDbContext.Set<NetworkBedLink>();
                IQueryable<TelligenceDevice> repository = from l in objDevRepo
                                                          //join h in objBedLinkRepo on l.tl_NetworkID equals h.IdNetwork into res
                                                          //from h in res.DefaultIfEmpty()
                                                          select new TelligenceDevice
                                                          {
                                                              tl_deviceID = l.tl_deviceID,
                                                              tl_DeviceName = l.tl_DeviceName,
                                                              tl_DeviceType = l.tl_DeviceType,
                                                              tl_ID = l.tl_ID,
                                                              tl_locationDescriptor = l.tl_locationDescriptor,
                                                              tl_IPAddress = l.tl_IPAddress,
                                                              tl_locationID = l.tl_locationID,
                                                              tl_MACAddress = l.tl_MACAddress,
                                                              tl_NetworkID = l.tl_NetworkID,
                                                              tl_psv_ID = l.tl_psv_ID,
                                                              tl_ty_ID = l.tl_ty_ID,
                                                              tl_ty_ = l.tl_ty_
                                                          };


                

                //IQueryable<TelligenceDevice> repository = mobjDbContext.Set<TelligenceDevice>().
                //   FromSql<TelligenceDevice>(@"select t.*,g.BedCount from TelligenceDevice t
                //   left join
                //   (
                //    select nbl.ln_IDNetwork as idnetwork, count(nbl.ln_IDBED) as bedCount from Network_Bed_Link nbl group by nbl.ln_IDNetwork
                //    )
                //   g on t.tl_NetworkID = g.idnetwork");

                if (includePredicates != null && includePredicates.Count() > 0)
                {
                    includePredicates.ToList().ForEach(x => repository = repository.Include(x));
                }

                if (pageNumber > 0)
                {
                    repository = repository.Skip((pageNumber - 1) * pageSize);
                }

                if (pageSize > 0)
                {
                    repository = repository.Take(pageSize);
                }

                //Add bed count
                objRet = repository.ToList();
                var objNetworkIds = objRet.Select(p => p.tl_NetworkID).ToList();
                var objBedLinkList = objBedLinkRepo.Where(p => objNetworkIds.Contains(p.IdNetwork)).ToList();
                foreach(TelligenceDevice td in objRet)
                {
                    td.BedCount = objBedLinkList.Where(p => p.IdNetwork == td.tl_NetworkID).Count();
                }
                
                
            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, "Unable to read GetAllWithBeds {0} records from DB", typeof(TelligenceDevice).Name);
                string message = string.Format("Unable to read all GetAllWithBeds records from DB", typeof(TelligenceDevice).Name);
                throw new Exception(message, e);
            }
            return objRet;
        }

        ///<inheritdoc/>
        public TelligenceDevice UpdateDevice(TelligenceDevice objTlDev, List<Bed> objBeds, IDictionary<short, int> objPortServerBeds, bool createNetwork, bool createPS)
        {
            try
            {
                PortServer objPS = null;
                var tlDeviceRepo = mobjDbContext.Set<TelligenceDevice>();
                var repository = mobjDbContext.Set<NetworkBedLink>();

                mobjDbContext.BeginTransaction();
                if (createNetwork)
                {
                    if (!objTlDev.tl_NetworkID.HasValue)
                    {
                        //if a network already exists, do not create a new one!
                        Network objNetwork = CreateNetworkEntry(objTlDev);
                        objTlDev.tl_NetworkID = objNetwork.Id;
                    }
                }
                if (createPS)
                {
                    CreatePortServerEntry(objTlDev);
                }

                if (objBeds != null)
                {
                    List<NetworkBedLink> objNBLList = new List<NetworkBedLink>();
                    if (objTlDev.tl_NetworkID.HasValue)
                    {
                        foreach (Bed b in objBeds)
                        {
                            NetworkBedLink objNBL = new NetworkBedLink();
                            objNBL.IdBed = b.Id;
                            objNBL.IdNetwork = objTlDev.tl_NetworkID.Value;
                            objNBL.Index = Convert.ToInt16(b.Index);
                            objNBLList.Add(objNBL);
                        }
                        var objToRemove = repository.Where(p => p.IdNetwork == objTlDev.tl_NetworkID.Value);
                        if (objToRemove.Count() > 0)
                        {
                            repository.RemoveRange(objToRemove);
                        }
                    }

                    objPS = mobjDbContext.Set<PortServer>().Where(p => p.Address == objTlDev.tl_IPAddress && p.Type == (int)PortServerType.StaffStation).FirstOrDefault();
                    if (objPS != null)
                    {
                        var portBedLinkRepo = mobjDbContext.Set<PortServerBedLink>();
                        var portBedLinksToRemove = portBedLinkRepo.Where(p => p.PortServerId == objPS.ID);
                        portBedLinkRepo.RemoveRange(portBedLinksToRemove);
                    }
                    

                    if (objNBLList.Count > 0)
                    {
                        repository.AddRange(objNBLList);
                        //Update portserver
                        UpdatePortServer(objTlDev, objPS, objNBLList, objPortServerBeds);
                    }
                    else if (objPS != null)
                    {
                        objPS.IDBED = 0;
                    }
                }

                //Add new telligence device
                tlDeviceRepo.Update(objTlDev);

                mobjDbContext.SaveChanges();
                mobjDbContext.CommitTransaction();

                if (objPS != null)
                {
                    mobjMsgCtrMgr.SendPortServerEdited(objPS, Digistat.FrameworkStd.MessageCenter.DestinationHostCodes.All, Digistat.FrameworkStd.MessageCenter.ApplicationCodes.All);
                }

                return objTlDev;

            }
            catch (Exception e)
            {
                mobjDbContext.RollbackTransaction();
                string message = "Error Create";
                mobjLoggerService.ErrorException(e, message);
                throw new Exception(message, e);
            }
        }

        public TelligenceDevice CreatePortServerForDevice(TelligenceDevice objTlDev)
        {
            try
            {
                PortServer objPs = CreatePortServerEntry(objTlDev);
                if (objPs != null)
                {
                    objTlDev.tl_psv_ID = objPs.ID;
                }
                return objTlDev;
            }
            catch (PortServerCreationException psExc)
            {
                throw psExc;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TelligenceDevice CreateNetworkForDevice(TelligenceDevice objTlDev)
        {
            try
            {
                var tlDeviceRepo = mobjDbContext.Set<TelligenceDevice>();
                mobjDbContext.BeginTransaction();
                Network objNetwork = CreateNetworkEntry(objTlDev);
                objTlDev.tl_NetworkID = objNetwork.Id;
                tlDeviceRepo.Update(objTlDev);
                mobjDbContext.SaveChanges();
                mobjDbContext.CommitTransaction();
                return objTlDev;

            }
            catch (Exception e)
            {
                mobjDbContext.RollbackTransaction();
                string message = "Error Create";
                mobjLoggerService.ErrorException(e, message);
                throw new Exception(message, e);
            }
        }

        ///<inheritdoc/>
        public TelligenceDevice CreateDevice(TelligenceDevice objTlDev, List<Bed> objBeds, IDictionary<short, int> objPortServerBeds, bool createNetwork, bool createPortServer)
        {
            try
            {
                var tlDeviceRepo = mobjDbContext.Set<TelligenceDevice>();
                //
                var repository = mobjDbContext.Set<NetworkBedLink>();


                mobjDbContext.BeginTransaction();
                List<NetworkBedLink> objNBLList = new List<NetworkBedLink>();
                if (createNetwork)
                {
                    Network objNetwork = CreateNetworkEntry(objTlDev);
                    objTlDev.tl_NetworkID = objNetwork.Id;

                    if (objBeds != null && objBeds.Count > 0)
                    {
                        foreach (Bed b in objBeds)
                        {
                            NetworkBedLink objNBL = new NetworkBedLink();
                            objNBL.IdBed = b.Id;
                            objNBL.IdNetwork = objNetwork.Id;
                            objNBL.Index = Convert.ToInt16(b.Index);
                            objNBLList.Add(objNBL);
                        }
                        var objToRemove = repository.Where(p => p.IdNetwork == objNetwork.Id);
                        if (objToRemove.Count() > 0)
                        {
                            repository.RemoveRange(objToRemove);
                        }

                        repository.AddRange(objNBLList);
                    }
                }

                if (createPortServer)
                {
                    PortServer objPs = CreatePortServerEntry(objTlDev);
                    if ((PortServerType)objPs.Type == PortServerType.StaffStation)
                    {
                        objTlDev.tl_psv_ID = objPs.ID;
                        UpdatePortServer(objTlDev, objPs, objNBLList, objPortServerBeds);
                    }
                    else
                    {
                        //TODO: a port server already exists but it's not a StaffStation. Send a warning...
                    }
                }

                //Add new telligence device
                tlDeviceRepo.Add(objTlDev);

                mobjDbContext.SaveChanges();
                mobjDbContext.CommitTransaction();
                return objTlDev;

            }
            catch (Exception e)
            {
                mobjDbContext.RollbackTransaction();
                string message = "Error Create";
                mobjLoggerService.ErrorException(e, message);
                throw new Exception(message, e);
            }
        }

 
      public TelligenceDevice Get(int id)
      {
         TelligenceDevice result = null;

            try
            {
                IQueryable<TelligenceDevice> repository = mobjDbContext.Set<TelligenceDevice>().Include(p => p.tl_ty_);
                result = repository.Where(x => x.tl_ID == id).SingleOrDefault();

            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, string.Format("Error reading TelligenceDevice with id {0} from DB", id));
                throw new Exception(string.Format("Error reading TelligenceDevice with id {0} from DB", id), e);
            }


            return result;

        }

        public TelligenceDevice SearchForDevice(int TLDeviceID, int systemID)
        {
            TelligenceDevice objRet = new TelligenceDevice();
            try
            {
                objRet = mobjDbContext.Set<TelligenceDevice>().Where(p => p.tl_deviceID == TLDeviceID && p.tl_ty_ID == systemID).FirstOrDefault();
            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, $"Unable SearchForDevice TLDeviceID={TLDeviceID}; systemID:{systemID}");
                string message = $"Unable SearchForDevice TLDeviceID={TLDeviceID}; systemID:{systemID}";
                throw new Exception(message, e);
            }
            return objRet;
        }

        public IEnumerable<TelligenceDevice> GetAllDevices()
        {

            //TODO Trace

            List<TelligenceDevice> result;

            try
            {
                IQueryable<TelligenceDevice> repository = mobjDbContext.Set<TelligenceDevice>().Include(p => p.tl_ty_);
                result = repository.ToList();
            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, "Unable to GetAllDevices");
                string message = string.Format("Unable to GetAllDevices");
                throw new Exception(message, e);
            }


            return result;

        }

        public Dictionary<int, string> GetTLDeviceTypes()
        {
            Dictionary<int, string> objRet = new Dictionary<int, string>();
            foreach (TelligenceXMLRPCClient.Entities.StaffStationTypes item in Enum.GetValues(typeof(TelligenceXMLRPCClient.Entities.StaffStationTypes)))
            {
                objRet.Add((int)item, item.ToString());
            }
            return objRet;
        }

        /// <summary>
        /// Update networkbedlinks. Removes all existing links for a specific location then writes the new entries.
        /// NOTE : this function acts only on beds for locations contained in objList collection. NetworkBedLinks bound to
        /// other locations will not be modified.
        /// </summary>
        /// <param name="objList"></param>
        /// <param name="idNetwork"></param>
        /// <returns></returns>
        public bool UpdateNetworkBedLink(List<Bed> objList, int idNetwork)
        {
            bool bolRet = false;
            try
            {
                var repository = mobjDbContext.Set<NetworkBedLink>();
                List<NetworkBedLink> objNBLList = new List<NetworkBedLink>();
                foreach (Bed b in objList)
                {
                    NetworkBedLink objNBL = new NetworkBedLink();
                    objNBL.IdBed = b.Id;
                    objNBL.IdNetwork = idNetwork;
                    objNBL.Index = Convert.ToInt16(b.Index);
                    objNBLList.Add(objNBL);
                }
                mobjDbContext.BeginTransaction();

                var objToRemove = repository.Where(p => p.IdNetwork == idNetwork);
                if (objToRemove.Count() > 0)
                {
                    repository.RemoveRange(objToRemove);
                }
                IEnumerable<int?> objLocationIds = objList.Select(p => p.IdLocation).Distinct();
                if (objLocationIds != null && objLocationIds.Count() > 0)
                {
                    //foreach (int? idLocation in objLocationIds)
                    //{
                    //   if (idLocation.HasValue)
                    //   {
                    //      List<Bed> objBedList = mobjDbContext.Set<Bed>().Where(p => p.IdLocation == idLocation.Value).ToList();

                    //      if (objBedList != null && objBedList.Count() > 0)
                    //      {
                    //         var objToRemove = repository.Where(p => p.IdNetwork == idNetwork && objBedList.Select(o => o.Id).Contains(p.IdBed)).ToList();
                    //         repository.RemoveRange(objToRemove);
                    //      }
                    //   }
                    //}
                    repository.AddRange(objNBLList);

                    //Update portserver (if MDI)
                    TelligenceDevice objDevice = mobjDbContext.Set<TelligenceDevice>().Where(p => p.tl_NetworkID == idNetwork).FirstOrDefault();

                    if (objDevice != null && Helpers.TelligenceHelper.IsMDIDevice(objDevice))
                    {
                        if (objList.Count() > 1)
                        {
                            throw new Exception(mobjDicSvc.XLate("An MDI Staff Station can be bound to only one bed"));
                        }
                        PortServer objPS = mobjDbContext.Set<PortServer>().Where(p => p.Address == objDevice.tl_IPAddress && p.Type == (int)PortServerType.StaffStation).FirstOrDefault();
                        objPS.IDBED = objList.FirstOrDefault().Id;
                    }

                }
                mobjDbContext.SaveChanges();

                mobjDbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                mobjDbContext.RollbackTransaction();
                mobjLoggerService.ErrorException(e, "Error UpdateNetworkBedLinkForLocation");
                string message = string.Format("Error UpdateNetworkBedLinkForLocation");
                throw new Exception(message, e);
            }
            return bolRet;
        }

        public string Delete(int id)
        {
            string strRet = string.Empty;
            try
            {
                mobjDbContext.BeginTransaction();

                var tlDeviceRepo = mobjDbContext.Set<TelligenceDevice>();

                TelligenceDevice tlDeviceItem = tlDeviceRepo.Where(p => p.tl_ID == id).FirstOrDefault();
                tlDeviceRepo.Remove(tlDeviceItem);

                var objPS = mobjDbContext.Set<PortServer>().Where(p => p.Address == tlDeviceItem.tl_IPAddress && p.Type == (int)PortServerType.StaffStation).FirstOrDefault();

                var networkBedLinkRepo = mobjDbContext.Set<NetworkBedLink>();
                networkBedLinkRepo.RemoveRange(networkBedLinkRepo.Where(p => p.IdNetwork == tlDeviceItem.tl_NetworkID));

                var portServerBedLinkRepo = mobjDbContext.Set<PortServerBedLink>();
                portServerBedLinkRepo.RemoveRange(portServerBedLinkRepo.Where(p => p.PortServerId == tlDeviceItem.tl_psv_ID));

                mobjDbContext.SaveChanges();
                mobjDbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                mobjDbContext.RollbackTransaction();

                mobjLoggerService.ErrorException(e, string.Format("Error deleting Telligence Device {0}", id));
                string message = string.Format("Error deleting Telligence Device {0}", id);
                throw new Exception(message, e);
            }
            return strRet;
        }

        private void UpdatePortServer(TelligenceDevice telligenceDevice, PortServer portServer, List<NetworkBedLink> networkBedLinks, IDictionary<short, int> objPortServerBeds)
        {
            if (portServer != null)
            {
                var intNBeds = networkBedLinks?.Count ?? 0;
                if (TelligenceHelper.IsMDIDevice(telligenceDevice))
                {
                    var alsPortGroups = new short[] { 0, 1, 2, 3 };
                    var alsPortNames = new[] { "0", "1" };

                    var portServerBedLinkRepo = mobjDbContext.Set<PortServerBedLink>();
                    var portBedLinksToRemove = portServerBedLinkRepo.Where(p => p.PortServerId == portServer.ID);
                    portServerBedLinkRepo.RemoveRange(portBedLinksToRemove);

                    if (intNBeds == 0)
                    {
                        portServer.IDBED = 0;
                    }
                    else if (intNBeds == 1 && objPortServerBeds.Keys.Count == alsPortGroups.Length)
                    {
                        portServer.IDBED = networkBedLinks.FirstOrDefault().IdBed;
                    }
                    else
                    {
                        portServer.IDBED = -1;

                        foreach (var group in alsPortGroups.Select((portGroup, index) => new { portGroup, index }))
                        {
                            if (objPortServerBeds.ContainsKey(group.portGroup))
                            {
                                foreach (var port in alsPortNames.Select((portName, index) => new { portName, index }))
                                {
                                    var objPortServerBedLink = new PortServerBedLink()
                                    {
                                        PortGroup = group.portGroup,
                                        PortName = port.portName,
                                        BedId = objPortServerBeds[group.portGroup],
                                        PortServerId = portServer.ID,
                                        PortId = (short)((16 * group.index) + port.index)
                                    };

                                    portServerBedLinkRepo.Add(objPortServerBedLink);
                                }
                            }
                        }
                    }
                }
                else if (intNBeds > 0)
                {
                    portServer.IDBED = networkBedLinks.FirstOrDefault().IdBed;
                }
                else
                {
                    portServer.IDBED = 0;
                }
            }
        }
    }
}
