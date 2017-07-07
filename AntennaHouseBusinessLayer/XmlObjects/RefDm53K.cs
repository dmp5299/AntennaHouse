using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;

namespace AntennaHouseBusinessLayer.XmlObjects
{
    public class RefDm53K : IRefDm
    {
        public string Modelic { get; set; }
        public string chapnum { get; set; }
        public string section { get; set; }
        public string subsect { get; set; }
        public string discode { get; set; }
        public string discodev { get; set; }
        public string incode { get; set; }
        public string incodev { get; set; }
        public string itemloc { get; set; }
        public string subject { get; set; }
        public string sdc { get; set; }

        public string buildFileString()
        {
            return "DMC-" + Modelic + "-A-" + chapnum + "-" + section + subsect + "-" + subject + "-" + discode + discodev + "-" + incode + incodev + "-" + itemloc + "_000.xml";
        }
    }
}
