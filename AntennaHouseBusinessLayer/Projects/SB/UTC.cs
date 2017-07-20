using AntennaHouseBusinessLayer.FOUtils;
using AntennaHouseBusinessLayer.XmlUtils;
using AntennaHousePdf.FileUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ionic.Zip;

namespace AntennaHouseBusinessLayer.Projects.SB
{
    public class UTC
    {
        public static PdfFile buildPdf(string xmlFile, string project, string subProject, string footer = null)
        {
            if (subProject == "Airbus")
            {
                System.Web.HttpContext.Current.Session["sbFooter"] = footer;
            }
            System.Web.HttpContext.Current.Session["subProject"] = subProject;
            Replace.replaceContentText(xmlFile, "<!NOTATION cgm SYSTEM>", "");
            Replace.replaceContentText(xmlFile, "encoding=\"UTF-16\"", "");
            byte[] pdfDoc = CreateDocument.SaxonBuild(xmlFile, project, subProject, XmlOperations.CheckForElement(xmlFile, "foldout"));
            string[] xml = xmlFile.Split('/');
            string xmlFile1 = xml[xml.Length - 1];
            xmlFile1 = xmlFile1.Replace(".xml", "");
            xmlFile1 = xmlFile1.Replace(".XML", "") + ".pdf";
            FileContentResult file = new FileContentResult(pdfDoc, "application/pdf");
            return new PdfFile { FileName = xmlFile1, PdfDoc = file };
        }
        
        public static MemoryStream buildZipFile(string[] fileEntries, string project, string subProject, string footer = null)
        {
            using (ZipFile zip = new ZipFile())
            {
                foreach (string fileEntry in fileEntries)
                {
                    PdfFile doc = buildPdf(fileEntry, project, subProject, footer);
                    string[] xml = fileEntry.Split('\\');
                    string xmlFile1 = xml[xml.Length - 1];
                    xmlFile1 = xmlFile1.Replace(".XML", ".pdf");
                    zip.AddEntry(xmlFile1.Replace(".xml", ".pdf"), doc.PdfDoc.FileContents);
                }
                var memStream = new MemoryStream();
                zip.Save(memStream);
                memStream.Position = 0;
                return memStream;
            }
        }
    }
}
