using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.XmlUtils
{
    class References
    {
        public static void addReferences(string xmlFile, XmlDocument dmodule)
        {
            XmlNode refs = dmodule.CreateElement("refs");
            XmlNodeList dmrefs = dmodule.SelectNodes("descendant::dmRef[not(ancestor::brexDmRef) and not(ancestor::applicCrossRefTableRef)]");
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
            addRefsTag(refs, dmodule, xmlFile);
        }

        public static void addRefsTag(XmlNode refs, XmlDocument doc, string xmlFile)
        {
            if (refs.ChildNodes.Count == 0)
            {
                XmlNode noConds = doc.CreateElement("noConds");
                refs.AppendChild(noConds);
            }
            doc.SelectSingleNode("descendant::content").AppendChild(refs);
            doc.Save(xmlFile);
        }

        public static string buildDmString(XmlNode module)
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
