﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;
using System.Security.Permissions;
using Saxon.Api;
using System.Configuration;
using XfoDotNetCtl;
using System.Xml;
using System.Collections;
using AntennaHouseBusinessLayer.XmlUtils;
using System.IO;
using AntennaHouseBusinessLayer.Factories;
using AntennaHouseBusinessLayer.DataModuleCreation;
using System.Web;
using AntennaHouseBusinessLayer.FOUtils;
using System.Web.Mvc;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.Projects.CMM
{
    public class Utas
    {
        public static PdfFile buildPdf(string pmFile, string project, string title, Boolean footerDate, string subProject = null)
        {
            System.Web.HttpContext.Current.Session["subProject"] = subProject;
            System.Web.HttpContext.Current.Session["TitleInfo"] = title;
            System.Web.HttpContext.Current.Session["footerDate"] = footerDate;
            Factory factory = new Factory();
            if (XmlOperations.CheckForDm(pmFile, "942"))
            {
                NumIndexDm num = (NumIndexDm)factory.GetDmClass(Factory.DmType.NumIndex, HttpContext.Current.Session["UserId"].ToString());
                num.buildDmFile(HttpContext.Current.Session["UserId"].ToString());
            }
            if (XmlOperations.CheckForDm(pmFile, "014"))
            {
                EquipDesignatorDm des = (EquipDesignatorDm)factory.GetDmClass(Factory.DmType.EquipmentDesignator, HttpContext.Current.Session["UserId"].ToString());
                des.buildDmFile(HttpContext.Current.Session["UserId"].ToString());
            }
            MergePm.mergeFiles(HttpContext.Current.Session["UserId"].ToString(), pmFile);
            byte[] pdfDoc = CreateDocument.SaxonBuild(pmFile, project, subProject, XmlOperations.CheckForElement(pmFile, "foldout"));
            string[] xml = pmFile.Split('/');
            string xmlFile1 = xml[xml.Length - 1];
            xmlFile1 = xmlFile1.Replace(".xml", "");
            xmlFile1 = xmlFile1.Replace(".XML", "") + ".pdf";
            FileContentResult file = new FileContentResult(pdfDoc, "application/pdf");
            return new PdfFile { FileName = xmlFile1, PdfDoc = file };
        }
    }
}
