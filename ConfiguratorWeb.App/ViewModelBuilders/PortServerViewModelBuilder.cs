using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using ConfiguratorWeb.App.Models.Telligence;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digistat.FrameworkStd.Enums;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class PortServerViewModelBuilder
   {
      private static IList<Bed> _beds= new List<Bed>(0);
      public static PortServerViewModel Build(PortServer source)
      {

         return Build(source, _beds);
      }

      public static PortServerViewModel Build(PortServer source , IList<Bed> bedsList )
      {
         _beds = bedsList;
         PortServerViewModel objDest = null;
         try
         {
            if (source != null)
            {
               
               objDest = new PortServerViewModel
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
                  Type = (Digistat.FrameworkStd.Enums.PortServerType)source.Type,
                  TypeDescription = ((Digistat.FrameworkStd.Enums.PortServerType)source.Type).GetDisplayAttribute(),
                  UpdateDate = source.UpdateDate,
                  UserName = source.UserName,
                  BedName = "",
                  IsTelligenceType = (source.Type==0)
               };
               if (bedsList.Count>0 && objDest.IDBED>0)
               {
                  Bed bed = bedsList.FirstOrDefault(i=>i.Id==objDest.IDBED);
                  if (bed != null)
                  {
                     objDest.BedName = bed.Name;
                  }
                  
               }
               if (source.ID==0)
               {
                  objDest.Type = PortServerType.Lantronix;
                  objDest.IsTelligenceType = false;
               }
               
            }
            
               
         }
         catch (Exception)
         {
         }

         return objDest;
      }

      private static bool checkIsTelligenceType(int sourceId)
      {
         throw new NotImplementedException();
      }

      public static IEnumerable<PortServerViewModel> BuildList(IEnumerable<PortServer> source, IList<Bed> bedsList)
      {
         _beds = bedsList;
         try
         {
            return source.Select(Build );
         }
         catch (Exception)
         {

            throw;
         }
      }
      public static IEnumerable<PortServerViewModel> BuildList(IEnumerable<PortServer> source)
      {
         try
         {
            return source.Select(Build );
         }
         catch (Exception)
         {

            throw;
         }
      }
      
   }
}
