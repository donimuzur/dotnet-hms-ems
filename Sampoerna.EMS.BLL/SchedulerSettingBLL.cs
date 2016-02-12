using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Quartz;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{

    public class SchedulerSettingBLL : ISchedulerSettingBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;

        private XElement _xmlData = null;
        private string _xmlName;


        public SchedulerSettingBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public void SetXmlFile(string fileName)
        {
            _xmlName = fileName;
            _xmlData = ReadXMLFile();
        }

        public SchedulerSetting GetMinutesCron()
        {
            //var xmlRoots = GetElement("schedule");
            //var xmlItems = xmlRoots.Elements("trigger");
            var xmlItems = GetElements("trigger");

            var data = new SchedulerSetting();

            foreach (var xElement in xmlItems)
            {
                //var cron = xElement.Element("cron");
                var cron = xElement.Descendants().First(x => x.Name.LocalName == "cron");
                if (cron != null)
                {
                    
                    var cronExpression = cron.Descendants().First(x => x.Name.LocalName == "cron-expression");//.Element("cron-expression");
                    var cronName = GetElementValue(cron.Descendants().First(x => x.Name.LocalName == "job-name")); //Element("job-name"));

                    var minutes = GetMinutesFromCronExpression(GetElementValue(cronExpression));
                    if (cronName.ToLower().Contains("daily"))
                    {
                        data.DailyMinutes = minutes;
                    }
                    else
                    {
                        data.MonthlyMinutes = minutes;
                    }
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.SchedulerSetingCronNotFound);
                }
            }



            //data.DailyMinutes = 
            
            return data;
        }

        public void Save(SchedulerSetting data)
        {
            //var xmlRoots = GetElement("schedule");
            var xmlItems = GetElements("trigger");

            foreach (var xElement in xmlItems)
            {
                var cron = xElement.Descendants().First(x => x.Name.LocalName == "cron");
                if (cron != null)
                {


                    var cronName = GetElementValue(cron.Descendants().First(x => x.Name.LocalName == "job-name"));

                    var minuteExpression = "";
                    var element = cron.Descendants().First(x => x.Name.LocalName == "cron-expression");//cron.Element("cron-expression");
                    if (cronName.ToLower().Contains("daily"))
                    {
                        minuteExpression = string.Format("0 0/{0} * 1/1 * ? *", data.DailyMinutes);
                        
                        
                    }
                    else
                    {
                        minuteExpression = string.Format("0 0/{0} * 1/1 * ? *", data.MonthlyMinutes); 
                        
                    }

                    if (element != null)
                    {
                        element.Value = minuteExpression; //SetValue(minuteExpression);
                        _xmlData.Save(_xmlName);
                    }
                    else
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.SchedulerSetingCronNotFound);
                    }
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.SchedulerSetingCronNotFound);
                }
            }
        }

        private string GetElementValue(XElement element)
        {

            if (element == null)
                return null;
            
            return element.Value;


        }

        private XElement ReadXMLFile()
        {
            
            if (_xmlName == null)
                return null;
            if (!File.Exists(_xmlName))
                return null;
            
            return XElement.Load(_xmlName);
        }

        private XElement GetElement(string elementName)
        {
            if (_xmlData == null)
                return null;
            //var test = _xmlData.Descendants().Where(x => x.Name.LocalName == "trigger");
            return _xmlData.Descendants().First(x => x.Name.LocalName == elementName);
        }

        private IEnumerable<XElement> GetElements(string elementName)
        {
            if (_xmlData == null)
                return null;
            return _xmlData.Descendants().Where(x => x.Name.LocalName == elementName);
            //return _xmlData.Elements(elementName);
        }


        private int GetMinutesFromCronExpression(string cronExpression)
        {
            var minuteSection = cronExpression.Split(' ')[1];
            var minuteString = minuteSection.Split('/')[1];


            return int.Parse(minuteString);
        }

        
    }
}
