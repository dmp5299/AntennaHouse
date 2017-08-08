using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AntennaHousePdf.Models;
using System.IO;
using XfoDotNetCtl;
using AntennaHouseBusinessLayer.XmlUtils;
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
using AntennaHouseBusinessLayer.FOUtils;
using AntennaHousePdf.FileUtils;
using AntennaHouseBusinessLayer.FileUtils;
using AntennaHouseBusinessLayer.Mail;
using AntennaHouseBusinessLayer.Projects.CMM;
using AntennaHouseBusinessLayer.Projects.PointOne;
using AntennaHouseBusinessLayer.Projects.FiftyThreeK;
using AntennaHouseBusinessLayer.Projects.PWC;
using AntennaHouseBusinessLayer.Projects.SB;
using AntennaHouseBusinessLayer.Projects.Acrolinx;
using AntennaHouseBusinessLayer.Projects.Rolls_Royce;

namespace AntennaHousePdf.Controllers
{
    public class PdfBuilderController : Controller
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
        public string FilleDropDown(string project)
        {
            return AntennaPdf.getSubProjects(project);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(AntennaPdf pdfParams)
        {
            if (Session["id"] != null)
            {
                try
                {
                    try
                    {
                        UploadGraphicFiles uploadGraphicFiles = new UploadGraphicFiles();
                        UploadXmlFiles uploadXmlFiles = new UploadXmlFiles();
                        uploadXmlFiles.uploadFiles(pdfParams.XmlFiles, "UserId", (pdfParams.Project == "CMM"));
                        if (pdfParams.Graphics[0] != null)
                        {
                            uploadGraphicFiles.uploadFiles(pdfParams.Graphics, "graphicFolder");
                        }
                        else
                        {
                            Session.Remove("graphicFolder");
                        }
                        PdfFile file = null;
                        switch (pdfParams.Project)
                        {
                            case "CMM":
                                StringBuilder sb = Brex.CheckBrex(Directory.GetFiles(System.Web.HttpContext.Current.Session["UserId"].ToString()));
                                if (sb.Length > 0)
                                {
                                    return File(Encoding.UTF8.GetBytes(sb.ToString()),
                                    "text/plain", "brexErrors.txt");
                                }
                                else
                                {
                                    file = Utas.buildPdf(XmlOperations.FindPmFile(pdfParams.XmlFiles), pdfParams.Project, pdfParams.UtasTitle, pdfParams.SubProject);
                                }
                                break;
                            case "4.1":
                                file = Pratt.buildPdf(XmlOperations.FindPmFile(pdfParams.XmlFiles), pdfParams.Project, pdfParams.SubProject);
                                break;
                            case "SB":
                                if (pdfParams.XmlFiles.Count > 1)
                                {
                                    string[] filesEntries = Directory.GetFiles(System.Web.HttpContext.Current.Session["UserId"].ToString());
                                    var memStream = UTC.buildZipFile(filesEntries, pdfParams.Project, pdfParams.SubProject, pdfParams.Footer);
                                    Response.AddHeader("Content-Disposition", "attachment; filename=Sbs.zip");
                                    return File(memStream, "application/zip");
                                }
                                else
                                {
                                    string[] arr = pdfParams.XmlFiles[0].FileName.Split('\\');
                                    string xmlFile = arr[arr.Length - 1];
                                    file = UTC.buildPdf(System.Web.HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, pdfParams.Project, pdfParams.SubProject,
                                    pdfParams.Footer);
                                }
                                break;
                            case "PWC":
                                if (pdfParams.XmlFiles.Count > 1)
                                {
                                    string[] filesEntries = Directory.GetFiles(System.Web.HttpContext.Current.Session["UserId"].ToString());
                                    var memStream = PwcSb.buildZipFile(filesEntries, pdfParams.Project, pdfParams.SubProject, pdfParams.Footer);
                                    Response.AddHeader("Content-Disposition", "attachment; filename=Sbs.zip");
                                    return File(memStream, "application/zip");
                                }
                                else
                                {
                                    string[] arr = pdfParams.XmlFiles[0].FileName.Split('\\');
                                    string xmlFile = arr[arr.Length - 1];
                                    file = PwcSb.buildPdf(System.Web.HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, pdfParams.Project, pdfParams.SubProject);
                                }
                                break;
                            case "53K":
                                if (pdfParams.XmlFiles.Count > 1)
                                {
                                    string[] filesEntries = Directory.GetFiles(System.Web.HttpContext.Current.Session["UserId"].ToString());
                                    var memStream = FiftyThreeK.buildZipFile(filesEntries, pdfParams.Project, pdfParams.SubProject);
                                    Response.AddHeader("Content-Disposition", "attachment; filename=Sbs.zip");
                                    return File(memStream, "application/zip");
                                }
                                else
                                {
                                    string[] arr = pdfParams.XmlFiles[0].FileName.Split('\\');
                                    string xmlFile = arr[arr.Length - 1];
                                    file = FiftyThreeK.buildPdf(System.Web.HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, pdfParams.Project, pdfParams.SubProject);
                                }
                                break;
                            case "Rolls Royce":
                                file = RollysRoyce.buildPdf(XmlOperations.FindPmFile(pdfParams.XmlFiles), pdfParams.Project, pdfParams.SubProject);
                                break;

                        }
                        Response.AddHeader("Content-Disposition", new System.Net.Mime.ContentDisposition("attachment")
                        { FileName = file.FileName.Replace(".XML", ".pdf") }.ToString());
                        Response.ContentType = "application/pdf";
                        return file.PdfDoc;
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
                    }
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
                    }
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
                    }
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
                    }
                }
                catch (HttpResponseException e)
                {
                    ModelState.AddModelError("", e.Response.ReasonPhrase.ToString() + ": " + e.Response.Content.ReadAsStringAsync().Result);
                    return View(pdfParams);
                }
                finally
                {
                    if (Session["graphicFolder"] != null)
                    {
                        Directory.Delete(Session["graphicFolder"].ToString(), true);
                    }
                    if (Session["UserId"] != null)
                    {
                        DirectoryInfo di = new DirectoryInfo(Session["UserId"].ToString());
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                        Directory.Delete(Session["UserId"].ToString(), true);
                    }
                }

            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }
    }
}