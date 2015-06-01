using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlAreaDataMapper : IXmlDataReader 
    {
        private XElement _xmData = null;
        private XmlDataMapper _xmlMapper = null;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public XmlAreaDataMapper()
        {
            _logger = new NullLogger();
            _uow = new SqlUnitOfWork(_logger);
            _xmlMapper = new XmlDataMapper("T1001K");
           
        }

        
        public List<T1001K> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T1001K>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T1001K();
                    item.BWKEY = xElement.Element("BWKEY").Value;
                    var companyCode  = xElement.Element("BUKRS").Value;
                    var companyById =
                        _uow.GetGenericRepository<T1001>()
                            .Get(p => p.BUKRS == companyCode)
                            .OrderByDescending(p => p.CREATED_DATE)
                            .FirstOrDefault();

                    item.CREATED_DATE = DateTime.Now;
                    if (companyById != null)
                    {
                        item.COMPANY_ID = companyById.COMPANY_ID;
                        items.Add(item);
                    }
                   
                }
                return items;
            }
             
        }


        public void InsertToDatabase()
        {
            var repo = _uow.GetGenericRepository<T1001K>();

            try
            {
                foreach (var item in Items)
                {
                    repo.Insert(item);

                }
            }
            catch (Exception ex)
            {
                _uow.RevertChanges();
            } 
            _uow.SaveChanges();
       
        }





    }
}
