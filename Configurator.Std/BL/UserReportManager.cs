using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Configurator.Std.BL
{
   public class UserReportManager : DalManagerBase<UserReport>, IUserReportManager
   {
      public UserReportManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }

     
      public UserReport GetUserReportById(int id)
      {
         IQueryable<Network> objNetwork = mobjDbContext.Set<Network>();
         IQueryable<UserReport> objDevRepo = mobjDbContext.Set<UserReport>();
         IQueryable<Location> objLocation = mobjDbContext.Set<Location>();
         IQueryable<HospitalUnit> objHU = mobjDbContext.Set<HospitalUnit>();

         try
         {
            IQueryable<UserReport> objRet = from d in objDevRepo where d.Id == id
                                            join s in objLocation on d.Network.LocationRef equals s.Id into ps1
                                            from leftHL1 in ps1.DefaultIfEmpty()
                                            join a in objHU on leftHL1.HospitalUnitGuid equals a.GUID into ps
                                            from leftHL in ps.DefaultIfEmpty()
                                            where (leftHL == null || leftHL.Current == true) && (leftHL1 == null || leftHL1 != null)
                                            select new UserReport
                                            {
                                               IssuedOnUTC = d.IssuedOnUTC,
                                               Id = d.Id,
                                               PatientId = d.PatientId,
                                               CurrentModule = d.CurrentModule,
                                               Hostname = d.Hostname,
                                               ReportDescription = d.ReportDescription,
                                               Status = d.Status,
                                               StatusChangeDateUTC = d.StatusChangeDateUTC.Value,
                                               UserId = d.UserId,
                                               UserVersion = d.UserVersion,
                                               ReportSnapshot = d.ReportSnapshot,
                                               Sync = d.Sync,
                                               StatusNote = d.StatusNote,
                                               LocationName = leftHL1.LocationName,
                                               PatientFullName = d.Patient.FullName,
                                               HU = leftHL.Name,
                                               Patient = d.Patient,
                                            };

            return objRet.SingleOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read UserReportItem {0} records from DB", typeof(UserReport).Name);
            string message = string.Format("Unable to read all UserReportItem records from DB", typeof(UserReport).Name);
            throw new Exception(message, e);
         }

      }

      public IQueryable<UserReport> GetUserReports(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<UserReport, object>>> includePredicates = null)
      {
         try
         {
            IQueryable<Network> objNetwork = mobjDbContext.Set<Network>();
            IQueryable<UserReport> objDevRepo = mobjDbContext.Set<UserReport>();
            IQueryable<Location> objLocation = mobjDbContext.Set<Location>();
            IQueryable<HospitalUnit> objHU = mobjDbContext.Set<HospitalUnit>();

            IQueryable<UserReport> objResult =  from d in objDevRepo
            join s in objLocation on d.Network.LocationRef equals s.Id into ps1 from leftHL1 in ps1.DefaultIfEmpty()
            join a in objHU on leftHL1.HospitalUnitGuid equals a.GUID   into ps from leftHL in ps.DefaultIfEmpty()
            where (leftHL == null || leftHL.Current == true) && (leftHL1 == null || leftHL1 != null)
            select new UserReport
            {
               IssuedOnUTC = d.IssuedOnUTC,
               Id = d.Id,
               PatientId = d.PatientId,
               CurrentModule = d.CurrentModule,
               Hostname = d.Hostname,
               ReportDescription = d.ReportDescription,
               Status = d.Status,
               StatusChangeDateUTC = d.StatusChangeDateUTC.Value,
               UserId = d.UserId,
               UserVersion = d.UserVersion,
               ReportSnapshot = d.ReportSnapshot,
               Sync = d.Sync,
               StatusNote = d.StatusNote,
               LocationName = leftHL1.LocationName,
               PatientFullName = d.Patient.FullName,
               HU = leftHL.Name,
               Patient = d.Patient,   
            };

   
            return objResult;

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read UserReportItem {0} records from DB", typeof(UserReport).Name);
            string message = string.Format("Unable to read all UserReportItem records from DB", typeof(UserReport).Name);
            throw new Exception(message, e);
         }
      }

      public void UpdateUserReport(int id , short status , DateTime? dateStatusUpdate, string statusnote)
      {
         UserReport objentity = new UserReport { Id=id, Status=status };
         var objDevRepo = mobjDbContext.Set<UserReport>();
         var objId = mobjDbContext.Set<UserReport>().Single(x => x.Id == id);
         objId.Status = status;
         objId.StatusChangeDateUTC = dateStatusUpdate;
         objId.StatusNote = statusnote;
         mobjDbContext.SaveChanges();




      }

      public void UpdateUserReportSnap(int id, byte[] status)
      {
         UserReport objentity = new UserReport { Id = id, ReportSnapshot = status };
         var objDevRepo = mobjDbContext.Set<UserReport>();
         var objId = mobjDbContext.Set<UserReport>().Where(x => x.Id == id).SingleOrDefault();
         objId.ReportSnapshot = status;
         mobjDbContext.SaveChanges();

      }

      public byte[] GetSnapshot (int id)
      {
         var objDevRepo = mobjDbContext.Set<UserReport>();
         var result = mobjDbContext.Set<UserReport>().Where(x => x.Id == id).Select(x=>x.ReportSnapshot).SingleOrDefault();

         return result;
      }

   }
}
