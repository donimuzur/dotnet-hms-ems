using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlBrandRegistrationWriter : XmlDataWriter
    {
        private IUnitOfWork _uow;
        private ILogger _logger;
        private IZaidmExProdTypeBLL _prodTypeBll;
        private IBrandRegistrationService _brandRegistrationBLL;
        private IExcisableGoodsTypeService _excisableGoodsTypeService;
        

        public XmlBrandRegistrationWriter(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;

            _prodTypeBll = new ZaidmExProdTypeBLL(_uow,_logger);
            _brandRegistrationBLL = new BrandRegistrationService(_uow,_logger);
            _excisableGoodsTypeService = new ExcisableGoodsTypeService(_uow,_logger);
        }

        public void CreateBrandRegXml(BrandXmlDto brandReg)
        {
            using (XmlWriter writer = XmlWriter.Create(brandReg.XmlPath))
            {
                writer.WriteStartElement("ZAIDM_BRAND_01");
                writer.WriteStartElement("IDOC");
                writer.WriteAttributeString("BEGIN", "1");

                writer.WriteStartElement("EDI_DC40");
                writer.WriteAttributeString("SEGMENT", "1");
                writer.WriteElementString("TABNAM", SetNullValue(null));
                writer.WriteElementString("MANDT", SetNullValue(null));
                writer.WriteElementString("DOCNUM", SetNullValue(null));
                writer.WriteElementString("DOCREL", SetNullValue(null));
                writer.WriteElementString("STATUS", SetNullValue(null));
                writer.WriteElementString("DIRECT", SetNullValue(null));
                writer.WriteElementString("OUTMOD", SetNullValue(null));
                writer.WriteElementString("IDOCTYP", "ZAIDM_CK5_01");
                writer.WriteElementString("MESTYP", "ZAIDM_EMS_CK5");
                writer.WriteElementString("MESCOD", "EMS");
                writer.WriteElementString("SNDPOR", SetNullValue(null));
                writer.WriteElementString("SNDPRT", SetNullValue(null));
                writer.WriteElementString("SNDPRN", SetNullValue(null));
                writer.WriteElementString("CREDAT", GetDateFormat(DateTime.Now));
                writer.WriteElementString("CRETIM", GetTimeFormat(DateTime.Now));
                writer.WriteElementString("SERIAL", SetNullValue(null));


                writer.WriteEndElement();//end element EDI_DC40

                writer.WriteStartElement("Z1A_BRAND");
                writer.WriteAttributeString("SEGMENT", "1");
                writer.WriteElementString("MANDT", "100"); // always 100 ?
                writer.WriteElementString("STICKER_ID", SetNullValue(null)); //need confirmation if need to be sent or not
                writer.WriteElementString("STICKER_CODE", brandReg.STICKER_CODE);
                writer.WriteElementString("PLANT",brandReg.WERKS);
                writer.WriteElementString("FA_CODE",brandReg.FA_CODE);
                writer.WriteElementString("PER_CODE",SetNullToSlash(brandReg.PER_CODE));
                writer.WriteElementString("PER_DESC",SetNullToSlash(brandReg.PER_CODE_DESC));
                writer.WriteElementString("BRAND_CE",SetNullToSlash(brandReg.BRAND_CE));
                writer.WriteElementString("SKEP_NO",SetNullToSlash(brandReg.SKEP_NO));
                writer.WriteElementString("SKEP_DATE",SetNullToSlash(GetDateFormat(brandReg.SKEP_DATE)));
                writer.WriteElementString("PROD_CODE",SetNullToSlash(brandReg.PROD_CODE));

                var prodType = _prodTypeBll.GetByCode(brandReg.PROD_CODE);
                writer.WriteElementString("PRODUCT_TYPE",SetNullToSlash(prodType.PRODUCT_TYPE));
                writer.WriteElementString("PRODUCT_ALIAS",SetNullToSlash(prodType.PRODUCT_ALIAS));

                var series = _brandRegistrationBLL.GetSeries(brandReg.SERIES_CODE);
                writer.WriteElementString("SERIES_CODE",SetNullToSlash(brandReg.SERIES_CODE));
                writer.WriteElementString("SERIES_VALUE",SetNullToSlash(series.SERIES_VALUE));

                writer.WriteElementString("CONTENT",SetNullToSlash(brandReg.BRAND_CONTENT));

                var market = _brandRegistrationBLL.GetMarket(brandReg.MARKET_ID);
                writer.WriteElementString("MARKET",SetNullToSlash(brandReg.MARKET_ID));
                writer.WriteElementString("MARKET_DESC",SetNullToSlash(market.MARKET_DESC));

                writer.WriteElementString("COUNTRY",SetNullToSlash(brandReg.COUNTRY));
                writer.WriteElementString("HJE_IDR",SetNullToSlash(brandReg.HJE_IDR));
                writer.WriteElementString("HJE_CURR",SetNullToSlash(brandReg.HJE_CURR));
                writer.WriteElementString("TARIFF",SetNullToSlash(brandReg.TARIFF));
                writer.WriteElementString("TARIFF_CURR",SetNullToSlash(brandReg.TARIF_CURR));

                writer.WriteElementString("COLOUR",SetNullToSlash(brandReg.COLOUR));

                var goodType = _excisableGoodsTypeService.GetById(brandReg.EXC_GOOD_TYP);
                writer.WriteElementString("EXC_GOOD_TYP",SetNullToSlash(brandReg.EXC_GOOD_TYP));
                writer.WriteElementString("EXC_TYP_DESC",SetNullToSlash(goodType.EXT_TYP_DESC));

                writer.WriteElementString("START_DATE",SetNullToSlash(GetDateFormat(brandReg.START_DATE)));
                writer.WriteElementString("END_DATE",SetNullToSlash(GetDateFormat(brandReg.END_DATE)));

                writer.WriteElementString("EXC_TYP_DESC",SetBoolToX(brandReg.STATUS));

                var modifiedDate = brandReg.MODIFIED_DATE.HasValue ? brandReg.MODIFIED_DATE : brandReg.CREATED_DATE;
                var modifiedBy = brandReg.MODIFIED_BY ?? brandReg.CREATED_BY;
                writer.WriteElementString("MODIFIED_DATE", SetNullToSlash(GetDateFormat(modifiedDate)));
                writer.WriteElementString("MODIFIED_BY", SetNullToSlash(modifiedBy));

                writer.WriteEndElement();//end element Z1A_BRAND
                
                writer.WriteEndElement();//end element IDOC
                writer.WriteEndElement();//end element ZAIDM_BRAND_01

                writer.WriteEndDocument();
            }
        }
    }
}
