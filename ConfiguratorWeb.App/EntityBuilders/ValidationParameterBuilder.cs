using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Online;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class ValidationParameterBuilder
   {

      public static ValidationParameter Build(ValidationParameterViewModel source)
      {
         ValidationParameter objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ValidationParameter
               {
                  
                ChannelID = source.ChannelID??string.Empty,
                Decimal = source.Decimal,
                DeviceID = source.DeviceID,
                DriverID = source.DriverID,
                GroupID = source.GroupID,
                Index = source.Index,
                IsAlwaysVisible = source.IsAlwaysVisible,
                IsAutoExecuteQuery = source.IsAutoExecuteQuery,
                IsEditable = source.IsEditable,
                IsMandatory = source.IsMandatory,
                IsManuallyAlarmable = source.IsManuallyAlarmable,
                IsSimpleChoiceOnly = source.IsSimpleChoiceOnly,
                ParameterID = source.ParameterID,
                PlausibilityMax = source.PlausibilityMax,
                PlausibilityMin = source.PlausibilityMin,
                RangeMax = source.RangeMax,
                RangeMin = source.RangeMin,
                SectionID =source.SectionID,
                SimpleChoiceGroup = source.SimpleChoiceGroup,
                SQLQuery = source.SQLQuery,
                UnitOfMeasureID =source.UnitOfMeasureID
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static ICollection<ValidationParameter> BuildList(ICollection<ValidationParameterViewModel> source)
      {
         try
         {
            return source.Select(Build).ToList();
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
