using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.Library
{
    class References
    {
        private XmlDocument doc;
        private XmlNode refs;
        private string xmlFile;

        public References(XmlDocument dmodule, string xmlFile)
        {
            this.xmlFile = xmlFile;
            doc = dmodule;
            refs = doc.CreateElement("refs");
        }

        public void addReferences()
        {
            XmlNodeList dmrefs = doc.SelectNodes("descendant::dmRef[not(ancestor::brexDmRef) and not(ancestor::applicCrossRefTableRef)]");
            List<string> refStrings = new List<string>();
            List<XmlNode> noDuplicates = new List<XmlNode>();
            foreach (XmlNode d in dmrefs)
            {
                XmlNode clone = d.CloneNode(true);
                string moduleString = buildDmString(d);
                if (!refStrings.Contains(moduleString))
                {
                    refStrings.Add(moduleString);
                    refs.AppendChild(clone);
                }
            }
            addRefsTag();
        }

        public void addRefsTag()
        {
            if (refs.ChildNodes.Count == 0)
            {
                XmlNode noConds = doc.CreateElement("noConds");
                refs.AppendChild(noConds);
            }
            doc.SelectSingleNode("descendant::content").AppendChild(refs);
            doc.Save(xmlFile);
        }

        public string buildDmString(XmlNode module)
        {
            string moduleString = "";
            XmlNode dmCode = module.SelectSingleNode("descendant::dmCode");
            XmlAttributeCollection attributes = dmCode.Attributes;
            foreach(XmlAttribute a in attributes){
                moduleString += a.InnerText;
            }
            return moduleString;
        }
    }
}
