using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AntennaHouseBusinessLayer.Interfaces;

namespace AntennaHouseBusinessLayer.FileUtils
{
    public class UploadSgmlFiles : IUploadFiles
    {
        private List<string> Files { get; set; }

        public UploadSgmlFiles()
        {
            Files = new List<string>();
        }

        public void uploadFiles(List<HttpPostedFileBase> files, string sessionId, Boolean cmm = false)
        {
            System.Web.HttpContext.Current.Session[sessionId] = "C:/inetpub/wwwroot/Sgml/" + string.Format(@"{0}", DateTime.Now.Ticks);
            Directory.CreateDirectory(System.Web.HttpContext.Current.Session[sessionId].ToString());
            foreach (HttpPostedFileBase xFile in files)
            {
                string[] arr = xFile.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                var data1 = new byte[xFile.ContentLength];
                xFile.InputStream.Read(data1, 0, xFile.ContentLength);
                using (var g = new FileStream(System.Web.HttpContext.Current.Session[sessionId] + "/" + graphicFile, FileMode.Create))
                {
                    g.Write(data1, 0, data1.Length);
                }
            }
        }
    }
}