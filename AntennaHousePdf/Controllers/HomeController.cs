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
using System.Text;
using System.IO;

namespace AntennaHousePdf.Controllers
{
    public class HomeController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Login(UserProfile user)
        {
            if (ModelState.IsValid)
            {
                using (AntennaHouseEntities db = new AntennaHouseEntities())
                {
                    var obj = db.UserProfiles.Where(a => a.UserName.Equals(user.UserName) && a.Password.Equals(user.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["id"] = obj.UserId.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        Session["FirstName"] = obj.FirstName.ToString();
                        Session["LastName"] = obj.LastName.ToString();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect username/password combination");
                        return View(user);
                    }
                }

            }
            return View(user);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            if (Session["id"] != null) 
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public byte[] processZipDocuments(string fileEntry, AntennaPdf pdfParams, CreateDocument doc1, GetPdf builder)
        {
            try
            {
                if (pdfParams.Project == "53K")
                {

                    Replace replacement = new Replace(Session["UserId"].ToString() + "/" + Path.GetFileName(fileEntry), "<!NOTATION cgm SYSTEM>", "");
                    replacement.replaceContentText();
                    replacement = new Replace(Session["UserId"].ToString() + "/" + Path.GetFileName(fileEntry), "encoding=\"UTF-16\"", "");
                    replacement.replaceContentText();

                    builder.fill53K(Path.GetFileName(fileEntry));

                }
                return doc1.SaxonBuild(fileEntry, pdfParams.Project, pdfParams.Project == "SB" ? pdfParams.SubProjectSB : null);
            }
            catch (XfoException e)
            {
                throw new XfoException(0, 0, fileEntry + " " + e.Message);
            }
            catch (NullReferenceException e)
            {
                throw new NullReferenceException(fileEntry + " " + e.Message);
            }
            catch (XmlException e)
            {
                throw new XmlException(fileEntry + " " + e.Message);
            }
            catch (Exception e)
            {
                throw new XmlException(fileEntry + " " + e.Message);
            }
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
                        
                        PdfFile file = builder.createDocument();/*
                        StringBuilder sb = new StringBuilder();
                        sb.Append(Session["UserName"] + " built pdf at " + DateTime.Now);
                       flush every 20 seconds as you do it
                        
                        System.IO.File.AppendAllText("C:/Users/dpothier/AppData/AntennaHouseLogData/" + "log.txt", sb.ToString());
                                                sb.Clear();*/
                        Response.AddHeader("Content-Disposition", new System.Net.Mime.ContentDisposition("attachment") { FileName = file.FileName.Replace(".XML",".pdf") }.ToString());
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
                            if(pdfParams.Graphics[0] != null) { uploadg.uploadFiles(pdfParams.Graphics, "graphicFolder"); }
                               
                            uploadx.uploadFiles(pdfParams.XmlFiles, "UserId");
                            string[] filesEntries = Directory.GetFiles(Session["UserId"].ToString());
                            foreach (string fileEntry in filesEntries)
                            {
                                byte[] doc = processZipDocuments(fileEntry,pdfParams,doc1, builder);
                                var pdfDoc = doc;
                                string[] xml = fileEntry.Split('\\');
                                string xmlFile1 = xml[xml.Length - 1];
                                xmlFile1 = xmlFile1.Replace(".XML", ".pdf");
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
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "Illigal argument: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = ("Illigal argument")
                    };
                    throw new HttpResponseException(resp);
                }/*
                catch (NullReferenceException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "Null Reference: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = ("Null Reference Exception")
                    };
                    throw new HttpResponseException(resp);
                }*/
                catch (XfoException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "XSL-FO Error: " + e.Message;
                    mail.sendMail(error);
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = ("XSL-FO Error")
                    };
                    throw new HttpResponseException(resp);
                }
                catch (XsltException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "XSL Transform Error: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "XSL Transform Error"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (IOException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "IO Exception: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "IO Exception"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (InvalidOperationException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "Invalid Operation: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Invalid Operation"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (ArgumentException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "Illigal Arguement Exception: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Illigal Arguement Exception"
                    };
                    throw new HttpResponseException(resp);
                }
                catch (XmlException e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "Xml Exception: " + e.Message;
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Xml Exception"
                    };
                    throw new HttpResponseException(resp);
                }/*
                catch (Exception e)
                {
                    Mail mail = new Mail();
                    string error = "Error buliding pdf from " + Session["FirstName"].ToString() + " " + Session["LastName"].ToString() + "<br/><br/>" + "Exception: " + e.Message;
                    mail.sendMail(error);
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(e.Message),
                        ReasonPhrase = "Exception"
                    };
                    throw new HttpResponseException(resp);
                }*/
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
                if (Session["UserId"] != null)
                {
                    Directory.Delete(Session["UserId"].ToString(), true);
                }
            }

        }
    }
}