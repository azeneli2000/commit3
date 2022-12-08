using Digistat.Dal.Interfaces;
using Digistat.FrameworkStd.Model.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.Monitoring
{
   public interface IMonitoringRawResponseManager : IDalManagerBase<MonitoringRawResponse>
   {
      MonitoringRawResponse GetByID(int id);
      MonitoringData GetMonitoringData(int id);
      List<MonitoringData> GetLastMonitoringInfo(MonitoringData.MonitoringType enuType, int reqFreqMin, out DateTime dtmLastRequest);

      MonitoringData GetLastMonitoringInfoByHostname(string strHostname);

      Dictionary<DateTime, double> GetNodeValueChart(string name, string component, string nodename, string endDate);

      IEnumerable<MonitoringDesktop> GetAvaliableDesktop();
      bool CheckHostAvaliable(string hostname);
      List<MonitoringData> GetMonitoringResponseByRequestId(int requestId);
   }
}
