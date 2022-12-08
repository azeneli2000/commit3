using ConfiguratorWeb.App.Enums;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class NetworkViewModelBuilder
   {
      public static NetworkViewModel Build(Network source)
      {
         NetworkViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new NetworkViewModel
               {
                  Id = source.Id,
                  AllowEdit = source.AllowEdit,
                  BedID = source.BedRef,
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
                  LocationID = source.LocationRef,
                  LockBed = source.LockBed,
                  MacAddress = source.MacAddress,
                  Modules = source.Modules,
                  Type = source.Type != null ? (NetworkTypeEnum)source.Type : NetworkTypeEnum.CentralStation,
                  TypeDescription = source.Type!=null?((NetworkTypeEnum)source.Type).GetDisplayAttribute(): string.Empty,
                  TypeShort = source.Type??(short)NetworkTypeEnum.CentralStation,
                  UserId = source.UserId,
                  UserVersion = source.UserVersion,
                  WebMenu = source.WebMenu,
                  WebModules = source.WebModules,
                  WorkstationLabel = source.WorkstationLabel,
                  BedList = source.NetworkBedLinks != null ? BuildNetworkBedLinks(source.NetworkBedLinks).OrderBy(a=>a.IdLocation) : null,
                  DefaultLocation = source.DefaultLocation!=null ? LocationViewModelBuilder.Build(source.DefaultLocation):new LocationViewModel(),
               };
            }
         }
         catch (Exception)
         {

            throw;
         }
         return objDest;
      }


     

      private static IEnumerable<BedViewModel>  BuildNetworkBedLinks( ICollection<NetworkBedLink> objBedLinks)
      {
         List<BedViewModel> objret = new List<BedViewModel>();
         if (objBedLinks != null)
         {
            foreach (NetworkBedLink objBed in objBedLinks)
            {
               if(objBed!=null && objBed.Bed!=null)
               {
                  objret.Add(BedViewModelBuilder.Build(objBed.Bed));
               }
               
            }
         }
         return (IEnumerable<BedViewModel>) objret;
      }


      

      public static IEnumerable<NetworkViewModel> BuildList(IEnumerable<Network> source)
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
