using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class SchedulerConfigJson
    {
        public IsReadConfig IsRead { get; set; }
    }

    public class IsReadConfig
    {
        public bool POA { get; set; }
		public bool POAMAP {get;set;}
		public bool BRANDREG {get;set;}
		public bool CK5	{get;set;}
		public bool InvMovement{get;set;}
		public bool BOMMAP{get;set;}
		public bool BLOCKSTOCK{get;set;}
		public bool PRDOUTPUT{get;set;}
		public bool CK1{get;set;}
		public bool PAYMENT{get;set;}
		public bool USER{get;set;}
		public bool COY{get;set;}
		public bool T001K{get;set;}
		public bool UOM{get;set;}
		public bool NPPBKC{get;set;}
		public bool KPPBC{get;set;}
		public bool VENDOR{get;set;}
		public bool MARKET{get;set;}
		public bool PRODTYP{get;set;}
		public bool PCODE{get;set;}
		public bool SERIES{get;set;}
		public bool T001W{get;set;}
		public bool GOODTYP{get;set;}
        public bool MATERIAL { get; set; }
    }
}
