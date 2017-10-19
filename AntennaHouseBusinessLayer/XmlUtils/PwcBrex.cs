using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using AntennaHouseBusinessLayer.FileUtils;
using System.Windows.Forms;
using System.Xml.XPath;

namespace AntennaHouseBusinessLayer.XmlUtils
{
    public class PwcBrex
    {
        public static StringBuilder CheckBrex(string[] xmlFiles)
        {
            string brexErrorFile = System.Web.HttpContext.Current.Session["UserId"].ToString() + "/BrexErrors.txt";
            StringBuilder SB = new StringBuilder();
            foreach (string xmlFile in xmlFiles)
            {
                XmlDocument brexModule = LoadXml(System.Configuration.ConfigurationManager.AppSettings["PWC-Brex"]);
                List<string> erros = new List<string>();
                foreach (XmlNode rule in brexModule.SelectNodes("descendant::structureObjectRule"))
                {
                    try
                    {
                        Dictionary<string, string> objectPaths = new Dictionary<string, string>();
                        XPathDocument document = new XPathDocument(xmlFile);
                        XPathNavigator navigator = document.CreateNavigator();
                        if (rule.SelectSingleNode("child::objectValue") == null)
                        {
                            string objectPath = rule.SelectSingleNode("child::objectPath").InnerText;
                            XPathExpression query = navigator.Compile(objectPath);
                            string allowedObjectFlag = "";
                            if (rule.SelectSingleNode("child::objectValue") == null)
                            {
                                allowedObjectFlag = rule.SelectSingleNode("child::objectPath").Attributes["allowedObjectFlag"].InnerText;
                            }
                            bool succeeded;
                            object result = navigator.Evaluate(query);

                            if (result is bool)
                            {
                                // We'll succeed if the result is true.
                                succeeded = (bool)result;
                            }
                            else if (result is double)
                            {
                                // We'll succeed if the result is non-zero.
                                succeeded = ((double)result) != 0d;
                            }
                            else if (result is string)
                            {
                                // We'll succeed if the result is non-empty.
                                succeeded = !String.IsNullOrEmpty((string)result);
                            }
                            else
                            {
                                // We'll succeed if the result is non-empty.
                                XPathNodeIterator iterator = (XPathNodeIterator)result;
                                succeeded = iterator.MoveNext();
                                if (succeeded && (rule.SelectSingleNode("child::objectValue") != null))
                                {
                                    Boolean isValid = checkAttValue(iterator.Current.Value, rule);
                                    if (!isValid)
                                    {
                                        SB.AppendLine("Attribute value " + iterator.Current.Value + " is invalid for " + objectPath);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(allowedObjectFlag))
                            {
                                switch (allowedObjectFlag)
                                {
                                    case "1":
                                        if (succeeded == false)
                                        {
                                            SB.AppendLine(Regex.Replace(rule.SelectSingleNode("child::objectUse").InnerText, @"\s+", " "));
                                        }
                                        break;
                                    case "0":
                                        if (succeeded == true)
                                        {
                                            SB.AppendLine(Regex.Replace(rule.SelectSingleNode("child::objectUse").InnerText, @"\s+", " "));
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                
            }
            return SB;
        }

            public static Boolean checkAttValue(string value, XmlNode rule)
        {
            Boolean valid = false;
            foreach (XmlNode objectValue in rule.SelectNodes("descendant::objectValue"))
            {
                if (value == objectValue.Attributes["valueAllowed"].InnerText)
                {
                    return true;
                }
            }
            return false;
        }

        private static Boolean evaluateOr(Boolean arg1, Boolean arg2)
        {
            if(arg1 || arg2)
            {
                return true;
            }
            return false;
        }

        private static Boolean evaluateAnd(Boolean arg1, Boolean arg2)
        {
            if (arg1 && arg2)
            {
                return true;
            }
            return false;
        }

        private static XmlDocument LoadXml(string xmlFile)
        {
            XmlDocument module = new XmlDocument();
            module.XmlResolver = new MyXmlUrlResolver();
            module.Load(xmlFile);
            return module;
        }

        public static Dictionary<int,string> createConditionalStatementList(List<int> list, string value)
        {
            Dictionary<int, string> conditionalList = new Dictionary<int, string>();
            foreach(int l in list)
            {
                conditionalList.Add(l, value);
            }
            return conditionalList;
        }

        private static Dictionary<string, string> parseXpathString(string xpathString)
        {
            List<int> andIndexes = xpathString.AllIndexesOf(" and ");
            List<int> orIndexes = xpathString.AllIndexesOf(" or ");

            List<string> expressions = new List<string>();
            Dictionary<string, string> expressionsWithOperations = new Dictionary<string, string>();
            Stack brackets = new Stack();
            Stack and = new Stack();
            int stringStart = 0;
            string last = "";
            if (andIndexes.Count > 0 || orIndexes.Count > 0)
            {
                Dictionary<int, string> andConditionalStatements = createConditionalStatementList(andIndexes, "and");
                Dictionary<int, string> orConditionalStatements = createConditionalStatementList(orIndexes, "or");
                try
                {
                    var orderedConditionalStatements = andConditionalStatements.Union(orConditionalStatements).ToDictionary(d => d.Key, v => v.Value).OrderBy(x => x.Key);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }

                for (int i = 0; i < xpathString.Length; i++)
                {
                    if (xpathString[i] == '[')
                    {
                        brackets.Push(xpathString[i]);
                    }
                    else if (xpathString[i] == ']')
                    {
                        brackets.Pop();
                    }
                    else if (andIndexes.Contains(i))
                    {
                        if (brackets.Count == 0)
                        {
                            if (last == " or ")
                            {
                                expressionsWithOperations.Add(xpathString.Substring(stringStart, (i - stringStart)), "and");
                                last = " or ";
                            }
                            else if (last == " and ")
                            {
                                expressionsWithOperations.Add(xpathString.Substring(stringStart, (i - stringStart)), "and");
                            }
                            else
                            {
                                expressionsWithOperations.Add(xpathString.Substring(stringStart, (i - stringStart)), "and");
                                last = " and ";
                            }
                            i += 5;
                            stringStart = i;
                        }
                    }
                    else if (orIndexes.Contains(i))
                    {
                        if (brackets.Count == 0)
                        {
                            if (last == " or ")
                            {
                                expressionsWithOperations.Add(xpathString.Substring(stringStart, (i - stringStart)), "or");

                            }
                            else if (last == " and ")
                            {
                                expressionsWithOperations.Add(xpathString.Substring(stringStart, (i - stringStart)), "or");
                                last = " and ";
                            }
                            else
                            {
                                expressionsWithOperations.Add(xpathString.Substring(stringStart, (i - stringStart)), "or");
                                last = " or ";
                            }
                            i += 4;
                            stringStart = i;
                        }
                    }
                    else if (i + 1 == xpathString.Length - 1)
                    {
                        expressionsWithOperations.Add(xpathString.Substring(stringStart), "");
                    }
                }
            }
            else
            {
                expressionsWithOperations.Add(xpathString, "");
            }
            return expressionsWithOperations;
        }
    }
}
