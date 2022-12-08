using System.Collections.Generic;

/// <summary>
/// Represent a whole set of transalations for a given 
/// combination (tuple) of DictionaryKey + Translation.
/// </summary>
namespace Configurator.Std.BL.Dictionary {
   public class TranslationsForKeyAndModule {
      private Dictionary<string, string> _translations;

      public TranslationsForKeyAndModule() 
      {
         _translations = new Dictionary<string, string>();
      }

      public TranslationsForKeyAndModule(List<string> languages) 
      {
         _translations = new Dictionary<string, string>();
         foreach (string s in languages)
            _translations.Add(s, string.Empty);
      }
      
      public string Id { get; set; }

      public string DictionaryKey { get; set; }

      public string Module { get; set; }

      public string Description { get; set; }

      public bool? IsSystem { get; set; }
      
      public Dictionary<string, string> Translations { get {return _translations; } }

   }

}
