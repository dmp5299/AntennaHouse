using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AntennaHousePdf.Models;
using AntennaHouseBusinessLayer.Interfaces;
using AntennaHouseBusinessLayer.Library;
using AntennaHouseBusinessLayer.Factories;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Windows.Forms;
using AntennaHousePdf.Factories;
using Ionic.Zip;
using AntennaHouseBusinessLayer.XmlUtils;

namespace AntennaHousePdf.Library
{
    public class GetPdf
    {
        private AntennaPdf antennaPdf;
        private UploadGraphicFiles uploadGraphicFiles;
        private UploadXmlFiles uploadXmlFiles;
        private CreateDocument doc1;

        public GetPdf(AntennaPdf antennaPdf)
        {
            this.antennaPdf = antennaPdf;
            uploadGraphicFiles = new UploadGraphicFiles();
            uploadXmlFiles = new UploadXmlFiles();
            doc1 = new CreateDocument();
        }

        private void uploadFiles()
        {
            uploadXmlFiles.uploadFiles(antennaPdf.XmlFiles, "UserId", (antennaPdf.Project == "CMM"));
            if (antennaPdf.Graphics[0] != null)
            {
                uploadGraphicFiles.uploadFiles(antennaPdf.Graphics, "graphicFolder");
            }
            else
            {
                HttpContext.Current.Session.Remove("graphicFolder");
            }
        }

        public Boolean checkForElement(string xml, string element)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            using (StreamReader stream = new System.IO.StreamReader(xml, true))
            {
                using (XmlReader pm = XmlReader.Create(stream, settings))
                {
                    PropertyInfo propertyInfo = pm.GetType().GetProperty("DisableUndeclaredEntityCheck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    propertyInfo.SetValue(pm, true);
                    if (pm.ReadToFollowing(element))
                    {
                        return true;
                    }
                    return false;
                }
            }
            
        }

        public void fill53K(string xmlFile)
        {
            XmlPopulatorFactory factory = new XmlPopulatorFactory();
            if (checkForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "supequi"))
            {
                SupportEquipmentPopulator s = (SupportEquipmentPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.SupportEquipment,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                s.loopElements();
            }
            if (checkForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "supply"))
            {
                SuppliesPopulator supply = (SuppliesPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Supplies,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                supply.loopElements();
            }
            if (checkForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "warning"))
            {
                WarningPopulator warning = (WarningPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Warnings,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                warning.loopElements();
            }
            if (checkForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "caution"))
            {
                WarningPopulator warning = (WarningPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Cautions,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                warning.loopElements();
            }
        }

        public string findPmFile(List<HttpPostedFileBase> xmlFiles)
        {
            foreach (HttpPostedFileBase xFile in xmlFiles)
            {
                string[] arr = xFile.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                if (graphicFile.Contains("PM") || graphicFile.Contains("pm")) return HttpContext.Current.Session["UserId"] + "/" + graphicFile;
            }
            throw new FileNotFoundException("PM file not found. A PM file must be included.");
        }

        //Get names and locations of all dmodules and put them in DmFiles array
        public Boolean checkForDm(string pmFile, string dm)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Ignore;
            using (StreamReader stream = new System.IO.StreamReader(pmFile, true))
            {
                using (XmlReader pm = XmlReader.Create(stream, settings))
                {
                    PropertyInfo propertyInfo = pm.GetType().GetProperty("DisableUndeclaredEntityCheck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    propertyInfo.SetValue(pm, true);
                    while (pm.ReadToFollowing("dmCode"))
                    {
                        pm.MoveToAttribute("infoCode");
                        if (pm.Value == dm)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public PdfFile createDocument()
        {
            //upload xml files and graphics to server
            uploadFiles(); 
            //check xml files count. if its more than one you're doing a merge, if not create the pdf
            if (antennaPdf.XmlFiles.Count == 1)
            {
                string[] arr = antennaPdf.XmlFiles[0].FileName.Split('\\');
                string xmlFile = arr[arr.Length - 1];
                string subFoler = null;
                if (antennaPdf.Project == "Rolls Royce") { subFoler = antennaPdf.Volume; }
                else if (antennaPdf.Project == "CMM") { subFoler = antennaPdf.SubProject;System.Web.HttpContext.Current.Session["subProject"] = subFoler; }
                else if (antennaPdf.Project == "SB")
                {
                    subFoler = antennaPdf.SubProjectSB;
                    if (antennaPdf.SubProjectSB == "Airbus")
                    {
                        System.Web.HttpContext.Current.Session["sbFooter"] = antennaPdf.Footer;
                    }
                }
                if(antennaPdf.Project == "53K")
                {
                    Replace replacement = new Replace(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "<!NOTATION cgm SYSTEM>", "");
                    replacement.replaceContentText();
                    replacement = new Replace(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "encoding=\"UTF-16\"", "");
                    replacement.replaceContentText();
                    fill53K(xmlFile);
                    
                }
                byte[] doc = doc1.SaxonBuild(System.Web.HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, antennaPdf.Project, subFoler,
                checkForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "foldout"));
                var pdfDoc = doc;
                string[] xml = antennaPdf.XmlFiles[0].FileName.Split('\\');
                string xmlFile1 = xml[xml.Length - 1];
                xmlFile1 = xmlFile1.Replace(".xml", "");
                xmlFile1 = xmlFile1.Replace(".XML", "") + ".pdf";
                FileContentResult file = new FileContentResult(doc, "application/pdf");
                return new PdfFile { FileName = xmlFile1, PdfDoc = file };
            }
            else
            {
                string pmFile = findPmFile(antennaPdf.XmlFiles);
                if (!checkForElement(pmFile, "dmodule"))
                {
                    if (antennaPdf.Project == "CMM")
                    {
                        System.Web.HttpContext.Current.Session["subProject"] = antennaPdf.SubProject;
                        Factory factory = new Factory();
                        if (checkForDm(pmFile, "013"))
                        {
                            NumIndexDm num = (NumIndexDm)factory.GetDmClass(Factory.DmType.NumIndex, HttpContext.Current.Session["UserId"].ToString());

                            num.buildDmFile(HttpContext.Current.Session["UserId"].ToString());
                        }
                        else if(checkForDm(pmFile, "942"))
                        {
                            NumIndexDm4Point1 num = (NumIndexDm4Point1)factory.GetDmClass(Factory.DmType.NumIndex4Point1, HttpContext.Current.Session["UserId"].ToString());

                            num.buildDmFile(HttpContext.Current.Session["UserId"].ToString());
                        }
                        if (checkForDm(pmFile, "014"))
                        {
                            EquipDesignatorDm des = (EquipDesignatorDm)factory.GetDmClass(Factory.DmType.EquipmentDesignator, HttpContext.Current.Session["UserId"].ToString());
                            des.buildDmFile(HttpContext.Current.Session["UserId"].ToString());
                        }
                    }
                    MergePm.mergeFiles(HttpContext.Current.Session["UserId"].ToString(),pmFile);
                }
                else
                {
                    throw new ArgumentException("Pm is already merged. Don't select multiple XML files.");
                }
                byte[] doc = doc1.SaxonBuild(pmFile, antennaPdf.Project, antennaPdf.SubProject, checkForElement(pmFile, "foldout"));
                string[] xml = pmFile.Split('/');
                string xmlFile1 = xml[xml.Length - 1];
                xmlFile1 = xmlFile1.Replace(".xml", ".pdf");
                FileContentResult file = new FileContentResult(doc, "application/pdf");
                return new PdfFile { FileName = xmlFile1, PdfDoc = file };
            }
        }
    }
}