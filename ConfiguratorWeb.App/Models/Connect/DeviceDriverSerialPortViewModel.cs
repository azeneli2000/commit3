using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class DeviceDriverSerialPortViewModel
   {
      [UIHint("SerialPortListEditor")]
      [Display(Name = "Serial Port")]
      
      public int SerialPort { get; set; }

      [UIHint("BitsPerSecondsListEditor")]
      [Display(Name = "Bits Per Seconds")]
      public string BitsPerSeconds { get; set; }

      [UIHint("ParityListEditor")]
      [Display(Name = "Parity")]
      public System.IO.Ports.Parity Parity { get; set; }

      [UIHint("DataBitsListEditor")]
      [Display(Name = "Data Bits")]
      public string DataBits { get; set; }

      [UIHint("StopBitsListEditor")]
      [Display(Name = "Stop Bits")]
      public System.IO.Ports.StopBits StopBits { get; set; }

      [UIHint("HandshakeListEditor")]
      [Display(Name = "Handshake")]
      public System.IO.Ports.Handshake Handshake { get; set; }

      [UIHint("DataModeListEditor")]
      [Display(Name = "Data Mode")]
      public string DataMode { get; set; }

      [UIHint("DataModeListEditor")]
      [Display(Name = "Data Mode")]
      public int DataModeId { get; set; }

      public string SmartCableID { get; set; }
   }
}