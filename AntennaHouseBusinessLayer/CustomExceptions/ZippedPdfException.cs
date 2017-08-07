using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntennaHouseBusinessLayer
{
    class ZippedPdfException : System.Exception
    {
        public ZippedPdfException(String message, Exception innerException) : base(message, innerException)

        {

        }
    }
}
