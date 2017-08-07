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
using System.Web.Configuration;
using AntennaHouseBusinessLayer.XmlUtils;

namespace AntennaHouseBusinessLayer.FOUtils
{
    public class CreateDocument
    {
        public static Processor createProcessor()
        {
            Processor processor = new Processor();
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.FileCheck());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.getGraphicPath());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.SbFooter());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.SubProject());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.GetReferences());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.ATAExt());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.UtasTitlePage());
            return processor;
        }
        

        [FileIOPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public static byte[] SaxonBuild(string xml, string project, string subProject = null, Boolean foldout = false)
        {
            var myUniqueFileName = string.Format(@"C:/inetpub/wwwroot/tempPdf/{0}.pdf", DateTime.Now.Ticks);
            byte[] pdf = null;
            Processor processor = createProcessor();
            XsltCompiler compiler = processor.NewXsltCompiler();
            string xmlpath = ConfigurationManager.AppSettings["root"] + ConfigurationManager.AppSettings["tempxml"];
            string xslpath = ConfigurationManager.AppSettings["projectDirectory"] + project + "/" + ((subProject != null) ? subProject + "/" : "") + project + "_" + "master.xsl";
            string psmi = ConfigurationManager.AppSettings["projectDirectory"] + project + "/" + (subProject != null ? subProject + "/" : "") + "psmi.xsl";
            // Create a Processor instance.
            compiler.ErrorList = new ArrayList();
            XfoObj obj = new XfoObj();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.XmlResolver = new MyXmlUrlResolver();
            // Create a transformer for the stylesheet.
            XsltTransformer transformer;
            try
            {
                transformer = compiler.Compile(new Uri(xslpath)).Load();
            }
            catch (Exception e)
            {
                if (compiler.ErrorList.Count > 0)
                {
                    string errors = "";
                    foreach (StaticError error in compiler.ErrorList)
                    {
                        errors += "At line " + error.LineNumber + ": " + error.Message + "\n";
                    }
                    throw new Exception(errors);
                }
                else
                {
                    throw new Exception(e.Message);
                }
            }
            MemoryStream inFo = new MemoryStream();
            // Load the source document
            XdmNode input = processor.NewDocumentBuilder().Build(XmlReader.Create(xml, settings));
            // Set the root node of the source document to be the initial context node
            transformer.InitialContextNode = input;
            transformer.InputXmlResolver = new MyXmlUrlResolver();
            // Create a serializer
            Serializer serializer = new Serializer();
            if (foldout == true)
            {
                serializer.SetOutputFile(System.Web.HttpContext.Current.Session["UserId"] + "/" + "foldout.fo");
            }
            else { serializer.SetOutputStream(inFo); }/*
            serializer.SetOutputFile(System.Web.HttpContext.Current.Session["UserId"] + "/" + "foldout.fo");*/
            transformer.Run(serializer);/*
            MessageBox.Show("done");*/
            if (foldout == true)
            {
                input = processor.NewDocumentBuilder().Build(new Uri(System.Web.HttpContext.Current.Session["UserId"] + "/" + "foldout.fo"));
                transformer = compiler.Compile(new Uri(psmi)).Load();
                transformer.InitialContextNode = input;
                serializer.SetOutputStream(inFo);
                transformer.Run(serializer);
            }
            try
            {
                using (FileStream outFs = File.Open(myUniqueFileName,
            FileMode.Create, FileAccess.ReadWrite))
                {
                    obj.OptionFileURI = "C:\\inetpub\\wwwroot\\config\\config.xml";

                    obj.BaseURI = Path.GetDirectoryName(xml) + "/";

                    obj.Render(inFo, outFs);

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
            finally
            {
                File.Delete(myUniqueFileName);
            }

        }
    }
}