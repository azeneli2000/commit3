using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model.Online;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class ValidationParameterViewModelBuilder
    {
        public static ValidationParameterViewModel Build(ValidationParameter source)
        {
            ValidationParameterViewModel objDest = null;
            try
            {
               if (source != null)
               {
                  objDest = new ValidationParameterViewModel
                  {
                     ParameterID = source.ParameterID,
                     UnitOfMeasureID = source.UnitOfMeasureID,
                     DriverID = source.DriverID,
                     DeviceID = source.DeviceID,
                     ChannelID = source.ChannelID,
                     GroupID = source.GroupID,
                     Decimal = source.Decimal,
                     Index = source.Index,
                     IsAlwaysVisible = source.IsAlwaysVisible,
                     IsAutoExecuteQuery = source.IsAutoExecuteQuery,
                     IsEditable = source.IsEditable,
                     IsMandatory = source.IsMandatory,
                     IsManuallyAlarmable = source.IsManuallyAlarmable,
                     IsSimpleChoiceOnly = source.IsSimpleChoiceOnly,
                     PlausibilityMax = source.PlausibilityMax,
                     PlausibilityMin = source.PlausibilityMin,
                     RangeMax = source.RangeMax,
                     RangeMin = source.RangeMin,
                     SectionID = source.SectionID,
                     SimpleChoiceGroup = source.SimpleChoiceGroup,
                     SQLQuery = source.SQLQuery,
                     ParameterDescription = (source.StdParameter != null) ? source.StdParameter.Description:null,
                     ParameterPrint = (source.StdParameter != null) ? source.StdParameter.Print : null,
                     ParameterMnemonic = (source.StdParameter != null) ? source.StdParameter.Mnemonic : null,
                     ParameterDataType = (source.StdParameter != null) ? source.StdParameter.DataType : null,
                     UnitDescription = (source.StdUnit != null) ? source.StdUnit.Description : null,
                     UnitPrint = (source.StdUnit != null) ? source.StdUnit.Print : null,
                     SectionDescription = (source.Section!=null)?source.Section.Name:null,
                     SectionIndex = (source.Section!=null)?source.Section.Index:0,
                     DriverName = (source.DriverInfo !=null )?source.DriverInfo.DriverName:source.DriverID,
                  };
               }
            }
            catch (Exception e)
            {

                throw;
            }

            return objDest;
        }

        public static ICollection<ValidationParameterViewModel> BuildList(ICollection<ValidationParameter> source)
        {
            try
            {
               if (source != null)
               {
                  return source.Select(Build).ToList();
               }
               else
               {
                  return null;
               }
               
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}