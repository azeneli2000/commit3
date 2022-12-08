using ConfiguratorWeb.App.Models.SystemMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Models.MonitoringApi;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using Digistat.FrameworkStd.UMSLegacy;
using System.Security.Cryptography;
using System.Text;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public class MonitoringDataForUniteViewModelBuilder
   {

      private static  ILoggerService mobjLogSvc;
      
      public static MonitoringDataForUniteViewModel Build(IEnumerable<MonitoringData> data,ILoggerService logSvc)
      {
         mobjLogSvc = logSvc;
         MonitoringDataForUniteViewModel objDest = null;
         try
         {
            if (data != null)
            {
               //DateTime receivingTimeUtc;
               //DateTime.TryParse(data.ReceivingTimeUTC, out receivingTimeUtc);
               objDest = new MonitoringDataForUniteViewModel
               {
                  SupervisorName = "Supervisor for Digistat",
                  //Applications = builApplications(data) //old skema
                  Applications = builApplicationsDigistat(data)
               };
            }
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e,$"Error in MonitoringDataForUniteViewModel:{e.Message}");
         }

         return objDest;
      }

      private static List<Application> builApplicationsDigistat(IEnumerable<MonitoringData> data)
      {
         var ret = new List<Application>();
         try
         {
            var digistat = new Application()
            {
               Path = $"/DigistatSuite",
               Name = "DigistatSuite",
               Status = buildStatusDigistat(data),
               Supervisors = buildSupervisorDigistat(data),
               Children = null
            };
            ret.Add(digistat);
         }
         catch (Exception e)
         {
            mobjLogSvc.ErrorException(e,$"Error in builApplicationsDigistat:{e.Message}");
         }

         return ret;
      }

      private static List<Supervisor> buildSupervisorDigistat(IEnumerable<MonitoringData> datas)
      {
         var lRet = new List<Supervisor>();
         
         try
         {
            foreach (MonitoringData service in datas)
            {
               lRet.Add(new Supervisor
               {
                  Status = buildStatus(service),
                  Descriptor = buildSupervisorDescripor(service.Module),
                  IsFailed = false,
                  IsStopped = false
               });
            }
         }
         catch (Exception e)
         {
            
         }
         return lRet;
      }

      private static Status buildStatusDigistat(IEnumerable<MonitoringData> datas)
      {
         DateTime receivingTimeUtc;
         int statusCode = 3; //buildStatusCode(data.Anomalies);
         string message = ""; //buildMessage(data.Anomalies);
         var data = datas.OrderByDescending(o => o.ReceivingTimeUTC).FirstOrDefault();
         DateTime.TryParse(data.ReceivingTimeUTC, out receivingTimeUtc);

         var lastAnomalieError = datas.Where(w => w.Anomalies.Count(a => a.Severity == Anomaly.SeverityValue.Error) > 0)
            ?.OrderByDescending(a => a.ReceivingTimeUTC);
         var lastAnomalieWarn = datas.Where(w => w.Anomalies.Count(a => a.Severity == Anomaly.SeverityValue.Warning) > 0)
            ?.OrderByDescending(a => a.ReceivingTimeUTC);

         if (lastAnomalieError.Count() >0)
         {
            statusCode = 0;
         }
         else
         {
            if (lastAnomalieWarn.Count() >0)
            {
               statusCode = 1;
            }
         }
         try
         {
            foreach (MonitoringData service in datas)
            {
               message += $"Digistat-{service.Name}: ";
               if (service.Anomalies.Count(a => a.Severity != Anomaly.SeverityValue.Information) > 0)
               {
                  
                  message += (service.Anomalies.Count(a => a.Severity == Anomaly.SeverityValue.Error) > 0)?"Error":"Warning";
               }
               else
               {
                  message += "OK";
               }
               
               message += "<br>";
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            throw;
         }


         Status status = new Status
         {
            StatusDateTime = receivingTimeUtc,
            StatusCode = statusCode,
            Message = message,
            Comment = null,
            Information = null
         };
         return status;
      }

      private static List<Application> builApplications(IEnumerable<MonitoringData> data)
      {
         var ret = new List<Application>();
         try
         {
            foreach (MonitoringData mData in data)
            {
               ret.Add(builApplication(mData));
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            throw ;
         }

         return ret;
      }
      private static Application builApplication(MonitoringData data)
      {
         var ret = new Application
         {
            Path = $"/{data.Name}",
            Name = data.Name,
            Status = buildStatus(data),
            Supervisors = buildSupervisor(data),
            Children = null
         };

         return ret;
      }

      private static List<Supervisor> buildSupervisor(MonitoringData data)
      {
         var lRet = new List<Supervisor>();
         try
         {
            foreach (var anomaly in data.Anomalies)
            {
               lRet.Add(new Supervisor
               {
                  Status = buildSupervisorStatus(anomaly,data.ReceivingTimeUTC),
                  Descriptor = buildSupervisorDescripor(data.Module),
                  IsFailed = false,
                  IsStopped = false
               });
            }  
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            throw;
         }

         return lRet;
      }

      private static Descriptor buildSupervisorDescripor(string module)
      {
         Descriptor descriptor = new Descriptor
         {
            Id = module + " {" + GuidFromString(module).ToString()+ "}",
            Name = module,
            Version = "",
            Description = "",
            MaxHistoryLength = 0,
            PathToExecutable = "",
            Arguments = "",
            ApplicationPath = "",
            //RedStatusAppStatLogLevel = 0,
            //YellowStatusAppStatLogLevel = 0,
            //PersistentAppStatLogId = 0
         };
         return descriptor;
      }

      private static Status buildSupervisorStatus(Anomaly anomaly,string ReceivingTimeUTC)
      {
         DateTime receivingTimeUtc;
         DateTime.TryParse(ReceivingTimeUTC, out receivingTimeUtc);
         List<Anomaly> dataAnomalies = new List<Anomaly>(){anomaly};
         Status status = new Status
         {
            StatusDateTime = receivingTimeUtc,
            StatusCode = buildStatusCode(dataAnomalies),
            Message = buildMessage(dataAnomalies),
            Comment = null,
            Information = null
         };
         return status;
      }

      private static Status buildStatus(MonitoringData data)
      {
         DateTime receivingTimeUtc;
         DateTime.TryParse(data.ReceivingTimeUTC, out receivingTimeUtc);
         Status status = new Status
         {
            StatusDateTime = receivingTimeUtc,
            StatusCode = buildStatusCode(data.Anomalies),
            Message = buildMessage(data.Anomalies),
            Comment = null,
            Information = null
         };
         return status;
      }

      private static string buildMessage(List<Anomaly> dataAnomalies)
      {
         string sRet = "";
         dataAnomalies.ForEach(r => { sRet += $"{r.Description}<br>";});
         if (dataAnomalies.Count==0)
         {
            sRet = "is running";
         }
         return sRet;
      }

      private static int buildStatusCode(List<Anomaly> dataAnomalies)
      {
         if (dataAnomalies is null || dataAnomalies.Count==0)
         {
            return (int)UniteSupervisorStatusCode.Green;
         }

         if (dataAnomalies.Count(r => r.Severity == Anomaly.SeverityValue.Error) > 0)
            return (int)UniteSupervisorStatusCode.Red;

         if (dataAnomalies.Count(r => r.Severity == Anomaly.SeverityValue.Warning) > 0)
            return (int)UniteSupervisorStatusCode.Yellow;

         return (int)UniteSupervisorStatusCode.Green;
      }

      private static Guid GuidFromString(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        } 
   }
   public enum UniteSupervisorStatusCode
   {
      /// <summary>
      /// Represents a failing Unite feature.
      /// </summary>
      Red = 0,

      /// <summary>
      /// Represents a feature that functions but with some (minor) problems.
      /// </summary>
      Yellow = 1,

      /// <summary>
      /// Represents a feature that functions normally.
      /// </summary>
      Green = 2,

      /// <summary>
      /// Represents a feature that is on standby. Deprecated, use Standby.
      /// </summary>
      [Obsolete]
      Blue = 3,

      /// <summary>
      /// Represents a feature that is on standby.
      /// </summary>
      Standby = 4,

      /// <summary>
      /// Represents a feature that is starting.
      /// </summary>
      Starting = 5,
   }

}