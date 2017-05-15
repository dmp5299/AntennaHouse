using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntennaHousePdf.Models
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void SaxonBuildTest()
        {
            var dave = "dave";
            var antenna = new AntennaHousePdf.Models.AntennaPdf();
            antenna.Project = "SB";
            var result = antenna.SaxonBuild("c:\\inetpub\\wwwroot\\testing\\DMC-HSENCCNTRL-A-73-23-19-00A06-930A-D_000-00.xml", "Airbus", false);
            Assert.IsNotNull(result);
        }
    }
}