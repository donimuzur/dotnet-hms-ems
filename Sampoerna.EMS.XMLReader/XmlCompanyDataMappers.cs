using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebComponents.Logger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlCompanyDataMapper : IXmlDataReader 
    {
        //private XElement _xmData = null;
        private XmlDataMapper _xmlMapper = null;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public XmlCompanyDataMapper()
        {
            _logger = new NullLogger();
            _uow = new SqlUnitOfWork(_logger);
            _xmlMapper = new XmlDataMapper("Company");
           
        }

        
        public List<T1001> Items
        {
            get
            {
                var xmlItems = _xmlMapper.GetElements("ITEM");
                var items = new List<T1001>();
                foreach (var xElement in xmlItems)
                {
                    var item = new T1001();
                    item.BUKRS = xElement.Element("BUKRS").Value;
                    item.BUKRSTXT = xElement.Element("BUKRSTXT").Value;
                    item.CREATED_DATE = DateTime.Now;
                    if (IsDataChanges(item))
                    {
                        items.Add(item);
                    }
                   
                }
                return items;
            }
             
        }

        private bool IsDataChanges(T1001 item)
        {
            var repo = _uow.GetGenericRepository<T1001>();
            var data = repo.Get(x => x.BUKRS.Equals(item.BUKRS, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x=>x.CREATED_DATE).LastOrDefault();
            if (data == null)
                return true;
            if (!item.BUKRSTXT.Equals(data.BUKRSTXT))
            {
                return true;
            }
           
            return false;
        }

        public void InsertToDatabase()
        {
            var repo = _uow.GetGenericRepository<T1001>();

            try
            {
                foreach (var item in Items)
                {
                    repo.Insert(item);

                }
            }
            catch //(Exception ex)
            {
                _uow.RevertChanges();
            } 
            _uow.SaveChanges();
       
        }





    }
}
