using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NPOI.SS.UserModel;

using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.UMSLegacy;
using Configurator.Std.BL.DasDrivers;
using NPOI.SS.Util;

namespace Configurator.Std.BL.Excel
{
   public class EventMappingExport: ExcelReader
   {

      private enum DriverRepositoryRowsIndex
      {
         DriverName = 0,
         DriverBuildVersion = 1,
         Manufacturer = 2,
         Device = 3,
         SupportedModels = 4,
         SignedXml = 6,
         EventCatalog = 10
      }

      private enum DriverRepositoryColumnsIndex
      {
         Label = 0,
         Value = 1,
      }

      private enum RemappedEventsColumnsIndex
      {
         Code = 0,
         Level = 1,
         Class = 2,
         Text = 3,
         TextShort = 4,
         NewLevel = 5,
         NewClass = 6,
         TextENG = 7,
         TextShortENG = 8,
         TextUser = 9,
         TextShortUser = 10
      }

      public EventMappingExport(ILoggerService loggerService) : base(loggerService) {}

      public DriverRepository GetDriverRepositoryFromXls(string filePath)
      {

         IWorkbook mobjWorkBook = null;
         try
         {
            mobjWorkBook = this.OpenFile(filePath);
            return ReadDriverRepositoryFromXls(mobjWorkBook);
            
         }
         catch (FileFormatException fe)
         {
            throw new FileFormatException(new Uri(filePath), fe.Message);
         }
         catch (Exception)
         {
            throw;
         }
         finally
         {
            if (mobjWorkBook != null)
            {
               mobjWorkBook.Close();
            }
         }
      }

      public DriverRepository GetDriverRepositoryFromXls(Stream file,  string fileExtension)
      {

         IWorkbook mobjWorkBook = null;
         try
         {
            mobjWorkBook = this.OpenFile(file, fileExtension);
            return ReadDriverRepositoryFromXls(mobjWorkBook);
         }
         catch
         {
            throw;
         }
         finally
         {
            if (mobjWorkBook != null)
            {
               mobjWorkBook.Close();
            }
         }
      }


      public CachedFile GetDriverRepositoryXls(DriverRepository driver, OfficeFileMode mode = OfficeFileMode.OpenXml)
      {
         IWorkbook mobjWorkBook = GetEmptyWorkbook(mode);

         try
         {

            ISheet mobjSheet = mobjWorkBook.CreateSheet(string.Format("Driver {0} v. {1}", driver.DriverName, driver.DriverVersion));

            //Driver Infos
            createInfoRow(mobjSheet, DriverRepositoryRowsIndex.DriverName, "DRIVER:", string.Format("{0} {1}", driver.DriverName, driver.DriverVersion));
            createInfoRow(mobjSheet, DriverRepositoryRowsIndex.DriverBuildVersion, "Build:", driver.DriverVersionBuild);
            createInfoRow(mobjSheet, DriverRepositoryRowsIndex.Manufacturer, "Manufacturer:", driver.Manufacturer);
            createInfoRow(mobjSheet, DriverRepositoryRowsIndex.Device, "Device:", driver.Device);
            createInfoRow(mobjSheet, DriverRepositoryRowsIndex.SupportedModels, "Supported models:", driver.Model);
            
            createInfoRow(mobjSheet, DriverRepositoryRowsIndex.SignedXml, "Signed xml:", driver.RemappedEvents);

            //Event Catalog Grid Header
            var header = mobjSheet.CreateRow((int)DriverRepositoryRowsIndex.EventCatalog - 1);
            header.CreateCell((int)RemappedEventsColumnsIndex.Code).SetCellValue("Code");
            header.CreateCell((int)RemappedEventsColumnsIndex.Level).SetCellValue("Level");
            header.CreateCell((int)RemappedEventsColumnsIndex.Class).SetCellValue("Class");
            header.CreateCell((int)RemappedEventsColumnsIndex.Text).SetCellValue("Text");
            header.CreateCell((int)RemappedEventsColumnsIndex.TextShort).SetCellValue("Short Text");
            header.CreateCell((int)RemappedEventsColumnsIndex.NewLevel).SetCellValue("New Level");
            header.CreateCell((int)RemappedEventsColumnsIndex.NewClass).SetCellValue("New Class");
            header.CreateCell((int)RemappedEventsColumnsIndex.TextENG).SetCellValue("Text Eng.");
            header.CreateCell((int)RemappedEventsColumnsIndex.TextShortENG).SetCellValue("Short Text Eng.");
            header.CreateCell((int)RemappedEventsColumnsIndex.TextUser).SetCellValue("Text User");
            header.CreateCell((int)RemappedEventsColumnsIndex.TextShortUser).SetCellValue("Short Text User");

            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.Code, 7000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.Level, 7000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.Class, 7000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.Text, 20000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.TextShort, 10000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.NewLevel, 7000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.NewClass, 7000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.TextENG, 20000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.TextShortENG, 10000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.TextUser, 20000);
            mobjSheet.SetColumnWidth((int)RemappedEventsColumnsIndex.TextShortUser, 10000);


            int idx = (int)DriverRepositoryRowsIndex.EventCatalog;
            foreach (var ev in driver.EventsMapping)
            {
               createEventRow(mobjSheet, idx, ev);
               idx++;
            }

            ISheet mobjOptionsSheet = mobjWorkBook.CreateSheet("Option Values");

            var alarmClassList = UMSFrameworkParser.GetAlarmClassList();
            var eventTypeList = UMSFrameworkParser.GetEventTypeList();

            for (int rowNum = 0; rowNum <= Math.Max(alarmClassList.Count, eventTypeList.Count); rowNum++) {
               mobjOptionsSheet.CreateRow(rowNum);
            }

            mobjOptionsSheet.GetRow(0).CreateCell(0).SetCellValue("Event Class");
            mobjOptionsSheet.GetRow(0).CreateCell(1).SetCellValue("Event Level");

            //Auto sizing
            mobjOptionsSheet.AutoSizeColumn(0);
            mobjOptionsSheet.AutoSizeColumn(1);

            int currRow = 1;
            foreach (var alarmClass in alarmClassList)
            {
               mobjOptionsSheet.GetRow(currRow).CreateCell(0).SetCellValue(alarmClass.Value);
               currRow++;
            }

            currRow = 1;
            foreach (var eventType in eventTypeList)
            {
               mobjOptionsSheet.GetRow(currRow).CreateCell(1).SetCellValue(eventType.Value);
               currRow++;
            }
          
            if (driver.EventsMapping.Count > 0)
            {

               CellRangeAddressList rangeList = new CellRangeAddressList();
               //add the data validation to the first column (1-100 rows) 
               rangeList.AddCellRangeAddress(new CellRangeAddress((int)DriverRepositoryRowsIndex.EventCatalog, mobjSheet.LastRowNum, (int)RemappedEventsColumnsIndex.NewClass, (int)RemappedEventsColumnsIndex.NewClass));
               string formula = string.Format("'{0}'!$A${1}:$A${2}", mobjOptionsSheet.SheetName, "2", alarmClassList.Count +1);
               AddDataValidation(mobjSheet, rangeList, formula, mode);


               rangeList = new CellRangeAddressList();
               //add the data validation to the first column (1-100 rows) 
            

               rangeList.AddCellRangeAddress(new CellRangeAddress((int) DriverRepositoryRowsIndex.EventCatalog,mobjSheet.LastRowNum, (int) RemappedEventsColumnsIndex.NewLevel,(int) RemappedEventsColumnsIndex.NewLevel));
               formula = string.Format("'{0}'!$B${1}:$B${2}", mobjOptionsSheet.SheetName, "2", eventTypeList.Count + 1);
               AddDataValidation(mobjSheet, rangeList, formula, mode);
            }

            //Get excel file Byte[]
            byte[] content = null;
            using (var memoryStream = new MemoryStream()) //creating memoryStream
            {
               mobjWorkBook.Write(memoryStream);
               content = memoryStream.ToArray();
            }

            string fileName = string.Format("EventCatalog_{0}_{1}.{2}", driver.DriverName, DateTime.Now.Date.ToShortDateString(), GetExtension(mode));

            return new CachedFile(fileName, content);

         }
         catch (Exception e)
         {
            string message = string.Format("Error exporting event catalog for driver with id {0}", driver.Id);
            throw new Exception(message, e);
         }
         finally
         {
            if (mobjWorkBook != null)
            {
               mobjWorkBook.Close();
            }
         }
      }

      #region private

      private static void createInfoRow(ISheet mobjSheet, DriverRepositoryRowsIndex rowIndex, string label, string value)
      {
         var row = mobjSheet.CreateRow((int)rowIndex);
         var cell = row.CreateCell((int)DriverRepositoryColumnsIndex.Label);
         cell.SetCellValue(label);
         cell = row.CreateCell((int)DriverRepositoryColumnsIndex.Value);
         cell.SetCellValue(value);
      }

      private static void createEventRow(ISheet mobjSheet, int rowIndex, DriverRepositoryEventCatalog value)
      {
         string levelString = value.Type == -1 ? string.Empty : UMSFrameworkParser.GetEventTypeDescription(value.Type);
         string classString = value.Class == -1 ? string.Empty : UMSFrameworkParser.GetAlarmClassDescription(value.Class);

         string newLevelString = value.NewLevel == -1 ? string.Empty : UMSFrameworkParser.GetEventTypeDescription((short)value.NewLevel);
         string newClassString = value.NewClass == -1 ? string.Empty : UMSFrameworkParser.GetAlarmClassDescription((short)value.NewClass);


         var row = mobjSheet.CreateRow(rowIndex);
         row.CreateCell((int)RemappedEventsColumnsIndex.Code).SetCellValue(value.Id);
         row.CreateCell((int)RemappedEventsColumnsIndex.Level).SetCellValue(levelString);
         row.CreateCell((int)RemappedEventsColumnsIndex.Class).SetCellValue(classString);
         row.CreateCell((int)RemappedEventsColumnsIndex.Text).SetCellValue(value.DescriptionLong);
         row.CreateCell((int)RemappedEventsColumnsIndex.TextShort).SetCellValue(value.DescriptionShort);
         row.CreateCell((int)RemappedEventsColumnsIndex.NewLevel).SetCellValue(newLevelString);
         row.CreateCell((int)RemappedEventsColumnsIndex.NewClass).SetCellValue(newClassString);
         row.CreateCell((int)RemappedEventsColumnsIndex.TextENG).SetCellValue(value.TextENG);
         row.CreateCell((int)RemappedEventsColumnsIndex.TextShortENG).SetCellValue(value.TextENGShort);
         row.CreateCell((int)RemappedEventsColumnsIndex.TextUser).SetCellValue(value.TextUser);
         row.CreateCell((int)RemappedEventsColumnsIndex.TextShortUser).SetCellValue(value.TextUserShort);
      }

      private DriverRepository ReadDriverRepositoryFromXls(IWorkbook mobjWorkBook)
      {
         DriverRepository result = new DriverRepository();
         ISheet mobjSheet = this.GetFirstSheet(mobjWorkBook);
         if (mobjSheet != null)
         {
            var driverName = GetDriverName(mobjSheet);

            result.DriverName = driverName.Substring(0, driverName.LastIndexOf(' '));
            result.DriverVersion = driverName.Substring(driverName.LastIndexOf(' ') -1);
            result.DriverVersionBuild = GetDriverBuildVersion(mobjSheet);
            result.Manufacturer = GetDriverManufacturer(mobjSheet);
            result.Device = GetDriverSupportedDevices(mobjSheet);
            result.Model = GetDriverSupportedModels(mobjSheet);
            result.RemappedEvents = GetDriverSignedXml(mobjSheet);

            if (mobjSheet.LastRowNum > (int)DriverRepositoryRowsIndex.EventCatalog)
            {
               for (int i = (int)DriverRepositoryRowsIndex.EventCatalog; i <= mobjSheet.LastRowNum; i++)
               {

                  var row = mobjSheet.GetRow(i);

                  //Record key
                  string eventId = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.Code);
                  string eventLevelString = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.Level);
                  string eventClassString = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.Class);

                  short ? eventLevel =  string.IsNullOrWhiteSpace(eventLevelString) ? (short?)null : UMSFrameworkParser.GetEventTypeValueFormDescription(eventLevelString);
                  short? eventClass = string.IsNullOrWhiteSpace(eventClassString) ? (short?)null : UMSFrameworkParser.GetAlarmClassValueFromDescription(eventClassString);

                  string newLevelString = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.NewLevel);
                  string newClassString = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.NewClass);

                  short? newLevel = string.IsNullOrWhiteSpace(newLevelString) ? (short?)null : UMSFrameworkParser.GetEventTypeValueFormDescription(newLevelString);
                  short? newClass = string.IsNullOrWhiteSpace(newClassString) ? (short?)null : UMSFrameworkParser.GetAlarmClassValueFromDescription(newClassString);


                  if (string.IsNullOrEmpty(eventId) || !eventLevel.HasValue || !eventClass.HasValue)
                  {
                     continue;
                  }

                  result.EventsMapping.Add(
                     new DriverRepositoryEventCatalog
                     {
                        Id = eventId,
                        Type = eventLevel.Value,
                        Class = eventClass.Value,
                        DescriptionLong = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.Text),
                        DescriptionShort = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.TextShort),
                        NewLevel = newLevel ?? -1,
                        NewClass = newClass ?? -1,
                        TextENG = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.TextENG),
                        TextENGShort = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.TextShortENG),
                        TextUser = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.TextUser),
                        TextUserShort = row.GetStringOrNullValue((int)RemappedEventsColumnsIndex.TextShortUser),
                     }
                  );

               }
            }
         }

         return result;

      }

      private static string GetDriverName(ISheet mobjSheet)
      {

         string val = GetDriverInfo(mobjSheet, DriverRepositoryRowsIndex.DriverName, "driver");

         if (string.IsNullOrEmpty(val))
         {
            throw new FileFormatException("Driver name not found in expected position");
         }
         return val;
      }

      private static string GetDriverBuildVersion(ISheet mobjSheet)
      {
         return GetDriverInfo(mobjSheet, DriverRepositoryRowsIndex.DriverBuildVersion, "build");
      }

      private static string GetDriverManufacturer(ISheet mobjSheet)
      {
         return GetDriverInfo(mobjSheet, DriverRepositoryRowsIndex.Manufacturer, "manufacturer");
      }

      private static string GetDriverSupportedDevices(ISheet mobjSheet)
      {
         return GetDriverInfo(mobjSheet, DriverRepositoryRowsIndex.Device, "device");
      }

      private static string GetDriverSupportedModels(ISheet mobjSheet)
      {
         return GetDriverInfo(mobjSheet, DriverRepositoryRowsIndex.SupportedModels, "supported models");
      }

      private static string GetDriverSignedXml(ISheet mobjSheet)
      {
         return GetDriverInfo(mobjSheet, DriverRepositoryRowsIndex.SignedXml, "signed xml");
      }

      private static string GetDriverInfo(ISheet mobjSheet, DriverRepositoryRowsIndex rowIndex, string columnLabel)
      {
         var driverNameRow = mobjSheet.GetRow((int)rowIndex);
         string label = driverNameRow.GetStringOrNullValue((int)DriverRepositoryColumnsIndex.Label);
         if (label == null || label.ToLower().Trim().TrimEnd(new[] { ':' }) != columnLabel.ToLower().Trim().TrimEnd(new[] { ':' }))
         {
            throw new FileFormatException(columnLabel.ToUpper() + " row not found in expected position");
         }
         return driverNameRow.GetStringOrNullValue((int)DriverRepositoryColumnsIndex.Value);
      }

      #endregion

   }
}
