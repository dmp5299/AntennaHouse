using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AntennaHousePdf.Interfaces
{
    public interface IUploadFiles
    { 
        void uploadFiles(List<HttpPostedFileBase> files, string sessionId, Boolean cmm = false);
    }
}
