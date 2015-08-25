using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlCk5DataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlCk5DataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);
           
        }
        
        private Enums.CK5Type GetEMSCk5Type(string type)
        {
            switch (type)
            {
                case "01":
                    return Enums.CK5Type.Domestic;
                case "02":
                    return Enums.CK5Type.Intercompany;
                case "03":
                    return Enums.CK5Type.ImporterToPlant;
                case "04":
                    return Enums.CK5Type.PortToImporter;
                case "05":
                    return Enums.CK5Type.Export;
            }
            return Enums.CK5Type.Domestic;

        }
        private Enums.CK5XmlStatus GetEMSCk5Status(string type)
        {
            switch (type)
            {
                case "10":
                    return Enums.CK5XmlStatus.StoCreated;
                case "16":
                    return Enums.CK5XmlStatus.GICompleted;
                case "21":
                    return Enums.CK5XmlStatus.GRCompleted;


            }
            return Enums.CK5XmlStatus.StoCreated; 

        }
        public List<CK5> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_CK5_HDR");
                var items = new List<CK5>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new CK5();
                        item.SUBMISSION_NUMBER = _xmlMapper.GetElementValue(xElement.Element("CK5_NUMBER"));
                        var status = _xmlMapper.GetElementValue(xElement.Element("STATUS"));
                        var type = _xmlMapper.GetElementValue(xElement.Element("CK5_PROCS_TYP"));
                        item.CK5_TYPE = GetEMSCk5Type(type);
                        var existingCk5 = GetExistingCK5(item.SUBMISSION_NUMBER);

                        if (existingCk5 != null)
                        {
                            item = existingCk5;
                            var statusCk5 = GetEMSCk5Status(status);
                            var typeCk5 = GetEMSCk5Type(type);
                            if (typeCk5 == Enums.CK5Type.Domestic)
                            {
                                if (statusCk5 == Enums.CK5XmlStatus.StoCreated)
                                {
                                    var stoNumber = _xmlMapper.GetElementValue(xElement.Element("STO_NUMBER"));
                                    item.STO_SENDER_NUMBER = stoNumber;
                                }

                                else if (statusCk5 == Enums.CK5XmlStatus.GICompleted)
                                {
                                    var giDate = _xmlMapper.GetElementValue(xElement.Element("GI_DATE"));
                                    item.GI_DATE = _xmlMapper.GetDate(giDate);
                                   
                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.GRCompleted)
                                {
                                    var grDate = _xmlMapper.GetElementValue(xElement.Element("GR_DATE"));
                                    item.GR_DATE = _xmlMapper.GetDate(grDate);

                                }
                            }
                            items.Add(item);

                        }
                       
                    }
                    catch (Exception ex)
                    {
                        _xmlMapper.Errors.Add(ex.Message);
                        continue;
                        

                    }
                    
                   
                }
                return items;
            }
             
        }

      
        public string InsertToDatabase()
        {
           return _xmlMapper.InsertToDatabase<CK5>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public CK5 GetExistingCK5(string ck5Number)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<CK5>()
                .Get(p => p.SUBMISSION_NUMBER == ck5Number).FirstOrDefault();
            return existingData;
        }



    }
}
