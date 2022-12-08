using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class OperatingBlockDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public int IDLocation { get; set; }        
        public string LocationName { get; set; }
        public int Index { get; set; }
        public string ParentID { get; set; }
        public string ParentBlock { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string ReasonForClosing { get; set; }
        public string SpecialRequests { get; set; }
    }
}
