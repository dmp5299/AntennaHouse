using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using XfoDotNetCtl;
using System.Xml;
using System.Xml.Resolvers;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Security.Permissions;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Configuration;
using Saxon.Api;
using System.Collections;

namespace AntennaHousePdf.Models
{
    public class AntennaPdf
    {    
        public string Footer { get; set;}
        public string Project { get; set; }
        public string SubProject { get; set; }
        public string SubProjectSB { get; set; }
        public string Volume { get; set; }
        public HttpPostedFileBase Xml { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<HttpPostedFileBase> Graphics {get;set; }
        public List<HttpPostedFileBase> XmlFiles { get; set; }
        public byte[] Pdf { get; set; }

        //This is an old out of date method, getProjects is now used
        public static List<SelectListItem> getStyleSheets()
        {
            
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["styleSheetDirectory"], "*.xsl");
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
                items.Add(new SelectListItem { Text = files[i], Value = files[i] });
            }
            return items;
        }

        public static List<SelectListItem> getProjects()
        {
            string[] projects = Directory.GetDirectories(ConfigurationManager.AppSettings["projectDirectory"]);
            List<SelectListItem> items = new List<SelectListItem>();
            Boolean selected=false;
            for (int i = 0; i < projects.Length; i++)
            {
                projects[i] = new DirectoryInfo(projects[i]).Name;
                if (projects[i] == "SB") selected = true;
                items.Add(new SelectListItem { Text = projects[i], Value = projects[i], Selected=selected });
                selected = false;
            }
            return items;
        }

        public static List<SelectListItem> getSubProjects(string project)
        {
            string[] projects = Directory.GetDirectories(ConfigurationManager.AppSettings["projectDirectory"]+"/"+ project+"/");
            List<SelectListItem> items = new List<SelectListItem>();
            Boolean selected = false;
            for (int i = 0; i < projects.Length; i++)
            {
                projects[i] = new DirectoryInfo(projects[i]).Name;
                items.Add(new SelectListItem { Text = projects[i], Value = projects[i], Selected = selected });
            }
            return items;
        }

        public static List<SelectListItem> getFooters()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "EAR 9E991",
                Value = "U.S. Export Classification: EAR 9E991"
                + "\nUTC AEROSPACE SYSTEMS PROPRIETARY - Subject to the restriction on the title or cover page.",
                Selected = true
            });
            items.Add(new SelectListItem
            {
                Text = "EAR EAR99",
                Value = "U.S. Export Classification: EAR EAR99"
               + "\nUTC AEROSPACE SYSTEMS PROPRIETARY - Subject to the restriction on the title or cover page.",
                Selected = false
            });
            items.Add(new SelectListItem
            {
                Text = "None",
                Value = "This document does not contain any export controlled technical data"
               + "\nUTC AEROSPACE SYSTEMS PROPRIETARY - Subject to the restriction on the title or cover page.",
                Selected = false
            });
            return items;
        }
    }
}
