using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using AntennaHousePdf.Controllers;

namespace AntennaHousePdf.SaxonExtensions
{
    public class TitlePage : ExtensionFunctionDefinition
    {
        public override QName FunctionName
        {
            get { return new QName("http://some.namespace.com", "getTitlePage"); }
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
            return new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_BOOLEAN), '?');
        }

        public override bool TrustResultType
        {
            get { return true; }
        }

        public override ExtensionFunctionCall MakeFunctionCall()
        {
            return new TitlePageCall();
        }
    }

    public class TitlePageCall : ExtensionFunctionCall
    {

        public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context)
        {
            string title = System.Web.HttpContext.Current.Session["titePage"].ToString();
            XdmAtomicValue result = new XdmAtomicValue(title);
            return (IXdmEnumerator)result.GetEnumerator();
        }
    }
}