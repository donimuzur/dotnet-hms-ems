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
            return Enums.CK5Type.Manual;

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
                case "03":
                    return Enums.CK5XmlStatus.Cancel;

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
                            if (typeCk5 != Enums.CK5Type.Manual)
                            {
                                if (statusCk5 == Enums.CK5XmlStatus.StoCreated)
                                {
                                    var stoNumber = _xmlMapper.GetElementValue(xElement.Element("STO_NUMBER"));
                                    item.STO_SENDER_NUMBER = stoNumber;
                                    item.STATUS_ID = Enums.DocumentStatus.STOCreated;
                                    
                                }

                                else 
                                    if (statusCk5 == Enums.CK5XmlStatus.GICompleted)
                                {
                                    var giDate = _xmlMapper.GetElementValue(xElement.Element("GI_DATE"));
                                    item.GI_DATE = _xmlMapper.GetDate(giDate);
                                    item.STATUS_ID = Enums.DocumentStatus.GICompleted;
                                    var ck5Item = GetExistingCK5Material(existingCk5.CK5_ID);
                                    if (ck5Item.Count > 0)
                                    {
                                        var ck5ItemIndex = 0;
                                        var xmlCk5Items = xElement.Elements("Z1A_CK5_ITM");
                                        if (ck5Item.Count() >= xmlCk5Items.Count())
                                        {
                                            foreach (var ckt5Item in xmlCk5Items)
                                            {
                                                var dn_number =
                                                    _xmlMapper.GetElementValue(ckt5Item.Element("DELIVERY_NOTE"));
                                                 var brand =
                                                    _xmlMapper.GetElementValue(ckt5Item.Element("MATERIAL"));

                                                 var ck5Ems = ck5Item.Where(x=>x.BRAND == brand).FirstOrDefault();
                                                if (ck5Ems!= null)
                                                {
                                                    ck5Ems.NOTE = dn_number;
                                                    _xmlMapper.InsertOrUpdate(ck5Ems);
                                                    
                                                }
                                                ck5ItemIndex++;
                                            }
                                        }
                                        
                                    }
                                    

                                }
                                else if (statusCk5 == Enums.CK5XmlStatus.GRCompleted)
                                {
                                    var grDate = _xmlMapper.GetElementValue(xElement.Element("GR_DATE"));
                                    item.GR_DATE = _xmlMapper.GetDate(grDate);
                                    item.STATUS_ID = Enums.DocumentStatus.GRCompleted;
                                    
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
                .Get(p => p.SUBMISSION_NUMBER.Contains(ck5Number)).FirstOrDefault();
            return existingData;
        }
        public List<CK5_MATERIAL> GetExistingCK5Material(long ck5Id)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<CK5_MATERIAL>()
                .Get(p => p.CK5_ID  == ck5Id).ToList();
            return existingData;
        }
       

    }
}
