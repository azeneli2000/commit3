using Microsoft.EntityFrameworkCore.Storage;
using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Configurator.Std.Models.StockManagement
{
    public  class ResourceCabinet 
    {
        public string StockRoomName { get; set; }
        public string StockRoomShortName { get; set; }
        public string StockRoomDescription { get; set; }
        public int StockRoomIndex { get; set; }
        public bool StockRoomIsForUnknown { get; set; }      
        public string StockRoomId { get; set; }

        public string CabinetGroupName { get; set; }
        public string CabinetGroupShortName { get; set; }
        public string CabinetGroupId { get; set; }
        public string CabinetGroupDescription { get; set; }
        public int CabinetGroupIndex { get; set; }
        public string CabinetName { get; set; }
        public string CabinetShortName { get; set; }                 
        public string CabinetId { get; set; }
        public string CabinetDescription { get; set; }
        public int CabinetIndex { get; set; }
        public bool CabinetIsBasket { get; set; }
        public bool CabinetIsTrolley { get; set; }
        public bool CabinetIsGenericKit { get; set; }
        public bool CabinetIsForNewPosition { get; set; }


        public string LocationShortName { get; set; }
        public string LocationName { get; set; }
        public string LocationDescription { get; set; }
        public int? LocationPositionNumber { get; set; }
        public int LocationIndex { get; set; }
        public string LocationId { get; set; }

        public string PositionShortName { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }
        public int PositionIndex { get; set; }
        public string PositionId { get; set; }

        public string ResourceId { get; set; }
        public string ResourceCode { get; set; }
        public string ResourceShortName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public string MainStore { get; set; }
        public double? IdealQuantity { get; set; }
        public double? AlarmQuantity { get; set; }
        public double? MinQuantity { get; set; }
        public string SpGuid { get; set; }
        public string  Type { get => ""; set { } }
        public  int  RowNr { get; set; }


    }
}
