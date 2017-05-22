using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AntennaHouseBusinessLayer.Interfaces
{
    public interface IBuildDm
    {
        void buildDmFile(string xmlFolder);
        List<XmlNode> getNodes();
    }
}