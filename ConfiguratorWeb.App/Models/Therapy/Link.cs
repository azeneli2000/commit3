using Digistat.FrameworkStd.Model.Therapy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
   public class Link
   {
      public string ExternalKey { get; set; }

      public List<AssociationStandardAction> Mixtures = new List<AssociationStandardAction>();

      public List<AssociationStandardAction> Protocols = new List<AssociationStandardAction>();

      public List<Profiles> Profiles { get; set; } = new List<Profiles>(); 

      public List<ResourceActionLink> Resources { get; set; } = new List<ResourceActionLink>(); 


   }
}
