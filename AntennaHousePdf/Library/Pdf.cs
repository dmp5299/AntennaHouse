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
using AntennaHousePdf.Models;

namespace AntennaHousePdf.Library
{
    abstract class Pdf
    {
        public abstract byte[] buildPdf(string xml);

        [FileIOPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public byte[] SaxonBuild(string xml, Models.AntennaPdf pdfParams)
        {
            byte[] pdf = null; Processor processor = new Processor();
            XsltCompiler compiler = processor.NewXsltCompiler();
            string xmlpath = ConfigurationManager.AppSettings["root"] + ConfigurationManager.AppSettings["tempxml"];
            string xslpath = ConfigurationManager.AppSettings["projectDirectory"] + pdfParams.Project + "/" + pdfParams.Project + "_" + "master.xsl";
            // Create a Processor instance.
            compiler.ErrorList = new ArrayList();
            XfoObj obj = new XfoObj();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.XmlResolver = new MyXmlUrlResolver();
            // Create a transformer for the stylesheet.
            XsltTransformer transformer = processor.NewXsltCompiler().Compile(new Uri(xslpath)).Load();
            MemoryStream inFo = new MemoryStream();
            // Load the source document
            XdmNode input = processor.NewDocumentBuilder().Build(new Uri(xml));
            // Set the root node of the source document to be the initial context node
            transformer.InitialContextNode = input;
            transformer.InputXmlResolver = new MyXmlUrlResolver();
            // Create a serializer
            Serializer serializer = new Serializer();
            serializer.SetOutputStream(inFo);
            transformer.Run(serializer);
            try
            {
                using (FileStream outFs = File.Open(ConfigurationManager.AppSettings["outputPdf"],
            FileMode.Create, FileAccess.ReadWrite))
                {
                    obj.BaseURI = Path.GetDirectoryName(xml) + "/";

                    obj.Render(inFo, outFs);

                    inFo.Close();

                    MemoryStream ms = new MemoryStream();

                    outFs.Position = 0;

                    outFs.CopyTo(ms);

                    pdf = ms.ToArray();

                    ms.Close();
                }
                return pdf;
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
