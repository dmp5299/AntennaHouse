using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;
using System.Xml;

namespace AntennaHouseBusinessLayer.Tools
{
    public class Warning : IToolsAndWarnings
    {
        public XmlNode WarningNode { get; set; }
    }

    
}
