using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sgml;
using System.Xml;
using System.IO;
using AntennaHouseBusinessLayer.XmlObjects;
using System.Text;
using System.Web.Mvc;
using System.Windows.Forms;
using Ionic.Zip;

namespace AntennaHousePdf.Models
{
    public class SgmlFile
    {
        public List<HttpPostedFileBase> SgmlFiles { get; set; }

        public static ConvertedXmlFile convertToXml(string xml)
        {
            TextReader reader = new StreamReader(xml);
            Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
            sgmlReader.DocType = "hs-cmm";
            sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
            sgmlReader.CaseFolding = Sgml.CaseFolding.ToLower;
            sgmlReader.InputStream = reader;
            sgmlReader.StripDocType = false;
           // create document
           XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(sgmlReader);
            byte[] byteDoc = Encoding.Default.GetBytes(doc.OuterXml);
            string xmlStringWithDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            string xmlString = Encoding.Default.GetString(byteDoc);
            xmlStringWithDeclaration += xmlString;
            string[] xmlParts = xml.Split('\\','/');
            string xmlFile1 = xmlParts[xmlParts.Length - 1];
            xmlFile1 = xmlFile1.Replace(".sgm", ".xml");
            return new ConvertedXmlFile
            {
                FileName = xmlFile1,
                XmlDoc = new FileContentResult(Encoding.UTF8.GetBytes(xmlStringWithDeclaration.Replace("\r\n", string.Empty)), "text/xml")
            };
        }

        public static MemoryStream buildZipFile(string[] fileEntries)
        {
            using (ZipFile zip = new ZipFile())
            {
                foreach (string fileEntry in fileEntries)
                {
                    ConvertedXmlFile doc = convertToXml(fileEntry);
                    string[] xml = fileEntry.Split('\\');
                    string xmlFile1 = xml[xml.Length - 1];
                    xmlFile1 = xmlFile1.Replace(".sgm", ".xml");
                    zip.AddEntry(xmlFile1, doc.XmlDoc.FileContents);
                }
                var memStream = new MemoryStream();
                zip.Save(memStream);
                memStream.Position = 0;
                return memStream;
            }
        }
    }
}