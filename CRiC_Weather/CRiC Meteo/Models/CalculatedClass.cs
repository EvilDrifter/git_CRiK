using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRiC_Meteo.Models
{
    public class CalculatedValueInMeteoStation
    {
        public string indexWMO_DB;
        public int basseinIndex;
        public double location_Xm, location_Ym;
        public double snowValue;
        public DateTime pointT;

        public static void UpdateXML(List<CalculatedValueInMeteoStation> scv, string fileNameXML)
        {
            WR_Xml.UpdateXMLFile(scv, $"~/XML_snowCalc/{fileNameXML}.xml");
        }
        public static List<CalculatedValueInMeteoStation> ReadXML(string fileNameXML)
        {
            List<CalculatedValueInMeteoStation> scv = new List<CalculatedValueInMeteoStation>();
            WR_Xml.ReadXMLFile(ref scv, $"~/XML_snowCalc/{fileNameXML}.xml");
            return scv;
        }
    }

    public class CalculatedValueInMapPoint
    {
        [XmlAttribute("X=")]
        public double koordMP_X { get; set; }
        [XmlAttribute("Y=")]
        public double koordMP_Y { get; set; }
        [XmlAttribute("SnowValue=")]
        public double snowValue { get; set; }

        public static void UpdateXML(List<CalculatedValueInMapPoint> scv, string fileNameXML)
        {
            WR_Xml.UpdateXMLFile(scv, $"../../XML_snowCalc/{fileNameXML}.xml");
        }
        public static List<CalculatedValueInMapPoint> ReadXML(string fileNameXML)
        {
            List<CalculatedValueInMapPoint> scv = new List<CalculatedValueInMapPoint>();
            WR_Xml.ReadXMLFile(ref scv, $"../../XML_snowCalc/{fileNameXML}.xml");
            return scv;
        }
    }
}
