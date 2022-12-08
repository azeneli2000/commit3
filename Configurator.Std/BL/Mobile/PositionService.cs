using System;
using System.Collections.Generic;
using System.Linq;
using Configurator.Std.BL.Hubs;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Mobile;

namespace Configurator.Std.BL.Mobile
{
   public class PositionService : IPositionService
   {
      private readonly IMessageCenterService mobjMsgCtrService;
      private readonly IPositionsManager mobjPositionsManager;
      private readonly IBedsManager mobjBedsManager;
      private readonly ILocationManager mobjLocationManager;

      public PositionService(IMessageCenterService msgCtrSrv,
         IPositionsManager positionManager,
         IBedsManager bedManager,
         ILocationManager locationManager)
      {
         mobjMsgCtrService = msgCtrSrv;
         mobjPositionsManager = positionManager;
         mobjBedsManager = bedManager;
         mobjLocationManager = locationManager; 
      }
      public PositionAssociation CreatePosition(PositionAssociation objPositionAssociation)
      {
         PositionAssociation objPosition = mobjPositionsManager.CreatePosition(objPositionAssociation);
         mobjMsgCtrService.SendMessage(MobileServiceHelper.NewPosition(objPosition.PositionCode));
         return objPosition;
      }

      public void DeletePosition(string positionCode)
      {
         mobjPositionsManager.DeletePosition(positionCode);
         mobjMsgCtrService.SendMessage(MobileServiceHelper.DeletePosition(positionCode));
      }

      public IQueryable<Bed> GetAviableBedPosition(string positionCode)
      {
         var linkedBedIds = mobjPositionsManager.GetAssociatedBedWherePositionCodeIsNot(positionCode).Select(p => p.BedId);

         var objDomainBed = mobjBedsManager.GetBedsWithFullData();

         return objDomainBed.Where(x => !linkedBedIds.Contains(x.Id));
      }

      public PositionAssociation GetPositionByPositionCode(string positionCode)
      {
         return mobjPositionsManager.GetPositionByPositionCode(positionCode);
      }

      public IEnumerable<PositionAssociation> GetPositions()
      {
         return mobjPositionsManager.GetPositions();
      }

      public PositionAssociation UpdatePosition(PositionAssociation objPositionAssociation)
      {
         PositionAssociation objPosition = mobjPositionsManager.UpdatePosition(objPositionAssociation);
         mobjMsgCtrService.SendMessage(MobileServiceHelper.UpdatePosition(objPosition.PositionCode));
         return objPosition;
      }
   }
}
