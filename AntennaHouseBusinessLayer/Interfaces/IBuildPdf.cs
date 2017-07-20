using AntennaHouseBusinessLayer.FOUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AntennaHouseBusinessLayer.Interfaces
{
    interface IBuildPdf
    {
        PdfFile buildPdf(string pmFile, string project, string subProject = null, string footer = null);
    }
}
