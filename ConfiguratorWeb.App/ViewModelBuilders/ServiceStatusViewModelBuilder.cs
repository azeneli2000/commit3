using ConfiguratorWeb.App.Models.Actions;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public class ServiceStatusViewModelBuilder
   {
      public static ServiceStatusViewModel Build(ServiceStatus source)
      {
         ServiceStatusViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new ServiceStatusViewModel
               {
                  Application = source.Application,
                  Hostname = source.Hostname,
                  LastUpdateUTC = source.LastUpdateUTC,
                  Status = source.Status
               };
            }
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }

      public static IEnumerable<ServiceStatusViewModel> BuildList(IEnumerable<ServiceStatus> source)
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
