using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntennaHouseBusinessLayer.Library
{
    class ChangeFileName
    {
        private string file;
        private int index;
        private string replacememt;
        private int length;

        public ChangeFileName(string file, int index, int length, string replacememt)
        {
            this.file = file;
            this.index = index;
            this.replacememt = replacememt;
            this.length = length;
        }

        public string changeNames()
        {
            string f = file.Replace(file.Substring(index, length), replacememt);
            System.IO.File.Move(file, f);
            return f;
        }
    }
}
