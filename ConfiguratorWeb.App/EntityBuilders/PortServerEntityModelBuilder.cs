using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.Telligence;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class PortServerEntityModelBuilder
   {
      public static PortServer Build(PortServerViewModel source)
      {
         PortServer objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new PortServer
               {
                  Address = source.Address,
                  AdministativeURI = source.AdministativeURI,
                  AuthInfo = source.AuthInfo,
                  DASBroker = source.DASBroker,
                  EncryptionKey = source.EncryptionKey,
                  FirstPort = source.FirstPort,
                  ID = source.ID,
                  IDBED = source.IDBED,
                  Password = source.Password,
                  PortCount = source.PortCount,
                  Type = (short)source.Type,
                  UpdateDate = source.UpdateDate,
                  UserName = source.UserName
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      public static IEnumerable<PortServer> BuildList(IEnumerable<PortServerViewModel> source)
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
