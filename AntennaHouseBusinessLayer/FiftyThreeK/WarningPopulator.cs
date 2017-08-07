using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;
using AntennaHouseBusinessLayer.Tools;
using AntennaHouseBusinessLayer.XmlObjects;
using System.Xml;
using System.Configuration;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.FiftyThreeK
{
    public class WarningPopulator : IXmlPopulator
    {
        private string xmlFile;
        private XmlDocument doc;
        private Warning warning;
        private string type;

        public WarningPopulator(string xmlFile, Type type)
        {
            checkType(type);
            this.xmlFile = xmlFile;
            doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(xmlFile);
        }

        private void checkType(Type type)
        {
            switch(type){
                case Type.Warning:
                    this.type = "warning";
                        break;
                case Type.Caution:
                    this.type = "caution";
                    break;
                default:
                    throw new ArgumentException("Only accepted types for WarningPopulator are warning and caution");
            }
        }

        public void loopElements()
        {
            XmlNodeList warnings = doc.SelectNodes("/descendant::"+this.type);
            foreach (XmlNode s in warnings)
            {
                if(s.SelectSingleNode("descendant::refdm") != null) { 
                string warningFile = buildDmoduleIdentifier(s);
                populateWarning(warningFile,s);
                }
            }
        }

        private string buildDmoduleIdentifier(XmlNode warning)
        {
            XmlNode avee = warning.SelectSingleNode("descendant::avee");
            RefDm53K refDm = new RefDm53K
            {
                Modelic = avee.SelectSingleNode("modelic").InnerText,
                sdc = avee.SelectSingleNode("sdc").InnerText,
                chapnum = avee.SelectSingleNode("chapnum").InnerText,
                section = avee.SelectSingleNode("section").InnerText,
                subsect = avee.SelectSingleNode("subsect").InnerText,
                subject = avee.SelectSingleNode("subject").InnerText,
                discode = avee.SelectSingleNode("discode").InnerText,
                discodev = avee.SelectSingleNode("discodev").InnerText,
                incode = avee.SelectSingleNode("incode").InnerText,
                incodev = avee.SelectSingleNode("incodev").InnerText,
                itemloc = avee.SelectSingleNode("itemloc").InnerText,
            };
            return refDm.buildFileString();
        }

        public void populateWarning(string file,XmlNode warning)
        {
            XmlNode externalWarningNode = getWarning(ConfigurationManager.AppSettings["Warnings"] + file);
            XmlNode importNode = doc.ImportNode(externalWarningNode,true);
            XmlNode warningParent = warning.ParentNode;
            warningParent.InsertBefore(importNode, warning);
            warningParent.RemoveChild(warning);
            doc.Save(xmlFile);
        }

        public void populateElements(string id)
        {
            XmlNode externalWarningNode = getWarning(ConfigurationManager.AppSettings["Warnings"] + id);
            string infoName = externalWarningNode.SelectSingleNode("descendant::para[1]").InnerText;
            
            doc.ReplaceChild(externalWarningNode, doc.SelectSingleNode(String.Format("descendant::"+this.type+"[descendant::infoname='{0}']", infoName)));
            doc.Save(xmlFile);
        }

        public XmlNode getWarning(string warningFile)
        {
            XmlDocument warningDoc = new XmlDocument();
            warningDoc.Load(warningFile);
            XmlNode warning   = warningDoc.SelectSingleNode("/descendant::"+type+"[1]");
            return warning;
        }

        public IToolsAndWarnings getElementVars(string id)
        {
            XmlNode fakeNode = doc.CreateElement("fakeNode");
            return new Warning
            {
                WarningNode = fakeNode
            };
        }

        public enum Type{
            Caution,
            Warning
        }
    }
}
