using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlPoaDataMapper : IXmlDataReader 
    {
        private XmlDataMapper _xmlMapper = null;
        public List<String> ErroList; 
        public XmlPoaDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
            ErroList = new List<string>();
        }

        
      
      
        public string InsertToDatabase()
        {
          return _xmlMapper.MoveFile();
        }

        public List<string> GetErrorList()
        {
            return _xmlMapper.Errors;
        }
    }
}
