using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Configurator.Std.BL.Monitoring
{
   public class MonitoringRawResponseManager : DalManagerBase<MonitoringRawResponse>, IMonitoringRawResponseManager
   {

      private IMonitoringRawRequestManager mobjReqManager;
      public MonitoringRawResponseManager(DigistatDBContext context, ILoggerService logSvc, IMonitoringRawRequestManager reqMan)
      {
         mobjDbContext = context;
         mobjLoggerService = logSvc;
         mobjReqManager = reqMan;
      }

      public MonitoringRawResponse GetByID(int id)
      {
         try
         {
            mobjLoggerService.Info("Executing Get for MonitoringRawResponse with id {0}", id);

            MonitoringRawResponse ret = mobjDbContext.Set<MonitoringRawResponse>()
               .Where(x => x.mre_id == id)
               .SingleOrDefault();

            return ret;
         }
         catch (Exception e)
         {
            string message = string.Format("Error on retrieve MonitoringRawResponse with id {0}", id);
            mobjLoggerService.ErrorException(e, message);
         }

         return null;
      }

      public MonitoringData GetMonitoringData(int id)
      {
         var objDbRecord = GetByID(id);
         if (objDbRecord != null)
         {
            try
            {
               MonitoringData ret = MonitoringData.BuildByRawResponse(objDbRecord, mobjLoggerService);
               return ret;
            }
            catch (Exception e)
            {
               string message = string.Format("Error on retrieve MonitoringRawResponse with id {0}", id);
               mobjLoggerService.ErrorException(e, message);
            }
         }
         return null;
      }

      public List<MonitoringData> GetLastMonitoringInfo(MonitoringData.MonitoringType enuType, int reqFreq, out DateTime dtmLastRequest)
      {
         try
         {
            mobjLoggerService.Info( $"Executing GetLastMonitoringInfo");
            var ret = new List<MonitoringData>();
            var now = DateTime.UtcNow;

          

            var type = (int)enuType;
            var days = 7;


            var grouped = mobjDbContext.Set<MonitoringRawResponse>().FromSqlRaw($@"select * from (
                  select i.*, ROW_NUMBER() OVER (partition by mre_Name order by mre_ReceivingTimeUTC desc) as NUM
                  from MonitoringRawResponse i
                  where i.mre_Type = {type} and i.mre_ReceivingTimeUTC > dateadd(day, -{days}, getutcdate())
                  ) a where NUM = 1 ").ToList();

            //NOTE:  the request for service and hw info use the same Type = 1
            var lastRequests = mobjReqManager.GetLastRequests( MonitoringData.MonitoringType.Service );
            //dtmLastRequest = lastRequests.Count() > 0 ? lastRequests[0].mrq_RequestTimeUTC : DateTime.MinValue;
            dtmLastRequest = lastRequests.Count() > 0 ? (new DateTime(lastRequests[0].mrq_RequestTimeUTC.Ticks, DateTimeKind.Utc)) : DateTime.MinValue;

            foreach (var monitoring in grouped)
            {
               if (monitoring != null)
               {
                  var temp = MonitoringData.BuildByRawResponse(monitoring, mobjLoggerService);
                  //temp.Components = temp.Components.OrderByDescending(c1 => c1.Indicators.Count > 0).ToList();

                  if (enuType == MonitoringData.MonitoringType.Service)
                  {
                     //if (monitoring.mrq_id != lastRequests[0].mrq_id && monitoring.mre_ReceivingTimeUTC > lastRequests[1].mrq_RequestTimeUTC)
                     var missingResponse = monitoring.mrq_id != lastRequests[0].mrq_id && monitoring.mrq_id != lastRequests[1].mrq_id;
                     var oldRequest = monitoring.mre_ReceivingTimeUTC.AddMinutes(2 * reqFreq) < now;
                     if ( missingResponse || oldRequest)
                     {
                        if (temp.Anomalies.Count > 0)
                        {
                  
                           temp.Anomalies = temp.Anomalies.OrderByDescending(x => x.Severity).ToList();
                        }
                        temp.Anomalies.Insert(0, new Anomaly(missingResponse ? "SERVICE_DOWN" : "OLD_DATA", Anomaly.SeverityValue.Error, $"{monitoring.mre_Name} is not responding, verify the log file in the application folder"));
                     }
                  }

                  ret.Add(temp);
               }  
            }

            return ret;
         }
         catch (Exception e)
         {
            string message = string.Format("Error on retrieve Last Monitoring Info");
            mobjLoggerService.ErrorException(e, message);
         }

         dtmLastRequest = DateTime.MinValue;
         return null;
      }



      public Dictionary<DateTime, double> GetNodeValueChart(string name, string component, string nodename, string endDate)
      {

         Dictionary<DateTime, double> indicators = new Dictionary<DateTime, double>();
         try
         {
            DbConnection connection = mobjDbContext.Database.GetDbConnection();
            string startDate = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm tt");
            var connectionState = connection.State;
            if (connectionState != ConnectionState.Open) connection.Open();
            using (var cmd = connection.CreateCommand())
            {
               cmd.CommandText = "SELECT r.Result, r.RangeTime from " +
               "(SELECT [mre_Status].value('(/Status/Components/Component[@name=''" + component + "'']/Indicators/Indicator[@name=''" + nodename + "'']/@value)[1]', 'int') AS Result, [mre_ReceivingTimeUTC] AS RangeTime FROM [MonitoringRawResponse] where mre_Name = '" + name + "' AND mre_ReceivingTimeUTC BETWEEN '" + endDate + "' AND '" + startDate + "' ) r " +
               " WHERE r.Result IS NOT NULL ORDER BY r.RangeTime ";

               using (var objReader = cmd.ExecuteReader())
               {
                  while (objReader.Read())
                  {
                     var x = (DateTime)objReader["RangeTime"];
                     var y = objReader["Result"].ToString();
                     if (!String.IsNullOrEmpty(y))
                     {
                        if(!indicators.ContainsKey(x))
                        indicators.Add((DateTime)objReader["RangeTime"], Convert.ToDouble(objReader["Result"].ToString()));
                     }
                  }
               }
            }
            
         }
         catch (Exception e)
         {
            string message = $"Error on chart request: {name}:{component} - {nodename}";
            mobjLoggerService.ErrorException(e, message);
         }
         return indicators;
      }

      public MonitoringData GetLastMonitoringInfoByHostname(string strHostname)
      {
         try
         {
            mobjLoggerService.Info($"Executing GetLastMonitoringInfoByHostname");
            
            var objRawResponse = mobjDbContext.Set<MonitoringRawResponse>().Where(x => x.mre_Hostname == strHostname && x.mre_Type==0).OrderByDescending(a => a.mre_ReceivingTimeUTC).FirstOrDefault();

            var ret = MonitoringData.BuildByRawResponse(objRawResponse, mobjLoggerService);
            ret.Components.Sort((c1, c2) => c2.Indicators.Count.CompareTo(c1.Indicators.Count));

            return ret;
         }
         catch (Exception e)
         {
            string message = $"Error on retrieve Last Monitoring Info for Hostname {strHostname}";
            mobjLoggerService.ErrorException(e, message);
         }

         return null;
      }

      public IEnumerable<MonitoringDesktop> GetAvaliableDesktop()
      {

         //EF version
         //TO FIX: Filter by mre_Type = 0 ? O mre_Name ='CONTROLBAR' 
         IQueryable<Location> objLocation = mobjDbContext.Set<Location>();
         IQueryable<HospitalUnit> objHU = mobjDbContext.Set<HospitalUnit>();
         IQueryable<User> objUser = mobjDbContext.Set<User>();

         var objMonitoring = mobjDbContext.Set<MonitoringRawResponse>().FromSqlRaw($@"
                        select mre_Hostname from (
                        select i.*, ROW_NUMBER() OVER (partition by mre_Name order by mre_ReceivingTimeUTC desc) as NUM
                        from [Digistat].[dbo].[MonitoringRawResponse] i
                        where i.mre_Type= 0 AND  i.mre_ReceivingTimeUTC > dateadd(day, -7, getutcdate())
                        ) a   GROUP BY mre_Hostname");

         var objNetwork = from d in mobjDbContext.Set<Network>().OrderByDescending(z => z.Id)
                          join s in objLocation on d.LocationRef equals s.Id into matching
                          from joinResult in matching.DefaultIfEmpty()
                          join a in objHU on joinResult.HospitalUnitGuid equals a.GUID
                          into matching2
                          from joinResult2 in matching2.DefaultIfEmpty()
                          join z in objUser on d.UserId equals z.Id
                          into matching3
                          from joinResult3 in matching3.DefaultIfEmpty()
                          where d.HostName != null && (joinResult2 == null || joinResult2.Current == true) && (joinResult3.Current == true || joinResult3 == null )
                          select new { Network = d, HU = joinResult.Id == 0 ? "" : joinResult2.Name, Locationname = joinResult.LocationName == null ? "" : joinResult.LocationName , Abbrev = joinResult3.Abbrev };


         var obJRet = from x in objNetwork
                
                      join y in objMonitoring on x.Network.HostName equals y.mre_Hostname into matching
                      from joinResult in matching.DefaultIfEmpty()
                     
                      select new MonitoringDesktop { Network = x.Network, isNetworkEnabled = joinResult.mre_Hostname != null, HU = x.HU, LocationName = x.Locationname, User = x.Abbrev, Module =x.Network.Modules };

         return obJRet;
       

      }

      public bool CheckHostAvaliable(string hostname)
      {
         try
         {

            var objMonitoring = mobjDbContext.Set<MonitoringRawResponse>().FromSqlRaw($@"select * from (
                  select i.*, ROW_NUMBER() OVER (partition by mre_Name order by mre_ReceivingTimeUTC desc) as NUM
                  from MonitoringRawResponse i
                  where  i.mre_ReceivingTimeUTC > dateadd(day, -7, getutcdate())
                  ) a where NUM = 1 and mre_Hostname = {hostname} ").ToList();
            if (objMonitoring.Count > 0)
            {
               return true;
            }
            else
            {
               return false;
            }

         }
         catch (Exception ex)
         {
            string message = $"Error on retrieve Hostname in Network {hostname}";
            mobjLoggerService.ErrorException(ex, message);
            return false;
         }

      }

      //Client,service, system info

      //public List<MonitoringRawResponse> GetMonitoringResponseByRequestId(int requestId)
      //{
      //   var result = new List<MonitoringRawResponse>();
      //   try
      //   {
      //      mobjLoggerService.Info($"Executing GetMonitoringResponseByRequestId");
            
      //      result = mobjDbContext.Set<MonitoringRawResponse>().AsNoTracking().Where(x => x.mrq_id == requestId).OrderBy(a => a.mre_id).ToList();

      //   }
      //   catch (Exception e)
      //   {
      //      mobjLoggerService.ErrorException(e,"Error reading MonitoringRawResponse");
      //   }
      //   return result;

      //}
      public List<MonitoringData> GetMonitoringResponseByRequestId(int requestId)
      {
         var result = new List<MonitoringData>();
         try
         {
            mobjLoggerService.Info($"Executing GetMonitoringResponseByRequestId");
            var objRawResponse = mobjDbContext.Set<MonitoringRawResponse>().AsNoTracking().Where(x => x.mrq_id == requestId).OrderBy(a => a.mre_id).ToList();
            foreach (var mrr in objRawResponse)
            {
               if (!result.Exists(f=>f.Name == mrr.mre_Name && f.Module== mrr.mre_Module))
               {
                  result.Add(MonitoringData.BuildByRawResponse(mrr, mobjLoggerService));
               }
            }
            //objRawResponse.ForEach(row =>
            //{
            //   result.Add(MonitoringData.BuildByRawResponse(row, mobjLoggerService));
            //});
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e,"Error reading MonitoringRawResponse in GetMonitoringResponseByRequestId");
         }
         return result;

      }

   }
   
}
