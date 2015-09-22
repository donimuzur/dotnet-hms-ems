﻿using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;

namespace Sampoerna.EMS.Website.Code
{
    public class PdfResult : PartialViewResult
    {
        // Setting a FileDownloadName downloads the PDF instead of viewing it
        public string FileDownloadName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Set the model and data
            context.Controller.ViewData.Model = Model;
            ViewData = context.Controller.ViewData;
            TempData = context.Controller.TempData;


            // Get the view name
            if (string.IsNullOrEmpty(ViewName))
            {
                ViewName = context.RouteData.GetRequiredString("action");
            }

            // Get the view
            ViewEngineResult viewEngineResult = null;
            if (View == null)
            {
                viewEngineResult = FindView(context);
                View = viewEngineResult.View;
            }

            // Render the view
            StringBuilder sb = new StringBuilder();
            using (TextWriter tr = new StringWriter(sb))
            {
                ViewContext viewContext = new ViewContext(context, View, ViewData, TempData, tr);
                View.Render(viewContext, tr);
            }
            if (viewEngineResult != null)
            {
                viewEngineResult.ViewEngine.ReleaseView(context, View);
            }

            // Create a PDF from the rendered view content
            var workStream = new MemoryStream();
            var document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, workStream);
            writer.CloseStream = false;
            document.Open();
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, stream, null as Stream);
            document.Close();

            // Save the PDF to the response stream
            FileContentResult result = new FileContentResult(workStream.ToArray(), "application/pdf")
            {
                FileDownloadName = FileDownloadName
            };
            result.ExecuteResult(context);
        }
    }
}