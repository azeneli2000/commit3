using System;
using System.Collections.Generic;
using System.Text;

using Digistat.Dal;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public class DriverRepositoryStandardParameterLinkManager : DalManagerBase<DriverRepositoryStandardParameterLink>, IDriverRepositoryStandardParameterLinkManager
    {

        public DriverRepositoryStandardParameterLinkManager(DigistatDBContext context, ILoggerService loggerService)
        {
            mobjDbContext = context;
            mobjLoggerService = loggerService;
        }


    }
}
