using System.Collections.Generic;

using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;

namespace Configurator.Std.BL.DAS3Plus
{
   /// <summary>
   /// IDASBrokerManager doesn't inherits IDALManagerBase because data are retrieved by systemoptions.
   /// A "DASBroker" table or similar does not exits.
   /// </summary>
   public interface IDASBrokerManager 
   {


      List<DASBroker> GetList();

   }
}