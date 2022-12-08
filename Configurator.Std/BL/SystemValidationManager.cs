using System;
using System.Collections.Generic;
using System.Linq;

using Configurator.Std.BL.Hubs;
using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System.Linq.Expressions;

namespace Configurator.Std.BL
{
   public class SystemValidationManager : DalManagerBase<TelligenceServer>, ISystemValidationManager
   {

      private readonly IDictionaryService mobjDicSvc;
      private readonly IDigistatConfiguration mobjDigCfg;
      private readonly IMessageCenterManager mobjMsgCtrMgr;

      #region Costructors

      public SystemValidationManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr,
         ILoggerService loggerService, IDictionaryService dicSvc, IDigistatConfiguration digCfg)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjDicSvc = dicSvc;
         mobjDigCfg = digCfg;
         mobjMsgCtrMgr = msgCtrMgr;
      }

      #endregion

      public List<Digistat.FrameworkStd.UMSLegacy.CDASConfigurationValidation> GetValidations()
      {
         List<Digistat.FrameworkStd.UMSLegacy.CDASConfigurationValidation> objRet =
            Digistat.FrameworkStd.UMSLegacy.SystemValidationUtils.GetValidations(mobjDigCfg.ConnectionString).ToList();
         return objRet;
      }
      public List<string> CanValidateConfiguration()
      {
         List<string> objRet = new List<string>();
         string retMessages = null; ;
         bool bolCan = Digistat.FrameworkStd.UMSLegacy.SystemValidationUtils.CanISignConfiguration(mobjDigCfg.ConnectionString, out retMessages);
         if (!string.IsNullOrEmpty(retMessages))
         {
            objRet.Add(retMessages);
         }
         return objRet;
      }


      public List<string> ValidateCurrentConfiguration(string userID,string reason)
      {
         List<string> objRet = new List<string>();
         try
         {
            string retMessages = null; ;
            bool bolCan = Digistat.FrameworkStd.UMSLegacy.SystemValidationUtils.CanISignConfiguration(mobjDigCfg.ConnectionString, out retMessages);
            if (!string.IsNullOrEmpty(retMessages))
            {
               objRet.Add(retMessages);
            }
            if (bolCan)
            {
              var objTmp =   Digistat.FrameworkStd.UMSLegacy.SystemValidationUtils.SignConfiguration(mobjDigCfg.ConnectionString, userID, reason);
              mobjMsgCtrMgr.SendConfigurationValidated();
            }
         }
         catch(Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error on ValidateCurrentConfiguration");
            string message = string.Format("Error on ValidateCurrentConfiguration");
            throw new Exception(message, e);
         }
         
         return objRet;
      }

   }
}
