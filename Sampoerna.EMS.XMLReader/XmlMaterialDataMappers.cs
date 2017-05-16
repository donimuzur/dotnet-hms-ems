﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.DAL;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlMaterialDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;

        public XmlMaterialDataMapper(string fileName)
        {
            _xmlMapper = new XmlDataMapper(fileName);

        }

       
        public List<ZAIDM_EX_MATERIAL> Items
        {
            get
            {
                var xmlRoot = _xmlMapper.GetElement("IDOC");
                var xmlItems = xmlRoot.Elements("E1MARAM");
                var items = new List<ZAIDM_EX_MATERIAL>();
                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var stickerCode = xElement.Element("MATNR").Value;

                        var baseUom = _xmlMapper.GetElementValue(xElement.Element("MEINS"));
                        var materialGroup = _xmlMapper.GetElementValue(xElement.Element("MATKL"));
                        var isClientDeletion = xElement.Element("LVORM") == null
                            ? false
                            : (xElement.Element("LVORM").Value == "X" ? true : false);

                        var E1MAKTM = xElement.Element("E1MAKTM");
                        string materialDes = string.Empty;
                        if (E1MAKTM != null)
                        {
                            materialDes = _xmlMapper.GetElementValue(E1MAKTM.Element("MAKTX"));
                        }
                        var plantList = xElement.Elements("E1MARCM");

                        if (plantList != null)
                        {
                            foreach (var plant in plantList)
                            {
                                var item = new ZAIDM_EX_MATERIAL();
                                item.STICKER_CODE = stickerCode;
                                item.MATERIAL_DESC = materialDes;
                                item.BASE_UOM_ID = baseUom;
                                item.MATERIAL_GROUP = materialGroup;
                                item.CLIENT_DELETION = isClientDeletion;
                                item.WERKS = _xmlMapper.GetElementValue(plant.Element("WERKS"));
                                item.PLANT_DELETION = plant.Element("LVORM") == null
                                    ? false
                                    : (plant.Element("LVORM").Value == "X" ? true : false);


                                item.ISSUE_STORANGE_LOC = _xmlMapper.GetElementValue(plant.Element("LGPRO"));
                                item.PURCHASING_GROUP = _xmlMapper.GetElementValue(plant.Element("EKGRP"));
                                var exGoodType = plant.Element("Z1A_ZAIDM_EX_GOODTYP");
                                if (exGoodType != null)
                                {
                                    var excGoodTypeTemp = _xmlMapper.GetElementValue(exGoodType.Element("EXC_GOOD_TYP"));
                                    if (excGoodTypeTemp == null) //default menjadi hasil tembakau
                                    {
                                        item.EXC_GOOD_TYP =
                                            EnumHelper.GetDescription(Sampoerna.EMS.Core.Enums.GoodsType.HasilTembakau);
                                    }
                                    else
                                    {
                                        item.EXC_GOOD_TYP = _xmlMapper.GetElementValue(exGoodType.Element("EXC_GOOD_TYP"));    
                                    }
                                    

                                }

                                //uom
                                var uomList = xElement.Elements("E1MARMM");
                                foreach (var element in uomList)
                                {
                                    var matUom = new MATERIAL_UOM();
                                    matUom.STICKER_CODE = stickerCode;
                                    matUom.WERKS = item.WERKS;
                                    var umrez = Convert.ToDecimal(_xmlMapper.GetElementValue(element.Element("UMREZ")));
                                    var umren = Convert.ToDecimal(_xmlMapper.GetElementValue(element.Element("UMREN")));
                                    matUom.UMREN = umrez/umren;
                                    matUom.MEINH = _xmlMapper.GetElementValue(element.Element("MEINH"));

                                    item.MATERIAL_UOM.Add(matUom);
                                }
                                item.CREATED_BY = Constans.PI;

                                item.IS_FROM_SAP = true;
                                var existingMaterial = GetMaterial(item.STICKER_CODE, item.WERKS);
                                if (existingMaterial != null)
                                {
                                    var tempUoms = item.MATERIAL_UOM;
                                    item.MATERIAL_UOM = null;
                                    item.MATERIAL_UOM = new List<MATERIAL_UOM>();
                                    item.HJE = existingMaterial.HJE;
                                    item.HJE_CURR = existingMaterial.HJE_CURR;
                                    item.TARIFF_CURR = existingMaterial.TARIFF_CURR;
                                    item.TARIFF = existingMaterial.TARIFF;
                                    foreach (var uom in existingMaterial.MATERIAL_UOM)
                                    {
                                        foreach (var tempUom in tempUoms)
                                        {
                                            if (uom.MEINH == tempUom.MEINH)
                                            {
                                                tempUom.MATERIAL_UOM_ID = uom.MATERIAL_UOM_ID;

                                            }
                                            item.MATERIAL_UOM.Add(tempUom);
                                        }
                                    }

                                    item.MODIFIED_BY = Constans.PI;
                                    item.CREATED_BY = existingMaterial.CREATED_BY;
                                    item.CREATED_DATE = existingMaterial.CREATED_DATE;
                                    item.MODIFIED_DATE = DateTime.Now;
                                    items.Add(item);

                                }
                                else
                                {
                                    item.CREATED_DATE = DateTime.Now;
                                    items.Add(item);
                                }


                                //material balance
                                var materialBalanceList = plant.Elements("Z1A_OPN_CLO_BALANCE");
                                foreach (var element in materialBalanceList)
                                {
                                    var matBalance = new ZAIDM_EX_MATERIAL_BALANCE();
                                    matBalance.MATERIAL_ID = _xmlMapper.GetElementValue(element.Element("MATNR"));
                                    matBalance.WERKS = _xmlMapper.GetElementValue(element.Element("WERKS"));
                                    matBalance.PERIOD_YEAR = Convert.ToInt32(_xmlMapper.GetElementValue(element.Element("LFGJA")));
                                    matBalance.PERIOD_MONTH = Convert.ToInt32(_xmlMapper.GetElementValue(element.Element("LFMON")));
                                    
                                    matBalance.LGORT = _xmlMapper.GetElementValue(element.Element("LGORT"));

                                    var exsitingMatBalance = GetMaterialBalance(matBalance.MATERIAL_ID, matBalance.WERKS,
                                        matBalance.PERIOD_MONTH.Value, matBalance.PERIOD_YEAR.Value, matBalance.LGORT);

                                    if (exsitingMatBalance != null)
                                    {
                                        //_xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL_BALANCE>().Delete(exsitingMatBalance);
                                        matBalance = exsitingMatBalance;

                                    }
                                    
                                    matBalance.OPEN_BALANCE = Convert.ToDecimal(_xmlMapper.GetElementValue(element.Element("OPN_BAL")));
                                    matBalance.CLOSE_BALANCE = Convert.ToDecimal(_xmlMapper.GetElementValue(element.Element("CLO_BAL")));


                                    _xmlMapper.InsertOrUpdate(matBalance);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _xmlMapper.Errors.Add(ex.Message);
                    }

                }
                
                return items;
            }

        }


        public MovedFileOutput InsertToDatabase()
        {
          
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_MATERIAL>(Items);
           
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }


        public ZAIDM_EX_MATERIAL GetMaterial(string materialNumber, string plant)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL>()
                .GetByID(materialNumber,plant);
            return existingData;
        }

        public ZAIDM_EX_MATERIAL_BALANCE GetMaterialBalance(string materialNumber, string plant, int month, int year, string lgort)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL_BALANCE>()
                .Get(x => x.MATERIAL_ID == materialNumber && x.WERKS == plant 
                    && x.PERIOD_MONTH == month && x.PERIOD_YEAR == year
                    && x.LGORT == lgort).FirstOrDefault();
            return existingData;
        }


    }
}
