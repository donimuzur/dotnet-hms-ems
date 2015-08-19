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
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.XMLReader
{
    class Program
    {
        
         static void Main(string[] args)
        {

            Service svc = new Service();
             //svc.Run();
            XmlCK5DataWriter rt= new XmlCK5DataWriter();
             var ck5 = new CK5();
             ck5.REGISTRATION_NUMBER = "8100000011";
             ck5.CK5_TYPE = Enums.CK5Type.Domestic;
             rt.CreateXML(ck5);
             Console.ReadLine();
        }
    }
}
