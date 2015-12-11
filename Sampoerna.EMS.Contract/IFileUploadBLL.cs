using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
   public interface IFileUploadBLL
   {
       void Save(FILE_UPLOAD file_upload);
   }
}
