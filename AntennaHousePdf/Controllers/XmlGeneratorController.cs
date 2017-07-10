using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using XmlOperationsBusinessLayer.XmlObjects;
using System.Windows.Forms;

namespace AntennaHousePdf.Controllers
{
    public class XmlGeneratorController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            if (Session["id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public string XmlData(string gridData)
        {
            string xml = "";
            JavaScriptSerializer json_deserializer = new JavaScriptSerializer();
            List<XmlData> rows = json_deserializer.Deserialize<List<XmlData>>(gridData);
            foreach (XmlData x in rows)
            {
                xml += @"<element1>" + x.ExCol1 + "</element>";
            }
            return xml;
        }
    }
}