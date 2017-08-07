using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace AntennaHousePdf.SaxonExtensions
{
    public class SbFooter : ExtensionFunctionDefinition
    {
        public override QName FunctionName
        {
            get { return new QName("http://some.namespace.com", "getSbFooter"); }
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
                return new XdmSequenceType[] { new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_STRING), '?')
               };
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
            return new SbFooterCall();
        }
    }

    public class SbFooterCall : ExtensionFunctionCall
    {
        public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context)
        {
            string footer="";
            footer = System.Web.HttpContext.Current.Session["sbFooter"].ToString();
            XdmAtomicValue result = new XdmAtomicValue(footer);
            return (IXdmEnumerator)result.GetEnumerator();
        }
    }
}