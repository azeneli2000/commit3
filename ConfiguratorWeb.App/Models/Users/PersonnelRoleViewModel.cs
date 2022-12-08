using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class PersonnelRoleViewModel
    {
        [Key]
        [StringLength(36)]
        public string GUID { get; set; }

        [Key]
        public int Version { get; set; }

        public bool Current { get; set; }

        [StringLength(15)]
        [DisplayName("Valid to Date")]
        public string ValidToDate { get; set; }

        [StringLength(50)]
        [DisplayName("External Key")]
        public string ExternalKey { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Role")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public string Notes { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [DisplayName("Mandatory")]
        public bool InPlanningByDefault { get; set; }

        [DisplayName("First Surgeon")]
        public bool IsFirstSurgeon { get; set; }

        public int? Index { get; set; }

    }
}