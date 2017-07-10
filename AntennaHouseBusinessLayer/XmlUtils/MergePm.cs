using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using AntennaHouseBusinessLayer.XmlObjects;
using AntennaHouseBusinessLayer.FileUtils;

namespace AntennaHouseBusinessLayer.XmlUtils
{
    public class MergePm
    { 
        public static void mergeFiles(string xmlFolder, string pmFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pmFile);
            string[] xmlFiles = Directory.GetFiles(xmlFolder, "DM*");
            for(int i = 0; i < xmlFiles.Length; i++)
            {
                if (xmlFiles[i].Contains('_'))
                {
                    xmlFiles[i] = ChangeFileName.changeNames(xmlFiles[i], xmlFiles[i].IndexOf('_'), 4, "");
                }
                xmlFiles[i] = xmlFiles[i].Replace(@"\","/");
                xmlFiles[i] = xmlFiles[i].Replace(@".XML", ".xml");                
            }
            XmlNodeList dmRefs = getDmRefs(pmFile, doc);
            insertModules(dmRefs, xmlFiles, xmlFolder, doc, pmFile);
        }
        
        private static XmlNodeList getDmRefs(string pmFile, XmlDocument doc)
        {
            XmlNodeList refs;
            doc.Load(pmFile);
            //set date
            PmDate pm = new PmDate(doc, pmFile);
            pm.setDate();
            try
            {
                refs = doc.SelectNodes("descendant::dmRef[not(ancestor::pmStatus)]");
                if (refs.Count == 0)
                {
                    throw new XmlException("There are no dmRefs in this document. Select a proper PM file and try again.");
                }
                return refs;
            }
            catch (XmlException)
            {
                throw;
            }
        }

        private static void insertModules(XmlNodeList dmRefs, string[] xmlFiles, string xmlFolder, XmlDocument doc, string pmFile)
        {
            List<string> files = new List<string>();
            string file = null;
            foreach (XmlNode dmRef in dmRefs)
            {
               
                file = buildDMString(dmRef);
                int pos = Array.IndexOf(xmlFiles, xmlFolder + "/" + file);
                if(pos > -1)
                {
                    XmlDocument dmodule = new XmlDocument();
                    dmodule.XmlResolver = new MyXmlUrlResolver();
                    dmodule.Load(xmlFolder + "/" + file);
                    if(dmodule.SelectSingleNode("descendant::refs") != null)
                    {
                        XmlNode refs = dmodule.SelectSingleNode("descendant::refs");
                        dmodule.SelectSingleNode("descendant::content").RemoveChild(refs);
                    }
                    References.addReferences(xmlFolder + "/" + file, dmodule);
                    XmlNode root = dmodule.DocumentElement;
                    root.Attributes.RemoveAll();
                    XmlNode dmCode = dmodule.SelectSingleNode("descendant::dmCode[1]");
                    addAttsToDmodule(root, dmCode, dmodule, xmlFolder + "/" + file);
                    XmlNode importNode = doc.ImportNode(root, true);
                    XmlNode parent = dmRef.ParentNode;
                    parent.InsertBefore(importNode, dmRef);
                    parent.RemoveChild(dmRef);
                }
                else
                {
                    throw new InvalidOperationException("Xml file " + file + " is not present. Add this module and try again");
                }
                doc.Save(pmFile);
            }
        }

        public static void addAttsToDmodule(XmlNode dmodule, XmlNode dmRef, XmlDocument doc, string xml)
        {
            XmlNode issueInfo = dmRef.SelectSingleNode("following-sibling::issueInfo");
            XmlNode language = dmRef.SelectSingleNode("following-sibling::language");
            dmodule.Attributes.Append(dmRef.Attributes["modelIdentCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["disassyCodeVariant"]);
            dmodule.Attributes.Append(dmRef.Attributes["systemCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["subSystemCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["subSubSystemCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["systemDiffCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["assyCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["disassyCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["infoCode"]);
            dmodule.Attributes.Append(dmRef.Attributes["infoCodeVariant"]);
            dmodule.Attributes.Append(dmRef.Attributes["itemLocationCode"]);
            dmodule.Attributes.Append(issueInfo.Attributes["issueNumber"]);
            dmodule.Attributes.Append(issueInfo.Attributes["inWork"]);
            dmodule.Attributes.Append(language.Attributes["languageIsoCode"]);
            dmodule.Attributes.Append(language.Attributes["countryIsoCode"]);
            doc.Save(xml);
        }

        private static string buildDMString(XmlNode dmRef)
        {
            XmlNode dmCode = dmRef.SelectSingleNode("descendant::dmCode");
            XmlAttributeCollection attributes = dmCode.Attributes;
            return loopAttributes(attributes).buildFileString();
        }

        private static void assignAttribute(XmlAttribute attribute, ref RefDm refDm)
        {
            switch (attribute.Name)
            {
                case "modelIdentCode":
                    refDm.ModelIdentCode = attribute.InnerText;
                    break;
                case "disassyCodeVariant":
                    refDm.DisassyCodeVariant = attribute.InnerText;
                    break;
                case "systemCode":
                    refDm.SystemCode = attribute.InnerText;
                    break;
                case "subSystemCode":
                    refDm.SubSystemCode = attribute.InnerText;
                    break;
                case "subSubSystemCode":
                    refDm.SubSubSystemCode = attribute.InnerText;
                    break;
                case "systemDiffCode":
                    refDm.SystemDiffCode = attribute.InnerText;
                    break;
                case "assyCode":
                    refDm.AssyCode = attribute.InnerText;
                    break;
                case "disassyCode":
                    refDm.DisassyCode = attribute.InnerText;
                    break;
                case "infoCode":
                    refDm.InfoCode = attribute.InnerText;
                    break;
                case "infoCodeVariant":
                    refDm.InfoCodeVariant = attribute.InnerText;
                    break;
                case "itemLocationCode":
                    refDm.ItemLocationCode = attribute.InnerText;
                    break;
                case "issueNumber":
                    refDm.IssueNumber = attribute.InnerText;
                    break;
                case "inWork":
                    refDm.InWork = attribute.InnerText;
                    break;
                case "languageIsoCode":
                    refDm.LanguageIsoCode = attribute.InnerText;
                    break;
                case "countryIsoCode":
                    refDm.CountryIsoCode = attribute.InnerText;
                    break;
            }
        }

        private static RefDm loopAttributes(XmlAttributeCollection attributes)
        {
            RefDm refDm = new RefDm();
            foreach (XmlAttribute attribute in attributes)
            {
                assignAttribute(attribute, ref refDm);
            }
            return refDm;
        }
    }
}