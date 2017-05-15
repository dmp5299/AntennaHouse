using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AntennaHousePdf.Library;

namespace AntennaHousePdf.Library
{
    public class Factory
    {
        public IBuildDm GetDmClass(DmType type)
        {
            switch (type)
            {
                case DmType.EquipmentDesignator:
                    return new EquipDesignatorDm();
                case DmType.NumIndex:
                    return new NumIndexDm();
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