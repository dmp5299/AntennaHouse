using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;
using AntennaHouseBusinessLayer.Tools;
using System.Xml;
using System.Configuration;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.Library
{
    public class SuppliesPopulator : IXmlPopulator
    {
        private string xmlFile;
        private XmlDocument doc;
        private SupportEquipmentAndSupplies supplies;

        public SuppliesPopulator(string xmlFile)
        {
            this.xmlFile = xmlFile;
            doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(xmlFile);
        }

        public void loopElements()
        {
            XmlNodeList supplies = doc.SelectNodes("/descendant::supply");
            foreach (XmlNode s in supplies)
            {
                string id = s.Attributes["id"].InnerText;
                this.supplies = (SupportEquipmentAndSupplies)getElementVars(id);
                if (this.supplies != null) 
                    {
                        populateElements(id);
                    }
            }
        }

        public void populateElements(string id)
        {
            if(doc.SelectSingleNode(String.Format("descendant::supply[@id='{0}']/descendant::nomen", id)) != null)
            {
                doc.SelectSingleNode(String.Format("descendant::supply[@id='{0}']/descendant::nomen", id)).InnerText = supplies.Nomen;
            }
            else
            {

                XmlNode nomen = doc.CreateElement("nomen");
                nomen.InnerText = supplies.Nomen;
                doc.SelectSingleNode(String.Format("descendant::supply[@id='{0}']", id)).AppendChild(nomen);
            }
            if (doc.SelectSingleNode(String.Format("descendant::supply[@id='{0}']/descendant::pnr", id))!=null)
            {
                doc.SelectSingleNode(String.Format("descendant::supply[@id='{0}']/descendant::pnr", id)).InnerText = supplies.Toolnbr;
            }
            else
            {
                XmlNode pnr = doc.CreateElement("pnr");
                pnr.InnerText = supplies.Toolnbr;
                doc.SelectSingleNode(String.Format("descendant::supply[@id='{0}']", id)).AppendChild(pnr);
            }
            doc.Save(xmlFile);
        }

        public IToolsAndWarnings getElementVars(string id)
        {
            XmlDocument suppDoc = new XmlDocument();
            suppDoc.Load(ConfigurationManager.AppSettings["Supplies"]);
            XmlNode s = suppDoc.SelectSingleNode(String.Format("descendant::conitem[@id='{0}']", id));
            if (s != null)
            {
                SupportEquipmentAndSupplies support = new SupportEquipmentAndSupplies
                {
                    Nomen = s.SelectSingleNode("descendant::nomen").InnerText,
                    Mfc = "nothing",
                    Toolnbr = s.SelectSingleNode("descendant::conitemid").Attributes["itemnbr"].InnerText
                }; return support;
            }
            else
            {
                return null;
            }
            
        }
    }
}
