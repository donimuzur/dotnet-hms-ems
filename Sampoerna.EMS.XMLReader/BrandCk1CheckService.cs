using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.DAL;
using Sampoerna.EMS.MessagingService;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.XMLReader
{
    
    
    public class BrandCk1CheckService
    {
        public ILogger logger;
        public IUnitOfWork uow;

        private ICK1Services _ck1Services;
        private IBrandRegistrationService _brandRegistrationService;
        private MessageService _messageService;

        private static int xMonth = 5;

        public BrandCk1CheckService()
        {
            logger = new NLogLogger();
            uow = new SqlUnitOfWork(logger);

            _ck1Services = new CK1Services(uow, logger);
            _brandRegistrationService = new BrandRegistrationService(uow,logger);
            _messageService = new MessageService(logger);
        }

        public void BrandCheckProcessCk1()
        {
            var allActiveBrand = _brandRegistrationService.GetAllActiveBrand("01"); //domestic is "01" , export is "02"
            var allLastCk1ItemXmonths = _ck1Services.GetLastXMonthsCk1(xMonth);

            var allCk1ItemCode = allLastCk1ItemXmonths.Select(x => x.FA_CODE + "-" + x.WERKS + "-" + x.MATERIAL_ID).ToList();

            var brandSoonInvalidTemp =
                allActiveBrand.Where(x => !allCk1ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.STICKER_CODE)))
                    .ToList();


            var brandNameSoonInvalid = brandSoonInvalidTemp.Select(x => x.BRAND_CE).Distinct().ToList();

            var brandSoonInvalid = brandSoonInvalidTemp.Where(x => brandNameSoonInvalid.Contains(x.BRAND_CE)).ToList();

            var brandSoonInvalidFinal =  brandSoonInvalid.Where(x => !allCk1ItemCode.Contains((x.FA_CODE + "-" + x.WERKS + "-" + x.STICKER_CODE)));

            var allPreviousCk1ItemXmonths = _ck1Services.GetLastXMonthsCk1(xMonth, true);

            //Dictionary<ZAIDM_EX_BRAND,List<CK1>> dictBrandCk1 = new Dictionary<ZAIDM_EX_BRAND, List<CK1>>();
            List<InvalidBrandByCk1ForEmail> invalidCk1List = new List<InvalidBrandByCk1ForEmail>();

            foreach (var brandInvalid in brandNameSoonInvalid)
            {


                var invalidCk1 = invalidCk1List.FirstOrDefault(x => x.BrandName == brandInvalid);
                

                if (invalidCk1 == null)
                {
                    var brandList = brandSoonInvalidFinal.Where(x => x.BRAND_CE == brandInvalid);
                    List<InvalidCk1Brand> invalidBrand = new List<InvalidCk1Brand>();
                    foreach (var zaidmExBrand in brandList)
                    {
                        var ck1IdList = allPreviousCk1ItemXmonths.Where(x => x.FA_CODE == zaidmExBrand.FA_CODE
                                                                             && x.WERKS == zaidmExBrand.WERKS
                                                                             && x.MATERIAL_ID == zaidmExBrand.STICKER_CODE
                                                                             && x.CK1_ID.HasValue
                                                                             )
                            .Select(x =>  x.CK1_ID.Value)
                            .ToList();
                        var lastCk1 = _ck1Services.GetCk1ByListContainIds(ck1IdList).OrderByDescending(x=> x.ORDER_DATE).FirstOrDefault();
                        invalidBrand.Add(new InvalidCk1Brand()
                        {
                            FaCode = zaidmExBrand.FA_CODE,
                            Werks = zaidmExBrand.WERKS,
                            StickerCode = zaidmExBrand.STICKER_CODE,
                            LastCk1 = lastCk1
                        });
                    }

                    invalidCk1 = new InvalidBrandByCk1ForEmail()
                    {
                        BrandName = brandInvalid,
                        BrandList = invalidBrand
                    };

                    invalidCk1List.Add(invalidCk1);
                }
            }
        }

        private void ProcessDataToEmail(List<InvalidBrandByCk1ForEmail> invalidCk1List)
        {
            //do email things here
        }
    }
}
