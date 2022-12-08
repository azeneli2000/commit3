using System;
using System.Collections.Generic;
using System.Linq;

using Configurator.Std.BL.Hubs;
using Microsoft.EntityFrameworkCore;

using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System.Linq.Expressions;

namespace Configurator.Std.BL
{
   public class TelligenceServerManager : DalManagerBase<TelligenceServer>, ITelligenceServerManager
   {
      
      private readonly IMessageCenterManager mobjMsgCtrMgr;
      private readonly IDictionaryService mobjDicSvc;
      
      #region Costructors

      public TelligenceServerManager(DigistatDBContext context, IMessageCenterManager msgCtrMgr, ILoggerService loggerService,IDictionaryService dicSvc)
      {
         mobjMsgCtrMgr = msgCtrMgr;
         mobjDbContext = context;
         mobjLoggerService = loggerService;
         mobjDicSvc = dicSvc;
      }

      #endregion

      public TelligenceServer Get(int id)
      {
         TelligenceServer result = null;

         try
         {
            IQueryable<TelligenceServer> repository = mobjDbContext.Set<TelligenceServer>();
            result = repository.Where(x => x.ts_ID == id).SingleOrDefault();

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e,string.Format( "Error reading TelligenceServer with id {0} from DB", id));
            throw new Exception(string.Format("Error reading TelligenceServer with id {0} from DB", id), e);
         }
         

         return result;

      }

      public string Delete(int id)
      {
         string strRet = string.Empty;
         try
         {
            var tlSystemRepo = mobjDbContext.Set<TelligenceSystem>();
            int intNSystems = tlSystemRepo.Where(p => p.ty_ts_ID == id).Count();
            if (intNSystems > 0)
            {
               strRet = mobjDicSvc.XLate("Cannot delete a Telligence Server bound to Telligence Systems. Remove Telligence Systems first.");
            }
            else
            {
               var objTLServerRepo = mobjDbContext.Set<TelligenceServer>();
               TelligenceServer tlServerItem = objTLServerRepo.Where(p => p.ts_ID == id).FirstOrDefault();
               objTLServerRepo.Remove(tlServerItem);
               mobjDbContext.SaveChanges();
            }

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, string.Format("Error deleting Telligence Server {0}", id));
            string message = string.Format("Error deleting Telligence Server {0}", id);
            throw new Exception(message, e);
         }
         return strRet;
      }

      //public IEnumerable<TelligenceServer> GetServerAll(int pageNumber = 0, int pageSize = 0)
      //{

      //   List<TelligenceServer> result;


      //   try
      //   {
      //      IQueryable<TelligenceServer> repository = mobjDbContext.Set<TelligenceServer>();

      //      if (pageNumber > 0)
      //      {
      //         repository = repository.Skip((pageNumber - 1) * pageSize);
      //      }

      //      if (pageSize > 0)
      //      {
      //         repository = repository.Take(pageSize);
      //      }

      //      result = repository.ToList();


      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to read all TelligenceServer from DB");
      //      string message = string.Format("Unable to read all TelligenceServer from DB");
      //      throw new Exception(message, e);
      //   }


      //   return result;

      //}


      //public IQueryable<TelligenceServer> GetQueriable(IEnumerable<System.Linq.Expressions.Expression<Func<TelligenceServer, object>>> includePredicates = null)
      //{
      //   IQueryable<TelligenceServer> result;

      //   try
      //   {
      //      result = mobjDbContext.Set<TelligenceServer>();
      //      if (includePredicates != null && includePredicates.Count() > 0)
      //      {
      //         includePredicates.ToList().ForEach(x => result = result.Include(x));
      //      }
      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e, "Unable to retrieve queriable for TelligenceServer entity");
      //      string message = string.Format("Unable to retrieve queriable for TelligenceServer entity");
      //      throw new Exception(message, e);
      //   }
      //   return result;

      //}

      //public TelligenceServer CreateServer(TelligenceServer tlSrv)
      //{


      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var tlserverRepo = mobjDbContext.Set<TelligenceServer>();

      //      //Prevent duplications
      //      TelligenceServer loadedTlSrv = tlserverRepo.SingleOrDefault(x => x.ts_serverurl.Trim().ToUpper() == tlSrv.ts_serverurl.Trim().ToUpper());
      //      if (loadedTlSrv != null)
      //      {
      //         throw new Exception(string.Format("Unable to create Telligence Server {0}; URL already exists.", tlSrv.ts_serverurl));
      //      }



      //      //Create new record
      //      tlserverRepo.Add(tlSrv);

      //      mobjDbContext.SaveChanges();
      //      mobjDbContext.CommitTransaction();

      //      return tlSrv;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, string.Format("Error creating Telligence Server {0}",tlSrv.ts_serverurl));
      //      string message = string.Format("Error creating Telligence Server {0}", tlSrv.ts_serverurl);
      //      throw new Exception(message, e);
      //   }

      //}


      //public TelligenceServer UpdateServer(TelligenceServer tserver)
      //{       
      //   try
      //   {

      //      mobjDbContext.BeginTransaction();

      //      var repository = mobjDbContext.Set<TelligenceServer>();

      //    repository.Update(tserver);

      //      mobjDbContext.SaveChanges();

      //      mobjDbContext.CommitTransaction();

      //      return tserver;

      //   }
      //   catch (Exception e)
      //   {
      //      mobjDbContext.RollbackTransaction();
      //      mobjLoggerService.ErrorException(e, string.Format("Error updating telligenceserver with id {0}", tserver.ts_ID));
      //      string message = string.Format("Error updating telligenceserver with id {0}", tserver.ts_ID);
      //      throw new Exception(message, e);
      //   }

      //}

   }
}
