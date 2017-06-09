using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace AntennaHouseBusinessLayer.Library
{
    public class MergePm
    {
        public string PmFile { get; set; }
        XmlDocument doc;
        string modelIdentCode, disassyCodeVariant, systemCode, subSystemCode,
                       subSubSystemCode, assyCode, disassyCode, infoCode, infoCodeVariant, itemLocationCode,
                       issueNumber, inWork, languageIsoCode, countryIsoCode, systemDiffCode;

        public MergePm(){}

        public void mergeFiles(string xmlFolder)
        {
            string[] xmlFiles = Directory.GetFiles(xmlFolder, "DM*");
            for(int i = 0; i < xmlFiles.Length; i++)
            {
                xmlFiles[i] = xmlFiles[i].Replace(@"\","/");
            }
            XmlNodeList dmRefs = getDmRefs();
            insertModules(dmRefs, xmlFiles, xmlFolder);
        }
        
        private XmlNodeList getDmRefs()
        {
            XmlNodeList refs;
            doc = new XmlDocument();
            doc.Load(PmFile);
            //set date
            PmDate pm = new PmDate(doc, PmFile);
            pm.setDate();
            try
            {
                refs = doc.SelectNodes("descendant::dmRef");
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

        private void insertModules(XmlNodeList dmRefs, string[] xmlFiles, string xmlFolder)
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
                    References references = new References(dmodule, xmlFolder + "/" + file);
                    references.addReferences();
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
                doc.Save(PmFile);
            }
        }

        public void addAttsToDmodule(XmlNode dmodule, XmlNode dmRef, XmlDocument doc, string xml)
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

        private string buildDMString(XmlNode dmRef)
        {
            XmlNode dmCode = dmRef.SelectSingleNode("descendant::dmCode");
            XmlAttributeCollection attributes = dmCode.Attributes;
            loopAttributes(attributes);
            return "DMC-" + modelIdentCode + "-" + systemDiffCode + "-" + systemCode + "-" + subSystemCode + subSubSystemCode + "-" + assyCode + "-" + disassyCode
                   + disassyCodeVariant + "-" + infoCode + infoCodeVariant + "-" +
                  itemLocationCode + ".xml";
        }

        private void assignAttribute(XmlAttribute attribute)
        {
            switch (attribute.Name)
            {
                case "modelIdentCode":
                    modelIdentCode = attribute.InnerText;
                    break;
                case "disassyCodeVariant":
                    disassyCodeVariant = attribute.InnerText;
                    break;
                case "systemCode":
                    systemCode = attribute.InnerText;
                    break;
                case "subSystemCode":
                    subSystemCode = attribute.InnerText;
                    break;
                case "subSubSystemCode":
                    subSubSystemCode = attribute.InnerText;
                    break;
                case "systemDiffCode":
                    systemDiffCode = attribute.InnerText;
                    break;
                case "assyCode":
                    assyCode = attribute.InnerText;
                    break;
                case "disassyCode":
                    disassyCode = attribute.InnerText;
                    break;
                case "infoCode":
                    infoCode = attribute.InnerText;
                    break;
                case "infoCodeVariant":
                    infoCodeVariant = attribute.InnerText;
                    break;
                case "itemLocationCode":
                    itemLocationCode = attribute.InnerText;
                    break;
                case "issueNumber":
                    issueNumber = attribute.InnerText;
                    break;
                case "inWork":
                    inWork = attribute.InnerText;
                    break;
                case "languageIsoCode":
                    languageIsoCode = attribute.InnerText;
                    break;
                case "countryIsoCode":
                    countryIsoCode = attribute.InnerText;
                    break;
            }
        }

        private void loopAttributes(XmlAttributeCollection attributes)
        {

            foreach (XmlAttribute attribute in attributes)
            {
                assignAttribute(attribute);
            }
        }
    }
}