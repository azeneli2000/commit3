using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Services
{
   public interface IMenuService
   {
      List<MenuViewModel> GetMenuForUser(User usr);

      List<MenuViewModel> GetMenuForCurrentUser();
      String GetCurrentUserName();
      String GetCurrentUserAbbrev();
      void ClearCache();
   }
}
