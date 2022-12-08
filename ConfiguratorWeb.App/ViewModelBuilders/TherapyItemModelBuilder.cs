using ConfiguratorWeb.App.Models.Therapy;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model.Therapy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
    public static class TherapyItemModelBuilder
   {
      public static TherapyItemModel Build(StandardAction source)
      {
         TherapyItemModel objDest = null;
         try
         {
            if (source != null)
            {
               string[] subs = { };
               string[] subs2 = { };
               string strLocationname = ""; 
               string strLocationname2 = ""; 
               double doubleToleranceDest =0;
               int? howmn = 0 ;
               double? evTval = 0;
               double? inTval = 0;
               if(source.HowManyTimes != null)
               {
                howmn = source.HowManyTimes;
               }
               else
               {
                howmn = null; 
               }
                if (source.EveryTimesValue != null)
                {
                    evTval = source.EveryTimesValue;
                }
                else
                {
                    evTval = null;
                }
                if (source.InTimesValue != null)
                {
                    inTval = source.InTimesValue;
                }
                else
                {
                    inTval = null;
                }
                if (source.Hours != null)
               {
                  subs = source.Hours.Split(',');
               }
               if (source.Days != null)
               {
                  subs2 = source.Days.Split(',');
               }
               if(source.Tolerance != null)
               {
                  doubleToleranceDest = source.Tolerance.Value.Minute;
                  var hcheck = source.Tolerance.Value.Hour;
                  if (doubleToleranceDest != 0 && hcheck != 0)
                  {
                     doubleToleranceDest = 90; 
                  }
                  
                  if (doubleToleranceDest == 0)
                  {
                     doubleToleranceDest = source.Tolerance.Value.Hour;
                     switch (doubleToleranceDest)
                     {
                        case 1:
                           doubleToleranceDest = 60;
                           break;
                        case 2:
                           doubleToleranceDest = 120;
                           break;
                        case 3:
                           doubleToleranceDest = 180;
                           break;
                        case 4:
                           doubleToleranceDest = 240;
                           break;
                        case 6:
                           doubleToleranceDest = 360;
                           break;
                        case 8:
                           doubleToleranceDest = 480;
                           break;
                        case 9:
                           doubleToleranceDest = 540;
                           break;
                        case 12:
                           doubleToleranceDest = 720;
                           break;

                     }
                    
                  }
               }
               if(source.Location != null)
               {
                  strLocationname = source.Location.LocationName;
                  strLocationname2 = source.LocationName;
                  
               }
               objDest = new TherapyItemModel()
               {
                  Id = source.IDStandardAction,
                  Name = source.Name,
                  Description = source.Description,
                  ItemType = source.ActionType.HasValue ? (TherapyItemType)source.ActionType.Value : TherapyItemType.Invalid,
                  //LocationName = source.Location != null ? source.Location.LocationName : String.Empty,
                  ClassName = source.ClassName,
                  //ForProtocolOnly = source.ForProtocolOnly,
                  Classification = new Classification()
                  {
                     Category = source.Category,
                     ClassCat = source.ClassCat.HasValue ? source.ClassCat.Value : null,
                     LocationRef = source.LocationRef.HasValue ? source.LocationRef.Value : null,
                     Incomplete = source.Incomplete,
                     Obsolete = source.Obsolete,
                     //CanBeMixtureComponent = source.MixtureComponent,
                     LocationName = source.LocationName,
                     SearchLevel = source.SearchLevel,
                     EnumSearchLevel = source.SearchLevel.HasValue ? (Classification.SearchLevelTherapy) source.SearchLevel.Value : Classification.SearchLevelTherapy.SearchLevelAll,
                     //Prescription = 
                     //QuickAction = 
                  },
                  QuantityDose = new QuantityDose()
                  {
                     AllowedMasks = source.AllowedMasks,
                     Mask = source.Mask.GetValueOrDefault(),
                     Amount = source.Amount.ToSafeString(),
                     AmountUnit = source.AmountUnit,
                     //AmountHardMax = source.AmountHardMax,
                     //AmountHardMin = source.AmountHardMin,
                     //AmountSoftMax = source.AmountSoftMax,
                     //AmountSoftMin = source.AmountSoftMin,
                     Concentration = source.Concentration.ToSafeString(),
                     ConcentrationUnit = source.ConcentrationUnit,
                     //ConcentrationHardMax = source.ConcentrationHardMax,
                     //ConcentrationHardMin = source.ConcentrationHardMin,
                     //ConcentrationSoftMax = source.ConcentrationSoftMax,
                     //ConcentrationSoftMin = source.ConcentrationSoftMin,
                     DrugSpeed = source.DrugSpeed.ToSafeString(),
                     DrugSpeedUnit = source.DrugSpeedUnit,
                     //DrugSpeedHardMax = source.DrugSpeedHardMax,
                     //DrugSpeedHardMin = source.DrugSpeedHardMin,
                     //DrugSpeedSoftMax = source.DrugSpeedSoftMax,
                     //DrugSpeedSoftMin = source.DrugSpeedSoftMin,
                     Duration = source.Duration,
                     DurationUnit = source.DurationUnit,
                     //DurationHardMax = source.DurationHardMax,
                     //DurationHardMin = source.DurationHardMin,
                     //DurationSoftMax = source.DurationSoftMax,
                     //DurationSoftMin = source.DurationSoftMin,
                     Speed = source.Speed.ToSafeString(),
                     SpeedUnit = source.SpeedUnit,
                     //SpeedHardMax = source.SpeedHardMax,
                     //SpeedHardMin = source.SpeedHardMin,
                     //SpeedSoftMax = source.SpeedSoftMax,
                     //SpeedSoftMin = source.SpeedSoftMin,
                     Volume = source.Volume.ToSafeString(),
                     VolumeUnit = source.VolumeUnit,
                     //VolumeHardMax = source.VolumeHardMax,
                     //VolumeHardMin = source.VolumeHardMin,
                     //VolumeSoftMax = source.VolumeSoftMax,
                     //VolumeSoftMin = source.VolumeSoftMin,
                     Factor = source.Factor.ToSafeString(),
                     ProductConcentration = source.ProductConcentration.ToSafeString(),
                     SchemaName = source.SchemaName,
                     AllowedDiluents = source.AllowedDiluents,
                     Diluent = source.Diluent,
                     AllowedWays = source.AllowedWays,
                     Way = source.Way,

                     Diluents = BoolStringPair.Build(source.AllowedDiluents, source.Diluent),
                     Ways = BoolStringPair.Build(source.AllowedWays, source.Way)
                  }
                  ,

                  Schedule = new Schedule()
                  {
                     Condition = source.Condition,
                     CustomOrders = source.CustomOrders,
                     Days = source.Days,
                     EveryTimesUnit = source.EveryTimesUnit.GetValueOrDefault(),
                     EveryTimesValue = evTval.ToSafeString(),
                     Hours = source.Hours,
                     aHours = subs,
                     aDays = subs2,
                     HowManyTimes = howmn,
                     InTimesUnit = source.InTimesUnit.GetValueOrDefault(),
                     InTimesValue = inTval.ToSafeString(),
                     PlanningType = source.PlanningType.GetValueOrDefault(),
                     Repeat = source.Repeat,
                     SchedPolicy = source.SchedPolicy.GetValueOrDefault(),
                     StartingFrom = source.StartingFrom,
                     TimeChangeType = source.TimeChangeType.GetValueOrDefault(),
                     Tolerance = source.Tolerance.GetValueOrDefault(),
                     strTolerance = doubleToleranceDest.ToString(CultureInfo.InvariantCulture),
                  },

                  Link = new Link()
                  {
                     ExternalKey = source.ExternalKey,
                     //Mixtures
                     //Profiles
                     //Protocols
                     //Resources
                  },

                  Properties = new Models.Therapy.Properties()
                  {
                     Barcodes = source.Barcodes,
                     FluidBalanceType = source.FluidBalanceType.GetValueOrDefault(),
                     HelpKeyExec = source.HelpKeyExec,
                     HelpKeyPresc = source.HelpKeyPresc,
                     PreparationType = source.PreparationType.GetValueOrDefault(),
                     PrescNotes = source.PrescNotes,
                     SecondSignatureExecType = source.SecondSignatureExecType.GetValueOrDefault(),
                     SecondSignatureType = source.SecondSignatureType.GetValueOrDefault(),
                     ValidationType = source.ValidationType.GetValueOrDefault()
                     
                  }

               };

            }
         }
         catch (Exception e)
         {
         }

         return objDest;
      }

      public static IEnumerable<TherapyItemModel> BuildList(IEnumerable<StandardAction> source)
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

     
      public static StandardAction ToStandardAction(TherapyItemModel source)
      {
         StandardAction ret = null;

         try
         {
            if (source != null)
            {

               string strDefaultDateTolerance = "2018-01-01 00:00:00.000";
               DateTime? objToleranceDate = DateTime.Parse(strDefaultDateTolerance);
               DateTime? dateOut = DateTime.Parse(strDefaultDateTolerance);



               if ( source.Schedule.strTolerance != null)
               {
                  double tolerance = Convert.ToDouble(source.Schedule.strTolerance,CultureInfo.InvariantCulture);
                  dateOut= objToleranceDate.Value.AddMinutes(tolerance);
               }

               ret = new StandardAction();

               ret.IDStandardAction = source.Id;
               ret.Name = source.Name;
               ret.Description = source.Description;
               ret.ActionType = (int)source.ItemType;
               ret.Category = source.Classification?.Category ?? null;
               ret.ClassCat = source.Classification?.ClassCat ?? null;
               ret.LocationRef = source.Classification?.LocationRef ?? null;
               ret.Incomplete = source.Classification?.Incomplete ?? false;
               ret.Obsolete = source.Classification?.Obsolete ?? false;

               ret.AllowedMasks = source.QuantityDose?.AllowedMasks ?? string.Empty;
               ret.Amount = source.QuantityDose?.Amount.ToNullDouble();
               ret.AmountUnit = source.QuantityDose?.AmountUnit ?? String.Empty;
               //ret.AmountSoftMin = source.QuantityDose?.AmountSoftMin ?? null;
               //ret.AmountHardMin = source.QuantityDose?.AmountHardMin ?? null;
               //ret.AmountHardMax = source.QuantityDose?.AmountHardMax ?? null;
               //ret.AmountSoftMax = source.QuantityDose?.AmountSoftMax ?? null;
               ret.Concentration = source.QuantityDose?.Concentration.ToNullDouble();
               ret.ConcentrationUnit = source.QuantityDose?.ConcentrationUnit ?? null;
               //ret.ConcentrationHardMax = source.QuantityDose?.ConcentrationHardMax ?? null;
               //ret.ConcentrationHardMin = source.QuantityDose?.ConcentrationHardMin ?? null;
               //ret.ConcentrationSoftMax = source.QuantityDose?.ConcentrationSoftMax ?? null;
               //ret.ConcentrationSoftMin = source.QuantityDose?.ConcentrationSoftMin ?? null;
               ret.DrugSpeed = source.QuantityDose?.DrugSpeed.ToNullDouble();
               ret.DrugSpeedUnit = source.QuantityDose?.DrugSpeedUnit ?? null;
               //ret.DrugSpeedHardMax = source.QuantityDose?.DrugSpeedHardMax ?? null;
               //ret.DrugSpeedHardMin = source.QuantityDose?.DrugSpeedHardMin ?? null;
               //ret.DrugSpeedSoftMax = source.QuantityDose?.DrugSpeedSoftMax ?? null;
               //ret.DrugSpeedSoftMin = source.QuantityDose?.DrugSpeedSoftMin ?? null;
               ret.Duration = (source.QuantityDose != null && source.QuantityDose.Duration != null) ? (int)source.QuantityDose.Duration : -1;
               if (ret.Duration == -1)
                  ret.Duration = null;
               ret.DurationUnit = source.QuantityDose?.DurationUnit ?? null;
               //ret.DurationHardMax = source.QuantityDose?.DurationHardMax ?? null;
               //ret.DurationHardMin = source.QuantityDose?.DurationHardMin ?? null;
               //ret.DurationSoftMax = source.QuantityDose?.DurationSoftMax ?? null;
               //ret.DurationSoftMin = source.QuantityDose?.DurationSoftMin ?? null;
               ret.Speed = source.QuantityDose?.Speed.ToNullDouble();
               ret.SpeedUnit = source.QuantityDose?.SpeedUnit ?? null;
               //ret.SpeedHardMax = source.QuantityDose?.SpeedHardMax ?? null;
               //ret.SpeedHardMin = source.QuantityDose?.SpeedHardMin ?? null;
               //ret.SpeedSoftMax = source.QuantityDose?.SpeedSoftMax ?? null;
               //ret.SpeedSoftMin = source.QuantityDose?.SpeedSoftMin ?? null;
               ret.Volume = source.QuantityDose?.Volume.ToNullDouble();
                    ret.VolumeUnit = source.QuantityDose?.VolumeUnit ?? null;
               //ret.VolumeHardMax = source.QuantityDose?.VolumeHardMax ?? null;
               //ret.VolumeHardMin = source.QuantityDose?.VolumeHardMin ?? null;
               //ret.VolumeSoftMax = source.QuantityDose?.VolumeSoftMax ?? null;
               //ret.VolumeSoftMin = source.QuantityDose?.VolumeSoftMin ?? null;
               ret.Factor = source.QuantityDose?.Factor.ToNullDouble();
                    ret.Mask = source.QuantityDose?.Mask ?? null;
               ret.ProductConcentration = source.QuantityDose?.ProductConcentration.ToNullDouble();
                    ret.SchemaName = source.QuantityDose?.SchemaName ?? null;

               ret.Condition = source.Schedule?.Condition ?? string.Empty;
               ret.CustomOrders = source.Schedule?.CustomOrders ?? string.Empty;
               ret.Days = source.Schedule?.Days ?? string.Empty; ;
               ret.EveryTimesUnit = source.Schedule?.EveryTimesUnit ?? null;
               ret.EveryTimesValue = source.Schedule?.EveryTimesValue.ToNullDouble();
                    ret.Hours = source.Schedule?.Hours ?? string.Empty; ;
               ret.HowManyTimes = source.Schedule?.HowManyTimes ?? null;
               ret.InTimesUnit = source.Schedule?.InTimesUnit ?? null;
               ret.InTimesValue = source.Schedule?.InTimesValue.ToNullDouble();
                    ret.PlanningType = source.Schedule?.PlanningType ?? null;
               ret.Repeat = source.Schedule?.Repeat ?? false;
               ret.SchedPolicy = source.Schedule?.SchedPolicy ?? null;
               ret.StartingFrom = source.Schedule?.StartingFrom;
               ret.TimeChangeType = source.Schedule?.TimeChangeType ?? null;
               ret.Tolerance = dateOut.GetValueOrDefault();
               //if (ret.Tolerance == DateTime.MinValue || ret.Tolerance == DateTime.MaxValue)
               //{
               //   ret.Tolerance = null;
               //}
               ret.ExternalKey = source.Link?.ExternalKey ?? string.Empty;

               ret.Barcodes = source.Properties?.Barcodes ?? string.Empty;
               ret.FluidBalanceType = source.Properties?.FluidBalanceType ?? null;
               ret.HelpKeyExec = source.Properties?.HelpKeyExec ?? string.Empty;
               ret.HelpKeyPresc = source.Properties?.HelpKeyPresc ?? string.Empty;
               ret.PreparationType = source.Properties?.PreparationType ?? null;
               ret.PrescNotes = source.Properties?.PrescNotes ?? string.Empty;
               ret.SecondSignatureExecType = source.Properties?.SecondSignatureExecType ?? null;
               ret.SecondSignatureType = source.Properties?.SecondSignatureType ?? null;
               ret.ValidationType = source.Properties?.ValidationType ?? null;

               ret.AllowedWays = source.QuantityDose?.AllowedWays ?? string.Empty;
               ret.Way = source.QuantityDose?.Way ?? string.Empty;
               ret.AllowedDiluents = source.QuantityDose?.AllowedDiluents ?? string.Empty;
               ret.Diluent = source.QuantityDose?.Diluent ?? string.Empty;

               //ret.MixtureComponent = source.Classification?.CanBeMixtureComponent ?? false;
               //ret.ForProtocolOnly = source.ForProtocolOnly ;

               ret.SearchLevel = (short?)source.Classification.EnumSearchLevel;
            }
         }
         catch (Exception ex)
         {
         }

         return ret;
      }


      public static StandardAssociationItem ToComponentLink(AssociationStandardAction source)
      {
         StandardAssociationItem ret = null;

         try
         {
            if (source != null)
            {
               ret = new StandardAssociationItem();

               
               ret.FatherID = source.IdFather;
               
               ret.ChildID = source.IdChild;
               ret.Index = source.Index;
               ret.LocationRef = source.LocationRef;
               ret.Amount = source.Amount;
               ret.Concentration = source.Concentration;
               ret.LocationName = source.LocationName;
               
            }
               
         }
         catch (Exception)
         {
            ret = null;
         }

         return ret;
      }

      public static AssociationStandardAction BuildComponentLink(StandardAssociationItem source)
      {
         AssociationStandardAction ret = null;

         try
         {
            if (source != null)
            {
               ret = new AssociationStandardAction();


               ret.IdFather = source.FatherID;

               ret.IdChild = source.ChildID;
               ret.Index = source.Index;
               ret.Concentration = source.Concentration.GetValueOrDefault();
               ret.Amount = source.Amount.GetValueOrDefault();
               ret.Volume = source.Volume.GetValueOrDefault();
               ret.Name = source.Name;
               ret.Description = source.Description;
               ret.Type = source.Type;
               ret.LocationRef = source.LocationRef;
               ret.LocationName = source.LocationName;

            }

         }
         catch (Exception)
         {
            ret = null;
         }

         return ret;
      }

      public static IEnumerable<StandardAssociationItem> ToMixtureComponentList(IEnumerable<AssociationStandardAction> source)
      {
         try
         {
            return source.Select(ToComponentLink);
         }
         catch (Exception)
         {

            throw;
         }
      }
      public static IEnumerable<AssociationStandardAction> ToMixtureComponentListView(IEnumerable<StandardAssociationItem> source)
      {
         try
         {
            return source.Select(BuildComponentLink);
         }
         catch (Exception)
         {

            throw;
         }
      }

        public static string ToSafeString(this double? source)
        {
            return source.HasValue ? source.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }

        public static double? ToNullDouble(this string source)
        {
            double? ret = null;

            if ( !string.IsNullOrEmpty(source) && double.TryParse(source, NumberStyles.Number, CultureInfo.InvariantCulture, out double d))
            {
                ret = d;
            }

            return  ret;
        }
    }
}
