using System;
using System.Collections.Generic;
using System.Text;
using Digistat.FrameworkStd.Model.Online;

namespace Configurator.Std.BL.Online
{
    public interface IOnlineQueriesManager: Digistat.Dal.Interfaces.IDalManagerBase<OnlineQuery>
    {
        
        bool Delete(int queryID,string usrAbbrev,string usrID);
        
    }
}
