using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.PBCK7AndPBCK3
{
    public class Pbck7Pbck3CreateViewModel : BaseModel
    {
        public string Pbck7Number { get; set; }
        public DateTime Pbck7Date { get; set; }
        public string Pbck3Number { get; set; }
        public DateTime? Pbck3Date { get; set; }
        public int DocumnetType { get; set; }
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public string NppbkcId { get; set; }
        public string Lampiran { get; set; }


        //selectList
        public SelectList NppbkIdList { get; set; }
        public SelectList PlantList { get; set; }




    }
}