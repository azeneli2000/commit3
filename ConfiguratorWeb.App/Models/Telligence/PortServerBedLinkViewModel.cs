using System.ComponentModel.DataAnnotations;

namespace ConfiguratorWeb.App.Models
{
    public sealed class PortServerBedLinkViewModel
    {
        [Required]
        public short PortGroup { get; set; }
        
        public BedViewModel Bed { get; set; }
    }
}