using Digistat.FrameworkStd.Model;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL
{
   public interface ISystemOptionsManager : Digistat.Dal.Interfaces.IDalManagerBase<SystemOption>
   {
      SystemOption Get(string guid);    
      List<SystemOption> GetByApplicationName(string applicationName);
      //void InsertSystemOption(UMS.Framework.Data.Types.SystemOption systemOption);
      //SystemOptions LoadSystemOptions(string applicationName);
      void UpdateValue(string guid, string value);
      void Delete(string entityId);
      IQueryable<SystemOption> GetSystemOptions(bool convertAppNameEmptyToNull=false);
      SystemOption Update(SystemOption option, string usrAbbrev, string strHostname);
      SystemOption Create(SystemOption option, string usrAbbrev, string strHostname);
   }
}