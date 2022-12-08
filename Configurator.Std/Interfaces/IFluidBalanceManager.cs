using Digistat.FrameworkStd.Model.FluidBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Configurator.Std.BL
{
   public interface IFluidBalanceManager : Digistat.Dal.Interfaces.IDalManagerBase<FluidBalanceItemModel>
   {
      IQueryable<FluidBalanceItemModel> GetFBStandarItem(int pageNumber = 0, int pageSize = 0, IEnumerable<Expression<Func<FluidBalanceItemModel, object>>> includePredicates = null);
      FluidBalanceItemModel GetFBStandarItemById(int id);
      FluidBalanceItemModel CreateFBStandardItem(FluidBalanceItemModel objNewItem);
      FluidBalanceItemModel UpdateFBStandardItem(FluidBalanceItemModel objUpdated);
      bool DeleteFBStandardItem(int id);
      
   }
}
