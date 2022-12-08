using System;
using System.Collections.Generic;
using System.Text;
using Configurator.Std.BL.Configurator;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public class DvdDrvLinkManager : DalManagerBase<DeviceDriver_Driver_Link>, IDvdDrvLinkManager
   {
      protected readonly IMessageCenterManager mobjMsgCtrMgr;
      protected readonly IConfiguratorWebConfiguration mobjWebCfg;
      #region Costructors

      public DvdDrvLinkManager(DigistatDBContext context, ILoggerService loggerService, IMessageCenterManager msgCtrSvc,
         IConfiguratorWebConfiguration confCfg)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrSvc;
         mobjWebCfg = confCfg;
      }

      #endregion
   }
}
