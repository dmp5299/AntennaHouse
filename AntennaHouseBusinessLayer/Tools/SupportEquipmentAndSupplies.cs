using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;

namespace AntennaHouseBusinessLayer.Tools
{
    public class SupportEquipmentAndSupplies : IToolsAndWarnings
    {
        public string Nomen { get; set; }
        public string Mfc { get; set; }
        public string Toolnbr { get; set; }
    }
}
