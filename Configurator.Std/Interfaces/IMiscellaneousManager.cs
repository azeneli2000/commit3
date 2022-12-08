using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IMiscellaneousManager : Digistat.Dal.Interfaces.IDalManagerBase<Miscellanea>
   {
      Miscellanea Get(string key);     
      string GetValue(string key);

      Miscellanea GetFromID(int id);
      void Remove(int id);

      void Delete(int id);
      
   }
}