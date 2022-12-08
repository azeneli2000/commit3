using System.Collections.Generic;
using Digistat.FrameworkStd.UMSLegacy;

namespace Configurator.Std.BL
{
   public interface ISystemValidationManager
   {
      List<CDASConfigurationValidation> GetValidations();

      List<string> ValidateCurrentConfiguration(string userID, string reason);

      List<string> CanValidateConfiguration();
   }
}