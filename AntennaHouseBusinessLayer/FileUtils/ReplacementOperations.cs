using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AntennaHousePdf.FileUtils
{
    public static class Replace
    {
        public static void replaceContentText(string fileLocation, string toReplace, string replace)
        {
            fileLocation = fileLocation.Replace(System.Environment.NewLine, "");
            replace = replace.Replace(System.Environment.NewLine, "");
            string str = File.ReadAllText(fileLocation);
            str = str.Replace(toReplace, replace);
            File.WriteAllText(fileLocation, str);
        }
    }
}
