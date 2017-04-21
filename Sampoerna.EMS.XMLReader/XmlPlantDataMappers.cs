﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPlantDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlPlantDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }

        
        public List<T001W> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("Z1AXX_T001W");
                var items = new List<T001W>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new T001W();
                        item.WERKS = _xmlMapper.GetElementValue(xElement.Element("WERKS"));
                        item.NAME1 = _xmlMapper.GetElementValue(xElement.Element("NAME1"));
                        item.ORT01 = _xmlMapper.GetElementValue(xElement.Element("ORT01"));
                        item.ADDRESS = _xmlMapper.GetElementValue(xElement.Element("STRAS")) + " " + item.ORT01;
                        item.CREATED_BY = Constans.PI;
                        var exisitingPlant = GetPlant(item.WERKS);
                        if (exisitingPlant != null)
                        {
                            item.NPPBKC_ID = exisitingPlant.NPPBKC_ID;
                            item.PHONE = exisitingPlant.PHONE;
                            item.SKEPTIS = exisitingPlant.SKEPTIS;
                            item.NPPBKC_IMPORT_ID = exisitingPlant.NPPBKC_IMPORT_ID;
                            item.ORT01 = exisitingPlant.ORT01;
                            item.IS_MAIN_PLANT = exisitingPlant.IS_MAIN_PLANT;
                            item.CREATED_BY = exisitingPlant.CREATED_BY;
                            item.CREATED_DATE = exisitingPlant.CREATED_DATE;
                            item.ADDRESS = exisitingPlant.ADDRESS;
                            item.MODIFIED_DATE = DateTime.Now;
                            item.MODIFIED_BY = Constans.PI;
                            item.ADDRESS_IMPORT = exisitingPlant.ADDRESS_IMPORT;
                            items.Add(item);

                        }
                        else
                        {
                            item.CREATED_DATE = DateTime.Now;
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


        public MovedFileOutput InsertToDatabase()
        {
          return _xmlMapper.InsertToDatabase<T001W>(Items);
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }

        public T001W GetPlant(string PlantId)
        {
            var exisitingPlant = _xmlMapper.uow.GetGenericRepository<T001W>()
                .GetByID(PlantId);
            return exisitingPlant;
        }





    }
}
