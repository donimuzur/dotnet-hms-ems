
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Sampoerna.EMS.CustomService.Data;

namespace Sampoerna.EMS.CustomService.Repositories
{
	public class UnitOfWork : IDisposable
	{
		#region Unit of Work DB Context
		public EMSDataModel Context
		{
			get;
			private set;
		}

		#endregion

		#region Unit of Work Constructors
		public UnitOfWork()
		{
			Context = new EMSDataModel();
		}
		#endregion

		#region Unit of Work Repositories
		public GenericRepository<USER> UserRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<USER, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MASTER_FINANCIAL_RATIO> FinancialRatioRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MASTER_FINANCIAL_RATIO, EMSDataModel>(Context);
			}
		}
		public GenericRepository<T001> CompanyRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<T001, EMSDataModel>(Context);
			}
		}

		public GenericRepository<SYS_REFFERENCES_TYPE> ReferenceTypeRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<SYS_REFFERENCES_TYPE, EMSDataModel>(Context);
			}
		}

		public GenericRepository<SYS_REFFERENCES> ReferenceRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<SYS_REFFERENCES, EMSDataModel>(Context);
			}
		}
		public GenericRepository<TARIFF> TariffRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<TARIFF, EMSDataModel>(Context);
			}
		}
		public GenericRepository<CHANGES_HISTORY> ChangeLogRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<CHANGES_HISTORY, EMSDataModel>(Context);
			}
		}

		public GenericRepository<WORKFLOW_HISTORY> WorkflowHistoryRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<WORKFLOW_HISTORY, EMSDataModel>(Context);
			}
		}

		public GenericRepository<EMAILVARIABEL> EmailVariableRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<EMAILVARIABEL, EMSDataModel>(Context);
			}
		}

		public GenericRepository<CONTENTEMAIL> EmailContentRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<CONTENTEMAIL, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MASTER_SUPPORTING_DOCUMENT> SupportDocRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MASTER_SUPPORTING_DOCUMENT, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MASTER_PRODUCT_TYPE> ProductTypeRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MASTER_PRODUCT_TYPE, EMSDataModel>(Context);
			}
		}

		public GenericRepository<POA_EXCISER> PoaExciserRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<POA_EXCISER, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MASTER_PLANT> PlantRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MASTER_PLANT, EMSDataModel>(Context);
			}
		}

		public GenericRepository<LFA1> AccountRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<LFA1, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MASTER_KPPBC> KppbcRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MASTER_KPPBC, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MASTER_NPPBKC> NppbkcRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MASTER_NPPBKC, EMSDataModel>(Context);
			}
		}

		public GenericRepository<EXCISE_CREDIT> ExciseCreditRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<EXCISE_CREDIT, EMSDataModel>(Context);
			}
		}

		public GenericRepository<POA> POARepository
		{
			get
			{
				return RepositoryFactory.GetInstance<POA, EMSDataModel>(Context);
			}
		}

		public GenericRepository<EXCISE_ADJUSTMENT_CALCULATE> ExciseAdjustmentRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<EXCISE_ADJUSTMENT_CALCULATE, EMSDataModel>(Context);
			}
		}

		public GenericRepository<CK1> CK1Repository
		{
			get
			{
				return RepositoryFactory.GetInstance<CK1, EMSDataModel>(Context);
			}
		}

		public GenericRepository<PRODUCT_DEVELOPMENT> ProductDevelopmentRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<PRODUCT_DEVELOPMENT, EMSDataModel>(Context);
			}
		}

		public GenericRepository<PRODUCT_DEVELOPMENT_DETAIL> ProductDevelopmentDetailRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<PRODUCT_DEVELOPMENT_DETAIL, EMSDataModel>(Context);
			}
		}

		public GenericRepository<ZAIDM_EX_BRAND> BrandRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<ZAIDM_EX_BRAND, EMSDataModel>(Context);
			}
		}

		public GenericRepository<ZAIDM_EX_MARKET> MarketRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<ZAIDM_EX_MARKET, EMSDataModel>(Context);
			}
		}

		public GenericRepository<ZAIDM_EX_MATERIAL> MaterialRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<ZAIDM_EX_MATERIAL, EMSDataModel>(Context);
			}
		}

		public GenericRepository<FILE_UPLOAD> FileUploadRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<FILE_UPLOAD, EMSDataModel>(Context);
			}
		}

		public GenericRepository<BROLE_MAP> RoleMapRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<BROLE_MAP, EMSDataModel>(Context);
			}
		}

		public GenericRepository<ADMIN_APPROVAL_VIEW> AdminApprovalViewRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<ADMIN_APPROVAL_VIEW, EMSDataModel>(Context);
			}
		}
		public GenericRepository<COMPANY_PLANT_MAPPING> CompanyPlantMappingRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<COMPANY_PLANT_MAPPING, EMSDataModel>(Context);
			}
		}

		public GenericRepository<CK1_EXCISE_CALCULATE> Ck1ExciseCalculatRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<CK1_EXCISE_CALCULATE, EMSDataModel>(Context);
			}
		}

		public GenericRepository<REPLACEMENT_DOCUMENTS> ReplacementDocumentRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<REPLACEMENT_DOCUMENTS, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MANUFACTURING_LISENCE_REQUEST> MANUFACTURINGLISENCEREQRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MANUFACTURING_LISENCE_REQUEST, EMSDataModel>(Context);
			}
		}

		public GenericRepository<MANUFACTURING_BOUND_CONDITION> MANUFACTURINGBOUNDCONDRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<MANUFACTURING_BOUND_CONDITION, EMSDataModel>(Context);
			}
		}

		public GenericRepository<BRAND_REGISTRATION_REQ> BrandRegistrationReqRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<BRAND_REGISTRATION_REQ, EMSDataModel>(Context);
			}
		}

		public GenericRepository<BRAND_REGISTRATION_REQ_DETAIL> BrandRegistrationReqDetailRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<BRAND_REGISTRATION_REQ_DETAIL, EMSDataModel>(Context);
			}
		}
		public GenericRepository<RECEIVED_DECREE> ReceivedDecreeRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<RECEIVED_DECREE, EMSDataModel>(Context);
			}
		}

		public GenericRepository<RECEIVED_DECREE_DETAIL> ReceivedDecreeDetailRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<RECEIVED_DECREE_DETAIL, EMSDataModel>(Context);
			}
		}
        public GenericRepository<ROLE_ADMIN_APPROVER_VIEW> RoleAdminApprovalRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<ROLE_ADMIN_APPROVER_VIEW, EMSDataModel>(Context);
            }
        }

        public GenericRepository<POA_DELEGATION> PoaDelegationRepository
		{
			get
			{
				return RepositoryFactory.GetInstance<POA_DELEGATION, EMSDataModel>(Context);
			}
		}

        public GenericRepository<POA_MAP> PoaMapRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<POA_MAP, EMSDataModel>(Context);
            }
        }

        public GenericRepository<EXCISE_CREDIT_APPROVED_DETAIL> ExciseCreditApprovedDetailRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<EXCISE_CREDIT_APPROVED_DETAIL, EMSDataModel>(Context);
            }
        }

        public GenericRepository<PRODUCT_DETAIL_VIEW> ProductDetailViewRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<PRODUCT_DETAIL_VIEW, EMSDataModel>(Context);
            }
        }

        public GenericRepository<PRODUCT_DEVELOPMENT_UPLOAD> ProductDevelopmentUploadRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<PRODUCT_DEVELOPMENT_UPLOAD, EMSDataModel>(Context);
            }
        }
        public GenericRepository<PRINTOUT_LAYOUT> PrintoutLayoutRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<PRINTOUT_LAYOUT, EMSDataModel>(Context);
            }
        }
        public GenericRepository<USER_PRINTOUT_LAYOUT> UserPrintoutLayoutRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<USER_PRINTOUT_LAYOUT, EMSDataModel>(Context);
            }
        }

        public GenericRepository<EXCISE_CREDIT_DETAILCK1> ExciseCreditDetailCK1
        {
            get
            {
                return RepositoryFactory.GetInstance<EXCISE_CREDIT_DETAILCK1, EMSDataModel>(Context);
            }
        }

        public GenericRepository<CK1_EXCISE_CALCULATE_ADJUST> Ck1ExciseCalculateAdjustRepository
        {
            get
            {
                return RepositoryFactory.GetInstance<CK1_EXCISE_CALCULATE_ADJUST, EMSDataModel>(Context);
            }
        }

        #endregion

        #region Unit of Work Business Methods
        public bool Save()
		{
			try
			{
				Context.SaveChanges();
				return true;
			}
			catch (DbEntityValidationException e)
			{
				var outputs = new List<String>();
				foreach (var evt in e.EntityValidationErrors)
				{
					outputs.Add(string.Format("{0} {1}: Entity of type \"{2}\" in state \"{3}\" has the following validation errors:",
						DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), evt.Entry.Entity.GetType().Name, evt.Entry.State));

					foreach (var ve in evt.ValidationErrors)
					{
						outputs.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
					}

				}
				System.IO.File.AppendAllLines(ConfigurationManager.AppSettings["LogPath"], outputs);
				throw e;
			}
		}
		#endregion

		#region IDisposable Implementation

		#region private IDisposable implementation variable declaration
		private bool disposed = false;
		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					Debug.WriteLine("Unit of Work is being disposed!");
					Context.Dispose();
				}
			}
			disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
