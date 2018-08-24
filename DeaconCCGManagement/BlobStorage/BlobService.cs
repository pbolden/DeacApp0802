using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DeaconCCGManagement.BlobStorage
{
    static public class BlobService
    {
        public static string GetNewFileName(string fileName)
        {
            string nameWithOutExt = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            string newFileName = SanitizeFileName(nameWithOutExt);
            return GetFileNameWithTimeStamp(newFileName, ext); 
        }
         
        private static string GetFileNameWithTimeStamp(string fileNameWitoutExt, string ext)
        {
            return $"{fileNameWitoutExt}-{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{ext.ToLower()}";
        }

        private static string SanitizeFileName(string fileName)
        {
            var sb = new StringBuilder();
            foreach (var chr in fileName)
                if (char.IsLetterOrDigit(chr) || chr == '_' || chr == '-') sb.Append(chr);
            
            return sb.ToString();
        }
    }
}