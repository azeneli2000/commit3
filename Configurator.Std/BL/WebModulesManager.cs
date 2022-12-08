using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.ControlBar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.BL
{
   public class WebModulesManager : DalManagerBase<WebModule>, IWebModulesManager
   {
      public WebModulesManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }
   }
}
