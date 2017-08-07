using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AntennaHouseBusinessLayer.Interfaces;
using AntennaHouseBusinessLayer.FileUtils;

namespace AntennaHousePdf.Factories
{
    public class FileUploadFactory
    {
        public IUploadFiles GetUploadClass(DmType type)
        {
            switch (type)
            {
                case DmType.UploadGraphicFiles:
                    return new UploadGraphicFiles();
                case DmType.UploadXmlFiles:
                    return new UploadXmlFiles();
                default:
                    throw new NotSupportedException();
            }
        }

        public enum DmType
        {
            UploadGraphicFiles,
            UploadXmlFiles
        }
    }
}