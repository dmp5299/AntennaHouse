using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntennaHouseBusinessLayer.Interfaces;
using AntennaHouseBusinessLayer.FiftyThreeK;

namespace AntennaHouseBusinessLayer.Factories
{
    public class XmlPopulatorFactory
    {
        public IXmlPopulator GetPopulatorClass(ElementType type, string xmlFolder)
        {
            switch (type)
            {
                case ElementType.SupportEquipment:
                    return new SupportEquipmentPopulator(xmlFolder);
                case ElementType.Supplies:
                    return new SuppliesPopulator(xmlFolder);
                case ElementType.Warnings:
                    return new WarningPopulator(xmlFolder, WarningPopulator.Type.Warning);
                case ElementType.Cautions:
                    return new WarningPopulator(xmlFolder,WarningPopulator.Type.Caution);
                default:
                    throw new NotSupportedException();
            }
        }

        public enum ElementType
        {
            SupportEquipment,
            Supplies,
            Warnings,
            Cautions
        }
    }
}
