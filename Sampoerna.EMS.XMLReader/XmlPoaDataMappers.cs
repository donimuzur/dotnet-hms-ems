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

        public XmlPoaDataMapper(string filename)
        {
            _xmlMapper = new XmlDataMapper(filename);
           
        }

        
      
      
        public string InsertToDatabase()
        {
          return _xmlMapper.MoveFile();
        }

       



    }
}
