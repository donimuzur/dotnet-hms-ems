using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlDataMapper
    {
        private const string RootPath = @"D:/";
        public XElement _xmlData = null;
        private string _xmlName = null;
        public XmlDataMapper(string xmlName)
        {
            _xmlName = xmlName;
            _xmlData = ReadXMLFile();

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
    }
}
