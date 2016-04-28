using System.Collections.Generic;
using Sampoerna.EMS.Core;
using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1MonitoringMutasiDto
    {
        public int Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public Enums.PBCK1Type Pbck1Type { get; set; }

        //get from monitoring usage
        public decimal? ExGoodsQuota { get; set; }
        /// <summary>
        /// calculate from pbck-1 child
        /// </summary>
        public decimal AdditionalExGoodsQuota { get; set; }
        /// <summary>
        /// Lates Saldo from LACK
        /// </summary>
        public decimal PreviousFinalBalance { get; set; }
        /// <summary>
        /// Get from Received Amount in CK5(data come from SAP)
        /// </summary>
        public decimal Received { get; set; }
        public decimal ReceivedAdditional { get; set; }
        public List<CK5Dto> Ck5List { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string SupPlant { get; set; }
        public string SupPlantDesc { get; set; }
        public string SupComp { get; set; }
        public string OriNppbkc { get; set; }
        public string OriKppbc { get; set; }
        public bool IsNppbkcImport { get; set; }
        public string ExcGoodsType { get; set; }
        public string RecComp { get; set; }
        public string RecNppbkc { get; set; }
        public string RecKppbc { get; set; }
     
    }
}
