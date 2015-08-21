using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.BLL.Test
{
    public class FakeStuffs
    {

        public static IEnumerable<USER> GetGenericUserStubs()
        {
            var users = new List<USER>();
            users.Add(new USER() { USER_ID = "1", FIRST_NAME = "Jeanette", LAST_NAME = "Kist", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "2", FIRST_NAME = "Jackqueline", LAST_NAME = "Samms", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "3", FIRST_NAME = "Lovella", LAST_NAME = "Chouinard", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "4", FIRST_NAME = "Valentine", LAST_NAME = "Alm", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "5", FIRST_NAME = "Aliza", LAST_NAME = "Hennen", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "6", FIRST_NAME = "Ilona", LAST_NAME = "Escobar", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "7", FIRST_NAME = "Shasta", LAST_NAME = "Ruis", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "8", FIRST_NAME = "Celsa", LAST_NAME = "Nickel", IS_ACTIVE = 10});
            users.Add(new USER() { USER_ID = "10", FIRST_NAME = "Gregg", LAST_NAME = "Fahey", IS_ACTIVE = 10});

            return users;
        }

       
       
      
        public static IEnumerable<T001> GetCompany()
        {

            var compannyDummy = new List<T001>();
            compannyDummy.Add(new T001() {  BUKRS = "101", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:11.317") });
            compannyDummy.Add(new T001() {  BUKRS = "102", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:12.980") });
            compannyDummy.Add(new T001() {  BUKRS = "103", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:56.660") });
            compannyDummy.Add(new T001() {  BUKRS = "104", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:58.143") });
            compannyDummy.Add(new T001() {  BUKRS = "102", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-06-29 10:55:58.143") });

            return compannyDummy;
        }

        public static IEnumerable<POA> GetPOA()
        {

            var poaDummy = new List<POA>();
            poaDummy.Add(new POA { ID_CARD = "123", PRINTED_NAME = "POA 1", POA_ID = "0000000001", MANAGER_ID = "0000000002", LOGIN_AS = "0000000001" });
            poaDummy.Add(new POA { ID_CARD = "345", PRINTED_NAME = "POA 2", POA_ID = "0000000002", MANAGER_ID = "0000000003", LOGIN_AS = "0000000002" });
            
            return poaDummy;
        }

        public static IEnumerable<ZAIDM_EX_NPPBKC> GetNppbkc()
        {

            var nppbckDummy = new List<ZAIDM_EX_NPPBKC>();
            nppbckDummy.Add(new ZAIDM_EX_NPPBKC(){ BUKRS = "ID02", CITY = "SURABAYA", CITY_ALIAS = "SBY", NPPBKC_ID = "01"});
            nppbckDummy.Add(new ZAIDM_EX_NPPBKC() { BUKRS = "ID03", CITY = "SURABAYA", CITY_ALIAS = "SBY", NPPBKC_ID = "02" });
           
            return nppbckDummy;
        }
        public static IEnumerable<MONTH> GetMonths()
        {

            var monthDummy = new List<MONTH>();
            monthDummy.Add(new MONTH() { MONTH_ID = 1, MONTH_NAME_IND = "Januari", MONTH_NAME_ENG = "January"});

            return monthDummy;
        }

        public static IEnumerable<SUPPLIER_PORT> GetSupplierPorts()
        {

            var portDummy = new List<SUPPLIER_PORT>();
            portDummy.Add(new SUPPLIER_PORT(){PORT_NAME = "SBY", SUPPLIER_PORT_ID = 1});
            portDummy.Add(new SUPPLIER_PORT() { PORT_NAME = "JKT", SUPPLIER_PORT_ID = 2 });

            return portDummy;
        }

        public static IEnumerable<ZAIDM_EX_GOODTYP> GetGoodTypes()
        {

            var fakeDatas = new List<ZAIDM_EX_GOODTYP>();
            fakeDatas.Add(new ZAIDM_EX_GOODTYP(){ EXC_GOOD_TYP = "01", EXT_TYP_DESC = "GOOD TYPE 1"});
            
            return fakeDatas;
        }

        public static IEnumerable<UOM> GetUomList()
        {

            var fakeDatas = new List<UOM>();
            fakeDatas.Add(new UOM() { UOM_ID = "kg", UOM_DESC = "kilogram", IS_EMS = true});

            return fakeDatas;
        }
        public static IEnumerable<ZAIDM_EX_MARKET> GetMarketList()
        {

            var fakeDatas = new List<ZAIDM_EX_MARKET>();
            fakeDatas.Add(new ZAIDM_EX_MARKET { MARKET_ID = "01", MARKET_DESC = "market1"});

            return fakeDatas;
        }

        public static IEnumerable<ZAIDM_EX_SERIES> GetSeriesList()
        {

            var fakeDatas = new List<ZAIDM_EX_SERIES>();
            fakeDatas.Add(new ZAIDM_EX_SERIES{ SERIES_CODE = "I", SERIES_VALUE = 223});

            return fakeDatas;
        }

        public static IEnumerable<ZAIDM_EX_PRODTYP> GetProdTypList()
        {

            var fakeDatas = new List<ZAIDM_EX_PRODTYP>();
            fakeDatas.Add(new ZAIDM_EX_PRODTYP { PROD_CODE = "P1", PRODUCT_TYPE = "PXXX", PRODUCT_ALIAS = "PP"});

            return fakeDatas;
        }
        
    }
}
