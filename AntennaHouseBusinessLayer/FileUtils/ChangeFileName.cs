using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntennaHouseBusinessLayer.FileUtils
{
    public static class ChangeFileName
    {
        public static string changeNames(string file, int index, int length, string replacememt)
        {
            string f = file.Replace(file.Substring(index, length), replacememt);
            System.IO.File.Move(file, f);
            return f;
        }
    }
}
