using AntennaHouseBusinessLayer.FOUtils;
using AntennaHouseBusinessLayer.XmlUtils;
using AntennaHousePdf.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows.Forms;
using System.Xml;

namespace AntennaHouseBusinessLayer.Projects.Acrolinx
{
    public class AcroLinx
    {
        public static PdfFile buildPdf(string[] xmlFiles, string project, string subProject)
        {
            System.Web.HttpContext.Current.Session["subProject"] = subProject;
            buildXmlFile(xmlFiles);
            byte[] pdfDoc = CreateDocument.SaxonBuild(System.Web.HttpContext.Current.Session["UserId"].ToString() + @"/acroResults.xml", 
                project, subProject);
            FileContentResult file = new FileContentResult(pdfDoc, "application/pdf");
            return new PdfFile { FileName = "acroResults.pdf", PdfDoc = file };
        }

        public static void buildXmlFile(string[] xmlFiles)
        {
            using (XmlWriter writer = XmlWriter.Create(System.Web.HttpContext.Current.Session["UserId"].ToString() + @"/acroResults.xml"))
            {
                writer.WriteStartElement("results");
                foreach (string file in xmlFiles)
                {
                    writer.WriteStartElement("module");
                    writer.WriteElementString("ata", getXmlFileName(file));
                    XmlDocument module = new XmlDocument();
                    module.Load(file);
                    string fileName = module.SelectSingleNode("descendant::identifier[parent::inputText]").Attributes["filename"].InnerText;
                    writer.WriteElementString("fileName", fileName);
                    int termCount = module.SelectNodes("descendant::termFlag[@type='terminology']").Count;
                    writer.WriteElementString("termCount", termCount.ToString());
                    int spellCount = module.SelectNodes("descendant::listOfSpellingFlags/spellingFlag").Count;
                    writer.WriteElementString("spellCount", spellCount.ToString());
                    int grammarCount = module.SelectNodes("descendant::grammar/listOfLangFlags/langFlag").Count;
                    writer.WriteElementString("grammarCount", grammarCount.ToString());
                    int styleCount = module.SelectNodes("descendant::style/listOfLangFlags/langFlag").Count;
                    writer.WriteElementString("styleCount", styleCount.ToString());/*
                    using (XmlReader reader = XmlReader.Create(file))
                    {
                        reader.ReadToFollowing("identifier");
                        string fileName = reader.GetAttribute("filename");
                        writer.WriteElementString("fileName", fileName);

                        reader.ReadToFollowing("spelling");
                        int spellCount = 0;
                        while (reader.ReadToFollowing("spellingFlag"))
                        {
                            spellCount++;
                        }
                        writer.WriteElementString("spellCount", spellCount.ToString());

                        reader.ReadToFollowing("grammar");
                        int grammarCount = 0;
                        while (reader.ReadToFollowing("langFlag"))
                        {
                            MessageBox.Show("gere");
                            grammarCount++;
                        }
                        writer.WriteElementString("grammarCount", grammarCount.ToString());

                        reader.ReadToFollowing("style");
                        int styleCount = 0;
                        while (reader.ReadToFollowing("langFlag"))
                        {
                            styleCount++;
                        }
                        writer.WriteElementString("styleCount", styleCount.ToString());

                        reader.ReadToFollowing("terminology");
                        int termCount = 0;
                        while (reader.ReadToFollowing("termFlag"))
                        {
                            termCount++;
                        }
                        writer.WriteElementString("termCount", termCount.ToString());

                    }*/
                        writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Flush();
            }
        }

        private static string getXmlFileName(string xmlFile)
        {
            string[] xml = xmlFile.Split('/');
            string xmlFile1 = xml[xml.Length - 1];
            xmlFile1 = xmlFile1.Replace(".xml", "");
            return xmlFile1.Replace(".XML", "");
        }
    }
}
