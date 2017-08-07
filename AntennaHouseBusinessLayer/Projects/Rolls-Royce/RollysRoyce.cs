using AntennaHouseBusinessLayer.Factories;
using AntennaHouseBusinessLayer.FOUtils;
using AntennaHouseBusinessLayer.XmlUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AntennaHouseBusinessLayer.Projects.Rolls_Royce
{
    public class RollysRoyce
    {
        public static PdfFile buildPdf(string pmFile, string project, string subProject = null)
        {
            System.Web.HttpContext.Current.Session["subProject"] = subProject;
            Factory factory = new Factory();
            MergePm.mergeFiles(HttpContext.Current.Session["UserId"].ToString(), pmFile);
            byte[] pdfDoc = CreateDocument.SaxonBuild(pmFile, project, subProject, XmlOperations.CheckForElement(pmFile, "foldout"));
            string[] xml = pmFile.Split('/');
            string xmlFile1 = xml[xml.Length - 1];
            xmlFile1 = xmlFile1.Replace(".xml", "");
            xmlFile1 = xmlFile1.Replace(".XML", "") + ".pdf";
            FileContentResult file = new FileContentResult(pdfDoc, "application/pdf");
            return new PdfFile { FileName = xmlFile1, PdfDoc = file };
        }
    }
}
