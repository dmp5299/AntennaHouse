using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Web.Mvc;
using XfoDotNetCtl;
using System.Xml;
using System.Xml.Resolvers;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Security.Permissions;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Configuration;
using Saxon.Api;
using System.Collections;

namespace AntennaHousePdf.Models
{
    public class AntennaPdf
    {    
        public string Footer { get; set;}
        public string UtasTitle { get; set; }
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
            items.Add(new SelectListItem { Text = "Select Project", Value = "Select Project", Selected = selected });
            for (int i = 0; i < projects.Length; i++)
            {
                projects[i] = new DirectoryInfo(projects[i]).Name;
                if (projects[i] != ".git")
                {
                    items.Add(new SelectListItem { Text = projects[i], Value = projects[i], Selected = selected });
                }
                selected = false;
            }
            return items;
        }
        
        public static string getSubProjects(string project)
        {
            string[] projects = Directory.GetDirectories(ConfigurationManager.AppSettings["projectDirectory"] + "/" + project + "/");
            string items = "";
            string selectedItem = "";
            if (HttpContext.Current.Session["subProject"] != null)
            {
                selectedItem = HttpContext.Current.Session["subProject"].ToString();
            }
            for (int i = 0; i < projects.Length; i++)
            {
                projects[i] = new DirectoryInfo(projects[i]).Name;
                if (projects[i] == selectedItem)
                {
                    items += "<option value='" + projects[i] + "' selected>" + projects[i] + "</option>";
                }
                else
                {
                    items += "<option value='" + projects[i] + "'>" + projects[i] + "</option>";
                }
            }
            HttpContext.Current.Session.Remove("subProject");
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

        public static List<SelectListItem> getTitles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "FAA ACCEPTED",
                Value = "true",
                Selected = true
            });
            items.Add(new SelectListItem
            {
                Text = "Not FAA ACCEPTED",
                Value = "false",
                Selected = false
            });
            return items;
        }
    }
}
