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
    public enum Outputs { Pdf, Web }

    //Trying to fetch entities internally when encountering external url references, does not work yet
    class MyXmlUrlResolver : XmlUrlResolver
    {

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == "http" || absoluteUri.AbsolutePath== @"/dtds/XMLISOENT/ISOEntities")
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(ConfigurationManager.AppSettings["entityFile"]);
                return stream;
            }
            else
            {
                try
                {
                    Stream stream = (Stream)base.GetEntity(absoluteUri, role, ofObjectToReturn);
                    return stream;
                }
                catch(Exception e)
                {/*
                    MessageBox.Show(e.Message);*/
                    return null;
                }
            }
        }

        [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return base.ResolveUri(baseUri, relativeUri);
        }
    }

    public class AntennaPdf
    {    
        public string Footer { get; set;}
        public string Xsl { get; set; }
        public string Project { get; set; }
        public string SubProject { get; set; }
        public string SubProjectSB { get; set; }
        public string Volume { get; set; }
        public string PublishMethod { get; set; }
        public HttpPostedFileBase Xml { get; set; }
        public Outputs Output { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<HttpPostedFileBase> Graphics {get;set; }
        public List<HttpPostedFileBase> XmlFiles { get; set; }

        public byte[] Pdf { get; set; }

        [FileIOPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public byte[] SaxonBuild(string xml, string subProject = null, Boolean foldout = false)
        {
            var myUniqueFileName = string.Format(@"C:/inetpub/wwwroot/tempPdf/{0}.pdf", DateTime.Now.Ticks);
            byte[] pdf = null; Processor processor = new Processor();
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.FileCheck());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.getGraphicPath());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.SbFooter());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.TitlePage());
            processor.RegisterExtensionFunction(new AntennaHousePdf.SaxonExtensions.GetReferences());
            XsltCompiler compiler = processor.NewXsltCompiler();
            string xmlpath = ConfigurationManager.AppSettings["root"] + ConfigurationManager.AppSettings["tempxml"];
            string xslpath = ConfigurationManager.AppSettings["projectDirectory"] + this.Project + "/" + ((subProject !=null) ? subProject+"/" : "") + this.Project + "_" + "master.xsl";
            MessageBox.Show(xslpath);
            string psmi = ConfigurationManager.AppSettings["projectDirectory"] + this.Project + "/" + (subProject != null ? subProject + "/" : "") + "psmi.xsl";
            
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
            catch(Exception e)
            {
                if(compiler.ErrorList.Count > 0)
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
            XdmNode input = processor.NewDocumentBuilder().Build(XmlReader.Create(xml,settings));
            // Set the root node of the source document to be the initial context node
            transformer.InitialContextNode = input;
            transformer.InputXmlResolver = new MyXmlUrlResolver();
            // Create a serializer
            Serializer serializer = new Serializer();
            if (foldout==true)
            {
                serializer.SetOutputFile(System.Web.HttpContext.Current.Session["UserId"] + "/" +"foldout.fo");
            }
            else { serializer.SetOutputStream(inFo); }
            transformer.Run(serializer);
            if (foldout==true)
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
