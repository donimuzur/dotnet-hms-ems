using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using SimpleInjector;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.XMLReader;
using Sampoerna.EMS.Contract;
namespace Sampoerna.HMS.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class ReadXmlFile : IJob
    {
        private readonly Container _container;

        public ReadXmlFile(Container container)
        {
            _container = container;
        }
        public void Execute(IJobExecutionContext context)
        {
            using (_container.BeginLifetimeScope())
            {
                
                
                var loggerFactory = _container.GetInstance<ILoggerFactory>();
                ILogger logger = loggerFactory.GetLogger("Scheduler");

                try
                {
                    logger.Info("Reading XML Material start on " + DateTime.Now);
                    //IXmlDataReader xmlData = new XmlAreaDataMapper();
                    //IXmlDataReader xmlData = new XmlPoaDataMapper();
                    //IXmlDataReader xmlData = new XmlMarketDataMapper();
                    //IXmlDataReader xmlData = new XmlPlantDataMapper();
                    //IXmlDataReader xmlData = new XmlPoaMapDataMapper();
                    // IXmlDataReader xmlData = new XmlUserDataMapper();
                    // IXmlDataReader xmlData = new XmlSeriesDataMapper();
                    //IXmlDataReader xmlData = new XmlProdTypeDataMapper();
                    //IXmlDataReader xmlData = new XmlKPPBCDataMapper();
                    //IXmlDataReader xmlData = new XmlPCodeDataMapper();
                    //IXmlDataReader xmlData = new XmlGoodsTypeDataMapper();
                    IXmlDataReader xmlData = new XmlMaterialDataMapper();
                    xmlData.InsertToDatabase();
                    logger.Info("Reading XML Material ended On " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    logger.Error("Reading XML crashed", ex);
                }
              }
        }
    }
}
