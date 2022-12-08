using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.DAS3Plus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Std.BL.DAS3Plus
{
   public class DASBrokerManager : IDASBrokerManager
   {

      protected readonly DigistatDBContext mobjDbContext;
      protected readonly ILoggerService mobjLogSvc;

      public DASBrokerManager(DigistatDBContext context, ILoggerService loggerService)
      {
         mobjDbContext = context;
         mobjLogSvc = loggerService;
      }


      public List<DASBroker> GetList()
      {
         List<DASBroker> objRet = new List<DASBroker>();

         using (var conn = mobjDbContext.Database.GetDbConnection())
         {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
               cmd.CommandText = @"select hostname from (
                  select a.op_HostName as Hostname, (select top 1 b.op_Value from SystemOptions b where op_Application = 'DASNODE' 
                  and op_Name = 'DASBrokerEnabled' and (b.op_HostName is NULL or b.op_HostName = a.op_HostName) order by b.op_Hostname DESC) as BrokerEnabled
                  from
                  (
                  select distinct op_Hostname 
                  from SystemOptions 
                  where op_Application like 'DASNODE|Broker'
                  and op_HostName is not null
                  ) a
                  ) g
                  where BrokerEnabled = '1'";
               using (var reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     string strHostname = reader.GetString(0);
                     if (!string.IsNullOrEmpty(strHostname))
                     {
                        string[] strSplitted = strHostname.Split('§');
                        if(strSplitted.Count()==2)
                        {
                           DASBroker objDB = new DASBroker() {
                              Hostname = strSplitted[0]
                           };
                           objRet.Add(objDB);
                        }
                     }  
                  }
               }
            }
            conn.Close();
         }
        
         return objRet;
      }
   }
}
