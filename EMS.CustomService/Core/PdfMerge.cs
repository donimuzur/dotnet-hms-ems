using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.CustomService.Core
{
    public class PdfMerge
    {
        public PdfMerge()
        {

        }

        public static bool Execute(String[] sourcePaths, String targetPath)
        {
            try
            {
                Console.WriteLine("Begin merging {0} pdf documents . . . ", sourcePaths.Length);
                byte[] mergedPdf = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Document document = new Document(PageSize.A4, 4, 3, 3, 3))
                    {
                        using (PdfCopy copy = new PdfSmartCopy(document, ms))
                        {
                            document.Open();

                            for (int i = 0; i < sourcePaths.Length; ++i)
                            {
                                Console.WriteLine("Extracting {0} . . . ", sourcePaths[i]);
                                PdfReader reader = new PdfReader(sourcePaths[i]);
                                // loop over the pages in that document
                                int n = reader.NumberOfPages;
                                for (int page = 0; page < n;)
                                {
                                    copy.AddPage(copy.GetImportedPage(reader, ++page));
                                }
                                copy.FreeReader(reader);
                                reader.Close();
                            }
                        }
                    }
                    mergedPdf = ms.ToArray();
                    Console.WriteLine(String.Format("Merging {0} bytes of pdf documents", mergedPdf.Length));
                    using (Stream os = new FileStream(targetPath, FileMode.Create))
                    {
                        os.Write(mergedPdf, 0, mergedPdf.Length);
                        os.Flush();

                    }
                    return true;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
