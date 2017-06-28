using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AntennaHouseBusinessLayer.Library;
using AntennaHouseBusinessLayer.Interfaces;
using System.Windows.Forms;

namespace AntennaHouseBusinessLayer.Factories
{
    public class Factory
    {
        public IBuildDm GetDmClass(DmType type, string xmlFolder)
        {
            switch (type)
            {
                case DmType.EquipmentDesignator:
                    return new EquipDesignatorDm(xmlFolder);
                case DmType.NumIndex:
                    return new NumIndexDm(xmlFolder);
                default:
                    throw new NotSupportedException();
            }
        }

        public enum DmType
        {
            EquipmentDesignator,
            NumIndex
        }
    }
}