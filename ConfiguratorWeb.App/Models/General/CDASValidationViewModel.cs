using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class CDASValidationViewModel
   {
      //
      // Summary:
      //     The identifier of the validation
      public string ID { get; set; }
      //
      // Summary:
      //     The user of the validation
      public string UserID { get; set; }
      //
      // Summary:
      //     The time stamp of the validation
      public DateTime TimeStamp { get; set; }
      //
      // Summary:
      //     the content of the validation
      public string Content { get; set; }
      //
      // Summary:
      //     The signature of the validation
      public string Signature { get; set; }
      //
      // Summary:
      //     The reason of the validation
      public string Reason { get; set; }
   }
}

