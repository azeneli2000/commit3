using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Therapy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Configurator.Std.BL
{
   public interface ITherapyDataManager: Digistat.Dal.Interfaces.IDalManagerBase<StandardAction>
   {
      IEnumerable<StandardAction> GetBy(string nameTherapyFilter, string descriptionTherapyFilter, string typeTherapyFilter);

      StandardAction GetByID(int id);

      void Add(StandardAction objStandardAction);

      void Delete(int id);

      IEnumerable<StandardAssociationItem> GetAssociationComponents(int standardActionId);

      bool SaveStandardAction(StandardAction objAction, List<StandardAssociationItem> objItems, List<Profiles> objProfiles, List<ResourceActionLink> objResources);

      IEnumerable<StandardAssociationItem> GetAssociationsByComponent(int intComponentID,int? type);

      bool RemoveStandardActionFromMixture(int stdActID);

      IEnumerable<Profiles> GetAllProfiles();

      IEnumerable<StandardAssociationItem> GetProfileAssociationsById(int id, int type);
      Profiles GetProfileById(int id);

      int? SaveProfile(Profiles profile);

      void SaveProfileAssociations(Profiles profile);
      void UpdateProfile(Profiles profile);

      IEnumerable<Profiles> GetProfileAssociationsByLink(int id);
      void SetProfileAssociationsByLink(int idStandardAction, IEnumerable<Profiles> idProfile);
      void DeleteProfile(int idProfile);
      bool ReorderProfiles(List<KeyValuePair<int, int>> reorderedElements);
      List<ResourceActionLink> GetAssociatedResources(int standardActionID);
      IQueryable<StandardResource> GetResources();

      IQueryable<StandardAction> GetAll(int? locId);

      bool CheckAsscoiatedLocations(int? tempLoc, int[] idItems);


      bool CheckCopyLocation(string name, int? location);

   }
}