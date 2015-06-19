using System;
using System.CodeDom;
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
using System.Configuration;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlDataMapper
    {
        private string RootPath= ConfigurationManager.AppSettings["XmlFolderPath"];
        public XElement _xmlData = null;
        private string _xmlName = null;
        public ILogger logger;
        public IUnitOfWork uow;
        public string _currFilePath;
        public string _currFileName;
        public XmlDataMapper(string xmlName)
        {
            
            _xmlName = xmlName;
            var files = Directory.GetFiles(RootPath);
            var currFilePath = (from f in files where f.Contains(_xmlName) select f).FirstOrDefault();
            _currFilePath = currFilePath;
            _currFileName = Path.GetFileName(currFilePath);
            
            _xmlData = ReadXMLFile();
            logger = new NullLogger();
            uow = new SqlUnitOfWork(logger);
          
        }


        private XElement ReadXMLFile()
        {

            if (_currFilePath == null)
                return null;
            return XElement.Load(Path.Combine(RootPath, _currFileName));
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
