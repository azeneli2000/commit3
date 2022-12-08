using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

using Configurator.Std.BL.Configurator;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using Microsoft.EntityFrameworkCore;

namespace Configurator.Std.BL.DasDrivers
{
    public class PortServerManager : DalManagerBase<PortServer>, IPortServerManager
    {

        private readonly IMessageCenterManager mobjMsgCtrMgr;

        public PortServerManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrMgr)
        {
            mobjDbContext = context;
            mobjLoggerService = loggerService;
            mobjMsgCtrMgr = msgCtrMgr;
            this.AfterSave += PortServerManager_AfterSave;
            this.AfterUpdate += PortServerManager_AfterUpdate;
        }


        public List<PortServer> GetTelligencePortServer()
        {
            return mobjDbContext.Set<PortServer>().Where(p => p.Type == 0).ToList();
        }


        public PortServer Get(int ID)
        {
            return mobjDbContext.Set<PortServer>().Where(p => p.ID == ID).FirstOrDefault();
        }

        public PortServer GetWithBeds(int ID)
        {
            PortServer result = null;
            try
            {
                IQueryable<PortServer> repository = mobjDbContext.Set<PortServer>().Include(x => x.PortServerBedLinks);
                result = repository.Where(p => p.ID == ID).SingleOrDefault();
                return result;
            }
            catch (Exception e)
            {
                mobjLoggerService.ErrorException(e, "Error reading PortServer GetWithBeds with id {0} from DB", ID);
                throw new Exception(string.Format("Error reading PortServer GetWithBeds with id {0} from DB", ID), e);
            }
        }

        public Dictionary<int, string> GetPortServerTypes()
        {
            Dictionary<int, string> objRet = new Dictionary<int, string>();
            foreach (Digistat.FrameworkStd.Enums.PortServerType item in Enum.GetValues(typeof(Digistat.FrameworkStd.Enums.PortServerType)))
            {
                objRet.Add((int)item, item.ToString());
            }
            return objRet;
        }


        public bool Delete(int ID)
        {
            bool bolRet = false;
            var objPsrepo = mobjDbContext.Set<PortServer>();
            PortServer tlPortServerItem = objPsrepo.Where(p => p.ID == ID).FirstOrDefault();
            if (tlPortServerItem.Type == 0)
            {
                var objTlgDeviceRepo = mobjDbContext.Set<TelligenceDevice>();
                var objTlgDeviceItem = objTlgDeviceRepo.Where(p => p.tl_psv_ID == ID).FirstOrDefault();
                if (objTlgDeviceItem != null)
                {
                    objTlgDeviceItem.tl_psv_ID = 0;
                    objTlgDeviceRepo.Update(objTlgDeviceItem);
                }

            }
            objPsrepo.Remove(tlPortServerItem);
            mobjDbContext.SaveChanges();
            mobjMsgCtrMgr.SendPortServerRemoved(tlPortServerItem);
            return bolRet;
        }


        public IQueryable<PortServerStatus> GetPortServerStatus(int psID)
        {
            try
            {
                IQueryable<PortServerStatus> objRet = mobjDbContext.Set<PortServerStatus>().Where(p => p.ID == psID);
                return objRet;
            }
            catch (Exception e)
            {
                string errMsg = $"Error reading PortServerStatus per ID {psID}";
                mobjLoggerService.ErrorException(e, errMsg);
                throw new Exception(errMsg, e);
            }

        }

        private void PortServerManager_AfterUpdate(object sender, EventArgs e)
        {
            PortServer entity = (PortServer)((SaveOrUpdateEventArgs)e).entity;
            mobjMsgCtrMgr.SendPortServerEdited(entity);
        }

        private void PortServerManager_AfterSave(object sender, EventArgs e)
        {
            PortServer entity = (PortServer)((SaveOrUpdateEventArgs)e).entity;
            mobjMsgCtrMgr.SendPortServerEdited(entity);
        }


    }
}
