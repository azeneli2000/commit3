using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class TreeListTemplate : ResourceCabinet
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string  Mode { get=> "";  }
        public bool HasChildren { get; set; }

        //public string StockRoomName { get; set; }
        //public string StockRoomDescription { get; set; }
        //public int StockRoomIndex { get; set; }
        //public bool StockRoomIsForUnknown { get; set; }

    }
}
