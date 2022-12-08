using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class UserReportViewModelBuilder
   {
      public static UserReportViewModel Build(UserReport source)
      {
         UserReportViewModel objDest = null;

         string strStatus = "";
         string strIssuedDate = "";
         string strChangeDate = "";
         switch(source.Status)
         {
            case 0:
               strStatus = "OPENED";
               break;
            case 1:
               strStatus = "CLOSED";
               break;
         }

         int intSnapshotLenght = 0;
         if(source.ReportSnapshot == null)
         {
            intSnapshotLenght = 0; 
         }
         else
         {
            intSnapshotLenght = source.ReportSnapshot.Length;
         }
        
         if (source.StatusChangeDateUTC == null)
         {
            strChangeDate = "";
         }
         else
         {
            strChangeDate = source.StatusChangeDateUTC.Value.ToLocalTime().ToString();

            var test = source.StatusChangeDateUTC.Value.ToLocalTime();
         }
         try
         {
            if (source != null)
            {
               objDest = new UserReportViewModel
               {
                  Id = source.Id,
                  //IssuedOnUTCstr = strIssuedDate,                  
                  IssuedOnUTC = source.IssuedOnUTC.ToLocalTime(),
                  Hostname = source.Hostname,
                  GUID = source.UserId,
                  PatientName = source.PatientFullName,
                  CurrentModule = source.CurrentModule,
                  Description = source.ReportDescription,
                  Snapshot = intSnapshotLenght,
                  Status = (Status)source.Status,
                  StatusNote = source.StatusNote,
                  LocationName = source.LocationName,
                  HU = source.HU,
                  StatusText = strStatus,
                  StatusChangeDateUTC = strChangeDate
               };
            }
         }
         catch (Exception e)
         {

            throw;
         }

         return objDest;
      }

      public static IEnumerable<UserReportViewModel> BuildList(IEnumerable<UserReport> source)
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
