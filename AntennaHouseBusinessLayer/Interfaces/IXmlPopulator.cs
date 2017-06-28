using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AntennaHouseBusinessLayer.Tools;

namespace AntennaHouseBusinessLayer.Interfaces
{
    public interface IXmlPopulator
    {
        void loopElements();

        void populateElements(string id=null);

        IToolsAndWarnings getElementVars(string id=null);
    }
}
