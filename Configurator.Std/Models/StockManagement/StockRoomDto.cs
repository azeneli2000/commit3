using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public  class StockRoomDto
    {
        public string StockRoomName { get; set; }
        public string StockRoomShortName { get; set; }
        public string StockRoomDescription { get; set; }
        public int StockRoomIndex { get; set; }
        public bool StockRoomIsForUnknown { get; set; }
        public string StockRoomId { get; set; }
    }
}
