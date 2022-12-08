using Digistat.FrameworkStd.Model.Therapy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Digistat.FrameworkStd.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ConfiguratorWeb.App.Models.Therapy
{

    //[ModelBinder(BinderType = typeof(Extensions.DecimalModelBinder))]
    public class TherapyItemModel
   {
      public TherapyItemModel()
      {
         QuantityDose = new QuantityDose(); 
      }
      
      public int Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string ClassName { get; set; }
      public TherapyItemType ItemType { get; set; }
      public string ItemTypeString { get { return ItemType.ToString(); } set { } }
      public Classification Classification { get; set; } = new Classification();
      public QuantityDose QuantityDose { get; set; }

      public Schedule Schedule { get; set; } = new Schedule();

      public Link Link { get; set; } = new Link();
      public Properties Properties { get; set; } = new Properties();

      //public List<TherapyItemModel> MixtureComponents { get; set; } = new List<TherapyItemModel>();
      //public List<StringsPair> MixtureComponentFormVal { get; set; } = new List<StringsPair>();

      //public List<TherapyItemModel> Components { get; set; } = new List<TherapyItemModel>();
      public List<AssociationStandardAction> ComponentsFormVal { get; set; } = new List<AssociationStandardAction>();

      //public bool ForProtocolOnly { get; set; }

      //LINK
   }

   public class AssociationStandardAction
   {
      public int Index { get; set; }

      public int IdFather { get; set; }

      public int IdChild { get; set; }

      public double? Concentration { get; set; }

      public double? Amount { get; set; }
      public double? Volume { get; set; }
      public string Name { get; set; }

      public string Description { get; set; }

      public int Type { get; set; }

      public int? LocationRef { get; set; }
      public string LocationName { get; set; }

   }
}
