using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace AntennaHouseBusinessLayer.Library
{
    public class Foldouts
    {
        public string Xml { get; }
        public string StyleSheet { get; }

        public Foldouts(string xml, string styleSheet)
        {
            this.Xml = xml;
            this.StyleSheet = styleSheet;
        }

        public void buildLandscapeFo()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(StyleSheet);
            xslt.Transform(Xml, "c:/AntennaHouse/test.fo");
        }
    }
}