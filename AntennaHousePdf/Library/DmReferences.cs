using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using AntennaHousePdf.Models;

namespace AntennaHousePdf.Library
{
    public class DmReferences
    {
        public string Xml { get; set; }

        public DmReferences(string xml) { this.Xml = xml; }

        /*public string getReferences()
        {
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = new MyXmlUrlResolver();
            using (XmlReader reader = XmlReader.Create(Xml, settings))
            {

            }
        }*/
    }
}