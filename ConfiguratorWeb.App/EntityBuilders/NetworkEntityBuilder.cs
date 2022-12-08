using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class NetworkEntityBuilder
   {
      public static Network Build(NetworkViewModel source)
      {
         Network objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new Network
               {
                  Id = source.Id,
                  AllowEdit = source.AllowEdit,
                  BedRef = GetBedID(source),
                  ControlbarCurrentVersion = source.ControlbarCurrentVersion,
                  CultureInfo = source.CultureInfo,
                  CurrentContactRef = source.CurrentContactRef,
                  CurrentProductName = source.CurrentProductName,
                  CurrentProductVersion = source.CurrentProductVersion,
                  DeployProductName = source.DeployProductName,
                  DeployProductVersion = source.DeployProductVersion,
                  HostName = source.HostName,
                  IpAddress = source.IpAddress,
                  IsEnabled = source.IsEnabled,
                  LastConnection = source.LastConnection,
                  LocationRef = source.LocationID,//source.DefaultLocation!=null?source.DefaultLocation.ID:0,
                  LockBed = source.LockBed,
                  MacAddress = source.MacAddress,
                  Modules = source.Modules,
                  Type = (short)source.Type,
                  UserId = source.UserId,
                  UserVersion = source.UserVersion,
                  WebMenu = source.WebMenu,
                  WebModules = source.WebModules,
                  WorkstationLabel = source.WorkstationLabel,
                  NetworkBedLinks = BuildNetworkBedLinks(source.Id,source.BedList),
                  
                  };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }

      private static int? GetBedID(NetworkViewModel source)
      {
         int? intRet = null;
         if (source != null)
         {
            if (source.Type == NetworkTypeEnum.BedSide && source.BedList != null && source.BedList.Count()==1)
            {
               intRet = source.BedList.FirstOrDefault().BedId;
            }
            else
            {
               intRet = source.BedID;
            }
         }
         return intRet;
      }


      private static ICollection<NetworkBedLink> BuildNetworkBedLinks(int idNetwork,IEnumerable<BedViewModel> objBeds)
      {
         ICollection<NetworkBedLink> objret = new List<NetworkBedLink>();
         if(objBeds!=null)
         {
            foreach(BedViewModel objBed in objBeds)
            {
               NetworkBedLink objBedLink = new NetworkBedLink();
               objBedLink.IdBed = objBed.BedId;
               objBedLink.IdNetwork = idNetwork;
               objBedLink.Index = Convert.ToInt16(objBed.BedIndex);
               objret.Add(objBedLink);
            }
         }
         return objret;
      }


      

      public static IEnumerable<Network> BuildList(IEnumerable<NetworkViewModel> source)
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
