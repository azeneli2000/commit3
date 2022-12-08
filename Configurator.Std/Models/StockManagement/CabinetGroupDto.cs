using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class CabinetGroupDto
    {
        public string CabinetGroupName { get; set; }
        public string CabinetGroupShortName { get; set; }
        public string CabinetGroupDescription { get; set; }
        public int CabinetGroupIndex { get; set; }
        public string CabinetGroupId { get; set; }
        public string StockRoomId { get; set; }

    }
}
