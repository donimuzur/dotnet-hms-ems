﻿using Sampoerna.EMS.CustomService.Data;
using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Core;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
//using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.CustomService.Services.ManufactureLicense
{
    public class LicenseRequestService : GenericService
    {
        //private EMSDataModel context;
        private SystemReferenceService refService;
        private List<String> fileExtList;
        //private DbContextTransaction transaction;
        private Dictionary<int, string> govstatusList;

        public LicenseRequestService() : base()
        {
            //context = new EMSDataModel();
            refService = new SystemReferenceService();
            //transaction = context.Database.BeginTransaction();

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
        /* 
        private Dictionary<int, string> exciseCreditType;
        private Dictionary<int, string> exciseCreditGuarantee;
        public LicenseRequestService() : base()
        {
            exciseCreditType = new Dictionary<int, string>();
            exciseCreditType.Add(1, "New Excise Credit");
            exciseCreditType.Add(2, "Excise Credit Adjustment");

            exciseCreditGuarantee = new Dictionary<int, string>();
            exciseCreditGuarantee.Add(1, "Company Guarantee");
            exciseCreditGuarantee.Add(2, "Bank Guarantee");
            exciseCreditGuarantee.Add(3, "Excise Bond");
        }
        */
        public List<string> GetFileExtList()
        {
            return fileExtList;
        }

        public MANUFACTURING_LISENCE_REQUEST InsertLicenseRequest(DateTime reqdate, Int64 IRId, string IRformNumb, Int64 StatusId, string UserId, Int32 UserRole = 0, string formNumber = "")
        {
            //var licenseReq = new MANUFACTURING_LISENCE_REQUEST();
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var licenseReq = new MANUFACTURING_LISENCE_REQUEST();
                        //context = new EMSDataModel();
                        //var nppbkc = GetNPPBKCID(IRId);
                        //if (nppbkc != null)
                        //{
                        //    licenseReq.NPPBKC_ID = nppbkc;
                        //}
                        licenseReq.VR_FORM_ID = IRId;
                        licenseReq.REQUEST_DATE = reqdate;
                        //licenseReq.MNF_FORM_NUMBER = GetFormNumber(IRformNumb);
                        licenseReq.MNF_FORM_NUMBER = formNumber;
                        licenseReq.CREATED_BY = UserId;
                        licenseReq.CREATED_DATE = DateTime.Now;
                        licenseReq.LASTAPPROVED_STATUS = StatusId;
                        //licenseReq.NPPBKC_ID = GetNPPBKCID(IRId);

                        context.MANUFACTURING_LISENCE_REQUEST.Add(licenseReq);
                        context.SaveChanges();
                        transaction.Commit();

                        Dictionary<string, string[]> changes = GetAllChanges(new MANUFACTURING_LISENCE_REQUEST(), licenseReq);
                        LogsActivity(licenseReq, changes, (int)Enums.MenuList.LicenseRequest, (int)Enums.ActionType.Created, UserRole, UserId, "");
                        return licenseReq;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Insert. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public MASTER_NPPBKC InsertNPPBKC(string  nppbkc, string address, string city, string city_alias, string text_to, string kppbc, string UserId)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var NppbkcMas = new MASTER_NPPBKC();
                        //context = new EMSDataModel();
                        var oldNppbkc = context.ZAIDM_EX_NPPBKC.Where(w => w.NPPBKC_ID == nppbkc).FirstOrDefault();
                        if (oldNppbkc == null)
                        {
                            NppbkcMas.NPPBKC_ID = nppbkc;
                            NppbkcMas.KPPBC_ADDRESS = address;
                            NppbkcMas.CITY = city;
                            NppbkcMas.CITY_ALIAS = city_alias;
                            NppbkcMas.TEXT_TO = text_to;
                            NppbkcMas.KPPBC_ID = kppbc;
                            NppbkcMas.CREATED_BY = UserId;
                            NppbkcMas.CREATED_DATE = DateTime.Now;

                            context.ZAIDM_EX_NPPBKC.Add(NppbkcMas);
                            context.SaveChanges();
                            transaction.Commit();
                        }
                        return NppbkcMas;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request NPPBKC Insert. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        private Dictionary<string, string[]> GetAllChanges(MANUFACTURING_LISENCE_REQUEST old, MANUFACTURING_LISENCE_REQUEST updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
                //if (old.MNF_REQUEST_ID != updated.MNF_REQUEST_ID )
                //{
                //    var oldvalue = old.MNF_REQUEST_ID == null ? "" : old.MNF_REQUEST_ID.ToString();
                //    var newvalue = updated.MNF_REQUEST_ID == null ? "" : updated.MNF_REQUEST_ID.ToString();
                //    changes.Add("MNF_REQUEST_ID", new string[] { oldvalue, newvalue });
                //}
                if (old.REQUEST_DATE != updated.REQUEST_DATE)
                {
                    var oldvalue = old.REQUEST_DATE == null || old.REQUEST_DATE == DateTime.MinValue ? "N/A" : old.REQUEST_DATE.ToString("dd MMMM yyyy HH:mm:ss");
                    var newvalue = updated.REQUEST_DATE == null ? "N/A" : updated.REQUEST_DATE.ToString("dd MMMM yyyy HH:mm:ss");
                    changes.Add("REQUEST_DATE", new string[] { oldvalue, newvalue });
                }
                if (old.MNF_FORM_NUMBER != updated.MNF_FORM_NUMBER )
                {
                    var oldvalue = old.MNF_FORM_NUMBER == null ? "N/A" : old.MNF_FORM_NUMBER.ToString();
                    var newvalue = updated.MNF_FORM_NUMBER == null ? "N/A" : updated.MNF_FORM_NUMBER.ToString();
                    changes.Add("MNF_FORM_NUMBER", new string[] { oldvalue, newvalue });
                }

                //if (old.CREATED_BY != updated.CREATED_BY)
                //{
                //    var oldvalue = old.CREATED_BY == null ? "" : old.CREATED_BY.ToString();
                //    var newvalue = updated.CREATED_BY == null ? "" : updated.CREATED_BY.ToString();
                //    changes.Add("CREATED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.CREATED_DATE != updated.CREATED_DATE)
                //{
                //    var oldvalue = old.CREATED_DATE == null ? "" : old.CREATED_DATE.ToString();
                //    var newvalue = updated.CREATED_DATE == null ? "" : updated.CREATED_DATE.ToString();
                //    changes.Add("CREATED_DATE", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTMODIFIED_BY != updated.LASTMODIFIED_BY)
                //{
                //    var oldvalue = old.LASTMODIFIED_BY == null ? "" : old.LASTMODIFIED_BY.ToString();
                //    var newvalue = updated.LASTMODIFIED_BY == null ? "" : updated.LASTMODIFIED_BY.ToString();
                //    changes.Add("LASTMODIFIED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTMODIFIED_DATE != updated.LASTMODIFIED_DATE)
                //{
                //    var oldvalue = old.LASTMODIFIED_DATE == null ? "" : old.LASTMODIFIED_DATE.ToString();
                //    var newvalue = updated.LASTMODIFIED_DATE == null ? "" : updated.LASTMODIFIED_DATE.ToString();
                //    changes.Add("LASTMODIFIED_DATE", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTAPPROVED_BY != updated.LASTAPPROVED_BY)
                //{
                //    var oldvalue = old.LASTAPPROVED_BY == null ? "" : old.LASTAPPROVED_BY.ToString();
                //    var newvalue = updated.LASTAPPROVED_BY == null ? "" : updated.LASTAPPROVED_BY.ToString();
                //    changes.Add("LASTAPPROVED_BY", new string[] { oldvalue, newvalue });
                //}
                //if (old.LASTAPPROVED_DATE != updated.LASTAPPROVED_DATE)
                //{
                //    var oldvalue = old.LASTAPPROVED_DATE == null ? "" : old.LASTAPPROVED_DATE.ToString();
                //    var newvalue = updated.LASTAPPROVED_DATE == null ? "" : updated.LASTAPPROVED_DATE.ToString();
                //    changes.Add("LASTAPPROVED_DATE", new string[] { oldvalue, newvalue });
                //}
                if (old.LASTAPPROVED_STATUS != updated.LASTAPPROVED_STATUS)
                {
                    //var oldvalue = old.LASTAPPROVED_STATUS.ToString() == null ? "" : old.LASTAPPROVED_STATUS.ToString();
                    //var newvalue = updated.LASTAPPROVED_STATUS.ToString() == null ? "" : updated.LASTAPPROVED_STATUS.ToString();
                    //changes.Add("LASTAPPROVED_STATUS", new string[] { oldvalue, newvalue });
                    var oldvalue = old.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(old.LASTAPPROVED_STATUS).REFF_KEYS;
                    var newvalue = updated.LASTAPPROVED_STATUS == 0 ? "N/A" : refService.GetReferenceById(updated.LASTAPPROVED_STATUS).REFF_KEYS;
                    changes.Add("LASTAPPROVED_STATUS", new string[] { oldvalue, newvalue });
                }

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
                if (old.NPPBKC_ID != updated.NPPBKC_ID)
                {
                    var oldvalue = old.NPPBKC_ID == null ? "N/A" : old.NPPBKC_ID.ToString();
                    var newvalue = updated.NPPBKC_ID == null ? "N/A" : updated.NPPBKC_ID.ToString();
                    changes.Add("NPPBKC_ID", new string[] { oldvalue, newvalue });
                }
                if (old.DECREE_STATUS != updated.DECREE_STATUS)
                {
                    string oldvalue = "";
                    string newvalue = "";

                    if (old.DECREE_STATUS == true)
                    {
                        oldvalue = "Approved";
                    }
                    else if (old.DECREE_STATUS == false)
                    {
                        oldvalue = "Rejected";
                    } else
                    {
                        oldvalue = "N/A";
                    }

                    if (updated.DECREE_STATUS == true)
                    {
                        newvalue = "Approved";
                    }
                    else if (updated.DECREE_STATUS == false)
                    {
                        newvalue = "Rejected";
                    }
                    else
                    {
                        newvalue = "N/A";
                    }

                    //var oldvalue = old.DECREE_STATUS == null ? "N/A" : old.DECREE_STATUS.ToString();
                    //var newvalue = updated.DECREE_STATUS == null ? "N/A" : updated.DECREE_STATUS.ToString();
                    
                    changes.Add("DECREE_STATUS", new string[] { oldvalue, newvalue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Request Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllDetailChanges(MANUFACTURING_BOUND_CONDITION old, MANUFACTURING_BOUND_CONDITION updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();
               
                if (old.NORTH != updated.NORTH)
                {
                    var oldvalue = old.NORTH == null ? "N/A" : old.NORTH.ToString();
                    var newvalue = updated.NORTH == null ? "N/A" : updated.NORTH.ToString();
                    changes.Add("NORTH", new string[] { oldvalue, newvalue });
                }
                if (old.SOUTH != updated.SOUTH)
                {
                    var oldvalue = old.SOUTH == null ? "N/A" : old.SOUTH.ToString();
                    var newvalue = updated.SOUTH == null ? "N/A" : updated.SOUTH.ToString();
                    changes.Add("SOUTH", new string[] { oldvalue, newvalue });
                }
                if (old.WEST != updated.WEST)
                {
                    var oldvalue = old.WEST == null ? "N/A" : old.WEST.ToString();
                    var newvalue = updated.WEST == null ? "N/A" : updated.WEST.ToString();
                    changes.Add("WEST", new string[] { oldvalue, newvalue });
                }
                if (old.EAST != updated.EAST)
                {
                    var oldvalue = old.EAST == null ? "N/A" : old.EAST.ToString();
                    var newvalue = updated.EAST == null ? "N/A" : updated.EAST.ToString();
                    changes.Add("EAST", new string[] { oldvalue, newvalue });
                }
                if (old.LAND_AREA != updated.LAND_AREA)
                {
                    var oldvalue = old.LAND_AREA == null ? "N/A" : old.LAND_AREA.ToString();
                    var newvalue = updated.LAND_AREA == null ? "N/A" : updated.LAND_AREA.ToString();
                    changes.Add("LAND_AREA", new string[] { oldvalue, newvalue });
                }
                if (old.BUILDING_AREA != updated.BUILDING_AREA)
                {
                    var oldvalue = old.BUILDING_AREA == null ? "N/A" : old.BUILDING_AREA.ToString();
                    var newvalue = updated.BUILDING_AREA == null ? "N/A" : updated.BUILDING_AREA.ToString();
                    changes.Add("BUILDING_AREA", new string[] { oldvalue, newvalue });
                }
                if (old.OWNERSHIP_STATUS != updated.OWNERSHIP_STATUS)
                {
                    var oldvalue = old.OWNERSHIP_STATUS == null ? "N/A" : old.OWNERSHIP_STATUS.ToString();
                    var newvalue = updated.OWNERSHIP_STATUS == null ? "N/A" : updated.OWNERSHIP_STATUS.ToString();
                    changes.Add("OWNERSHIP_STATUS", new string[] { oldvalue, newvalue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Request Detail Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        private Dictionary<string, string[]> GetAllPTChanges(MANUFACTURING_PRODUCT_TYPE old, MANUFACTURING_PRODUCT_TYPE updated)
        {
            try
            {
                var changes = new Dictionary<string, string[]>();

                if (old.PROD_CODE != updated.PROD_CODE && updated.PROD_CODE != null)
                {
                    var oldvalue = old.PROD_CODE == null || old.PROD_CODE == "" ? "N/A" : GetProductTypeLong(old.PROD_CODE);
                    var newvalue = updated.PROD_CODE == null || updated.PROD_CODE == "" ? "N/A" : GetProductTypeLong(updated.PROD_CODE);
                    changes.Add("PROD_CODE", new string[] { oldvalue, newvalue });
                }
                if (old.OTHERS_PROD_TYPE != updated.OTHERS_PROD_TYPE && updated.OTHERS_PROD_TYPE != "")
                {
                    var oldvalue = old.OTHERS_PROD_TYPE == "" || old.OTHERS_PROD_TYPE == null ? "N/A" : old.OTHERS_PROD_TYPE;
                    var newvalue = updated.OTHERS_PROD_TYPE == "" || updated.OTHERS_PROD_TYPE == null ? "N/A" : updated.OTHERS_PROD_TYPE;
                    changes.Add("OTHERS_PROD_TYPE", new string[] { oldvalue, newvalue });
                }
                return changes;
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on Manufacture License Request Detail Get Change For Log. See Inner Exception property to see details", ex);
            }
        }

        public MANUFACTURING_LISENCE_REQUEST UpdateLicenseRequest(DateTime reqdate, Int64 LRId, Int64 AppStatus, string UserId, Int32 UserRole, int ActionType)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        var licenseReq = new MANUFACTURING_LISENCE_REQUEST();
                        //transaction = context.Database.BeginTransaction();
                        Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
                        var OldlicenseReq = new MANUFACTURING_LISENCE_REQUEST();
                        //context = new EMSDataModel();
                        var where = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_REQUEST_ID.Equals(LRId));
                        if (where != null)
                        {
                            licenseReq = where.FirstOrDefault();
                            OldlicenseReq = SetOldValueToTempModel(licenseReq);
                            licenseReq.REQUEST_DATE = reqdate;
                            licenseReq.LASTMODIFIED_BY = UserId;
                            licenseReq.LASTMODIFIED_DATE = DateTime.Now;
                            licenseReq.LASTAPPROVED_STATUS = AppStatus;
                            //licenseReq.LASTAPPROVED_BY = UserId;
                            changes = GetAllChanges(OldlicenseReq, licenseReq);
                            context.SaveChanges();
                            transaction.Commit();
                            LogsActivity(licenseReq, changes, (int)Enums.MenuList.LicenseRequest, ActionType, UserRole, UserId, "");
                        }
                        return licenseReq;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Update. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public MANUFACTURING_LISENCE_REQUEST RemoveLicenseRequest(Int64 LRId)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var licenseReq = new MANUFACTURING_LISENCE_REQUEST();
                        var where = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_REQUEST_ID.Equals(LRId));
                        if (where != null)
                        {
                            licenseReq = where.FirstOrDefault();
                            context.MANUFACTURING_LISENCE_REQUEST.Remove(licenseReq);
                            context.SaveChanges();
                            transaction.Commit();
                        }
                        return licenseReq;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Remove. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public void InsertBoundCondition(long ReqId = 0, string North = "", string East = "", string South = "", string West = "", decimal LandArea = 0, decimal BuildingArea = 0, string OwnershipStatus = "", long InterviewDetailId = 0, string CreatedBy = "")
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var quer = new MANUFACTURING_BOUND_CONDITION();
                        quer.NORTH = North;
                        quer.EAST = East;
                        quer.SOUTH = South;
                        quer.WEST = West;
                        quer.LAND_AREA = LandArea;
                        quer.BUILDING_AREA = BuildingArea;
                        quer.OWNERSHIP_STATUS = OwnershipStatus;
                        quer.MNF_REQUEST_ID = ReqId;
                        quer.VR_FORM_DETAIL_ID = InterviewDetailId;

                        context.MANUFACTURING_BOUND_CONDITION.Add(quer);
                        context.SaveChanges();
                        transaction.Commit();
                        Dictionary<string, string[]> changes = GetAllDetailChanges(new MANUFACTURING_BOUND_CONDITION(), quer);
                        LogsChages(ReqId, changes, (int)Enums.MenuList.LicenseRequest, CreatedBy);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Bound Condition Insert. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public void UpdateBoundCondition(long CondId, string North, string East, string South, string West, decimal LandArea, decimal BuildingArea, string OwnershipStatus)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var quer = new MANUFACTURING_BOUND_CONDITION();
                        Dictionary<string, string[]> changes = new Dictionary<string, string[]>();
                        var Oldquer = new MANUFACTURING_BOUND_CONDITION();
                        
                        var where = context.MANUFACTURING_BOUND_CONDITION.Where(w => w.MNF_COND_ID.Equals(CondId));
                        if (where != null)
                        {
                            quer = where.FirstOrDefault();
                            Oldquer = SetOldValueToTempModelDetail(quer);
                            quer.NORTH = North;
                            quer.EAST = East;
                            quer.SOUTH = South;
                            quer.WEST = West;
                            quer.LAND_AREA = decimal.Truncate(LandArea);
                            quer.BUILDING_AREA = decimal.Truncate(BuildingArea);
                            quer.OWNERSHIP_STATUS = OwnershipStatus;
                            changes = GetAllDetailChanges(Oldquer, quer);
                            context.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Bound Condition Update. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public void InsertProductType(string ProdCode, string OtherProdType, bool actived, long ReqId, string CreatedBy)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var quer = new MANUFACTURING_PRODUCT_TYPE();
            
                        if (ProdCode != "" )
                        { quer.PROD_CODE = ProdCode; }
                        quer.OTHERS_PROD_TYPE = OtherProdType;
                        quer.IS_ACTIVE = actived;
                        quer.MNF_REQUEST_ID  = ReqId;

                        context.MANUFACTURING_PRODUCT_TYPE.Add(quer);
                        context.SaveChanges();
                        transaction.Commit();

                        Dictionary<string, string[]> changes = GetAllPTChanges(new MANUFACTURING_PRODUCT_TYPE(), quer);
                        LogsChages(ReqId, changes, (int)Enums.MenuList.LicenseRequest, CreatedBy);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Product Type Insert. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public void InsertAllProductType(long ReqId, List<string> ProdCode, string CreatedBy)
        {
            using (var context = new EMSDataModel())
            {   
                    try
                    {
                        //context = new EMSDataModel();
                        var removedCode = context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_REQUEST_ID.Equals(ReqId) && !ProdCode.Contains(w.PROD_CODE) && w.OTHERS_PROD_TYPE == "");
                        if(removedCode.Any())
                        {
                            foreach (var row in removedCode)
                            {
                                using (var context_1 = new EMSDataModel())
                                {
                                    using (var transaction_1 = context_1.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var _row = context_1.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_PROD_TYPE_ID.Equals(row.MNF_PROD_TYPE_ID)).FirstOrDefault();
                                            _row.IS_ACTIVE = false;
                                            context_1.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            transaction_1.Rollback();
                                            throw this.HandleException("Exception occured on Manufacture License Request - Product Type Insert All. See Inner Exception property to see details", e);
                                        }
                                        finally
                                        {
                                            transaction_1.Dispose();
                                            context_1.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                        var oldCode = context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_REQUEST_ID.Equals(ReqId) && ProdCode.Contains(w.PROD_CODE));

                        var NewProdCode = new List<string>();

                        if(oldCode.Any())
                        {
                            foreach (var row in oldCode)
                            {
                                using (var context_2 = new EMSDataModel())
                                {
                                    using (var transaction_2 = context_2.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var _row = context_2.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_PROD_TYPE_ID.Equals(row.MNF_PROD_TYPE_ID)).FirstOrDefault();
                                            _row.IS_ACTIVE = true;
                                            context_2.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            transaction_2.Rollback();
                                            throw this.HandleException("Exception occured on Manufacture License Request - Product Type Insert All. See Inner Exception property to see details", e);
                                        }
                                        finally
                                        {
                                            transaction_2.Dispose();
                                            context_2.Dispose();
                                        }
                                    }
                                }
                            }
                            
                            var oldcodelist = oldCode.Select(s => s.PROD_CODE).ToList();
                            NewProdCode = ProdCode.Where(w => !oldcodelist.Contains(w)).ToList();
                        }
                        else
                        {
                            NewProdCode = ProdCode;
                        }
                
                        if (NewProdCode != null)
                        {
                            foreach(var row in NewProdCode)
                            {
                                InsertProductType(row, "", true, ReqId, CreatedBy);
                            }
                        }                
                    }
                    catch (Exception e)
                    {
                        throw this.HandleException("Exception occured on Manufacture License Request - Product Type Insert All. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        context.Dispose();
                    }
            }
        }

        public void InsertAllOtherProductType(long ReqId, List<string> ProdCode, string CreatedBy)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context = new EMSDataModel();
                        var removedCode = context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_REQUEST_ID.Equals(ReqId) && !ProdCode.Contains(w.OTHERS_PROD_TYPE) && w.PROD_CODE == null);
                        if (removedCode.Any())
                        {
                            foreach (var row in removedCode)
                            {
                                using (var context_1 = new EMSDataModel())
                                {
                                    using (var transaction_1 = context_1.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            //context = new EMSDataModel();
                                            var _row = context_1.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_PROD_TYPE_ID.Equals(row.MNF_PROD_TYPE_ID)).FirstOrDefault();
                                            _row.IS_ACTIVE = false;
                                            context_1.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            transaction_1.Rollback();
                                            throw this.HandleException("Exception occured on Manufacture License Request - Other Product Type Insert All. See Inner Exception property to see details", e);
                                        }
                                        finally
                                        {
                                            transaction_1.Dispose();
                                            context_1.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                        var oldCode = context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_REQUEST_ID.Equals(ReqId) && ProdCode.Contains(w.OTHERS_PROD_TYPE));

                        var NewProdCode = new List<string>();

                        if (oldCode.Any())
                        {
                            foreach (var row in oldCode)
                            {
                                using (var context_2 = new EMSDataModel())
                                {
                                    using (var transaction_2 = context_2.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            //context = new EMSDataModel();
                                            var _row = context_2.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_PROD_TYPE_ID.Equals(row.MNF_PROD_TYPE_ID)).FirstOrDefault();
                                            _row.IS_ACTIVE = true;
                                            context_2.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            transaction_2.Rollback();
                                            throw this.HandleException("Exception occured on Manufacture License Request - Other Product Type Insert All. See Inner Exception property to see details", e);
                                        }
                                        finally
                                        {
                                            transaction_2.Dispose();
                                            context_2.Dispose();
                                        }
                                    }
                                }
                            }
                            var oldcodelist = oldCode.Select(s => s.OTHERS_PROD_TYPE).ToList();
                            NewProdCode = ProdCode.Where(w => !oldcodelist.Contains(w)).ToList();                    
                        }
                        else
                        {
                            NewProdCode = ProdCode;
                        }

                        if (NewProdCode != null)
                        {
                            foreach (var row in NewProdCode)
                            {
                                InsertProductType("", row, true, ReqId, CreatedBy);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request - Other Product Type Insert All. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public void RemoveBoundConditionSelected(long LRId, string Createdby, List<MANUFACTURING_BOUND_CONDITION> DeletedList)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context = new EMSDataModel();
                        var deleteList = context.MANUFACTURING_BOUND_CONDITION.Where(w => w.MNF_REQUEST_ID.Equals(LRId));
                        if (deleteList.Count() > 0)
                        {
                            foreach (var delete in deleteList)
                            {
                                var oldData = DeletedList.Where(w => w.MNF_COND_ID == delete.MNF_COND_ID).FirstOrDefault();
                                if (oldData != null)
                                {
                                    Dictionary<string, string[]> changes = GetAllDetailChanges(oldData, new MANUFACTURING_BOUND_CONDITION());
                                    LogsChages(LRId, changes, (int)Enums.MenuList.LicenseRequest, Createdby);
                                }
                            }
                            //context = new EMSDataModel();
                            //transaction = context.Database.BeginTransaction();
                            var deleteList2 = context.MANUFACTURING_BOUND_CONDITION.Where(w => w.MNF_REQUEST_ID.Equals(LRId));
                            foreach (var delete in deleteList2)
                            {
                                context.MANUFACTURING_BOUND_CONDITION.Remove(delete);
                            }
                            context.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request - Bound Condition Remove Selected. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public void RemoveAllBoundCondition(Int64 OPTId)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context = new EMSDataModel();
                        var where = context.MANUFACTURING_BOUND_CONDITION.Where(w => w.MNF_REQUEST_ID.Equals(OPTId));
                        if (where.Any())
                        {
                            foreach (var aa in where)
                            {                        
                                context.MANUFACTURING_BOUND_CONDITION.Remove(aa);                        
                            }
                            context.SaveChanges();
                            transaction.Commit();
                        }                
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request - Bound Condition Remove All. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public MANUFACTURING_PRODUCT_TYPE RemoveProductType(Int64 OPTId)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        var prodtypeReq = new MANUFACTURING_PRODUCT_TYPE();
                        //context = new EMSDataModel();

                        var where = context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.MNF_PROD_TYPE_ID.Equals(OPTId));
                        if (where != null)
                        {
                            prodtypeReq = where.FirstOrDefault();
                            context.MANUFACTURING_PRODUCT_TYPE.Remove(prodtypeReq);
                            context.SaveChanges();
                            transaction.Commit();
                        }
                        return prodtypeReq;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request - Product Type Remove. See Inner Exception property to see details", e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
        }

        public IQueryable<MANUFACTURING_PRODUCT_TYPE> GetProductTypeById()
        {
            var context = new EMSDataModel();
            try
            {   
                var result = context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.IS_ACTIVE == true);
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }

        public string GetProductTypeLong(string ProdCodeId)
        {
            using (var context = new EMSDataModel())
            {
                try
                {

                    var prodtype = context.ZAIDM_EX_PRODTYP.Where(w => w.PROD_CODE.Equals(ProdCodeId)).Select(s => s.PRODUCT_TYPE).FirstOrDefault();

                    return prodtype;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public string GetBaNumberFromInterview(string MnfFormNum)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    string BANum = "";

                    var prodtype = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_FORM_NUMBER.Equals(MnfFormNum)).Select(s => s.VR_FORM_ID).FirstOrDefault();
                    if (prodtype != 0)
                    {
                        BANum = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(prodtype)).Select(s => s.BA_NUMBER).FirstOrDefault();
                    }
                    return BANum;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public string GetInterviewNumFromInterview(string MnfFormNum)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    string InterviewNum = "";

                    var prodtype = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_FORM_NUMBER.Equals(MnfFormNum)).Select(s => s.VR_FORM_ID).FirstOrDefault();
                    if (prodtype != 0)
                    {
                        InterviewNum = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(prodtype)).Select(s => s.FORM_NUMBER).FirstOrDefault();
                    }
                    return InterviewNum;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Product Type Service. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public List<MANUFACTURING_PRODUCT_TYPE> GetAllProductTypeFromLR()
        {   
            try
            {
                var context = new EMSDataModel();
                return context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.IS_ACTIVE == true && w.PROD_CODE != null).ToList();
            }
            catch (Exception ex)
            {
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }

        public string GetFormNumber(string IRformNumb)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    IRformNumb = IRformNumb.Substring(10, IRformNumb.Length - 10);
                    var lastNumber = "";
                    var manlicensereq = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_FORM_NUMBER.Contains(IRformNumb)).OrderByDescending(o => o.MNF_FORM_NUMBER);
                    if (manlicensereq.Any())
                    {
                        lastNumber = manlicensereq.FirstOrDefault().MNF_FORM_NUMBER;
                    }
                    var number = 1;
                    if (lastNumber != "" && lastNumber != null)
                    {
                        number = Convert.ToInt32(lastNumber.Substring(0, 10)) + 1;
                    }
                    var nextNumber = number.ToString().PadLeft(10, '0');
                    var finalNumber = nextNumber + IRformNumb;
                    return finalNumber;
                }
                catch (Exception ex)
                {
                    throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public string GetNPPBKCID(long InterviewRequestId)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var nppbkcid = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(InterviewRequestId)).Select(s => s.NPPBKC_ID).FirstOrDefault();
                    return nppbkcid;
                }
                catch (Exception ex)
                {
                    throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public List<string> GetAllNPPBKCId()
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    //context = new EMSDataModel();
                    return context.MANUFACTURING_LISENCE_REQUEST.Where(w=>w.NPPBKC_ID != null).Select(s=>s.NPPBKC_ID).ToList();
                }
                catch (Exception ex)
                {
                    throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public IQueryable<MANUFACTURING_LISENCE_REQUEST> GetAllNPPBKCForFilter()
        {
            var context = new EMSDataModel();
            try
            {   
                return context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.NPPBKC_ID != null);
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }

        public IEnumerable<INTERVIEW_REQUEST> GetCompType()
        {
            var context = new EMSDataModel();
            try
            {   
                return context.INTERVIEW_REQUEST;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MANUFACTURING_LISENCE_REQUEST> GetAll()
        {
            var context = new EMSDataModel();
            try
            {   
                return context.MANUFACTURING_LISENCE_REQUEST;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MANUFACTURING_LISENCE_REQUEST> GetAll2(EMSDataModel dbx)
        {
            //var context = new EMSDataModel();
            try
            {
                return dbx.MANUFACTURING_LISENCE_REQUEST;
            }
            catch (Exception ex)
            {
                dbx.Dispose();
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MANUFACTURING_LISENCE_REQUEST> GetAllByUser(string username)
        {
            var context = new EMSDataModel();
            try
            {
                return context.MANUFACTURING_LISENCE_REQUEST.Where(w=>w.CREATED_BY == username);
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MANUFACTURING_BOUND_CONDITION> GetBoundCondAll(EMSDataModel dbx)
        {
            //var context = new EMSDataModel();
            try
            {
                return dbx.MANUFACTURING_BOUND_CONDITION;
            }
            catch (Exception ex)
            {
                dbx.Dispose();
                throw this.HandleException("Exception occured on BoundConditionService. See Inner Exception property to see details", ex);
            }
        }



        public IEnumerable<MASTER_NPPBKC> GetNPPBKC()
        {
            try
            {
                return this.uow.NppbkcRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MANUFACTURING_LISENCE_REQUEST> GetLicenseRequestById(Int64 Id)
        {
            var context = new EMSDataModel();
            try
            {
                var result = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_REQUEST_ID.Equals(Id));
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<MANUFACTURING_BOUND_CONDITION> GetBoundConditionById(Int64 Id)
        {
            var context = new EMSDataModel();
            try
            {
                var result = context.MANUFACTURING_BOUND_CONDITION.Where(w => w.MNF_REQUEST_ID.Equals(Id));
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture Bound Condition. See Inner Exception property to see details", ex);
            }
        }

        public int DetailManufactureCount(Int64 Id)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var result = context.INTERVIEW_REQUEST_DETAIL.Where(w => w.VR_FORM_ID.Equals(Id)).Count();
                    return result;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }
        /*
        public IEnumerable<MASTER_FINANCIAL_RATIO> GetFinancialStatements(int submitYear, string company)
        {
            try
            {
                const int DURATION = 2;
                List<int> years = new List<int>();
                for (var i = DURATION; i > 0; i--)
                {
                    years.Add(submitYear - i);
                }

                return this.uow.FinancialRatioRepository.GetMany(x => years.Contains(x.YEAR_PERIOD) && x.BUKRS == company);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }
        */
        public IQueryable<INTERVIEW_REQUEST_DETAIL> GetDetailAll()
        {
            var context = new EMSDataModel();
            try
            {
                var result = context.INTERVIEW_REQUEST_DETAIL;
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }        

        private void LogsActivity(MANUFACTURING_LISENCE_REQUEST data, Dictionary<string, string[]> changes, int formType, int actionType, int role, string actor, string comment = null)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    foreach (var map in changes)
                    {
                       
                    refService.AddChangeLog(context,
                            formType,
                            data.MNF_REQUEST_ID.ToString(),
                            map.Key,
                            map.Value[0],
                            map.Value[1],
                            actor,
                            DateTime.Now
                            );
                    }

                    refService.AddWorkflowHistory(context,
                        formType,
                        Convert.ToInt64(data.MNF_REQUEST_ID),
                        actionType,
                        actor,
                        DateTime.Now,
                        role,
                        comment
                        );
                    context.SaveChanges();

                }
                catch (Exception e)
                {
                    throw this.HandleException("Exception occured on Manufacture License Request Insert. See Inner Exception property to see details", e);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public void LogsChages(long Id, Dictionary<string, string[]> changes, int formType, string actor)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    //context = new EMSDataModel();
                    foreach (var map in changes)
                    {
                        refService.AddChangeLog(context,
                            formType,
                            Id.ToString(),
                            map.Key,
                            map.Value[0],
                            map.Value[1],
                           actor,
                           DateTime.Now
                            );
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Save Log. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        private MANUFACTURING_LISENCE_REQUEST SetOldValueToTempModel(MANUFACTURING_LISENCE_REQUEST IRData)
        {
            var OldIRequest = new MANUFACTURING_LISENCE_REQUEST()
            {
                MNF_REQUEST_ID = IRData.MNF_REQUEST_ID,
                VR_FORM_ID = IRData.VR_FORM_ID,
                REQUEST_DATE = IRData.REQUEST_DATE,
                MNF_FORM_NUMBER = IRData.MNF_FORM_NUMBER,
                CREATED_BY = IRData.CREATED_BY,
                CREATED_DATE = IRData.CREATED_DATE,
                LASTMODIFIED_BY = IRData.LASTMODIFIED_BY,
                LASTMODIFIED_DATE = IRData.LASTMODIFIED_DATE,
                LASTAPPROVED_BY = IRData.LASTAPPROVED_BY,
                LASTAPPROVED_DATE = IRData.LASTAPPROVED_DATE,
                LASTAPPROVED_STATUS = IRData.LASTAPPROVED_STATUS,
                DECREE_NO = IRData.DECREE_NO,
                DECREE_DATE = IRData.DECREE_DATE,
                NPPBKC_ID = IRData.NPPBKC_ID,
                DECREE_STATUS = IRData.DECREE_STATUS
            };
            return OldIRequest;
        }

        private MANUFACTURING_BOUND_CONDITION SetOldValueToTempModelDetail(MANUFACTURING_BOUND_CONDITION IRData)
        {
            var OldIRequest = new MANUFACTURING_BOUND_CONDITION()
            {
                MNF_REQUEST_ID = IRData.MNF_REQUEST_ID,
                MNF_COND_ID = IRData.MNF_COND_ID,
                VR_FORM_DETAIL_ID = IRData.VR_FORM_DETAIL_ID,
                NORTH = IRData.NORTH,
                WEST = IRData.WEST,
                SOUTH = IRData.SOUTH,
                EAST = IRData.EAST,
                LAND_AREA = IRData.LAND_AREA,
                BUILDING_AREA = IRData.BUILDING_AREA,
                OWNERSHIP_STATUS = IRData.OWNERSHIP_STATUS
            };
            return OldIRequest;
        }

        public void DeleteFileUpload(long fileid, string updatedby)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        ;
                        ;
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
                        throw this.HandleException("Exception occured on Manufacture License Request Delete File. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
            
        }

        public void InsertFileUpload(long IRId, string Path, string CreatedBy, long DocId, bool IsGovDoc, string Filename)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var now = DateTime.Now;
                        //context = new EMSDataModel();
                        //transaction = context.Database.BeginTransaction();
                        var UploadFile = new FILE_UPLOAD();
                        UploadFile.FORM_TYPE_ID = Convert.ToInt32(Enums.FormList.License);
                        UploadFile.FORM_ID = IRId.ToString();
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
                        UploadFile.FILE_NAME = Filename;
                        context.FILE_UPLOAD.Add(UploadFile);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Detail Upload File. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }
                    
        }

        public string GetSupportingDocName(long Id)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var docname = "";

                    var doc = context.MASTER_SUPPORTING_DOCUMENT.Where(w => w.DOCUMENT_ID == Id);
                    if (doc.Any())
                    {
                        docname = doc.Select(s => s.SUPPORTING_DOCUMENT_NAME).FirstOrDefault();
                    }
                    return docname;
                }
                catch(Exception e)
                {
                    throw this.HandleException("Exception occured on Manufacture License Request Supporting Doc Name. See Inner Exception property to see details", e);
                }
                finally
                {
                    context.Dispose();
                }
            }
               
        }
        public INTERVIEW_REQUEST UpdateNPPBKCIR(long Id, string nppbkc)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    //context = new EMSDataModel();

                    var InterReq = new INTERVIEW_REQUEST();

                    var where = context.INTERVIEW_REQUEST.Where(w => w.VR_FORM_ID.Equals(Id));
                    if (where.Count() > 0)
                    {
                        InterReq = where.FirstOrDefault();
                        InterReq.NPPBKC_ID = nppbkc;
                        context.SaveChanges();
                    }
                    return InterReq;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on NPPBKC Interview Request Update. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }
        
        public MANUFACTURING_LISENCE_REQUEST UpdateBASKEP(long IRId, bool BAStatus, string BANumber, DateTime BADate, string NPPBKC, Int64 LastApprovedStatus, string ModifiedBy, int ActionType, Int32 UserRole, string Comment, long VR_FORM_ID)
        {
            var context = new EMSDataModel();
            var transaction = context.Database.BeginTransaction();
                
                    try
                    {
                        //context = new EMSDataModel();
                        //transaction = context.Database.BeginTransaction();
                        var now = DateTime.Now;
                        var LRequest = new MANUFACTURING_LISENCE_REQUEST();
                        var OldLRequest = new MANUFACTURING_LISENCE_REQUEST();
                        var Where = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_REQUEST_ID.Equals(IRId));
                        if (Where.Count() > 0)
                        {
                            LRequest = Where.FirstOrDefault();
                            OldLRequest = SetOldValueToTempModel(LRequest);
                            LRequest.LASTMODIFIED_BY = ModifiedBy;
                            LRequest.LASTMODIFIED_DATE = now;
                            LRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                            //LRequest.LASTAPPROVED_BY = ModifiedBy;
                            //LRequest.LASTAPPROVED_DATE = now;
                            LRequest.DECREE_STATUS = BAStatus;
                            LRequest.DECREE_NO = BANumber;
                            LRequest.DECREE_DATE = BADate;
                            LRequest.NPPBKC_ID = NPPBKC;
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        if (Where.Count() > 0)
                        {
                            Dictionary<string, string[]> changes = GetAllChanges(OldLRequest, LRequest);
                            LogsActivity(LRequest, changes, (int)Enums.MenuList.LicenseRequest, ActionType, UserRole, ModifiedBy, Comment);
                            /* Update NPPBKC to Interview Request */
                            UpdateNPPBKCIR(VR_FORM_ID, NPPBKC);
                        }
                        return LRequest;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Update. See Inner Exception property to see details", ex);
                    }
                    //finally
                    //{
                    //    transaction.Dispose();
                    //    context.Dispose();
                    //}
                  
        }

        public List<long> GetLicenseNeedApproveWithSameNPPBKC(string Approver)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    //context = new EMSDataModel();
                    var listIR = new List<long>();
                    var ApproverNPPBKC = context.POA_MAP.Where(w => w.POA_ID == Approver);
                    if (ApproverNPPBKC.Any())
                    {
                        var NPPBKC = ApproverNPPBKC.Select(s => s.NPPBKC_ID).ToList();
                        listIR = context.MANUFACTURING_LISENCE_REQUEST.Where(w => NPPBKC.Contains(w.INTERVIEW_REQUEST.NPPBKC_ID)).Select(s => s.MNF_REQUEST_ID).ToList();
                    }
                    return listIR;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved With Same NPPBKC. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public List<long> GetLicenseLastApproveAfterSubmit(string Approver)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    //context = new EMSDataModel();
                    var listIR = new List<long>();
                    var ApproverAll = context.POA_MAP.Where(w => w.POA_ID == Approver);
                    if (ApproverAll.Any())
                    {
                        var statusIdWait = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
                        var statusIdGov = refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID;
                        var poaid = ApproverAll.Select(s=>s.POA_ID).ToList();

                        listIR = context.MANUFACTURING_LISENCE_REQUEST.Where(w => (w.LASTAPPROVED_STATUS == statusIdWait || w.LASTAPPROVED_STATUS == statusIdGov) && poaid.Contains(w.LASTAPPROVED_BY)).Select(s => s.MNF_REQUEST_ID).ToList();
                    }
                    return listIR;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved With Same NPPBKC. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public List<long> GetLicenseNeedApproveWithoutNPPBKC(string Approver)
        {
            using (var context = new EMSDataModel())
            {
                try
                {

                    var listIR = new List<long>();
                    var isExciser = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true && w.POA_ID == Approver);
                    if (isExciser.Any())
                    {
                        var statusId = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
                        var statusIdcompleted = refService.GetRefByKey("COMPLETED").REFF_ID;
                        var statusIdcanceled = refService.GetRefByKey("CANCELED").REFF_ID;
                        //listIR = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.LASTAPPROVED_STATUS == statusId && w.NPPBKC_ID != null).Select(s => s.MNF_REQUEST_ID).ToList();
                        listIR = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.LASTAPPROVED_STATUS == statusId || w.LASTAPPROVED_STATUS == statusIdcompleted || w.LASTAPPROVED_STATUS == statusIdcanceled).Select(s => s.MNF_REQUEST_ID).ToList();  //&& w.NPPBKC_ID != null
                    }
                    return listIR;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved Without NPPBKC. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
                
        }

        public List<long> GetInterviewNeedApproveWithNPPBKCButNoExcise(string Approver)
        {
            using (var context = new EMSDataModel())
            {
                try
                {

                    var listIR = new List<long>();
                    var Exciser = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true);
                    var isExciser = Exciser.Where(w => w.POA_ID == Approver);
                    if (isExciser.Any())
                    {
                        var statusId = refService.GetRefByKey("WAITING_POA_APPROVAL").REFF_ID;
                        var statusIdskep = refService.GetRefByKey("WAITING_POA_SKEP_APPROVAL").REFF_ID;
                        var statusIdcompleted = refService.GetRefByKey("COMPLETED").REFF_ID;
                        var statusIdcanceled = refService.GetRefByKey("CANCELED").REFF_ID;
                        var Interview = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.LASTAPPROVED_STATUS == statusId || w.LASTAPPROVED_STATUS == statusIdskep || w.LASTAPPROVED_STATUS == statusIdcompleted || w.LASTAPPROVED_STATUS == statusIdcanceled); //w.NPPBKC_ID != null &&
                        if (Interview.Any())
                        {
                            var listExciser = Exciser.Select(s => s.POA_ID).ToList();
                            var NPPBKCwithoutExciser = context.POA_MAP.Where(w => !listExciser.Contains(w.POA_ID)).Select(s => s.NPPBKC_ID).ToList();
                            listIR = Interview.Where(w => NPPBKCwithoutExciser.Contains(w.INTERVIEW_REQUEST.NPPBKC_ID) || w.INTERVIEW_REQUEST.NPPBKC_ID == null).Select(s => s.MNF_REQUEST_ID).ToList();
                        }
                    }
                    return listIR;
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get IR Approved Without NPPBKC. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }   
        }

        public IQueryable<FILE_UPLOAD> GetFileUploadByLRId(long LRId)
        {
            var context = new EMSDataModel();
            try
            {   
                var strID = LRId.ToString();
                var intFormType = Convert.ToInt32(Enums.FormList.License);
                return context.FILE_UPLOAD.Where(w => w.FORM_ID == strID && w.FORM_TYPE_ID == intFormType && w.IS_ACTIVE == true);
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on Manufacture License Request Detail File Upload List. See Inner Exception property to see details", ex);
            }
        }

        public MANUFACTURING_LISENCE_REQUEST UpdateStatus(EMSDataModel dbx, long LRId, Int64 LastApprovedStatus = 0, string ModifiedBy = "", int ActionType = 0, Int32 UserRole = 0, string Comment = "")
        {
            //var context = new EMSDataModel();
            var transaction = dbx.Database.BeginTransaction();
            try
            {
                var now = DateTime.Now;
                var LRequest = new MANUFACTURING_LISENCE_REQUEST();
                var OldLRequest = new MANUFACTURING_LISENCE_REQUEST();
                var Where = dbx.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_REQUEST_ID.Equals(LRId));
                if (Where.Count() > 0)
                {
                    LRequest = Where.FirstOrDefault();
                    OldLRequest = SetOldValueToTempModel(LRequest);
                    LRequest.LASTMODIFIED_BY = ModifiedBy;
                    LRequest.LASTMODIFIED_DATE = now;
                    LRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                    if (ActionType == (int)Enums.ActionType.Approve || ActionType == (int)Enums.ActionType.Revise)
                    {
                        LRequest.LASTAPPROVED_BY = ModifiedBy;
                        LRequest.LASTAPPROVED_DATE = now;
                    }
                    dbx.SaveChanges();
                }
                transaction.Commit();
                if (Where.Count() > 0)
                {
                    Dictionary<string, string[]> changes = GetAllChanges(OldLRequest, LRequest);
                    LogsActivity(LRequest, changes, (int)Enums.MenuList.LicenseRequest, ActionType, UserRole, ModifiedBy, Comment);
                }

                return LRequest;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw this.HandleException("Exception occured on Manufacture License License Request Update. See Inner Exception property to see details", ex);
            }
            //finally
            //{
            //    context.Dispose();
            //    transaction.Dispose();
            //}

           
        }

        public Dictionary<int, string> GetGovStatusList()
        {
            return govstatusList;
        }

        public MANUFACTURING_LISENCE_REQUEST UpdateDECREESKEP(long IRId, bool DECREEStatus, string DECREENo, DateTime DECREEDate, Int64 LastApprovedStatus, string ModifiedBy, int ActionType, Int32 UserRole, string Comment)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context = new EMSDataModel();
                        //transaction = context.Database.BeginTransaction();
                        var now = DateTime.Now;
                        var LRequest = new MANUFACTURING_LISENCE_REQUEST();
                        var OldLRequest = new MANUFACTURING_LISENCE_REQUEST();
                        var Where = context.MANUFACTURING_LISENCE_REQUEST.Where(w => w.MNF_REQUEST_ID.Equals(IRId));
                        if (Where.Count() > 0)
                        {
                            LRequest = Where.FirstOrDefault();
                            OldLRequest = SetOldValueToTempModel(LRequest);
                            LRequest.LASTMODIFIED_BY = ModifiedBy;
                            LRequest.LASTMODIFIED_DATE = now;
                            LRequest.LASTAPPROVED_STATUS = LastApprovedStatus;
                            LRequest.LASTAPPROVED_BY = ModifiedBy;
                            LRequest.LASTAPPROVED_DATE = now;
                            LRequest.DECREE_STATUS = DECREEStatus;
                            LRequest.DECREE_NO = DECREENo;
                            LRequest.DECREE_DATE = DECREEDate;
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        if (Where.Count() > 0)
                        {
                            Dictionary<string, string[]> changes = GetAllChanges(OldLRequest, LRequest);
                            LogsActivity(LRequest, changes, (int)Enums.MenuList.LicenseRequest, ActionType, UserRole, ModifiedBy, Comment);
                        }
                        return LRequest;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw this.HandleException("Exception occured on Manufacture License Request Update. See Inner Exception property to see details", ex);
                    }
                    finally
                    {
                        transaction.Dispose();
                        context.Dispose();
                    }
                }
            }       
        }

        public List<POA> GetPOAApproverList(long IRId)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var ListPOA = new List<POA>();
                    var RealListPOA = new List<POA>();
                    var IRequest = GetLicenseRequestById(IRId).FirstOrDefault();

                    if (IRequest != null)
                    {
                        //var NPPBKCId = IRequest.NPPBKC_ID;
                        var NPPBKCId = GetNPPBKCID(IRequest.VR_FORM_ID);
                        if (IRequest.SYS_REFFERENCES.REFF_KEYS != ReferenceLookup.Instance.GetReferenceKey(ReferenceKeys.ApprovalStatus.AwaitingPoaSkepApproval) && IRequest.LASTAPPROVED_BY == null)
                        {
                            var ListPOA_Nppbkc = context.POA_MAP.Where(w => w.NPPBKC_ID.Equals(NPPBKCId) && w.POA.IS_ACTIVE == true && w.POA_ID != IRequest.CREATED_BY).Select(s => s.POA_ID).ToList();
                            var OriexcisePOA = context.POA_EXCISER.Where(w => w.IS_ACTIVE_EXCISER == true).Select(s => s.POA_ID).ToList();
                            var excisePOA = new List<string>();
                            if (ListPOA_Nppbkc.Count() == 0)
                            {
                                excisePOA = OriexcisePOA.Where(w => ListPOA_Nppbkc.Contains(w)).ToList();
                            }
                            var ListPOAExcise_Raw = context.POA.Where(w => w.POA_ID != IRequest.CREATED_BY && w.IS_ACTIVE == true);
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
                            var lastApprover = IRequest;
                            if (lastApprover != null)
                            {
                                ListPOA.Add(new POA
                                {
                                    POA_ID = lastApprover.LASTAPPROVED_BY,
                                    POA_EMAIL = lastApprover.USER1.EMAIL,
                                    PRINTED_NAME = lastApprover.USER1.FIRST_NAME
                                });
                                var poadelegate = GetPOADelegationOfUser(lastApprover.LASTAPPROVED_BY);
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
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get POA Approver. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public POA_DELEGATION GetPOADelegationOfUser(string UserId)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var now = DateTime.Now.Date;
                    return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_FROM.Equals(UserId)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw this.HandleException("Exception occured on Manufacture License Interview Request Get Delegate. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }   
        }

        public IQueryable<POA_DELEGATION> GetPOADelegatedUser(string UserId)
        {
            var context = new EMSDataModel();
            try
            {
                var now = DateTime.Now.Date;
                return context.POA_DELEGATION.Where(w => w.DATE_FROM <= now && w.DATE_TO >= now && w.POA_TO.Equals(UserId));
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on Manufacture License Interview Request Get Delegate. See Inner Exception property to see details", ex);
            }
        }

        public IQueryable<INTERVIEW_REQUEST> GetIRCompleteAll()
        {
            try
            {
                var context = new EMSDataModel();
                var reff_value = refService.GetRefByKey("COMPLETED").REFF_ID;
                var ML_list = GetAll2(context).Select(s=>s.VR_FORM_ID);
                var result = context.INTERVIEW_REQUEST.Where(w => w.LASTAPPROVED_STATUS.Equals(reff_value) && !ML_list.Contains(w.VR_FORM_ID));
                return result;
            }
            catch (Exception ex)
            {
                //context.Dispose();
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetIRIDAll()
        {
            using (var context = new EMSDataModel())
            {
                try
                {

                    var result = context.MANUFACTURING_LISENCE_REQUEST.Select(s => s.VR_FORM_ID).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }   
        }

        public List<long> GetStatusIDAll()
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    var result = context.MANUFACTURING_LISENCE_REQUEST.Select(s => s.LASTAPPROVED_STATUS).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }   
        }

        public IQueryable<SYS_REFFERENCES> GetReffValueAll()
        {
            var context = new EMSDataModel();
            try
            {   
                var result = context.SYS_REFFERENCES;
                return result;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
            }
        }

        public List<long> GetLRIdProdType(string prod_code)
        {
            using (var context = new EMSDataModel())
            {
                try
                {
                    //context = new EMSDataModel();
                    return context.MANUFACTURING_PRODUCT_TYPE.Where(w => w.PROD_CODE == prod_code).Select(s => s.MNF_REQUEST_ID).ToList();
                }
                catch (Exception ex)
                {
                    throw HandleException("Exception occured on Manufacture License Request. See Inner Exception property to see details", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public IQueryable<vwMLLicenseRequest> GetvwLicenseRequestAll()
        {
            var context = new EMSDataModel();
            try
            {   
                return context.vwMLLicenseRequest;
            }
            catch (Exception ex)
            {
                context.Dispose();
                throw this.HandleException("Exception occured on ExciseCreditService. See Inner Exception property to see details", ex);
            }
        }
    }
}
