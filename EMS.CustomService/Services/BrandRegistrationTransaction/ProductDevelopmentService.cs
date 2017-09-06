using Sampoerna.EMS.Core;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sampoerna.EMS.CustomService.Services.BrandRegistrationTransaction
{
    public class ProductDevelopmentService : GenericService
    {
        private SystemReferenceService refService;
        private EMSDataModel context;
        private Dictionary<string, string> weekType;

        public ProductDevelopmentService() : base()
        {
            this.refService = new SystemReferenceService();
            context = new EMSDataModel();

            weekType = new Dictionary<string, string>();
            weekType.Add("1", "1"); weekType.Add("2", "2"); weekType.Add("3", "3"); weekType.Add("4", "4"); weekType.Add("5", "5");
            weekType.Add("6", "6"); weekType.Add("7", "7"); weekType.Add("8", "8"); weekType.Add("9", "9"); weekType.Add("10", "10");
            weekType.Add("11", "11"); weekType.Add("12", "12"); weekType.Add("13", "13"); weekType.Add("14", "14"); weekType.Add("15", "15");
            weekType.Add("16", "16"); weekType.Add("17", "17"); weekType.Add("18", "18"); weekType.Add("19", "19"); weekType.Add("20", "20");
            weekType.Add("21", "21"); weekType.Add("22", "22"); weekType.Add("23", "23"); weekType.Add("24", "24"); weekType.Add("25", "25");
            weekType.Add("26", "26"); weekType.Add("27", "27"); weekType.Add("28", "28"); weekType.Add("29", "29"); weekType.Add("30", "30");
            weekType.Add("31", "31"); weekType.Add("32", "32"); weekType.Add("33", "33"); weekType.Add("34", "34"); weekType.Add("35", "35");
            weekType.Add("36", "36"); weekType.Add("37", "37"); weekType.Add("38", "38"); weekType.Add("39", "39"); weekType.Add("40", "40");
            weekType.Add("41", "41"); weekType.Add("42", "42"); weekType.Add("43", "43"); weekType.Add("44", "44"); weekType.Add("45", "45");
            weekType.Add("46", "46"); weekType.Add("47", "47"); weekType.Add("48", "48"); weekType.Add("49", "49"); weekType.Add("50", "50");
            weekType.Add("51", "51"); weekType.Add("52", "52"); weekType.Add("53", "53"); weekType.Add("54", "54");             
        }

        public Dictionary<string, string> GetWeek()
        {
            return weekType;
        }
        #region Get Data & Find Detail

        public IQueryable<USER> GetAllUser()
        {
            try
            {
                var result = this.uow.UserRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Configuration Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<PRODUCT_DEVELOPMENT> GetProductDevelopment()
        {
            try
            {
                var result = this.uow.ProductDevelopmentRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<PRODUCT_DEVELOPMENT_DETAIL> GetProductDevDetail()
        {
            try
            {
                var result = this.uow.ProductDevelopmentDetailRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }       

        public IQueryable<T001> GetCompanies()
        {
            try
            {
                var result = this.uow.CompanyRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<COUNTRY> GetCountry()
        {
            try
            {
                var result = this.uow.CountryRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_PLANT> GetPlant()
        {
            try
            {
                var result = this.uow.PlantRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_MARKET> GetMarket()
        {
            try
            {
                var result = this.uow.MarketRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_BRAND> GetBrand()
        {
            try
            {
                var result = this.uow.BrandRepository.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_MATERIAL> GetAllMaterial()
        {
            try
            {
                var result = this.uow.MaterialRepository.GetAll().Where(m => m.EXC_GOOD_TYP == "01");
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }


        public IQueryable<ZAIDM_EX_MATERIAL> GetAllMaterialUsed()
        {
            try
            {
                var result = this.uow.MaterialRepository.GetAll().Where(m => m.EXC_GOOD_TYP == "01" && m.USED_PRODUCT_DEVELOPMENT == false);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
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

        public ZAIDM_EX_MATERIAL FindItemDescription(string code)
        {
            try
            {
                var result = this.uow.MaterialRepository.GetFirst(mt => mt.STICKER_CODE == code.ToUpper());
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public ZAIDM_EX_MARKET FindMarketDescription(string code)
        {
            try
            {
                var result = this.uow.MarketRepository.GetFirst(mt => mt.MARKET_ID == code.ToUpper());
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_PLANT FindPlantDescription(string code)
        {
            try
            {
                var result = this.uow.PlantRepository.GetFirst(mt => mt.WERKS == code.ToUpper());
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_PLANT FindPlantDescriptionByName(string namePLant)
        {
            try
            {
                var result = this.uow.PlantRepository.GetFirst(mt => mt.NAME1 == namePLant);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }


        public T001 FindCompanyDescription(string code)
        {
            try
            {
                var result = this.uow.CompanyRepository.GetFirst(mt => mt.BUKRS == code.ToUpper());
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public PRODUCT_DEVELOPMENT FindProductDevelopment(long ID)
        {
            try
            {
                var result = this.uow.ProductDevelopmentRepository.Find(ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public PRODUCT_DEVELOPMENT_DETAIL FindProductDevDetail(long ID)
        {
            try
            {
                var result = this.uow.ProductDevelopmentDetailRepository.Find(ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<PRODUCT_DEVELOPMENT_DETAIL> GetProductDetailByProductID(long ProductID)
        {
            try
            {
                return this.uow.ProductDevelopmentDetailRepository.GetManyQueryable(a => a.PD_ID == ProductID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public USER FindUserDetail(string USERID)
        {
            try
            {
                return this.uow.UserRepository.Find(USERID);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> FindSupportDetail(long FormID, string Bukrs)
        {
            try
            {
                //var result = this.uow.SupportDocRepository.GetAll().Where(a => a.FORM_ID == FormID && a.BUKRS == Bukrs);                
                var result = this.uow.SupportDocRepository.GetMany(sd => sd.FORM_ID == FormID && sd.BUKRS == Bukrs && sd.IS_ACTIVE == true);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_PLANT> FindPlantNonImport(string Bukrs)
        {
            try
            {
                var data = this.uow.CompanyPlantMappingRepository.GetManyQueryable(a => a.BUKRS == Bukrs).Select(x => x.BWKEY);
                var plant = this.uow.PlantRepository.GetManyQueryable(x => data.Contains(x.WERKS)).Where(x => x.NPPBKC_ID != null);
                return plant;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MASTER_PLANT> FindPlantImport(string Bukrs)
        {
            try
            {
                var data = this.uow.CompanyPlantMappingRepository.GetManyQueryable(a => a.BUKRS == Bukrs).Select(x => x.BWKEY);
                var plant = this.uow.PlantRepository.GetManyQueryable(x => data.Contains(x.WERKS)).Where(x => x.NPPBKC_IMPORT_ID != null);
                return plant;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_MATERIAL> GetAllMaterialByPlant(string werks)
        {
            try
            {
                var result = this.uow.MaterialRepository.GetAll().Where(m => m.EXC_GOOD_TYP == "01" && m.WERKS == werks);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<ZAIDM_EX_MATERIAL> GetAllMaterialUsedByPlant(string werks)
        {
            try
            {
                var result = this.uow.MaterialRepository.GetAll().Where(m => m.EXC_GOOD_TYP == "01" && (m.USED_PRODUCT_DEVELOPMENT == false || m.USED_PRODUCT_DEVELOPMENT == null) && m.WERKS == werks);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public PRODUCT_DEVELOPMENT GetLastRecordPD()
        {
            try
            {
                return this.uow.ProductDevelopmentRepository.GetAll().OrderByDescending(m => m.PD_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public PRODUCT_DEVELOPMENT_DETAIL GetLastRecordPDDetail()
        {
            try
            {
                return this.uow.ProductDevelopmentDetailRepository.GetAll().OrderByDescending(m => m.PD_DETAIL_ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC FindNppbkcByPlant(string plantName)
        {
            try
            {
                var temp = this.uow.PlantRepository.GetFirst(pl => pl.NAME1 == plantName);
                var result = this.uow.NppbkcRepository.GetFirst(np => np.NPPBKC_ID == temp.NPPBKC_ID);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC FindNppbkcDetail(string nppbkcId)
        {
            try
            {
                var result = this.uow.NppbkcRepository.GetFirst(np => np.NPPBKC_ID == nppbkcId);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public bool IsAdminExciser(string userId)
        {

            var result = this.uow.PoaExciserRepository.GetFirst(pe => pe.POA_ID == userId && pe.IS_ACTIVE_EXCISER == true);
            
            if(result !=null)
            {
                return true;
            }

            return false;
        }

        public bool IsCreatorPRD(string userID)
        {
            var result = this.uow.RoleMapRepository.GetFirst(us => us.MSACCT.ToUpper() == userID.ToUpper() && us.BROLE == "10087894");

            if(result !=null)
            {
                return true;
            }
            return false;
        }

        public bool IsUnderPlant (string FaCode, string PlantCode)
        {          
            var result = this.uow.MaterialRepository.GetFirst(m => m.STICKER_CODE == FaCode && m.WERKS == PlantCode);
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUnderCompany (string PlantCode, string CompanyCode)
        {
            var checkCompany = this.uow.CompanyPlantMappingRepository.GetFirst(c => c.BWKEY == PlantCode && c.BUKRS == CompanyCode);
            if (checkCompany != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public IEnumerable<USER> GetAdminExciser()
        {
            try
            {            
                List<string> exciser = this.uow.PoaExciserRepository.GetMany(x => x.IS_ACTIVE_EXCISER == true).Select(y=>y.POA_ID.Trim().ToUpper()).ToList();
                return this.uow.UserRepository.GetMany(x => exciser.Contains(x.USER_ID.Trim().ToUpper()));
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<string> GetAdminExciserByNPPBKC(string nppbkcId)
        {
            try
            {               
                var result = new List<string>();
                // var user = this.uow.PoaMapRepository.GetManyQueryable(pm => pm.WERKS == plantName).ToList();    // if parameter = plant/werks          
                var poaList = this.uow.PoaMapRepository.GetManyQueryable(pm => pm.NPPBKC_ID == nppbkcId).Select(x => x.POA_ID).Distinct().ToList();
                foreach(var data in poaList)
                {
                   var status = this.uow.PoaExciserRepository.GetFirst(ex => ex.POA_ID == data && ex.IS_ACTIVE_EXCISER == true);

                   if(status != null)
                   {
                      result.Add(data);
                   }
                }
                return result;                                
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<string> GetPlantByNppbkc (string nppbkcID)
        {
            try
            {                
                var data = this.uow.PoaMapRepository.GetManyQueryable(pm => pm.NPPBKC_ID == nppbkcID).Select(x => x.WERKS).Distinct().ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }


        public T001 GetCompany(string id)
        {
            try
            {
                return this.uow.CompanyRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<PRODUCT_DETAIL_VIEW> GetProductDetailView()
        {
            try
            {
                return this.uow.ProductDetailViewRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public COMPANY_PLANT_MAPPING GetPlantCompanyByPlant (string PlantCode)
        {
            try
            {
                return this.uow.CompanyPlantMappingRepository.Find(PlantCode);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public ZAIDM_EX_BRAND GetFaCodeLatestSKEP(string FaOld)
        {
            try
            {
                var result = this.uow.BrandRepository.GetAll().Where(br => br.FA_CODE == FaOld).OrderByDescending(m => m.SKEP_DATE).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }

        }

        public void CreateBrand (ZAIDM_EX_BRAND data)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.ZAIDM_EX_BRAND.Add(data);
                        context.SaveChanges();

                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create Brand Record Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public void GetUpdateUsedMaterial(string faCodeNew, string werks, bool isUsed, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.ZAIDM_EX_MATERIAL.SingleOrDefault(a=>a.STICKER_CODE == faCodeNew && a.WERKS == werks);
                        if (old != null)
                        {
                            var data = (ZAIDM_EX_MATERIAL)context.Entry(old).GetDatabaseValues().ToObject();

                            data.WERKS = werks;
                            data.MODIFIED_BY = user;
                            data.MODIFIED_DATE = DateTime.Now;
                            data.USED_PRODUCT_DEVELOPMENT = isUsed;

                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion

        #region Create

        public long CreateProduct(PRODUCT_DEVELOPMENT data)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {                        
                        context.PRODUCT_DEVELOPMENT.Add(data);
                        context.SaveChanges();
                       
                        transaction.Commit();
                        return data.PD_ID;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public long CreateProductDetail(PRODUCT_DEVELOPMENT_DETAIL data, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.PRODUCT_DEVELOPMENT_DETAIL.Add(data);
                        context.SaveChanges();
                        data.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);

                        var changes = GetAllChanges(null, data);
                        LogsActivity(context, data.PD_DETAIL_ID.ToString(), changes, formType, actionType, role, user);

                        transaction.Commit();
                        return data.PD_DETAIL_ID;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Create PD Detail Service. See Inner Exception property to see details", ex);
                    }

                }
            }

        }

        #endregion

        #region Update
        
        public void EditProduct(long PD_ID, int nextAction, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT.Find(PD_ID);
                        if (old != null)
                        {
                            var data = (PRODUCT_DEVELOPMENT)context.Entry(old).GetDatabaseValues().ToObject();
                          
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;
                            data.NEXT_ACTION = nextAction;
                            
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            
                            transaction.Commit();
                        }                     
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }         
        }

        public void EditProductDetail(long status, long id, string oldCode, string newCode, string oldDesc, string newDesc, string hl, string marketId,  int formType, int actionType, int role, string user, int? country, string week)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT_DETAIL.Find(id);
                        if (old != null)
                        {
                            var data = (PRODUCT_DEVELOPMENT_DETAIL)context.Entry(old).GetDatabaseValues().ToObject();
                            data.FA_CODE_OLD = oldCode;
                            data.FA_CODE_NEW = newCode;
                            data.FA_CODE_OLD_DESCR = oldDesc;
                            data.FA_CODE_NEW_DESCR = newDesc;
                            data.HL_CODE = hl;
                            data.MARKET_ID =marketId;
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;
                            data.HL_CODE = hl;
                            data.STATUS_APPROVAL = status;
                            data.COUNTRY_ID = country;
                            data.WEEK = week;

                            var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            LogsActivity(context, id.ToString(), changes, formType, actionType, role, user);
                            transaction.Commit();
                        }                                        
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public void EditDetailStatusApproval(long status, long detailId, int formType, int actionType, int role, string user)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT_DETAIL.Find(detailId);
                        if (old != null)
                        {
                            var data = (PRODUCT_DEVELOPMENT_DETAIL)context.Entry(old).GetDatabaseValues().ToObject();
                          
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;                          
                            data.STATUS_APPROVAL = status;

                            //var changes = GetAllChanges(old, data);
                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();
                            //LogsActivity(context, id.ToString(), changes, formType, actionType, role, user);
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion

        #region Changes Log
        /// <summary> Part of Changes Log Step which Mark All Available Changes </summary>        
        /// <param name="old"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        private Dictionary<string, string[]> GetAllChanges(PRODUCT_DEVELOPMENT_DETAIL old, PRODUCT_DEVELOPMENT_DETAIL updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                var columns = new string[]
                     {
                     "FA_CODE_OLD",
                     "FA_CODE_NEW",
                     "HL_CODE",
                     "MARKET_ID",
                     "FA_CODE_OLD_DESCR",
                     "FA_CODE_NEW_DESCR",
                     "WERKS",
                     "IS_IMPORT",
                     "PD_ID",
                     "BUKRS",
                     "STATUS_APPROVAL",
                     "COUNTRY_ID",
                     "WEEK"
                     };
                var oldProps = new Dictionary<string, object>();
                var props = new Dictionary<string, object>();

                foreach (var prop in updated.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    props.Add(prop.Name, prop.GetValue(updated, null));
                    if (old != null)
                        oldProps.Add(prop.Name, prop.GetValue(old, null));
                    else
                        oldProps.Add(prop.Name, null);
                }
                foreach (var item in props)
                {
                    var oldValue = (oldProps[item.Key] != null) ? oldProps[item.Key].ToString() : "N/A";
                    var newValue = (item.Value != null) ? item.ToString() : "N/A";
                    if (!columns.Contains(item.Key))
                        continue;
                    if (item.Key == "BUKRS")
                    {
                        context = new EMSDataModel();
                        var Company = context.T001;

                        if (item.Value != null)                                                    
                            newValue = updated.BUKRS == null ? "N/A" : Company.Where(w => w.BUKRS == updated.BUKRS).FirstOrDefault().BUTXT;
                        if (oldProps[item.Key] != null)                            
                            oldValue = old.BUKRS == null ? "N/A" : Company.Where(w => w.BUKRS == old.BUKRS).FirstOrDefault().BUTXT;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }
                    if (item.Key == "MARKET_ID")
                    {
                        context = new EMSDataModel();
                        var TheMarket = context.ZAIDM_EX_MARKET;
                        if (item.Value != null)                            
                            newValue = updated.MARKET_ID == null ? "N/A" : TheMarket.Where(w => w.MARKET_ID == updated.MARKET_ID).FirstOrDefault().MARKET_DESC;
                        if (oldProps[item.Key] != null)                            
                            oldValue = old.MARKET_ID == null? "N/A" : TheMarket.Where(w => w.MARKET_ID == old.MARKET_ID).FirstOrDefault().MARKET_DESC;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }
                    if (item.Key == "WERKS")
                    {
                        context = new EMSDataModel();
                        var ThePlant = context.T001W;
                        if (item.Value != null)                            
                            newValue = updated.WERKS == null ? "N/A" : ThePlant.Where(w => w.WERKS == updated.WERKS).FirstOrDefault().NAME1;
                        if (oldProps[item.Key] != null)                            
                            oldValue = old.WERKS == null ? "N/A" : ThePlant.Where(w => w.WERKS == old.WERKS).FirstOrDefault().NAME1;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }
                    if (item.Key == "STATUS_APPROVAL")
                    {
                      
                        if (item.Value != null)                            
                           newValue = updated.STATUS_APPROVAL == 0 ? "N/A" : refService.GetReferenceById(updated.STATUS_APPROVAL).REFF_VALUE;

                        if (oldProps[item.Key] != null)                            
                            oldValue = old.STATUS_APPROVAL == 0 ? "N/A" : refService.GetReferenceById(old.STATUS_APPROVAL).REFF_VALUE;

                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }
                    if (item.Key == "COUNTRY_ID")
                    {
                        context = new EMSDataModel();
                        var TheCountry = context.COUNTRY;
                        if (item.Value != null)
                            newValue = updated.COUNTRY_ID == null ? "N/A" : TheCountry.Where(w => w.COUNTRY_ID == updated.COUNTRY_ID).FirstOrDefault().COUNTRY_NAME;
                        if (oldProps[item.Key] != null)
                            oldValue = old.COUNTRY_ID == null ? "N/A" : TheCountry.Where(w => w.COUNTRY_ID == old.COUNTRY_ID).FirstOrDefault().COUNTRY_NAME;
                        if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                            changes.Add(item.Key, new string[] { oldValue, newValue });
                        continue;
                    }
                    if (item.Value != null)
                    {
                        if (item.Value is decimal)
                            newValue = ((decimal)item.Value).ToString("C2");

                        else
                            newValue = item.Value.ToString();
                    }

                    if (oldProps[item.Key] != null)
                    {
                        if (oldProps[item.Key] is decimal)
                            oldValue = ((decimal)oldProps[item.Key]).ToString("C2");

                        else
                            oldValue = oldProps[item.Key].ToString();
                    }
                    if (oldValue.Trim().ToUpper() != newValue.Trim().ToUpper())
                        changes.Add(item.Key, new string[] { oldValue, newValue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        /// <summary> Add record to Changes Log and Workflow History </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="changes"></param>
        /// <param name="formType"></param>
        /// <param name="actionType"></param>
        /// <param name="role"></param>
        /// <param name="actor"></param>
        /// <param name="comment"></param>
        private void LogsActivity(EMSDataModel context, string detailID, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            try
            {
                foreach (var map in changes)
                {
                    refService.AddChangeLog(context,
                        formType,
                        detailID,
                        map.Key,
                        map.Value[0],
                        map.Value[1],
                       actor,
                       DateTime.Now
                        );
                }

                refService.AddWorkflowHistory(context,
                    formType,
                    Convert.ToInt64(detailID),
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
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }

        }
        #endregion

        #region Change Status
        public PRODUCT_DEVELOPMENT_DETAIL ChangeStatus(long id, Core.ReferenceKeys.ApprovalStatus status, int formType, int actionType, int role, string user, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT_DETAIL.Find(id);
                        var data = (PRODUCT_DEVELOPMENT_DETAIL)context.Entry(old).GetDatabaseValues().ToObject();
                        data.STATUS_APPROVAL = refService.GetReferenceByKey(status).REFF_ID;
                        if (status == Core.ReferenceKeys.ApprovalStatus.Completed)
                        {
                            data.LASTAPPROVED_BY = user;
                            data.LASTAPPROVED_DATE = DateTime.Now;
                        }
                        else if (status == Core.ReferenceKeys.ApprovalStatus.AwaitingAdminApproval)
                        {
                            data.LASTMODIFIED_BY = user;
                            data.LASTMODIFIED_DATE = DateTime.Now;
                        }

                        data.APPROVAL_STATUS = context.SYS_REFFERENCES.Find(data.STATUS_APPROVAL);
                        Dictionary<string, string[]> changes = GetAllChanges(old, data);
                        context.Entry(old).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        LogsActivity(context, id.ToString(), changes, formType, actionType, role, user, comment);
                        transaction.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        #endregion

        #region Upload
        public void CreateFileUpload(long PD_ID, string Path, string CreatedBy, long DocId, bool IsGovDoc, string FileName)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {                    
                        var UploadFile = new FILE_UPLOAD();

                        UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormType.ProductDevelopment);
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
                        throw this.HandleException("Exception occured on Product Development Service Upload File. See Inner Exception property to see details", ex);
                    }
                }
            }                              
        }

        public IQueryable<FILE_UPLOAD> GetFileUploadByPDID(long PD_ID)
        {
            try
            {
                var strID = PD_ID.ToString();
                var intFormType = Convert.ToInt32(Enums.FormType.ProductDevelopment);
                return this.uow.FileUploadRepository.GetAll().Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }
      
        public IQueryable<PRODUCT_DEVELOPMENT_UPLOAD> GetItemUploadAll()
        {
            try
            {
                var result = this.uow.ProductDevelopmentUploadRepository.GetAll().Where(a=>a.PD_DETAIL_ID == null);
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public bool AddItemUpload(IEnumerable<PRODUCT_DEVELOPMENT_UPLOAD> files)
        {
            try
            {
                using (var context = new EMSDataModel())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (var item in files)
                        {
                            context.PRODUCT_DEVELOPMENT_UPLOAD.Add(item);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development - SaveUploadedFilesItem(). See Inner Exception property to see details", ex);
            }
        }

        public void AddDetailItemUpload(PRODUCT_DEVELOPMENT_UPLOAD itemUpload)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT_UPLOAD.Find(itemUpload.FILE_ID);
                        if (old != null)
                        {
                            var data = (PRODUCT_DEVELOPMENT_UPLOAD)context.Entry(old).GetDatabaseValues().ToObject();

                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public void UpdateItemUpload(long fileID, long detailId, bool isActive)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT_UPLOAD.Find(fileID);
                        if (old != null)
                        {
                            var data = (PRODUCT_DEVELOPMENT_UPLOAD)context.Entry(old).GetDatabaseValues().ToObject();

                            data.PD_DETAIL_ID = detailId;
                            data.UPLOAD_DATE = DateTime.Now;
                            data.IS_ACTIVE = isActive;

                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();

                            transaction.Commit();
                        }
                    }                   
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }
        public void UpdateOtherDocStatus(long fileID, bool isActive)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var old = context.PRODUCT_DEVELOPMENT_UPLOAD.Find(fileID);
                        if (old != null)
                        {
                            var data = (PRODUCT_DEVELOPMENT_UPLOAD)context.Entry(old).GetDatabaseValues().ToObject();
                            
                            data.UPLOAD_DATE = DateTime.Now;
                            data.IS_ACTIVE = isActive;

                            context.Entry(old).CurrentValues.SetValues(data);
                            context.SaveChanges();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
                    }
                }
            }
        }

        public IEnumerable<PRODUCT_DEVELOPMENT_UPLOAD> GetOtherDocsFile(long detailID)
        {
            try
            {               
                return this.uow.ProductDevelopmentUploadRepository.GetMany( x => x.PD_DETAIL_ID == detailID && x.IS_ACTIVE == true && x.DOCUMENT_ID == null).OrderByDescending(x => x.UPLOAD_DATE);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        //public IEnumerable<PRODUCT_DEVELOPMENT_UPLOAD> GetSupportDocsFile(long detailID)
        //{
        //    try
        //    {
        //        return this.uow.ProductDevelopmentUploadRepository.GetMany(x => x.PD_DETAIL_ID == detailID && x.IS_ACTIVE == true && x.DOCUMENT_ID != null).OrderByDescending(x => x.UPLOAD_DATE);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
        //    }
        //}

        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> GetSupportingDocuments(long formID, string company)
        {
            try
            {
                var approved = GetRefByKey(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Completed));
                return this.uow.SupportDocRepository.GetMany(
                    x => x.FORM_ID == formID &&
                    x.LASTAPPROVED_STATUS == approved.REFF_ID &&
                    x.IS_ACTIVE &&
                    x.BUKRS == company);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<MASTER_SUPPORTING_DOCUMENT> GetSupportDoc(long detailID, long formID, string company)
        {
            try
            {
                var supportingDocs = GetSupportingDocuments(formID, company).ToList();
                var result = new List<MASTER_SUPPORTING_DOCUMENT>();
                foreach (var data in supportingDocs)
                {
                    data.PRODUCT_DEVELOPMENT_UPLOAD = GetUploadedFilesSupportDoc(detailID).Where(x => x.DOCUMENT_ID == data.DOCUMENT_ID).ToList();
                    result.Add(data);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<PRODUCT_DEVELOPMENT_UPLOAD> GetUploadedFilesSupportDoc(long detailID)
        {
            try
            {
                var approved = GetRefByKey(ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.Completed));
                return this.uow.ProductDevelopmentUploadRepository.GetMany(
                    x => x.PD_DETAIL_ID == detailID &&
                    x.IS_ACTIVE == true && x.DOCUMENT_ID != null 
                    ).OrderByDescending(x => x.UPLOAD_DATE);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Product Development Service. See Inner Exception property to see details", ex);
            }
        }
        public SYS_REFFERENCES GetRefByKey(string key)
        {
            try
            {
                //return this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_KEYS == key.ToUpper());
                return this.uow.ReferenceRepository.GetFirst(reff => reff.REFF_KEYS == key.ToUpper() && reff.IS_ACTIVE.HasValue && reff.IS_ACTIVE.Value);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on SystemReferenceService. See Inner Exception property to see details", ex);
            }
        }
     
        #endregion

    }
}
