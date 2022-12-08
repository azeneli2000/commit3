using System;
using System.Collections.Generic;
using System.Linq;
using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
namespace ConfiguratorWeb.App.Builders{
   
   public class DictionaryRowGridBuilder {
      
      /* Use the keypairs in the given dictionary to build a new TranslationsForKeyAndModule. */
      public static Configurator.Std.BL.Dictionary.TranslationsForKeyAndModule GetTranslationsForKeyAndModule(Dictionary<string, string> d) {
         try {
            Configurator.Std.BL.Dictionary.TranslationsForKeyAndModule ret = new Configurator.Std.BL.Dictionary.TranslationsForKeyAndModule();
            foreach (string k in d.Keys) {
               //CA1304 e CA1308 consigliano k.ToUpperInvariant() e modificare i case non lo faccio ora per non ritestare.... 
               // ....alla prima occasione sarebbe meglio farlo
               switch (k.ToLower()) {
                  // These come from the grid... NOP.
                  case "sort": case "group": case "filter":
                     break;
                  // start of real data items.
                  case "id":
                     ret.Id = d[k];
                     break;
                  case "dictionarykey":
                     ret.DictionaryKey = d[k]; 
                     break;
                  case "module":
                     ret.Module = d[k]; 
                     break;
                  case "issystem":
                     ret.IsSystem = Convert.ToBoolean(d[k]);
                     break;
                  case "description":
                     ret.Description = d[k];
                     break;
                  //otherwise, assume it's a language...
                  default:   
                     ret.Translations.Add(k, d[k]); 
                     break;
               }
            }
            if (string.IsNullOrEmpty(ret.Module))
               ret.Module = string.Empty;
            return ret;
         } 
         catch { throw; }
      }

   } // class

} // namespace
