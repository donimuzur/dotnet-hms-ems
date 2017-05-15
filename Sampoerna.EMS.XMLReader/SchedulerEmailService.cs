using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Sampoerna.EMS.MessagingService;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.XMLReader
{
    public class SchedulerEmailService
    {
        public ILogger logger;
        public IUnitOfWork uow;
        
        private IGenericRepository<QUOTA_MONITORING_DETAIL> _repositoryQuotaMonitoringDetail;
        private IPBCK1BLL _pbck1BLL;
        private MessagingService.MessageService _messageService;
        private string includeString = "QUOTA_MONITORING_DETAIL";


        public SchedulerEmailService()
        {
            logger = new NLogLogger();
            uow = new SqlUnitOfWork(logger);
            
            _repositoryQuotaMonitoringDetail = uow.GetGenericRepository<QUOTA_MONITORING_DETAIL>();
            _pbck1BLL = new PBCK1BLL(uow,logger);
            _messageService = new MessageService(logger);
            BLLMapper.InitializePBCK1();
            BLLMapper.Initialize();
        }

        public void CheckAndSendEmailQuotaMonitoring()
        {
            var notSentListId = _repositoryQuotaMonitoringDetail.Get(x => x.EMAIL_STATUS == Core.Enums.EmailStatus.NotSent, null, "QUOTA_MONITORING,USER").Select(x=> x.QUOTA_MONITORING).Distinct().ToList();
            
            foreach (var i in notSentListId)
            {
                var success = false;
                Pbck1Dto pbck1Data;
                var goodTypeList = new List<string>();
                if (i.EX_GROUP_TYPE == (int)Core.Enums.ExGoodsType.EtilAlcohol)
                {
                    goodTypeList = new List<string>() { "04" };
                }
                else
                {
                    goodTypeList = new List<string>() { "02" };
                }
                var mailNotif = ProcessEmailQuotaNotification(i,out pbck1Data,goodTypeList);
                if (mailNotif != null)
                {
                    if (mailNotif.IsCCExist)
                    {
                        success = _messageService.SendEmailToListWithCC(mailNotif.To, mailNotif.CC, mailNotif.Subject, mailNotif.Body, false);
                    }
                    else
                    {
                        success = _messageService.SendEmailToList(mailNotif.To, mailNotif.Subject, mailNotif.Body, false);
                    }
                    var emailStatus = success ? Core.Enums.EmailStatus.Sent : Core.Enums.EmailStatus.NotSent;
                    _pbck1BLL.UpdateAllEmailStatus(pbck1Data, emailStatus, i.EX_GROUP_TYPE.Value);
                }
            }
            
        }


        private MailNotification ProcessEmailQuotaNotification(QUOTA_MONITORING data,out Pbck1Dto pbck1Data,List<string> goodTypeList)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();




            var toUserIdList = data.QUOTA_MONITORING_DETAIL.Where(x => x.ROLE_ID != (int) Core.Enums.UserRole.Controller).Select(x=> x.USER).ToList();
            var ccControllerIdList = data.QUOTA_MONITORING_DETAIL.Where(x => x.ROLE_ID == (int)Core.Enums.UserRole.Controller).Select(x => x.USER).ToList();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            if (data.EX_GROUP_TYPE == (int) Core.Enums.ExGoodsType.HasilTembakau)
            {
                pbck1Data =
                    _pbck1BLL.GetPbck1CompletedDocumentByPlantAndSubmissionDate(data.SUPPLIER_WERKS,
                        data.SUPPLIER_NPPBKC_ID,
                        DateTime.Now, data.NPPBKC_ID, goodTypeList).LastOrDefault();
            }
            else
            {
                pbck1Data =
                    _pbck1BLL.GetPbck1CompletedDocumentByExternalAndSubmissionDate(data.SUPPLIER_WERKS,
                        data.SUPPLIER_NPPBKC_ID,
                        DateTime.Now, data.NPPBKC_ID, goodTypeList).LastOrDefault();
            }
            

            rc.Subject = "PBCK-1  Quota is currently below " + data.WARNING_LEVEL + "% of Approved Qty.";
            bodyMail.Append("Dear Team,<br />");

            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");

            bodyMail.Append(BuildBodyMailForQuotaNotification(pbck1Data,  webRootUrl,  data));

            rc.To.AddRange(toUserIdList.Select(x=> x.EMAIL).ToList());
            
            rc.Body = bodyMail.ToString();
            foreach (var controller in ccControllerIdList)
            {
                rc.IsCCExist = true;
                rc.CC.Add(controller.EMAIL);
            }

            return rc;





        }

        private string BuildBodyMailForQuotaNotification(Pbck1Dto pbck1Data, string webRootUrl,  QUOTA_MONITORING dataMonitoring)
        {
            var bodyMail = new StringBuilder();
            var supplier = dataMonitoring.SUPPLIER_WERKS;
            bodyMail.Append("<table><tr><td>PBCK1 Number </td><td>: " + pbck1Data.Pbck1Number + "</td></tr>");
            bodyMail.Append("<tr><td>NPPBKC Destination </td><td>: " + dataMonitoring.NPPBKC_ID  + "</td></tr>");

            

            bodyMail.Append("<tr><td>Plant & NPPBKC Supplier </td><td>: " + supplier + " - " +
                            dataMonitoring.SUPPLIER_NPPBKC_ID + "</td></tr>");

            //bodyMail.Append("<tr><td>Total Qty Approved </td><td>: " +
            //                ConvertHelper.ConvertDecimalToStringMoneyFormat(pbck1Data.QtyApproved) + " " + pbck1Data.RequestQtyUomName + "</td></tr>");
            //bodyMail.Append("<tr><td>Total Quota Qty Used </td><td>: " +
            //                ConvertHelper.ConvertDecimalToStringMoneyFormat(quotaDetail.QtyCk5) + " " + pbck1Data.RequestQtyUomName + "</td></tr>");
            //bodyMail.Append("<tr><td>Total Quota Qty Remain </td><td>: " +
            //                ConvertHelper.ConvertDecimalToStringMoneyFormat(quotaDetail.RemainQuota) + " " + pbck1Data.RequestQtyUomName + "</td></tr>");

            string userName = "";

            var creator = _pbck1BLL.GetPbck1Creator(pbck1Data.Pbck1Id);
            userName = creator.LAST_NAME + ", " + creator.FIRST_NAME;



            bodyMail.Append("<tr><td>Creator</td><td> : " + userName + "</td></tr>");

            var poa = _pbck1BLL.GetPbck1POA(pbck1Data.Pbck1Id);

            userName = poa.LAST_NAME + ", " + poa.FIRST_NAME;
            bodyMail.Append("<tr><td>POA Approver</td><td> : " + userName + "</td></tr>");





            bodyMail.Append("<tr colspan='2'><td><i>To VIEW, Please click this <a href='" + webRootUrl + "/PBCK1/QuotaMonitoring/" + dataMonitoring.MONITORING_ID + "'><u>link</u></a> to view detailed information</i></td></tr>");



            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");

            return bodyMail.ToString();
        } 

    }
}
