using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AntennaHouseBusinessLayer.Library
{
    class PmDate
    {
        private XmlDocument pmFile;
        private string file;

        public PmDate(XmlDocument pmFile, string file)
        {
            this.pmFile = pmFile;
            this.file = file;
            pmFile.Load(file);
            setDate();
        }

        public void setDate()
        {
            XmlNode issueDate = pmFile.SelectSingleNode("descendant::issueDate[1]");
            DateTime thisDay = DateTime.Today;
            issueDate.Attributes["day"].InnerText = thisDay.Day.ToString();
            issueDate.Attributes["month"].InnerText = thisDay.Month.ToString();
            issueDate.Attributes["year"].InnerText = thisDay.Year.ToString();
            pmFile.Save(file);
        }
    }
}
