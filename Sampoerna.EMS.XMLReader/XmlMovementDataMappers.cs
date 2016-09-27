using System;
using System.Collections.Generic;

using System.Linq;

using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;


namespace Sampoerna.EMS.XMLReader
{
    public class XmlMovementDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
        private string _xmlFileName;

        public XmlMovementDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
            _xmlFileName = filename.Split('\\')[filename.Split('\\').Length - 1];
        }


        public List<INVENTORY_MOVEMENT> Items
        {
           get
            {
                var xmlRoot = _xmlMapper.GetElement("InvMovementDetails");
                var xmlItems = xmlRoot.Elements("Movement");
                var items = new List<INVENTORY_MOVEMENT>();

                DeleteDataByXmlFile(_xmlFileName);

                foreach (var xElement in xmlItems)
                {
                    try
                    {
                        var item = new INVENTORY_MOVEMENT();
                        item.MAT_DOC = _xmlMapper.GetElementValue(xElement.Element("MatDoc"));
                        //var existingData = GetMovement(item.MAT_DOC);

                        //if (existingData != null)
                        //{
                        //    item = existingData;
                            
                        //}
                        

                        
                        item.MVT = _xmlMapper.GetElementValue(xElement.Element("MvT"));
                        item.MATERIAL_ID = _xmlMapper.GetElementValue(xElement.Element("Material"));
                        item.PLANT_ID = _xmlMapper.GetElementValue(xElement.Element("Plnt"));
                        var dblQty = Convert.ToDouble(_xmlMapper.GetElementValue(xElement.Element("Quantity")));
                        item.QTY = Convert.ToDecimal(dblQty);
                        item.VENDOR = _xmlMapper.GetElementValue(xElement.Element("Vendor"));
                        item.BATCH = _xmlMapper.GetElementValue(xElement.Element("Batch"));
                        item.BUN = _xmlMapper.GetElementValue(xElement.Element("BUn"));
                        item.PURCH_DOC = _xmlMapper.GetElementValue(xElement.Element("PurchDoc"));
                        item.POSTING_DATE = _xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("PstngDate")));
                        item.ENTRY_DATE = _xmlMapper.GetDateDotSeparator(_xmlMapper.GetElementValue(xElement.Element("EntryDate")));
                        item.CREATED_USER = _xmlMapper.GetElementValue(xElement.Element("Username"));
                        item.ORDR = _xmlMapper.GetElementValue(xElement.Element("Order"));
                        item.XML_FILE = _xmlFileName;
                        items.Add(item);
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
            var retVal = _xmlMapper.InsertToDatabase<INVENTORY_MOVEMENT>(Items);
            ProcessToDailyProduction();
            return retVal;

        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }
        //public INVENTORY_MOVEMENT GetPCode(string PCode)
        //{
        //    var exisitingPlant = _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_PCODE>()
        //        .GetByID(PCode);
        //    return exisitingPlant;
        //}

        public INVENTORY_MOVEMENT GetMovement(string matdoc)
        {
            var existingMvmt = _xmlMapper.uow.GetGenericRepository<INVENTORY_MOVEMENT>()
                .Get(x => x.MAT_DOC == matdoc, null, "").FirstOrDefault();

            return existingMvmt;
        }

        public void DeleteDataByXmlFile(string xmlFileName)
        {
            var repo = _xmlMapper.uow.GetGenericRepository<INVENTORY_MOVEMENT>();
            var listInvMovement = repo.Get(x => x.XML_FILE == xmlFileName).ToList();
            
            

            if (listInvMovement.Any())
            {
                foreach (var inventoryMovement in listInvMovement)
                {
                    repo.Delete(inventoryMovement);
                }

                _xmlMapper.uow.SaveChanges();
            }
                
        }


        public void ProcessToDailyProduction()
        {

            try
            {
                var mvtTypeList = new List<string>()
                {
                    EnumHelper.GetDescription(Core.Enums.MovementTypeCode.Receiving101)
                    
                };

                var periodMonth = DateTime.Today.AddDays(-1).Month;
                var periodYear = DateTime.Today.AddDays(-1).Year;
                var companyMapping = _xmlMapper.uow.GetGenericRepository<T001K>().Get().ToList();
                var companyData = _xmlMapper.uow.GetGenericRepository<T001>().Get().ToList();
                var plantData = _xmlMapper.uow.GetGenericRepository<T001W>().Get().ToList();
                var tisDesc = EnumHelper.GetDescription(Enums.GoodsType.TembakauIris);
                var dataMaterial =
                    _xmlMapper.uow.GetGenericRepository<ZAIDM_EX_BRAND>()
                        .Get(x => x.EXC_GOOD_TYP == tisDesc && x.PROD_CODE == "05")

                        .ToList();
                var listMaterial = dataMaterial.Select(x => x.FA_CODE + "-" + x.WERKS).Distinct().ToList();

                var data = _xmlMapper.uow.GetGenericRepository<INVENTORY_MOVEMENT>()
                    .Get(x => x.POSTING_DATE.Value.Month == periodMonth
                        && x.POSTING_DATE.Value.Year == periodYear
                        && listMaterial.Contains(x.MATERIAL_ID + "-" + x.PLANT_ID)
                        && mvtTypeList.Contains(x.MVT))
                    .GroupBy(x => new {x.PLANT_ID, x.MATERIAL_ID, x.POSTING_DATE})
                    .Select(x => new INVENTORY_MOVEMENT()
                    {
                        MATERIAL_ID = x.Key.MATERIAL_ID,
                        POSTING_DATE = x.Key.POSTING_DATE,
                        PLANT_ID = x.Key.PLANT_ID,
                        QTY = x.Sum(y => y.QTY)
                    })
                    .ToList();

                var dataJoined = (from dt in data
                    join mapping in companyMapping on dt.PLANT_ID equals mapping.BWKEY
                    join comp in companyData on mapping.BUKRS equals comp.BUKRS
                    join plant in plantData on dt.PLANT_ID equals plant.WERKS
                    join mat in dataMaterial on new {dt.PLANT_ID, dt.MATERIAL_ID} equals
                        new {PLANT_ID = mat.WERKS, MATERIAL_ID = mat.FA_CODE}
                    where mat.PACKED_ADJUSTED.HasValue && mat.PACKED_ADJUSTED.Value
                    select new PRODUCTION()
                    {
                        BRAND_DESC = mat.BRAND_CE,
                        CREATED_BY = "PI",
                        CREATED_DATE = DateTime.Now,
                        COMPANY_CODE = mapping.BUKRS,
                        COMPANY_NAME = comp.BUTXT,
                        FA_CODE = dt.MATERIAL_ID,
                        PLANT_NAME = plant.NAME1,
                        PRODUCTION_DATE = dt.POSTING_DATE.Value,
                        QTY_PACKED = dt.QTY*1000,
                        PACKED_ADJUSTED = 0,
                        UOM = "G",
                        WERKS = dt.PLANT_ID,
                        ZB = 0,
                        QTY = dt.QTY*1000
                    }
                    ).ToList();

                var repoProduction = _xmlMapper.uow.GetGenericRepository<PRODUCTION>();
                foreach (var production in dataJoined)
                {
                    var dataProd =
                        repoProduction.Get(
                            x =>
                                x.FA_CODE == production.FA_CODE && x.WERKS == production.WERKS &&
                                x.PRODUCTION_DATE == production.PRODUCTION_DATE).SingleOrDefault();

                    if (dataProd == null)
                    {
                        repoProduction.Insert(production);
                    }
                    else
                    {
                        dataProd.QTY_PACKED = production.QTY_PACKED;
                    }
                }

                _xmlMapper.uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _xmlMapper.Errors.Add(ex.Message);

            }

        }

    }
}
