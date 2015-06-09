using System;
using System.Collections.Generic;
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
            var timer = new Stopwatch();
            timer.Start();
            //IXmlDataReader xmlData = new XmlAreaDataMapper();
            //IXmlDataReader xmlData = new XmlPoaDataMapper();
            //IXmlDataReader xmlData = new XmlMarketDataMapper();
             //IXmlDataReader xmlData = new XmlPlantDataMapper();
            IXmlDataReader xmlData = new XmlPoaMapDataMapper();
            xmlData.InsertToDatabase();
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
