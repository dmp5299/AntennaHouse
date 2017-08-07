using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AntennaHouseBusinessLayer.XmlObjects
{
    public class ConvertedXmlFile
    {
        public string FileName { get; set; }
        public FileContentResult XmlDoc { get; set; }
    }
}
