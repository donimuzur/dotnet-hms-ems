using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.AspnetWebForms
{
    public partial class GenericReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                string reportPath = Session[Constans.SessionKey.ReportPath].ToString();
                var reportDataSources = (List<ReportDataSource>)Session[Constans.SessionKey.ReportDataSources];

                myviewer.ProcessingMode = ProcessingMode.Local;
                myviewer.LocalReport.ReportPath = Server.MapPath("~/" + reportPath);
                myviewer.LocalReport.EnableExternalImages = true;
                
                //set ReportParameters
                var parameters = (List<ReportParameter>) Session[Constans.SessionKey.ReportParameters];
                if(parameters != null)
                    myviewer.LocalReport.SetParameters(parameters);

                //Set ReportDataSource
                myviewer.LocalReport.DataSources.Clear();
                if (reportDataSources == null) return;
                foreach (var reportDataSource in reportDataSources)
                {
                    myviewer.LocalReport.DataSources.Add(reportDataSource);
                }
            }
        }
    }
}