using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AntennaHouseBusinessLayer.Interfaces;
using AntennaHouseBusinessLayer.Tools;
using System.Configuration;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.Library
{
    public class SupportEquipmentPopulator: IXmlPopulator
    {
        private string xmlFile;
        private XmlDocument doc;
        private SupportEquipmentAndSupplies supportEquipment;

        public SupportEquipmentPopulator(string xmlFile)
        {
            this.xmlFile = xmlFile;
            doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(xmlFile);
        }

        public void loopElements()
        {
            XmlNodeList supportEquipment = doc.SelectNodes("/descendant::supequi");
            foreach (XmlNode s in supportEquipment)
            {
                string id = s.Attributes["id"].InnerText;
                this.supportEquipment = (SupportEquipmentAndSupplies)getElementVars(id);
                if (this.supportEquipment != null) { populateElements(id); }
                    
            }
        }

        public void populateElements(string id)
        {
            if(doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']/descendant::nomen", id))!=null)
            {
                doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']/descendant::nomen", id)).InnerText = supportEquipment.Nomen;
            }
           else
            {
                XmlNode nomen = doc.CreateElement("nomen");
                nomen.InnerText = supportEquipment.Nomen;
                doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']", id)).AppendChild(nomen);
            }
            if (doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']/descendant::mfc", id)) != null)
            {
                doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']/descendant::mfc", id)).InnerText = supportEquipment.Mfc;
            }
            else
            {
                XmlNode mfc = doc.CreateElement("mfc");
                mfc.InnerText = supportEquipment.Mfc;
                doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']", id)).AppendChild(mfc);
            }
            if (doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']/descendant::pnr", id)) != null)
            {
                doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']/descendant::pnr", id)).InnerText = supportEquipment.Toolnbr;
            }
            else
            {
                XmlNode pnr = doc.CreateElement("pnr");
                pnr.InnerText = supportEquipment.Toolnbr;
                doc.SelectSingleNode(String.Format("descendant::supequi[@id='{0}']", id)).AppendChild(pnr);
            }
            doc.Save(xmlFile);
        }


        public IToolsAndWarnings getElementVars(string id)
        {
            XmlDocument suppDoc = new XmlDocument();
            suppDoc.Load(ConfigurationManager.AppSettings["SupportEquipment"]);
            XmlNode s = suppDoc.SelectSingleNode(String.Format("descendant::toolinfo[@id='{0}']",id));
            if (s != null)
            {
                SupportEquipmentAndSupplies support = new SupportEquipmentAndSupplies
                {
                    Nomen = s.SelectSingleNode("descendant::nomen").InnerText,
                    Mfc = s.SelectSingleNode("descendant::toolid").Attributes["mfc"].InnerText,
                    Toolnbr = s.SelectSingleNode("descendant::toolid").Attributes["toolnbr"].InnerText
                }; return support;
            }
            else
            {
                return null;
            }
           
        }
    }
}
