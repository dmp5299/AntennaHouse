using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AntennaHousePdf.Models;
using AntennaHousePdf.Library;
using AntennaHouseBusinessLayer.Library;
using System.IO;
using XfoDotNetCtl;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;
using System.Xml.Xsl;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Xml;
using Ionic.Zip;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using AntennaHouseBusinessLayer.Factories;

namespace AntennaHousePdf.Controllers
{
    public class HomeController : Controller
    {        
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Success = true;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(AntennaPdf pdfParams)
        {
            CreateDocument doc1 = new CreateDocument();
            try
            {
                try
                {
                    GetPdf builder = new GetPdf(pdfParams);
                    
                    if (pdfParams.Project == "CMM" || pdfParams.Project == "Rolls Royce" || pdfParams.XmlFiles.Count == 1)
                    {
                        PdfFile file = builder.createDocument();
                        Response.AddHeader("Content-Disposition", new System.Net.Mime.ContentDisposition("attachment") { FileName = file.FileName }.ToString());
                        Response.ContentType = "application/pdf";
                        return file.PdfDoc;
                    }
                    else
                    {
                        if (pdfParams.SubProjectSB == "Airbus")
                    {
                        System.Web.HttpContext.Current.Session["sbFooter"] = pdfParams.Footer;
                    }
                        using (ZipFile zip = new ZipFile())
                        {
                            UploadGraphicFiles uploadg = new UploadGraphicFiles();
                            UploadXmlFiles uploadx = new UploadXmlFiles();
                            if(pdfParams.Graphics[0] != null)
                                uploadg.uploadFiles(pdfParams.Graphics, "graphicsFolder");
                            uploadx.uploadFiles(pdfParams.XmlFiles, "UserId");
                            string[] filesEntries = Directory.GetFiles(Session["UserId"].ToString());
                            foreach (string fileEntry in filesEntries)
                            {
                                byte[] doc = doc1.SaxonBuild(fileEntry,pdfParams.Project, pdfParams.SubProjectSB);
                                var pdfDoc = doc;
                                string[] xml = fileEntry.Split('\\');
                                string xmlFile1 = xml[xml.Length - 1];
                                zip.AddEntry(xmlFile1.Replace(".xml", ".pdf"), pdfDoc);
                            }
                            var memStream = new MemoryStream();
                            Response.AddHeader("Content-Disposition", "attachment; filename=Sbs.zip");
                            zip.Save(memStream);
                            memStream.Position = 0;
                            return File(memStream, "application/zip");
                        }
                    }

                }
                catch (ArgumentNullException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = ("Illigal argument")
                    };
                    throw new HttpResponseException(resp);
                }
                catch (XfoException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = ("XSL-FO Error")
                    };
                    throw new HttpResponseException(resp);
                }
                catch (XsltException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "XSL Transform Error"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (IOException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "IO Exception"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (InvalidOperationException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Invalid Operation"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (ArgumentException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Illigal Arguement Exception"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (NullReferenceException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message + e.StackTrace),
                        ReasonPhrase = "Null Reference Exception"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (XmlException e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Xml Exception"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (Exception e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Exception"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            catch (HttpResponseException e)
            {
                ModelState.AddModelError("", e.Response.ReasonPhrase.ToString() + ": " + e.Response.Content.ReadAsStringAsync().Result);
                return View(pdfParams);
            }
            finally
            {
                if(Session["graphicFolder"]!=null)
                {
                    Directory.Delete(Session["graphicFolder"].ToString(),true);
                }
                if (Session["UserId"]!= null)
                {
                    Directory.Delete(Session["UserId"].ToString(), true);
                }
                Session.Clear();
            }

        }
    }
}