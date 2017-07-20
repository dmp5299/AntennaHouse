using AntennaHouseBusinessLayer.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AntennaHouseBusinessLayer.XmlUtils;
using AntennaHousePdf.FileUtils;

namespace AntennaHouseBusinessLayer.FiftyThreeK
{
    public class FiftyThreeKOps
    {
        public static void Fill53K(string xmlFile)
        {
            Replace.replaceContentText(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "<!NOTATION cgm SYSTEM>", "");
            Replace.replaceContentText(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "encoding=\"UTF-16\"", "");
            XmlPopulatorFactory factory = new XmlPopulatorFactory();
            if (XmlOperations.CheckForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "supequi"))
            {
                SupportEquipmentPopulator s = (SupportEquipmentPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.SupportEquipment,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                s.loopElements();
            }
            if (XmlOperations.CheckForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "supply"))
            {
                SuppliesPopulator supply = (SuppliesPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Supplies,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                supply.loopElements();
            }
            if (XmlOperations.CheckForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "warning"))
            {
                WarningPopulator warning = (WarningPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Warnings,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                warning.loopElements();
            }
            if (XmlOperations.CheckForElement(HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile, "caution"))
            {
                WarningPopulator warning = (WarningPopulator)factory.GetPopulatorClass(XmlPopulatorFactory.ElementType.Cautions,
                HttpContext.Current.Session["UserId"].ToString() + "/" + xmlFile);
                warning.loopElements();
            }
        }
    }
}
