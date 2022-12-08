using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Online;
using Newtonsoft.Json;

namespace Configurator.Std.BL.Online
{
    public class OnlineQueriesManager : DalManagerBase<OnlineQuery>, IOnlineQueriesManager
    {
        protected readonly IMessageCenterManager mobjMsgCtrMgr;
        protected readonly IDigistatConfiguration mobjDigCfg;
        public OnlineQueriesManager(DigistatDBContext context, ILoggerService loggerService,
            IMessageCenterManager msgCtrSvc,IDigistatConfiguration digCfg)
        {
            mobjDbContext = context;
            mobjLoggerService = loggerService;
            mobjMsgCtrMgr = msgCtrSvc;
            mobjDigCfg = digCfg;
        }
        
        public bool Delete(int queryID,string usrAbbrev,string usrID)
        {
            bool bolRet = false;
            try
            {
            
                var objQueryRepo = mobjDbContext.Set<OnlineQuery>();

                OnlineQuery qp =  objQueryRepo.FirstOrDefault(p => p.Id == queryID);
                if (qp != null)
                {
                    
                    mobjDbContext.Remove(qp);
                    mobjDbContext.SaveChanges();
                    mobjLoggerService.Info($"The query [{JsonConvert.SerializeObject(qp)}] was deleted ");
                    //Send Message
                    mobjMsgCtrMgr.SendOnlineQueryDeleted(queryID);

                    bolRet = true;
                }
                else
                {
                    throw new Exception($"No query found with ID {queryID}");
                }

            }
            catch (Exception ex)
            {

                string errMsg = $"Error on ValidationGroupManager for group {queryID} - Delete";
                mobjLoggerService.ErrorException(ex, errMsg);
                throw new Exception(errMsg, ex);
            }
            return bolRet;
        }

        protected override void OnAfterSave(EventArgs e)
        {
            
            base.OnAfterSave(e);
            try
            {
               
               var a = (SaveOrUpdateEventArgs)e;
               var b = (OnlineQuery)a.entity;
               mobjMsgCtrMgr.SendOnlineQueryCreated(b.Id);
            }
            catch (Exception exception)
            {
               
            }
            

        }

        protected override void OnAfterUpdate(EventArgs e)
        {
            base.OnAfterUpdate(e);
            try
            {
               var a = (SaveOrUpdateEventArgs)e;
               var b = (OnlineQuery)a.entity;
               mobjMsgCtrMgr.SendOnlineQueryEdited(b.Id);
            }
            catch (Exception exception)
            {
               
            }
        }
    }
}
