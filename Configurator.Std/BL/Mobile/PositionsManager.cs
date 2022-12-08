using Configurator.Std.BL.Hubs;
using Configurator.Std.Exceptions;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.Mobile
{
   public class PositionsManager : IPositionsManager
   {
      private readonly IDigistatConfiguration mobjDigConfig;
      private readonly ILoggerService mobjLoggerService;
      public PositionsManager(IDigistatConfiguration digConfig, ILoggerService loggerService)
      {
         mobjDigConfig = digConfig;
         mobjLoggerService = loggerService;
      }
      public PositionAssociation CreatePosition(PositionAssociation objPositionAssociation)
      {
         PositionAssociation objRet = null;
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            try
            {
               int intCount = context.Set<PositionAssociation>()
                  .Where(p => string.Equals(p.PositionCode.Trim(), objPositionAssociation.PositionCode.Trim(), StringComparison.InvariantCultureIgnoreCase))
                  .Count();

               if (intCount == 0)
               {
                  context.BeginTransaction();

                  if (objPositionAssociation.PositionBedLinks != null && objPositionAssociation.PositionBedLinks.Any())
                  {
                     foreach (PositionBedLink objnet in objPositionAssociation.PositionBedLinks)
                     {
                        objnet.PositionCode = objPositionAssociation.PositionCode;
                        objnet.PositionAssociation = objPositionAssociation;
                     }
                  }
                  context.Set<PositionAssociation>().Add(objPositionAssociation);
                  context.SaveChanges();
                  context.CommitTransaction();
                  objRet = objPositionAssociation;
               }
               else
               {
                  throw new PositionException(string.Format("Postition with PositionCode {0} already exist", objPositionAssociation.PositionCode));
               }
            }
            catch (Exception e)
            {
               if (e is PositionException)
               {
                  throw;
               }
               context.RollbackTransaction();
               string errMsg = "Error on CreatePosition";
               mobjLoggerService.ErrorException(e, errMsg);
               throw new Exception(errMsg, e);
            }
         }
         return objRet;
      }
      public void DeletePosition(string positionCode)
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            try
            {
               mobjLoggerService.Info("Deleting Position with PositionCode {0}", positionCode);

               context.BeginTransaction();

               PositionAssociation objEntity = context.Set<PositionAssociation>()
                  .Include(x => x.PositionBedLinks)
                  .Where(x => string.Equals(x.PositionCode, positionCode, StringComparison.InvariantCultureIgnoreCase))
                  .SingleOrDefault();

               if (objEntity == null)
               {
                  throw new PositionException(string.Format("Unable to delete position with PositionCode {0}; position not found.", positionCode));
               }

               context.Set<PositionBedLink>()
                  .RemoveRange(objEntity.PositionBedLinks);

               context.Set<PositionAssociation>()
                  .Remove(objEntity);

               context.SaveChanges();
               context.CommitTransaction();

               mobjLoggerService.Info("Position with PositionCode {0} removed succesfully", positionCode);

            }
            catch (Exception e)
            {
               context.RollbackTransaction();
               string message = string.Format("Error removing Position with PositionCode {0}", positionCode);
               mobjLoggerService.ErrorException(e, message);
               throw new PositionException(message);
            }
         }
      }
      public PositionAssociation GetPositionByPositionCode(string positionCode)
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            try
            {
               mobjLoggerService.Info("Executing Get for Position with PositionCode {0}", positionCode);

               return context.Set<PositionAssociation>()
                  .Include(x => x.PositionBedLinks)
                  .ThenInclude(c => c.Bed)
                  .ThenInclude(d => d.Location)
                  .Where(x => string.Equals(x.PositionCode, positionCode, StringComparison.InvariantCultureIgnoreCase))
                  .SingleOrDefault();
            }
            catch (Exception e)
            {
               string message = string.Format("Error on retrieve Position with PositionCode {0}", positionCode);
               mobjLoggerService.ErrorException(e, message);
               throw new PositionException(message);
            }
         }
      }
      public IList<PositionAssociation> GetPositions()
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            try
            {
               mobjLoggerService.Info("Executing Get for Positions");

               return context.Set<PositionAssociation>()
                  .Include(x => x.PositionBedLinks)
                  .ThenInclude(c => c.Bed)
                  .ToList();
            }
            catch (Exception e)
            {
               string message = string.Format("Error on retrieve Positions");
               mobjLoggerService.ErrorException(e, message);
               throw new PositionException(message);
            }
         }
      }
      public PositionAssociation UpdatePosition(PositionAssociation objPositionAssociation)
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            try
            {
               context.BeginTransaction();

               var objOldPosition = context.Set<PositionAssociation>()
                  .Include(x => x.PositionBedLinks)
                  .Where(x => string.Equals(x.PositionCode, objPositionAssociation.PositionCode, StringComparison.InvariantCultureIgnoreCase))
                  .SingleOrDefault();

               if (objOldPosition == null)
               {
                  throw new PositionException(string.Format("Unable to Update position with PositionCode {0}; position not found.", objPositionAssociation.PositionCode));
               }

               context.Set<PositionBedLink>()
                  .RemoveRange(objOldPosition.PositionBedLinks.ToList());

               objOldPosition.Description = objPositionAssociation.Description;

               if (objPositionAssociation.PositionBedLinks != null && objPositionAssociation.PositionBedLinks.Any())
               {
                  foreach (PositionBedLink objnet in objPositionAssociation.PositionBedLinks)
                  {
                     objnet.PositionCode = objOldPosition.PositionCode;
                     objnet.PositionAssociation = objOldPosition;
                  }
               }

               context.Set<PositionBedLink>()
                  .AddRange(objPositionAssociation.PositionBedLinks);

               objOldPosition.PositionBedLinks = objPositionAssociation.PositionBedLinks;

               context.Set<PositionAssociation>()
                  .Update(objOldPosition);

               context.SaveChanges();
               context.CommitTransaction();

               return objPositionAssociation;
            }
            catch (Exception e)
            {
               context.RollbackTransaction();
               string errMsg = "Error on UpdatePosition";
               mobjLoggerService.ErrorException(e, errMsg);
               throw new PositionException(errMsg);
            }
         }
      }
      public IList<PositionBedLink> GetAssociatedBedWherePositionCodeIsNot(string positionCode)
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            try
            {
               return context.Set<PositionBedLink>()
                  .Where(x => !string.Equals(x.PositionCode, positionCode, StringComparison.InvariantCultureIgnoreCase))
                  .ToList();
            }
            catch (Exception e)
            {
               string errMsg = "Error on GetAllAssociatedBed";
               mobjLoggerService.ErrorException(e, errMsg);
               throw new PositionException(errMsg);
            }
         }
      }
   }
}
