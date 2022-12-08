using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class DriverCapabilityEntityBuilder
   {
      public static DriverRepositoryStandardParameterLink Build(DriverCapabilityViewModel source)
      {
         DriverRepositoryStandardParameterLink objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DriverRepositoryStandardParameterLink
               {
                  DeviceText = source.DeviceText,
                  DeviceUnitText = source.DeviceUnitText,
                  DriverRepositoryId = source.DriverRepositoryId,
                  DeviceId = source.DeviceID,
                  StandardParameterId= source.IdParameter,
                  StandardParameter = new StandardParameter {Mnemonic=source.Mnemonic, Description = source.Name,DataType=source.StandardParameterDataType },
                  Sporadic=Convert.ToInt16(source.Sporadic),
                  IsEnabled= source.Enabled,
                  StandardUnitId= source.IDUnit,
                  StandardDeviceTypeId = source.StandardDeviceTypeID,
                  StandardDeviceType = new StandardDeviceType { Description=source.Type},
                  StandardUnit= new StandardUnit {  Description=source.Unit ,Print = source.StandardParameterPrint},
                  MustBeSaved = source.MustBeSaved,
                  StandardParameterIdAlias = source.StandardParameterIDAlias!=null&& source.StandardParameterIDAlias.Trim().ToUpper()!="NULL"? source.StandardParameterIDAlias : null ,
                  
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DriverRepositoryStandardParameterLink> BuildList(IEnumerable<DriverCapabilityViewModel> source)
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
   }
}
