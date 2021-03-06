﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPoaDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
        public List<String> ErroList; 
        public XmlPoaDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
            ErroList = new List<string>();
        }

        public List<POA> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1A_POA");
                var items = new List<POA>();
                foreach (var xElement in xmlItems)
                {
                    continue;
                    
                    //try
                    //{
                    //    var item = new POA();
                    //    var poaCodeXml = xElement.Element("POA_ID").Value;
                    //    var exisitingPoa = GetExPoa(poaCodeXml);
                    //    var podDateXml = _xmlMapper.GetDate(xElement.Element("MODIFIED_DATE").Value);
                    //    item.POA_ID = poaCodeXml;
                    //    item.ID_CARD = xElement.Element("POA_ID_CARD").Value;
                    //    item.PRINTED_NAME = xElement.Element("POA_PRINTED_NAME").Value;
                    //    item.POA_PHONE = xElement.Element("POA_PHONE").Value;
                    //    item.POA_ADDRESS = xElement.Element("POA_ADDRESS").Value;
                    //    item.LOGIN_AS = poaCodeXml;

                    //    item.CREATED_DATE = DateTime.Now;
                    //    item.IS_ACTIVE = true;
                    //    if (exisitingPoa != null)
                    //    {
                    //        item.POA_EMAIL = exisitingPoa.POA_EMAIL;
                    //        item.TITLE = exisitingPoa.TITLE;
                    //        if (podDateXml > exisitingPoa.CREATED_DATE)
                    //        {
                    //            items.Add(item);
                    //        }
                    //        else
                    //        {
                                
                    //            continue;

                    //        }
                    //    }
                    //    else
                    //    {
                    //        item.CREATED_BY = "PI";
                    //        item.POA_EMAIL = "";
                    //        item.TITLE = "";
                    //        items.Add(item);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    _xmlMapper.Errors.Add(ex.Message);
                    //    continue;

                    //}
                    

                }
                return items;
            }

        }


        public MovedFileOutput InsertToDatabase()
        {
            //MovedFileOutput result = new MovedFileOutput(_xmlMapper._xmlName,true);
            _xmlMapper.Errors.Add("XML POA currently not read by this build");
            //return result;
            return _xmlMapper.InsertToDatabase<POA>(Items);

        }

        public POA GetExPoa(string PoaCode)
        {
            var exisitingPoa = _xmlMapper.uow.GetGenericRepository<POA>()
                            .Get(p => p.POA_ID == PoaCode)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();
            return exisitingPoa;
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }
    }
}
