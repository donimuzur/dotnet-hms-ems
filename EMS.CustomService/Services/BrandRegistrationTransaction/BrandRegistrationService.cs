using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Sampoerna.EMS.Core;
using System.Data.Entity;
using Sampoerna.EMS.CustomService.Core;


namespace Sampoerna.EMS.CustomService.Services.BrandRegistrationTransaction
{
    public class BrandRegistrationService : GenericService
    {
        private DbContextTransaction transaction;
        private SystemReferenceService refService;
        private EMSDataModel context;
        private List<String> fileExtList;
        private Dictionary<int, string> govstatusList;




        public BrandRegistrationService() : base()
        {
            this.refService = new SystemReferenceService();
            context = new EMSDataModel();

            fileExtList = new List<string>();
            fileExtList.Add(".txt");
            fileExtList.Add(".csv");
            fileExtList.Add(".pdf");
            fileExtList.Add(".xls");
            fileExtList.Add(".xlsx");
            fileExtList.Add(".doc");
            fileExtList.Add(".docx");

            govstatusList = new Dictionary<int, string>();
            govstatusList.Add(1, "Approved");
            govstatusList.Add(0, "Rejected");

        }

        #region Get Data

        public SYS_REFFERENCES GetReffById(long Id)
        {
            var Reff = context.SYS_REFFERENCES.Where(w => w.REFF_ID == Id).FirstOrDefault();
            return Reff;
        }

        public IEnumerable<BRAND_REGISTRATION_REQ> GetAll()
        {
            try
            {
                return this.uow.BrandRegistrationReqRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ChangeRequestService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<BRAND_REGISTRATION_REQ> GetBrandRegistrationRequest()
        {
            try
            {
                var result = this.uow.BrandRegistrationReqRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<BRAND_REGISTRATION_REQ_DETAIL> GetBrandRegistrationReqDetail()
        {
            try
            {
                var result = this.uow.BrandRegistrationReqDetailRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<BRAND_REGISTRATION_REQ> GetBrandRegistrationById(Int64 Id)
        {
            try
            {
                var result = context.BRAND_REGISTRATION_REQ.Where(w => w.REGISTRATION_ID.Equals(Id));
                return result;
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Interview Request. See Inner Exception property to see details", ex);
            }
        }

        public List<POA> GetPOAApproverList(long Id)
        {
            try
            {
                var ListPOA = new List<POA>();
                var RealListPOA = new List<POA>();
                var BrandRegData = GetBrandRegistrationById(Id).FirstOrDefault();
                if (BrandRegData != null)
                {
                    var NPPBKCId = BrandRegData.NPPBKC_ID;
                    if (BrandRegData.APPROVAL_STATUS.REFF_KEYS != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval) && BrandRegData.LASTAPPROVED_BY == null)
                    {
                        var ListPOA_Nppbkc = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != BrandRegData.CREATED_BY).Select(s => s.POA_ID).ToList();
                        var OriexcisePOA = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true).Select(s => s.POA_ID).ToList();
                        var excisePOA = new List<string>();
                        if (ListPOA_Nppbkc.Count() == 0)
                        {
                            excisePOA = OriexcisePOA.Where(w => ListPOA_Nppbkc.Contains(w)).ToList();
                        }
                        var ListPOAExcise_Raw = context.POA.Where(w => w.POA_ID != BrandRegData.CREATED_BY && w.IS_ACTIVE == true);
                        if (excisePOA.Count() == 0 || excisePOA == null)
                        {
                            ListPOAExcise_Raw = ListPOAExcise_Raw.Where(w => OriexcisePOA.Contains(w.POA_ID));
                        }
                        else
                        {
                            ListPOAExcise_Raw = ListPOAExcise_Raw.Where(w => excisePOA.Contains(w.POA_ID));
                        }
                        foreach (var poaresult in ListPOAExcise_Raw)
                        {
                            ListPOA.Add(new POA
                            {
                                POA_ID = poaresult.POA_ID,
                                POA_EMAIL = poaresult.POA_EMAIL,
                                PRINTED_NAME = poaresult.PRINTED_NAME
                            });
                            var poadelegate = GetPOADelegationOfUser(poaresult.POA_ID);
                            if (poadelegate != null)
                            {
                                ListPOA.Add(new POA
                                {
                                    POA_ID = poadelegate.POA_TO,
                                    POA_EMAIL = poadelegate.USER1.EMAIL,
                                    PRINTED_NAME = poadelegate.USER1.FIRST_NAME
                                });
                            }
                        }
                    }
                    else
                    {
                        var lastApprover = BrandRegData;
                        if (lastApprover != null)
                        {
                            var CurrentPOA = context.POA.Where(w => w.POA_ID.Equals(lastApprover.LASTAPPROVED_BY)).FirstOrDefault();
                            ListPOA.Add(new POA
                            {
                                POA_ID = lastApprover.LASTAPPROVED_BY,
                                POA_EMAIL = CurrentPOA.POA_EMAIL,
                                PRINTED_NAME = CurrentPOA.PRINTED_NAME
                            });
                            var poadelegate = GetPOADelegationOfUser(lastApprover.LASTAPPROVED_BY);
                            if (poadelegate != null)
                            {
                                var CurrentPOADelegate = context.POA.Where(w => w.POA_ID.Equals(poadelegate.POA_TO)).FirstOrDefault();
                                ListPOA.Add(new POA
                                {
                                    POA_ID = poadelegate.POA_TO,
                                    POA_EMAIL = CurrentPOA.POA_EMAIL,
                                    PRINTED_NAME = CurrentPOA.PRINTED_NAME
                                });
                            }
                        }
                    }
                    if (ListPOA.Count() > 0)
                    {
                        foreach (var realPoa in ListPOA)
                        {
                            if (!RealListPOA.Where(w => w.POA_ID == realPoa.POA_ID).Any())
                            {
                                RealListPOA.Add(realPoa);
                            }
                        }
                    }
                }
                return RealListPOA;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on BRand Registration Request Get POA Approver. See Inner Exception property to see details", ex);
            }
        }

        //public List<POA> GetPOAApproverList_Old(string NPPBKCId, string CreatedBy, string LastApprovedBy = "")
        //{
        //    var ListPOA = new List<POA>();
        //    var POA_Excisers = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true).Select(s => s.POA_ID).ToList();

        //    if (LastApprovedBy != "" || LastApprovedBy != null)
        //    {
        //        var ListPOA_Raw = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && POA_Excisers.Contains(w.POA_ID) && w.POA.IS_ACTIVE == true && w.POA_ID != CreatedBy);
        //        if (ListPOA_Raw.Any())
        //        {
        //            foreach (var POA_Approver in ListPOA_Raw)
        //            {
        //                ListPOA.Add(new POA
        //                {
        //                    POA_ID = POA_Approver.POA_ID,
        //                    POA_EMAIL = POA_Approver.POA.POA_EMAIL,
        //                    PRINTED_NAME = POA_Approver.POA.PRINTED_NAME
        //                });
        //            }
        //        }
        //        else
        //        {
        //            var ListPOAExicser_Raw = context.POA.Where(w => POA_Excisers.Contains(w.POA_ID) && w.IS_ACTIVE == true && w.POA_ID != CreatedBy).ToList();
        //            if (ListPOAExicser_Raw.Any())
        //            {
        //                foreach (var POA_Approver in ListPOAExicser_Raw)
        //                {
        //                    ListPOA.Add(new POA
        //                    {
        //                        POA_ID = POA_Approver.POA_ID,
        //                        POA_EMAIL = POA_Approver.POA_EMAIL,
        //                        PRINTED_NAME = POA_Approver.PRINTED_NAME
        //                    });
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        var POA_Approver = context.POA.Where(w => w.POA_ID.Equals(LastApprovedBy)).FirstOrDefault();
        //        ListPOA.Add(new POA
        //        {
        //            POA_ID = POA_Approver.POA_ID,
        //            POA_EMAIL = POA_Approver.POA_EMAIL,
        //            PRINTED_NAME = POA_Approver.PRINTED_NAME
        //        });
        //    }

        //    return ListPOA;

        //}

        public bool CheckPOANPPBKC(string NPPBKCId, string CurrentUserId)
        {
            var result = false;
            var poa_map = context.POA_MAP.Where(w => w.POA_ID.Equals(CurrentUserId) && w.NPPBKC_ID == NPPBKCId);
            if (poa_map.Any())
            {
                result = true;
            }

            return result;
        }


        public List<POA> GetPOAApproverList2(long CRId)
        {
            try
            {
                var ListPOA = new List<POA>();
                var CRequest = GetBrandRegistrationById(CRId).FirstOrDefault();
                if (CRequest != null)
                {
                    var NPPBKCId = CRequest.NPPBKC_ID;
                    if (CRequest.LASTAPPROVED_STATUS != refService.GetRefByKey("WAITING_GOVERNMENT_APPROVAL").REFF_ID)
                    {
                        var ListPOA_Raw = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != CRequest.CREATED_BY);
                        if (ListPOA_Raw.Any())
                        {
                            foreach (var poaresult in ListPOA_Raw)
                            {
                                ListPOA.Add(new POA
                                {
                                    POA_ID = poaresult.POA_ID,
                                    POA_EMAIL = poaresult.POA.POA_EMAIL,
                                    PRINTED_NAME = poaresult.POA.PRINTED_NAME
                                });
                                var poadelegate = GetPOADelegationOfUser(poaresult.POA_ID);
                                if (poadelegate != null)
                                {
                                    ListPOA.Add(new POA
                                    {
                                        POA_ID = poadelegate.POA_TO,
                                        POA_EMAIL = poadelegate.USER1.EMAIL,
                                        PRINTED_NAME = poadelegate.USER1.FIRST_NAME
                                    });
                                }
                            }
                        }
                        else if (NPPBKCId != "" && NPPBKCId != null)
                        {
                            var ListPOAExcise_Raw = context.POA.Where(w => w.POA_ID != CRequest.CREATED_BY && w.IS_ACTIVE == true && w.TITLE == "Excise Executive");
                            foreach (var poaresult in ListPOAExcise_Raw)
                            {
                                ListPOA.Add(new POA
                                {
                                    POA_ID = poaresult.POA_ID,
                                    POA_EMAIL = poaresult.POA_EMAIL,
                                    PRINTED_NAME = poaresult.PRINTED_NAME
                                });
                                var poadelegate = GetPOADelegationOfUser(poaresult.POA_ID);
                                if (poadelegate != null)
                                {
                                    ListPOA.Add(new POA
                                    {
                                        POA_ID = poadelegate.POA_TO,
                                        POA_EMAIL = poadelegate.USER1.EMAIL,
                                        PRINTED_NAME = poadelegate.USER1.FIRST_NAME
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        //var lastApprover = CRequest;
                        //if (lastApprover != null)
                        //{
                        //    ListPOA.Add(new POA
                        //    {
                        //        POA_ID = lastApprover.LASTAPPROVED_BY,
                        //        POA_EMAIL = lastApprover.USER1.EMAIL,
                        //        PRINTED_NAME = lastApprover.USER1.FIRST_NAME
                        //    });
                        //    var poadelegate = GetPOADelegationOfUser(lastApprover.LASTAPPROVED_BY);
                        //    if (poadelegate != null)
                        //    {
                        //        ListPOA.Add(new POA
                        //        {
                        //            POA_ID = poadelegate.POA_TO,
                        //            POA_EMAIL = poadelegate.USER1.EMAIL,
                        //            PRINTED_NAME = poadelegate.USER1.FIRST_NAME
                        //        });
                        //    }
                        //}
                    }
                    if (ListPOA.Count() > 0)
                    {
                        ListPOA = ListPOA.Distinct().ToList();
                    }
                }
                return ListPOA;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Change Request Get POA Approver. See Inner Exception property to see details", ex);
            }
        }


        public BRAND_REGISTRATION_REQ FindBrandRegistrationReq(long RegID)
        {
            try
            {
                var result = this.uow.BrandRegistrationReqRepository.Find(RegID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public BRAND_REGISTRATION_REQ_DETAIL FindBrandRegistrationReqDetail(long PD_Detail_ID)
        {
            try
            {
                var result = this.uow.BrandRegistrationReqDetailRepository.Find(PD_Detail_ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<BRAND_REGISTRATION_REQ_DETAIL> GetBrandDetailByRegistrationID(long RegistrationID)
        {
            try
            {
                return this.uow.BrandRegistrationReqDetailRepository.GetManyQueryable(a => a.REGISTRATION_ID == RegistrationID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        //public IQueryable<PRODUCT_DEVELOPMENT_DETAIL> GetAllProductByAction()
        //{
        //    //var plant = this.uow.PlantRepository.GetManyQueryable(x => data.Contains(x.WERKS)).Where(x => x.NPPBKC_ID != null);
        //    try
        //    {
        //        var temp = this.uow.ProductDevelopmentRepository.GetManyQueryable(a => a.NEXT_ACTION == 1 || a.NEXT_ACTION == 2);
        //        var result = this.uow.ProductDevelopmentDetailRepository.GetManyQueryable(x;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
        //    }
        //}

        public BRAND_REGISTRATION_REQ GetLastRecordBrandReq()
        {
            try
            {
                return this.uow.BrandRegistrationReqRepository.GetAll().OrderByDescending(m => m.REGISTRATION_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_PLANT FindPlantByNppbkcID(string Nppbkc_ID)
        {
            try
            {
                var result = this.uow.PlantRepository.GetFirst(pl => pl.NPPBKC_ID == Nppbkc_ID || pl.NPPBKC_IMPORT_ID == Nppbkc_ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }

        public MASTER_PLANT FindMainPlantByNppbkcID(string Nppbkc_ID)
        {
            try
            {
                var result = this.uow.PlantRepository.GetFirst(pl => (pl.NPPBKC_ID == Nppbkc_ID || pl.NPPBKC_IMPORT_ID == Nppbkc_ID) && pl.IS_MAIN_PLANT == true);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }

        public IQueryable<ZAIDM_EX_BRAND> FindBrandByCompany(string PlantID)
        {
            try
            {
                var result = context.ZAIDM_EX_BRAND.Where(w => w.WERKS == PlantID && w.STATUS == true).OrderByDescending(o => o.SKEP_DATE);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }

        public IQueryable<ZAIDM_EX_BRAND> FindBrandByCompanies(List<MASTER_PLANT> list_of_plants)
        {
            try
            {
                var plants = list_of_plants.Select(s => s.WERKS).ToList();
                var result = context.ZAIDM_EX_BRAND.Where(w => plants.Contains(w.WERKS) && w.STATUS == true).Distinct().OrderByDescending(o => o.SKEP_DATE);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }


        public ZAIDM_EX_BRAND FindBrandByFACode(string FA_Code)
        {
            try
            {
                var result = context.ZAIDM_EX_BRAND.Where(w => w.FA_CODE == FA_Code).OrderByDescending(o => o.SKEP_DATE).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }

        public ZAIDM_EX_BRAND FindBrandMaster(string FA_Code_Old, string FA_Code_New, int RegistrationType)
        {
            try
            {
                var result = new ZAIDM_EX_BRAND();
                switch(RegistrationType)
                {
                    case 1:
                        result = context.ZAIDM_EX_BRAND.Where(w => w.FA_CODE == FA_Code_New).OrderByDescending(o => o.SKEP_DATE).FirstOrDefault();
                        break;

                    case 2:
                        result = context.ZAIDM_EX_BRAND.Where(w => w.FA_CODE == FA_Code_Old).OrderByDescending(o => o.SKEP_DATE).FirstOrDefault();
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }

        public BRAND_REGISTRATION_REQ_DETAIL FindBrandTransaction(string FA_Code_Old, string FA_Code_New, int RegistrationType)
        {
            try
            {
                long CancelReffId = refService.GetRefByKey("CANCELED").REFF_ID;
                var result = new BRAND_REGISTRATION_REQ_DETAIL();
                switch (RegistrationType)
                {
                    case 1:
                        result = context.BRAND_REGISTRATION_REQ_DETAIL.Where(w => w.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW == FA_Code_New && w.BRAND_REGISTRATION_REQ.LASTAPPROVED_STATUS != CancelReffId).OrderBy(o => o.BRAND_REGISTRATION_REQ.CREATED_DATE).FirstOrDefault();
                        break;

                    case 2:
                        result = context.BRAND_REGISTRATION_REQ_DETAIL.Where(w => w.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW == FA_Code_Old && w.BRAND_REGISTRATION_REQ.LASTAPPROVED_STATUS != CancelReffId).OrderBy(o => o.BRAND_REGISTRATION_REQ.CREATED_DATE).FirstOrDefault();
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }


        public T001 FindCompanyByNppbkcID (string Bukrs)
        {
            try
            {
                var result = this.uow.CompanyRepository.GetFirst(cm => cm.BUKRS == Bukrs);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC FindNppbkcDetail (string Nppbkc_ID)
        {
            try
            {
                var result = this.uow.NppbkcRepository.GetFirst(np => np.NPPBKC_ID == Nppbkc_ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }
        }

        public List<string> GetNPPBKCByUser(string UserId)
        {
            try
            {
                var ListNPPBKC = new List<string>();
                var ListNPPBKC_Raw = context.POA_MAP.Where(w => w.POA.IS_ACTIVE == true && w.POA_ID.Equals(UserId));
                if (ListNPPBKC_Raw.Any())
                {
                    foreach (var dataResult in ListNPPBKC_Raw)
                    {
                        ListNPPBKC.Add(dataResult.NPPBKC_ID);
                    }
                }
                if (ListNPPBKC.Count() > 0)
                {
                    ListNPPBKC = ListNPPBKC.Distinct().ToList();
                }
                return ListNPPBKC;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Change Request Get NPPBKC from POA Map. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC GetNppbkc(object id)
        {
            try
            {
                var nppbkc = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == id.ToString()).FirstOrDefault();
                var plants = context.T001W.Where(w => w.NPPBKC_ID == nppbkc.NPPBKC_ID).Select(s => s.WERKS).ToList();
                var plantsMap = context.T001K.Where(w => plants.Contains(w.BWKEY)).FirstOrDefault();
                var companyMap = context.T001.Where(w => w.BUKRS == plantsMap.BUKRS).FirstOrDefault();
                nppbkc.COMPANY = companyMap;

                return nppbkc;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }

        //public IQueryable<vwProductDevDetail> GetProductDevDetail(int RegistrationType, string nppbkc)
        //{
        //    try
        //    {
        //        var werks = context.T001W.Where(w => w.NPPBKC_ID.Equals(nppbkc)).Select(s => s.WERKS).ToList();
        //        var result = context.vwProductDevDetail.Where(w => w.NEXT_ACTION == RegistrationType && werks.Contains(w.WERKS));
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
        //    }
        //}

        public List<vwProductDevDetail> GetProductDevDetail(int RegistrationType, string nppbkc, long RegId)
        {
            try
            {
                var context = new EMSDataModel();
                //var werks = context.T001W.Where(w => w.NPPBKC_ID.Equals(nppbkc)).Select(s => s.WERKS).ToList();
                var result = context.vwProductDevDetail.Where(w => w.NEXT_ACTION == RegistrationType).ToList();
                var registrationDet = context.BRAND_REGISTRATION_REQ_DETAIL.Where(w => w.REGISTRATION_ID == RegId);
                var registrationDetList = MapToVWProductDevDetail(registrationDet);
                foreach (var rece in registrationDetList)
                {
                    result.Add(rece);
                }
                result = result.Distinct().ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
            }
        }

        public List<vwProductDevDetail> MapToVWProductDevDetail(IQueryable<BRAND_REGISTRATION_REQ_DETAIL> registrationDetail)
        {
            var list = new List<vwProductDevDetail>();
            foreach (var s in registrationDetail)
            {
                var proddevdet = new vwProductDevDetail
                {
                    PD_NO = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.PD_NO,
                    NEXT_ACTION = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.NEXT_ACTION,
                    PD_DETAIL_ID = s.PD_DETAIL_ID,
                    FA_CODE_OLD = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
                    FA_CODE_OLD_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
                    FA_CODE_NEW = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
                    FA_CODE_NEW_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
                    HL_CODE = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
                    MARKET_ID = s.PRODUCT_DEVELOPMENT_DETAIL.MARKET_ID,
                    MARKET_DESC = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
                    WERKS = s.PRODUCT_DEVELOPMENT_DETAIL.WERKS,
                    PRODUCTION_CENTER = s.PRODUCT_DEVELOPMENT_DETAIL.T001W.NAME1,
                    IS_IMPORT = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT,
                    PD_ID = s.PRODUCT_DEVELOPMENT_DETAIL.PD_ID,
                    REQUEST_NO = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
                    BUKRS = s.PRODUCT_DEVELOPMENT_DETAIL.BUKRS,
                    COMPANY_NAME = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
                    LASTAPPROVED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_BY,
                    LASTAPPROVED_DATE = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_DATE,
                    LASTAPPROVED_STATUS = s.PRODUCT_DEVELOPMENT_DETAIL.STATUS_APPROVAL,
                    LASTMODIFIED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTMODIFIED_BY
                };
                list.Add(proddevdet);
            }
            //var result = receiveDet.Select(s => new vwProductDevDetail
            //{
            //    PD_NO = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.PD_NO,
            //    NEXT_ACTION = s.PRODUCT_DEVELOPMENT_DETAIL.PRODUCT_DEVELOPMENT.NEXT_ACTION,
            //    PD_DETAIL_ID = s.PD_DETAIL_ID,
            //    FA_CODE_OLD = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD,
            //    FA_CODE_OLD_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_OLD_DESCR,
            //    FA_CODE_NEW = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW,
            //    FA_CODE_NEW_DESCR = s.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW_DESCR,
            //    HL_CODE = s.PRODUCT_DEVELOPMENT_DETAIL.HL_CODE,
            //    MARKET_ID = s.PRODUCT_DEVELOPMENT_DETAIL.MARKET_ID,
            //    MARKET_DESC = s.PRODUCT_DEVELOPMENT_DETAIL.ZAIDM_EX_MARKET.MARKET_DESC,
            //    WERKS = s.PRODUCT_DEVELOPMENT_DETAIL.WERKS,
            //    PRODUCTION_CENTER = s.PRODUCT_DEVELOPMENT_DETAIL.T001W.NAME1,
            //    IS_IMPORT = s.PRODUCT_DEVELOPMENT_DETAIL.IS_IMPORT,
            //    PD_ID = s.PRODUCT_DEVELOPMENT_DETAIL.PD_ID,
            //    REQUEST_NO = s.PRODUCT_DEVELOPMENT_DETAIL.REQUEST_NO,
            //    BUKRS = s.PRODUCT_DEVELOPMENT_DETAIL.BUKRS,
            //    COMPANY_NAME = s.PRODUCT_DEVELOPMENT_DETAIL.T001.BUTXT,
            //    LASTAPPROVED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_BY,
            //    LASTAPPROVED_DATE = s.PRODUCT_DEVELOPMENT_DETAIL.LASTAPPROVED_DATE,
            //    LASTAPPROVED_STATUS = s.PRODUCT_DEVELOPMENT_DETAIL.STATUS_APPROVAL,
            //    LASTMODIFIED_BY = s.PRODUCT_DEVELOPMENT_DETAIL.LASTMODIFIED_BY
            //}).ToList();
            return list;
        }



        public POA_DELEGATION GetPOADelegationOfUser(string UserId)
        {
            try
            {
                var now = DateTime.Now.Date;
                return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_FROM.Equals(UserId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Get Delegate. See Inner Exception property to see details", ex);
            }
        }

        public POA GetPOAData(string UserId)
        {
            try
            {
                var now = DateTime.Now.Date;
                return context.POA.Where(w => w.POA_ID.Equals(UserId)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Get POA. See Inner Exception property to see details", ex);
            }

        }

        public long GetMailId(string template_name)
        {
            try
            {
                return context.CONTENTEMAIL.Where(w => w.EMAILNAME.Equals(template_name)).Select(s => s.CONTENTEMAILID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Get Mail Id. See Inner Exception property to see details", ex);
            }

        }

        public string GetProductAlias(long Id)
        {
            try
            {
                string result = "";

                var list_of_productcode = context.BRAND_REGISTRATION_REQ_DETAIL.Where(w => w.REGISTRATION_ID == Id).Select(s => s.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS).Distinct().ToList();

                foreach(var product_alias in list_of_productcode)
                {
                    if (result != "")
                    {
                        result += "," + product_alias;
                    }
                    else
                    {
                        result += product_alias;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Get Product Type. See Inner Exception property to see details", ex);
            }

        }





        #endregion

        #region Create
        public BRAND_REGISTRATION_REQ CreateBrand(BRAND_REGISTRATION_REQ data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.BRAND_REGISTRATION_REQ.Add(data);
                        context.SaveChanges();
                        data.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(data.LASTAPPROVED_STATUS);

                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data, changes, formType, actionType, role, user);

                        transaction.Commit();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create Brand Registration Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public void CreateBrandDetail(BRAND_REGISTRATION_REQ_DETAIL data)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.BRAND_REGISTRATION_REQ_DETAIL.Add(data);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create Brand Detail Service. See Inner Exception property to see details", ex);
                    }

                }
            }
        }

        public string SetNewFormNumber(string partial_number)
        {
            var lastFormNumber = context.BRAND_REGISTRATION_REQ.Where(x => x.REGISTRATION_NO.Contains(partial_number)).OrderByDescending(o => o.REGISTRATION_NO).Select(s => s.REGISTRATION_NO).FirstOrDefault();
            if (lastFormNumber == "" || lastFormNumber == null)
            {
                lastFormNumber = "0";
            }
            else
            {
                lastFormNumber = lastFormNumber.Substring(0, 10);
            }
            var numb = Convert.ToInt32(lastFormNumber) + 1;
            var finalNumb = numb.ToString().PadLeft(10, '0');
            return finalNumb + "/" + partial_number;
        }


        #endregion

        #region Upload File

        public List<string> GetFileExtList()
        {
            return fileExtList;
        }

        public void InsertFileUpload(long CRId, string Path, string CreatedBy, long DocId, bool IsGovDoc, string FileName)
        {
            try
            {
                var now = DateTime.Now;
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var UploadFile = new FILE_UPLOAD();
                UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormList.BrandReq);
                UploadFile.FORM_ID = CRId.ToString();
                UploadFile.PATH_URL = Path;
                UploadFile.UPLOAD_DATE = now;
                UploadFile.CREATED_BY = CreatedBy;
                UploadFile.CREATED_DATE = now;
                UploadFile.LASTMODIFIED_BY = CreatedBy;
                UploadFile.LASTMODIFIED_DATE = now;
                if (DocId != 0)
                {
                    UploadFile.DOCUMENT_ID = DocId;
                }
                UploadFile.IS_GOVERNMENT_DOC = IsGovDoc;
                UploadFile.IS_ACTIVE = true;
                UploadFile.FILE_NAME = FileName;
                context.FILE_UPLOAD.Add(UploadFile);
                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Brand Registration Upload File. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<FILE_UPLOAD> GetFileUploadByCRId(long CRId)
        {
            try
            {
                var strID = CRId.ToString();
                var intFormType = Convert.ToInt32(Enums.FormList.BrandReq);
                return context.FILE_UPLOAD.Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration File Upload List. See Inner Exception property to see details", ex);
            }
        }

        public void DeleteFileUpload(long fileid, string updatedby)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var now = DateTime.Now;
                var fileupload = new FILE_UPLOAD();
                var Where = context.FILE_UPLOAD.Where(w => w.FILE_ID.Equals(fileid));
                if (Where.Count() > 0)
                {
                    fileupload = Where.FirstOrDefault();
                    fileupload.LASTMODIFIED_BY = updatedby;
                    fileupload.LASTMODIFIED_DATE = now;
                    fileupload.IS_ACTIVE = false;
                    context.SaveChanges();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Brand Registration File. See Inner Exception property to see details", ex);
            }
        }


        public string GetSupportingDocName(long Id)
        {
            var docname = "";
            context = new EMSDataModel();
            var doc = context.MASTER_SUPPORTING_DOCUMENT.Where(w => w.DOCUMENT_ID == Id);
            if (doc.Any())
            {
                docname = doc.Select(s => s.SUPPORTING_DOCUMENT_NAME).FirstOrDefault();
            }
            return docname;
        }




        public void CreateFileUpload(long PD_ID, string Path, string CreatedBy, long DocId, bool IsGovDoc, string FileName)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var UploadFile = new FILE_UPLOAD();

                        UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormType.BrandRegistrationReq);
                        UploadFile.FORM_ID = PD_ID.ToString();
                        UploadFile.PATH_URL = Path;
                        UploadFile.UPLOAD_DATE = DateTime.Now;
                        UploadFile.CREATED_BY = CreatedBy;
                        UploadFile.CREATED_DATE = DateTime.Now;
                        UploadFile.LASTMODIFIED_BY = CreatedBy;
                        UploadFile.LASTMODIFIED_DATE = DateTime.Now;
                        UploadFile.FILE_NAME = FileName;

                        if (DocId != 0)
                        {
                            UploadFile.DOCUMENT_ID = DocId;
                        }
                        UploadFile.IS_GOVERNMENT_DOC = IsGovDoc;
                        UploadFile.IS_ACTIVE = true;

                        context.FILE_UPLOAD.Add(UploadFile);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Brand Registration Service Upload File. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        //public IQueryable<FILE_UPLOAD> GetFileUploadByRegID(long REg_ID)
        //{
        //    try
        //    {
        //        var strID = REg_ID.ToString();
        //        var intFormType = Convert.ToInt32(Enums.FormType.BrandRegistrationReq);
        //        return this.uow.FileUploadRepository.GetAll().Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
        //    }
        //}

        public IQueryable<FILE_UPLOAD> GetFileUploadByRegID(long RegId)
        {
            try
            {
                var strID = RegId.ToString();
                var intFormType = Convert.ToInt32(Enums.FormList.BrandReq);
                return context.FILE_UPLOAD.Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true && w.IS_GOVERNMENT_DOC == false);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration File Upload List. See Inner Exception property to see details", ex);
            }
        }

        public List<MASTER_PLANT> GetPlantByNPPBKC(string NPPBKCId)
        {
            try
            {
                var nppbkc = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == NPPBKCId.ToString()).FirstOrDefault();
                var plants = context.T001W.Where(w => w.NPPBKC_ID == nppbkc.NPPBKC_ID).ToList();
                return plants;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }


        }

        //public void DeleteFileUpload(long fileid, string updatedby)
        //{
        //    using (var context = new EMSDataModel())
        //    {
        //        using (var transaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var now = DateTime.Now;
        //                var fileupload = new FILE_UPLOAD();
        //                var Where = context.FILE_UPLOAD.Where(w => w.FILE_ID.Equals(fileid));
        //                if (Where.Count() > 0)
        //                {
        //                    fileupload = Where.FirstOrDefault();
        //                    fileupload.LASTMODIFIED_BY = updatedby;
        //                    fileupload.LASTMODIFIED_DATE = now;
        //                    fileupload.IS_ACTIVE = false;
        //                    context.SaveChanges();
        //                }
        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                throw this.HandleException("Exception occured on Brand Registration Service. See Inner Exception property to see details", ex);
        //            }
        //        }
        //    }

        //}
        #endregion

        #region Update

        public Boolean Edit(BRAND_REGISTRATION_REQ updateData, int ActionType = 0, Int32 UserRole = 0)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
                var now = DateTime.Now;
                var OldBRequest = new BRAND_REGISTRATION_REQ();
                var BRequest = new BRAND_REGISTRATION_REQ();
                var Where = context.BRAND_REGISTRATION_REQ.Where(w => w.REGISTRATION_ID.Equals(updateData.REGISTRATION_ID));
                if (Where.Count() > 0)
                {
                    BRequest = Where.FirstOrDefault();
                    OldBRequest = SetOldValueToTempModel(BRequest);
                    BRequest.SUBMISSION_DATE = updateData.SUBMISSION_DATE;
                    BRequest.REGISTRATION_TYPE = updateData.REGISTRATION_TYPE;
                    BRequest.EFFECTIVE_DATE = updateData.EFFECTIVE_DATE;
                    BRequest.NPPBKC_ID = updateData.NPPBKC_ID;
                    BRequest.LASTMODIFIED_DATE = now;
                    BRequest.LASTAPPROVED_STATUS = updateData.LASTAPPROVED_STATUS;
                    changes = GetAllChanges(OldBRequest, BRequest);
                    context.SaveChanges();
                }
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    LogsActivity(context, BRequest, changes, (int)Enums.MenuList.BrandRegistrationReq, ActionType, UserRole, updateData.LASTMODIFIED_BY, "");
                }

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
            }
        }

        public Boolean EditSKEP(BRAND_REGISTRATION_REQ updateData, int ActionType = 0, Int32 UserRole = 0)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
                var now = DateTime.Now;
                var OldBRequest = new BRAND_REGISTRATION_REQ();
                var BRequest = new BRAND_REGISTRATION_REQ();
                var Where = context.BRAND_REGISTRATION_REQ.Where(w => w.REGISTRATION_ID.Equals(updateData.REGISTRATION_ID));
                if (Where.Count() > 0)
                {
                    BRequest = Where.FirstOrDefault();
                    OldBRequest = SetOldValueToTempModel(BRequest);
                    BRequest.DECREE_DATE = updateData.DECREE_DATE;
                    BRequest.DECREE_NO = updateData.DECREE_NO;
                    BRequest.DECREE_STARTDATE = updateData.DECREE_STARTDATE;
                    BRequest.DECREE_STATUS = updateData.DECREE_STATUS;
                    BRequest.LASTMODIFIED_DATE = now;
                    BRequest.LASTMODIFIED_BY = updateData.LASTMODIFIED_BY;
                    //BRequest.LASTAPPROVED_BY = updateData.LASTMODIFIED_BY;
                    //BRequest.LASTAPPROVED_DATE = now;
                    BRequest.LASTAPPROVED_STATUS = updateData.LASTAPPROVED_STATUS;
                    changes = GetAllChanges(OldBRequest, BRequest);
                    context.SaveChanges();
                }
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    LogsActivity(context, BRequest, changes, (int)Enums.MenuList.BrandRegistration, ActionType, UserRole, updateData.LASTMODIFIED_BY, "");
                }

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
            }
        }



        //public bool Edit(BRAND_REGISTRATION_REQ data, int formType, int actionType, int role, string user)
        //{
        //    using (var context = new EMSDataModel())
        //    {
        //        using (var transaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var old = context.BRAND_REGISTRATION_REQ.Find(data.REGISTRATION_ID); // br detail not yet accomodated
        //                Dictionary<string, string[]> changes = GetAllChanges(old, data);
        //                context.Entry(old).CurrentValues.SetValues(data);
        //                context.SaveChanges();
        //                LogsActivity(context, data, changes, formType, actionType, role, user);
        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
        //            }
        //        }
        //    }
        //    return true;
        //}

        public void DeleteBrandRegistrationDetail(long RegID)
        {
            try
            {
                var context = new EMSDataModel();
                var deleteList = context.BRAND_REGISTRATION_REQ_DETAIL.Where(w => w.REGISTRATION_ID.Equals(RegID));
                if (deleteList.Count() > 0)
                {
                    foreach (var delete in deleteList)
                    {
                        context.BRAND_REGISTRATION_REQ_DETAIL.Remove(delete);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Penetapan SKEP Service. See Inner Exception property to see details", ex);
            }
        }

        #endregion

        #region Changes Log

        private BRAND_REGISTRATION_REQ SetOldValueToTempModel(BRAND_REGISTRATION_REQ BRData)
        {
            var OldBRequest = new BRAND_REGISTRATION_REQ()
            {
                REGISTRATION_ID = BRData.REGISTRATION_ID,
                REGISTRATION_NO = BRData.REGISTRATION_NO,
                SUBMISSION_DATE = BRData.SUBMISSION_DATE,
                REGISTRATION_TYPE = BRData.REGISTRATION_TYPE,
                NPPBKC_ID = BRData.NPPBKC_ID,
                EFFECTIVE_DATE = BRData.EFFECTIVE_DATE,
                CREATED_BY = BRData.CREATED_BY,
                CREATED_DATE = BRData.CREATED_DATE,
                LASTMODIFIED_BY = BRData.LASTMODIFIED_BY,
                LASTMODIFIED_DATE = BRData.LASTMODIFIED_DATE,
                LASTAPPROVED_BY = BRData.LASTAPPROVED_BY,
                LASTAPPROVED_DATE = BRData.LASTAPPROVED_DATE,
                LASTAPPROVED_STATUS = BRData.LASTAPPROVED_STATUS,
                DECREE_STATUS = BRData.DECREE_STATUS,
                DECREE_DATE = BRData.DECREE_DATE,
                DECREE_STARTDATE = BRData.DECREE_STARTDATE
            };
            return OldBRequest;
        }


        private Dictionary<string, string[]> GetAllChanges(BRAND_REGISTRATION_REQ old, BRAND_REGISTRATION_REQ updated)
        {
            try
            {
                if (old == null)
                {
                    old = new BRAND_REGISTRATION_REQ();
                }
                var changes = new Dictionary<string, string[]>();
                if (old.REGISTRATION_NO != updated.REGISTRATION_NO)
                {
                    var oldvalue = old.REGISTRATION_NO == null ? "N/A" : old.REGISTRATION_NO.ToString();
                    var newvalue = updated.REGISTRATION_NO == null ? "N/A" : updated.REGISTRATION_NO.ToString();
                    changes.Add("REGISTRATION_NO", new string[] { oldvalue, newvalue });
                }
                if (old.SUBMISSION_DATE != updated.SUBMISSION_DATE)
                {
                    var oldvalue = old.SUBMISSION_DATE == DateTime.MinValue ? "N/A" : old.SUBMISSION_DATE.ToString();
                    var newvalue = updated.SUBMISSION_DATE == DateTime.MinValue ? "N/A" : updated.SUBMISSION_DATE.ToString();
                    changes.Add("SUBMISSION_DATE", new string[] { oldvalue, newvalue });
                }
                if (old.REGISTRATION_TYPE != updated.REGISTRATION_TYPE)
                {
                    var oldvalue = old.REGISTRATION_TYPE == 0 ? "N/A" : old.REGISTRATION_TYPE == 1 ? "New Brand Registration" : "Update HJE";
                    var newvalue = updated.REGISTRATION_TYPE == 0 ? "N/A" : updated.REGISTRATION_TYPE == 1 ? "New Brand Registration" : "Update HJE";
                    changes.Add("REGISTRATION_TYPE", new string[] { oldvalue, newvalue });
                }
                if (old.NPPBKC_ID != updated.NPPBKC_ID)
                {
                    var oldvalue = old.NPPBKC_ID == null ? "N/A" : old.NPPBKC_ID.ToString();
                    var newvalue = updated.NPPBKC_ID == null ? "N/A" : updated.NPPBKC_ID.ToString();
                    changes.Add("NPPBKC_ID", new string[] { oldvalue, newvalue });
                }
                if (old.EFFECTIVE_DATE != updated.EFFECTIVE_DATE)
                {
                    var oldvalue = old.EFFECTIVE_DATE == DateTime.MinValue ? "N/A" : old.EFFECTIVE_DATE.ToString();
                    var newvalue = updated.EFFECTIVE_DATE == DateTime.MinValue ? "N/A" : updated.EFFECTIVE_DATE.ToString();
                    changes.Add("EFFECTIVE_DATE", new string[] { oldvalue, newvalue });
                }

                //if (old.CREATED_BY != updated.CREATED_BY)
                //{
                //    var oldvalue = old.CREATED_BY == null ? "N/A" : old.CREATED_BY.ToString();
                //    var newvalue = updated.CREATED_BY == null ? "N/A" : updated.CREATED_BY.ToString();
                //    changes.Add("CREATED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.CREATED_DATE != updated.CREATED_DATE)
                //{
                //    var oldvalue = old.CREATED_DATE == null ? "N/A" : old.CREATED_DATE.ToString();
                //    var newvalue = updated.CREATED_DATE == null ? "N/A" : updated.CREATED_DATE.ToString();
                //    changes.Add("CREATED_DATE", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTMODIFIED_BY != updated.LASTMODIFIED_BY)
                //{
                //    var oldvalue = old.LASTMODIFIED_BY == null ? "N/A" : old.LASTMODIFIED_BY.ToString();
                //    var newvalue = updated.LASTMODIFIED_BY == null ? "N/A" : updated.LASTMODIFIED_BY.ToString();
                //    changes.Add("LASTMODIFIED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTMODIFIED_DATE != updated.LASTMODIFIED_DATE)
                //{
                //    var oldvalue = old.LASTMODIFIED_DATE == null ? "N/A" : old.LASTMODIFIED_DATE.ToString();
                //    var newvalue = updated.LASTMODIFIED_DATE == null ? "N/A" : updated.LASTMODIFIED_DATE.ToString();
                //    changes.Add("LASTMODIFIED_DATE", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTAPPROVED_BY != updated.LASTAPPROVED_BY)
                //{
                //    var oldvalue = old.LASTAPPROVED_BY == null ? "N/A" : old.LASTAPPROVED_BY.ToString();
                //    var newvalue = updated.LASTAPPROVED_BY == null ? "N/A" : updated.LASTAPPROVED_BY.ToString();
                //    changes.Add("LASTAPPROVED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTAPPROVED_DATE != updated.LASTAPPROVED_DATE)
                //{
                //    var oldvalue = old.LASTAPPROVED_DATE == null ? "N/A" : old.LASTAPPROVED_DATE.ToString();
                //    var newvalue = updated.LASTAPPROVED_DATE == null ? "N/A" : updated.LASTAPPROVED_DATE.ToString();
                //    changes.Add("LASTAPPROVED_DATE", new string[] { oldvalue, newvalue });
                //}
                if (old.LASTAPPROVED_STATUS != updated.LASTAPPROVED_STATUS)
                {
                    var oldvalue = old.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(old.LASTAPPROVED_STATUS).REFF_VALUE;
                    var newvalue = updated.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(updated.LASTAPPROVED_STATUS).REFF_VALUE;
                    changes.Add("LASTAPPROVED_STATUS", new string[] { oldvalue, newvalue });
                }
                if (old.LASTAPPROVED_STATUS == refService.GetRefByKey("WAITING_GOVERNMENT_APPROVAL").REFF_ID)
                {
                    var oldvalue1 = "N/A";
                    var newvalue1 = updated.DECREE_STATUS == null ? "N/A" : updated.DECREE_STATUS == true ? "Approved" : "Rejected";
                    changes.Add("DECREE_STATUS", new string[] { oldvalue1, newvalue1 });

                    //if (old.DECREE_STATUS != updated.DECREE_STATUS)
                    //{
                    //    var oldvalue = old.DECREE_STATUS == null ? "N/A" : old.DECREE_STATUS.ToString();
                    //    var newvalue = updated.DECREE_STATUS == null ? "N/A" : updated.DECREE_STATUS.ToString();
                    //    changes.Add("DECREE_STATUS", new string[] { oldvalue, newvalue });
                    //}
                    if (old.DECREE_NO != updated.DECREE_NO)
                    {
                        var oldvalue = old.DECREE_NO == null ? "N/A" : old.DECREE_NO.ToString();
                        var newvalue = updated.DECREE_NO == null ? "N/A" : updated.DECREE_NO.ToString();
                        changes.Add("DECREE_NO", new string[] { oldvalue, newvalue });
                    }
                    if (old.DECREE_DATE != updated.DECREE_DATE)
                    {
                        var oldvalue = old.DECREE_DATE == null ? "N/A" : old.DECREE_DATE.ToString();
                        var newvalue = updated.DECREE_DATE == null ? "N/A" : updated.DECREE_DATE.ToString();
                        changes.Add("DECREE_DATE", new string[] { oldvalue, newvalue });
                    }
                    if (old.DECREE_STARTDATE != updated.DECREE_STARTDATE)
                    {
                        var oldvalue = old.DECREE_STARTDATE == null ? "N/A" : old.DECREE_STARTDATE.ToString();
                        var newvalue = updated.DECREE_STARTDATE == null ? "N/A" : updated.DECREE_STARTDATE.ToString();
                        changes.Add("DECREE_STARTDATE", new string[] { oldvalue, newvalue });
                    }

                }



                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Get Change For Log. See Inner Exception property to see details", ex);
            }
        }


        /// <summary> Part of Changes Log Step which Mark All Available Changes </summary>        
        /// <param name="old"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        //private Dictionary<string, string[]> GetAllChanges(BRAND_REGISTRATION_REQ old, BRAND_REGISTRATION_REQ updated)
        //{
        //    try
        //    {
        //        var changes = new Dictionary<string, string[]>();
        //        var columns = new string[]
        //             {
        //             //"PROD_CODE",
        //             //"PRODUCT_TYPE",
        //             //"PRODUCT_ALIAS",
        //             //"CK4CEDITABLE",
        //             //"IS_DELETED",
        //             //"APPROVALSTATUS"
        //             };
        //        var oldProps = new Dictionary<string, object>();
        //        var props = new Dictionary<string, object>();

        //        foreach (var prop in updated.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //        {
        //            props.Add(prop.Name, prop.GetValue(updated, null));
        //            if (old != null)
        //                oldProps.Add(prop.Name, prop.GetValue(old, null));
        //            else
        //                oldProps.Add(prop.Name, null);
        //        }
        //        foreach (var item in props)
        //        {
        //            var oldValue = (oldProps[item.Key] != null) ? oldProps[item.Key].ToString() : "N/A";
        //            var newValue = (item.Value != null) ? item.ToString() : "N/A";
        //            if (!columns.Contains(item.Key))
        //                continue;

        //            if (item.Key == "APPROVALSTATUS")
        //            {
        //                if (item.Value != null)
        //                    newValue = ((SYS_REFFERENCES)item.Value).REFF_VALUE;

        //                if (oldProps[item.Key] != null)
        //                    oldValue = ((SYS_REFFERENCES)oldProps[item.Key]).REFF_VALUE;
        //                if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
        //                    changes.Add(item.Key, new string[] { oldValue, newValue });
        //                continue;
        //            }
        //            if (item.Value != null)
        //            {
        //                if (item.Value is decimal)
        //                    newValue = ((decimal)item.Value).ToString("C2");

        //                else
        //                    newValue = item.Value.ToString();
        //            }

        //            if (oldProps[item.Key] != null)
        //            {
        //                if (oldProps[item.Key] is decimal)
        //                    oldValue = ((decimal)oldProps[item.Key]).ToString("C2");

        //                else
        //                    oldValue = oldProps[item.Key].ToString();
        //            }
        //            if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
        //                changes.Add(item.Key, new string[] { oldValue, newValue });
        //        }
        //        return changes;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
        //    }
        //}

        /// <summary> Add record to Changes Log and Workflow History </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="changes"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="actor"></param>
        /// <param name="comment"></param>
        private void LogsActivity(EMSDataModel context, BRAND_REGISTRATION_REQ data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        data.REGISTRATION_ID.ToString(),
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(data.REGISTRATION_ID),
                    actionType,
                    actor,
                    DateTime.Now,
                    role,
                    comment
                    );
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }

        public void LogsPrintActivity(int formType, string actor)
        {
            try
            {
                EMSDataModel context = new EMSDataModel();
                refService.AddChangeLog(context,
                    formType,
                    "PRINT",
                    "",
                    "",
                    "",
                    actor,
                    DateTime.Now
                    );

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Brand Registration Request Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Change Status
        public Boolean UpdateStatus(long BRId, Int64 LastApprovedStatus = 0, string ModifiedBy = "", int ActionType = 0, Int32 UserRole = 0, string Comment = "")
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var now = DateTime.Now;
                var BRequest = new BRAND_REGISTRATION_REQ();
                var OldBRequest = new BRAND_REGISTRATION_REQ();
                var Where = context.BRAND_REGISTRATION_REQ.Where(w => w.REGISTRATION_ID.Equals(BRId));
                if (Where.Count() > 0)
                {
                    BRequest = Where.FirstOrDefault();
                    OldBRequest = SetOldValueToTempModel(BRequest);
                    BRequest.LASTMODIFIED_BY = ModifiedBy;
                    BRequest.LASTMODIFIED_DATE = now;
                    BRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                    //if (ActionType != (int)Enums.ActionType.Revise && ActionType != (int)Enums.ActionType.Cancel)
                    if (ActionType == (int)Enums.ActionType.Approve || ActionType == (int)Enums.ActionType.Revise)
                    {
                        BRequest.LASTAPPROVED_BY = ModifiedBy;
                        BRequest.LASTAPPROVED_DATE = now;
                    }
                }
                context.SaveChanges();
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    Dictionary<string, string[]> changes = GetAllChanges(OldBRequest, BRequest);
                    LogsActivity(context, BRequest, changes, (int)Enums.MenuList.BrandRegistrationReq, ActionType, UserRole, ModifiedBy, Comment);
                }
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License Change Request Update. See Inner Exception property to see details", ex);
            }
        }

        public Boolean InsertToBrand(long BRId, string nppbkc)
        {
            try
            {
                context = new EMSDataModel();
                transaction = context.Database.BeginTransaction();
                var plants = GetPlantByNPPBKC(nppbkc).ToList();
                var items = context.BRAND_REGISTRATION_REQ_DETAIL.Where(w => w.REGISTRATION_ID == BRId).ToList();

                foreach (var plant in plants)
                {
                    foreach (var item in items)
                    {
                        var data = new ZAIDM_EX_BRAND();
                        data.WERKS = plant.WERKS;
                        data.FA_CODE = item.PRODUCT_DEVELOPMENT_DETAIL.FA_CODE_NEW;
                        data.STICKER_CODE = "0";
                        data.SERIES_CODE = "0";
                        data.BRAND_CE = item.BRAND_CE;
                        data.SKEP_NO = item.BRAND_REGISTRATION_REQ.DECREE_NO;
                        data.SKEP_DATE = item.BRAND_REGISTRATION_REQ.DECREE_DATE;
                        data.PROD_CODE = item.PROD_CODE;
                        data.BRAND_CONTENT = item.BRAND_CONTENT;
                        data.MARKET_ID = item.MARKET_ID;
                        data.COUNTRY = "ID";
                        data.HJE_IDR = item.HJE;
                        data.HJE_CURR = "IDR";
                        data.TARIFF = item.TARIFF;
                        data.TARIF_CURR = "IDR";
                        data.EXC_GOOD_TYP = item.PROD_CODE;
                        data.STATUS = true;
                        data.CREATED_DATE = DateTime.Now;
                        data.CREATED_BY = item.BRAND_REGISTRATION_REQ.CREATED_BY;
                        data.MODIFIED_DATE = DateTime.Now;
                        data.MODIFIED_BY = item.BRAND_REGISTRATION_REQ.CREATED_BY;
                        data.BAHAN_KEMASAN = item.MATERIAL_PACKAGE;

                        context.ZAIDM_EX_BRAND.Add(data);
                        context.SaveChanges();
                    }
                }

                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Brand Registration Completed. See Inner Exception property to see details", ex);
            }
        }


        #endregion

        #region Helper

        public Dictionary<int, string> GetGovStatusList()
        {
            return govstatusList;
        }

        #endregion

        #region ViewBrand

        public IQueryable<vwBrandRegistration> GetvwBrandRegistrationAll()
        {
            try
            {
                context = new EMSDataModel();
                return context.vwBrandRegistration;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }
        #endregion
    }
}
