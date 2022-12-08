using Digistat.Dal.Data;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Therapy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Configurator.Std.BL
{
   public class TherapyDataManager : DalManagerBase<StandardAction>, ITherapyDataManager
   {
      public TherapyDataManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

      public IEnumerable<StandardAction> GetBy(string nameTherapyFilter, string descriptionTherapyFilter, string typeTherapyFilter)
      {
         List<StandardAction> ret = null;

         try
         {
            IQueryable<StandardAction> repository = mobjDbContext.Set<StandardAction>();

            IQueryable<StandardAction> finalRep = repository.AsQueryable();


            string[] splitted = typeTherapyFilter.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Count() >= 3)
            {
               splitted = null;
            }

            if (splitted != null && splitted.Length > 0)
            {
               var filteredByType = new List<int>();
               foreach (string strType in splitted)
               {

                  if (Enum.TryParse<TherapyItemType>(strType, out TherapyItemType enuType) && enuType != TherapyItemType.Invalid)
                  //if (int.TryParse(strType, out int intType))
                  {
                     filteredByType.Add((int)enuType);
                  }
               }
               finalRep = finalRep.Where(x => x.ActionType.HasValue && filteredByType.Contains(x.ActionType.Value));
            }

            if (!string.IsNullOrWhiteSpace(nameTherapyFilter))
            {
               finalRep = finalRep.Where(a => a.Name.Contains(nameTherapyFilter));
            }

            if (!string.IsNullOrWhiteSpace(descriptionTherapyFilter))
            {
               finalRep = finalRep.Where(a => a.Description.Contains(descriptionTherapyFilter));
            }

            ret = finalRep.Include(d => d.Location).ToList();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardActions. GetBy Failed.");
         }


         return ret;
      }

      public StandardAction GetByID(int id)
      {
         mobjLoggerService.Info("Executing Get for StandardAction with id {0}", id);

         StandardAction result = null;

         try
         {

            IQueryable<StandardAction> repository = mobjDbContext.Set<StandardAction>();
            IQueryable<StandardAction> finalRep = repository.AsQueryable();

            result = finalRep.Where(x => x.IDStandardAction == id).Include(d => d.Location).SingleOrDefault();

            //TODO Trace
            mobjLoggerService.Info("StandardAction with id {0} retrived from DB", id);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading StandardAction with id {0} from DB", id);
            //throw new Exception(string.Format("Error reading StandardAction  with id {0} from DB", id), e);
         }


         return result;
      }


      public void Add(StandardAction objStandardAction)
      {
         if (objStandardAction != null && objStandardAction.IDStandardAction == 0)
         {
            this.Create(objStandardAction);
         }
      }

      public new void Update(StandardAction standardAction)
      {
         if (standardAction != null)
         {
            if (standardAction.ActionType == (int)TherapyItemType.Mixture)
            {
               throw new Exception($"Cannot directly update this StandardAction with id {standardAction.IDStandardAction} because it's a Mixture !");
            }
            else
            {
               base.Update(standardAction);
            }
         }
      }

      public void Delete(int id)
      {
         var item = GetByID(id);
         if (item != null)
         {
            try
            {
               mobjDbContext.BeginTransaction();
               mobjLoggerService.Info("Executing Delete StandardAction with id {0}", id);
               mobjDbContext.Set<StandardAction>().Remove(item);
               if (item.ActionType.HasValue && (item.ActionType.Value == (int)TherapyItemType.Mixture || item.ActionType.Value == (int)TherapyItemType.Protocol))
               {
                  mobjLoggerService.Info("Deleting components");

                  var sac_sac = mobjDbContext.Set<StandardAssociationItem>().Where(a => a.FatherID == id);
                  if (sac_sac.Count() > 0)
                  {
                     mobjDbContext.RemoveRange(sac_sac);
                  }
               }

               mobjLoggerService.Info("Deleting Resources Links");
               var resources = mobjDbContext.Set<ResourceActionLink>().Where(r=>r.IdStandardAction == id);
               if (resources.Count() > 0)
               {
                  mobjDbContext.RemoveRange(resources);
               }

               mobjLoggerService.Info("Deleting Profiles associations");
               var profLinks = mobjDbContext.Set<ProfileAssociation>().Where(a => a.IdStandardAction == id);
               if (profLinks.Count() > 0)
               {
                  mobjDbContext.RemoveRange(profLinks);
               }

               mobjDbContext.SaveChanges();
               mobjDbContext.CommitTransaction();
            }
            catch (Exception e)
            {
               mobjDbContext.RollbackTransaction();
               mobjLoggerService.Info("Failed Deleting Protocol with id {0}", id);
            }
         }
      }


      public IEnumerable<StandardAssociationItem> GetAssociationComponents(int standardActionId)
      {
         mobjLoggerService.Info("Executing Get all mixtures");

         try
         {

            IQueryable<StandardAssociationItem> result = from association in mobjDbContext.Set<StandardAssociationItem>()
                                                                     join standardAction in mobjDbContext.Set<StandardAction>()
                                                                     on association.ChildID equals standardAction.IDStandardAction where association.FatherID == standardActionId orderby association.Index
                                                                     select new StandardAssociationItem
                                                                     {
                                                                        ChildID = association.ChildID,
                                                                        FatherID = standardAction.IDStandardAction,
                                                                        Index = association.Index,
                                                                        Concentration = standardAction.Concentration.Value,
                                                                        Amount = standardAction.Amount,
                                                                        Volume = standardAction.Volume,
                                                                        Name = standardAction.Name,
                                                                        Description = standardAction.Description,
                                                                        LocationRef = standardAction.LocationRef,
                                                                        LocationName = standardAction.LocationName
                                                                     };
            return result;

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading components for the  StandardAction mixture  with id {0} from DB", standardActionId);
         }

         return null;
      }

     
      public IEnumerable<StandardAssociationItem> GetAssociationsByComponent(int intComponentID,int? type)
      {
         IEnumerable<StandardAssociationItem> ret = null;

         mobjLoggerService.Info("Get mixtures list containing a given component");

         try
         {
            IQueryable<StandardAssociationItem> result = null; 

            if (type == null)
            {
               result = from association in mobjDbContext.Set<StandardAssociationItem>()
                        join standardAction in mobjDbContext.Set<StandardAction>()
                        on association.FatherID equals standardAction.IDStandardAction
                        where association.ChildID == intComponentID
                        select new StandardAssociationItem
                        {
                           Type = standardAction.ActionType.Value
                        };

            }
            else
            {
               result = from association in mobjDbContext.Set<StandardAssociationItem>()
                        join standardAction in mobjDbContext.Set<StandardAction>().Include(x=>x.Location)
                        on association.FatherID equals standardAction.IDStandardAction
                        where association.ChildID == intComponentID where standardAction.ActionType == type.Value
                        select new StandardAssociationItem
                        {
                           Name = standardAction.Name,
                           Description = standardAction.Description,
                           ChildID = standardAction.IDStandardAction,
                           LocationRef = standardAction.LocationRef,
                           LocationName = standardAction.Location.LocationName
                        };
            }

            return result;

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error Ocurred");
         }


         return ret;
      }

      public bool RemoveStandardActionFromMixture(int stdActID)
      {
         mobjLoggerService.Info("Get mixtures list containing a given component");

         try
         {
            mobjDbContext.BeginTransaction();
            var sacLink = mobjDbContext.Set<StandardAssociationItem>();
            var mixtures = sacLink.Where(a => a.ChildID == stdActID);
            int count = 0;
            foreach (var comp in mixtures)
            {
               sacLink.Remove(comp);
               count++;
            };

            if( count > 0)
            {
               mobjDbContext.SaveChanges();
               mobjDbContext.CommitTransaction();
            }
            else
            {
               mobjLoggerService.Info($"no mixture components for id {stdActID} found");
            }
            return true;
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error Ocurred");
         }
         return false;
      }

      private List<StandardAssociationItem> ToMixtureComponentsList(List<StandardAction> components, StandardAction main)
      {
         List<StandardAssociationItem> ret = null;

         if (components != null && main != null)
         {
            foreach (var item in components)
            {
               ret.Add(new StandardAssociationItem() { FatherID = main.IDStandardAction, ChildID = item.IDStandardAction });
            }
         }
         return ret;
      }

      
      public bool SaveStandardAction(StandardAction objAction, List<StandardAssociationItem> objItems, List<Profiles> objProfiles, List<ResourceActionLink> objResources)
      {
         try
         {
            //TODO Trace
            mobjLoggerService.Info("Saving {1} with id {0}", objAction.IDStandardAction, objAction.ActionType.ToString());

            mobjDbContext.BeginTransaction();

            var repository = mobjDbContext.Set<StandardAction>();

            if (objAction.IDStandardAction <= 0)
            {
               repository.Add(objAction);
               mobjDbContext.SaveChanges();
               if (objAction.IDStandardAction <= 0)
               {
                  throw new Exception("Failed to add Standard Action " + objAction.IDStandardAction.ToString());
               }
            }
            else
            {
               repository.Update(objAction);
            }

            switch(objAction.ActionType)
            {
               case (int)TherapyItemType.Mixture:
               case  (int)TherapyItemType.Protocol:
               {
                  var sacLink = mobjDbContext.Set<StandardAssociationItem>();

                  //remove current components
                  foreach (var comp in sacLink.Where(a => a.FatherID == objAction.IDStandardAction))
                  {
                     sacLink.Remove(comp);
                  }

                  //store new components list
                  if (objItems != null)
                  {
                     foreach (var comp in objItems)
                     {
                        comp.FatherID = objAction.IDStandardAction;
                     }

                     sacLink.AddRange(objItems);
                  }
               }
               break;
            }

            var profLink = mobjDbContext.Set<ProfileAssociation>();
            profLink.RemoveRange(profLink.Where(a => a.IdStandardAction == objAction.IDStandardAction));

            foreach (var prof in objProfiles)
            {
               ProfileAssociation objNewPorfileAssociation = new ProfileAssociation
               {
                  Id = prof.IdProfile,
                  IdStandardAction = objAction.IDStandardAction
               };
               mobjDbContext.Set<ProfileAssociation>().Add(objNewPorfileAssociation);
               mobjDbContext.SaveChanges();
            }

            var resLink = mobjDbContext.Set<ResourceActionLink>();
            resLink.RemoveRange(resLink.Where(link => link.IdStandardAction == objAction.IDStandardAction));

            if (objResources != null)
            {
               foreach (var res in objResources)
               {
                  res.IdStandardAction = objAction.IDStandardAction;
                  resLink.Add(res);
               }
            }

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

            //TODO Trace
            mobjLoggerService.Info("Protocol with id {0} saved succesfully", objAction.IDStandardAction);

            return true;

         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error saving Protocol with id {0}", objAction.IDStandardAction);
         }
         return false;
      }


      public IEnumerable<Profiles> GetAllProfiles()
      {
         try
         {
                var result1 = mobjDbContext.Set<Profiles>().Include(x => x.Location).OrderBy(p => p.Index);
                return result1;
         }
         catch(Exception ex)
         {
            mobjLoggerService.ErrorException(ex, "Error retrieving profiles");
         }
         return null; 
     
      }

      public IEnumerable<StandardAssociationItem> GetProfileAssociationsById(int id, int type)
      {
         IQueryable<StandardAssociationItem> result = null;

            result = from association in mobjDbContext.Set<ProfileAssociation>()
                     join profiles in mobjDbContext.Set<Profiles>()
                     on association.Id equals profiles.IdProfile
                     join standarAction in mobjDbContext.Set<StandardAction>()
                     on association.IdStandardAction equals standarAction.IDStandardAction
                     where profiles.IdProfile == id where standarAction.ActionType == type
                     select new StandardAssociationItem
                     {
                         Name = standarAction.Name,
                         Description = standarAction.Description,
                         ChildID = standarAction.IDStandardAction,
                         LocationRef = standarAction.LocationRef,
                         LocationName = standarAction.LocationName

                     };


            return result;
      }



      public IEnumerable<Profiles> GetProfileAssociationsByLink(int id)
      {
         IQueryable<Profiles> result = null;

         result = from association in mobjDbContext.Set<ProfileAssociation>()
                  join profiles in mobjDbContext.Set<Profiles>().Include(x=>x.Location)
                  on association.Id equals profiles.IdProfile
                  where association.IdStandardAction == id
                  select new Profiles
                  {
                     Name = profiles.Name,
                     Description = profiles.Description,
                     IdProfile = profiles.IdProfile,
                     LocationRef = profiles.LocationRef,
                     LocationNameForLink = profiles.Location.LocationName

                  };

         return result;
      }

      public Profiles GetProfileById(int id)
      {
         var result = mobjDbContext.Set<Profiles>().Where(x => x.IdProfile == id).Include(d => d.Location).First();
         return result;
      }

      public int? SaveProfile(Profiles profile)
      {
         try
         {
            var rep = mobjDbContext.Set<Profiles>().Add(profile);

            mobjDbContext.SaveChanges();
            return profile.IdProfile;

         }catch(Exception ex)
         {
            mobjLoggerService.ErrorException(ex, "Error saving Profile with id {0}");

         }

         return null;
      }

      public void SaveProfileAssociations(Profiles profile)
      {
         try { 
         mobjLoggerService.Info("Saving Profile with id {0}", profile.IdProfile);

         mobjDbContext.BeginTransaction();

         int newProfileId = 0;
         if(profile.IdProfile>0)
         {
            UpdateProfile(profile);
            newProfileId = profile.IdProfile;
         }
         else
         {
            newProfileId = SaveProfile(profile).Value;
         }
          
            //remove current components
            foreach (var comp in mobjDbContext.Set<ProfileAssociation>().Where(a => a.Id == profile.IdProfile))
         {
               mobjDbContext.Set<ProfileAssociation>().Remove(comp);
         }
         foreach(var std in profile.AssociationId)
         {
            var newAssociation = new ProfileAssociation
            {
               Id = newProfileId,
               IdStandardAction = std
            };
            mobjDbContext.Set<ProfileAssociation>().Add(newAssociation);
         }

         mobjDbContext.SaveChanges();

         mobjDbContext.CommitTransaction();


         //TODO Trace
         mobjLoggerService.Info("Profile with id {0} saved succesfully", profile.IdProfile);

      }
      catch (Exception e)
      {
         mobjDbContext.RollbackTransaction();
         mobjLoggerService.ErrorException(e, "Error saving Profile with id {0}", profile.IdProfile);
      }

      }

      public void UpdateProfile(Profiles profile)
      {
         try
         {
            var rep = mobjDbContext.Set<Profiles>().Update(profile);
            base.
            mobjDbContext.SaveChanges();


         }
         catch (Exception ex)
         {
            mobjLoggerService.ErrorException(ex, "Error saving Profile with id {0}");

         }
      }

 

      public void SetProfileAssociationsByLink(int idStandardAction, IEnumerable<Profiles> idProfile)
      {
         try
         {
            mobjDbContext.BeginTransaction();

            foreach (var comp in mobjDbContext.Set<ProfileAssociation>().Where(a => a.IdStandardAction == idStandardAction))
            {
               mobjDbContext.Set<ProfileAssociation>().Remove(comp);
            }
           

            foreach (var prof in idProfile)
            {
               ProfileAssociation objNewPorfileAssociation = new ProfileAssociation
               {
                  Id = prof.IdProfile,
                  IdStandardAction = idStandardAction
               };
               mobjDbContext.Set<ProfileAssociation>().Add(objNewPorfileAssociation);
               mobjDbContext.SaveChanges();
            }

            mobjDbContext.CommitTransaction();

         }
         catch(Exception ex)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(ex, "Error saving Profile Associations from Link with id {0}");
         }
  
      }

      public void DeleteProfile(int idProfile)
      {
         try
         {
            mobjLoggerService.Info("Deleting Profile with id {0}", idProfile);


            mobjDbContext.BeginTransaction();

            //remove current components
            foreach (var comp in mobjDbContext.Set<ProfileAssociation>().Where(a => a.Id == idProfile))
            {
               mobjDbContext.Set<ProfileAssociation>().Remove(comp);
            }

            var profiles = mobjDbContext.Set<Profiles>();

            var objProfile = profiles.Where(x=>x.IdProfile== idProfile).First();
            
            mobjDbContext.Set<Profiles>().Remove(objProfile);

            foreach (var profile in profiles.Where(p => p.Index > objProfile.Index))
            {
               profile.Index -= 1;
               profiles.Update(profile);
            }

            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();
            mobjLoggerService.Info("Profile with id {0} deleted succesfully", idProfile);
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error deleting Profile with id {0}", idProfile);
         }
      }

     
      public bool ReorderProfiles(List<KeyValuePair<int, int>> reorderedElements)
      {
         try
         {
            var profiles = mobjDbContext.Set<Profiles>();
            var profilesLenght = profiles.Count();
            mobjDbContext.BeginTransaction();
            foreach (var item in reorderedElements)
            {
               mobjLoggerService.Info("Reordering Profile with id {0}", item.Key);
               var current = profiles.Where(p => p.IdProfile == item.Key).FirstOrDefault();

               if (current == null)
               {
                  mobjLoggerService.Info("Therapy profile with id {0} do not exists", item.Key);
                  continue;
               }
               else
               {

                  var newIndex = item.Value;
                  if (newIndex != current.Index)
                  {
                     current.Index = newIndex;
                     profiles.Update(current);
                  }

                  profiles.Update(current);
               }
            }
            mobjDbContext.SaveChanges();
            mobjDbContext.CommitTransaction();

            mobjLoggerService.Info("Profiles reordered succesfully");
            return true;
         }
         catch (Exception e)
         {
            mobjDbContext.RollbackTransaction();
            mobjLoggerService.ErrorException(e, "Error reordering Profiles");
         }
         return false;
      }

      public List<ResourceActionLink> GetAssociatedResources(int standardActionID)
      {
         mobjLoggerService.Info("Retrieving resources for StandardAction with id {0}", standardActionID);

         try
         {
            //var ret = mobjDbContext.Set<ResourceActionLink>().Where(r => r.IdStandardAction == standardActionID).OrderBy(r=>r.Index);
            var links = mobjDbContext.Set<ResourceActionLink>();
            var resources = mobjDbContext.Set<StandardResource>();

            var ret = from lnk in links where lnk.IdStandardAction == standardActionID
                      join res in resources on lnk.IdResource equals res.ResID  where res.ResCurrent == true
                      select new { Link = lnk, Resource = res };



            return ret.ToList().Select((result) =>
            {
               result.Link.Resources = new List<StandardResource>() { result.Resource };
               result.Link.ResourceName = result.Resource.ResName;
               return result.Link;
            }).OrderBy(r=>r.Index).ToList();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error retrieving resources");
         }
         return null;
      }

      public IQueryable<StandardResource> GetResources()
      {
         mobjLoggerService.Info("Retrieving All resources");

         try
         {
            var links = mobjDbContext.Set<ResourceActionLink>();
            var resources = mobjDbContext.Set<StandardResource>().Where(x=> x.ResCurrent == true) ;


                //var ret = from res in resources
                //          where res.ResCurrent == true
                //          join lnk in links on res.ResID equals lnk.IdResource into join1
                //          from o in join1.DefaultIfEmpty()
                //          select new { Link = o, Resource = res };

                //return ret.Where(o => o.Link == null || o.Link.IdStandardAction != excludeStandardActionID).Select(o => o.Resource).ToList();
                return resources;

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error retrieving resources");
         }
         return null;
      }

      public IQueryable<StandardAction> GetAll(int? locId)
      {
         try
         {
            IQueryable<StandardAction> result2 = null;
            
            result2 = mobjDbContext.Set<StandardAction>().Include(l => l.Location);
            if (locId != -1)
            {
               result2 = result2.Where(x => (x.LocationRef.HasValue && x.LocationRef == locId) || !x.LocationRef.HasValue);
            }
         
            var objFilterOptions = mobjDbContext.Set<SystemOption>().Where(p => p.Name.Contains("ClassFilter")).ToList();
            Dictionary<int, string> systemOptionAssociated = new Dictionary<int, string>();
            foreach (var sys in objFilterOptions)
            {
               string value = sys.Value;
               int index = Int32.Parse(sys.Name.Substring(11));
               systemOptionAssociated.Add(index, value);
            }

            var res = new List<StandardAction>();
            foreach (var x in result2.ToList())
            {
               if (x.ClassCat != null)
               {
                  if (systemOptionAssociated.Count() > 0)
                  {
                     foreach (var sys in systemOptionAssociated)
                     {
                        if (sys.Key == x.ClassCat)
                        {
                           x.ClassName = sys.Value;
                           break;
                        }
                     }
                  }
               }
               res.Add(x);
            }
            return result2 = res.AsQueryable<StandardAction>();
         }
         catch (Exception ex)
         {
            mobjLoggerService.ErrorException(ex, "Error retrieving Standard Actions");
         }
         return null;
      }

      public bool CheckAsscoiatedLocations( int? tempLoc, int[] idItems)
      {

         try
         {
            bool finalResult = true;
            if (tempLoc != null)
            {
               for (int i = 0; i < idItems.Length; i++)
               {
                  var singleitem = mobjDbContext.Set<StandardAction>().Where(x => x.IDStandardAction == idItems[i]).FirstOrDefault();
                  if (singleitem.LocationRef != null)
                  {
                     if (tempLoc != singleitem.LocationRef)
                     {
                        finalResult = false;
                     }
                  }

               }
            }
            else
            {
               for (int i = 0; i < idItems.Length; i++)
               {
                  var singleitem = mobjDbContext.Set<StandardAction>().Where(x => x.IDStandardAction == idItems[i]).FirstOrDefault();
                  if (singleitem.LocationRef != null)
                  {
                    
                        finalResult = false;
                     
                  }

               }
            }
            return finalResult;

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading components for the  StandardAction ");
         }

         return false;
      }

      public bool CheckCopyLocation(string name, int? location)
      {
         var resultm = mobjDbContext.Set<StandardAction>().Where(x => x.Name == name && x.LocationRef == location).Count();
         if(resultm > 0)
         {
            return false;
         }
         else
         {
            return true; 
         }
      }


    }
}

