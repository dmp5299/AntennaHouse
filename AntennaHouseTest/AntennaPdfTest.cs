using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AntennaHousePdf;
using System.Web.Mvc;
using AntennaHousePdf.Library;

namespace AntennaHouseTest
{
    [TestClass]
    public class AntennaPdfTest
    {
        [TestMethod]
        public void SaxonBuildTest()
        {
            var antenna = new AntennaHousePdf.Models.AntennaPdf();
            antenna.Project = "SB";
            var result = antenna.SaxonBuild("c:\\inetpub\\wwwroot\\testing\\DMC-HSENCCNTRL-A-73-23-19-00A06-930A-D_000-00.xml", "Airbus", false);
            Assert.IsNotNull(result);
        }
    }
}
