using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Configurator.Std.BL;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkWebExtensions.Attributes;
using Digistat.FrameworkWebExtensions.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace ConfiguratorWeb.App.Controllers
{
   [ApiController]
   [Route("api/[controller]/[action]")]


   public class ClinicalLogApiController : Controller 
   {
      private ILoggerService mobjLogSvc;
      private IPatientManager mobjPatientSvc;
      public ClinicalLogApiController(ILoggerService logSvc, IPatientManager patientSvc)
      {
         mobjLogSvc = logSvc;
         mobjPatientSvc = patientSvc;
      }

      /// <summary>
      /// Write Clinical log
      /// </summary>
      /// <param name="message"></param>
      /// <param name="logType">Warning = 0,Error = 1,Information = 2,SuccessAudit = 3</param>
      /// <param name="user"></param>
      /// <param name="patientCode"></param>
      /// <param name="applicationName"></param>
      /// <param name="deviceName"></param>
      /// <returns></returns>
      private ActionResult Write(/*[FromForm, Required]*/ string message,
         //[FromForm,Required][SwaggerParameter("Warning = 0,Error = 1,Information = 2,SuccessAudit = 3")]
         EventLogEntryType logType, /*[FromForm]*/string user, /*[FromForm]*/string patientCode
         ,  /*[FromForm]*/string applicationName, /*[FromForm]*/string deviceName )
      {
         int code = 101;
         string area = "UNI";
         EventLogEntryType logEntryType = EventLogEntryType.Information;
         
         if (string.IsNullOrWhiteSpace(message))
            return StatusCode(400, Json(new {errorMessage = "message field is empty", success = false}));

         if (string.IsNullOrWhiteSpace(deviceName))
            deviceName = "webApi";

         try
         {
            int logPatientCode = 0;
            //int.TryParse(patientCode, out logPatientCode );
            if (patientCode != null)
            {
               {
                  Patient p = mobjPatientSvc.GetByPatientCode(patientCode);
                  if (p != null && p.PatientRef.HasValue)
                  {
                     logPatientCode = p.PatientRef.Value;
                  }
                  else
                  {
                     logPatientCode = 0;
                  }
               }
            }

            if(mobjLogSvc.WriteClinicalLog(code, message, logType, user, logPatientCode,  applicationName, deviceName,area))
               return StatusCode(201, Json(new {errorMessage = "", success = true}));
            else
               return StatusCode(StatusCodes.Status500InternalServerError, Json(new {errorMessage = "message not write correctly", success = false}));
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status400BadRequest, Json(new {errorMessage = "", success = true}));
         }
         
         
      }
      
      
      /// <summary>
      /// Write message in  Clinical log
      /// </summary>
      /// <param name="body"></param>
      /// <returns></returns>
      /// <remarks>
      /// The functionality is protected by authentication.
      /// </remarks>
      [HttpPost]
      [BasicAuthAttribute("")]
      [ValidateModelState]
      [SwaggerResponse(StatusCodes.Status201Created, "Message inserted in clinical log")]
      [SwaggerResponse(StatusCodes.Status400BadRequest,  "Bad Request")]
      [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returned status code when user could not be authenticated (i.e. bad username or password)")]
      [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned status code when a generic error happen writing the message")]
      public IActionResult Write([FromBody]ClinicalLogMessage body)
      {
         return Write(body.message, body.logType, body.user, body.patientCode, body.applicationName, body.deviceName);
      }
      
   }


   [DataContract]
   public partial class ClinicalLogMessage 
   {
      /// <summary>
      /// Information to write
      /// </summary>
      [DataMember(Name="message")]
      [Required]
      [MaxLength(4000)]
      public string message { get; set; }

      /// <summary>
      /// "Type of log: Warning ,Error ,Information ,SuccessAudit"
      /// </summary>
      [DataMember(Name="logType")]
      [Required]
      //[EnumDataType(typeof(EventLogEntryType))]  
      public EventLogEntryType logType { get; set; }

      [DataMember(Name="user")]
      [MaxLength(5)]
      public string user { get; set; }
      /// <summary>
      /// Patient code
      /// </summary>
      [DataMember(Name="patientCode")]
      [MaxLength(50)]
      public string patientCode { get; set; }

      [DataMember(Name="applicationName")]
      [MaxLength(20)]
      public string applicationName { get; set; }

      [DataMember(Name="deviceName")]
      [MaxLength(50)]
      public string deviceName { get; set; }

      
      public string ToJson()
      {
         return JsonConvert.SerializeObject(this, Formatting.Indented);
      }
      
   }

   public class ClinicalLogApiModelExample : IExamplesProvider<ClinicalLogMessage> 
   {
      public ClinicalLogMessage GetExamples()
      {
         return new ClinicalLogMessage
         {
            message = "something happened on this device",
            logType = EventLogEntryType.Information,
            user = "user1",
            patientCode = "abc3423",
            applicationName = "smartcontroll",
            deviceName = "web api"

            //Lang = "en-GB",
            //Currency = "GBP",
            //Address = new AddressModel
            //{
            //   Address1 = "1 Gwalior Road",
            //   Locality = "London",
            //   Country = "GB",
            //   PostalCode = "SW15 1NP"
            //},
            //Items = new[]
            //{
            //   new ItemModel
            //   {
            //      ItemId = "ABCD",
            //      ItemType = ItemType.Product,
            //      Price = 20,
            //      Quantity = 1,
            //      RestrictedCountries = new[] { "US" }
            //   }
            //}
         };
      }

      public void Apply(OpenApiOperation operation, OperationFilterContext context)
      {
         if (context.MethodInfo == null) return;

         var controllerAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true);
         var actionAttributes = context.MethodInfo.GetCustomAttributes(true);
      }
   }
}

