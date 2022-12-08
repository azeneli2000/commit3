using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class PersonnelViewModel
    {
        [StringLength(36)]
        public string ID { get; set; }

        public int Version { get; set; }

        [DisplayName("Valid to Date")]
        public DateTime? ValidToDate { get; set; }

        [DisplayName("Current")]
        public bool? Current { get; set; }

        [StringLength(50)]
        [DisplayName("External Key")]
        public string ExternalKey { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string Initial { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Notes { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Qualification { get; set; }

        [StringLength(30)]
        [DisplayName("Telephone Number")]
        public string TelephoneNumber { get; set; }

        [StringLength(50)]
        [DisplayName("Email Address")]
        public string EMailAddress { get; set; }

        [StringLength(30)]
        [DisplayName("Cell Phone")]
        public string CellPhone { get; set; }

        [StringLength(30)]
        public string Beeper { get; set; }

        [DisplayName("Enabled")]
        public bool? IsEnabled { get; set; }

        public string GUIDs { get; set; }

    }
}