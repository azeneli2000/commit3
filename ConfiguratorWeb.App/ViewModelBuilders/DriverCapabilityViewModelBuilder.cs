using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfiguratorWeb.App.Enums;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class DriverCapabilityViewModelBuilder
    {
        public static DriverCapabilityViewModel Build(DriverRepositoryStandardParameterLink source)
        {
            DriverCapabilityViewModel objDest = null;
            try
            {
                if (source != null)
                {
                    objDest = new DriverCapabilityViewModel
                    {
                        DeviceID=source.DeviceId,
                        DeviceText=source.DeviceText,
                        DeviceUnitText=source.DeviceUnitText,
                        Enabled=source.IsEnabled,
                        IDUnit=source.StandardUnitId,
                        IdParameter = source.StandardParameterId,
                        Mnemonic=(source.StandardParameter!=null?source.StandardParameter.Mnemonic:string.Empty),
                        Name = (source.StandardParameter != null ? source.StandardParameter.Description : string.Empty),
                        Sporadic=source.Sporadic,
                        Type = (source.StandardDeviceType != null ? source.StandardDeviceType.Description??string.Empty : string.Empty),
                        TypeShort = (source.StandardDeviceType != null ? source.StandardDeviceType.Print??string.Empty : string.Empty),
                        Unit = (source.StandardUnit != null ? source.StandardUnit.Print : string.Empty),
                        StandardDeviceTypeID = source.StandardDeviceTypeId,
                        MustBeSaved = source.MustBeSaved, 
                        StandardParameterDataType = source.StandardParameter!=null?source.StandardParameter.DataType:string.Empty,
                        StandardParameterPrint = source.StandardParameter!=null?source.StandardParameter.Print:string.Empty,
                        StandardParameterIDAlias = source.StandardParameterIdAlias!=null?source.StandardParameterIdAlias:null, 
                        SporadicModel =new SporadicViewModel { SporadicId = source.Sporadic, SporadicName = ((DriverSporadic)source.Sporadic).ToString() },
                        StandardParameterIsMissing = source.StandardParameter==null,
                        StandardParameterIsWaveForm = source.StandardParameter!=null && new []{"WF","WAVEFORM"}.Contains(source.StandardParameter.DataType.ToUpper())
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }

            return objDest;
        }

        public static IEnumerable<DriverCapabilityViewModel> BuildList(IEnumerable<DriverRepositoryStandardParameterLink> source)
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