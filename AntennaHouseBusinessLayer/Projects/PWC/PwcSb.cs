using AntennaHouseBusinessLayer.FOUtils;
using AntennaHouseBusinessLayer.XmlUtils;
using AntennaHousePdf.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AntennaHouseBusinessLayer.Projects.PWC
{
    public class PwcSb
    {
        public static PdfFile buildPdf(string xmlFile, string project, string subProject = null)
        {
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
    }
}
