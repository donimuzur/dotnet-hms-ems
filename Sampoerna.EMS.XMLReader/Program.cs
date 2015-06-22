using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sampoerna.EMS.Contract;

namespace Sampoerna.EMS.XMLReader
{
    class Program
    {
        
         static void Main(string[] args)
        {

            Service svc = new Service();
            svc.PoaRunning();
            svc.PoaMapRunning();
            svc.CompanyRunning();
            svc.KPPBCRunning();
            svc.NPPBKCRunning();
            svc.VendorRunning();
             svc.PCodeRunning();
             svc.PlantRunning();
             svc.MarketRunning();
             svc.GoodTypeRunning();
             svc.UoMRunning();
             svc.ProdTypeRunning();
             svc.SeriesRunning();
             svc.BrandRunning();
             svc.MaterialRunning();

            Console.ReadLine();
        }
    }
}
