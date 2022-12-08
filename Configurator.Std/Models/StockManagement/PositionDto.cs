using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class PositionDto
    {
        public string PositionShortName { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }
        public int PositionIndex { get; set; }
        public string PositionId { get; set; }
        public string LocationId { get; set; }
    }
}
