using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace AntennaHousePdf.SaxonExtensions
{

        public class UtasTitlePage : ExtensionFunctionDefinition
        {
            public override QName FunctionName
            {
                get { return new QName("http://some.namespace.com", "getTitleInfo"); }
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
                return new GetTitleInfo();
            }
        }

        public class GetTitleInfo : ExtensionFunctionCall
        {

            public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context)
            {
                string ata = "";
                ata = System.Web.HttpContext.Current.Session["TitleInfo"].ToString();
                XdmAtomicValue result = new XdmAtomicValue(ata);
                return (IXdmEnumerator)result.GetEnumerator();
            }
        }
    }
