using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Web.Mvc;
using AntennaHousePdf.Library;

namespace AntennaHouseTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Mock<HttpSessionStateBase> session = new Mock<HttpSessionStateBase>();
            session.SetupGet(s => s["UserId"]).Returns(string.Format(@"{0}", DateTime.Now.Ticks));
            session.SetupGet(s => s["Last4DigitsOfSsn"]).Returns("9999");


            Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
            httpContext.SetupGet(c => c.Session).Returns(session.Object);


            ControllerContext ctx = new ControllerContext();
            ctx.HttpContext = httpContext.Object;
        }
    }
}
