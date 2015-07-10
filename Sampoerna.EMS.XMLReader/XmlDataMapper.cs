﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;
using Sampoerna.EMS.BusinessObject;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebCompoments.NLogLogger;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlDataMapper
    {
        public XElement _xmlData = null;
        public string _xmlName = null;
        public ILogger logger;
        public IUnitOfWork uow;
        public XmlDataMapper(string xmlName)
        {
            
            _xmlName = xmlName;
            _xmlData = ReadXMLFile();
            logger = new NLogLogger();
            uow = new SqlUnitOfWork(logger);
          
        }


        private XElement ReadXMLFile()
        {

            if (_xmlName == null)
                return null;
            if (!File.Exists(_xmlName))
                return null;
            return XElement.Load(_xmlName);
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
       
        public void InsertToDatabase<T>(List<T> items) where T : class
        {
            var repo = uow.GetGenericRepository<T>();
            
            try
            {
                foreach (var item in items)
                {
                   
                    repo.InsertOrUpdate(item);
                    uow.SaveChanges();
                }
                
            }
            
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                uow.RevertChanges();
            }
         
            MoveFile();
        }
        public void InsertToDatabase<T>(T data) where T : class
        {
            var repo = uow.GetGenericRepository<T>();

            try
            {
                
                    repo.InsertOrUpdate(data);

                
                uow.SaveChanges();
            }

            catch (Exception ex)
            {
                logger.Error(ex.Message);
                uow.RevertChanges();
            }

            MoveFile();
        }
        private void MoveFile()
        {
            try
            {
                string sourcePath = _xmlName;
                string archievePath = ConfigurationManager.AppSettings["XmlArchievePath"];
                var sourcefileName = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(archievePath, sourcefileName);
                if (File.Exists(destPath))
                    return;

                File.Move(sourcePath, destPath);
            }
            catch (Exception ex)
            {
                
                throw;
            }
          
        }


    }
}
