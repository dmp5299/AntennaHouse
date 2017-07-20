using AntennaHouseBusinessLayer.FileUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;


namespace AntennaHouseBusinessLayer.XmlUtils
{
    public class XmlOperations
    {
        public static void uploadFiles(List<HttpPostedFileBase> graphics, List<HttpPostedFileBase> xmlFiles, string project)
        {
            UploadGraphicFiles uploadGraphicFiles = new UploadGraphicFiles();
            UploadXmlFiles uploadXmlFiles = new UploadXmlFiles();
            uploadXmlFiles.uploadFiles(xmlFiles, "UserId", (project == "CMM"));
            if (graphics[0] != null)
            {
                uploadGraphicFiles.uploadFiles(graphics, "graphicFolder");
            }
            else
            {
                HttpContext.Current.Session.Remove("graphicFolder");
            }
        }

        //Get names and locations of all dmodules and put them in DmFiles array
        public static Boolean CheckForDm(string pmFile, string dm)
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

        public static Boolean CheckForElement(string xml, string element)
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

        

        public static string FindPmFile(List<HttpPostedFileBase> xmlFiles)
        {
            foreach (HttpPostedFileBase xFile in xmlFiles)
            {
                string[] arr = xFile.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                if (graphicFile.Contains("PM") || graphicFile.Contains("pm")) return HttpContext.Current.Session["UserId"] + "/" + graphicFile;
            }
            throw new FileNotFoundException("PM file not found. A PM file must be included.");
        }
    }
}
