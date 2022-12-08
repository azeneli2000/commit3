using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Digistat.FrameworkStd.Model;

namespace Configurator.Std.BL
{
   public interface IPatientManager : Digistat.Dal.Interfaces.IDalManagerBase<Patient>
   {
      Patient GetByPatientCode(string patientCode);
   }
}