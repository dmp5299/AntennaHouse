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
    }
}