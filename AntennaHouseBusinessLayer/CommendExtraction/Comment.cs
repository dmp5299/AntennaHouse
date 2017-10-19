using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntennaHouseBusinessLayer.CommentExtraction
{
    public class Comment
    {
        public int Item { get; set; }
        public int Page { get; set; }
        public string PdfComment { get; set; }
    }
}
