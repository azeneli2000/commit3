using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfiguratorWeb.App.Controllers;
using Digistat.FrameworkStd.Model.CDSS;
using Newtonsoft.Json;

namespace ConfiguratorWeb.App.EntityBuilders
{
    public static class CDSSRuleEntityBuilder
    {
        public static Digistat.FrameworkStd.Model.CDSS.CDSSRule Build(CDSSRuleViewModel source)
        {
          Digistat.FrameworkStd.Model.CDSS.CDSSRule objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new Digistat.FrameworkStd.Model.CDSS.CDSSRule
                    {
                       Id = source.ID,
                       Name = source.Name,
                       RuleType = source.RuleType,
                       DllFile = source.DllFile,
                       DllRuleName = source.DllRuleName,
                       AutoActivate = source.AutoActivate,
                       ExecuteAtStartup = source.ExecuteAtStartup,
                       Interval = source.Interval,
                       IsGeneric = source.IsGeneric,
                       KillTimeout = source.KillTimeout,
                       MessageType = source.MessageType,
                       MethodCode = source.MethodCode,
                       Times = source.Times,
                       TriggerType = source.TriggerType,
                       ValidityTimeout = source.ValidityTimeout,
                       RuleOptions = GetOptions(source.Options,source.ID),
                       RuleOutputParameters = GetOutoutParameters(source.OutputParameters,source.ID),
                       IsTest = source.IsTest,
                       IsActive = source.IsActive,
                       Description = source.Description,
                       Code = source.Code,
                       ClientVisible = source.ClientVisible,
                       ClientEditable = source.ClientEditable,
                       ClientConfigurable = source.ClientConfigurable,
                       Uri = source.Uri,
                       ClientCopyable = source.ClientCopyable,
                       RuleLocationLinks = GetLocationLinks(source.Locations,source.ID)
                       
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }

            return objDest;
        }

        private static ICollection<CDSSRuleLocationLink> GetLocationLinks(string locations, int sourceId)
        {
           List<CDSSRuleLocationLink> locationLinks = new List<CDSSRuleLocationLink>();
           if (locations != null)
              foreach (string locId in locations.Split(","))
              {
                 var item = new CDSSRuleLocationLink();
                 if (int.TryParse(locId, out int lId))
                 {
                    item.IDLocation = lId;
                    item.RuleID = sourceId;
                    locationLinks.Add(item);
                 }
              }

           return locationLinks;
        }

        public static IEnumerable<Digistat.FrameworkStd.Model.CDSS.CDSSRule> BuildList(IEnumerable<CDSSRuleViewModel> source)
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


        public static ICollection<CDSSRuleOption> GetOptions(string strOptions,int id)
      {
         List<CDSSRuleOption> objRet = new List<CDSSRuleOption>();
         if (!string.IsNullOrEmpty(strOptions))
         {
            objRet = JsonConvert.DeserializeObject<List<CDSSRuleOption>>(strOptions);
            foreach (CDSSRuleOption opt in objRet)
            {
               opt.RuleID = id;
            }
            //List<string> objSplitted = strOptions.Split(";").ToList();
            //foreach(string s in objSplitted)
            //{
            //   if (!string.IsNullOrEmpty(s))
            //   {
            //      CDSSRuleOption opt = new CDSSRuleOption
            //      {
            //         RuleID = id,
            //         PatientID = 0,
            //         Name = "",
            //         Value = "",
            //         Type = 0,
            //         MinLimit = null,
            //         MaxLimit = null,
            //         ListItems = "",
            //         Description = "",
            //         IsVisible = true,
            //         Label = null
            //      };
            //      string[] astr =  s.Split(":");
            //      opt.Name = astr[0];
            //      opt.Value = astr[1];
                  
            //      objRet.Add(opt);
            //   }
            //}
         }
         return objRet;
      }
   
        public static ICollection<CDSSRuleOutputParameter> GetOutoutParameters(string strOutParameters,int id)
        {
           List<CDSSRuleOutputParameter> objRet = new List<CDSSRuleOutputParameter>();
           if (!string.IsNullOrEmpty(strOutParameters))
           {
              List<string> objSplitted = strOutParameters.Split(";").ToList();
              foreach(string s in objSplitted)
              {
                 if (!string.IsNullOrEmpty(s))
                 {
                    CDSSRuleOutputParameter opt = new CDSSRuleOutputParameter();
                    string[] astr =  s.Split(",");
                    opt.ParameterName = astr[0];
                    
                    opt.Type = (astr.GetUpperBound(0)>0)?astr[1]:"ST";
                    opt.UM = (astr.GetUpperBound(0)>1)?astr[2]:"99999";
                    opt.RuleID = id;
                    objRet.Add(opt);
                 }
              }
           }
           return objRet;
        }
        
   }

}