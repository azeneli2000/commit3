using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Digistat.FrameworkStd.Interfaces;

using Digistat.FrameworkStd.Model;
using Configurator.Std.BL.Hubs;
using Digistat.Dal.Data;



namespace Configurator.Std.BL
{
   public class PatientManager : DalManagerBase<Patient>, IPatientManager
   {

      #region Costructors


      public PatientManager(DigistatDBContext context,  ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLoggerService = loggerService;
      }
     
      

      #endregion

      #region Data reading functions
      /// <summary>
      /// Get Patient by patientCode (detached)
      /// </summary>
      /// <param name="patientCode"></param>
      /// <returns></returns>
      public Patient GetByPatientCode(string patientCode)
      {
         //TODO Trace
         mobjLoggerService.Info("Executing Get for Patient with code {0}", patientCode);

         Patient result = null;

         try
         {

            IQueryable<Patient> repository = mobjDbContext.Set<Patient>();

            //TODO Trace
            mobjLoggerService.Info("Reading Patient with code {0} from DB", patientCode);
            result = repository.AsNoTracking().OrderByDescending(f=>f.Id).FirstOrDefault(x => x.PatientCode == patientCode);

            //TODO Trace
            mobjLoggerService.Info("Patient with code {0} retrived from DB", patientCode);

         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error reading Patient with code {0} from DB", patientCode);
            throw new Exception(string.Format("Error reading Patient role with code {0} from DB", patientCode), e);
         }
         
         return result;
      }
      #endregion

      
      
   }
}
