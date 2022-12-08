using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.CDSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class CDSSRuleViewModelBuilder
   {
      public static CDSSRuleViewModel Build(Digistat.FrameworkStd.Model.CDSS.CDSSRule source)
      {
         CDSSRuleViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new CDSSRuleViewModel
               {
                  ID = source.Id,
                  Name = source.Name,
                  RuleType = source.RuleType,
                  RuleTypeDescr = GetRuleTypeDescr(source.RuleType),
                  DllFile = source.DllFile,
                  DllRuleName = source.DllRuleName,
                  AutoActivate = source.AutoActivate,
                  ExecuteAtStartup = source.ExecuteAtStartup,
                  Interval = source.Interval,
                  IsGeneric = source.IsGeneric,
                  IsGenericDescr = GetRuleDataTypeDescr(source.IsGeneric),
                  KillTimeout = source.KillTimeout,
                  MessageType = source.MessageType,
                  MethodCode = source.MethodCode,
                  Times = source.Times,
                  TriggerType = source.TriggerType,
                  TriggerTypeDescr = GetTriggerTypeDescr(source.TriggerType),
                  ValidityTimeout = source.ValidityTimeout,
                  Options = GetOptions(source.RuleOptions),
                  OutputParameters = GetOutputParameters(source.RuleOutputParameters),
                  IsActive = source.IsActive,

                  Description = source.Description,
                  Code = source.Code,
                  ClientVisible = source.ClientVisible,
                  ClientEditable = source.ClientEditable,
                  ClientConfigurable = source.ClientConfigurable,
                  Uri = source.Uri,
                  ClientCopyable = source.ClientCopyable,

                  Locations = LocationLinks()

               };

               string LocationLinks()
               {
                  if (source.RuleLocationLinks != null)
                  {
                     return String.Join(",",source.RuleLocationLinks.Select((a) => a.IDLocation.ToString()).ToArray());
                  }
                  else
                  {
                     return null;
                  }
               }
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }


      private static string GetRuleDataTypeDescr(bool sourceIsGeneric)
      {
         return (sourceIsGeneric ? "General" : "Patient");
         
      }

      private static string GetRuleTypeDescr(int sourceRuleType)
      {
         string ruleTypeDescr = "";
         switch (sourceRuleType)
         {
            case 0:
               ruleTypeDescr = "DLL";
               break;
            case 1: 
               ruleTypeDescr = "Script";
               break;
         }
         return ruleTypeDescr;
      }
      private static string GetTriggerTypeDescr(int sourceRuleType)
      {
         string triggerTypeDescr = "";
         switch (sourceRuleType)
         {
            case 0:
               triggerTypeDescr = "Periodic";
               break;
            case 1: 
               triggerTypeDescr = "Scheduled";
               break;
            case 2: 
               triggerTypeDescr = "Messages";
               break;
            case 3: 
               triggerTypeDescr = "Multi";
               break;
         }
         return triggerTypeDescr;
      }

      public static IEnumerable<CDSSRuleViewModel> BuildList(IEnumerable<Digistat.FrameworkStd.Model.CDSS.CDSSRule> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch (Exception)
         {

            throw;
         }
      }

      
      private static string GetOutputParameters(ICollection<CDSSRuleOutputParameter> rops)
      {
         string strRet = null;
         StringBuilder objSB = new StringBuilder();
         if (rops != null)
         {
            foreach(CDSSRuleOutputParameter opt in rops)
            {
               //objSB.Append(opt.ParameterName + "," + opt.Type + ","+opt.UM+";");
               //Type and Unit are read from standardParameter
               objSB.Append(opt.ParameterName + ";");
            }
            strRet = objSB.ToString();
         }
         return strRet;
      }
      public static string GetOptions(ICollection<CDSSRuleOption> opts)
      {
         string strRet = null;
         StringBuilder objSB = new StringBuilder();
         if (opts != null && opts.Count>0)
         {
            //foreach(CDSSRuleOption opt in opts)
            //{
            //   objSB.Append(opt.Name + ":" + opt.Value + ";");
            //}
            //strRet = objSB.ToString();
            strRet = JsonConvert.SerializeObject(opts);

         }
         return strRet;
      }
   }
}
