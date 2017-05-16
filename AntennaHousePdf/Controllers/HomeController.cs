﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AntennaHousePdf.Models;
using AntennaHousePdf.Library;
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
using AntennaHousePdf.Factories;

namespace AntennaHousePdf.Controllers
{
    public class HomeController : Controller
    {
        private List<string> graphicFiles = new List<string>();
        private List<string> xmlFilesArray = new List<string>();

        
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Success = true;
            return View();
        }

        public void uploadGraphics(List<HttpPostedFileBase> graphics)
        {
            Session["graphicFolder"] = "C:/inetpub/wwwroot/Graphics/" + string.Format(@"{0}", DateTime.Now.Ticks);
            Directory.CreateDirectory(Session["graphicFolder"].ToString());
            foreach (HttpPostedFileBase graphic in graphics)
            {
                string[] arr = graphic.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];

                graphicFiles.Add(graphicFile);

                var data1 = new byte[graphic.ContentLength];
                graphic.InputStream.Read(data1, 0, graphic.ContentLength);
                using (var g = new FileStream(Session["graphicFolder"].ToString()+"/"+graphicFile, FileMode.Create))
                {
                    g.Write(data1, 0, data1.Length);
                }
            }
        }

        public void uploadXml(List<HttpPostedFileBase> xmlFiles, Boolean cmm)
        {
            Session["UserId"] = "C:/inetpub/wwwroot/Xml/" + string.Format(@"{0}", DateTime.Now.Ticks);
            Directory.CreateDirectory(Session["UserId"].ToString());
            Boolean pmFound = false;
            foreach (HttpPostedFileBase xFile in xmlFiles)
            {
                string[] arr = xFile.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                if (cmm)
                {
                    if (graphicFile.Contains("PM") || graphicFile.Contains("pm")) pmFound = true;
                }
                var data1 = new byte[xFile.ContentLength];
                xFile.InputStream.Read(data1, 0, xFile.ContentLength);
                using (var g = new FileStream(Session["UserId"] + "/" + graphicFile, FileMode.Create))
                {
                    g.Write(data1, 0, data1.Length);
                }
            }
            if (cmm && !pmFound) throw new FileNotFoundException("PM file not found. A PM file must be included.");
        }

        public string findPmFile(List<HttpPostedFileBase> xmlFiles)
        {
            foreach (HttpPostedFileBase xFile in xmlFiles)
            {
                string[] arr = xFile.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                xmlFilesArray.Add(graphicFile);
                if (graphicFile.Contains("PM") || graphicFile.Contains("pm")) return Session["UserId"] + "/" + graphicFile;
            }
            throw new FileNotFoundException("PM file not found. A PM file must be included.");
        }

        //Get names and locations of all dmodules and put them in DmFiles array
        public Boolean checkForDm(string pmFile, string dm)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Ignore;
            using (StreamReader stream = new System.IO.StreamReader(pmFile, true))
            {
                using (XmlReader pm = XmlReader.Create(stream, settings))
                {
                    PropertyInfo propertyInfo = pm.GetType().GetProperty("DisableUndeclaredEntityCheck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    propertyInfo.SetValue(pm, true);
                    while (pm.ReadToFollowing("dmCode"))
                    {
                        pm.MoveToAttribute("infoCode");
                        if (pm.Value == dm)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public Boolean checkForElement(string xml, string element)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            using (StreamReader stream = new System.IO.StreamReader(xml, true))
            {
                using (XmlReader pm = XmlReader.Create(stream, settings))
                {
                    PropertyInfo propertyInfo = pm.GetType().GetProperty("DisableUndeclaredEntityCheck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    propertyInfo.SetValue(pm, true);
                    if (pm.ReadToFollowing(element))
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(AntennaPdf pdfParams)
        {
            try
            {
                try
                {
                    uploadXml(pdfParams.XmlFiles, (pdfParams.Project == "CMM"));
                    if (pdfParams.Graphics[0] != null)
                    {
                        uploadGraphics(pdfParams.Graphics);/*
                            Bitmap img = new Bitmap("C:/inetpub/wwwroot/Graphics/ICN-73030-3611531102-001-01.cgm");
                            MessageBox.Show(img.Height.ToString());*/
                    }
                    if (pdfParams.XmlFiles.Count == 1)
                    {
                        string[] arr = pdfParams.XmlFiles[0].FileName.Split('\\');
                        string xmlFile = arr[arr.Length - 1];
                        xmlFilesArray.Add(xmlFile);
                        string subFoler=null;
                        if (pdfParams.Project == "Rolls Royce") { subFoler = pdfParams.Volume; } else if (pdfParams.Project == "CMM") { subFoler = pdfParams.SubProject; }
                        else if (pdfParams.Project == "SB"){
                            subFoler = pdfParams.SubProjectSB;
                            if(pdfParams.SubProjectSB == "Airbus")
                            {
                                Session["sbFooter"] = pdfParams.Footer;
                            }
                        }
                        byte[] doc = pdfParams.SaxonBuild(Session["UserId"].ToString() + "/" + xmlFile, subFoler, 
                        checkForElement(Session["UserId"].ToString() + "/" + xmlFile, "foldout"));
                        var pdfDoc = doc;
                        string[] xml = pdfParams.XmlFiles[0].FileName.Split('\\');
                        string xmlFile1 = xml[xml.Length - 1];
                        xmlFile1 = xmlFile1.Replace(".xml", "");
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + xmlFile1.Replace(".XML", "") + ".pdf");
                        Response.ContentType = "application/pdf";
                        FileContentResult file = new FileContentResult(doc, "application/pdf");
                        return file;
                    }
                    else if (pdfParams.Project == "CMM" || pdfParams.Project == "Rolls Royce")
                    {

                        string pmFile = findPmFile(pdfParams.XmlFiles);
                        if (!checkForElement(pmFile, "dmodule"))
                        {
                            if (pdfParams.Project == "CMM")
                            {
                                Factory factory = new Factory();
                                if (checkForDm(pmFile, "013"))
                                {
                                    
                                    NumIndexDm num = (NumIndexDm)factory.GetDmClass(Factory.DmType.NumIndex);
                                    num.buildDmFile(Session["UserId"].ToString());
                                }
                                if (checkForDm(pmFile, "014"))
                                {
                                    EquipDesignatorDm des = (EquipDesignatorDm)factory.GetDmClass(Factory.DmType.EquipmentDesignator);
                                    des.buildDmFile(Session["UserId"].ToString());
                                }
                            }
                            MergePm merge;
                            merge = new MergePm();
                            merge.PmFile = pmFile;
                            merge.mergeFiles(Session["UserId"].ToString());
                        }
                        else
                        {
                            throw new ArgumentException("Pm is already merged. Don't select multiple XML files.");
                        }
                        byte[] doc = pdfParams.SaxonBuild(pmFile, pdfParams.SubProject, checkForElement(pmFile, "foldout"));
                        string[] xml = pmFile.Split('/');
                        string xmlFile1 = xml[xml.Length - 1];
                        Response.AddHeader("Content-Disposition", new System.Net.Mime.ContentDisposition("attachment") { FileName = xmlFile1.Replace(".xml", ".pdf") }.ToString());
                        Response.ContentType = "application/pdf";
                        FileContentResult file = new FileContentResult(doc, "application/pdf");
                        return file;
                    }
                    else
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            string[] filesEntries = Directory.GetFiles(Session["UserId"].ToString());
                            foreach (string fileEntry in filesEntries)
                            {
                                byte[] doc = pdfParams.SaxonBuild(fileEntry,pdfParams.SubProjectSB);
                                var pdfDoc = doc;
                                string[] xml = fileEntry.Split('\\');
                                string xmlFile1 = xml[xml.Length - 1];
                                xmlFilesArray.Add(xmlFile1);
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
                /*
                System.IO.DirectoryInfo di = new DirectoryInfo("C:/inetpub/wwwroot/Graphics");
                foreach (FileInfo file in di.GetFiles())
                {
                    MessageBox.Show(graphicFiles.Count.ToString());
                    MessageBox.Show(graphicFiles.Contains(file.Name).ToString());
                    if (graphicFiles.Contains(file.Name))
                    {
                        file.Delete();
                    }
                }*/
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