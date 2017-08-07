using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AntennaHouseBusinessLayer.Interfaces;
using System.Windows.Forms;
using AntennaHouseBusinessLayer.DataModuleCreation;

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
                case DmType.NumIndex4Point1:
                    return new NumIndexDm4Point1(xmlFolder);
                default:
                    throw new NotSupportedException();
            }
        }

        public enum DmType
        {
            EquipmentDesignator,
            NumIndex,
            NumIndex4Point1
        }
    }
}