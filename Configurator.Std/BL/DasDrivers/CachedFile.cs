using Digistat.FrameworkStd.Helpers;
using Digistat.FrameworkStd.UMSLegacy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.BL.DasDrivers
{
   public class CachedFile
   {

      public string Name { get; set; }
      public byte[] Content { get; set; }
      public string Mimetype { get; set; }

      public CachedFile() { }

      public CachedFile(string name, Stream content)
      {         
         this.Name = name;
         this.Mimetype = getMIMEType(this.Name);
         this.Content = ConversionsHelper.StreamToByteArray(content);

         //var memoryStream = new MemoryStream();
         //content.CopyTo(memoryStream);
         //byte[] myBynary = memoryStream.ToArray();
         //this.Content = memoryStream.ToArray();
      }

      public CachedFile(string name, Byte[] content)
      {
         this.Name = name;
         this.Mimetype = getMIMEType(this.Name);
         this.Content = content;

         //var memoryStream = new MemoryStream();
         //content.CopyTo(memoryStream);
         //byte[] myBynary = memoryStream.ToArray();
         //this.Content = memoryStream.ToArray();
      }

      public bool CheckIsCompressed()
      {
         return UMSFrameworkCompression.IsCompressedData(Content);
      }

      private string getMIMEType(string filename)
      {

         string result = System.Net.Mime.MediaTypeNames.Application.Octet;

         var extension = System.IO.Path.GetExtension(filename).ToLower().Trim();

         switch (extension)
         {
            case "png":
               result = "image/png";
               break;
            case "jpe":
            case "jpg":
            case "jpeg":
               result = System.Net.Mime.MediaTypeNames.Image.Jpeg;
               break;
            case "gif":
               result = System.Net.Mime.MediaTypeNames.Image.Gif;
               break;
            case "tif":
            case "tiff":
               result = System.Net.Mime.MediaTypeNames.Image.Tiff;
               break;
            case "bm":
            case "bmp":
               result = "image/bmp";
               break;
            case "g3":
               result = "image/g3fax";
               break;
            case "ico":
               result = "image/x-icon";
               break;
            case "doc":
            case "dot":
               result = "application/msword";
               break;
            case "docx":
               result = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
               break;
            case "dotx":
               result = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
               break;
            case "docm":
            case "dotm":
               result = "application/vnd.ms-word.document.macroEnabled.12";
               break;
            case "xls":
            case "xlt":
            case "xla":
               result = "application/vnd.ms-excel";
               break;
            case "xlsx":
               result = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
               break;
            case "xltx":
               result = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
               break;
            case "xlsm":
               result = "application/vnd.ms-excel.sheet.macroEnabled.12";
               break;
            case "xltm":
               result = "application/vnd.ms-excel.template.macroEnabled.12";
               break;
            case "xlam":
               result = "application/vnd.ms-excel.addin.macroEnabled.12";
               break;
            case "xlsb":
               result = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
               break;
            case "ppt":
            case "pot":
            case "pps":
            case "ppa":
               result = "application/vnd.ms-powerpoint";
               break;
            case "pptx":
               result = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
               break;
            case "potx":
               result = "application/vnd.openxmlformats-officedocument.presentationml.template";
               break;
            case "ppsx":
               result = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
               break;
            case "ppam":
               result = "application/vnd.ms-powerpoint.addin.macroEnabled.12";
               break;
            case "pptm":
               result = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
               break;
            case "potm":
               result = "application/vnd.ms-powerpoint.template.macroEnabled.12";
               break;
            case "ppsm":
               result = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
               break;
            case "mdb":
               result = "application/vnd.ms-access";
               break;

         }

         return result;
      }


   }
}
