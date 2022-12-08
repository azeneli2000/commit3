using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configurator.Std.BL.CDSS;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.CDSS;

namespace Configurator.Std.BL.Vitals
{
   public interface ICDSSManager
   {
      List<CDSSRule> GetAll(bool validOnly = false);
      CDSSRule Get(int id);

      CDSSRule Update(CDSSRule entity);
      CDSSRule UpdateCustom(CDSSRule entity, bool clearAll = false);

      CDSSRule Create(CDSSRule entity);

      CDSSRule Delete(CDSSRule entity);

      //Tuple<bool,IEnumerable<string> > Compile(CDSSRule rule);

      //Task<CDSSAnswer> GetDllListByMessage(string tokenId);
      //Task<CDSSAnswer> GetDllMethodListByMessage(string strDllFileName,string tokenId);
      //Task<CDSSAnswer> GetDllOutputParamsByMessage(string strRuleName,string strDllFileName,string tokenId);
      //Task<CDSSAnswer> CompileByMessage(int id,string tokenId);
      //Task<CDSSAnswer> RunningTestByMessage(int id, string inputValues,string tokenId);
      List<CDSSRuleOutputParameter> GetAllOutputParamters(bool v);
      int CheckDllRuleMethod(string dllFile, string dllRuleName, int iD);
      ICollection<DriverRepositoryStandardParameterLink> GetAllCapabilities();
      bool SaveCapabilitiesAndSendMessage(ICollection<DriverRepositoryStandardParameterLink> capabilities);
      //Task<CDSSAnswer> GetDllSettingsByMessage(string ruleMetodh, string dllName,string tokenId);
      
      CDSSAnswer GetDllListByMessageSync(string tokenId);
      CDSSAnswer GetDllMethodListByMessageSync(string strDllFileName, string tokenId);
      CDSSAnswer GetDllOutputParamsByMessageSync(string strRuleName, string strDllFileName, string tokenId);
      CDSSAnswer GetDllSettingsByMessageSync(string ruleMetodh, string dllName, string tokenId);
      CDSSAnswer CompileByMessageSync(int id, string tokenId);
      CDSSAnswer RunningTestByMessageSync(int id, string inputValues, string tokenId);
      
   }
}