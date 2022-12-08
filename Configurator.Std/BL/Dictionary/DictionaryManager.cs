using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Exceptions;

using Digistat.FrameworkStd.Model;
using Digistat.Dal.Data;


namespace Configurator.Std.BL.Dictionary {

   public class DictionaryManager : Digistat.Dal.Data.DalManagerBase<Digistat.FrameworkStd.Model.Dictionary>, IDictionaryManager {

      private readonly ISynchronizationService objSynchroService;
      private readonly IPermissionsService objPermissionsService;
      private readonly IMiscellaneousManager objMiscManager;
      private readonly IPermissionsService mobjPermSvc;
      private List<string> objLanguages = new List<string>();

      public DictionaryManager(DigistatDBContext objContext, ILoggerService objLoggerService,
         ISynchronizationService objSynchronizationService,
         IPermissionsService objPermissionsService,
         IMiscellaneousManager objMiscellaneousManager
         ,IPermissionsService permSvc)
      {
         mobjDbContext = objContext;
         mobjLoggerService = objLoggerService;
         mobjPermSvc = permSvc;

         objSynchroService = objSynchronizationService;
         this.objPermissionsService = objPermissionsService;
         objMiscManager = objMiscellaneousManager;

         this.AfterSave += writeLastUpdateDateTime;
         this.AfterUpdate += writeLastUpdateDateTime;

      }

      #region Private Utilities
      private void writeLastUpdateDateTime(object sender, EventArgs e) {
         try {
            writeLastUpdateDateTime();
         }
         catch { throw; }
      }

      private void writeLastUpdateDateTime() {
         //update Miscellanea (SS-0220.008)
         //DictionaryLastUpdateUTC set to current UTC date time value 
         // in the format yyyy-MM-dd HH:mm:ss.fff (CultureInfo InvariantCulture)
         const string KEY = "DictionaryLastUpdateUTC";
         try {
            Miscellanea m;
            bool found = true;
            try {
               m = objMiscManager.Find(x => x.Key == KEY).FirstOrDefault();
            }
            catch { 
               m = new Miscellanea(){ Key = KEY };
               found = false;
            }
            m.Value = DateTime.Now
               .ToUniversalTime()
               .ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            if (found)
               objMiscManager.Update(m);
            else
               objMiscManager.Create(m);
            System.Diagnostics.Debug.WriteLine("Miscellanea updated:", m.Value);
         }
         catch { throw; }
      }

      #endregion
      
      public List<Digistat.FrameworkStd.Model.Dictionary> LoadDictionaryAll() {
         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryView,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         try {
            List<Digistat.FrameworkStd.Model.Dictionary> ret =
               mobjDbContext.Set<Digistat.FrameworkStd.Model.Dictionary>().ToList();
            //Ok, I have all items, so I can get the languages...
            objLanguages = ret
               .Select(x => x.Language)
               .Distinct() 
               .OrderBy(x => x)
               .ToList();
            return ret;
         }
         catch (Exception e) { throw e; }
      }

      public Digistat.FrameworkStd.Model.Dictionary AddKey(Digistat.FrameworkStd.Model.Dictionary objTranslation) { 
         /* In this context, Key is NOT the entity (table) key but a Digistat concept,
         corresponding to a (DictionaryKey, Module) tuple. 
         Module can be an empty string.
         */

         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryKeyNew,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         // Sanity check: A complete translation must be provided
         // Check 1st what I can without accessing "the database".
         if (string.IsNullOrEmpty(objTranslation.Language) || string.IsNullOrEmpty(objTranslation.Value))
            throw new ArgumentException("Translation is incomplete!");
         
         // Sanity check: the given Key must be a new one!
         if (this.Exists(t => t.DictionaryKey == objTranslation.DictionaryKey && t.Module == objTranslation.Module))
            throw new Exception ("Key already exists!");
         
         // Add the new entity
         try {
            base.Create(objTranslation);
            return objTranslation;
         } 
         catch { throw; }
      }

      public bool DeleteKey(string strDictionaryKey, string strModule)
      {
         /* In this context, Key is NOT the entity key but a Digistat concept,
         corresponding to a (DictionaryKey, Module) tuple. 
         Module can be an empty string.
         */

         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryKeyDelete,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         // Delete all entities matching (DictionaryKey, Module)
         bool oneOrMoreDeleted = false;
         try {
            foreach (Digistat.FrameworkStd.Model.Dictionary toBeDeleted in base.Find(
                  x =>
                     x.DictionaryKey == strDictionaryKey &&
                     x.Module == strModule
            ))
            {
               mobjDbContext.Remove(toBeDeleted);
               oneOrMoreDeleted = true;
            }
            if (oneOrMoreDeleted) {
               writeLastUpdateDateTime();
               mobjDbContext.SaveChanges();
            } 
            return true;
         }
         catch { throw; }
      }
      
      #region Translations CRUD
      
      // Create
      public Digistat.FrameworkStd.Model.Dictionary AddTranslation(Digistat.FrameworkStd.Model.Dictionary objTranslation) {
         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryAdd,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         /* Sanity check */
         // SS-0200.007 - (DictionaryKey, Module, Language) is enforced by DAL
         if (string.IsNullOrEmpty(objTranslation.Value))
            throw new  ArgumentOutOfRangeException("Translation value cannot be empty.");

         // Add the new entity
         try {
            base.Create(objTranslation);
            return objTranslation;
         } 
         catch { throw; }
      }

      //Retrieve
      public Digistat.FrameworkStd.Model.Dictionary Get(string strDictionaryKey, string strModule, string strLanguage) {
         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryView,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         try {
            return mobjDbContext.Set<Digistat.FrameworkStd.Model.Dictionary>()
               .Where(x =>
                  x.DictionaryKey == strDictionaryKey &&
                  x.Module == strModule &&
                  x.Language == strLanguage
               )
               .FirstOrDefault();
         }
         catch (Exception e) { 
            throw e; 
         }
      }

      //Delete
      public bool DeleteTranslation(Digistat.FrameworkStd.Model.Dictionary objTranslation)
      {
         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryDelete,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         try
         {
            Digistat.FrameworkStd.Model.Dictionary toBeDeleted = this.Get(
               objTranslation.DictionaryKey, objTranslation.Module, objTranslation.Language);
            mobjDbContext.Remove(toBeDeleted);
            writeLastUpdateDateTime();
            mobjDbContext.SaveChanges();
            return true;
         }
         catch { throw; }
      }

      //Update
      public Digistat.FrameworkStd.Model.Dictionary UpdateTranslation(Digistat.FrameworkStd.Model.Dictionary objTranslation) {

         // Check Authorisation
         try
         {
            if (!objPermissionsService.CheckPermission(
                Defs.Permissions.DictionaryEdit,
               objSynchroService.GetCurrentUser())
            )
               throw new UserAuthorizationException();
         }
         catch { throw; }

         /* Sanity check */
         // SS-0200.007 - (DictionaryKey, Module, Language) is enforced by DAL
         if (string.IsNullOrEmpty(objTranslation.Value))
            throw new ArgumentOutOfRangeException("Translation value cannot be empty");
         
         // Update the entity
         try {
            base.Update(objTranslation);
            return objTranslation;
         } 
         catch { throw; }

      }
   
      #endregion

      public List<string> Languages() { 
         if (objLanguages.Count == 0) {
            objLanguages = mobjDbContext.Set<Digistat.FrameworkStd.Model.Dictionary>()
               .Select(t => t.Language)
               .Distinct()
               .OrderBy(x => x)
               .ToList();
         }
         return objLanguages; 
      }

      public bool GetEditPermission(User objUser)
      {
          return mobjPermSvc.CheckPermission(Defs.Permissions.DictionaryEdit, objUser);
      }
      public bool GetAddKeyPermission(User objUser)
      {
          return mobjPermSvc.CheckPermission(Defs.Permissions.DictionaryKeyNew, objUser);
      }
      public bool GetDeleteKeyPermission(User objUser)
      {
          return mobjPermSvc.CheckPermission(Defs.Permissions.DictionaryKeyDelete, objUser);
      }
      public bool GetSystemEditPermission(User objUser)
      {
          return mobjPermSvc.CheckPermission(Defs.Permissions.DictionarySysEdit, objUser);
      }
      
      public DataTable GetAllTraslationsListedForKey() 
      {
         return getDataAsDataTable();
      }

      public void UpdateAllTraslationsListedForKey(TranslationsForKeyAndModule x) 
      {
         updateData(x);
      }

      private DataTable getDataAsDataTable() 
      {
         DataTable T = null;
         try {
            List<Digistat.FrameworkStd.Model.Dictionary> Data = this.LoadDictionaryAll();
            Dictionary<string, TranslationsForKeyAndModule> Items = getGridRows(Data);

            // Now init a flat table and load it with the DictionaryGridViewModel.
            // Init Table
            foreach (TranslationsForKeyAndModule r in Items.Values) {
               T = flatten(r);
               break;
            }
            // Load table with data
            loadDataTable(T, Items);

            return T;
         }
         catch { throw; }
      }

      private Dictionary<string, TranslationsForKeyAndModule> getGridRows(List<Digistat.FrameworkStd.Model.Dictionary> Items) 
      {
         Dictionary<string, TranslationsForKeyAndModule> ret =
            new Dictionary<string, TranslationsForKeyAndModule>();
         try {
            List<string> languages = getLanguages(Items);
            string k = "";
            foreach (Digistat.FrameworkStd.Model.Dictionary d in Items) {
               k = "{dictionarykey:\"" + d.DictionaryKey + "\", module=\"" + d.Module + "\"}";
               if (! ret.ContainsKey(k))
                  ret.Add(k, new TranslationsForKeyAndModule(languages)
                     {
                        Id = k,
                        DictionaryKey = d.DictionaryKey,
                        Module = d.Module,
                        Description = d.Description,
                        IsSystem = d.IsSystem
                     });
               ret[k].Translations[d.Language] = d.Value;
            }
         }
         catch { throw; }
         return ret;
      }

      private List<string> getLanguages(List<Digistat.FrameworkStd.Model.Dictionary> Items) {
         try {
            List<string> languages = Items
               .Select(x => x.Language)
               .Distinct() 
               .OrderBy(x => x)
               .ToList();
               return languages;
         } 
         catch {
            throw;
         }
      }

      private bool loadDataTable(DataTable T, Dictionary<string, TranslationsForKeyAndModule> D) 
      {
         foreach (TranslationsForKeyAndModule r in D.Values) {
            T.LoadDataRow(toRawData(r).ToArray(), true);
         }
         return true;
      }

      private List<object> toRawData(TranslationsForKeyAndModule r) 
      {
         List<object> ret = new List<object>();
         foreach (var p in r.GetType().GetProperties()) {
            if (
               p.GetValue(r, null) != null &&
               p.GetValue(r, null).GetType() == typeof(Dictionary<string, string>)
            )
               foreach (string s in ((Dictionary<string, string>) p.GetValue(r, null)).Values)
                  ret.Add(s);
            else
               ret.Add(p.GetValue(r, null));
         }
         return ret;
      }

      private DataTable flatten(TranslationsForKeyAndModule r) 
      {
         DataTable t = new DataTable();
         try {
            foreach (var p in r.GetType().GetProperties()) {
               if (
                  p.GetValue(r, null) != null &&
                  p.GetValue(r, null).GetType() == typeof(Dictionary<string, string>)
               ) {
                  foreach (string s in ((Dictionary<string, string>) p.GetValue(r, null)).Keys)
                     t.Columns.Add(s);
               }
               else {
                  t.Columns.Add(p.Name);
               }
            }
         } 
         catch (Exception e) {
            throw e;
         }
         return t;
      }

      private void updateData(TranslationsForKeyAndModule T) 
      {
         
         Digistat.FrameworkStd.Model.Dictionary t;
         
         try {
            foreach (string k in T.Translations.Keys) {
               
               t = Get(T.DictionaryKey, T.Module, k);
               if (t == null) {
                  t = createNewTranslation(T, k);
                  if (!string.IsNullOrEmpty(t.Value)) 
                     AddTranslation(t);
               } else {
                  if (string.IsNullOrEmpty(T.Translations[k]) || T.Translations[k].Trim().Equals(string.Empty))
                     DeleteTranslation(t); //Must remove empty translations.
                  else
                     UpdateTranslation(getUpdatedTranslation(t, T.Description, T.IsSystem, T.Translations[k]));
               }
            
            }
         } 
         catch { 
            throw;
         }

      }

      /* A new Digistat.FrameworkStd.Model.Dictionary based on data from a specific sub-translation (Lang). */
      private Digistat.FrameworkStd.Model.Dictionary createNewTranslation(TranslationsForKeyAndModule row, string lang) 
      {
         return new Digistat.FrameworkStd.Model.Dictionary() 
         {
            DictionaryKey = row.DictionaryKey,
            Module = row.Module,
            Description = row.Description,
            IsSystem = row.IsSystem,
            Language = lang,
            Value = row.Translations[lang]
         };
      }

      /* Updates some values of the given Translation. 
         WARNING! This is NOT a pure function!
         The 1st argument is modified.
      */
      private Digistat.FrameworkStd.Model.Dictionary getUpdatedTranslation(
         Digistat.FrameworkStd.Model.Dictionary t, string Description, bool? IsSystem, string Value) 
      {
         t.Description = Description;
         t.IsSystem = IsSystem;
         t.Value = Value;
         return t;
      }
   }
}
