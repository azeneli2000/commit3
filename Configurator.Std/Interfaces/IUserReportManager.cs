using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Configurator.Std.BL
{
   public interface IUserReportManager : Digistat.Dal.Interfaces.IDalManagerBase<UserReport>
   {
      IQueryable<UserReport>  GetUserReports(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<UserReport, object>>> includePredicates = null);
      UserReport GetUserReportById(int id);
      //UserReport CreateUserReport(UserReport objNewItem);
      void  UpdateUserReport(int id, short status, DateTime? dateStatusUpdate, string statusnote);
      //bool DeleteFBStandardItem(int id);
      byte[] GetSnapshot(int id);

      void UpdateUserReportSnap(int id, byte[] status);
   }
}
