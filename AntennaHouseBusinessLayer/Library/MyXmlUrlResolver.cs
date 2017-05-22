using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace AntennaHouseBusinessLayer.Library
{
    //Trying to fetch entities internally when encountering external url references, does not work yet
    public class MyXmlUrlResolver : XmlUrlResolver
    {

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == "http" || absoluteUri.AbsolutePath == @"/dtds/XMLISOENT/ISOEntities")
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
                catch (Exception e)
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
}