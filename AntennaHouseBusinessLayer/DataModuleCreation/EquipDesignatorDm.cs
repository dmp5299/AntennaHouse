using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using AntennaHouseBusinessLayer.Interfaces;

namespace AntennaHouseBusinessLayer.DataModuleCreation
{
    public class EquipDesignator
    {
        public string DisCode { get; set; }
        public string RefDesignator { get; set; }
    }

    public class EquipDesignatorDm : IBuildDm
    {
        string Location { get; }
        public List<string> DmFiles { get; }
        List<EquipDesignator> DesignatorList { get; }

        public EquipDesignatorDm(string xmlFolder)
        {
            DmFiles = new List<string>();
            Location = xmlFolder;
            DesignatorList = new List<EquipDesignator>();
            getDmFiles();
        }

        public EquipDesignatorDm()
        {
        }

        public void buildDmFile(string xmlFolder)
        {
            List<XmlNode> nodes = getNodes();

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode dmodule = doc.CreateElement("dmodule");

            XmlNode identAndStatusSection = doc.CreateElement("identAndStatusSection");

            XmlNode dmAddress = doc.CreateElement("dmAddress");

            XmlNode dmIdent = doc.CreateElement("dmIdent");

            //populate dmCode--------------------------
            XmlNode dmCode = doc.CreateElement("dmCode");

            XmlAttribute assyCode = doc.CreateAttribute("assyCode");
            assyCode.InnerText = nodes[0].Attributes["assyCode"].InnerText;
            dmCode.Attributes.Append(assyCode);
            XmlAttribute disassyCode = doc.CreateAttribute("disassyCode");
            disassyCode.InnerText = nodes[0].Attributes["disassyCode"].InnerText;
            dmCode.Attributes.Append(disassyCode);

            XmlAttribute disassyCodeVariant = doc.CreateAttribute("disassyCodeVariant");
            disassyCodeVariant.InnerText = nodes[0].Attributes["disassyCodeVariant"].InnerText;
            dmCode.Attributes.Append(disassyCodeVariant);

            XmlAttribute infoCode = doc.CreateAttribute("infoCode");
            infoCode.InnerText = "014";
            dmCode.Attributes.Append(infoCode);

            XmlAttribute infoCodeVariant = doc.CreateAttribute("infoCodeVariant");
            infoCodeVariant.InnerText = "A";
            dmCode.Attributes.Append(infoCodeVariant);

            XmlAttribute itemLocationCode = doc.CreateAttribute("itemLocationCode");
            itemLocationCode.InnerText = nodes[0].Attributes["itemLocationCode"].InnerText;
            dmCode.Attributes.Append(itemLocationCode);

            XmlAttribute modelIdentCode = doc.CreateAttribute("modelIdentCode");
            modelIdentCode.InnerText = nodes[0].Attributes["modelIdentCode"].InnerText;
            dmCode.Attributes.Append(modelIdentCode);

            XmlAttribute subSubSystemCode = doc.CreateAttribute("subSubSystemCode");
            subSubSystemCode.InnerText = nodes[0].Attributes["subSubSystemCode"].InnerText;
            dmCode.Attributes.Append(subSubSystemCode);

            XmlAttribute subSystemCode = doc.CreateAttribute("subSystemCode");
            subSystemCode.InnerText = nodes[0].Attributes["subSystemCode"].InnerText;
            dmCode.Attributes.Append(subSystemCode);

            XmlAttribute systemCode = doc.CreateAttribute("systemCode");
            systemCode.InnerText = nodes[0].Attributes["systemCode"].InnerText;
            dmCode.Attributes.Append(systemCode);

            XmlAttribute systemDiffCode = doc.CreateAttribute("systemDiffCode");
            systemDiffCode.InnerText = nodes[0].Attributes["systemDiffCode"].InnerText;
            dmCode.Attributes.Append(systemDiffCode);

            dmIdent.AppendChild(dmCode);

            //Language element------------------------------------------------

            XmlNode language = doc.CreateElement("language");

            XmlAttribute countryIsoCode = doc.CreateAttribute("countryIsoCode");
            countryIsoCode.InnerText = "US";
            language.Attributes.Append(countryIsoCode);

            XmlAttribute languageIsoCode = doc.CreateAttribute("languageIsoCode");
            languageIsoCode.InnerText = "sx";
            language.Attributes.Append(languageIsoCode);

            dmIdent.AppendChild(language);

            //issueInfo element------------------------------------------------

            XmlNode issueInfo = doc.CreateElement("issueInfo");

            XmlAttribute inWork = doc.CreateAttribute("inWork");
            inWork.InnerText = "00";
            issueInfo.Attributes.Append(inWork);

            XmlAttribute issueNumber = doc.CreateAttribute("issueNumber");
            issueNumber.InnerText = "000";
            issueInfo.Attributes.Append(issueNumber);

            dmIdent.AppendChild(issueInfo);

            dmAddress.AppendChild(dmIdent);

            //End of dmIdent-----------------------------------------------------

            XmlNode dmAddressItems = doc.CreateElement("dmAddressItems");

            //IssueDate----------------------------------------------------------

            XmlNode issueDate = doc.CreateElement("issueDate");

            XmlAttribute day = doc.CreateAttribute("day");
            day.InnerText = nodes[1].Attributes["day"].InnerText;
            issueDate.Attributes.Append(day);

            XmlAttribute month = doc.CreateAttribute("month");
            month.InnerText = nodes[1].Attributes["month"].InnerText;
            issueDate.Attributes.Append(month);

            XmlAttribute year = doc.CreateAttribute("year");
            year.InnerText = nodes[1].Attributes["year"].InnerText;
            issueDate.Attributes.Append(year);

            dmAddressItems.AppendChild(issueDate);

            //dmTitle-----------------------------------------------------------

            XmlNode dmTitle = doc.CreateElement("dmTitle");

            XmlNode techName = doc.CreateElement("techName");
            techName.InnerText = "Electronic engine control";

            XmlNode infoName = doc.CreateElement("infoName");
            infoName.InnerText = "Alphabetic and alphanumeric index";
            dmTitle.AppendChild(techName);
            dmTitle.AppendChild(infoName);

            dmAddressItems.AppendChild(dmTitle);

            dmAddress.AppendChild(dmAddressItems);

            identAndStatusSection.AppendChild(dmAddress);

            //dmStatus------------------------------------------------------

            XmlNode dmStatus = doc.CreateElement("dmStatus");

            XmlAttribute issueType = doc.CreateAttribute("issueType");
            issueType.InnerText = "new";
            dmStatus.Attributes.Append(issueType);

            XmlNode security = doc.CreateElement("security");

            XmlAttribute securityClassification = doc.CreateAttribute("securityClassification");
            securityClassification.InnerText = "01";
            security.Attributes.Append(securityClassification);

            dmStatus.AppendChild(security);

            XmlNode dataRestrictions = doc.CreateElement("dataRestrictions");
            XmlNode restrictionInstructions = doc.CreateElement("restrictionInstructions");
            XmlNode dataDistribution = doc.CreateElement("dataDistribution");

            restrictionInstructions.AppendChild(dataDistribution);

            XmlNode exportControl = doc.CreateElement("exportControl");
            XmlNode exportRegistrationStmt = doc.CreateElement("exportRegistrationStmt");
            XmlNode simplePara = doc.CreateElement("simplePara");
            exportRegistrationStmt.AppendChild(simplePara);
            exportControl.AppendChild(exportRegistrationStmt);
            restrictionInstructions.AppendChild(exportControl);
            dataRestrictions.AppendChild(restrictionInstructions);
            dmStatus.AppendChild(dataRestrictions);

            XmlNode responsiblePartnerCompany = doc.CreateElement("responsiblePartnerCompany");

            XmlAttribute enterpriseCode = doc.CreateAttribute("enterpriseCode");
            enterpriseCode.InnerText = "";
            responsiblePartnerCompany.Attributes.Append(enterpriseCode);
            dmStatus.AppendChild(responsiblePartnerCompany);

            XmlNode originator = doc.CreateElement("originator");

            XmlAttribute enterpriseCode1 = doc.CreateAttribute("enterpriseCode");
            enterpriseCode1.InnerText = "";
            originator.Attributes.Append(enterpriseCode1);
            dmStatus.AppendChild(originator);

            XmlNode applic = doc.CreateElement("applic");
            XmlNode displayText = doc.CreateElement("displayText");
            XmlNode simplePara1 = doc.CreateElement("simplePara");
            simplePara1.InnerText = "All";
            displayText.AppendChild(simplePara1);
            applic.AppendChild(displayText);
            dmStatus.AppendChild(applic);

            XmlNode brexDmRef = doc.CreateElement("brexDmRef");
            XmlNode dmRef = doc.CreateElement("dmRef");
            XmlAttribute xlink_actuate = doc.CreateAttribute("xlink:actuate");
            xlink_actuate.InnerText = "onRequest";
            dmRef.Attributes.Append(xlink_actuate);
            XmlAttribute xlink_href = doc.CreateAttribute("xlink:href");
            xlink_href.InnerText = "URN:S1000D:DMC-S1000DBIKE-AAA-D00-00-00-00AA-022A-D_005";
            dmRef.Attributes.Append(xlink_href);
            XmlAttribute xlink_show = doc.CreateAttribute("xlink:show");
            xlink_show.InnerText = "replace";
            dmRef.Attributes.Append(xlink_show);
            XmlAttribute xlink_type = doc.CreateAttribute("xlink:type");
            xlink_type.InnerText = "simple";
            dmRef.Attributes.Append(xlink_type);

            XmlNode dmRefIdent = doc.CreateElement("dmRefIdent");

            XmlNode dmCode1 = doc.CreateElement("dmCode");

            XmlAttribute assyCode1 = doc.CreateAttribute("assyCode");
            assyCode1.InnerText = "00";
            dmCode1.Attributes.Append(assyCode1);

            XmlAttribute disassyCode1 = doc.CreateAttribute("disassyCode");
            assyCode1.InnerText = "00";
            dmCode1.Attributes.Append(disassyCode1);

            XmlAttribute disassyCodeVariant1 = doc.CreateAttribute("disassyCodeVariant");
            disassyCodeVariant1.InnerText = "AA";
            dmCode1.Attributes.Append(disassyCodeVariant1);

            XmlAttribute infoCode1 = doc.CreateAttribute("infoCode");
            infoCode1.InnerText = "022";
            dmCode1.Attributes.Append(infoCode1);

            XmlAttribute infoCodeVariant1 = doc.CreateAttribute("infoCodeVariant");
            infoCodeVariant1.InnerText = "A";
            dmCode1.Attributes.Append(infoCodeVariant1);

            XmlAttribute itemLocationCode1 = doc.CreateAttribute("itemLocationCode");
            itemLocationCode1.InnerText = "D";
            dmCode1.Attributes.Append(itemLocationCode1);

            XmlAttribute modelIdentCode1 = doc.CreateAttribute("modelIdentCode");
            modelIdentCode1.InnerText = "S1000DBIKE";
            dmCode1.Attributes.Append(modelIdentCode1);

            XmlAttribute subSubSystemCode1 = doc.CreateAttribute("subSubSystemCode");
            subSubSystemCode1.InnerText = "0";
            dmCode1.Attributes.Append(subSubSystemCode1);

            XmlAttribute subSystemCode1 = doc.CreateAttribute("subSystemCode");
            subSystemCode1.InnerText = "0";
            dmCode1.Attributes.Append(subSystemCode1);

            XmlAttribute systemCode1 = doc.CreateAttribute("systemCode");
            systemCode1.InnerText = "D00";
            dmCode1.Attributes.Append(systemCode1);

            XmlAttribute systemDiffCode1 = doc.CreateAttribute("systemDiffCode");
            systemDiffCode1.InnerText = "AAA";
            dmCode1.Attributes.Append(systemDiffCode1);

            dmRefIdent.AppendChild(dmCode1);

            XmlNode issueInfo1 = doc.CreateElement("issueInfo");
            XmlAttribute inWork1 = doc.CreateAttribute("inWork");
            inWork1.InnerText = "00";
            issueInfo1.Attributes.Append(inWork1);

            XmlAttribute issueNumber1 = doc.CreateAttribute("issueNumber");
            issueNumber1.InnerText = "005";
            issueInfo1.Attributes.Append(issueNumber1);
            dmRefIdent.AppendChild(issueInfo1);

            dmRef.AppendChild(dmRefIdent);
            brexDmRef.AppendChild(dmRef);
            dmStatus.AppendChild(brexDmRef);

            XmlNode qualityAssurance = doc.CreateElement("qualityAssurance");
            XmlNode unverified = doc.CreateElement("unverified");
            qualityAssurance.AppendChild(unverified);
            dmStatus.AppendChild(qualityAssurance);


            identAndStatusSection.AppendChild(dmStatus);

            dmodule.AppendChild(identAndStatusSection);

            XmlNode content = doc.CreateElement("content");

            XmlNode table = doc.CreateElement("table");
            XmlNode title = doc.CreateElement("title");
            title.InnerText = "Equipment Designator";

            table.AppendChild(title);
            XmlNode tgroup = doc.CreateElement("tgroup");
            XmlAttribute cols = doc.CreateAttribute("cols");
            cols.InnerText = "2";
            tgroup.Attributes.Append(cols);
            XmlNode colspec = doc.CreateElement("colspec");
            XmlNode colspec1 = doc.CreateElement("colspec");
            XmlAttribute att1 = doc.CreateAttribute("colwidth");
            att1.InnerText = "1.5in";
            XmlAttribute att2 = doc.CreateAttribute("colwidth");
            att2.InnerText = "1.5in";
            colspec.Attributes.Append(att1);
            colspec1.Attributes.Append(att2);
            XmlAttribute align = doc.CreateAttribute("align");
            align.InnerText = "center";
            tgroup.AppendChild(colspec);
            tgroup.AppendChild(colspec1);

            XmlNode thead = doc.CreateElement("thead");
            XmlNode headrow = doc.CreateElement("row");

            XmlNode headentry = doc.CreateElement("entry");
            headentry.Attributes.Append(align);
            XmlNode headerPara = doc.CreateElement("para");
            headerPara.InnerText = "Disassembly Code-Item Number";
            headentry.AppendChild(headerPara);

            XmlNode headentry1 = doc.CreateElement("entry");
            XmlNode headerPara1 = doc.CreateElement("para");
            XmlAttribute refAlign = doc.CreateAttribute("align");
            refAlign.InnerText = "center";
            headerPara1.InnerText = "Reference Designator"; headentry1.Attributes.Append(refAlign);
            headentry1.AppendChild(headerPara1);
            XmlNode headentry2 = null;
            headrow.AppendChild(headentry1);
            headrow.AppendChild(headentry);
            thead.AppendChild(headrow);

            tgroup.AppendChild(thead);
            XmlNode tbody = doc.CreateElement("tbody");
            List<EquipDesignator> SortedList = DesignatorList.OrderBy(o => o.RefDesignator).ToList();
            foreach (EquipDesignator equip in SortedList)
            {
                XmlAttribute align1 = doc.CreateAttribute("align");
                align1.InnerText = "center";
                XmlNode row = doc.CreateElement("row");
                XmlNode entry = doc.CreateElement("entry");
                entry.Attributes.Append(align1);
                XmlNode para1 = doc.CreateElement("para");
                para1.InnerText = equip.DisCode;
                entry.AppendChild(para1);
                XmlAttribute align2 = doc.CreateAttribute("align");
                align2.InnerText = "center";
                XmlNode entry1 = doc.CreateElement("entry");
                entry1.Attributes.Append(align2);
                XmlNode para = doc.CreateElement("para");
                para.InnerText = equip.RefDesignator;
                entry1.AppendChild(para);
                row.AppendChild(entry1);
                row.AppendChild(entry);
                tbody.AppendChild(row);
            }
            tgroup.AppendChild(tbody);
            table.AppendChild(tgroup);
            content.AppendChild(table);

            dmodule.AppendChild(content);
            string fileName = "DMC-" + modelIdentCode.InnerText + "-" + systemDiffCode.InnerText + "-" + systemCode.InnerText
                + "-" + subSystemCode.InnerText + subSubSystemCode.InnerText + "-" + assyCode.InnerText + "-" + disassyCode.InnerText
                    + disassyCodeVariant.InnerText + "-" + infoCode.InnerText + infoCodeVariant.InnerText + "-" +
                   itemLocationCode.InnerText + ".xml";
            doc.AppendChild(dmodule);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmldecl, root);
            doc.Save(string.Concat(xmlFolder + "/" + fileName));
        }

        public void getDmFiles()
        {
            string[] files = Directory.GetFiles(Location);
            foreach (string file in files)
            {
                if(!file.Contains("PM") && !file.Contains("pm"))
                {
                    DmFiles.Add(file);
                }
            }
        }

        public List<XmlNode> getNodes()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(string.Concat(DmFiles[0]));
            XmlNode dmCode = doc.SelectSingleNode("descendant::dmCode[1]");
            XmlNode issueDate = doc.SelectSingleNode("descendant::issueDate[1]");
            List<XmlNode> list = new List<XmlNode>();
            list.Add(dmCode);
            list.Add(issueDate);
                foreach (string file in DmFiles)
                {
                doc.Load(file);
                XmlNodeList referenceDesignators = doc.SelectNodes("descendant::referenceDesignator");
                if (referenceDesignators.Count > 0)
                {
                    foreach (XmlNode referenceDesignator in referenceDesignators)
                    {
                        XmlNode catalog = referenceDesignator.SelectSingleNode("ancestor::catalogSeqNumber");
                        string[] csns = catalog.Attributes["catalogSeqNumberValue"].InnerText.Split(' ');
                        string csn = csns[csns.Length - 1];
                        string firstPart = csn.Substring(0, 3);
                        string secondPart = csn.Substring(3);
                        EquipDesignator d = new EquipDesignator
                        {
                            DisCode = firstPart + "-" + secondPart,
                            RefDesignator = referenceDesignator.InnerText
                        };
                        DesignatorList.Add(d);
                    }
                }
            }
            return list;
        }
    }
}