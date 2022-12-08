using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class LocationDto
    {
        public string LocationShortName { get; set; }
        public string LocationName { get; set; }
        public string LocationDescription { get; set; }
        public int LocationPositionNumber { get; set; }
        public int LocationIndex { get; set; }
        public string LocationId { get; set; }
        public string CabinetId { get; set; }

    }
}
