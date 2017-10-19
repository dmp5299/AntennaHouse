using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AntennaHouseBusinessLayer.XmlUtils;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;

namespace AntennaHouseBusinessLayer.XmlUtils
{
    public class Brex
    {
        public static StringBuilder CheckBrex(string[] xmlFiles)
        {

            string brexErrorFile = System.Web.HttpContext.Current.Session["UserId"].ToString() + "/BrexErrors.txt";
            StringBuilder SB = new StringBuilder();
            foreach (string xmlFile in xmlFiles)
            {
                XmlDocument dataModule = LoadXml(xmlFile);
                XmlDocument brexModule = LoadXml(System.Configuration.ConfigurationManager.AppSettings["Brex"]);
                List<string> erros = new List<string>();
                foreach (XmlNode rule in brexModule.SelectNodes("descendant::structureObjectRule"))
                {
                    string objectPath = rule.SelectSingleNode("child::objectPath").InnerText;
                    if (rule.SelectSingleNode("child::objectValue") == null)
                    {
                        XmlNode node = dataModule.SelectSingleNode(objectPath);
                        if (node != null)
                        {
                            SB.AppendLine(Regex.Replace(rule.SelectSingleNode("child::objectUse").InnerText + " " + xmlFile, @"\s+", " "));
                        }
                    }
                    else
                    {
                        //get attribute to test
                        string path = rule.SelectSingleNode("child::objectPath").InnerText;
                        string attribute = path.Substring(path.IndexOf('@') + 1);
                        //get all elements with given attribute
                        XmlNodeList attList = dataModule.SelectNodes("//*[@" + attribute + "]");

                        //check if attribute exists in document
                        if (attList != null)
                        {
                            //loop though all nodes with attribute
                            foreach (XmlNode node in attList)
                            {
                                //get attribute value
                                string att = node.Attributes[attribute].InnerText;
                                bool valid = false;
                                //loop through allowed attribute values in brex and see if you can get a match
                                foreach (XmlNode objectValue in rule.SelectNodes("child::objectValue"))
                                {
                                    if (objectValue.Attributes["valueForm"].InnerText == "single")
                                    {
                                        if (att == objectValue.Attributes["valueAllowed"].InnerText)
                                        {
                                            valid = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        //split the attribute value into 2 strings
                                        string[] values = objectValue.Attributes["valueAllowed"].InnerText.Split('~');
                                        //get minimum and maximum range for possible values
                                        string min = Regex.Match(values[0], "[0-9]+").Value;
                                        string max = Regex.Match(values[1], "[0-9]+").Value;
                                        //get starting strng of attribute values to concat later
                                        string start = Regex.Match(values[0], "[a-zA-Z]+").Value;

                                        //loop through range of values and check if attribute value matches up with any in the range.
                                        for (int i = Int32.Parse(min); i <= Int32.Parse(max); i++)
                                        {
                                            if (att == String.Concat(start, i.ToString()))
                                            {
                                                valid = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!valid)
                                {
                                    SB.AppendLine(attribute + " has an invalid attribute value in " + xmlFile);
                                }
                            }
                        }
                    }
                }
            }
            return SB;
        }

        private static XmlDocument LoadXml(string xmlFile)
        {
            XmlDocument module = new XmlDocument();
            module.XmlResolver = new MyXmlUrlResolver();
            module.Load(xmlFile);
            return module;
        }
    }
}
