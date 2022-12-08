using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Digistat.FrameworkStd.Interfaces;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace Configurator.Std.BL.Excel
{

   public abstract class ExcelReader
   {
      private readonly ILoggerService mobjLoggerService;

      public enum OfficeFileMode
      {
         BIFF,
         OpenXml
      }

      public ExcelReader(ILoggerService loggerService)
      {
         mobjLoggerService = loggerService;
      }

      public IWorkbook OpenFile(string filePath)
      {
         //TODO Trace
         mobjLoggerService.Info("Loading excel file {0}", filePath);

         try
         {
            //File not found
            if (!File.Exists(filePath))
            {
               mobjLoggerService.Error("Error loading excel document {0}: file not found", filePath);
               throw new FileNotFoundException("The specified file does not exists", filePath);
            }

            string fileExt = Path.GetExtension(filePath).ToLower();

            //File is not an excel document
            if (fileExt != "xls" && fileExt != "xlsx")
            {
               mobjLoggerService.Error("Error loading excel document {0}: unespected file extension ({1})", filePath, fileExt);
               throw new FileFormatException(new Uri(filePath), "The file is not a valid excel document");
            }

            IWorkbook mobjWorkBook = null;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {

               //Binary Interchange File Format (BIFF) prior to 2007
               if (filePath == ".xls")
               {
                  mobjWorkBook = new HSSFWorkbook(file);
               }

               //Open XML format
               if (filePath == ".xlsx")
               {
                  mobjWorkBook = new XSSFWorkbook(file);
               }
            }

            //TODO Trace
            mobjLoggerService.Info("Excel file {0} loaded", filePath);

            return mobjWorkBook;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error trying to load excel file {0}", filePath);
            throw;
         }
      }

      public IWorkbook GetEmptyWorkbook(OfficeFileMode mode)
      {            //Binary Interchange File Format (BIFF) prior to 2007
         if (mode == OfficeFileMode.BIFF)
         {
            return new HSSFWorkbook();
         }

         //Open XML format
         if (mode == OfficeFileMode.OpenXml)
         {
            return new XSSFWorkbook();
         }

         return null;
      }
      
      public void AddDataValidation(ISheet mobjSheet, CellRangeAddressList rangeList, string formula, OfficeFileMode mode) {

         IDataValidation dataValidation = null;

         if (mode == OfficeFileMode.BIFF)
         {
            dataValidation =  new HSSFDataValidation(rangeList, DVConstraint.CreateFormulaListConstraint(formula));
         }

         //Open XML format
         if (mode == OfficeFileMode.OpenXml)
         {
            //CellRangeAddressList cellRangeFieldsType = new CellRangeAddressList(1, 100, 0, 0);
            var dataValidationHelper = new XSSFDataValidationHelper((XSSFSheet)mobjSheet);
            //.CreateintConstraint(OperatorType.BETWEEN, "0", Int64.MaxValue.ToString());
            dataValidation = dataValidationHelper.CreateValidation((XSSFDataValidationConstraint)dataValidationHelper.CreateFormulaListConstraint(formula), rangeList);
         }


         mobjSheet.AddValidationData(dataValidation);
      }

      public string GetExtension(OfficeFileMode mode)
      {
         switch (mode)
         {
            case OfficeFileMode.BIFF:
               return "xls";
            case OfficeFileMode.OpenXml:
               return "xlsx";
            default:
               return "";
         }
      }

      public IWorkbook OpenFile(Stream file, string extension)
      {
         //TODO Trace
         mobjLoggerService.Info("Loading excel file from stream");

         try
         {

            IWorkbook mobjWorkBook = null;

            //Binary Interchange File Format (BIFF) prior to 2007
            if (extension == ".xls")
            {
               mobjWorkBook = new HSSFWorkbook(file);
            }

            //Open XML format
            if (extension == ".xlsx")
            {
               mobjWorkBook = new XSSFWorkbook(file);
            }

            //TODO Trace
            mobjLoggerService.Info("Excel file loaded from stream");

            return mobjWorkBook;
         }
         catch (Exception e)
         {
            mobjLoggerService.ErrorException(e, "Error trying to load excel file from stream");
            throw;
         }
         finally
         {
            file.Close();
            file.Dispose();
         }
      }

      public ISheet GetFirstSheet(IWorkbook workbook)
      {


         if (workbook == null)
         {
            mobjLoggerService.Error("Unable to get first sheet: workbook is null");
            return null;
         }


         if (workbook.NumberOfSheets == 0)
         {
            mobjLoggerService.Error("Unable to get first sheet: workbook has no sheets");
            return null;

         }

         return workbook.GetSheetAt(0);
      }

      public ISheet GetSheetByName(IWorkbook workbook, string sheetName)
      {


         if (workbook == null)
         {
            mobjLoggerService.Error("Unable to get sheet named {0}: workbook is null", sheetName);
            return null;
         }


         var sheet = workbook.GetSheet(sheetName);

         if (sheet == null)
         {
            mobjLoggerService.Error("Unable to get sheet named {0}: workbook has no sheets", sheetName);
            return null;
         }

         return sheet;
      }

      public string GetFirstSheetName(IWorkbook workbook)
      {
         string strRet = null;
         if (workbook != null)
         {
            if (workbook.NumberOfSheets > 0)
            {
               strRet = workbook.GetSheetAt(0).SheetName;
            }
         }
         return strRet;
      }
   }

}
