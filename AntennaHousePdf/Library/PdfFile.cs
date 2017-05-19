using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AntennaHousePdf.Library
{
    public class PdfFile
    {
        public string FileName { get; set; }
        public FileContentResult PdfDoc { get; set; }
    }
}