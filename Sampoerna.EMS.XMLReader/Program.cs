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
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.XMLReader
{
    class Program
    {
        
         static void Main(string[] args)
        {

            Service svc = new Service();
             svc.Run();
          
             Console.ReadLine();
        }
    }
}
