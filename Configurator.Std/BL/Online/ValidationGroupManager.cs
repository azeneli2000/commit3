using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Configurator.Std.BL.Hubs;
using Configurator.Std.Exceptions;
using Configurator.Std.Enums;
using Digistat.FrameworkStd.Model.Online;
using Digistat.FrameworkStd.Model;
using Newtonsoft.Json;
//using Newtonsoft.Json;

namespace Configurator.Std.BL.OnLine
{
   public class ValidationGroupManager : DalManagerBase<ValidationGroup>, IValidationGroupManager
   {


      protected readonly IMessageCenterManager mobjMsgCtrMgr;
      protected IStandardParametersManager mobjStdParMgr;
      protected IStandardUnitsManager mobjStdUnitMgr;
      protected IDriverRepositoriesManager mobjDriverMgr;
      protected readonly IDigistatConfiguration mobjDigCfg;

      #region Costructors

      public ValidationGroupManager(DigistatDBContext context, ILoggerService loggerService,
         IMessageCenterManager msgCtrSvc,IStandardParametersManager parMgr,IStandardUnitsManager unitMgr,
      IDriverRepositoriesManager driverMgr,IDigistatConfiguration digCfg)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjMsgCtrMgr = msgCtrSvc;
         mobjStdParMgr = parMgr;
         mobjStdUnitMgr = unitMgr;
         mobjDriverMgr = driverMgr;
         mobjDigCfg = digCfg;

      }

      #endregion

      #region Data reading functions

      public ValidationGroup Get(int id)
      {

         //TODO Trace
         mobjLoggerService.Info("Executing Get for ValidationGroup with id {0}", id);

         ValidationGroup result = null;

         try
         {
            var objVSRepo = mobjDbContext.Set<ValidationSection>();
            IQueryable<ValidationGroup> repository = mobjDbContext.Set<ValidationGroup>().Include(p=>p.Locations)
               .Include(c=>c.Parameters);
            result = repository.Where(x => x.ID == id).SingleOrDefault();

            //Retrieve StandardParameters
            if (result.Parameters != null)
            {
               List<StandardParameter> objParList = mobjStdParMgr.GetMulti(result.Parameters.Select(p => p.ParameterID).ToList());
               if (objParList != null)
               {
                  foreach(ValidationParameter vpar in result.Parameters)
                  {
                     vpar.StdParameter = objParList.Where(p => p.Id == vpar.ParameterID).FirstOrDefault();
                     if(vpar.SectionID.HasValue && vpar.SectionID.Value != 0)
                     {
                        vpar.Section = objVSRepo.Where(p => p.ID == vpar.SectionID).FirstOrDefault();
                     }
                     //If section is not bound, or no seciton exists, put a dummy one
                     if (vpar.Section == null)
                     {
                        vpar.Section = new ValidationSection { Index=-1,Name=string.Empty,ID=0 };
                     }
                  }
               }
               result.Parameters= result.Parameters.OrderBy(p=>p.Section.Index).OrderBy(p => p.Index).ToList();


               List<StandardUnit> objUnitList = mobjStdUnitMgr.GetMulti(result.Parameters.Select(p => p.UnitOfMeasureID).ToList());
               if (objUnitList != null)
               {
                  foreach (ValidationParameter vpar in result.Parameters)
                  {
                     vpar.StdUnit = objUnitList.FirstOrDefault(p => p.Id == vpar.UnitOfMeasureID);
                  }
               }

               List<(string Id, int Version, string DriverName)> objDriverList = mobjDriverMgr.GetActivesLight();  
                     
                  
               if (objDriverList != null)
               {
                  foreach (ValidationParameter vpar in result.Parameters)
                  {
                     (string Id, int Version, string DriverName) firstOrDefault = objDriverList.FirstOrDefault(p => p.Id == vpar.DriverID);


                        vpar.DriverInfo = new DriverRepository();
                        vpar.DriverInfo.Id = firstOrDefault.Id;
                        vpar.DriverInfo.DriverName = firstOrDefault.DriverName;
                     
                  }
               }

               


            }

            //TODO Trace
            mobjLoggerService.Info("ValidationGroup with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ValidationGroup with id {0} from DB", id);
            throw new Exception(string.Format("Error reading ValidationGroup with id {0} from DB", id), e);
         }

         return result;

      }



      public IEnumerable<ValidationGroup> GetList()
      {
         //TODO Trace
         mobjLoggerService.Info("Executing GetList for ValidationGroup");

         IQueryable<ValidationGroup> result = null;

         try
         {
            //Set detached

            result = mobjDbContext.Set<ValidationGroup>().Include(p => p.Locations).Where(p=>p.IsDeleted==false).OrderBy(o => o.Index);

            //TODO Trace
            mobjLoggerService.Info("Reading ValidationGroup List from DB");

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ValidationGroup List from DB");
            throw new Exception("Error reading ValidationGroup List from DB");
         }

         finally
         {
            //disposeContext();
         }


         return result;
      }






      #endregion

      #region Data Write


      public bool MoveValidationGroup(int valGroupID, MoveDirection direction)
      {
         bool bolRet = false;
         if (valGroupID != 0)
         {
            try
            {
               mobjDbContext.BeginTransaction();

               ValidationGroup objToMove = mobjDbContext.Set<ValidationGroup>().Where(p => p.ID == valGroupID).FirstOrDefault();
               if (objToMove != null)
               {
                  ValidationGroup objInstead = null;
                  if (direction == MoveDirection.Up)
                  {
                     objInstead = mobjDbContext.Set<ValidationGroup>().Where(p => p.Index < objToMove.Index && p.IsDeleted == false).OrderByDescending(p => p.Index).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int? tmp = objInstead.Index;
                        objInstead.Index = objToMove.Index;
                        objToMove.Index = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
                  else
                  {
                     objInstead = mobjDbContext.Set<ValidationGroup>().Where(p => p.Index > objToMove.Index && p.IsDeleted == false).OrderBy(p => p.Index).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int? tmp = objInstead.Index;
                        objInstead.Index = objToMove.Index;
                        objToMove.Index = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
               }
               
               mobjDbContext.CommitTransaction();
               mobjMsgCtrMgr.SendOnlineValidationGroupEdited(valGroupID);
            }
            catch (Exception ex)
            {

               mobjDbContext.RollbackTransaction();
               string errMsg = "Error on MoveValidationGroup";
               mobjLoggerService.ErrorException(ex, errMsg);
               throw new Exception(errMsg, ex);
            }

         }
         return bolRet;
      }

      public bool Create(ValidationGroup vg,string usrAbbrev)
      {
         bool bolRet = false;
         try
         {
            mobjDbContext.BeginTransaction();
            var objVGrepo = mobjDbContext.Set<ValidationGroup>();
            var objVGParListRepo = mobjDbContext.Set<ValidationParameter>();
            var objVGLocations = mobjDbContext.Set<ValidationGroupLocation>();

            //Fix ChannelID for validationParameters
            if (vg.Parameters != null)
            {
               foreach (ValidationParameter vp in vg.Parameters)
               {
                  vp.ChannelID = string.Empty;
                  if (!vp.Index.HasValue || vp.Index==0)
                  {
                     vp.Index = vg.Parameters.Max(v => v.Index) + 1;
                  }
                  if (!vp.Index.HasValue)
                  {
                     int? intMaxValue = vg.Parameters.Max(p => p.Index);
                     if (intMaxValue == null)
                     {
                        intMaxValue = 0;
                     }
                     vp.Index = intMaxValue;
                  }
               }
            }
            

            //Set index
            int? currIndex = objVGrepo.Max(p => p.Index);
            if (currIndex.HasValue)
            {
               vg.Index = currIndex.Value + 1;
            }
            else
            {
               vg.Index = 1;
            }

            

            objVGrepo.Add(vg);
            mobjDbContext.SaveChanges();

            //Send Message
            mobjMsgCtrMgr.SendOnlineValidationGroupAdded(vg.ID);

            mobjDbContext.CommitTransaction();

            
            
            string strSerialized = JsonConvert.SerializeObject(vg);
            mobjLoggerService.WriteClinicalLog(100, $"Validation Group CREATED. Value: {strSerialized}", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,
               usrAbbrev, 0, mobjDigCfg.ModuleName, string.Empty, "UMS");
            bolRet = true;
         }
         catch (Exception ex)
         {

            mobjDbContext.RollbackTransaction();
            string errMsg = "Error on ValidationGroupManager - Create";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }


      public bool Update(ValidationGroup vg,string usrAbbrev)
      {
         bool bolRet = false;
         try
         {
            mobjDbContext.BeginTransaction();
            var objVGrepo = mobjDbContext.Set<ValidationGroup>();
            var objVGParListRepo = mobjDbContext.Set<ValidationParameter>();
            var objVGLocations = mobjDbContext.Set<ValidationGroupLocation>();

            //Fix ChannelID,Decimal for validationParameters
            if (vg.Parameters != null)
            {
               foreach (ValidationParameter vp in vg.Parameters)
               {
                  vp.GroupID = vg.ID;
                  vp.ChannelID = string.Empty;
                  if (vp.Decimal == null)
                  {
                     vp.Decimal = 0;
                  }
                  if (!vp.Index.HasValue)
                  {
                     int? intMaxValue = vg.Parameters.Max(p => p.Index);
                     if (intMaxValue == null)
                     {
                        intMaxValue = 0;
                     }
                     vp.Index = intMaxValue;
                  }
               }
            }


            //Set index
            if (!vg.Index.HasValue)
            {
               int? currIndex = objVGrepo.Max(p => p.Index);
               if (currIndex.HasValue)
               {
                  vg.Index = currIndex.Value + 1;
               }
               else
               {
                  vg.Index = 1;
               }
            }
            



            

            //loop through all parameters: remove and re-insert
            List<ValidationParameter> objParList =  objVGParListRepo.AsNoTracking().Where(p => p.GroupID == vg.ID).ToList();
            objVGParListRepo.RemoveRange(objParList);
            mobjDbContext.SaveChanges();
            objVGParListRepo.AddRange(vg.Parameters);


            //loop through all locations: remove and re-insert
            List<ValidationGroupLocation> objLocList = objVGLocations.AsNoTracking().Where(p => p.ValidationGroupID == vg.ID).ToList();
            objVGLocations.RemoveRange(objLocList);
            mobjDbContext.SaveChanges();
            objVGLocations.AddRange(vg.Locations);

            mobjDbContext.SaveChanges();


            objVGrepo.Update(vg);
            mobjDbContext.SaveChanges();

            //Send Message
            mobjMsgCtrMgr.SendOnlineValidationGroupEdited(vg.ID);

            mobjDbContext.CommitTransaction();

            
            string strSerialized = JsonConvert.SerializeObject(vg);
            mobjLoggerService.WriteClinicalLog(100, $"Validation Group {vg.ID} UPDATED. Value: {strSerialized}", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,
               usrAbbrev, 0, mobjDigCfg.ModuleName, string.Empty, "UMS");
            bolRet = true;

         }
         catch (Exception ex)
         {

            mobjDbContext.RollbackTransaction();
            string errMsg = $"Error on ValidationGroupManager for group {vg.ID} - Update";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }



      public bool Delete(int vgID,string usrAbbrev,string usrID)
      {
         bool bolRet = false;
         try
         {
            
            var objVGrepo = mobjDbContext.Set<ValidationGroup>();

            ValidationGroup vg =  objVGrepo.Where(p => p.ID == vgID).FirstOrDefault();
            if (vg != null)
            {
               mobjDbContext.BeginTransaction();
               vg.IsDeleted = true;
               vg.LastUpdate = DateTime.Now;
               vg.UserID = usrID;
               objVGrepo.Update(vg);
               //TODO: Send Message
               
               mobjDbContext.SaveChanges();

               //Send Message
               mobjMsgCtrMgr.SendOnlineValidationGroupDeleted(vg.ID);

               mobjDbContext.CommitTransaction();

               mobjLoggerService.WriteClinicalLog(100, $"Validation Group {vg.ID} DELETED", Digistat.FrameworkStd.Enums.EventLogEntryType.Information,
                  usrAbbrev, 0, mobjDigCfg.ModuleName, string.Empty, "UMS");
               bolRet = true;
            }
            else
            {
               throw new Exception($"No ValidationGroup found with ID {vgID}");
            }

         }
         catch (Exception ex)
         {
            try
            {
               mobjDbContext.RollbackTransaction();
            }
            catch (Exception) { }
            string errMsg = $"Error on ValidationGroupManager for group {vgID} - Delete";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }


      #endregion

      #region ValidationSection

      public IEnumerable<ValidationSection> GetSectionList()
      {
         //TODO Trace
         mobjLoggerService.Info("Executing GetList for ValidationSection");

         IQueryable<ValidationSection> result = null;

         try
         {
            //Set detached

            result = mobjDbContext.Set<ValidationSection>().AsNoTracking().OrderBy(o => o.Index);

            //TODO Trace
            mobjLoggerService.Info("Reading ValidationSection List from DB");

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ValidationSection List from DB");
            throw new Exception("Error reading ValidationSection List from DB");
         }

         finally
         {
            //disposeContext();
         }


         return result;
      }


      public ValidationSection CreateSection(ValidationSection vSect)
      {
         ValidationSection objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();
            var objVSRepo = mobjDbContext.Set<ValidationSection>();
            if (objVSRepo.Any(_ => _.Name == vSect.Name && _.Index == vSect.Index))
            {
               //string errMsg = $"Section already exist";
               //throw new Exception(errMsg);
               return vSect;
            }
            int? intCurrIndex = 0;
            if (vSect.Index == 0)
            {
               intCurrIndex = objVSRepo.Max(p => p.Index) + 1;
               if (intCurrIndex == null)
               {
                  intCurrIndex = 0;
               }
               vSect.Index = intCurrIndex.Value+1;
            }
            
            objVSRepo.Add(vSect);
            mobjDbContext.SaveChanges();

            mobjDbContext.CommitTransaction();
            //Send Message
            mobjMsgCtrMgr.SendOnlineValidationGroupSectionAdded(vSect.ID);
            objRet = vSect;
         }
         catch (Exception ex)
         {
            string strObjSerialized = "";
            try
            {
               strObjSerialized = JsonConvert.SerializeObject(vSect);
            }catch(Exception e) { }
            mobjDbContext.RollbackTransaction();
            string errMsg = $"Error on CreateSection. ValidationSection : {strObjSerialized}";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return objRet;
      }

      public ValidationSection UpdateSection(ValidationSection vSect)
      {
         ValidationSection objRet = null;
         try
         {
            mobjDbContext.BeginTransaction();
            var objVSRepo = mobjDbContext.Set<ValidationSection>();
            var objFound = objVSRepo.Where(p => p.ID == vSect.ID).FirstOrDefault();
            if (objFound != null)
            {
               objFound.Name = vSect.Name;
               objFound.Index = vSect.Index;
               objVSRepo.Update(objFound);
               mobjDbContext.SaveChanges();
            }
            mobjDbContext.CommitTransaction();
            //Send Message
            mobjMsgCtrMgr.SendOnlineValidationGroupSectionEdited(vSect.ID);
            objRet = objFound;
         }
         catch (Exception ex)
         {
            string strObjSerialized = "";
            try
            {
               strObjSerialized = JsonConvert.SerializeObject(vSect);
            }
            catch (Exception e) { }
            mobjDbContext.RollbackTransaction();
            string errMsg = $"Error on UpdateSection. ValidationSection : {strObjSerialized}";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return objRet;
      }

      

      public bool DeleteSection(int sectId)
      {
         bool bolRet = false;
         try
         {
            var objVSRepo = mobjDbContext.Set<ValidationSection>();
            var objFound = objVSRepo.Where(p => p.ID == sectId).FirstOrDefault();
            if (objFound != null)
            {
               objVSRepo.Remove(objFound);
               mobjDbContext.SaveChanges();
               //Send Message
               mobjMsgCtrMgr.SendOnlineValidationGroupSectionDeleted(sectId);
            }
            bolRet = true;
         }
         catch (Exception ex)
         {
            string errMsg = $"Error on DeleteSection. ID : {sectId}";
            mobjLoggerService.ErrorException(ex, errMsg);
            throw new Exception(errMsg, ex);
         }
         return bolRet;
      }


      public bool MoveSection(int sectID, MoveDirection direction)
      {
         bool bolRet = false;
         if (sectID != 0)
         {
            try
            {
               mobjDbContext.BeginTransaction();

               ValidationSection objToMove = mobjDbContext.Set<ValidationSection>().Where(p => p.ID == sectID).FirstOrDefault();
               if (objToMove != null)
               {
                  ValidationSection objInstead = null;
                  if (direction == MoveDirection.Down)
                  {
                     objInstead = mobjDbContext.Set<ValidationSection>().Where(p => p.Index < objToMove.Index).OrderByDescending(p => p.Index).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int tmp = objInstead.Index;
                        objInstead.Index = objToMove.Index;
                        objToMove.Index = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
                  else
                  {
                     objInstead = mobjDbContext.Set<ValidationSection>().Where(p => p.Index > objToMove.Index).OrderBy(p => p.Index).FirstOrDefault();
                     if (objInstead != null)
                     {
                        int tmp = objInstead.Index;
                        objInstead.Index = objToMove.Index;
                        objToMove.Index = tmp;
                        mobjDbContext.SaveChanges();
                     }
                  }
               }
               mobjDbContext.CommitTransaction();
               //Send Message
               mobjMsgCtrMgr.SendOnlineValidationGroupSectionEdited(sectID);

            }
            catch (Exception ex)
            {

               mobjDbContext.RollbackTransaction();
               string errMsg = "Error on MoveSection";
               mobjLoggerService.ErrorException(ex, errMsg);
               throw new Exception(errMsg, ex);
            }

         }
         return bolRet;
      }

      #endregion




   }
}
