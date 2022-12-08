
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;

namespace ConfiguratorWeb.App.Extensions.Helpers
{
   public static class DropdownValuesHelper
   {

      public static IEnumerable<Models.General.DropdownModel> GetDriverTypeList(IDictionaryService mobjDictionaryService, IEnumerable<int> supportedDriverTypes = null)
      {

         if (supportedDriverTypes == null || !supportedDriverTypes.Any())
         {
            supportedDriverTypes = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDriverTypeValues();
         }

         return supportedDriverTypes
            .Select(x => new Models.General.DropdownModel
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDriverTypeDescription((short)x))

            }).OrderBy(x => x.Text);
      }

      public static IEnumerable<Models.General.DropdownModel> GetConnectionTypeList(IDictionaryService mobjDictionaryService, IEnumerable<int> supportedConnectionTypes = null, int? defaultConnectionTypes = null)
      {

         if (supportedConnectionTypes == null || !supportedConnectionTypes.Any())
         {
            supportedConnectionTypes = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetConnectionTypeValues();
         }

         if (defaultConnectionTypes.HasValue && !supportedConnectionTypes.Any(x => x == defaultConnectionTypes.Value))
         {
            ((List<int>)supportedConnectionTypes).Add(defaultConnectionTypes.Value);
         }

         return supportedConnectionTypes
            .Select(x => new Models.General.DropdownModel
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetConnectionTypeDescription(x))
            }).Where(x => !string.IsNullOrWhiteSpace(x.Text)).OrderBy(x => x.Text);
      }

      public static IEnumerable<Models.General.DropdownModel> GeAlarmSystemTypeNameList(IDictionaryService mobjDictionaryService, short? supportedAlarmSystemType)
      {
         if (!supportedAlarmSystemType.HasValue)
         {
            supportedAlarmSystemType = 0;
         }

         return Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetAlarmSystemTypes(supportedAlarmSystemType)
            .Select(x => new Models.General.DropdownModel
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetAlarmSystemTypeDescription(x), StringParseMethod.Html)
            })
            .OrderBy(x => x.Text)
            .ToList();
      }


      public static IEnumerable<Models.General.DropdownModel> GetDataModelList(IDictionaryService mobjDictionaryService)
      {

         return Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDataModes()
            .Select(x => new Models.General.DropdownModel
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDataModeDescription(x))
            })
         .ToList();
      }

      public static IEnumerable<Models.General.DropdownModel> GetSocketTypeList(IDictionaryService mobjDictionaryService)
      {

         return Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetSocketTypesValues()
            .Select(x => new Models.General.DropdownModel
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetSocketTypeDescription(x))
            })
            .OrderBy(x => x.Text)
            .ToList();
      }

      public static IEnumerable<Models.General.DropdownModel> GetDeviceTypeList(IDictionaryService mobjDictionaryService)
      {

         return Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDeviceTypesValues()
                    .Select(x => new Models.General.DropdownModel
                    {
                       Value = x.ToString(),
                       Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetDeviceTypeDescription(x), StringParseMethod.Html)
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
      }
      public static IEnumerable<Models.General.DropdownModel> GetCdssRuleTypeList(IDictionaryService mobjDictionaryService)
      {

         return Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetRuleTypesValues()
                    .Select(x => new Models.General.DropdownModel
                    {
                       Value = x.ToString(),
                       Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetRuleTypeDescription(x), StringParseMethod.Html)
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
      }


      public static  IEnumerable<Models.General.DropdownModel> GetCdssDataTypeList(IDictionaryService mobjDictionaryService)
      {
         var cdssDataTypeList = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetCdssDataTypesValues()
            .Select(x => new Models.General.DropdownModel
            {
               Value = x.ToString(),
               Text = mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetCdssDataTypeDescription(x), StringParseMethod.Html)
            })
            .OrderBy(x => x.Text)
            .ToList();
         return cdssDataTypeList;
      }
   }
}
