using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;
using System.Security.Permissions;
using Saxon.Api;
using System.Configuration;
using XfoDotNetCtl;
using System.Xml;
using System.Collections;
using AntennaHouseBusinessLayer.XmlUtils;
using System.IO;
using AntennaHouseBusinessLayer.Factories;
using AntennaHouseBusinessLayer.DataModuleCreation;
using System.Web;
using AntennaHouseBusinessLayer.FOUtils;
using System.Web.Mvc;
using System.Windows.Forms;
using AntennaHouseBusinessLayer.FileUtils;
using AntennaHouseBusinessLayer.FiftyThreeK;
using AntennaHousePdf.FileUtils;

namespace AntennaHouseBusinessLayer.Projects.FiftyThreeK
{
    public class FiftyThreeK
    {
        public static PdfFile buildPdf(string xmlFile, string project, string subProject = null)
        {
            System.Web.HttpContext.Current.Session["subProject"] = subProject;
            Replace.replaceContentText(xmlFile, "<!NOTATION cgm SYSTEM>", "");
            Replace.replaceContentText(xmlFile, "encoding=\"UTF-16\"", "");
            fill53K(xmlFile);
            byte[] pdfDoc = CreateDocument.SaxonBuild(xmlFile, project, subProject, XmlOperations.CheckForElement(xmlFile, "foldout"));
            string[] xml = xmlFile.Split('/');
            string xmlFile1 = xml[xml.Length - 1];
            xmlFile1 = xmlFile1.Replace(".xml", "");
            xmlFile1 = xmlFile1.Replace(".XML", "") + ".pdf";
            FileContentResult file = new FileContentResult(pdfDoc, "application/pdf");
            return new PdfFile { FileName = xmlFile1, PdfDoc = file };
        }

        public static void fill53K(string xmlFile)
        {
            XmlPopulatorFactory factory = new XmlPopulatorFactory();
            if (XmlOperations.CheckForElement(xmlFile, "supequi"))
            {
                SupportEquipmentPopulator s = (SupportEquipmentPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.SupportEquipment,
                xmlFile);
                s.loopElements();
            }
            if (XmlOperations.CheckForElement(xmlFile, "supply"))
            {
                SuppliesPopulator supply = (SuppliesPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Supplies,
                xmlFile);
                supply.loopElements();
            }
            if (XmlOperations.CheckForElement(xmlFile, "warning"))
            {
                WarningPopulator warning = (WarningPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Warnings,
                xmlFile);
                warning.loopElements();
            }
            if (XmlOperations.CheckForElement(xmlFile, "caution"))
            {
                WarningPopulator warning = (WarningPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Cautions,
                xmlFile);
                warning.loopElements();
            }
        }
    }
}
