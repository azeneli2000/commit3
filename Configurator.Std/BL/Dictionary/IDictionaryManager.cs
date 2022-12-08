using System.Collections.Generic;
using System.Data;
using Digistat.FrameworkStd.Model;
using Configurator.Std;

namespace Configurator.Std.BL.Dictionary
{
   public interface IDictionaryManager : Digistat.Dal.Interfaces.IDalManagerBase<Digistat.FrameworkStd.Model.Dictionary>
   {
      /// <summary>
      /// Get a single translation, id = {Key, Module, Language}
      /// </summary>
      /// <param name="DictionaryKey"></param>
      /// <param name="Module"></param>
      /// <param name="Language"></param>
      /// <returns></returns>
      Digistat.FrameworkStd.Model.Dictionary Get(string DictionaryKey, string Module, string Language);

      /// <summary>
      /// Get all translations in the database.
      /// </summary>
      /// <returns></returns>
      List<Digistat.FrameworkStd.Model.Dictionary> LoadDictionaryAll();

      /// <summary>
      /// Return the whole set of translations available, pivoted on (DictionaryKey + Module).
      /// </summary>
      /// <returns></returns>
      DataTable GetAllTraslationsListedForKey();

      /// <summary>
      /// Update the translations contained in the argument.
      /// </summary>
      /// <param name="x">This object contains the translations to be updated. </param>
      void UpdateAllTraslationsListedForKey(TranslationsForKeyAndModule x);

      /// <summary>
      /// Add a Translation. It's Key (DictionaryKey+Module) MUST be non-existing.
      /// </summary>
      /// <param name="Translation"></param>
      /// <returns></returns>
      Digistat.FrameworkStd.Model.Dictionary AddKey(Digistat.FrameworkStd.Model.Dictionary Translation);

      /// <summary>
      /// Delete all translations with given {DictionaryKey, Module}
      /// </summary>
      /// <param name="Item"></param>
      /// <returns></returns>
      bool DeleteKey(string DictionaryKey, string Module);

      /// <summary>
      /// Add a new Translation.
      /// </summary>
      /// <param name="Translation"></param>
      /// <returns></returns>
      Digistat.FrameworkStd.Model.Dictionary AddTranslation(Digistat.FrameworkStd.Model.Dictionary Translation);

      /// <summary>
      /// Update an existing Translation.
      /// </summary>
      /// <param name="Translation"></param>
      /// <returns></returns>
      Digistat.FrameworkStd.Model.Dictionary UpdateTranslation(Digistat.FrameworkStd.Model.Dictionary Translation);
      
      /// <summary>
      /// Delete a single translation, id = {DictionaryKey, Module, Language}
      /// </summary>
      /// <param name="Translation"></param>
      /// <returns></returns>
      bool DeleteTranslation(Digistat.FrameworkStd.Model.Dictionary Translation);

      /// <summary>
      /// Return a list of the Languages available in the whole set of Translations.
      /// </summary>
      /// <returns></returns>
      List<string> Languages();

      bool GetEditPermission(User objUser);

      bool GetAddKeyPermission(User objUser);

      bool GetDeleteKeyPermission(User objUser);

      bool GetSystemEditPermission(User objUser);

   }
}