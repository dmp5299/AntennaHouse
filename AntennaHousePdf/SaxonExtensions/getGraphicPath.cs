using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using AntennaHousePdf.Controllers;

namespace AntennaHousePdf.SaxonExtensions
{
    public class getGraphicPath : ExtensionFunctionDefinition
    {
        public override QName FunctionName
        {
            get { return new QName("http://some.namespace.com", "getGraphicPath"); }
        }

        public override int MinimumNumberOfArguments
        {
            get { return 1; }
        }

        public override int MaximumNumberOfArguments
        {
            get { return 1; }
        }

        public override XdmSequenceType[] ArgumentTypes
        {
            get
            {
                return new XdmSequenceType[] { new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_STRING), '?') };
            }
        }

        public override XdmSequenceType ResultType(XdmSequenceType[] ArgumentTypes)
        {
            return new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_STRING), '?');
        }

        public override bool TrustResultType
        {
            get { return true; }
        }

        public override ExtensionFunctionCall MakeFunctionCall()
        {
            return new getGraphicPathCall();
        }
    }

    public class getGraphicPathCall : ExtensionFunctionCall
    {

        public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context)
        {
            string graphicFolder = "";
            if (System.Web.HttpContext.Current.Session["graphicFolder"]!=null)
            {
                graphicFolder = System.Web.HttpContext.Current.Session["graphicFolder"].ToString();
            }
            XdmAtomicValue result = new XdmAtomicValue(graphicFolder);

            return (IXdmEnumerator)result.GetEnumerator();
        }
    }
}