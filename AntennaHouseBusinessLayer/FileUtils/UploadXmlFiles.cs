using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AntennaHouseBusinessLayer.Interfaces;

namespace AntennaHouseBusinessLayer.Library
{
    public class UploadXmlFiles : IUploadFiles
    {
        private List<string> Files { get; set; }

        public UploadXmlFiles()
        {
            Files = new List<string>();
        }

        public void uploadFiles(List<HttpPostedFileBase> files, string sessionId, Boolean cmm = false)
        {
            System.Web.HttpContext.Current.Session[sessionId] = "C:/inetpub/wwwroot/Xml/" + string.Format(@"{0}", DateTime.Now.Ticks);
            Directory.CreateDirectory(System.Web.HttpContext.Current.Session[sessionId].ToString());
            Boolean pmFound = false;
            foreach (HttpPostedFileBase xFile in files)
            {
                string[] arr = xFile.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                if (cmm)
                {
                    if (graphicFile.Contains("PM") || graphicFile.Contains("pm")) pmFound = true;
                }
                var data1 = new byte[xFile.ContentLength];
                xFile.InputStream.Read(data1, 0, xFile.ContentLength);
                using (var g = new FileStream(System.Web.HttpContext.Current.Session[sessionId] + "/" + graphicFile, FileMode.Create))
                {
                    g.Write(data1, 0, data1.Length);
                }
            }
            if (cmm && !pmFound) throw new FileNotFoundException("PM file not found. A PM file must be included.");
        }
    }
}