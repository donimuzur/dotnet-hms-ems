using System;
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
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
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
                    
                    var stickerCode = xElement.Element("MATNR").Value;
                    var baseUom = xElement.Element("MEINS").Value;
                    var materialGroup = xElement.Element("MATKL") == null ? null : xElement.Element("MATKL").Value;
                    var isClientDeletion = xElement.Element("LVORM") == null
                        ? false
                        : (xElement.Element("LVORM").Value == "X" ? true : false);

                    var E1MAKTM = xElement.Element("E1MAKTM");
                    string materialDes = string.Empty;
                    if (E1MAKTM != null)
                    {
                        materialDes = E1MAKTM.Element("MAKTX") == null ? string.Empty : E1MAKTM.Element("MAKTX").Value;
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
                            item.WERKS = plant.Element("WERKS").Value;
                            item.PLANT_DELETION = plant.Element("LVORM") == null
                            ? false
                            : (plant.Element("LVORM").Value == "X" ? true : false);


                            item.ISSUE_STORANGE_LOC = plant.Element("LGPRO") == null ? string.Empty : (plant.Element("LGPRO").Value == "/" ? null :plant.Element("LGPRO").Value);
                            item.PURCHASING_GROUP = plant.Element("EKGRP") == null ? null : plant.Element("EKGRP").Value;
                            var exGoodType = plant.Element("Z1A_ZAIDM_EX_GOODTYP");
                            if (exGoodType != null)
                            {
                                item.EXC_GOOD_TYP = exGoodType.Element("EXC_GOOD_TYP").Value;

                            }

                            //uom
                            var uomList = xElement.Elements("E1MARMM");
                            foreach (var element in uomList)
                            {
                                var matUom = new MATERIAL_UOM();
                                matUom.STICKER_CODE = stickerCode;
                                matUom.WERKS = item.WERKS;
                                matUom.UMREZ = Convert.ToDecimal(element.Element("UMREZ").Value);
                                matUom.UMREN = Convert.ToDecimal(element.Element("UMREN").Value);
                                matUom.MEINH = element.Element("MEINH").Value;

                                item.MATERIAL_UOM.Add(matUom);
                            }

                            item.IS_FROM_SAP = true;
                            var existingMaterial = GetMaterial(item.STICKER_CODE, item.WERKS);
                            if (existingMaterial != null)
                            {
                                var tempUoms = item.MATERIAL_UOM;
                                item.MATERIAL_UOM = null;
                                item.MATERIAL_UOM = new List<MATERIAL_UOM>();
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


                                item.CREATED_DATE = existingMaterial.CREATED_DATE;
                                item.MODIFIED_DATE = DateTime.Now;
                                items.Add(item);

                            }
                            else
                            {
                                item.CREATED_DATE = DateTime.Now;
                                items.Add(item);
                            }
                        }
                        
                        
                    }
                    
                   

                   

                }
                
                
                return items;
            }

        }


        public string InsertToDatabase()
        {
          
            return _xmlMapper.InsertToDatabase<ZAIDM_EX_MATERIAL>(Items);
           
        }

        
     
        public ZAIDM_EX_MATERIAL GetMaterial(string materialNumber, string plant)
        {
            var existingData = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_MATERIAL>()
                .GetByID(materialNumber,plant);
            return existingData;
        }

        


    }
}
