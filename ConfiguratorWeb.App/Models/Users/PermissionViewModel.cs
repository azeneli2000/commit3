using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class PermissionViewModel
    {
        public int? Id { get; set; }

        //[StringLength(100)]
        [DisplayName("Name")]
        public string FunctionName { get; set; }

      //  [StringLength(10)]
        [DisplayName("Permission Modifier")]
        public string PermissionCode { get; set; }

        [DisplayName("Permission Level")]
        [Required]
        public int PriorityLevel { get; set; }

      [DisplayName("Module")]
      public string ModuleName { get; set; }

      [DisplayName("Description")]
      public string Description { get; set; }
   }
}