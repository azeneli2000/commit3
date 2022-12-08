using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.FluidBalance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Configurator.Std.BL
{
   public class FluidBalanceManager : DalManagerBase<FluidBalanceItemModel>, IFluidBalanceManager
   {
  
      public FluidBalanceManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }
     
      public IQueryable<FluidBalanceItemModel> GetFBStandarItem(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<FluidBalanceItemModel, object>>> includePredicates = null)
      {
         try
         {
            var objDevRepo = mobjDbContext.Set<FluidBalanceItemModel>().Include(d => d.Location);
            IQueryable<FluidBalanceItemModel> repository = from l in objDevRepo.AsQueryable()
                                                      select new FluidBalanceItemModel
                                                      {
                                                         Name = l.Name,
                                                         Description = l.Description,
                                                         Id = l.Id,
                                                         Labels = l.Labels,
                                                         Mode = l.Mode,
                                                         Once = l.Once,
                                                         Permanent = l.Permanent,
                                                         Sql = l.Sql,
                                                         Index = l.Index,
                                                         
                                                         Location = l.Location
                                                      };          

            if (includePredicates != null && includePredicates.Count() > 0)
            {
               includePredicates.ToList().ForEach(x => repository = repository.Include(x));
            }
            else
            {
               repository = repository.OrderBy(i => i.Index);
            }

            if (pageNumber > 0)
            {
               repository = repository.Skip((pageNumber - 1) * pageSize);
            }

            if (pageSize > 0)
            {
               repository = repository.Take(pageSize);
            }
            return repository;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Unable to read FBStandarItem {0} records from DB", typeof(FluidBalanceItemModel).Name);
            string message = string.Format("Unable to read all FBStandarItem records from DB", typeof(FluidBalanceItemModel).Name);
            throw new Exception(message, e);
         }
      }

      public FluidBalanceItemModel GetFBStandarItemById(int id)
      {
         FluidBalanceItemModel result = null;

         try
         {
            IQueryable<FluidBalanceItemModel> repository = mobjDbContext.Set<FluidBalanceItemModel>().Include(d => d.Location);
            result = repository.Where(x => x.Id == id).SingleOrDefault();
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, string.Format("Error reading FBStandarItem with id {0} from DB", id));
            throw new Exception(string.Format("Error reading FBStandarItem with id {0} from DB", id), e);
         }
         return result;
      }


      public FluidBalanceItemModel CreateFBStandardItem(FluidBalanceItemModel objNewItem)
      {
         try
         {
            var objRepo = mobjDbContext.Set<FluidBalanceItemModel>();
            try
            {
               objNewItem.Index = objRepo.Max(i => i.Index) + 1;
            } 
            catch (Exception)
            {
               objNewItem.Index = 1;
            }
            //Add new FBStandardItem device

            var itm = objRepo.Add(objNewItem);
            mobjDbContext.SaveChanges();

            return itm.Entity;

         }
         catch (Exception e)
         {
            string message = "Error Create";
            mobjLoggerService.ErrorException(e, message);
            throw new Exception(message, e);
         }
      }

      public FluidBalanceItemModel UpdateFBStandardItem(FluidBalanceItemModel objUpdated)
      {
         FluidBalanceItemModel objRet = null;
         try
         {
            //Update FBSTandardItem Systems
            var objRepo = mobjDbContext.Set<FluidBalanceItemModel>();
            var itm = objRepo.Update(objUpdated);
            mobjDbContext.SaveChanges();

            objRet = itm.Entity;
         }
         catch (Exception e)
         {
            string errMsg = "Error on creating FBSTandardItem";
            mobjLoggerService.ErrorException(e, errMsg);
            throw new Exception(errMsg, e);
         }
         return objRet;
      }

      public bool DeleteFBStandardItem(int id)
      {
         //string strRet = string.Empty;

         bool bolResult = false; 
         try
         {
               var objRepo = mobjDbContext.Set<FluidBalanceItemModel>();
               FluidBalanceItemModel objItem = objRepo.Where(p => p.Id== id).FirstOrDefault();
               objRepo.Remove(objItem);
               mobjDbContext.SaveChanges();
            bolResult = true; 
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, string.Format("Error deleting FBSTandardItem System {0}", id));
            string message = string.Format("Error deleting FBSTandardItem System {0}", id);
            throw new Exception(message, e);
         }
         //return strRet;

         return bolResult; 
      }
   }
}
