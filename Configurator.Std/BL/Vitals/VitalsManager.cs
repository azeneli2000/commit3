using Configurator.Std.BL.Hubs;
using Configurator.Std.Helpers;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Enums.Vitals;
using Digistat.FrameworkStd.Extensions;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Vitals;
using Digistat.FrameworkStd.UMSLegacy;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Configurator.Std.BL.Vitals
{
   public class VitalsManager : IVitalsManager
   {
      private const string SQL_DELETE_RECORDS = @"
                            DELETE ADM
                            FROM ActualDatasetMedia ADM
	                            INNER JOIN ActualDatasetItem ADI ON ADI.ai_ID = ADM.am_ai_ID
	                            INNER JOIN ActualDataset AD ON AD.ad_ID = ADI.ai_ad_ID
	                            INNER JOIN ActualDatasetEnroll ADE ON ADE.ade_ID = AD.ad_ade_ID
                            WHERE ADE.ade_sd_ID = {0};

                            DELETE ADI
                            FROM ActualDatasetItem ADI
	                            INNER JOIN ActualDataset AD ON AD.ad_ID = ADI.ai_ad_ID
	                            INNER JOIN ActualDatasetEnroll ADE ON ADE.ade_ID = AD.ad_ade_ID
                            WHERE ADE.ade_sd_ID = {0};

                            DELETE AD
                            FROM ActualDataset AD
	                            INNER JOIN ActualDatasetEnroll ADE ON ADE.ade_ID = AD.ad_ade_ID
                            WHERE ADE.ade_sd_ID = {0};

                            DELETE ADE
                            FROM ActualDatasetEnroll ADE
                            WHERE ADE.ade_sd_ID = {0};";

      private static readonly Regex mobjScriptInline = new Regex("\\s", RegexOptions.IgnoreCase | RegexOptions.Multiline);

      private class ExportImportDataset
      {
         public const string TRANSLATE_PREFIX = "Dataset-";
         public const string TRANSLATE_MODULE = "Vitals";

         public StandardDataset StandardDataset { get; set; }

         public IList<Digistat.FrameworkStd.Model.Dictionary> Terms { get; set; }
      }

      #region Costructors

      private readonly ILoggerService mobjLoggerService;
      private readonly IDigistatEnvironmentService mobjEnvService;
      private readonly DigistatDBContext mobjDbContext;
      protected readonly IMessageCenterManager mobjMsgCtrMgr;
      protected readonly IDigistatConfiguration mobjConfig;

      public VitalsManager(DigistatDBContext context, IDigistatConfiguration config, ILoggerService loggerService, IDigistatEnvironmentService envSvc, IMessageCenterManager msgCtrSvc)
      {
         mobjDbContext = context;
         mobjConfig = config;
         mobjLoggerService = loggerService;
         mobjEnvService = envSvc;
         mobjMsgCtrMgr = msgCtrSvc;
      }

      #endregion Costructors

      /// <summary>
      /// Returns the list of all sd, by default the sd deleted are included in the return list
      /// </summary>
      /// <param name="validOnly"> if validOnly is true, deleted sd will not be returned </param>
      /// <returns></returns>
      public List<StandardDataset> GetAll(bool validOnly = false)
      {
         List<StandardDataset> objRet = null;
         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDataset> repository = mobjDbContext.Set<StandardDataset>();
            objRet = repository.ToList();
            if (validOnly)
            {
               objRet = objRet.Where(a => a.sd_DateTimeDeletedUTC == null).ToList();
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading StandardDataset GetAll from DB");
            throw new Exception("Error reading StandardDataset", e);
         }
         return objRet;
      }

      public List<StandardDataset> GetAllScoreDS(bool validOnly = false)
      {
         List<StandardDataset> objRet = null;
         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDataset> repository = mobjDbContext.Set<StandardDataset>().Where(a => a.sd_Type == 1);
            objRet = repository.ToList();
            if (validOnly)
            {
               objRet = objRet.Where(a => a.sd_DateTimeDeletedUTC == null).ToList();
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardDataset GetAllScoreDS from DB");
            throw new Exception("Error reading StandardDataset", e);
         }
         return objRet;
      }

      public StandardDatasetItem GetItem(Guid itemID)
      {
         StandardDatasetItem objRet = null;
         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDatasetItem> repository = mobjDbContext.Set<StandardDatasetItem>();
            objRet = repository.Include(x => x.si_sd_).Include(p => p.StandardDatasetSubItems).Where(p => p.si_ID == itemID).FirstOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading StandardDatasetItem  id {itemID} from DB");
            throw new Exception($"Error reading StandardDatasetItem  id {itemID} from DB", e);
         }
         return objRet;
      }

      public List<StandardDatasetItem> GetItemsForDS(Guid dsID)
      {
         List<StandardDatasetItem> objRet = null;
         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDatasetItem> repository = mobjDbContext.Set<StandardDatasetItem>();
            objRet = repository.Where(p => p.si_sd_ID == dsID).ToList();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading GetItemsForDS  id {dsID} from DB");
            throw new Exception($"Error reading GetItemsForDS  id {dsID} from DB", e);
         }
         return objRet;
      }

      public StandardDatasetItem SetItem(StandardDatasetItem objItem)
      {
         StandardDatasetItem objRet = objItem;
         try
         {
            var itemRepo = mobjDbContext.Set<StandardDatasetItem>();
            if (objItem.si_ID != Guid.Empty)
            {
               var datasetRepo = mobjDbContext.Set<StandardDataset>();
               var objDataset = datasetRepo.FirstOrDefault(x => x.sd_ID == objItem.si_sd_ID);
               var isPublished = objDataset?.sd_Published == true;

               if (isPublished)
               {
                  var currentItem = itemRepo.FirstOrDefault(x => x.si_ID == objItem.si_ID);
                  currentItem.si_HL7_Identifier = objItem.si_HL7_Identifier;
                  currentItem.si_HL7_UnitIdentifier = objItem.si_HL7_UnitIdentifier;
                  itemRepo.Update(currentItem);
               }
               else
               {
                  itemRepo.Update(objItem);
               }
            }
            else
            {
               itemRepo.Add(objItem);
            }
            mobjDbContext.SaveChanges();

            mobjMsgCtrMgr.SendVitalsConfigUpdated(objItem.si_sd_ID);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error saving Item id {objItem.si_ID} from DB");
            throw new Exception($"Error saving Item id {objItem.si_ID} from DB", e);
         }
         return objRet;
      }

      public List<StandardDatasetSubItems> GetSubItemsForItem(Guid itemID)
      {
         List<StandardDatasetSubItems> objRet = null;
         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDatasetSubItems> repository = mobjDbContext.Set<StandardDatasetSubItems>();
            objRet = repository.Where(p => p.li_si_ID == itemID).ToList();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading GetSubItemsForItem  id {itemID} from DB");
            throw new Exception($"Error reading GetSubItemsForItem  id {itemID} from DB", e);
         }
         return objRet;
      }

      public List<StandardDatasetScoreDescription> GetStdScoreDescriptions(Guid sdID)
      {
         List<StandardDatasetScoreDescription> objRet = null;
         try
         {
            IQueryable<StandardDatasetScoreDescription> repository = mobjDbContext.Set<StandardDatasetScoreDescription>();
            objRet = repository.Where(p => p.dsr_sd_ID == sdID).ToList();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading GetStdScoreDescriptions  sdid {sdID} from DB");
            throw new Exception($"Error reading GetStdScoreDescriptions  sdid {sdID} from DB", e);
         }
         return objRet;
      }

      public StandardDatasetScoreDescription GetStdScoreDescription(Guid scoreDescriptionId)
      {
         StandardDatasetScoreDescription objRet = null;
         try
         {
            IQueryable<StandardDatasetScoreDescription> repository = mobjDbContext.Set<StandardDatasetScoreDescription>();
            objRet = repository.Where(p => p.dsr_ID == scoreDescriptionId).FirstOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading StandardDatasetSubItems  id {scoreDescriptionId} from DB");
            throw new Exception($"Error reading StandardDatasetSubItems  id {scoreDescriptionId} from DB", e);
         }
         return objRet;
      }

      public StandardDatasetSubItems GetSubItem(Guid subItemID)
      {
         StandardDatasetSubItems objRet = null;
         try
         {
            //Set detached
            //mobjDbContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<StandardDatasetSubItems> repository = mobjDbContext.Set<StandardDatasetSubItems>();
            objRet = repository.Where(p => p.li_ID == subItemID).FirstOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error reading StandardDatasetSubItems  id {subItemID} from DB");
            throw new Exception($"Error reading StandardDatasetSubItems  id {subItemID} from DB", e);
         }
         return objRet;
      }

      public StandardDatasetSubItems SetSubItem(StandardDatasetSubItems objSub)
      {
         StandardDatasetSubItems objRet = objSub;
         try
         {
            var subItemrepo = mobjDbContext.Set<StandardDatasetSubItems>();
            if (objSub.li_ID != Guid.Empty)
            {
               subItemrepo.Update(objSub);
            }
            else
            {
               subItemrepo.Add(objSub);
            }
            mobjDbContext.SaveChanges();

            var loadedEntity = subItemrepo.Include(x => x.li_si_).Single(x => x.li_ID == objSub.li_ID);
            mobjMsgCtrMgr.SendVitalsConfigUpdated(loadedEntity.li_si_.si_sd_ID);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error saving Subitem id {objSub.li_ID} from DB");
            throw new Exception($"Error saving Subitem id {objSub.li_ID} from DB", e);
         }
         return objRet;
      }

      public StandardDataset Get(Guid id)
      {
         StandardDataset result = null;
         try
         {
            IQueryable<StandardDataset> repository = mobjDbContext.Set<StandardDataset>();
            result = repository.Include(x => x.Locations).Where(x => x.sd_ID == id).SingleOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardDataset with id {0} from DB", id);
            throw new Exception(string.Format("Error reading StandardDataset with id {0} from DB", id), e);
         }

         return result;
      }

      public bool HasRecords(Guid id)
      {
         try
         {
            var objRepository = mobjDbContext.Set<ActualDataset>();
            return objRepository.AsNoTracking().Any(x => x.ad_ade_.ade_sd_ID == id);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading ActualDataset with standard id {0} from DB", id);
            throw new Exception(string.Format("Error reading ActualDataset with standard id {0} from DB", id), e);
         }
      }

      public Dictionary<int, string> GetItemTypes()
      {
         Dictionary<int, string> objRet = new Dictionary<int, string>();
         foreach (Digistat.FrameworkStd.Enums.Vitals.ParamType item in Enum.GetValues(typeof(Digistat.FrameworkStd.Enums.Vitals.ParamType)))
         {
            if (item != Digistat.FrameworkStd.Enums.Vitals.ParamType.OcrImage)
               objRet.Add((int)item, item.ToString());
         }
         return objRet;
      }

      public Dictionary<int, string> GetDatasetTimings()
      {
         Dictionary<int, string> objRet = new Dictionary<int, string>();
         foreach (Digistat.FrameworkStd.Enums.Vitals.Timing item in Enum.GetValues(typeof(Digistat.FrameworkStd.Enums.Vitals.Timing)))
         {
            objRet.Add((int)item, item.ToString());
         }
         return objRet;
      }

      public Dictionary<int, string> GetDatasetTypes()
      {
         Dictionary<int, string> objRet = new Dictionary<int, string>();
         foreach (Digistat.FrameworkStd.Enums.Vitals.DatasetType item in Enum.GetValues(typeof(Digistat.FrameworkStd.Enums.Vitals.DatasetType)))
         {
            objRet.Add((int)item, item.ToString());
         }

         return objRet;
      }

      public void DeleteSubItem(StandardDatasetSubItems objSub, string usrAbbrev)
      {
         try
         {
            if (objSub != null)
            {
               var subItemrepo = mobjDbContext.Set<StandardDatasetSubItems>();
               StandardDatasetSubItems loadedEntity = subItemrepo.Include(x => x.li_si_).SingleOrDefault(x => x.li_ID == objSub.li_ID);
               if (loadedEntity == null)
               {
                  throw new Exception(string.Format("Unable to delete StandardDatasetSubItems with id {0}; SubItem not found.", objSub.li_ID));
               }
               subItemrepo.Remove(loadedEntity);
               mobjDbContext.SaveChanges();
               mobjLoggerService.Write(100, $"Vitals: Removed SubItem {objSub.li_ID} for Item {objSub.li_si_ID} ", EventLogEntryType.Information, usrAbbrev);

               mobjMsgCtrMgr.SendVitalsConfigUpdated(loadedEntity.li_si_.si_sd_ID);
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error removing StandardDatasetSubItems id {objSub.li_ID} from DB");
            throw new Exception($"Error removing StandardDatasetSubItems id {objSub.li_ID} from DB", e);
         }
      }

      public void DeleteDatasetScoreDescription(StandardDatasetScoreDescription objSD, string usrAbbrev)
      {
         try
         {
            if (objSD != null)
            {
               var scoreRepo = mobjDbContext.Set<StandardDatasetScoreDescription>();
               StandardDatasetScoreDescription loadedEntity = scoreRepo.SingleOrDefault(x => x.dsr_ID == objSD.dsr_ID);
               if (loadedEntity == null)
               {
                  throw new Exception(string.Format("Unable to delete StandardDatasetScoreDescription with id {0}; DatasetScoreDescription not found.", objSD.dsr_ID));
               }
               scoreRepo.Remove(loadedEntity);
               mobjDbContext.SaveChanges();
               mobjLoggerService.Write(100, $"Vitals: Removed DatasetScoreDescription {objSD.dsr_ID} for Dataset {objSD.dsr_sd_ID} ", EventLogEntryType.Information, usrAbbrev);

               mobjMsgCtrMgr.SendVitalsConfigUpdated(loadedEntity.dsr_sd_ID);
            }
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error removing StandardDatasetScoreDescription id {objSD.dsr_ID} from DB");
            throw new Exception($"Error removing StandardDatasetScoreDescription id {objSD.dsr_ID} from DB", e);
         }
      }

      public void DeleteItem(StandardDatasetItem objItem, string usrAbbrev)
      {
         try
         {
            if (objItem != null)
            {
               var itemRepo = mobjDbContext.Set<StandardDatasetItem>();
               var subItemRepo = mobjDbContext.Set<StandardDatasetSubItems>();
               var actualItemRepo = mobjDbContext.Set<ActualDatasetItem>();
               var actualDatasetMediaRepo = mobjDbContext.Set<ActualDatasetMedia>();

               StandardDatasetItem loadedEntity = itemRepo.SingleOrDefault(x => x.si_ID == objItem.si_ID);
               if (loadedEntity == null)
               {
                  throw new Exception(string.Format("Unable to delete StandardDatasetItem with id {0}; Item not found.", objItem.si_ID));
               }
               if (mobjDbContext.Database.CurrentTransaction == null)
               {
                  mobjDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
               }

               List<ActualDatasetItem> objActualItemList = actualItemRepo.Where(p => p.ai_si_ID == loadedEntity.si_ID).ToList();
               List<ActualDatasetMedia> objActualItemMediaList = actualDatasetMediaRepo.Where(p => p.am_ai_.ai_si_ID == loadedEntity.si_ID).ToList();
               actualDatasetMediaRepo.RemoveRange(objActualItemMediaList);
               actualItemRepo.RemoveRange(objActualItemList);
               mobjDbContext.SaveChanges();

               List<StandardDatasetSubItems> objSubItemList = subItemRepo.Where(p => p.li_si_ID == loadedEntity.si_ID).ToList();
               subItemRepo.RemoveRange(objSubItemList);

               itemRepo.Remove(loadedEntity);
               mobjDbContext.SaveChanges();
               mobjDbContext.Database.CommitTransaction();
               mobjLoggerService.Write(100, $"Vitals: Removed Item {objItem.si_ID} for Dataset {objItem.si_sd_ID} ", EventLogEntryType.Information, usrAbbrev);

               mobjMsgCtrMgr.SendVitalsConfigUpdated(loadedEntity.si_sd_ID);
            }
         }
         catch (Exception e)
         {
            if (mobjDbContext.Database.CurrentTransaction != null)
            {
               mobjDbContext.Database.RollbackTransaction();
            }

            mobjLoggerService.ErrorException(e, $"Error removing StandardDatasetItem id {objItem.si_ID} from DB");
            throw new Exception($"Error removing StandardDatasetItem id {objItem.si_ID} from DB", e);
         }
      }

      public void DeleteStandardDataset(Guid dsId, string usrAbbrev)
      {
         try
         {
            if (dsId != Guid.Empty)
            {
               StandardDataset dsRepo = mobjDbContext.Set<StandardDataset>().Where(a => a.sd_ID == dsId).FirstOrDefault();
               //IList<StandardDatasetScoreDescription> dsItemsScore = mobjDbContext.Set<StandardDatasetScoreDescription>().Where(a => a.dsr_sd_ID == dsId).ToList();
               //IList<StandardDatasetItem> dsItemsRepo = mobjDbContext.Set<StandardDatasetItem>().Where(a=>a.si_sd_ID==dsId).ToList();
               //List<int> itemsIds = dsItemsRepo.Select(p => p.si_ID).ToList();
               //IList<StandardDatasetSubItems> dsSubItemsRepo = new List<StandardDatasetSubItems>();

               //StandardDatasetItem loadedEntity = itemRepo.SingleOrDefault(x => x.si_ID == objItem.si_ID);
               if (dsRepo == null)
               {
                  throw new Exception(string.Format("Unable to delete StandardDataset with id {0}; Item not found.", dsId));
               }
               if (mobjDbContext.Database.CurrentTransaction == null)
               {
                  mobjDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
               }

               //remove subitems
               //if (itemsIds.Count() > 0)
               //{
               //   dsSubItemsRepo = mobjDbContext.Set<StandardDatasetSubItems>().Where(a => itemsIds.Contains(a.li_si_ID)).ToList();
               //   mobjDbContext.Set<StandardDatasetSubItems>().RemoveRange(dsSubItemsRepo);
               //}

               ////remove items
               //mobjDbContext.Set<StandardDatasetItem>().RemoveRange(dsItemsRepo);
               ////remove score items
               //mobjDbContext.Set<StandardDatasetScoreDescription>().RemoveRange(dsItemsScore);
               //remove dataset
               // mobjDbContext.Set<StandardDataset>().Remove(dsRepo);
               dsRepo.sd_DateTimeDeletedUTC = DateTime.Now.ToUniversalTime();

               mobjDbContext.SaveChanges();
               mobjDbContext.Database.CommitTransaction();
               //  mobjLoggerService.Write(100, $"Vitals: Removed SubItems : {string.Join(", ",dsSubItemsRepo.Select(a=>a.li_ID))} , Items : {string.Join(", ", dsItemsRepo.Select(a => a.si_ID))}, Score Items: {string.Join(", ", dsItemsScore.Select(a => a.dsr_ID))} for Dataset {dsId} ", EventLogEntryType.Information, usrAbbrev);
               mobjLoggerService.Write(100, $"Vitals: Removed Dataset {dsId} ", EventLogEntryType.Information, usrAbbrev);

               mobjMsgCtrMgr.SendVitalsConfigUpdated(dsId);
            }
         }
         catch (Exception e)
         {
            if (mobjDbContext.Database.CurrentTransaction != null)
            {
               mobjDbContext.Database.RollbackTransaction();
            }

            mobjLoggerService.ErrorException(e, $"Error removing StandardDataset id {dsId} from DB");
            throw new Exception($"Error removing StandardDataset id {dsId} from DB", e);
         }
      }

      public StandardDataset SetDataset(StandardDataset objDS)
      {
         StandardDataset objRet = objDS;
         try
         {
            var dsRepo = mobjDbContext.Set<StandardDataset>();
            var locRepo = mobjDbContext.Set<StandardDatasetLocation>();

            if (objDS.sd_ID != Guid.Empty)
            {
               if (objDS.Locations != null)
               {
                  var objLocList = locRepo.AsNoTracking().Where(x => x.StandardDatasetID == objDS.sd_ID).ToList();
                  locRepo.RemoveRange(objLocList);
                  mobjDbContext.SaveChanges();
                  locRepo.AddRange(objDS.Locations);
               }

               dsRepo.Update(objDS);
            }
            else
            {
               dsRepo.Add(objDS);
            }
            mobjDbContext.SaveChanges();

            mobjMsgCtrMgr.SendVitalsConfigUpdated(objDS.sd_ID);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error saving StandardDataset id {objDS.sd_ID} from DB");
            throw new Exception($"Error saving StandardDataset id {objDS.sd_ID} from DB", e);
         }
         return objRet;
      }

      public StandardDatasetScoreDescription SetScoreDescription(StandardDatasetScoreDescription objSD)
      {
         StandardDatasetScoreDescription objRet = objSD;
         try
         {
            var sdRepo = mobjDbContext.Set<StandardDatasetScoreDescription>();
            if (objSD.dsr_ID != Guid.Empty)
            {
               sdRepo.Update(objSD);
            }
            else
            {
               sdRepo.Add(objSD);
            }
            mobjDbContext.SaveChanges();

            mobjMsgCtrMgr.SendVitalsConfigUpdated(objSD.dsr_sd_ID);
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, $"Error saving StandardDatasetScoreDescription id {objSD.dsr_ID} from DB");
            throw new Exception($"Error saving StandardDatasetScoreDescription id {objSD.dsr_ID} from DB", e);
         }
         return objRet;
      }

      public StandardDataset SetDataset(StandardDataset objSD, bool useOcrImg, string usrAbbrev)
      {
         StandardDataset objRet = objSD;
         try
         {
            var stDatasetRepo = mobjDbContext.Set<StandardDataset>();
            var itemRepo = mobjDbContext.Set<StandardDatasetItem>();
            var subItemRepo = mobjDbContext.Set<StandardDatasetSubItems>();
            var locationRepo = mobjDbContext.Set<StandardDatasetLocation>();

            if (mobjDbContext.Database.CurrentTransaction == null)
            {
               mobjDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            }

            if (objSD.sd_ID != Guid.Empty)
            {
               var bolIsPublished = stDatasetRepo.AsNoTracking().FirstOrDefault(x => x.sd_ID == objSD.sd_ID).sd_Published;
               if (bolIsPublished)
               {
                  throw new InvalidOperationException("The dataset is published.");
               }

               if (objSD.Locations != null)
               {
                  var objLocList = locationRepo.AsNoTracking().Where(x => x.StandardDatasetID == objSD.sd_ID).ToList();
                  locationRepo.RemoveRange(objLocList);
                  mobjDbContext.SaveChanges();
                  locationRepo.AddRange(objSD.Locations);
               }

               stDatasetRepo.Update(objSD);
            }
            else
            {
               objSD.sd_Published = false;
               stDatasetRepo.Add(objSD);
            }

            mobjDbContext.SaveChanges();

            var lstDatasetItem = itemRepo.Where(p => p.si_sd_ID == objRet.sd_ID).ToList();
            bool hasOcrImage = lstDatasetItem != null ? lstDatasetItem.Where(x => x.si_Type == (int)Digistat.FrameworkStd.Enums.Vitals.ParamType.OcrImage).Count() > 0 : false;

            if (useOcrImg && objSD.sd_Ocr)
            {
               if (!hasOcrImage)
               {
                  itemRepo.Add(new StandardDatasetItem()
                  {
                     si_Type = (int)Digistat.FrameworkStd.Enums.Vitals.ParamType.OcrImage,
                     si_Name = "OCR",
                     si_Label = "OCR",
                     si_sd_ID = objRet.sd_ID,
                     si_Index = int.MaxValue
                  });
               }
            }
            else
            {
               if (hasOcrImage)
               {
                  foreach (var ocrImageItem in lstDatasetItem.Where(x => x.si_Type == (int)Digistat.FrameworkStd.Enums.Vitals.ParamType.OcrImage))
                  {
                     List<StandardDatasetSubItems> objSubItemList = subItemRepo.Where(p => p.li_si_ID == ocrImageItem.si_ID).ToList();
                     subItemRepo.RemoveRange(objSubItemList);
                     itemRepo.Remove(ocrImageItem);
                     mobjLoggerService.Write(100, $"Vitals: Removed Item {ocrImageItem.si_ID} for Dataset {ocrImageItem.si_sd_ID} ", EventLogEntryType.Information, usrAbbrev);
                  }
               }
            }

            mobjDbContext.SaveChanges();

            mobjDbContext.Database.CommitTransaction();

            mobjMsgCtrMgr.SendVitalsConfigUpdated(objSD.sd_ID);
         }
         catch (Exception e)
         {
            mobjDbContext.Database.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error saving dataset id {objSD.sd_ID} from DB");
            throw new Exception($"Error saving dataset id {objSD.sd_ID} from DB", e);
         }
         return objRet;
      }

      public StandardDataset SetDatasetPublished(StandardDataset objSD, string usrAbbrev)
      {
         StandardDataset objRet = null;

         try
         {
            var stDatasetRepo = mobjDbContext.Set<StandardDataset>();
            var locationRepo = mobjDbContext.Set<StandardDatasetLocation>();

            if (objSD.sd_ID != Guid.Empty)
            {
               if (mobjDbContext.Database.CurrentTransaction == null)
               {
                  mobjDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
               }

               objRet = stDatasetRepo.FirstOrDefault(x => x.sd_ID == objSD.sd_ID);

               objRet.sd_EnabledByDefault = objSD.sd_EnabledByDefault;
               objRet.sd_Global = objSD.sd_Global;
               objRet.sd_HL7_Score = objSD.sd_HL7_Score.NullIfEmpty();

               mobjDbContext.SaveChanges();

               var objLocList = locationRepo.AsNoTracking().Where(x => x.StandardDatasetID == objSD.sd_ID).ToList();
               locationRepo.RemoveRange(objLocList);
               mobjDbContext.SaveChanges();

               if (objSD.Locations != null)
               {
                  locationRepo.AddRange(objSD.Locations);
                  mobjDbContext.SaveChanges();
               }

               mobjDbContext.Database.CommitTransaction();

               mobjMsgCtrMgr.SendVitalsConfigUpdated(objSD.sd_ID);
            }
         }
         catch (Exception e)
         {
            mobjDbContext.Database.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error saving dataset id {objSD.sd_ID} from DB");
            throw new Exception($"Error saving dataset id {objSD.sd_ID} from DB", e);
         }

         return objRet;
      }

      public string Export(Guid sdID)
      {
         string strJsonCrypted = null;
         var objExportImportDataset = ExportToClass(sdID);

         if (objExportImportDataset != null)
         {
            var objJsonSettings = new JsonSerializerSettings()
            {
               Formatting = Formatting.None,
               NullValueHandling = NullValueHandling.Ignore,
               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var strJson = JsonConvert.SerializeObject(objExportImportDataset, objJsonSettings);

            strJsonCrypted = UMSFrameworkCompatibility.EncryptString(strJson, nameof(VitalsManager));
         }

         return strJsonCrypted;
      }

      public void Import(string content, string usrAbbrev)
      {
         ExportImportDataset objExportImportDataset = null;

         try
         {
            var strJsonUncrypted = UMSFrameworkCompatibility.DecryptString(content, nameof(VitalsManager));
            objExportImportDataset = JsonConvert.DeserializeObject<ExportImportDataset>(strJsonUncrypted);

            if (objExportImportDataset?.StandardDataset == null)
            {
               throw new InvalidOperationException();
            }
         }
         catch
         {
            throw new InvalidOperationException("Invalid file.");
         }

         ImportFromClass(objExportImportDataset, usrAbbrev);
      }

      public void SetPublished(Guid sdID, bool state, string usrAbbrev)
      {
         try
         {
            var objStandardDatasetRepository = mobjDbContext.Set<StandardDataset>();

            if (mobjDbContext.Database.CurrentTransaction == null)
            {
               mobjDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            }

            var objStandardDataset = objStandardDatasetRepository
               .Include(x => x.StandardDatasetItem)
               .ThenInclude(x => x.StandardDatasetSubItems)
               .FirstOrDefault(x => x.sd_ID == sdID);

            if (objStandardDataset != null && state != objStandardDataset.sd_Published)
            {
               objStandardDataset.sd_Published = state;
               if (!state)
               {
                  if (HasRecords(sdID))
                  {
                     throw new InvalidOperationException("The dataset has records");
                  }
               }
               else
               {
                  ValidateBeforePusblish(objStandardDataset);
                  mobjDbContext.Database.ExecuteSqlRaw(SQL_DELETE_RECORDS, sdID);
               }

               mobjDbContext.SaveChanges();
               mobjDbContext.Database.CommitTransaction();

               mobjMsgCtrMgr.SendVitalsConfigUpdated(sdID);

               mobjLoggerService.Write(0, $"Dataset \"{objStandardDataset.sd_Name}\" ({sdID}) {(state ? "published" : "unpublished")}", EventLogEntryType.Information, usrAbbrev, 0, LogType.CLN, mobjConfig.ModuleName);
            }
         }
         catch (Exception e)
         {
            mobjDbContext.Database.RollbackTransaction();
            mobjLoggerService.ErrorException(e, $"Error saving dataset id {sdID} from DB");
            throw new Exception($"Error saving dataset id {sdID} from DB", e);
         }
      }

      private ExportImportDataset ExportToClass(Guid sdID)
      {
         try
         {
            ExportImportDataset objResult = null;
            var objStandardDatasetRepository = mobjDbContext.Set<StandardDataset>();
            var objDictionaryRepository = mobjDbContext.Set<Digistat.FrameworkStd.Model.Dictionary>();

            var objStandardDataset = objStandardDatasetRepository
                .Include(x => x.StandardDatasetItem)
                .ThenInclude(x => x.StandardDatasetSubItems)
                .Include(x => x.StandardDatasetScoreDescription)
                .Include(x => x.Locations)
                .FirstOrDefault(x => x.sd_ID == sdID);

            if (objStandardDataset != null)
            {
               var objTerms = new[] { $"{ExportImportDataset.TRANSLATE_PREFIX}{objStandardDataset.sd_Name}" }
                   .Concat(objStandardDataset.StandardDatasetItem.Select(x => $"{ExportImportDataset.TRANSLATE_PREFIX}{x.si_Label}"))
                   .Concat(objStandardDataset.StandardDatasetItem.Select(x => $"{ExportImportDataset.TRANSLATE_PREFIX}{x.si_Description}"))
                   .Concat(objStandardDataset.StandardDatasetItem.SelectMany(x => x.StandardDatasetSubItems).Select(x => $"{ExportImportDataset.TRANSLATE_PREFIX}{x.li_Label}"))
                   .ToList();

               var objTermsTranslated = objDictionaryRepository
                      .Where(x => objTerms.Contains(x.DictionaryKey) && x.Module == ExportImportDataset.TRANSLATE_MODULE)
                      .ToList();

               objResult = new ExportImportDataset()
               {
                  StandardDataset = objStandardDataset,
                  Terms = objTermsTranslated
               };
            }

            return objResult;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error exporting StandardDataset with id {0}", sdID);
            throw new Exception(string.Format("Error exporting StandardDataset with id {0}", sdID), e);
         }
      }

      private void ImportFromClass(ExportImportDataset exportImportDataset, string usrAbbrev)
      {
         ExportImportDataset objInitialData = null;
         var objStandardDataset = exportImportDataset.StandardDataset;

         try
         {
            objInitialData = ExportToClass(objStandardDataset.sd_ID);

            if (mobjDbContext.Database.CurrentTransaction == null)
            {
               mobjDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            }

            var objDatasetRepository = mobjDbContext.Set<StandardDataset>();
            var objDatasetItemRepository = mobjDbContext.Set<StandardDatasetItem>();
            var objDatasetSubItemsRepository = mobjDbContext.Set<StandardDatasetSubItems>();
            var objDatasetLocationRepository = mobjDbContext.Set<StandardDatasetLocation>();
            var objDatasetScoreDescriptionRepository = mobjDbContext.Set<StandardDatasetScoreDescription>();
            var objLocationRepository = mobjDbContext.Set<Location>();
            var objDictionaryRepository = mobjDbContext.Set<Digistat.FrameworkStd.Model.Dictionary>();

            mobjDbContext.Database.ExecuteSqlRaw(SQL_DELETE_RECORDS, objStandardDataset.sd_ID);

            var objCurrentDataset = objDatasetRepository.FirstOrDefault(x => x.sd_ID == objStandardDataset.sd_ID);
            if (objCurrentDataset == null)
            {
               objCurrentDataset = VitalsHelper.Clone(objStandardDataset);
               objDatasetRepository.Add(objCurrentDataset);
            }
            else if (objCurrentDataset.sd_Published && objCurrentDataset.sd_DateTimeDeletedUTC.HasValue)
            {
               throw new InvalidOperationException("This published dataset has been deleted.");
            }
            else if (objCurrentDataset.sd_Published)
            {
               throw new InvalidOperationException("This dataset is published.");
            }
            else
            {
               VitalsHelper.Copy(objStandardDataset, objCurrentDataset);
               objCurrentDataset.sd_DateTimeDeletedUTC = null;
               objDatasetRepository.Update(objCurrentDataset);
            }

            mobjDbContext.SaveChanges();

            var objCurrentSubItems = objDatasetSubItemsRepository.Where(x => x.li_si_.si_sd_ID == objCurrentDataset.sd_ID).ToArray();
            var objCurrentItems = objDatasetItemRepository.Where(x => x.si_sd_ID == objCurrentDataset.sd_ID).ToArray();
            var objCurrentScoreDescriptions = objDatasetScoreDescriptionRepository.Where(x => x.dsr_sd_ID == objCurrentDataset.sd_ID).ToArray();
            var objCurrentLocations = objDatasetLocationRepository.Where(x => x.StandardDatasetID == objStandardDataset.sd_ID).ToArray();

            objDatasetSubItemsRepository.RemoveRange(objCurrentSubItems);
            objDatasetItemRepository.RemoveRange(objCurrentItems);
            objDatasetScoreDescriptionRepository.RemoveRange(objCurrentScoreDescriptions);
            objDatasetLocationRepository.RemoveRange(objCurrentLocations);
            mobjDbContext.SaveChanges();

            if (objStandardDataset.StandardDatasetItem?.Count > 0)
            {
               foreach (var objItem in objStandardDataset.StandardDatasetItem)
               {
                  VitalsHelper.GuardGuid(objItem.si_ID, $"The id of the item '{objItem.si_Name}' is invalid");
                  var objCurrentItem = objDatasetItemRepository.FirstOrDefault(x => x.si_ID == objItem.si_ID);

                  if (objCurrentItem == null)
                  {
                     var objItemToImport = VitalsHelper.Clone(objItem);
                     objDatasetItemRepository.Add(objItemToImport);
                  }
                  else if (objCurrentItem.si_sd_ID != objCurrentDataset.sd_ID)
                  {
                     throw new NotImplementedException($"The item '{objItem.si_ID}' is linked to another dataset.");
                  }
                  else
                  {
                     VitalsHelper.Copy(objItem, objCurrentItem);
                     objDatasetItemRepository.Update(objCurrentItem);
                  }

                  mobjDbContext.SaveChanges();

                  if (objItem.StandardDatasetSubItems?.Count > 0)
                  {
                     foreach (var objSubItem in objItem.StandardDatasetSubItems)
                     {
                        VitalsHelper.GuardGuid(objSubItem.li_ID, $"The id of the sub-item '{objSubItem.li_Label}' is invalid");
                        var objCurrentSubItem = objDatasetSubItemsRepository.FirstOrDefault(x => x.li_ID == objSubItem.li_ID);

                        if (objCurrentSubItem == null)
                        {
                           var objSubItemToImport = VitalsHelper.Clone(objSubItem);
                           objDatasetSubItemsRepository.Add(objSubItemToImport);
                        }
                        else if (objCurrentSubItem.li_si_ID != objItem.si_ID)
                        {
                           throw new NotImplementedException($"The sub-item '{objSubItem.li_ID}' is linked to another item.");
                        }
                        else
                        {
                           VitalsHelper.Copy(objSubItem, objCurrentSubItem);
                           objDatasetSubItemsRepository.Update(objCurrentSubItem);
                        }

                        mobjDbContext.SaveChanges();
                     }
                  }
               }
            }

            if (objStandardDataset.StandardDatasetScoreDescription?.Count > 0)
            {
               foreach (var objScoreDescription in objStandardDataset.StandardDatasetScoreDescription)
               {
                  VitalsHelper.GuardGuid(objScoreDescription.dsr_ID, $"The id of the score description '{objScoreDescription.dsr_Description}' is invalid");
                  var objCurrentScoreDescription = objDatasetScoreDescriptionRepository.FirstOrDefault(x => x.dsr_ID == objScoreDescription.dsr_ID);

                  if (objCurrentScoreDescription == null)
                  {
                     var objScoreDescriptionToImport = VitalsHelper.Clone(objScoreDescription);
                     objDatasetScoreDescriptionRepository.Add(objScoreDescriptionToImport);
                  }
                  else if (objCurrentScoreDescription.dsr_sd_ID != objStandardDataset.sd_ID)
                  {
                     throw new NotImplementedException($"The score description '{objScoreDescription.dsr_ID}' is linked to another dataset.");
                  }
                  else
                  {
                     VitalsHelper.Copy(objScoreDescription, objCurrentScoreDescription);
                     objDatasetScoreDescriptionRepository.Update(objCurrentScoreDescription);
                  }

                  mobjDbContext.SaveChanges();
               }
            }

            if (objStandardDataset.Locations?.Count > 0)
            {
               foreach (var objDatasetLocation in objStandardDataset.Locations.GroupBy(x => x.LocationID).Select(x => x.First()))
               {
                  if (objLocationRepository.Any(x => x.Id == objDatasetLocation.LocationID))
                  {
                     objDatasetLocationRepository.Add(new StandardDatasetLocation() { LocationID = objDatasetLocation.LocationID, StandardDatasetID = objStandardDataset.sd_ID });
                  }
               }

               mobjDbContext.SaveChanges();
            }

            if (exportImportDataset.Terms?.Count > 0)
            {
               var objCurrentTerms = objDictionaryRepository
                  .Where(x => x.DictionaryKey.StartsWith(ExportImportDataset.TRANSLATE_PREFIX) && x.Module == ExportImportDataset.TRANSLATE_MODULE)
                  .ToList();

               foreach (var objTerm in exportImportDataset.Terms)
               {
                  var objCurrentTerm = objCurrentTerms.FirstOrDefault(x => x.DictionaryKey == objTerm.DictionaryKey && x.Language == objTerm.Language && x.Module == objTerm.Module);
                  if (objCurrentTerm != null)
                  {
                     objCurrentTerm.Value = objTerm.Value;
                  }
                  else
                  {
                     objDictionaryRepository.Add(objTerm);
                  }
               }

               mobjDbContext.SaveChanges();
            }

            mobjDbContext.Database.CommitTransaction();
            if (objInitialData == null)
            {
               mobjLoggerService.Write(0, $"Import new dataset: {objStandardDataset.sd_Name}", EventLogEntryType.Information, usrAbbrev, 0, LogType.CLN, mobjConfig.ModuleName);
            }
            else
            {
               var objJsonSettings = new JsonSerializerSettings()
               {
                  Formatting = Formatting.None,
                  NullValueHandling = NullValueHandling.Ignore,
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               };
               var strInitialJson = JsonConvert.SerializeObject(objInitialData, objJsonSettings);
               mobjLoggerService.Write(0, $"Import existing dataset: {objStandardDataset.sd_Name} (old: {strInitialJson})", EventLogEntryType.Information, usrAbbrev, 0, LogType.CLN, mobjConfig.ModuleName);
            }

            mobjMsgCtrMgr.SendVitalsConfigUpdated(objStandardDataset.sd_ID, true);
         }
         catch (Exception e)
         {
            if (mobjDbContext.Database.CurrentTransaction != null)
            {
               mobjDbContext.Database.RollbackTransaction();
            }

            mobjLoggerService.ErrorException(e, $"Error importing JSON: {objStandardDataset.sd_Name}.");
            throw;
         }
      }

      private void ValidateBeforePusblish(StandardDataset standardDataset)
      {
         var objDatasetRules = new Dictionary<Func<StandardDataset, bool>, string>()
         {
            { x => (DatasetType)x.sd_Type == DatasetType.Score && mobjScriptInline.Replace(x.sd_Script ?? string.Empty, string.Empty).Length == 0, "Score script is undefined." },
            { x => (DatasetType)x.sd_Type == DatasetType.Score && x.sd_ScoreDescriptionScript?.Length > 0 && mobjScriptInline.Replace(x.sd_ScoreDescriptionScript ?? string.Empty, string.Empty).Length == 0, "Score description script is invalid." },
            { x => (Timing)x.sd_Timing == Timing.Variable && mobjScriptInline.Replace(x.sd_IntervalScript ?? string.Empty, string.Empty).Length == 0, "Interval script is undefined." },
            { x => x.sd_Ocr && x.sd_OcrDevice <= 0, "OCR Device is undefined." },
            { x => x.StandardDatasetItem.Count == 0, "Missing items." },
         };
         var objItemRules = new Dictionary<Func<StandardDatasetItem, bool>, Func<StandardDatasetItem, string>>()
         {
            { x => ((ParamType)x.si_Type == ParamType.List || (ParamType)x.si_Type == ParamType.NumericList) && x.StandardDatasetSubItems.Count == 0, x => $"Item \"{x.si_Name}\": missing subitems." },
            { x => (ParamType)x.si_Type == ParamType.NumericList && mobjScriptInline.Replace(x.si_Script ?? string.Empty, string.Empty).Length == 0, x => $"Item \"{x.si_Name}\": script is undefined." },
            { x => x.si_HL7_UnitIdentifier.NullIfEmpty() != null && x.si_HL7_UnitIdentifier.NullIfEmpty() == null, x => $"Item \"{x.si_Name}\": id for HL7 message undefined." },
         };

         var objErrorMessages = objDatasetRules
            .Where(x => x.Key.Invoke(standardDataset))
            .Select(x => x.Value)
            .ToList();

         var objItemsErrorMessages = standardDataset.StandardDatasetItem
            .OrderBy(x => x.si_Index)
            .SelectMany(x => objItemRules.Where(y => y.Key.Invoke(x)).Select(y => y.Value.Invoke(x)))
            .ToList();

         objErrorMessages.AddRange(objItemsErrorMessages);

         if (objErrorMessages.Count > 0)
         {
            throw new InvalidOperationException(string.Join(Environment.NewLine, objErrorMessages));
         }
      }
   }
}