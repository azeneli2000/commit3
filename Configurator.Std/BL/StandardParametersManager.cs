using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;
using Configurator.Std.BL.Hubs;

namespace Configurator.Std.BL
{
   public class StandardParametersManager : DalManagerBase<StandardParameter>, IStandardParametersManager
   {

      #region Costructors

      private readonly IMessageCenterManager mobjMsgCtrMgr;
      

      public StandardParametersManager(DigistatDBContext context, ILoggerService loggerService,IMessageCenterManager msgCtrMgr)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrMgr;

         this.AfterSave += StandardParametersManager_AfterSave;
         this.AfterUpdate += StandardParametersManager_AfterUpdate;
      }

      private void StandardParametersManager_AfterUpdate(object sender, EventArgs e)
      {
         StandardParameter entity = (StandardParameter)((SaveOrUpdateEventArgs)e).entity;

         //Send notification to Message Center
         mobjMsgCtrMgr.SendStandardParameterUpdated(entity.Id.ToString(), "UPDATE");
      }

      private void StandardParametersManager_AfterSave(object sender, EventArgs e)
      {
         StandardParameter entity = (StandardParameter)((SaveOrUpdateEventArgs)e).entity;
         //Send notification to Message Center
         mobjMsgCtrMgr.SendStandardParameterUpdated(entity.Id.ToString(),"ADD");
      }

      #endregion

      public bool SendRefreshMessageToConnect()
      {
         bool bolRet = false;
         try
         {
            mobjMsgCtrMgr.SendStandardParameterRefresh();
         }
         catch (Exception e)
         {
            bolRet = false;
            mobjLoggerService.ErrorException(e, "Error on SendRefreshMessageToConnect");
            throw new Exception(string.Format("Error on SendRefreshMessageToConnect"), e);
         }
         return bolRet;
      }

      #region Data reading functions

      public StandardParameter Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for StandardParametersManager with id {0}", id);

         StandardParameter result = null;

         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            if (id == 0)
            {
               return new StandardParameter();
            }

            IQueryable<StandardParameter> repository = mobjDbContext.Set<StandardParameter>();

            //TODO Trace
            mobjLoggerService.Info("Reading StandardParameter with id {0} from DB", id);
            result = repository.Where(x => x.Id == id).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("StandardParameter with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardParameter with id {0} from DB", id);
            throw new Exception(string.Format("Error reading StandardParameter with id {0} from DB", id), e);
         }


         return result;

      }


      public List<StandardParameter> GetMulti(List<int> ids)
      {
         List<StandardParameter> result = null;
         string strIds = string.Empty;
         try
         {

            if (ids != null)
            {
               strIds = string.Join(",", ids.ToArray());
               IQueryable<StandardParameter> repository = mobjDbContext.Set<StandardParameter>();
               result = repository.Where(x => ids.Contains(x.Id)).ToList();
            }
            else
            {
               return null;
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardParameter with GetMulti {0} from DB", strIds);
            throw new Exception(string.Format("Error reading StandardParameter with GetMulti {0} from DB", strIds), e);
         }
         return result;
      }


      public List<StandardParameter> GetListByDeviceId(int deviceId)
      {
         string query = $@"select distinct   par_ID, par_DataType, par_Description, par_Print, par_UOM_IDs, par_UCUMCaseSensitive
                        , par_Devices, par_Parameters,par_CaseSensitive,par_Mnemonic
                        ,par_Type,par_Classes,par_Notes,par_IsSystem, par_IsVariableContentWaveform
                  from DriverRepositoryStandardParameterLink a
                  inner join StandardParameter b on a.lnk_par_ID = b.par_ID
                  inner join DriverRepository dr  ON dr.drv_Id = a.lnk_drv_Id
                  cross apply ( select * FROM dbo.fn_split(  dr.drv_DeviceType,',' ) ) as F
                  where dr.drv_Current = 1
                  and F.item =  {deviceId}
                  order by 1";
         List<StandardParameter> objRet = mobjDbContext.Set<StandardParameter>().FromSqlRaw(query).ToList();
         return objRet;
      }


      public List<StandardParameter> GetListByDriverID(string drvID)
      {
         string query = $@"select distinct par_ID, par_DataType, par_Description, par_Print, par_UOM_IDs, par_UCUMCaseSensitive
                        , par_Devices, par_Parameters,par_CaseSensitive,par_Mnemonic
                        ,par_Type,par_Classes,par_Notes,par_IsSystem, par_IsVariableContentWaveform
                  from DriverRepositoryStandardParameterLink a
                  inner join StandardParameter b on a.lnk_par_ID = b.par_ID
                  inner join DriverRepository dr  ON dr.drv_Id = a.lnk_drv_Id
                  where dr.drv_Current = 1
                  and dr.drv_ID = '{drvID}'
                  order by 1";
         List<StandardParameter> objRet = mobjDbContext.Set<StandardParameter>().FromSqlRaw(query).ToList();
         return objRet;
      }

      public List<StandardParameter> GetListByDriverIdOfWaveformType(string driverId){

         // d.lnk_drv_Id, sp.par_ID, sp.par_Print, sp.par_IsVariableContentWaveform

         string query = $@"select 
                           par_ID, par_DataType, par_Description, par_Print, par_UOM_IDs, par_UCUMCaseSensitive, 
                           par_Devices, par_Parameters,par_CaseSensitive,par_Mnemonic, 
                           par_Type,par_Classes,par_Notes,par_IsSystem, par_IsVariableContentWaveform 
                        from [DriverRepositoryStandardParameterLink] d 
                          inner join [StandardParameter] sp on d.lnk_par_ID = sp.par_ID  
                        where d.lnk_drv_Id  = '{driverId}'
                          and d.lnk_IsEnabled = 1
                          and sp.par_DataType = 'WAVEFORM'";

         var queryResult = mobjDbContext.Set<StandardParameter>().FromSqlRaw(query).ToList();
         return queryResult;
      }

      #endregion

      #region Data Writing functions

      public void Delete(int entityId)
      {

         //TODO Trace
         mobjLoggerService.Info("Deleting StandardParameter with id {0}", entityId);

         try
         {

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<StandardParameter>();

            StandardParameter loadedEntity = repository.SingleOrDefault(x => x.Id == entityId);
            if (loadedEntity == null)
            {
               throw new Exception(string.Format("Unable to update update standard with id {0}; update standard not found.", entityId));
            }

            repository.Remove(loadedEntity);

            mobjDbContext.SaveChanges();
            mobjMsgCtrMgr.SendStandardParameterUpdated(entityId.ToString(),"DELETE");
            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("StandardParameter with id {0} removed succesfully", loadedEntity.Id);

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error removing update standard with id {0}", entityId);
            string message = string.Format("Error removing update standard with id {0}", entityId);
            throw new Exception(message, e);
         }
         
      }

      #endregion
   }
}
