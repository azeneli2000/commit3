using Digistat.FrameworkStd.Model;
using ConfiguratorWeb.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.UMSLegacy;
using Digistat.FrameworkStd.Model.DAS3Plus;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class OutputStateViewModelBuilder
   {
      public static DasOutputStateViewModel Build(DasOutputState source, IDictionaryService dictionarySrv, bool clearStartEndDate=false)
      {
         DasOutputStateViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new DasOutputStateViewModel
               {
                  IsNew = false,
                  LocationId = source.LocationId,
                  LocationDescription = source.LocationId <= 0 ? string.Empty : source.Location.LocationName,
                  BedId = source.BedId,
                  BedDescription = source.BedId <= 0 ? string.Empty : source.Bed.Name,
                  //PatientId = source.PatientId,
                  //PatientDescription = source.PatientId <=0 ? string.Empty : string.Format("{0} {1}", source.Patient.FamilyName, source.Patient.GivenName),
                  Type = source.Type,
                  TypeDescription = dictionarySrv.XLate(UMSFrameworkParser.GetOutputTypeDescription(source.Type)),
                  IsSystem = source.IsSystem,
                  SamplingSeconds = source.SamplingSeconds,
                  StartDateUtc = source.StartDateUtc,
                  StopDateUtc = source.StopDateUtc,
               };
               if (clearStartEndDate)
               {
                  if(source.StartDateUtc.Month==1 && source.StartDateUtc.Day == 1 && source.StartDateUtc.Year == 1753)
                  {
                     objDest.StartDateUtc = null;
                  }
                  if (source.StopDateUtc.Month == 1 && source.StopDateUtc.Day == 1 && source.StopDateUtc.Year == 1753)
                  {
                     objDest.StopDateUtc = null;
                  }
               }
            }
         }
         catch (Exception e)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<DasOutputStateViewModel> BuildList(IEnumerable<DasOutputState> source, IDictionaryService dictionarySrv)
      {
         try
         {
            return source.Select(x => Build(x, dictionarySrv));
         }
         catch (Exception)
         {

            throw;
         }
      }


      //private static string GetDeviceTypeDesc(string devTypeInt)
      //{
      //   StringBuilder objSb = new StringBuilder();
      //   if(!string.IsNullOrEmpty(devTypeInt))
      //   {
      //      List<string> objDevDescs = new List<string>();
      //      List<string> objDevTypes = devTypeInt.Split(',',StringSplitOptions.RemoveEmptyEntries).ToList();
      //      foreach(string s in objDevTypes)
      //      {
      //         int intTmp = 0;
      //         if(Int32.TryParse(s, out intTmp))
      //         {
      //            DeviceType objDevTypeEn = ((DeviceType)intTmp);
      //            objDevDescs.Add(objDevTypeEn.ToString());
      //         }
      //      }
      //      objSb.Append(String.Join(", ", objDevDescs.ToArray()));
      //   }
      //   return objSb.ToString();
      //}
      //public static string getAlarmSystemType(DriverRepository source, IDictionaryService dictionarySrv)
      //{
      //   var _val = source.AlarmSystemType.HasValue? source.AlarmSystemType.Value: Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.AlarmSystemDefaultValue();
      //   return dictionarySrv.XLate(UMSFrameworkParser.GetAlarmSystemTypeDescription(_val), StringParseMethod.Html);
      //}
      //public static string getAlarmSupportType(short source, IDictionaryService dictionarySrv)
      //{
      //   return dictionarySrv.XLate(UMSFrameworkParser.GetAlarmSupportTypeDescription(source), StringParseMethod.Html);
      //}
   }
}