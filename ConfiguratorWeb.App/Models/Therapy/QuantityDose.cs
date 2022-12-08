
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
   public class QuantityDose
   {
      public QuantityDose()
      {
         
      }
      public string SchemaName { get; set; }
      public string AllowedMasks { get; set; }
      public int Mask { get; set; }
      public string VolumeUnit { get; set; }
        public string Volume { get; set; }
      //public string VolumeSoftMin { get; set; }
      //public string VolumeHardMin { get; set; }
      //public string VolumeSoftMax { get; set; }
      //public string VolumeHardMax { get; set; }
      public string AmountUnit { get; set; }
        public string Amount { get; set; }
      //public string AmountSoftMin { get; set; }
      //public string AmountHardMin { get; set; }
      //public string AmountSoftMax { get; set; }
      //public string AmountHardMax { get; set; }
      public string SpeedUnit { get; set; }
        public string Speed { get; set; }
      //public string SpeedSoftMin { get; set; }
      //public string SpeedHardMin { get; set; }
      //public string SpeedSoftMax { get; set; }
      //public string SpeedHardMax { get; set; }

      public string DrugSpeedUnit { get; set; }
        public string DrugSpeed { get; set; }
      //public string DrugSpeedSoftMin { get; set; }
      //public string DrugSpeedHardMin { get; set; }
      //public string DrugSpeedSoftMax { get; set; }
      //public string DrugSpeedHardMax { get; set; }

      public string ConcentrationUnit { get; set; }
        public string Concentration { get; set; }
        //public string ConcentrationSoftMin { get; set; }
        //public string ConcentrationHardMin { get; set; }
        //public string ConcentrationSoftMax { get; set; }
        //public string ConcentrationHardMax { get; set; }
        public string ProductConcentration { get; set; }
      public string ProductConcentrationUnit { get; set; }

      public string DurationUnit { get; set; }
        public int? Duration { get; set; }
        //public string DurationSoftMin { get; set; }
        //public string DurationHardMin { get; set; }
        //public string DurationSoftMax { get; set; }
        //public string DurationHardMax { get; set; }
        public string Factor { get; set; }
      public string AllowedWays { get; set; }
      public string Way { get; set; }
      public string AllowedDiluents { get; set; }
      public string Diluent { get; set; }
      public List<BoolStringPair> Ways { get; set; } = new List<BoolStringPair>();
      public List<BoolStringPair> Diluents { get; set; } = new List<BoolStringPair>();

      public void RebuildAllowedProperties()
      {
         string allowed;
         string selected;
         BoolStringPair.GetAllowed(Diluents, out allowed, out selected);
         AllowedDiluents = allowed;
         Diluent = selected;

         BoolStringPair.GetAllowed(Ways, out allowed, out selected);
         AllowedWays = allowed;
         Way = selected;

         //BoolStringPair.GetAllowed(   , out allowed, out selected);
         //AllowedMasks = allowed;
         //Mask = int.Parse(selected);
      }
   }

   
}
