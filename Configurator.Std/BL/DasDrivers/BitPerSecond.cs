using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.BL.DasDrivers
{


   public enum BitPerSecond
   {
      [Display(Name = "300")]
      Baud300 = 300,
      [Display(Name = "600")]
      Baud600 = 600,
      [Display(Name = "1200")]
      Baud1200 = 1200,
      [Display(Name = "2400")]
      Baud2400 = 2400,
      [Display(Name = "4800")]
      Baud4800 = 4800,
      [Display(Name = "9600")]
      Baud9600 = 9600,
      [Display(Name = "10020")]  //PrismaFlex
      Baud10020 = 10020,
      [Display(Name = "14400")]
      Baud14400 = 14400,
      [Display(Name = "19200")]
      Baud19200 = 19200,
      [Display(Name = "38400")]
      Baud38400 = 38400,
      [Display(Name = "57600")]
      Baud57600 = 57600,
      [Display(Name = "115200")]
      Baud115200 = 115200,
   }
}
