using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AntennaHousePdf.Interfaces
{
    public interface IBuildDm
    {
        void buildDmFile(string xmlFolder);
        List<XmlNode> getNodes();
    }
}