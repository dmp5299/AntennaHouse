using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;

namespace AntennaHouseBusinessLayer.XmlObjects
{
    public class RefDm : IRefDm
    {
        public string ModelIdentCode { get; set; }
        public string DisassyCodeVariant { get; set; }
        public string SystemCode { get; set; }
        public string SubSystemCode { get; set; }
        public string SubSubSystemCode { get; set; }
        public string AssyCode { get; set; }
        public string DisassyCode { get; set; }
        public string InfoCode { get; set; }
        public string InfoCodeVariant { get; set; }
        public string ItemLocationCode { get; set; }
        public string ItemissueNumberloc { get; set; }
        public string InWork { get; set; }
        public string LanguageIsoCode { get; set; }
        public string CountryIsoCode { get; set; }
        public string SystemDiffCode { get; set; }
        public string IssueNumber { get; set; }

        public string buildFileString()
        {
            return "DMC-" + ModelIdentCode + "-" + SystemDiffCode + "-" + SystemCode + "-" + SubSystemCode + SubSubSystemCode + "-" + AssyCode + "-" + DisassyCode
                   + DisassyCodeVariant + "-" + InfoCode + InfoCodeVariant + "-" +
                  ItemLocationCode + ".xml";
        }
    }
}
