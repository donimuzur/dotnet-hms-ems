
using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Sampoerna.EMS.Utils
{
    public static class ZipHelper
    {
        public static void CreateZipSample()
        {
            using (ZipArchive newFile = ZipFile.Open(@"D:\Temp\EMS\zipFolder\testzip", ZipArchiveMode.Create))
            {
                //Here are two hard-coded files that we will be adding to the zip
                //file.  If you don't have these files in your system, this will
                //fail.  Either create them or change the file names.
                newFile.CreateEntryFromFile(@"D:\Temp\EMS\Test1.txt", "File1.txt");
                newFile.CreateEntryFromFile(@"D:\Temp\EMS\Test2.txt", "Test2.txt", CompressionLevel.Fastest);
            }
        }

        public static void CreateZip(List<string> listFile , string folderPath, string zipFileName)
        {
            //sFileName = folderPath + "XmlLogZip" + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") ;

            //string zipName = folderPath + "XmlLogZip" + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + userId;

            using (ZipArchive newFile = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
            {
                //Here are two hard-coded files that we will be adding to the zip
                //file.  If you don't have these files in your system, this will
                //fail.  Either create them or change the file names.
                foreach (var fileName in listFile)
                {
                    newFile.CreateEntryFromFile(folderPath + fileName, fileName, CompressionLevel.Fastest);
                }
                
            }
        }
    }
}
