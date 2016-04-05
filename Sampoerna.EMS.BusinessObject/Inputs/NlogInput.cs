using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class NlogGetByParamInput
    {
        public string FileName { get; set; }
        public int? Month { get; set; }
        public string LogDate { get; set; }
    }

    public class BackupXmlLogInput
    {
        public string FileName { get; set; }

        public int? Month { get; set; }
        
        public string FolderPath { get; set; }

        public string FileZipName { get; set; }
    }
}
