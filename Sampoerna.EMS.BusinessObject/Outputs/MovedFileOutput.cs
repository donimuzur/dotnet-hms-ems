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

        public List<string> ErrorList { get; set; } 

        public MovedFileOutput(string fileName, bool isError = false, List<string> errors = null )
        {
            this.FileName = fileName;
            this.IsError = isError;
            if (errors != null)
            {
                this.ErrorList = errors;
            }
            else
            {
                this.ErrorList = new List<string>();
            }
        }
    }
}
