using Digistat.FrameworkStd.Model.Vitals;
using System;
using System.Collections.Generic;

namespace Configurator.Std.BL.Vitals
{
   public interface IVitalsManager
   {
      StandardDataset Get(Guid id);

      bool HasRecords(Guid id);

      List<StandardDataset> GetAll(bool validOnly = false);

      List<StandardDataset> GetAllScoreDS(bool validOnly = false);

      List<StandardDatasetItem> GetItemsForDS(Guid dsID);

      StandardDatasetItem GetItem(Guid itemID);

      StandardDatasetSubItems GetSubItem(Guid subItemID);

      List<StandardDatasetSubItems> GetSubItemsForItem(Guid itemID);

      Dictionary<int, string> GetItemTypes();

      Dictionary<int, string> GetDatasetTimings();

      Dictionary<int, string> GetDatasetTypes();

      StandardDataset SetDataset(StandardDataset ds);

      StandardDatasetItem SetItem(StandardDatasetItem objSub);

      StandardDatasetSubItems SetSubItem(StandardDatasetSubItems objSub);

      void DeleteSubItem(StandardDatasetSubItems objSub, string usrAbbrev);

      void DeleteItem(StandardDatasetItem objItem, string usrAbbrev);

      List<StandardDatasetScoreDescription> GetStdScoreDescriptions(Guid sdID);

      StandardDatasetScoreDescription GetStdScoreDescription(Guid scoreDescriptionId);

      StandardDatasetScoreDescription SetScoreDescription(StandardDatasetScoreDescription objSD);

      void DeleteDatasetScoreDescription(StandardDatasetScoreDescription objSD, string usrAbbrev);

      void DeleteStandardDataset(Guid dsId, string usrAbbrev);

      StandardDataset SetDataset(StandardDataset objSD, bool useOcrImg, string usrAbbrev);

      StandardDataset SetDatasetPublished(StandardDataset objSD, string usrAbbrev);

      string Export(Guid sdID);

      void Import(string content, string usrAbbrev);

      void SetPublished(Guid sdID, bool state, string usrAbbrev);
   }
}