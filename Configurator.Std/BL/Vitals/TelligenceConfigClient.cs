using Configurator.Std.Exceptions;

using Configurator.Std.BL;
using Configurator.Std.BL.DasDrivers;
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Model.Integration.Telligence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TelligenceXMLRPCClient.Entities;
using Configurator.Std.Helpers;

namespace Configurator.Std.BL.Vitals
{
   public class TelligenceConfigClientManager :  ITelligenceConfigClientManager
   {

      private readonly ISystemOptionsService mobjSysOptSvc;
      private readonly IDigistatConfiguration mobjCfgSvc;
      private readonly ILoggerService mobjLogSvc;
      private readonly ISystemOptionsManager mobjSysOptMgr;
      private readonly ITelligenceDeviceManager mobjTelDevMgr;
      private readonly ITelligenceSystemManager mobjTelSysMgr;
      private readonly ITelligenceServerManager mobjTelSrvMgr;
      private readonly IDictionaryService mobjDicSvc;
      private readonly IPortServerManager mobjPSMgr;
      private TelligenceConfigHandlerConfiguration mobjCfgHandlerSettings = null;
      private string mstrSysOptGUID = null;

      public TelligenceConfigClientManager(ISystemOptionsService sysOptSvc, IDigistatConfiguration objCfgSvc, ILoggerService logSvc
         ,ISystemOptionsManager sysOptMgr,ITelligenceDeviceManager telDevMgr,ITelligenceSystemManager telSysMgr,ITelligenceServerManager telSrvMgr
         ,IDictionaryService dicSvc,IPortServerManager psMgr)
      {
         mobjSysOptSvc = sysOptSvc;
         mobjLogSvc = logSvc;
         mobjCfgSvc = objCfgSvc;
         mobjSysOptMgr = sysOptMgr;
         mobjTelDevMgr = telDevMgr;
         mobjTelSysMgr = telSysMgr;
         mobjTelSrvMgr = telSrvMgr;
         mobjDicSvc = dicSvc;
         mobjPSMgr = psMgr;
         InitConfigFromSysOpt();
      }

      private void InitConfigFromSysOpt()
      {
         mobjCfgHandlerSettings = new TelligenceConfigHandlerConfiguration();
         mobjCfgHandlerSettings.ServerURL = "http://[SERVERNAME]/TelligenceConfigHandler/TelligenceConfigHandler.rem";
         mobjCfgHandlerSettings.Username = "uacadmin";
         mobjCfgHandlerSettings.Password = Digistat.FrameworkStd.UMSLegacy.UMSFrameworkCompatibility.EncryptString("setmeup", null);
         string strConfigSerialized = GetXMLFromObject(mobjCfgHandlerSettings);
         SystemOption objSysOption = mobjSysOptSvc.CheckAndCreateSystemOption(mobjCfgSvc.ModuleName, null, null, null, "TelligenceHandlerConfig", strConfigSerialized, "Telligence Config Handler configuration", Digistat.FrameworkStd.Enums.OptionType.Text, 0, 0, 0, true);
         mstrSysOptGUID = objSysOption.Guid;
         if (!string.IsNullOrEmpty(objSysOption.Value))
         {
            mstrSysOptGUID = objSysOption.Guid;
            mobjCfgHandlerSettings = (TelligenceConfigHandlerConfiguration)ObjectToXML(objSysOption.Value, typeof(TelligenceConfigHandlerConfiguration));
         }

      }





      public List<Tuple<int, string>> ImportAllTLDevices(int serverID)
      {
         List<Tuple<int, string>> objRet = new  List<Tuple<int, string>>();
         try
         {

            //Retrieve server data

            TelligenceServer objServer =  mobjTelSrvMgr.Find(p => p.ts_ID == serverID).FirstOrDefault();
            if(objServer!=null && !string.IsNullOrEmpty(objServer.ts_cfgHandlerURL))
            {
               TelligenceXMLRPCClient.TelligenceConfigReader objReader = new TelligenceXMLRPCClient.TelligenceConfigReader(objServer.ts_cfgHandlerURL, objServer.ts_cfgHandlerUsername, objServer.ts_cfgHandlerPassword);
               List<TelligenceXMLRPCClient.Entities.TLSourceHostcs> objHosts = objReader.GetSourceHosts();
               System.Threading.Thread.Sleep(300);
               Tuple<List<TelligenceXMLRPCClient.Entities.TLDeviceDetail>,List<string>> objDevices = objReader.GetAvailableDevices();
               if (objHosts != null && objHosts.Count > 0)
               {
                  foreach (TelligenceXMLRPCClient.Entities.TLSourceHostcs objHost in objHosts)
                  {
                     objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"System retrieved - GUID:{objHost.SourceID} ID:{objHost.Id}")));
                     TelligenceSystem objSystem = mobjTelSysMgr.GetByHostID(objHost.Id);
                     
                     if (objSystem == null)
                     {
                        
                        //Create new telligence system
                        TelligenceSystem objSysToCreate = new TelligenceSystem();
                        objSysToCreate.ty_hostID = objHost.Id;
                        objSysToCreate.ty_telGUID = objHost.SourceID;
                        objSysToCreate.ty_ts_ID = serverID;
                        objSystem = mobjTelSysMgr.Create(objSysToCreate);
                        objRet.Add(new Tuple<int, string>(0, $"TLSystem created - GUID:{objHost.SourceID} TLSYSID:{objSystem.ty_ID}"));
                     }
                     else
                     {
                        objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"TLSystem already exists - GUID:{objHost.SourceID} TLSYSID:{objSystem.ty_ID}")));
                     }
                     List<TelligenceXMLRPCClient.Entities.TLDeviceDetail> objListDevicesForHost = objDevices.Item1.Where(p => p.Location != null && p.Location.HostID == objHost.Id).ToList();
                     
                     if (objListDevicesForHost != null)
                     {
                        foreach (TelligenceXMLRPCClient.Entities.TLDeviceDetail objDevice in objListDevicesForHost)
                        {
                           objRet.Add(new Tuple<int, string>(0, $"Device retrieved - {objDevice.ID} TLSYSID:{objSystem.ty_ID}"));
                           TelligenceDevice objDeviceCheck = mobjTelDevMgr.SearchForDevice(objDevice.ID, objSystem.ty_ID);
                           if (objDeviceCheck == null)
                           {
                              TelligenceDevice objToCreate = new TelligenceDevice();
                              objToCreate.tl_deviceID = objDevice.ID;
                              objToCreate.tl_DeviceType = objDevice.HwType;
                              objToCreate.tl_IPAddress = objDevice.IpAddress;
                              objToCreate.tl_DeviceName = objDevice.Description;
                              objToCreate.tl_locationDescriptor = objDevice.LocationDescriptor;
                              objToCreate.tl_locationID = objDevice.LocationId;
                              objToCreate.tl_MACAddress = objDevice.MacAddress;
                              objToCreate.tl_ty_ID = objSystem.ty_ID;
                              bool bolCreatePortServer = false;
                              TelligenceXMLRPCClient.Entities.StaffStationTypes devType = (TelligenceXMLRPCClient.Entities.StaffStationTypes)objDevice.HwType;
                              if (devType == TelligenceXMLRPCClient.Entities.StaffStationTypes.MDI || devType == TelligenceXMLRPCClient.Entities.StaffStationTypes.Hybrid)
                              {
                                 bolCreatePortServer = true;
                              }
                              mobjTelDevMgr.CreateDevice(objToCreate, null, null, true, bolCreatePortServer);
                              objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"TLDevice created - DEVICEID:{objToCreate.tl_deviceID} TLSYSID:{objSystem.ty_ID}")));

                           }
                           else
                           {
                              bool bolUpdateDevice = false;
                              objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"TLDevice already exists - ID:{objDeviceCheck.tl_ID} DEVICEID:{objDeviceCheck.tl_deviceID} TLSYSID:{objSystem.ty_ID}")));
                              if(!objDeviceCheck.tl_NetworkID.HasValue)
                              {
                                 //create network
                                 objDeviceCheck = mobjTelDevMgr.CreateNetworkForDevice(objDeviceCheck);
                                 objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"Created network for Device - ID:{objDeviceCheck.tl_ID} DEVICEID:{objDeviceCheck.tl_deviceID} TLSYSID:{objSystem.ty_ID} NETWORKID:{objDeviceCheck.tl_NetworkID}")));
                              }
                              if(TelligenceHelper.IsMDIDevice(objDeviceCheck))
                              {
                                 if (objDeviceCheck.tl_psv_ID == 0 && objDeviceCheck.tl_DeviceType != null)
                                 {
                                    try
                                    {
                                       objDeviceCheck = mobjTelDevMgr.CreatePortServerForDevice(objDeviceCheck);
                                       objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"Created port server for Device - ID:{objDeviceCheck.tl_ID} DEVICEID:{objDeviceCheck.tl_deviceID} TLSYSID:{objSystem.ty_ID} NETWORKID:{objDeviceCheck.tl_psv_ID}")));
                                       bolUpdateDevice = true;
                                    }
                                    catch (PortServerCreationException)
                                    {
                                       objRet.Add(new Tuple<int, string>(1, mobjDicSvc.XLate($"A Port server of a wrong type already exists - ID:{objDeviceCheck.tl_ID} DEVICEID:{objDeviceCheck.tl_deviceID} TLSYSID:{objSystem.ty_ID} NETWORKID:{objDeviceCheck.tl_psv_ID}")));
                                    }
                                 }
                                 else
                                 {
                                    //Maybe a port server has been removed
                                    if (mobjPSMgr.Find(p => p.ID == objDeviceCheck.tl_psv_ID && p.Type== (short)PortServerType.StaffStation).Count() == 0)
                                    {
                                       try
                                       {
                                          objDeviceCheck = mobjTelDevMgr.CreatePortServerForDevice(objDeviceCheck);
                                          bolUpdateDevice = true;
                                          objRet.Add(new Tuple<int, string>(0, mobjDicSvc.XLate($"Created port server for Device - ID:{objDeviceCheck.tl_ID} DEVICEID:{objDeviceCheck.tl_deviceID} TLSYSID:{objSystem.ty_ID} NETWORKID:{objDeviceCheck.tl_psv_ID}")));
                                       }
                                       catch(PortServerCreationException)
                                       {
                                          objRet.Add(new Tuple<int, string>(1, mobjDicSvc.XLate($"A Port server of a wrong type already exists - ID:{objDeviceCheck.tl_ID} DEVICEID:{objDeviceCheck.tl_deviceID} TLSYSID:{objSystem.ty_ID} NETWORKID:{objDeviceCheck.tl_psv_ID}")));
                                       }
                                       
                                       
                                       
                                    }
                                 }
                              }
                              if (bolUpdateDevice)
                              {
                                 mobjTelDevMgr.Update(objDeviceCheck);
                              }
                              
                           }
                         
                        }
                     }
                  }

               }
               else
               {
                  objRet.Add(new Tuple<int, string>(1, mobjDicSvc.XLate($"No systems retrieved")));
               }
            }
            else
            {
               objRet.Add(new Tuple<int, string>(1, mobjDicSvc.XLate($"Server information needed. Please set \"Telligence Config Handler URL\" to proceed")));
            }

            
         }
         catch (Exception exc)
         {
            mobjLogSvc.ErrorException(exc, "Error on ImportAllTLDevices ");
            throw new Exception("Error on ImportAllTLDevices", exc);
         }
         return objRet;
      }

      public Dictionary<int,string> GetAvailableTLSystems()
      {
         Dictionary<int, string> objRet = null;
         try
         {
            TelligenceXMLRPCClient.TelligenceConfigReader objReader = new TelligenceXMLRPCClient.TelligenceConfigReader(mobjCfgHandlerSettings.ServerURL, mobjCfgHandlerSettings.Username, mobjCfgHandlerSettings.Password);
            List<TelligenceXMLRPCClient.Entities.TLSourceHostcs> objList = objReader.GetSourceHosts();
            if (objList != null)
            {
               objRet = new Dictionary<int, string>();
               foreach (TelligenceXMLRPCClient.Entities.TLSourceHostcs sh in objList)
               {
                  objRet.Add(sh.Id, sh.SourceID);
               }
            }

         }
         catch (Exception exc)
         {
            mobjLogSvc.ErrorException(exc, "Error querying TelligenceXMLRPCClient.GetSourceHosts");
            throw new Exception("Error querying TelligenceXMLRPCClient.GetSourceHosts", exc);
         }
         
         return objRet;
      }


      public List<TelligenceXMLRPCClient.Entities.TLDeviceDetail> GetAvailableTLDevices()
      {
         Tuple<List<TelligenceXMLRPCClient.Entities.TLDeviceDetail>,List<string>> objRet = null;
         try
         {
            TelligenceXMLRPCClient.TelligenceConfigReader objReader = new TelligenceXMLRPCClient.TelligenceConfigReader(mobjCfgHandlerSettings.ServerURL, mobjCfgHandlerSettings.Username, mobjCfgHandlerSettings.Password);
            objRet = objReader.GetAvailableDevices();
         }
         catch(Exception exc)
         {
            mobjLogSvc.ErrorException(exc, "Error querying TelligenceXMLRPCClient.GetAvailableDevices");
            throw new Exception("Error querying TelligenceXMLRPCClient.GetAvailableDevices", exc);
         }
         if (objRet != null)
         {
            return objRet.Item1;
         }
         else
         {
            return null;
         }
      }


      public TelligenceConfigHandlerConfiguration GetCurrentSetting()
      {
         return mobjCfgHandlerSettings;
      }


      public   void SetSettings(TelligenceConfigHandlerConfiguration objToSet)
      {
         mobjSysOptMgr.UpdateValue(mstrSysOptGUID, GetXMLFromObject(objToSet));
         mobjSysOptSvc.ReloadSystemOptions(mobjCfgSvc.ModuleName);
         mobjCfgHandlerSettings = objToSet;
      }

      private static string GetXMLFromObject(object o)
      {
         StringWriter sw = new StringWriter();
         XmlTextWriter tw = null;
         try
         {
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            tw = new XmlTextWriter(sw);
            serializer.Serialize(tw, o);
         }
         catch (Exception)
         {
            //Handle Exception Code
         }
         finally
         {
            sw.Close();
            if (tw != null)
            {
               tw.Close();
            }
         }
         return sw.ToString();
      }

      public static Object ObjectToXML(string xml, Type objectType)
      {
         StringReader strReader = null;
         XmlSerializer serializer = null;
         XmlTextReader xmlReader = null;
         Object obj = null;
         try
         {
            strReader = new StringReader(xml);
            serializer = new XmlSerializer(objectType);
            xmlReader = new XmlTextReader(strReader);
            obj = serializer.Deserialize(xmlReader);
         }
         catch (Exception)
         {
            //Handle Exception Code
         }
         finally
         {
            if (xmlReader != null)
            {
               xmlReader.Close();
            }
            if (strReader != null)
            {
               strReader.Close();
            }
         }
         return obj;
      }
   }
}
