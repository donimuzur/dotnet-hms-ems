using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class MovedFileOutput
    {
        public string FileName { get; set; }
        public bool IsError { get; set; }

        public MovedFileOutput(string fileName, bool isError = false)
        {
            this.FileName = fileName;
            this.IsError = isError;
        }
    }
}
