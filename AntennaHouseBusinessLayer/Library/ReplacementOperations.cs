using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AntennaHousePdf.Library
{
    public class Replace
    {
        public string[] fileEntries = null;
        private string fileLocation, toReplace, replace;

        public Replace(string fileLocation, string toReplace, string replace)
        {
            this.fileLocation = fileLocation;
            this.toReplace = toReplace;
            fileLocation = fileLocation.Replace(System.Environment.NewLine, "");
            this.replace = replace;
            replace = replace.Replace(System.Environment.NewLine, "");
        }

        public void checkForBackSlash()
        {
            fileLocation = fileLocation.TrimEnd('\\') + @"\";
        }
        /*
        public void replaceFileText()
        {
            fileEntries = Directory.GetFiles(this.fileLocation);
            foreach (string file in fileEntries)
            {
                if (file.Contains(fileLocation))
                {
                    replaceText(file);
                }
            }
        }
        */
        public void replaceContentText()
        {/*
            fileEntries = Directory.GetFiles(this.fileLocation, "*.xml");
            foreach (string file in fileEntries)
            {
                string str = File.ReadAllText(file);/*
                str = str.Replace(System.Environment.NewLine, "");
                
            }*/
            string str = File.ReadAllText(fileLocation);
            str = str.Replace(toReplace, replace);
            File.WriteAllText(fileLocation, str);
        }

        public void replaceText()
        {
            string fileNew = fileLocation.Replace("<!NOTATION cgm SYSTEM>", "");
            File.Move(fileLocation, fileNew);
        }
    }
}
