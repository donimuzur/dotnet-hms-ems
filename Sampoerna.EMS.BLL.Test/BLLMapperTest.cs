using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Website;

namespace Sampoerna.EMS.BLL.Test
{
    [TestClass]
    public class BLLMapperTest
    {
       
        [TestInitialize]
        public void SetUp()
        {
            
            BLLMapper.Initialize();
           
            

        }

        [TestCleanup]
        public void TestCleanup()
        {
            
        }

        [TestMethod]
        public void MappingPlantDto()
        {
            var t001w = new T001W();
            t001w.WERKS = "ID02";
            t001w.NAME1 = "Pabrik 02";

            var result = Mapper.Map<PlantDto>(t001w);
            Assert.IsNull(result.NPPBKC_ID);
            Assert.IsNotNull(result.WERKS);
            Assert.AreEqual(t001w.NAME1, result.NAME1);

        }

        [TestMethod]
        public void MappingPOA_MAPDto()
        {
            var source = new POA_MAP();
            source.WERKS = "ID02";
            source.POA_ID = "POA 01";
            

            var result = Mapper.Map<POA_MAPDto>(source);
            Assert.IsNull(result.NPPBKC_ID);
            Assert.IsNotNull(result.WERKS);
            Assert.AreEqual(result.POA_ID, source.POA_ID);

        }

        [TestMethod]
        public void MappingPlant()
        {
            var t001w = new T001W();
            t001w.WERKS = "ID02";
            t001w.NAME1 = "Pabrik 02";

            var result = Mapper.Map<Plant>(t001w);
        
            Assert.AreEqual(t001w.WERKS + "-" + t001w.NAME1, result.DROPDOWNTEXTFIELD);

        }
        
    }
}
