using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AntennaHousePdf.Models;
using System.Xml;
using System.IO;
using System.Text;
using AntennaHouseBusinessLayer.XmlObjects;
using AntennaHouseBusinessLayer.FileUtils;

namespace AntennaHousePdf.Controllers
{
    public class SgmlConverterController : Controller
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

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(SgmlFile sgml)
        {
            if (Session["id"] != null)
            {
                UploadSgmlFiles uploadSgmlFiles = new UploadSgmlFiles();
                uploadSgmlFiles.uploadFiles(sgml.SgmlFiles, "sgmlPath", false);
                if (sgml.SgmlFiles.Count == 1)
                {
                    string[] arr = sgml.SgmlFiles[0].FileName.Split('\\');
                    string sgmlFile = arr[arr.Length - 1];
                    ConvertedXmlFile doc = SgmlFile.convertToXml(Session["sgmlPath"] + "/" + sgmlFile);
                    Response.AddHeader("Content-Disposition", new System.Net.Mime.ContentDisposition("attachment")
                    { FileName = doc.FileName.Replace(".sgm", ".xml") }.ToString());
                    Response.ContentType = "text/xml";
                    return doc.XmlDoc;
                }
                else
                {
                    string[] filesEntries = Directory.GetFiles(System.Web.HttpContext.Current.Session["sgmlPath"].ToString());
                    var memStream = SgmlFile.buildZipFile(filesEntries);
                    Response.AddHeader("Content-Disposition", "attachment; filename=Xml.zip");
                    return File(memStream, "application/zip");
                }
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

        }
    }
    }