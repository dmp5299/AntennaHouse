using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AntennaHousePdf.Interfaces;
using System.IO;

namespace AntennaHousePdf.Library
{
    public class UploadGraphicFiles : IUploadFiles
    {
        private List<string> Files { get; set; }

        public UploadGraphicFiles()
        {
            Files = new List<string>();
        }

        public void uploadFiles(List<HttpPostedFileBase> files, string sessionId, Boolean cmm = false)
        {
            System.Web.HttpContext.Current.Session[sessionId] = "C:/inetpub/wwwroot/Graphics/" + string.Format(@"{0}", DateTime.Now.Ticks);
            Directory.CreateDirectory(System.Web.HttpContext.Current.Session[sessionId].ToString());
            foreach (HttpPostedFileBase graphic in files)
            {
                string[] arr = graphic.FileName.Split('\\');
                string graphicFile = arr[arr.Length - 1];
                Files.Add(graphicFile);
                var data1 = new byte[graphic.ContentLength];
                graphic.InputStream.Read(data1, 0, graphic.ContentLength);
                using (var g = new FileStream(System.Web.HttpContext.Current.Session[sessionId].ToString() + "/" + graphicFile, FileMode.Create))
                {
                    g.Write(data1, 0, data1.Length);
                }
            }
        }
    }
}