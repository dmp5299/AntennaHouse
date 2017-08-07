
using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace AntennaHousePdf.SaxonExtensions
{
    public class FileCheck : ExtensionFunctionDefinition{
        public override QName FunctionName
        {
            get { return new QName("http://some.namespace.com", "fileExists"); }
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
            return new FileCheckCall();
        }
    }

    public class FileCheckCall : ExtensionFunctionCall
    {

        public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context)
        {
            Boolean exists = arguments[0].MoveNext();
            if (exists)
            {
               
                XdmAtomicValue arg = (XdmAtomicValue)arguments[0].Current;
                string val = (string)arg.Value;
                if(System.Web.HttpContext.Current.Session["graphicFolder"]!=null)
                {
                    Boolean graphicExists = System.IO.File.Exists(System.Web.HttpContext.Current.Session["graphicFolder"].ToString() + "/" + val);
                    XdmAtomicValue result = new XdmAtomicValue(graphicExists);
                    return (IXdmEnumerator)result.GetEnumerator();
                }
                return (IXdmEnumerator)new XdmAtomicValue(false).GetEnumerator();
            }
            else
            {
                return EmptyEnumerator.INSTANCE;
            }
        }
    }
}