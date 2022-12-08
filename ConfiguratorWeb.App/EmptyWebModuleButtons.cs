using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.ControlBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App
{
 
   public class EmptyWebModuleButtons : IWebModuleButtonsService
   {
      public List<WebModuleButton> WebModuleButtons => new List<WebModuleButton>();

      public List<WebModule> WebModulesWithButtons
      {
         get
         {
            return null;
         }
      }

      public List<string> GetModulesList(User user)
      {
         return new List<string>();
      }

      public List<string> GetTotalModuleList()
      {
         return new List<string>();
      }

      public void InitWebModules()
      {
         //Do nothing
      }

      public void ReloadWebModuleButtons()
      {
         //Do nothing
      }
   }
}
