using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlDataMapper
    {
        private const string RootPath = @"D:/XML_EMS/";
        public XElement _xmlData = null;
        private string _xmlName = null;
        public ILogger logger;
        public IUnitOfWork uow;

        public XmlDataMapper(string xmlName)  
        {
            _xmlName = xmlName;
            _xmlData = ReadXMLFile();
            logger = new NullLogger();
            uow = new SqlUnitOfWork(logger);
          
        }


        private XElement ReadXMLFile()
        {
            return XElement.Load(Path.Combine(RootPath, _xmlName +  ".xml"));
        }

        public XElement GetElement(string elementName)
        {
            if (_xmlData == null)
                return null;
            
             return _xmlData.Element(elementName);
        }
        public IEnumerable<XElement> GetElements(string elementName)
        {
            if (_xmlData == null)
                return null;

            return _xmlData.Elements(elementName);
        }
        public void InsertToDatabase<T>(List<T> items) where  T : class 
        {
            var repo = uow.GetGenericRepository<T>();

            try
            {
                foreach (var item in items)
                {
                    repo.Insert(item);

                }
            }
            catch (Exception ex)
            {
                uow.RevertChanges();
            }
            uow.SaveChanges();

        }

    }
}
