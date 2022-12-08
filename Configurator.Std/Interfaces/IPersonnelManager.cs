using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IPersonnelManager : Digistat.Dal.Interfaces.IDalManagerBase<Personnel>
   {    
      Personnel Get(string id);
      void Remove(string personnelId);
   }
}