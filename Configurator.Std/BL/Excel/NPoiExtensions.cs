using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Configurator.Std.BL.Excel
{
   public static class NPoiExtensions
   {

      public static bool TryGetValue(this ISheet sheet, int rowIndex, int colIndex, out string value)
      {
         bool bolRet = false;
         value = null;

         if (sheet != null)
         {
            if (sheet.LastRowNum >= rowIndex)
            {
               if (sheet.GetRow(0).LastCellNum >= colIndex)
               {
                  var cell = sheet.GetRow(rowIndex).GetCell(colIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK);

                  if (cell != null)
                  {
                     value = cell.ToString();
                     bolRet = true;
                  }
               }
            }
         }

         return bolRet;
      }

      public static bool TryGetCellValue(this IRow row, int colIndex, out string value)
      {
         bool bolRet = false;
         value = null;

         if (row != null)
         {
            if (row.LastCellNum >= (int)colIndex)
            {
               var cell = row.GetCell((int)colIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK);

               if (cell != null)
               {
                  value = cell.ToString();
                  bolRet = true;

               }
            }
         }

         return bolRet;
      }


      /// <summary>
      /// Get cell value as string
      /// </summary>
      /// <returns>Returns cell value as string or null if not available</returns>
      public static string GetStringOrNullValue(this IRow row, int colIndex)
      {

         if (row == null) return null;
         if (row.LastCellNum < colIndex) return null;

         var cell = row.GetCell((int)colIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK);

         if (cell == null)
         {
            return null;
         }

         switch (cell.CellType)
         {
            case CellType.String:
               return cell.StringCellValue;
            default:
               return cell.ToString();
         }
      }


      /// <summary>
      /// Get cell value as short integer
      /// </summary>
      /// <returns>Returns cell value as short integer or null if not short integer</returns>
      public static Int16? GetShortOrNullValue(this IRow row, int colIndex)
      {
         double? value = row.GetDoubleOrNullValue(colIndex);

         if (!value.HasValue) return null;

         try
         {
            return Convert.ToInt16(value.Value);
         }
         catch (Exception)
         {
            return null;
         }
      }

      /// <summary>
      /// Get cell value as integer
      /// </summary>
      /// <returns>Returns cell value as integer or null if not integer</returns>
      public static Int32? GetIntOrNullValue(this IRow row, int colIndex)
      {
         double? value = row.GetDoubleOrNullValue(colIndex);

         if (!value.HasValue) return null;

         try
         {
            return Convert.ToInt32(value.Value);
         }
         catch (Exception)
         {
            return null;
         }       
      }

      /// <summary>
      /// Get cell value as long integer
      /// </summary>
      /// <returns>Returns cell value as long integer or null if not long integer</returns>
      public static Int64? GetLongOrNullValue(this IRow row, int colIndex)
      {
         double? value = row.GetDoubleOrNullValue(colIndex);

         if (!value.HasValue) return null;

         try
         {
            return Convert.ToInt16(value.Value);
         }
         catch (Exception)
         {
            return null;
         }
      }

      /// <summary>
      /// Get cell value as decimal
      /// </summary>
      /// <returns>Returns cell value as decimal or null if not decimal</returns>
      public static Decimal? GetDecimalOrNullValue(this IRow row, int colIndex)
      {
         double? value = row.GetDoubleOrNullValue(colIndex);

         if (!value.HasValue) return null;

         try
         {
            return Convert.ToDecimal(value.Value);
         }
         catch (Exception)
         {
            return null;
         }
      }

      /// <summary>
      /// Get cell value as double
      /// </summary>
      /// <returns>Returns cell value as double or null if not numeric</returns>
      public static double? GetDoubleOrNullValue(this IRow row, int colIndex)
      {

         if (row == null) return null;
         if (row.LastCellNum < colIndex) return null;

         var cell = row.GetCell((int)colIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK);

         if (cell == null) {
            return null;
         }

         double result;

         switch (cell.CellType)
         {
            case CellType.Numeric:
               return cell.NumericCellValue;
            case CellType.String:
               return double.TryParse(cell.StringCellValue, out result) ? result : (double?)null;
            default:
               return double.TryParse(cell.ToString(), out result) ? result : (double?)null;
         }
      }

      /// <summary>
      /// Get cell value as boolean
      /// </summary>
      /// <returns>Returns cell value as boolean or null if not boolean</returns>
      public static bool? GetBoolOrNullValue(this IRow row, int colIndex)
      {

         if (row == null) return null;
         if (row.LastCellNum < colIndex) return null;

         var cell = row.GetCell((int)colIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK);

         if (cell == null)
         {
            return null;
         }

         Boolean result;

         switch (cell.CellType)
         {
            case CellType.Boolean:
               return cell.BooleanCellValue;
            case CellType.String:
               return Boolean.TryParse(cell.StringCellValue, out result) ? result : (bool?)null;
            default:
               return Boolean.TryParse(cell.ToString(), out result) ? result : (bool?)null;
         }
      }

   }
}
